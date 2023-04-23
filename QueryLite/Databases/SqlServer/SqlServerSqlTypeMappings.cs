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
using System;
using System.Data;

namespace QueryLite.Databases.SqlServer {

    public static class SqlServerSqlTypeMappings {

        public static SqlDbType GetDbType(Type type) {

            if(type == typeof(Guid)) {
                return SqlDbType.UniqueIdentifier;
            }
            if(type == typeof(Guid?)) {
                return SqlDbType.UniqueIdentifier;
            }

            if(type == typeof(bool)) {
                return SqlDbType.Bit;
            }
            if(type == typeof(bool?)) {
                return SqlDbType.Bit;
            }

            if(type == typeof(Bit)) {
                return SqlDbType.Bit;
            }
            if(type == typeof(Bit?)) {
                return SqlDbType.Bit;
            }

            if(type == typeof(byte[])) {
                return SqlDbType.Binary;
            }
            if(type == typeof(byte?[])) {
                return SqlDbType.Binary;
            }

            if(type == typeof(DateTimeOffset)) {
                return SqlDbType.DateTimeOffset;
            }
            if(type == typeof(DateTimeOffset?)) {
                return SqlDbType.DateTimeOffset;
            }

            if(type == typeof(DateTime)) {
                return SqlDbType.DateTime;
            }
            if(type == typeof(DateTime?)) {
                return SqlDbType.DateTime;
            }

            if(type == typeof(TimeOnly)) {
                return SqlDbType.Time;
            }
            if(type == typeof(TimeOnly?)) {
                return SqlDbType.Time;
            }

            if(type == typeof(DateOnly)) {
                return SqlDbType.Date;
            }
            if(type == typeof(DateOnly?)) {
                return SqlDbType.Date;
            }

            if(type == typeof(decimal)) {
                return SqlDbType.Decimal;
            }
            if(type == typeof(decimal?)) {
                return SqlDbType.Decimal;
            }

            if(type == typeof(double)) {
                return SqlDbType.Float;
            }
            if(type == typeof(double?)) {
                return SqlDbType.Float;
            }

            if(type == typeof(float)) {
                return SqlDbType.Real;
            }
            if(type == typeof(float?)) {
                return SqlDbType.Real;
            }

            if(type.IsEnum) {
                return SqlDbType.Int;
            }

            if(type == typeof(short)) {
                return SqlDbType.SmallInt;
            }
            if(type == typeof(short?)) {
                return SqlDbType.SmallInt;
            }

            if(type == typeof(int)) {
                return SqlDbType.Int;
            }
            if(type == typeof(int?)) {
                return SqlDbType.Int;
            }

            if(type == typeof(long)) {
                return SqlDbType.BigInt;
            }
            if(type == typeof(long?)) {
                return SqlDbType.BigInt;
            }

            if(type == typeof(string)) {
                return SqlDbType.NVarChar;
            }

            if(type.IsAssignableTo(typeof(IGuidType))) {
                return SqlDbType.UniqueIdentifier;
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return SqlDbType.NVarChar;
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return SqlDbType.SmallInt;
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return SqlDbType.Int;
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return SqlDbType.BigInt;
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return SqlDbType.Bit;
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

        public static string ConvertToSql(object value) {

            if(value is bool boolValue) {
                return boolValue ? "1" : "0";
            }
            if(value is Bit bitValue) {
                return bitValue.Value ? "1" : "0";
            }
            if(value is byte[] byteArray) {
                return "0x" + (BitConverter.ToString(byteArray)).Replace("-", string.Empty);
            }
            if(value is byte byteValue) {
                return byteValue.ToString();
            }
            if(value is DateTimeOffset dateTimeOffsetValue) {
                return $"'{Helpers.EscapeForSql(dateTimeOffsetValue.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'";
            }
            if(value is DateTime dateTimeValue) {
                return $"'{Helpers.EscapeForSql(dateTimeValue.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'";
            }
            if(value is TimeOnly timeOnly) {
                return $"'{Helpers.EscapeForSql(timeOnly.ToString("HH:mm:ss.fffffff"))}'";
            }
            if(value is DateOnly dateOnly) {
                return $"'{Helpers.EscapeForSql(dateOnly.ToString("yyyy-MM-dd"))}'";
            }
            if(value is decimal decimalValue) {
                return $"{Helpers.EscapeForSql(decimalValue.ToString())}";
            }
            if(value is double doubleValue) {
                return $"{Helpers.EscapeForSql(doubleValue.ToString())}";
            }
            if(value is float floatValue) {
                return $"{Helpers.EscapeForSql(floatValue.ToString())}";
            }
            if(value is Guid guidValue) {
                return $"'{Helpers.EscapeForSql(guidValue.ToString())}'";
            }
            if(value is short shortValue) {
                return shortValue.ToString();
            }
            if(value is int intValue) {
                return intValue.ToString();
            }
            if(value is long longValue) {
                return longValue.ToString();
            }
            if(value is string stringValue) {
                return $"'{Helpers.EscapeForSql(stringValue)}'";
            }

            if(value is IKeyValue keyValue) {
                return ConvertToSql(keyValue.GetValueAsObject());
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