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
using Microsoft.Data.Sqlite;
using QueryLite.Utility;

namespace QueryLite.Databases.Sqlite {

    public static class SqliteTypeMappings {

        public readonly static SqliteTypeMap TypeMapper = new SqliteTypeMap();

        public static SqliteToStringFunctions ToSqlStringFunctions { get; } = new SqliteToStringFunctions();
    }

    public sealed class SqliteToStringFunctions : AToSqlStringFunctions {

        public override string ToSqlString(bool value) => value ? "true" : "false";

        public override string ToSqlString(Bit value) => value.Value ? "true" : "false";

        public override string ToSqlString(byte[] value) => $"x'{Convert.ToHexString(value)}'";

        public override string ToSqlString(byte value) => value.ToString();

        public override string ToSqlString(DateTimeOffset value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'";

        public override string ToSqlString(DateTime value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'";

        public override string ToSqlString(TimeOnly value) => $"'{Helpers.EscapeForSql(value.ToString("HH:mm:ss.fffffff"))}'";

        public override string ToSqlString(DateOnly value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd"))}'";

        public override string ToSqlString(decimal value) => value != 0 ? $"'{Helpers.EscapeForSql(value.ToString())}'" : "0";

        public override string ToSqlString(double value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public override string ToSqlString(float value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        //public override string ToSqlString(Guid value) => $"'{Helpers.EscapeForSql(value.ToString())}'";
        public override string ToSqlString(Guid value) => $"x'{Convert.ToHexString(value.ToByteArray())}'";

        public override string ToSqlString(short value) => value != 0 ? value.ToString() : "0";

        public override string ToSqlString(int value) => value != 0 ? value.ToString() : "0";

        public override string ToSqlString(long value) => value != 0 ? value.ToString() : "0";

        public override string ToSqlString(string value) => value.Length > 0 ? $"'{Helpers.EscapeForSql(value)}'" : "''";

        public override string ToSqlString(Json value) => $"'{Helpers.EscapeForSql(value.Value)}'";

        public override string ToSqlString(Jsonb value) => $"'{Helpers.EscapeForSql(value.Value)}'";

        public override string? GetCSharpCodeSet(Type dotNetType) {

            if(dotNetType == typeof(string)) {
                return "\"\"";
            }
            return null;
        }
    }

    /// <summary>
    /// Map of csharp types to their Sqlite SqliteType value.
    /// </summary>
    public class SqliteTypeMap : ATypeMap<SqliteType> {

        public override SqliteType Guid => SqliteType.Blob;
        public override SqliteType String => SqliteType.Text;
        public override SqliteType Boolean => SqliteType.Integer;
        public override SqliteType ByteArray => SqliteType.Blob;
        public override SqliteType SByte => SqliteType.Blob;
        public override SqliteType Byte => SqliteType.Blob;
        public override SqliteType DateTimeOffset => SqliteType.Text;
        public override SqliteType DateTime => SqliteType.Text;
        public override SqliteType TimeOnly => SqliteType.Text;
        public override SqliteType DateOnly => SqliteType.Text;
        public override SqliteType Decimal => SqliteType.Text;
        public override SqliteType Double => SqliteType.Real;
        public override SqliteType Float => SqliteType.Real;
        public override SqliteType Short => SqliteType.Integer;
        public override SqliteType Integer => SqliteType.Integer;
        public override SqliteType Long => SqliteType.Integer;
        public override SqliteType Bit => SqliteType.Blob;
        public override SqliteType Json => SqliteType.Text;
        public override SqliteType JsonB => SqliteType.Text;
    }
}