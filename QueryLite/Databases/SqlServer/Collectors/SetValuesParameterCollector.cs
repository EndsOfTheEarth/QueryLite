/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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
using Microsoft.Data.SqlClient;
using QueryLite.Utility;
using System.Data;
using System.Text;

namespace QueryLite.Databases.SqlServer.Collectors {

    internal sealed class SqlServerSetValuesParameterCollector : ISetValuesCollector {

        public SqlServerParameters Parameters { get; } = new SqlServerParameters(initParams: 1);

        private readonly StringBuilder _sql;
        private readonly StringBuilder? _paramSql;

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;
        private readonly bool _useAlias;

        private int _counter;

        public SqlServerSetValuesParameterCollector(StringBuilder sql, StringBuilder? paramSql, IDatabase database, CollectorMode collectorMode, bool useAlias) {
            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;
            _paramSql = paramSql;
            _useAlias = useAlias;
        }

        private SqlServerSetValuesParameterCollector AddParameter(IColumn column, SqlDbType dbType, object? value) {

            string paramName;

            if(_counter < ParamNameCache.ParamNames.Length) {
                paramName = ParamNameCache.ParamNames[_counter];
            }
            else {
                paramName = $"@{_counter}";
            }

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    _paramSql!.Append(',');
                }

                _counter++;

                SqlHelper.AppendEncloseColumnName(_sql, column);

                _paramSql!.Append(paramName);

                if(value == null) {
                    value = DBNull.Value;
                }
                Parameters.ParameterList.Add(new SqlParameter(parameterName: paramName, value) { SqlDbType = dbType });
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                _counter++;

                if(_useAlias) {
                    SqlHelper.AppendEncloseAlias(_sql, column.Table.Alias);
                    _sql.Append('.');
                }
                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=').Append(paramName);

                if(value == null) {
                    value = DBNull.Value;
                }
                Parameters.ParameterList.Add(new SqlParameter(parameterName: paramName, value) { SqlDbType = dbType });
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        private SqlServerSetValuesParameterCollector AddFunction(IColumn column, IFunction function) {

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    _paramSql!.Append(',');
                }

                _counter++;

                SqlHelper.AppendEncloseColumnName(_sql, column);
                _paramSql!.Append(function.GetSql(_database, useAlias: _useAlias, parameters: Parameters));
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                _counter++;

                if(_useAlias) {
                    SqlHelper.AppendEncloseAlias(_sql, column.Table.Alias);
                    _sql.Append('.');
                }
                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=').Append(function.GetSql(_database, useAlias: _useAlias, parameters: Parameters));
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        private static SqlDbType GetEnumDbType<ENUM>() where ENUM : notnull, Enum {

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

        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : struct, Enum {
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
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<Guid> column, AFunction<Guid> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<bool> column, AFunction<bool> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<Bit> column, AFunction<Bit> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<decimal> column, AFunction<decimal> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<short> column, AFunction<short> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<int> column, AFunction<int> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<long> column, AFunction<long> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<float> column, AFunction<float> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<double> column, AFunction<double> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<byte> column, AFunction<byte> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set(AColumn<IGeography> column, AFunction<IGeography> value) {
            return AddFunction(column, value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, Guid> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.SmallInt, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, short> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.SmallInt, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Int, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, int> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Int, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.BigInt, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, long> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.BigInt, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.NVarChar, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, string> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.NVarChar, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Bit, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, bool> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Bit, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Decimal , value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Decimal, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.DateTime, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.DateTime, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.DateTimeOffset, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.DateTimeOffset, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {

            return AddParameter(column, SqlDbType.Date, value.Value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Date, value?.Value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Time, value.Value.ToTimeSpan());
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Time, value?.Value.ToTimeSpan());
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Real, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, float> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Real, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Float, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, double> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Float, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Bit, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return AddParameter(column, SqlDbType.Bit, value?.Value);
        }
    }

    internal sealed class SqlServerSetValuesCollector : ISetValuesCollector {

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;
        private readonly bool _useAlias;

        public SqlServerSetValuesCollector(StringBuilder sql, IDatabase database, CollectorMode collectorMode, bool useAlias) {
            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;

            if(_collectorMode == CollectorMode.Insert) {
                ParamsSql = new StringBuilder();
            }
            _useAlias = useAlias;
        }

        private readonly StringBuilder _sql;
        public StringBuilder? ParamsSql;

        private bool _first = true;

        private SqlServerSetValuesCollector SetValue(IColumn column, string value) {

            if(_collectorMode == CollectorMode.Insert) {

                if(!_first) {
                    _sql.Append(',');
                    ParamsSql!.Append(',');
                }
                else {
                    _first = false;
                }
                SqlHelper.AppendEncloseColumnName(_sql, column);
                ParamsSql!.Append(value);
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(!_first) {
                    _sql.Append(',');
                }
                else {
                    _first = false;
                }
                if(_useAlias) {
                    SqlHelper.AppendEncloseAlias(_sql, column.Table.Alias);
                    _sql.Append('.');
                }
                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=').Append(value);
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }
        public ISetValuesCollector Set(Column<string> column, string value) {

            ArgumentNullException.ThrowIfNull(value);

            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return SetValue(column, EnumHelper.GetEnumNumberAsString(value));
        }

        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : struct, Enum {

            if(value != null) {
                return SetValue(column, EnumHelper.GetEnumNumberAsString(value.Value));
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

        public ISetValuesCollector Set(AColumn<IGeography> column, AFunction<IGeography> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, Guid> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, short> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, int> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, long> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, string> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, bool> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, float> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, double> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqlServerSqlTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }
    }
}