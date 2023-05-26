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
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace QueryLite.PreparedQuery {

    public interface IParameter<PARAMETERS> {

        string Name { get; set; }
        Type GetValueType();
        object? GetValue(PARAMETERS parameters);
    }

    public interface IParameter<PARAMETERS, TYPE> : IParameter<PARAMETERS> {

    }

    public class Parameter<PARAMETERS, TYPE> : IParameter<PARAMETERS, TYPE> {

        public string Name { get; set; } = string.Empty;

        public Type GetValueType() => typeof(TYPE);

        private Func<PARAMETERS, TYPE> _getValueFunc;

        public Parameter(Func<PARAMETERS, TYPE> function) {
            _getValueFunc = (parameters) => function(parameters);
        }
        public object? GetValue(PARAMETERS parameters) {
            return _getValueFunc(parameters);
        }
    }

    public interface IParameterMapper {
        CreateParameterDelegate GetCreateParameterDelegate(Type type);
    }

    public delegate DbParameter CreateParameterDelegate(string name, object? value);

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
    }
}