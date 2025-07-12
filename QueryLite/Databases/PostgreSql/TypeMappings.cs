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

namespace QueryLite.Databases.PostgreSql {

    public static class PostgreSqlTypeMappings {

        public readonly static PostgreSqlTypeMap TypeMapper = new PostgreSqlTypeMap();

        public static PostgreSqlToStringFunctions ToSqlStringFunctions { get; } = new PostgreSqlToStringFunctions();
    }

    public sealed class PostgreSqlToStringFunctions : AToSqlStringFunctions {

        public override string ToSqlString(bool value) => value ? "true" : "false";

        public override string ToSqlString(Bit value) => value.Value ? "true" : "false";

        public override string ToSqlString(byte[] value) => $"decode('{Convert.ToHexString(value)}', 'hex')";

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

        public override string ToSqlString(string value) => value.Length > 0 ? $"'{Helpers.EscapeForSql(value)}'" : "''";

        public override string? GetCSharpCodeSet(Type dotNetType) {

            if(dotNetType == typeof(string)) {
                return "string.Empty";
            }
            return null;
        }
    }

    /// <summary>
    /// Map of csharp types to their PostgreSql NpgsqlDbType value.
    /// </summary>
    public class PostgreSqlTypeMap : ATypeMap<NpgsqlDbType> {

        public override NpgsqlDbType Guid => NpgsqlDbType.Uuid;
        public override NpgsqlDbType String => NpgsqlDbType.Varchar;
        public override NpgsqlDbType Boolean => NpgsqlDbType.Boolean;
        public override NpgsqlDbType ByteArray => NpgsqlDbType.Bytea;
        public override NpgsqlDbType SByte => NpgsqlDbType.Smallint;
        public override NpgsqlDbType Byte => NpgsqlDbType.Smallint;
        public override NpgsqlDbType DateTimeOffset => NpgsqlDbType.TimestampTz;
        public override NpgsqlDbType DateTime => NpgsqlDbType.Timestamp;
        public override NpgsqlDbType TimeOnly => NpgsqlDbType.Time;
        public override NpgsqlDbType DateOnly => NpgsqlDbType.Date;
        public override NpgsqlDbType Decimal => NpgsqlDbType.Numeric;
        public override NpgsqlDbType Double => NpgsqlDbType.Double;
        public override NpgsqlDbType Float => NpgsqlDbType.Real;
        public override NpgsqlDbType Short => NpgsqlDbType.Smallint;
        public override NpgsqlDbType Integer => NpgsqlDbType.Integer;
        public override NpgsqlDbType Long => NpgsqlDbType.Bigint;
        public override NpgsqlDbType Bit => NpgsqlDbType.Boolean;
    }
}