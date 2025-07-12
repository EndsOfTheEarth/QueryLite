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
using NpgsqlTypes;
using QueryLite.Utility;
using System;
using System.Collections.Generic;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace QueryLite.Databases.PostgreSql {

    public static class PostgreSqlTypeMappings {

#if NET9_0_OR_GREATER
        private static readonly Lock _lock = new Lock();
#else
        private static readonly object _lock = new object();
#endif
        private static readonly Dictionary<Type, NpgsqlDbType> DbTypeLookup = new Dictionary<Type, NpgsqlDbType>() {
            { typeof(Guid), NpgsqlDbType.Uuid },
            { typeof(Guid?), NpgsqlDbType.Uuid },
            { typeof(bool), NpgsqlDbType.Boolean },
            { typeof(bool?), NpgsqlDbType.Boolean },
            { typeof(Bit), NpgsqlDbType.Boolean },
            { typeof(Bit?), NpgsqlDbType.Boolean },
            { typeof(byte[]), NpgsqlDbType.Bytea },
            { typeof(byte?[]), NpgsqlDbType.Bytea },
            { typeof(byte), NpgsqlDbType.Smallint },
            { typeof(DateTimeOffset), NpgsqlDbType.TimestampTz },
            { typeof(DateTimeOffset?), NpgsqlDbType.TimestampTz },
            { typeof(DateTime), NpgsqlDbType.Timestamp },
            { typeof(DateTime?), NpgsqlDbType.Timestamp },
            { typeof(TimeOnly), NpgsqlDbType.Time },
            { typeof(TimeOnly?), NpgsqlDbType.Time },
            { typeof(DateOnly), NpgsqlDbType.Date },
            { typeof(DateOnly?), NpgsqlDbType.Date },
            { typeof(decimal), NpgsqlDbType.Numeric },
            { typeof(decimal?), NpgsqlDbType.Numeric },
            { typeof(double), NpgsqlDbType.Double },
            { typeof(double?), NpgsqlDbType.Double },
            { typeof(float), NpgsqlDbType.Real },
            { typeof(float?), NpgsqlDbType.Real },
            { typeof(short), NpgsqlDbType.Smallint },
            { typeof(short?), NpgsqlDbType.Smallint },
            { typeof(int), NpgsqlDbType.Integer },
            { typeof(int?), NpgsqlDbType.Integer },
            { typeof(long), NpgsqlDbType.Bigint },
            { typeof(long?), NpgsqlDbType.Bigint },
            { typeof(string), NpgsqlDbType.Varchar }
        };

        private static NpgsqlDbType AddDbType(Type type, NpgsqlDbType dbType) {

            lock(_lock) {
                DbTypeLookup.TryAdd(type, dbType);
            }
            return dbType;
        }

        public static NpgsqlDbType GetNpgsqlDbType(Type type) {

            if(DbTypeLookup.TryGetValue(type, out NpgsqlDbType dbType)) {
                return dbType;
            }

            /*
             *  Map Key Types
             */
            if(type.IsAssignableTo(typeof(IGuidType))) {
                return AddDbType(type, NpgsqlDbType.Uuid);
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return AddDbType(type, NpgsqlDbType.Varchar);
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return AddDbType(type, NpgsqlDbType.Smallint);
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return AddDbType(type, NpgsqlDbType.Integer);
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return AddDbType(type, NpgsqlDbType.Bigint);
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return AddDbType(type, NpgsqlDbType.Boolean);
            }

            /*
             *  Map Custom Types
             */
            if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                return AddDbType(type, NpgsqlDbType.Uuid);
            }
            if(type.IsAssignableTo(typeof(IValue<short>))) {
                return AddDbType(type, NpgsqlDbType.Smallint);
            }
            if(type.IsAssignableTo(typeof(IValue<int>))) {
                return AddDbType(type, NpgsqlDbType.Integer);
            }
            if(type.IsAssignableTo(typeof(IValue<long>))) {
                return AddDbType(type, NpgsqlDbType.Bigint);
            }
            if(type.IsAssignableTo(typeof(IValue<string>))) {
                return AddDbType(type, NpgsqlDbType.Varchar);
            }
            if(type.IsAssignableTo(typeof(IValue<bool>))) {
                return AddDbType(type, NpgsqlDbType.Boolean);
            }
            if(type.IsAssignableTo(typeof(IValue<decimal>))) {
                return AddDbType(type, NpgsqlDbType.Numeric);
            }
            if(type.IsAssignableTo(typeof(IValue<DateTime>))) {
                return AddDbType(type, NpgsqlDbType.Timestamp);
            }
            if(type.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                return AddDbType(type, NpgsqlDbType.TimestampTz);
            }
            if(type.IsAssignableTo(typeof(IValue<DateOnly>))) {
                return AddDbType(type, NpgsqlDbType.Date);
            }
            if(type.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                return AddDbType(type, NpgsqlDbType.Time);
            }
            if(type.IsAssignableTo(typeof(IValue<float>))) {
                return AddDbType(type, NpgsqlDbType.Real);
            }
            if(type.IsAssignableTo(typeof(IValue<double>))) {
                return AddDbType(type, NpgsqlDbType.Double);
            }
            if(type.IsAssignableTo(typeof(IValue<Bit>))) {
                return AddDbType(type, NpgsqlDbType.Boolean);
            }

            Type? underlyingType = Nullable.GetUnderlyingType(type);

            if(underlyingType != null) {

                /*
                 *  Map Nullable Key Types
                 */
                if(underlyingType.IsAssignableTo(typeof(IGuidType))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Uuid);
                }
                if(underlyingType.IsAssignableTo(typeof(IStringType))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Varchar);
                }
                if(underlyingType.IsAssignableTo(typeof(IInt16Type))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Smallint);
                }
                if(underlyingType.IsAssignableTo(typeof(IInt32Type))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Integer);
                }
                if(underlyingType.IsAssignableTo(typeof(IInt64Type))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Bigint);
                }
                if(underlyingType.IsAssignableTo(typeof(IBoolType))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Boolean);
                }

                /*
                 *  Map Nullable Custom Types
                 */
                if(underlyingType.IsAssignableTo(typeof(IValue<Guid>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Uuid);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<short>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Smallint);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<int>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Integer);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<long>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Bigint);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<string>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Varchar);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<bool>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Boolean);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<decimal>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Numeric);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateTime>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Timestamp);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.TimestampTz);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateOnly>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Date);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Time);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<float>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Real);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<double>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Double);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<Bit>))) {
                    return AddDbType(underlyingType, NpgsqlDbType.Boolean);
                }
            }

            /*
             * Map Enum Types
             */
            Type? enumType = null;

            if(type.IsEnum) {
                enumType = type;
            }
            else {  //Check to see if this is a nullable enum type

                if(underlyingType != null && underlyingType.IsEnum) {
                    enumType = underlyingType;
                }
            }

            if(enumType != null) {

                NumericType integerType = EnumHelper.GetNumericType(enumType);

                if(integerType == NumericType.UShort) {
                    return AddDbType(type, NpgsqlDbType.Smallint);
                }
                else if(integerType == NumericType.Short) {
                    return AddDbType(type, NpgsqlDbType.Smallint);
                }
                else if(integerType == NumericType.UInt) {
                    return AddDbType(type, NpgsqlDbType.Integer);
                }
                else if(integerType == NumericType.Int) {
                    return AddDbType(type, NpgsqlDbType.Integer);
                }
                else if(integerType == NumericType.ULong) {
                    return AddDbType(type, NpgsqlDbType.Bigint);
                }
                else if(integerType == NumericType.Long) {
                    return AddDbType(type, NpgsqlDbType.Bigint);
                }
                else if(integerType == NumericType.SByte) {
                    return AddDbType(type, NpgsqlDbType.Smallint);
                }
                else if(integerType == NumericType.Byte) {
                    return AddDbType(type, NpgsqlDbType.Smallint);
                }
                else {
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
                }
            }
            throw new Exception($"Unknown PostgreSql parameter type '{type.FullName}'");
        }

        public static string ToSqlString(bool value) => value ? "true" : "false";

        public static string ToSqlString(Bit value) => value.Value ? "true" : "false";

        public static string ToSqlString(byte[] value) => $"decode('{Convert.ToHexString(value)}', 'hex')";

        public static string ToSqlString(byte value) => value.ToString();

        public static string ToSqlString(DateTimeOffset value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'";

        public static string ToSqlString(DateTime value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'";

        public static string ToSqlString(TimeOnly value) => $"'{Helpers.EscapeForSql(value.ToString("HH:mm:ss.fffffff"))}'";

        public static string ToSqlString(DateOnly value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd"))}'";

        public static string ToSqlString(decimal value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public static string ToSqlString(double value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public static string ToSqlString(float value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public static string ToSqlString(Guid value) => $"'{Helpers.EscapeForSql(value.ToString())}'";

        public static string ToSqlString(short value) => value != 0 ? value.ToString() : "0";

        public static string ToSqlString(int value) => value != 0 ? value.ToString() : "0";

        public static string ToSqlString(long value) => value != 0 ? value.ToString() : "0";

        public static string ToSqlString(string value) => value.Length > 0 ? $"'{Helpers.EscapeForSql(value)}'" : "''";

        public static string ToSqlString(IGuidType value) => ToSqlString(value.Value);

        public static string ToSqlString(IStringType value) => ToSqlString(value.Value);

        public static string ToSqlString(IInt16Type value) => ToSqlString(value.Value);

        public static string ToSqlString(IInt32Type value) => ToSqlString(value.Value);

        public static string ToSqlString(IInt64Type value) => ToSqlString(value.Value);

        public static string ToSqlString(IBoolType value) => ToSqlString(value.Value);

        public static string ConvertToSql(object value) {

            if(value is bool boolValue) {
                return ToSqlString(boolValue);
            }
            if(value is Bit bitValue) {
                return ToSqlString(bitValue);
            }
            if(value is byte[] byteArray) {
                return ToSqlString(byteArray);
            }
            if(value is byte byteValue) {
                return ToSqlString(byteValue);
            }
            if(value is DateTimeOffset dateTimeOffsetValue) {
                return ToSqlString(dateTimeOffsetValue);
            }
            if(value is DateTime dateTimeValue) {
                return ToSqlString(dateTimeValue);
            }
            if(value is TimeOnly timeOnly) {
                return ToSqlString(timeOnly);
            }
            if(value is DateOnly dateOnly) {
                return ToSqlString(dateOnly);
            }
            if(value is decimal decimalValue) {
                return ToSqlString(decimalValue);
            }
            if(value is double doubleValue) {
                return ToSqlString(doubleValue);
            }
            if(value is float floatValue) {
                return ToSqlString(floatValue);
            }
            if(value is Guid guidValue) {
                return ToSqlString(guidValue);
            }
            if(value is short shortValue) {
                return ToSqlString(shortValue);
            }
            if(value is int intValue) {
                return ToSqlString(intValue);
            }
            if(value is long longValue) {
                return ToSqlString(longValue);
            }
            if(value is string stringValue) {
                return ToSqlString(stringValue);
            }

            if(value is IGuidType guidType) {
                return ToSqlString(guidType);
            }
            if(value is IStringType stringType) {
                return ToSqlString(stringType);
            }
            if(value is IInt16Type int16Type) {
                return ToSqlString(int16Type);
            }
            if(value is IInt32Type int32Type) {
                return ToSqlString(int32Type);
            }
            if(value is IInt64Type int64Type) {
                return ToSqlString(int64Type);
            }
            if(value is IBoolType boolType) {
                return ToSqlString(boolType);
            }

            if(value is IValue<Guid> guidIValue) {
                return ToSqlString(guidIValue.Value);
            }
            if(value is IValue<short> shortIValue) {
                return ToSqlString(shortIValue.Value);
            }
            if(value is IValue<int> intIValue) {
                return ToSqlString(intIValue.Value);
            }
            if(value is IValue<long> longIValue) {
                return ToSqlString(longIValue.Value);
            }
            if(value is IValue<string> stringIValue) {
                return ToSqlString(stringIValue.Value);
            }
            if(value is IValue<bool> boolIValue) {
                return ToSqlString(boolIValue.Value);
            }
            if(value is IValue<decimal> decimalIValue) {
                return ToSqlString(decimalIValue.Value);
            }
            if(value is IValue<DateTime> dateTimeIValue) {
                return ToSqlString(dateTimeIValue.Value);
            }
            if(value is IValue<DateTimeOffset> dateTimeOffsetIValue) {
                return ToSqlString(dateTimeOffsetIValue.Value);
            }
            if(value is IValue<DateOnly> dateOnlyIValue) {
                return ToSqlString(dateOnlyIValue.Value);
            }
            if(value is IValue<TimeOnly> timeOnlyIValue) {
                return ToSqlString(timeOnlyIValue.Value);
            }
            if(value is IValue<float> floatIValue) {
                return ToSqlString(floatIValue.Value);
            }
            if(value is IValue<double> doubleIValue) {
                return ToSqlString(doubleIValue.Value);
            }
            if(value is IValue<Bit> bitIValue) {
                return ToSqlString(bitIValue.Value);
            }

            Type type = value.GetType();

            if(type.IsEnum) {
                return ((int)value).ToString()!;
            }
            throw new Exception($"Unknown PostgreSql parameter type '{value.GetType().FullName}'");
        }

        public static string? GetCSharpCodeSet(Type dotNetType) {

            if(dotNetType == typeof(string)) {
                return "string.Empty";
            }
            return null;
        }
    }
}