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
using System.Data;




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

        public readonly static PostgreSqlTypeMapper TypeMapper = new PostgreSqlTypeMapper();

        public static IToSqlStringFunctions ToSqlStringFunctions { get; } = new PostgreSqlToStringFunctions();

        private readonly static Dictionary<Type, ToSqlStringDelegate> ToSqlStringDelegateLookup = new Dictionary<Type, ToSqlStringDelegate>() {
            { typeof(bool), value => ToSqlStringFunctions.ToSqlString((bool)value) },
            { typeof(Bit), value => ToSqlStringFunctions.ToSqlString((Bit)value) },
            { typeof(byte[]), value => ToSqlStringFunctions.ToSqlString((byte[])value) },
            { typeof(byte), value => ToSqlStringFunctions.ToSqlString((byte)value) },
            { typeof(DateTimeOffset), value => ToSqlStringFunctions.ToSqlString((DateTimeOffset)value) },
            { typeof(DateTime), value => ToSqlStringFunctions.ToSqlString((DateTime)value) },
            { typeof(TimeOnly), value => ToSqlStringFunctions.ToSqlString((TimeOnly)value) },
            { typeof(DateOnly), value => ToSqlStringFunctions.ToSqlString((DateOnly)value) },
            { typeof(decimal), value => ToSqlStringFunctions.ToSqlString((decimal)value) },
            { typeof(double), value => ToSqlStringFunctions.ToSqlString((double)value) },
            { typeof(float), value => ToSqlStringFunctions.ToSqlString((float)value) },
            { typeof(Guid), value => ToSqlStringFunctions.ToSqlString((Guid)value) },
            { typeof(short), value => ToSqlStringFunctions.ToSqlString((short)value) },
            { typeof(int), value => ToSqlStringFunctions.ToSqlString((int)value) },
            { typeof(long), value => ToSqlStringFunctions.ToSqlString((long)value) },
            { typeof(string), value => ToSqlStringFunctions.ToSqlString((string)value) }
        };

        public static string ConvertToSql(object value) {

            Type type = value.GetType();

            if(ToSqlStringDelegateLookup.TryGetValue(type, out ToSqlStringDelegate? toSqlStringDelegate)) {
                return toSqlStringDelegate(value);
            }

            toSqlStringDelegate = ToSqlStringMapper.GetMapping(value, ToSqlStringFunctions);

            lock(_lock) {
                ToSqlStringDelegateLookup.TryAdd(type, toSqlStringDelegate);
            }
            return toSqlStringDelegate(value);
        }
    }

    public class PostgreSqlToStringFunctions : IToSqlStringFunctions {

        public string ToSqlString(bool value) => value ? "true" : "false";

        public string ToSqlString(Bit value) => value.Value ? "true" : "false";

        public string ToSqlString(byte[] value) => $"decode('{Convert.ToHexString(value)}', 'hex')";

        public string ToSqlString(byte value) => value.ToString();

        public string ToSqlString(DateTimeOffset value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'";

        public string ToSqlString(DateTime value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'";

        public string ToSqlString(TimeOnly value) => $"'{Helpers.EscapeForSql(value.ToString("HH:mm:ss.fffffff"))}'";

        public string ToSqlString(DateOnly value) => $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd"))}'";

        public string ToSqlString(decimal value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public string ToSqlString(double value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public string ToSqlString(float value) => value != 0 ? $"{Helpers.EscapeForSql(value.ToString())}" : "0";

        public string ToSqlString(Guid value) => $"'{Helpers.EscapeForSql(value.ToString())}'";

        public string ToSqlString(short value) => value != 0 ? value.ToString() : "0";

        public string ToSqlString(int value) => value != 0 ? value.ToString() : "0";

        public string ToSqlString(long value) => value != 0 ? value.ToString() : "0";

        public string ToSqlString(string value) => value.Length > 0 ? $"'{Helpers.EscapeForSql(value)}'" : "''";

        public string ToSqlString(IGuidType value) => ToSqlString(value.Value);

        public string ToSqlString(IStringType value) => ToSqlString(value.Value);

        public string ToSqlString(IInt16Type value) => ToSqlString(value.Value);

        public string ToSqlString(IInt32Type value) => ToSqlString(value.Value);

        public string ToSqlString(IInt64Type value) => ToSqlString(value.Value);

        public string ToSqlString(IBoolType value) => ToSqlString(value.Value);

        public string? GetCSharpCodeSet(Type dotNetType) {

            if(dotNetType == typeof(string)) {
                return "string.Empty";
            }
            return null;
        }
    }

    /// <summary>
    /// Map of csharp types to their PostgreSql NpgsqlDbType value.
    /// </summary>
    public class PostgreSqlTypeMapper : ITypeMap<NpgsqlDbType> {

        public Dictionary<Type, NpgsqlDbType> DbTypeLookup { get; }

        public PostgreSqlTypeMapper() {
            DbTypeLookup = ITypeMap<NpgsqlDbType>.GetDbTypeLookup(this);
        }

        public NpgsqlDbType GetDbType(Type type) => ((ITypeMap<NpgsqlDbType>)this).GetDbTypeProtected(type);

        public NpgsqlDbType Guid => NpgsqlDbType.Uuid;
        public NpgsqlDbType String => NpgsqlDbType.Varchar;
        public NpgsqlDbType Boolean => NpgsqlDbType.Boolean;
        public NpgsqlDbType ByteArray => NpgsqlDbType.Bytea;
        public NpgsqlDbType Byte => NpgsqlDbType.Smallint;
        public NpgsqlDbType DateTimeOffset => NpgsqlDbType.TimestampTz;
        public NpgsqlDbType DateTime => NpgsqlDbType.Timestamp;
        public NpgsqlDbType TimeOnly => NpgsqlDbType.Time;
        public NpgsqlDbType DateOnly => NpgsqlDbType.Date;
        public NpgsqlDbType Decimal => NpgsqlDbType.Numeric;
        public NpgsqlDbType Double => NpgsqlDbType.Double;
        public NpgsqlDbType Float => NpgsqlDbType.Real;
        public NpgsqlDbType Short => NpgsqlDbType.Smallint;
        public NpgsqlDbType Integer => NpgsqlDbType.Integer;
        public NpgsqlDbType Long => NpgsqlDbType.Bigint;
        public NpgsqlDbType Bit => NpgsqlDbType.Boolean;
    }
}