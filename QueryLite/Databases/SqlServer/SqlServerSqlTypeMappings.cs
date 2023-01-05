using System;
using System.Data;

namespace QueryLite.Databases.SqlServer {

    public static class SqlServerSqlTypeMappings {

        public static DbType GetDbType(Type type) {

            if(type == typeof(Guid)) {
                return DbType.Guid;
            }
            if(type == typeof(Guid?)) {
                return DbType.Guid;
            }

            if(type == typeof(bool)) {
                return DbType.Boolean;
            }
            if(type == typeof(bool?)) {
                return DbType.Boolean;
            }

            if(type == typeof(byte[])) {
                return DbType.Binary;
            }
            if(type == typeof(byte?[])) {
                return DbType.Binary;
            }

            if(type == typeof(DateTimeOffset)) {
                return DbType.DateTimeOffset;
            }
            if(type == typeof(DateTimeOffset?)) {
                return DbType.DateTimeOffset;
            }

            if(type == typeof(DateTime)) {
                return DbType.DateTime;
            }
            if(type == typeof(DateTime?)) {
                return DbType.DateTime;
            }

            if(type == typeof(decimal)) {
                return DbType.Decimal;
            }
            if(type == typeof(decimal?)) {
                return DbType.Decimal;
            }

            if(type == typeof(double)) {
                return DbType.Double;
            }
            if(type == typeof(double?)) {
                return DbType.Double;
            }

            if(type == typeof(float)) {
                return DbType.Single;
            }
            if(type == typeof(float?)) {
                return DbType.Single;
            }

            if(type.IsEnum) {
                return DbType.Int32;
            }

            if(type == typeof(short)) {
                return DbType.Int16;
            }
            if(type == typeof(short?)) {
                return DbType.Int16;
            }

            if(type == typeof(int)) {
                return DbType.Int32;
            }
            if(type == typeof(int?)) {
                return DbType.Int32;
            }

            if(type == typeof(long)) {
                return DbType.Int64;
            }
            if(type == typeof(long?)) {
                return DbType.Int64;
            }

            if(type == typeof(string)) {
                return DbType.String;
            }

            if(type.IsAssignableTo(typeof(IGuidType))) {
                return DbType.Guid;
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return DbType.String;
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return DbType.Int16;
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return DbType.Int32;
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return DbType.Int64;
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return DbType.Boolean;
            }
            throw new Exception($"Unknown SqlServer parameter type '{type.FullName}'");
        }

        public static object ConvertToRawType(object value) {

            if(value is Enum) {
                return (int)value;
            }
            return value;
        }

        public static string ConvertToSql(object value) {

            if(value is bool boolValue) {
                return boolValue ? "1" : "0";
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