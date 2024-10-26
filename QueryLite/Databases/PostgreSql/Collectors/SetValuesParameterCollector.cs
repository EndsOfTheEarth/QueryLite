/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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
using Npgsql;
using NpgsqlTypes;
using QueryLite.Utility;
using System;
using System.Data;
using System.Text;

namespace QueryLite.Databases.PostgreSql.Collectors {

    internal class PostgreSqlSetValuesParameterCollector : ISetValuesCollector {

        public PostgreSqlParameters Parameters { get; } = new PostgreSqlParameters(initParams: 1);

        private readonly StringBuilder _sql;
        private readonly StringBuilder? _paramSql;

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;
        private readonly bool _useAlias;

        private int _counter;

        public PostgreSqlSetValuesParameterCollector(StringBuilder sql, StringBuilder? paramSql, IDatabase database, CollectorMode collectorMode, bool useAlias) {

            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;
            _paramSql = paramSql;
            _useAlias = useAlias;
        }

        private PostgreSqlSetValuesParameterCollector AddParameter(IColumn column, NpgsqlDbType dbType, object? value) {

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
                Parameters.ParameterList.Add(new NpgsqlParameter(parameterName: paramName, value) { NpgsqlDbType = dbType });
            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                _counter++;

                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=').Append(paramName);

                if(value == null) {
                    value = DBNull.Value;
                }
                Parameters.ParameterList.Add(new NpgsqlParameter(parameterName: paramName, value) { NpgsqlDbType = dbType });
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        private PostgreSqlSetValuesParameterCollector AddFunction(IColumn column, IFunction function) {

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

                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=').Append(function.GetSql(_database, useAlias: _useAlias, parameters: Parameters));
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Varchar, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Varchar, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Uuid, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Uuid, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Smallint, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Smallint, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Integer, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Integer, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Bigint, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Bigint, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Boolean, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, NpgsqlDbType.Boolean, value?.Value);
        }

        private static NpgsqlDbType GetEnumDbType<ENUM>() where ENUM : notnull, Enum {

            NumericType integerType = EnumHelper.GetNumericType<ENUM>();

            switch(integerType) {
                case NumericType.UShort:
                case NumericType.Short:
                    return NpgsqlDbType.Smallint;
                case NumericType.Int:
                case NumericType.UInt:
                    return NpgsqlDbType.Integer;
                case NumericType.Long:
                case NumericType.ULong:
                    return NpgsqlDbType.Bigint;
                case NumericType.Byte:
                case NumericType.SByte:
                    return NpgsqlDbType.Smallint;
                default:
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
            }
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), EnumHelper.GetEnumAsNumber(value));
        }
        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : struct, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), value != null ? EnumHelper.GetEnumAsNumber(value.Value) : null);
        }

        public ISetValuesCollector Set(Column<string> column, string value) {
            return AddParameter(column, NpgsqlDbType.Varchar, value);
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {
            return AddParameter(column, NpgsqlDbType.Varchar, value);
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return AddParameter(column, NpgsqlDbType.Uuid, value);
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {
            return AddParameter(column, NpgsqlDbType.Uuid, value);
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return AddParameter(column, NpgsqlDbType.Boolean, value);
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {
            return AddParameter(column, NpgsqlDbType.Boolean, value);
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return AddParameter(column, NpgsqlDbType.Boolean, value.Value);
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {
            return AddParameter(column, NpgsqlDbType.Boolean, value?.Value);
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return AddParameter(column, NpgsqlDbType.Numeric, value);
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {
            return AddParameter(column, NpgsqlDbType.Numeric, value);
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return AddParameter(column, NpgsqlDbType.Smallint, value);
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {
            return AddParameter(column, NpgsqlDbType.Smallint, value);
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return AddParameter(column, NpgsqlDbType.Integer, value);
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {
            return AddParameter(column, NpgsqlDbType.Integer, value);
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return AddParameter(column, NpgsqlDbType.Bigint, value);
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {
            return AddParameter(column, NpgsqlDbType.Bigint, value);
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return AddParameter(column, NpgsqlDbType.Real, value);
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {
            return AddParameter(column, NpgsqlDbType.Real, value);
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return AddParameter(column, NpgsqlDbType.Double, value);
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {
            return AddParameter(column, NpgsqlDbType.Double, value);
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return AddParameter(column, NpgsqlDbType.Time, value.ToTimeSpan());
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {
            return AddParameter(column, NpgsqlDbType.Time, value?.ToTimeSpan());
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return AddParameter(column, NpgsqlDbType.Timestamp, value);
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {
            return AddParameter(column, NpgsqlDbType.Timestamp, value);
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return AddParameter(column, NpgsqlDbType.Date, value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            return AddParameter(column, NpgsqlDbType.Date, value?.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return AddParameter(column, NpgsqlDbType.TimestampTz, new DateTimeOffset(value.UtcDateTime));
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {
            return AddParameter(column, NpgsqlDbType.TimestampTz, value != null ? new DateTimeOffset(value.Value.UtcDateTime) : null);
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return AddParameter(column, NpgsqlDbType.Smallint, value);
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {
            return AddParameter(column, NpgsqlDbType.Smallint, value);
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return AddParameter(column, NpgsqlDbType.Bytea, value);
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {
            return AddParameter(column, NpgsqlDbType.Bytea, value);
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

        public ISetValuesCollector SetGuid<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<Guid> {
            return AddParameter(column, NpgsqlDbType.Uuid, value.Value);
        }
        public ISetValuesCollector SetGuid<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<Guid> {
            return AddParameter(column, NpgsqlDbType.Uuid, value?.Value);
        }

        public ISetValuesCollector SetShort<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<short> {
            return AddParameter(column, NpgsqlDbType.Smallint, value.Value);
        }
        public ISetValuesCollector SetShort<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<short> {
            return AddParameter(column, NpgsqlDbType.Smallint, value?.Value);
        }

        public ISetValuesCollector SetInt<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<int> {
            return AddParameter(column, NpgsqlDbType.Integer, value.Value);
        }
        public ISetValuesCollector SetInt<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<int> {
            return AddParameter(column, NpgsqlDbType.Integer, value?.Value);
        }

        public ISetValuesCollector SetLong<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<long> {
            return AddParameter(column, NpgsqlDbType.Bigint, value.Value);
        }
        public ISetValuesCollector SetLong<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<long> {
            return AddParameter(column, NpgsqlDbType.Bigint, value?.Value);
        }

        public ISetValuesCollector SetString<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<string> {
            return AddParameter(column, NpgsqlDbType.Varchar, value.Value);
        }
        public ISetValuesCollector SetString<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<string> {
            return AddParameter(column, NpgsqlDbType.Varchar, value?.Value);
        }

        public ISetValuesCollector SetBool<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<bool> {
            return AddParameter(column, NpgsqlDbType.Boolean, value.Value);
        }
        public ISetValuesCollector SetBool<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<bool> {
            return AddParameter(column, NpgsqlDbType.Boolean, value?.Value);
        }
    }

    internal class PostgreSqlSetValuesCollector : ISetValuesCollector {

        private readonly StringBuilder _sql;
        public StringBuilder? ParamsSql;

        private readonly IDatabase _database;
        private readonly CollectorMode _collectorMode;

        private bool _first = true;

        public PostgreSqlSetValuesCollector(StringBuilder sql, IDatabase database, CollectorMode collectorMode) {

            _sql = sql;
            _database = database;
            _collectorMode = collectorMode;

            if(_collectorMode == CollectorMode.Insert) {
                ParamsSql = new StringBuilder();
            }
        }

        private PostgreSqlSetValuesCollector SetValue(IColumn column, string value) {

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

            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
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

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value));
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

        public ISetValuesCollector SetGuid<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<Guid> {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
        }
        public ISetValuesCollector SetGuid<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<Guid> {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector SetShort<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<short> {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
        }
        public ISetValuesCollector SetShort<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<short> {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector SetInt<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<int> {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
        }
        public ISetValuesCollector SetInt<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<int> {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector SetLong<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<long> {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
        }
        public ISetValuesCollector SetLong<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<long> {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector SetString<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<string> {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
        }
        public ISetValuesCollector SetString<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<string> {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector SetBool<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column, CUSTOM_TYPE value) where CUSTOM_TYPE : struct, IValue<bool> {
            return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value));
        }
        public ISetValuesCollector SetBool<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column, CUSTOM_TYPE? value) where CUSTOM_TYPE : struct, IValue<bool> {

            if(value != null) {
                return SetValue(column, PostgreSqlTypeMappings.ToSqlString(value.Value.Value));
            }
            return SetValue(column, "null");
        }
    }
}