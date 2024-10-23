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
using NpgsqlTypes;
using QueryLite.Utility;
using System;
using System.Data;

namespace QueryLite.Databases.PostgreSql {

    public static class PostgreSqlTypeMappings {

        public static NpgsqlDbType GetNpgsqlDbType(Type type) {

            if(type == typeof(Guid)) {
                return NpgsqlDbType.Uuid;
            }
            if(type == typeof(Guid?)) {
                return NpgsqlDbType.Uuid;
            }

            if(type == typeof(bool)) {
                return NpgsqlDbType.Boolean;
            }
            if(type == typeof(bool?)) {
                return NpgsqlDbType.Boolean;
            }

            if(type == typeof(Bit)) {
                return NpgsqlDbType.Boolean;
            }
            if(type == typeof(Bit?)) {
                return NpgsqlDbType.Boolean;
            }

            if(type == typeof(byte[])) {
                return NpgsqlDbType.Bytea;
            }
            if(type == typeof(byte?[])) {
                return NpgsqlDbType.Bytea;
            }

            if(type == typeof(byte)) {
                return NpgsqlDbType.Smallint;
            }

            if(type == typeof(DateTimeOffset)) {
                return NpgsqlDbType.TimestampTz;
            }
            if(type == typeof(DateTimeOffset?)) {
                return NpgsqlDbType.TimestampTz;
            }

            if(type == typeof(DateTime)) {
                return NpgsqlDbType.Timestamp;
            }
            if(type == typeof(DateTime?)) {
                return NpgsqlDbType.Timestamp;
            }

            if(type == typeof(TimeOnly)) {
                return NpgsqlDbType.Time;
            }
            if(type == typeof(TimeOnly?)) {
                return NpgsqlDbType.Time;
            }

            if(type == typeof(DateOnly)) {
                return NpgsqlDbType.Date;
            }
            if(type == typeof(DateOnly?)) {
                return NpgsqlDbType.Date;
            }

            if(type == typeof(decimal)) {
                return NpgsqlDbType.Numeric;
            }
            if(type == typeof(decimal?)) {
                return NpgsqlDbType.Numeric;
            }

            if(type == typeof(double)) {
                return NpgsqlDbType.Double;
            }
            if(type == typeof(double?)) {
                return NpgsqlDbType.Double;
            }

            if(type == typeof(float)) {
                return NpgsqlDbType.Real;
            }
            if(type == typeof(float?)) {
                return NpgsqlDbType.Real;
            }

            if(type.IsEnum) {
                return NpgsqlDbType.Integer;
            }

            if(type == typeof(short)) {
                return NpgsqlDbType.Smallint;
            }
            if(type == typeof(short?)) {
                return NpgsqlDbType.Smallint;
            }

            if(type == typeof(int)) {
                return NpgsqlDbType.Integer;
            }
            if(type == typeof(int?)) {
                return NpgsqlDbType.Integer;
            }

            if(type == typeof(long)) {
                return NpgsqlDbType.Bigint;
            }
            if(type == typeof(long?)) {
                return NpgsqlDbType.Bigint;
            }

            if(type == typeof(string)) {
                return NpgsqlDbType.Varchar;
            }

            if(type.IsAssignableTo(typeof(IGuidType))) {
                return NpgsqlDbType.Uuid;
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return NpgsqlDbType.Varchar;
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return NpgsqlDbType.Smallint;
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return NpgsqlDbType.Integer;
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return NpgsqlDbType.Bigint;
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return NpgsqlDbType.Boolean;
            }

            if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                return NpgsqlDbType.Uuid;
            }
            if(type.IsAssignableTo(typeof(IValue<short>))) {
                return NpgsqlDbType.Smallint;
            }
            if(type.IsAssignableTo(typeof(IValue<int>))) {
                return NpgsqlDbType.Integer;
            }
            if(type.IsAssignableTo(typeof(IValue<long>))) {
                return NpgsqlDbType.Bigint;
            }
            if(type.IsAssignableTo(typeof(IValue<string>))) {
                return NpgsqlDbType.Varchar;
            }
            if(type.IsAssignableTo(typeof(IValue<bool>))) {
                return NpgsqlDbType.Boolean;
            }
            throw new Exception($"Unknown PostgreSql parameter type '{type.FullName}'");
        }

        public static object ConvertToRawType(object value) {

            if(value is DateTimeOffset dateTimeOffsetValue) {   //time stamp with time zone must be sent in UTC for Npgsql to work
                return new DateTimeOffset(dateTimeOffsetValue.UtcDateTime);
            }
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

        public static string ToSqlString(bool value) => value ? "true" : "false";

        public static string ToSqlString(Bit value) => value.Value ? "true" : "false";

        public static string ToSqlString(byte[] value) => $"decode('{(BitConverter.ToString(value)).Replace("-", string.Empty)}', 'hex')";

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