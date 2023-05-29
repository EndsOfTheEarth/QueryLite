/*
 * MIT License
 *
 * Copyright (c) 2023 EndsOfTheEarth
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/
using QueryLite.Databases.SqlServer;
using QueryLite.Databases;
using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics.Metrics;
using System.Data.Common;

namespace QueryLite {

    internal class PreparedInsertTemplate<PARAMETERS> : IPreparedInsertSet<PARAMETERS>, IIPreparedInsertReturning<PARAMETERS> {

        public ITable Table { get; }
        public Action<IPreparedSetValuesCollector<PARAMETERS>>? SetValues { get; private set; }
        public List<IField>? ReturningFields;

        public PreparedInsertTemplate(ITable table) {
            Table = table;
        }

        public IIPreparedInsertReturning<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values) {
            SetValues = values;
            return this;
        }

        public IPreparedInsertBuild<RESULT> Returning<RESULT>(Func<IResultRow, RESULT>? returning) {
            return new PreparedInsertTemplate<PARAMETERS, RESULT>(Table, SetValues!, returning);
        }

        public IPreparedInsertQuery<PARAMETERS> Build() {
            
        }
    }

    internal class PreparedInsertTemplate<PARAMETERS, RESULT> : IPreparedInsertBuild<RESULT> {

        public ITable Table { get; }
        public Action<IPreparedSetValuesCollector<PARAMETERS>>? SetValues { get; private set; }
        public Func<IResultRow, RESULT>? Returning { get; }

        public PreparedInsertTemplate(ITable table, Action<IPreparedSetValuesCollector<PARAMETERS>>? setValues, Func<IResultRow, RESULT>? returning) {
            Table = table;
            SetValues = setValues;
            Returning = returning;
        }

        public IPreparedInsertQuery<RESULT> Build() {
            
        }
    }







    internal sealed class SqlServerInsertQueryGenerator {

        public string GetSql<PARAMETERS>(PreparedInsertTemplate<PARAMETERS> template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            sql.Append("INSERT INTO ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);

            if(useParameters == Parameters.On || (useParameters == Parameters.Default && Settings.UseParameters)) {

                StringBuilder paramSql = StringBuilderCache.Acquire();

                SqlServerPreparedSetValuesCollector<PARAMETERS> valuesCollector = new SqlServerPreparedSetValuesCollector<PARAMETERS>(sql, paramSql: paramSql, database, CollectorMode.Insert);

                sql.Append('(');

                template.SetValues!(valuesCollector); //Note: This outputs sql to the sql string builder

                sql.Append(')');

                parameters = valuesCollector.Parameters;

                GetReturningSyntax(template, sql);

                sql.Append(" VALUES(").Append(paramSql).Append(')');

                StringBuilderCache.Release(paramSql);
            }
            else {

                return AddParameter(column, SqlDbType.__, value);
                /*
                SqlServerPreparedSetValuesCollector<PARAMETERS> valuesCollector = new SqlServerPreparedSetValuesCollector<PARAMETERS>(sql, paramSql: null, database, CollectorMode.Insert);

                sql.Append('(');
                template.ValuesCollector!(valuesCollector); //Note: This outputs sql to the sql string builder
                sql.Append(')');

                parameters = null;

                GetReturningSyntax(template, sql);

                sql.Append(" VALUES(").Append(valuesCollector.ParamsSql).Append(')');
                */
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }

        private static void GetReturningSyntax<PARAMETERS>(PreparedInsertTemplate<PARAMETERS> template, StringBuilder sql) {

            if(template.ReturningFields != null && template.ReturningFields.Count > 0) {

                sql.Append(" OUTPUT");

                bool first = true;

                foreach(IColumn column in template.ReturningFields) {

                    if(!first) {
                        sql.Append(',');
                    }
                    else {
                        first = false;
                    }
                    sql.Append(" INSERTED.");
                    SqlServerHelper.AppendColumnName(sql, column);
                }
            }
        }
    }

    public interface IInsertParameter<PARAMETERS> {

        DbParameter CreateParameter(PARAMETERS parameters);
    }

    internal class InsertParameter<PARAMETERS> : IInsertParameter<PARAMETERS> {

        public InsertParameter(string paramName, SqlDbType dbType, Func<PARAMETERS, object> function) {
            ParamName = paramName;
            DbType = dbType;
            Function = function;
        }
        public string ParamName { get; }
        public SqlDbType DbType { get; }
        public Func<PARAMETERS, object> Function { get; }

        public DbParameter CreateParameter(PARAMETERS parameters) {
            return new SqlParameter(parameterName: ParamName, dbType: DbType) {
                Value = Function(parameters) ?? DBNull.Value
            };
        }
    }

    internal class SqlServerPreparedSetValuesCollector<PARAMETERS> : IPreparedSetValuesCollector<PARAMETERS> {

        //public SqlServerParameters Parameters { get; } = new SqlServerParameters(initParams: 1);

        public List<InsertParameter<PARAMETERS>> InsertParameters { get; } = new List<QueryLite.InsertParameter<PARAMETERS>>();

        private StringBuilder _sql;
        private StringBuilder? _paramSql;

        private IDatabase _database;
        private CollectorMode _collectorMode;

        public SqlServerPreparedSetValuesCollector(StringBuilder sql, StringBuilder? paramSql, IDatabase database, CollectorMode mode) {
            _sql = sql;
            _paramSql = paramSql;
            _database = database;
            _collectorMode = mode;
        }

        private IPreparedSetValuesCollector<PARAMETERS> AddParameter(IColumn column, SqlDbType dbType, object func) {
            string paramName;

            int count = InsertParameters.Count;

            if(ParamNameCache.ParamNames.Length < count) {
                paramName = ParamNameCache.ParamNames[count];
            }
            else {
                paramName = $"@{count}";
            }
            
            InsertParameters.Add(new InsertParameter<PARAMETERS>(paramName: paramName, dbType, function: (Func<PARAMETERS, object>)func));

            if(_collectorMode == CollectorMode.Insert) {

                if(count > 0) {
                    _sql.Append(',');
                    _paramSql!.Append(',');
                }

                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);

                _paramSql!.Append(paramName);

                //if(value == null) {
                //    value = DBNull.Value;
                //}
                //Parameters.ParameterList.Add(new SqlParameter(parameterName: paramName, value) { SqlDbType = dbType });
            }
            else if(_collectorMode == CollectorMode.Update) {

                /*
                if(_counter > 0) {
                    _sql.Append(',');
                }

                SqlServerHelper.AppendEnclose(_sql, column.Table.Alias, forceEnclose: false);
                _sql.Append('.');
                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);
                _sql.Append('=').Append(paramName);

                Parameters.ParameterList.Add(new SqlParameter(parameterName: paramName, value) { SqlDbType = dbType });
                */
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<string> column, Func<PARAMETERS, string> value) {
            return AddParameter(column, SqlDbType.NVarChar, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<string> column, Func<PARAMETERS, string?> value) {
            return AddParameter(column, SqlDbType.NVarChar, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<Guid> column, Func<PARAMETERS, Guid> value) {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<Guid> column, Func<PARAMETERS, Guid?> value) {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<bool> column, Func<PARAMETERS, bool> value) {
            return AddParameter(column, SqlDbType.TinyInt, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<bool> column, Func<PARAMETERS, bool?> value) {
            return AddParameter(column, SqlDbType.TinyInt, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<Bit> column, Func<PARAMETERS, Bit> value) {
            return AddParameter(column, SqlDbType.Bit, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<Bit> column, Func<PARAMETERS, Bit?> value) {
            return AddParameter(column, SqlDbType.Bit, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<decimal> column, Func<PARAMETERS, decimal> value) {
            return AddParameter(column, SqlDbType.Decimal, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<decimal> column, Func<PARAMETERS, decimal?> value) {
            return AddParameter(column, SqlDbType.Decimal, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<short> column, Func<PARAMETERS, short> value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<short> column, Func<PARAMETERS, short?> value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<int> column, Func<PARAMETERS, int> value) {
            return AddParameter(column, SqlDbType.Int, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<int> column, Func<PARAMETERS, int?> value) {
            return AddParameter(column, SqlDbType.Int, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<long> column, Func<PARAMETERS, long> value) {
            return AddParameter(column, SqlDbType.BigInt, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<long> column, Func<PARAMETERS, long?> value) {
            return AddParameter(column, SqlDbType.BigInt, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<float> column, Func<PARAMETERS, float> value) {
            return AddParameter(column, SqlDbType.Real, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<float> column, Func<PARAMETERS, float?> value) {
            return AddParameter(column, SqlDbType.Real, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<double> column, Func<PARAMETERS, double> value) {
            return AddParameter(column, SqlDbType.Float, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<double> column, Func<PARAMETERS, double?> value) {
            return AddParameter(column, SqlDbType.Float, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<TimeOnly> column, Func<PARAMETERS, TimeOnly> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<TimeOnly> column, Func<PARAMETERS, TimeOnly?> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateTime> column, Func<PARAMETERS, DateTime> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateTime> column, Func<PARAMETERS, DateTime?> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateOnly> column, Func<PARAMETERS, DateOnly> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateOnly> column, Func<PARAMETERS, DateOnly?> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateTimeOffset> column, Func<PARAMETERS, DateTimeOffset> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateTimeOffset> column, Func<PARAMETERS, DateTimeOffset?> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<byte> column, Func<PARAMETERS, byte> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<byte> column, Func<PARAMETERS, byte?> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<byte[]> column, Func<PARAMETERS, byte[]> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<byte[]> column, Func<PARAMETERS, byte[]?> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<ENUM>(Column<ENUM> column, Func<PARAMETERS, ENUM> value) where ENUM : notnull, Enum {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<ENUM>(NullableColumn<ENUM> column, Func<PARAMETERS, ENUM?> value) where ENUM : notnull, Enum {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<StringKey<TYPE>> column, Func<PARAMETERS, StringKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<StringKey<TYPE>> column, Func<PARAMETERS, StringKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<GuidKey<TYPE>> column, Func<PARAMETERS, GuidKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, Func<PARAMETERS, GuidKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<ShortKey<TYPE>> column, Func<PARAMETERS, ShortKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, Func<PARAMETERS, ShortKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<IntKey<TYPE>> column, Func<PARAMETERS, IntKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<IntKey<TYPE>> column, Func<PARAMETERS, IntKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<LongKey<TYPE>> column, Func<PARAMETERS, LongKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<LongKey<TYPE>> column, Func<PARAMETERS, LongKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<BoolValue<TYPE>> column, Func<PARAMETERS, BoolValue<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, Func<PARAMETERS, BoolValue<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<string> column, AFunction<string> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<Guid> column, AFunction<Guid> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<bool> column, AFunction<bool> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<Bit> column, AFunction<Bit> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<decimal> column, AFunction<decimal> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<short> column, AFunction<short> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<int> column, AFunction<int> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<long> column, AFunction<long> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<float> column, AFunction<float> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<double> column, AFunction<double> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<byte> column, AFunction<byte> value) {
            return AddParameter(column, SqlDbType.__, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return AddParameter(column, SqlDbType.__, value);
        }
    }
}