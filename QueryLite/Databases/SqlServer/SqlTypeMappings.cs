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

        public readonly static SqlServerTypeMapper TypeMapper = new SqlServerTypeMapper();

        public static IToSqlStringFunctions ToSqlStringFunctions { get; } = new SqlServerToStringFunctions();

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

    public class SqlServerToStringFunctions : IToSqlStringFunctions {

        public string ToSqlString(bool value) => value ? "1" : "0";

        public string ToSqlString(Bit value) => value.Value ? "1" : "0";

        public string ToSqlString(byte[] value) => $"0x{Convert.ToHexString(value)}";

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

        public string ToSqlString(string value) => value.Length > 0 ? $"N'{Helpers.EscapeForSql(value)}'" : "''";

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
    /// Map of csharp types to their SQL Server SqlDbType value.
    /// </summary>
    public class SqlServerTypeMapper : ITypeMap<SqlDbType> {

        public Dictionary<Type, SqlDbType> DbTypeLookup { get; }

        public SqlServerTypeMapper() {
            DbTypeLookup = ITypeMap<SqlDbType>.GetDbTypeLookup(this);
        }

        public SqlDbType GetDbType(Type type) => ((ITypeMap<SqlDbType>)this).GetDbTypeProtected(type);

        public SqlDbType Guid => SqlDbType.UniqueIdentifier;
        public SqlDbType String => SqlDbType.NVarChar;
        public SqlDbType Boolean => SqlDbType.Bit;
        public SqlDbType ByteArray => SqlDbType.Binary;
        public SqlDbType Byte => SqlDbType.SmallInt;
        public SqlDbType DateTimeOffset => SqlDbType.DateTimeOffset;
        public SqlDbType DateTime => SqlDbType.DateTime;
        public SqlDbType TimeOnly => SqlDbType.Time;
        public SqlDbType DateOnly => SqlDbType.Date;
        public SqlDbType Decimal => SqlDbType.Decimal;
        public SqlDbType Double => SqlDbType.Float;
        public SqlDbType Float => SqlDbType.Real;
        public SqlDbType Short => SqlDbType.SmallInt;
        public SqlDbType Integer => SqlDbType.Int;
        public SqlDbType Long => SqlDbType.BigInt;
        public SqlDbType Bit => SqlDbType.Bit;
    }
}