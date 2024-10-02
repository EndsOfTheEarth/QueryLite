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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace QueryLite.Databases {

    public static class SqlHelper {

        public static void AppendEncloseTableName(StringBuilder sql, ITable table) {

            string value = table.TableName;

            if(table.Enclose || value.Contains(' ')) {
                sql.Append('[').Append(value).Append(']');
            }
            else {
                sql.Append(value);
            }
        }

        public static void AppendEncloseColumnName(StringBuilder sql, IColumn column) {

            string value = column.ColumnName;

            if(column.Enclose || value.Contains(' ')) {
                sql.Append('[').Append(value).Append(']');
            }
            else {
                sql.Append(value);
            }
        }

        public static void AppendEncloseSchemaName(StringBuilder sql, string value) {

            if(value.Contains(' ')) {
                sql.Append('[').Append(value).Append(']');
            }
            else {
                sql.Append(value);
            }
        }

        public static void AppendEncloseAlias(StringBuilder sql, string value) {

            if(value.Contains(' ')) {
                sql.Append('[').Append(value).Append(']');
            }
            else {
                sql.Append(value);
            }
        }

        public static string EncloseTableName(ITable table) {

            string value = table.TableName;

            if(table.Enclose || value.Contains(' ')) {
                value = $"[{value}]";
            }
            return value;
        }

        public static string EncloseColumnName(IColumn column) {

            string value = column.ColumnName;

            if(column.Enclose || value.Contains(' ')) {
                value = $"[{value}]";
            }
            return value;
        }

        public static string EncloseSchemaName(string value) {
            return value.Contains(' ') ? $"[{value}]" : value;
        }
    }

    /// <summary>
    /// This class is used to convert an integer to a generic enum type without causing the integer to be boxed
    /// </summary>
    /// <typeparam name="INTEGER"></typeparam>
    /// <typeparam name="ENUM"></typeparam>
    internal static class IntegerToEnum<INTEGER, ENUM> where INTEGER : struct, IComparable, IFormattable, IConvertible, IComparable<INTEGER>, IEquatable<INTEGER> where ENUM : Enum {

        private static readonly Converter<INTEGER, ENUM> _function;

        static IntegerToEnum() {
            DynamicMethod method = new DynamicMethod(
                name: "",
                returnType: typeof(ENUM),
                parameterTypes: new[] { typeof(INTEGER) },
                restrictedSkipVisibility: true
            );
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);
            _function = (Converter<INTEGER, ENUM>)method.CreateDelegate(typeof(Converter<INTEGER, ENUM>));
        }
        public static ENUM Convert(INTEGER value) {
            return _function(value);
        }
    }

    /// <summary>
    /// Cache parameter names to reduce memory allocations
    /// </summary>
    internal static class ParamNameCache {

        public static string[] ParamNames = new string[50];

        static ParamNameCache() {

            for(int index = 0; index < ParamNames.Length; index++) {
                ParamNames[index] = $"@{index}";
            }
        }
    }

    public enum NumericType {
        UShort,
        Short,
        Int,
        UInt,
        Long,
        ULong,
        Byte,
        SByte
    }

    /// <summary>
    /// This EnumHelper is used to convert generic ENUMs to number values quickly and without creating objects on the heap
    /// </summary>
    public static class EnumHelper {

        public static NumericType GetNumericType<ENUM>() where ENUM : Enum {

            Type underlyingType = Enum.GetUnderlyingType(typeof(ENUM));

            NumericType type;

            if(underlyingType == typeof(ushort)) {
                type = NumericType.UShort;
            }
            else if(underlyingType == typeof(short)) {
                type = NumericType.Short;
            }
            else if(underlyingType == typeof(uint)) {
                type = NumericType.UInt;
            }
            else if(underlyingType == typeof(int)) {
                type = NumericType.Int;
            }
            else if(underlyingType == typeof(ulong)) {
                type = NumericType.ULong;
            }
            else if(underlyingType == typeof(long)) {
                type = NumericType.Long;
            }
            else if(underlyingType == typeof(sbyte)) {
                type = NumericType.SByte;
            }
            else if(underlyingType == typeof(byte)) {
                type = NumericType.Byte;
            }
            else {
                throw new ArgumentException($"Unknown {nameof(ENUM)} type. Type = {underlyingType.FullName}");
            }
            return type;
        }

        public static NumericType GetNumericType(Type enumType) {

            Type underlyingType = Enum.GetUnderlyingType(enumType);

            NumericType type;

            if(underlyingType == typeof(ushort)) {
                type = NumericType.UShort;
            }
            else if(underlyingType == typeof(short)) {
                type = NumericType.Short;
            }
            else if(underlyingType == typeof(uint)) {
                type = NumericType.UInt;
            }
            else if(underlyingType == typeof(int)) {
                type = NumericType.Int;
            }
            else if(underlyingType == typeof(ulong)) {
                type = NumericType.ULong;
            }
            else if(underlyingType == typeof(long)) {
                type = NumericType.Long;
            }
            else if(underlyingType == typeof(sbyte)) {
                type = NumericType.SByte;
            }
            else if(underlyingType == typeof(byte)) {
                type = NumericType.Byte;
            }
            else {
                throw new ArgumentException($"Unknown {nameof(enumType)} type. Type = {underlyingType.FullName}");
            }
            return type;
        }

        public static ushort UnsafeConvertToUShort<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, ushort>(ref enumValue);
        }
        public static short UnsafeConvertToShort<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, short>(ref enumValue);
        }
        public static uint UnsafeConvertToUInt<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, uint>(ref enumValue);
        }
        public static int UnsafeConvertToInt<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, int>(ref enumValue);
        }
        public static ulong UnsafeConvertToULong<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, ulong>(ref enumValue);
        }
        public static long UnsafeConvertToLong<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, long>(ref enumValue);
        }
        public static sbyte UnsafeConvertToSByte<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, sbyte>(ref enumValue);
        }
        public static byte UnsafeConvertToByte<ENUM>(ENUM enumValue) where ENUM : Enum {
            return Unsafe.As<ENUM, byte>(ref enumValue);
        }

        public static object GetEnumAsNumber<ENUM>(ENUM value) where ENUM : notnull, Enum {

            Type enumType = Enum.GetUnderlyingType(typeof(ENUM));

            if(enumType == typeof(byte)) {
                return ((byte)((object)value));
            }
            else if(enumType == typeof(sbyte)) {
                return ((sbyte)((object)value));
            }
            else if(enumType == typeof(short)) {
                return ((short)((object)value));
            }
            else if(enumType == typeof(ushort)) {
                return ((ushort)((object)value));
            }
            else if(enumType == typeof(int)) {
                return ((int)((object)value));
            }
            else if(enumType == typeof(uint)) {
                return ((uint)((object)value));
            }
            else if(enumType == typeof(long)) {
                return ((long)((object)value));
            }
            else if(enumType == typeof(ulong)) {
                return ((ulong)((object)value));
            }
            else {
                throw new ArgumentException($"{nameof(ENUM)} is an unknown type '{typeof(ENUM)}'");
            }
        }

        public static string GetEnumNumberAsString<ENUM>(ENUM value) where ENUM : notnull, Enum {

            Type enumType = Enum.GetUnderlyingType(typeof(ENUM));

            if(enumType == typeof(byte)) {
                return ((byte)((object)value)).ToString();
            }
            else if(enumType == typeof(sbyte)) {
                return ((sbyte)((object)value)).ToString();
            }
            else if(enumType == typeof(short)) {
                return ((short)((object)value)).ToString();
            }
            else if(enumType == typeof(ushort)) {
                return ((ushort)((object)value)).ToString();
            }
            else if(enumType == typeof(int)) {
                return ((int)((object)value)).ToString();
            }
            else if(enumType == typeof(uint)) {
                return ((uint)((object)value)).ToString();
            }
            else if(enumType == typeof(long)) {
                return ((long)((object)value)).ToString();
            }
            else if(enumType == typeof(ulong)) {
                return ((ulong)((object)value)).ToString();
            }
            else {
                throw new ArgumentException($"{nameof(ENUM)} is an unknown type '{typeof(ENUM)}'");
            }
        }
    }
}