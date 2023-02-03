using NpgsqlTypes;
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

            if(type == typeof(byte[])) {
                return NpgsqlDbType.Bytea;
            }
            if(type == typeof(byte?[])) {
                return NpgsqlDbType.Bytea;
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

        public static string ConvertToSql(object value) {

            if(value is bool boolValue) {
                return boolValue ? "true" : "false";
            }
            if(value is byte[] byteArray) {
                return $"decode('{(BitConverter.ToString(byteArray)).Replace("-", string.Empty)}', 'hex')";
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