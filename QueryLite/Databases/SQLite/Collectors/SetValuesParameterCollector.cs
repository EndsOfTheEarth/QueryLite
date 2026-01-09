/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
using Microsoft.Data.Sqlite;
using QueryLite.Utility;
using System.Text;

namespace QueryLite.Databases.Sqlite.Collectors {

    internal class SqliteSetValuesParameterCollector : ISetValuesCollector {

        public SqliteParameters Parameters { get; } = new SqliteParameters(initParams: 1);

        private readonly StringBuilder _sql;
        private readonly StringBuilder? _paramSql;

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;
        private readonly bool _useAlias;

        private int _counter;

        public SqliteSetValuesParameterCollector(StringBuilder sql, StringBuilder? paramSql, IDatabase database, CollectorMode collectorMode, bool useAlias) {

            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;
            _paramSql = paramSql;
            _useAlias = useAlias;
        }

        private SqliteSetValuesParameterCollector AddParameter(IColumn column, SqliteType dbType, object? value) {

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

                SqlHelper.AppendEncloseColumnName(_sql, column, EncloseWith.SquareBracket);

                _paramSql!.Append(paramName);

                if(value == null) {
                    value = DBNull.Value;
                }
                Parameters.ParameterList.Add(new SqliteParameter(name: paramName, value) { SqliteType = dbType });
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                _counter++;

                SqlHelper.AppendEncloseColumnName(_sql, column, EncloseWith.SquareBracket);
                _sql.Append('=').Append(paramName);

                if(value == null) {
                    value = DBNull.Value;
                }
                Parameters.ParameterList.Add(new SqliteParameter(name: paramName, value) { SqliteType = dbType });
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        private SqliteSetValuesParameterCollector AddFunction(IColumn column, IFunction function) {

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    _paramSql!.Append(',');
                }

                _counter++;

                SqlHelper.AppendEncloseColumnName(_sql, column, EncloseWith.SquareBracket);
                _paramSql!.Append(function.GetSql(_database, useAlias: _useAlias, parameters: Parameters));
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                _counter++;

                SqlHelper.AppendEncloseColumnName(_sql, column, EncloseWith.SquareBracket);
                _sql.Append('=').Append(function.GetSql(_database, useAlias: _useAlias, parameters: Parameters));
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        private static SqliteType GetEnumDbType<ENUM>() where ENUM : notnull, Enum {

            NumericType integerType = EnumHelper.GetNumericType<ENUM>();

            switch(integerType) {
                case NumericType.UShort:
                case NumericType.Short:
                    return SqliteType.Integer;
                case NumericType.Int:
                case NumericType.UInt:
                    return SqliteType.Integer;
                case NumericType.Long:
                case NumericType.ULong:
                    return SqliteType.Integer;
                case NumericType.Byte:
                case NumericType.SByte:
                    return SqliteType.Integer;
                default:
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
            }
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), EnumHelper.GetEnumAsNumber(value));
        }
        public ISetValuesCollector Set<ENUM>(NColumn<ENUM> column, ENUM? value) where ENUM : struct, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), value != null ? EnumHelper.GetEnumAsNumber(value.Value) : null);
        }

        public ISetValuesCollector Set(Column<string> column, string value) {
            return AddParameter(column, SqliteType.Text, value);
        }

        public ISetValuesCollector Set(NColumn<string> column, string? value) {
            return AddParameter(column, SqliteType.Text, value);
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return AddParameter(column, SqliteType.Blob, value);
        }

        public ISetValuesCollector Set(NColumn<Guid> column, Guid? value) {
            return AddParameter(column, SqliteType.Blob, value);
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return AddParameter(column, SqliteType.Text, value);
        }

        public ISetValuesCollector Set(NColumn<bool> column, bool? value) {
            return AddParameter(column, SqliteType.Text, value);
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return AddParameter(column, SqliteType.Text, value.Value);
        }

        public ISetValuesCollector Set(NColumn<Bit> column, Bit? value) {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return AddParameter(column, SqliteType.Real, value);
        }

        public ISetValuesCollector Set(NColumn<decimal> column, decimal? value) {
            return AddParameter(column, SqliteType.Real, value);
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(NColumn<short> column, short? value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(NColumn<int> column, int? value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(NColumn<long> column, long? value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return AddParameter(column, SqliteType.Real, value);
        }

        public ISetValuesCollector Set(NColumn<float> column, float? value) {
            return AddParameter(column, SqliteType.Real, value);
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return AddParameter(column, SqliteType.Real, value);
        }

        public ISetValuesCollector Set(NColumn<double> column, double? value) {
            return AddParameter(column, SqliteType.Real, value);
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return AddParameter(column, SqliteType.Text, value.ToTimeSpan());
        }

        public ISetValuesCollector Set(NColumn<TimeOnly> column, TimeOnly? value) {
            return AddParameter(column, SqliteType.Text, value?.ToTimeSpan());
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return AddParameter(column, SqliteType.Text, value);
        }

        public ISetValuesCollector Set(NColumn<DateTime> column, DateTime? value) {
            return AddParameter(column, SqliteType.Text, value);
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return AddParameter(column, SqliteType.Text, value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(NColumn<DateOnly> column, DateOnly? value) {
            return AddParameter(column, SqliteType.Text, value?.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return AddParameter(column, SqliteType.Text, new DateTimeOffset(value.UtcDateTime));
        }

        public ISetValuesCollector Set(NColumn<DateTimeOffset> column, DateTimeOffset? value) {
            return AddParameter(column, SqliteType.Text, value != null ? new DateTimeOffset(value.Value.UtcDateTime) : null);
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(NColumn<byte> column, byte? value) {
            return AddParameter(column, SqliteType.Integer, value);
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return AddParameter(column, SqliteType.Blob, value);
        }

        public ISetValuesCollector Set(NColumn<byte[]> column, byte[]? value) {
            return AddParameter(column, SqliteType.Blob, value);
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
            return AddParameter(column, SqliteType.Blob, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Guid> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Blob, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Integer, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, short> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Integer, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Integer, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, int> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Integer, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Integer, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, long> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Integer, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, string> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, bool> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value.Value);
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, new DateTimeOffset(value.Value.UtcDateTime));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value != null ? new DateTimeOffset(value.Value.Value.UtcDateTime) : null);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value.ToTimeSpan());
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value.ToTimeSpan());
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, float> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, double> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Real, value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set(Column<Json> column, Json value) {
            return AddParameter(column, SqliteType.Text, value.Value);
        }
        public ISetValuesCollector Set(NColumn<Json> column, Json? value) {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set(Column<Jsonb> column, Jsonb value) {
            return AddParameter(column, SqliteType.Text, value.Value);
        }
        public ISetValuesCollector Set(NColumn<Jsonb> column, Jsonb? value) {
            return AddParameter(column, SqliteType.Text, value?.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Json> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Json> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Jsonb> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value.Value.Value);
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Jsonb> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            return AddParameter(column, SqliteType.Text, value?.Value.Value);
        }
    }

    internal class SqliteSetValuesCollector : ISetValuesCollector {

        private readonly StringBuilder _sql;
        public StringBuilder? ParamsSql;

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;

        private bool _first = true;

        public SqliteSetValuesCollector(StringBuilder sql, IDatabase database, CollectorMode collectorMode) {

            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;

            if(_collectorMode == CollectorMode.Insert) {
                ParamsSql = new();
            }
        }

        private SqliteSetValuesCollector SetValue(IColumn column, string value) {

            if(_collectorMode == CollectorMode.Insert) {

                if(!_first) {
                    _sql.Append(',');
                    ParamsSql!.Append(',');
                }
                else {
                    _first = false;
                }
                SqlHelper.AppendEncloseColumnName(_sql, column, EncloseWith.SquareBracket);

                ParamsSql!.Append(value);
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(!_first) {
                    _sql.Append(',');
                }
                else {
                    _first = false;
                }
                SqlHelper.AppendEncloseColumnName(_sql, column, EncloseWith.SquareBracket);
                _sql.Append('=').Append(value);
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }
        public ISetValuesCollector Set(Column<string> column, string value) {

            ArgumentNullException.ThrowIfNull(value);

            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<string> column, string? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<Guid> column, Guid? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<bool> column, bool? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<Bit> column, Bit? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<decimal> column, decimal? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<short> column, short? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<int> column, int? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<long> column, long? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<float> column, float? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<double> column, double? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<TimeOnly> column, TimeOnly? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<DateTime> column, DateTime? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<DateOnly> column, DateOnly? value) {
            
            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<DateTimeOffset> column, DateTimeOffset? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<byte> column, byte? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<byte[]> column, byte[]? value) {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return SetValue(column, EnumHelper.GetEnumNumberAsString(value));
        }

        public ISetValuesCollector Set<ENUM>(NColumn<ENUM> column, ENUM? value) where ENUM : struct, Enum {

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
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Guid> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, short> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, int> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, long> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, string> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, bool> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }
        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, decimal> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTime> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTimeOffset> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, TimeOnly> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, float> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, double> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Bit> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Json> column, Json value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<Json> column, Json? value) {
            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Jsonb> column, Jsonb value) {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value));
        }

        public ISetValuesCollector Set(NColumn<Jsonb> column, Jsonb? value) {
            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Json> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Json> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Jsonb> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value));
        }

        public ISetValuesCollector Set<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Jsonb> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {

            if(value != null) {
                return SetValue(column, SqliteTypeMappings.ToSqlStringFunctions.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }
    }
}