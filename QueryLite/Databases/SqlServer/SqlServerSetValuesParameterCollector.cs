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
using QueryLite.PreparedQuery;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerSetValuesParameterCollector : ISetValuesCollector {

        public SqlServerParameters Parameters { get; } = new SqlServerParameters(initParams: 1);

        private readonly StringBuilder _sql;
        public StringBuilder? ParamSql;

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;

        private int _counter;

        public SqlServerSetValuesParameterCollector(StringBuilder sql, IDatabase database, CollectorMode collectorMode) {
            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;

            if(_collectorMode == CollectorMode.Insert) {
                ParamSql = new StringBuilder();
            }
        }

        private ISetValuesCollector AddParameter(IColumn column, SqlDbType dbType, object? value) {

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    ParamSql!.Append(',');
                }

                string paramName = $"@{_counter++}";

                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);

                ParamSql!.Append(paramName);

                if(value == null) {
                    value = DBNull.Value;
                }
                Parameters.ParameterList.Add(new SqlParameter(parameterName: paramName, value) { SqlDbType = dbType });
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                string paramName = $"@{_counter++}";

                SqlServerHelper.AppendEnclose(_sql, column.Table.Alias, forceEnclose: false);
                _sql.Append('.');
                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);
                _sql.Append('=').Append(paramName);

                Parameters.ParameterList.Add(new SqlParameter(parameterName: paramName, value) { SqlDbType = dbType });
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        private ISetValuesCollector AddFunction(IColumn column, SqlDbType dbType, IFunction function) {

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    ParamSql!.Append(',');
                }
                _counter++;
                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);
                ParamSql!.Append(function.GetSql(_database, useAlias: true, parameters: Parameters));
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }
                _counter++;

                SqlServerHelper.AppendEnclose(_sql, column.Table.Alias, forceEnclose: false);
                _sql.Append('.');
                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);
                _sql.Append('=').Append(function.GetSql(_database, useAlias: true, parameters: Parameters));
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.NVarChar, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.NVarChar, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.SmallInt, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.SmallInt, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Int, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Int, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.BigInt, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.BigInt, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Bit, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Bit, value?.Value);
        }

        private SqlDbType GetEnumDbType<ENUM>() where ENUM : notnull, Enum {

            NumericType integerType = EnumHelper.GetNumericType<ENUM>();

            switch(integerType) {
                case NumericType.UShort:
                case NumericType.Short:
                    return SqlDbType.SmallInt;
                case NumericType.Int:
                case NumericType.UInt:
                    return SqlDbType.Int;
                case NumericType.Long:
                case NumericType.ULong:
                    return SqlDbType.BigInt;
                case NumericType.Byte:
                case NumericType.SByte:
                    return SqlDbType.TinyInt;
                default:
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
            }
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), value);
        }

        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : notnull, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), value);
        }

        public ISetValuesCollector Set(Column<string> column, string value) {
            return AddParameter(column, SqlDbType.NVarChar, value);
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {
            return AddParameter(column, SqlDbType.NVarChar, value);
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value);
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value);
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return AddParameter(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {
            return AddParameter(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return AddParameter(column, SqlDbType.Bit, value.Value);
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {
            return AddParameter(column, SqlDbType.Bit, value?.Value);
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return AddParameter(column, SqlDbType.Decimal, value);
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {
            return AddParameter(column, SqlDbType.Decimal, value);
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return AddParameter(column, SqlDbType.Int, value);
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {
            return AddParameter(column, SqlDbType.Int, value);
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return AddParameter(column, SqlDbType.BigInt, value);
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {
            return AddParameter(column, SqlDbType.BigInt, value);
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return AddParameter(column, SqlDbType.Real, value);
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {
            return AddParameter(column, SqlDbType.Real, value);
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return AddParameter(column, SqlDbType.Float, value);
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {
            return AddParameter(column, SqlDbType.Float, value);
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return AddParameter(column, SqlDbType.Time, value.ToTimeSpan());
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {
            return AddParameter(column, SqlDbType.Time, value?.ToTimeSpan());
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return AddParameter(column, SqlDbType.DateTime, value);
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {
            return AddParameter(column, SqlDbType.DateTime, value);
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return AddParameter(column, SqlDbType.Date, value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            return AddParameter(column, SqlDbType.Date, value?.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return AddParameter(column, SqlDbType.DateTimeOffset, value);
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {
            return AddParameter(column, SqlDbType.DateTimeOffset, value);
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return AddParameter(column, SqlDbType.Binary, value);
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {
            return AddParameter(column, SqlDbType.Binary, value);
        }

        public ISetValuesCollector Set(AColumn<string> column, AFunction<string> value) {
            return AddFunction(column, SqlDbType.NVarChar, value);
        }

        public ISetValuesCollector Set(AColumn<Guid> column, AFunction<Guid> value) {
            return AddFunction(column, SqlDbType.UniqueIdentifier, value);
        }

        public ISetValuesCollector Set(AColumn<bool> column, AFunction<bool> value) {
            return AddFunction(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(AColumn<Bit> column, AFunction<Bit> value) {
            return AddFunction(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(AColumn<decimal> column, AFunction<decimal> value) {
            return AddFunction(column, SqlDbType.Decimal, value);
        }

        public ISetValuesCollector Set(AColumn<short> column, AFunction<short> value) {
            return AddFunction(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(AColumn<int> column, AFunction<int> value) {
            return AddFunction(column, SqlDbType.Int, value);
        }

        public ISetValuesCollector Set(AColumn<long> column, AFunction<long> value) {
            return AddFunction(column, SqlDbType.BigInt, value);
        }

        public ISetValuesCollector Set(AColumn<float> column, AFunction<float> value) {
            return AddFunction(column, SqlDbType.Real, value);
        }

        public ISetValuesCollector Set(AColumn<double> column, AFunction<double> value) {
            return AddFunction(column, SqlDbType.Float, value);
        }

        public ISetValuesCollector Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return AddFunction(column, SqlDbType.Time, value);
        }

        public ISetValuesCollector Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return AddFunction(column, SqlDbType.DateTime, value);
        }

        public ISetValuesCollector Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return AddFunction(column, SqlDbType.Date, value);
        }

        public ISetValuesCollector Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return AddFunction(column, SqlDbType.DateTimeOffset, value);
        }

        public ISetValuesCollector Set(AColumn<byte> column, AFunction<byte> value) {
            return AddFunction(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return AddFunction(column, SqlDbType.Binary, value);
        }
    }

    internal sealed class SqlServerSetValuesCollector : ISetValuesCollector {

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;

        public SqlServerSetValuesCollector(StringBuilder sql, IDatabase database, CollectorMode collectorMode) {
            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;

            if(_collectorMode == CollectorMode.Insert) {
                ParamsSql = new StringBuilder();
            }
        }

        private readonly StringBuilder _sql;
        public StringBuilder? ParamsSql;

        private bool _first = true;

        private ISetValuesCollector SetValue(IColumn column, string value) {

            if(_collectorMode == CollectorMode.Insert) {

                if(!_first) {
                    _sql.Append(',');
                    ParamsSql!.Append(',');
                }
                else {
                    _first = false;
                }
                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);
                ParamsSql!.Append(value);
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(!_first) {
                    _sql.Append(',');
                }
                else {
                    _first = false;
                }
                SqlServerHelper.AppendEnclose(_sql, column.Table.Alias, forceEnclose: false);
                _sql.Append('.');
                SqlServerHelper.AppendEnclose(_sql, column.ColumnName, forceEnclose: false);
                _sql.Append('=').Append(value);
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }
        public ISetValuesCollector Set(Column<string> column, string value) {

            ArgumentNullException.ThrowIfNull(value);

            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return SetValue(column, ((int)(object)value).ToString());   //TODO: Find way to convert enum to integer without allocating an object on the heap 
        }

        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : notnull, Enum {

            if(value != null) {
                return SetValue(column, ((int)(object)value).ToString());   //TODO: Find way to convert enum to integer without allocating an object on the heap 
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(AColumn<string> column, AFunction<string> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<Guid> column, AFunction<Guid> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<bool> column, AFunction<bool> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<Bit> column, AFunction<Bit> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<decimal> column, AFunction<decimal> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<short> column, AFunction<short> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<int> column, AFunction<int> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<long> column, AFunction<long> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<float> column, AFunction<float> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<double> column, AFunction<double> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<byte> column, AFunction<byte> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }
    }
}