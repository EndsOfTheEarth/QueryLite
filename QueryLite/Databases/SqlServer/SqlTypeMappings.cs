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
using System.Data;

namespace QueryLite.Databases.SqlServer {

    public static class SqlServerSqlTypeMappings {

        public readonly static SqlServerTypeMapper TypeMapper = new SqlServerTypeMapper();

        public static SqlServerToStringFunctions ToSqlStringFunctions { get; } = new SqlServerToStringFunctions();
    }

    public sealed class SqlServerToStringFunctions : AToSqlStringFunctions {

        public override string ToSqlString(bool value) => value ? "1" : "0";

        public override string ToSqlString(Bit value) => value.Value ? "1" : "0";

        public override string ToSqlString(byte[] value) => $"0x{Convert.ToHexString(value)}";

        public override string ToSqlString(byte value) => value.ToString();

        public override string ToSqlString(DateTimeOffset value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'";

        public override string ToSqlString(DateTime value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'";

        public override string ToSqlString(TimeOnly value) => $"'{Helpers.EscapeForSql(value.ToString("HH:mm:ss.fffffff"))}'";

        public override string ToSqlString(DateOnly value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd"))}'";

        public override string ToSqlString(decimal value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public override string ToSqlString(double value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public override string ToSqlString(float value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public override string ToSqlString(Guid value) => $"'{Helpers.EscapeForSql(value.ToString())}'";

        public override string ToSqlString(short value) => value != 0 ? value.ToString() : "0";

        public override string ToSqlString(int value) => value != 0 ? value.ToString() : "0";

        public override string ToSqlString(long value) => value != 0 ? value.ToString() : "0";

        public override string ToSqlString(string value) => value.Length > 0 ? $"N'{Helpers.EscapeForSql(value)}'" : "''";

        public override string? GetCSharpCodeSet(Type dotNetType) {

            if(dotNetType == typeof(string)) {
                return "string.Empty";
            }
            return null;
        }
    }

    /// <summary>
    /// Map of csharp types to their SQL Server SqlDbType value.
    /// </summary>
    public class SqlServerTypeMapper : ATypeMap<SqlDbType> {

        public override SqlDbType Guid => SqlDbType.UniqueIdentifier;
        public override SqlDbType String => SqlDbType.NVarChar;
        public override SqlDbType Boolean => SqlDbType.Bit;
        public override SqlDbType ByteArray => SqlDbType.Binary;
        public override SqlDbType SByte => SqlDbType.SmallInt;
        public override SqlDbType Byte => SqlDbType.SmallInt;
        public override SqlDbType DateTimeOffset => SqlDbType.DateTimeOffset;
        public override SqlDbType DateTime => SqlDbType.DateTime;
        public override SqlDbType TimeOnly => SqlDbType.Time;
        public override SqlDbType DateOnly => SqlDbType.Date;
        public override SqlDbType Decimal => SqlDbType.Decimal;
        public override SqlDbType Double => SqlDbType.Float;
        public override SqlDbType Float => SqlDbType.Real;
        public override SqlDbType Short => SqlDbType.SmallInt;
        public override SqlDbType Integer => SqlDbType.Int;
        public override SqlDbType Long => SqlDbType.BigInt;
        public override SqlDbType Bit => SqlDbType.Bit;
    }
}