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
using QueryLite.Utility;
using System;
using System.Collections.Generic;
using System.Data;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace QueryLite.Databases.SqlServer {

    public static class SqlServerSqlTypeMappings {

#if NET9_0_OR_GREATER
        private static readonly Lock _lock = new Lock();
#else
        private static readonly object _lock = new object();
#endif

        private static readonly Dictionary<Type, SqlDbType> DbTypeLookup = new Dictionary<Type, SqlDbType>() {

            { typeof(Guid), SqlDbType.UniqueIdentifier },
            { typeof(Guid?), SqlDbType.UniqueIdentifier },
            { typeof(bool),  SqlDbType.Bit },
            { typeof(bool?), SqlDbType.Bit },
            { typeof(Bit), SqlDbType.Bit },
            { typeof(Bit?),  SqlDbType.Bit },
            { typeof(byte[]), SqlDbType.Binary },
            { typeof(byte?[]), SqlDbType.Binary },
            { typeof(byte), SqlDbType.SmallInt },
            { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
            { typeof(DateTimeOffset?), SqlDbType.DateTimeOffset },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(DateTime?), SqlDbType.DateTime },
            { typeof(TimeOnly), SqlDbType.Time },
            { typeof(TimeOnly?), SqlDbType.Time },
            { typeof(DateOnly), SqlDbType.Date },
            { typeof(DateOnly?), SqlDbType.Date },
            { typeof(decimal), SqlDbType.Decimal },
            { typeof(decimal?), SqlDbType.Decimal },
            { typeof(double), SqlDbType.Float },
            { typeof(double?), SqlDbType.Float },
            { typeof(float), SqlDbType.Real },
            { typeof(float?), SqlDbType.Real },
            { typeof(short), SqlDbType.SmallInt },
            { typeof(short?), SqlDbType.SmallInt },
            { typeof(int), SqlDbType.Int },
            { typeof(int?), SqlDbType.Int },
            { typeof(long), SqlDbType.BigInt },
            { typeof(long?), SqlDbType.BigInt },
            { typeof(string), SqlDbType.NVarChar }
        };

        private static SqlDbType AddDbType(Type type, SqlDbType dbType) {

            lock(_lock) {
                DbTypeLookup.TryAdd(type, dbType);
            }
            return dbType;
        }

        public static SqlDbType GetDbType(Type type) {

            if(DbTypeLookup.TryGetValue(type, out SqlDbType dbType)) {
                return dbType;
            }

            if(type.IsEnum) {
                return AddDbType(type, SqlDbType.Int);
            }

            /*
             *  Map Key Types
             */
            if(type.IsAssignableTo(typeof(IGuidType))) {
                return AddDbType(type, SqlDbType.UniqueIdentifier);
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return AddDbType(type, SqlDbType.NVarChar);
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return AddDbType(type, SqlDbType.SmallInt);
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return AddDbType(type, SqlDbType.Int);
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return AddDbType(type, SqlDbType.BigInt);
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return AddDbType(type, SqlDbType.Bit);
            }

            /*
             *  Map Custom Types
             */
            if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                return AddDbType(type, SqlDbType.UniqueIdentifier);
            }
            if(type.IsAssignableTo(typeof(IValue<short>))) {
                return AddDbType(type, SqlDbType.SmallInt);
            }
            if(type.IsAssignableTo(typeof(IValue<int>))) {
                return AddDbType(type, SqlDbType.Int);
            }
            if(type.IsAssignableTo(typeof(IValue<long>))) {
                return AddDbType(type, SqlDbType.BigInt);
            }
            if(type.IsAssignableTo(typeof(IValue<string>))) {
                return AddDbType(type, SqlDbType.NVarChar);
            }
            if(type.IsAssignableTo(typeof(IValue<bool>))) {
                return AddDbType(type, SqlDbType.Bit);
            }
            if(type.IsAssignableTo(typeof(IValue<decimal>))) {
                return AddDbType(type, SqlDbType.Decimal);
            }
            if(type.IsAssignableTo(typeof(IValue<DateTime>))) {
                return AddDbType(type, SqlDbType.DateTime);
            }
            if(type.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                return AddDbType(type, SqlDbType.DateTimeOffset);
            }
            if(type.IsAssignableTo(typeof(IValue<DateOnly>))) {
                return AddDbType(type, SqlDbType.Date);
            }
            if(type.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                return AddDbType(type, SqlDbType.Time);
            }
            if(type.IsAssignableTo(typeof(IValue<float>))) {
                return AddDbType(type, SqlDbType.Real);
            }
            if(type.IsAssignableTo(typeof(IValue<double>))) {
                return AddDbType(type, SqlDbType.Float);
            }
            if(type.IsAssignableTo(typeof(IValue<Bit>))) {
                return AddDbType(type, SqlDbType.Bit);
            }

            Type? underlyingType = Nullable.GetUnderlyingType(type);

            /*
             *  Map Nullable Key Types
             */
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IGuidType))) {
                return AddDbType(underlyingType, SqlDbType.UniqueIdentifier);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IStringType))) {
                return AddDbType(underlyingType, SqlDbType.NVarChar);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IInt16Type))) {
                return AddDbType(underlyingType, SqlDbType.SmallInt);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IInt32Type))) {
                return AddDbType(underlyingType, SqlDbType.Int);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IInt64Type))) {
                return AddDbType(underlyingType, SqlDbType.BigInt);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IBoolType))) {
                return AddDbType(underlyingType, SqlDbType.Bit);
            }

            /*
             *  Map Nullable Custom Types
             */
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<Guid>))) {
                return AddDbType(underlyingType, SqlDbType.UniqueIdentifier);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<short>))) {
                return AddDbType(underlyingType, SqlDbType.SmallInt);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<int>))) {
                return AddDbType(underlyingType, SqlDbType.Int);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<long>))) {
                return AddDbType(underlyingType, SqlDbType.BigInt);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<string>))) {
                return AddDbType(underlyingType, SqlDbType.NVarChar);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<bool>))) {
                return AddDbType(underlyingType, SqlDbType.Bit);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<decimal>))) {
                return AddDbType(underlyingType, SqlDbType.Decimal);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<DateTime>))) {
                return AddDbType(underlyingType, SqlDbType.DateTime);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                return AddDbType(underlyingType, SqlDbType.DateTimeOffset);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<DateOnly>))) {
                return AddDbType(underlyingType, SqlDbType.Date);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                return AddDbType(underlyingType, SqlDbType.Time);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<float>))) {
                return AddDbType(underlyingType, SqlDbType.Real);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<double>))) {
                return AddDbType(underlyingType, SqlDbType.Float);
            }
            if(underlyingType != null && underlyingType.IsAssignableTo(typeof(IValue<Bit>))) {
                return AddDbType(underlyingType, SqlDbType.Bit);
            }

            throw new Exception($"Unknown SqlServer parameter type '{type.FullName}'");
        }

        public static object ConvertToRawType(object value) {

            if(value is Enum) {
                return (int)value;
            }
            if(value is DateOnly dateOnly) {
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            if(value is TimeOnly timeOnly) {
                return timeOnly.ToTimeSpan();
            }
            return value;
        }

        public static string ToSqlString(bool value) => value ? "1" : "0";

        public static string ToSqlString(Bit value) => value.Value ? "1" : "0";

        public static string ToSqlString(byte[] value) => "0x" + (BitConverter.ToString(value)).Replace("-", string.Empty);

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

        public static string ToSqlString(string value) => value.Length > 0 ? $"N'{Helpers.EscapeForSql(value)}'" : "''";

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
            if(value is IValue<Bit> butIValue) {
                return ToSqlString(butIValue.Value);
            }

            Type type = value.GetType();

            if(type.IsEnum) {
                return ((int)value).ToString()!;
            }
            throw new Exception($"Unknown SqlServer parameter type '{value.GetType().FullName}'");
        }

        public static string? GetCSharpCodeSet(Type dotNetType) {

            if(dotNetType == typeof(string)) {
                return "string.Empty";
            }
            return null;
        }
    }
}