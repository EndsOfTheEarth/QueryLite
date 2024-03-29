﻿/*
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
using Npgsql;
using NpgsqlTypes;
using QueryLite.Utility;
using System;

namespace QueryLite.Databases.SqlServer {

    public sealed class PostgreSqlParameterMapper : IPreparedParameterMapper {

        public CreateParameterDelegate GetCreateParameterDelegate(Type type) {

            if(type == typeof(Guid)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) {
                    Value = value
                };
            }
            else if(type == typeof(Guid?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(string)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Varchar) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(short)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                    Value = value
                };
            }
            else if(type == typeof(short?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(int)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                    Value = value
                };
            }
            else if(type == typeof(int?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(long)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                    Value = value
                };
            }
            else if(type == typeof(long?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(bool)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                    Value = value
                };
            }
            else if(type == typeof(bool?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(Bit)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                    Value = ((Bit)value!).Value
                };
            }
            else if(type == typeof(Bit?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                    Value = value != null ? ((Bit?)value).Value.Value : DBNull.Value
                };
            }
            else if(type == typeof(decimal)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Numeric) {
                    Value = value
                };
            }
            else if(type == typeof(decimal?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Numeric) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(float)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Real) {
                    Value = value
                };
            }
            else if(type == typeof(float?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Real) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(double)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Double) {
                    Value = value
                };
            }
            else if(type == typeof(double?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Double) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(byte[])) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bytea) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(DateTime)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Timestamp) {
                    Value = value
                };
            }
            else if(type == typeof(DateTime?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Timestamp) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(DateTimeOffset)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.TimestampTz) {
                    Value = value != null ? new DateTimeOffset(((DateTimeOffset)value).UtcDateTime) : DBNull.Value
                };
            }
            else if(type == typeof(DateTimeOffset?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.TimestampTz) {
                    Value = value != null ? new DateTimeOffset(((DateTimeOffset)value).UtcDateTime) : DBNull.Value
                };
            }

            else if(type == typeof(DateOnly)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Date) {
                    Value = ((DateOnly)value!).ToDateTime(TimeOnly.MinValue)
                };
            }
            else if(type == typeof(DateOnly?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Date) {
                    Value = value != null ? ((DateOnly?)value).Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value
                };
            }

            else if(type == typeof(TimeOnly)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Time) {
                    Value = ((TimeOnly)value!).ToTimeSpan()
                };
            }
            else if(type == typeof(TimeOnly?)) {
                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Time) {
                    Value = value != null ? ((TimeOnly?)value).Value.ToTimeSpan() : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IGuidType))) {

                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) {
                    Value = value != null ? ((IGuidType)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IStringType))) {

                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Varchar) {
                    Value = value != null ? ((IStringType)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IInt16Type))) {

                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                    Value = value != null ? ((IInt16Type)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IInt32Type))) {

                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                    Value = value != null ? ((IInt32Type)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IInt64Type))) {

                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                    Value = value != null ? ((IInt64Type)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IBoolType))) {

                return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                    Value = value != null ? ((IBoolType)value).Value : DBNull.Value
                };
            }

            Type? enumType = null;

            if(type.IsEnum) {
                enumType = type;
            }
            else {  //Check to see if this is a nullable enum type

                Type? underlyingType = Nullable.GetUnderlyingType(type);

                if(underlyingType != null && underlyingType.IsEnum) {
                    enumType = underlyingType;
                }
            }

            if(enumType != null) {

                NumericType integerType = EnumHelper.GetNumericType(enumType);

                if(integerType == NumericType.UShort) {
                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? (short)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.Short) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? (short)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.UInt) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                        Value = value != null ? (int)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.Int) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                        Value = value != null ? (int)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.ULong) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                        Value = value != null ? (long)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.Long) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                        Value = value != null ? (long)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.SByte) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bit) {
                        Value = value != null ? (short)value : DBNull.Value
                    };
                }
                else if(integerType == NumericType.Byte) {

                    return (string name, object? value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? (byte)value : DBNull.Value
                    };
                }
                else {
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
                }
            }
            else {
                throw new Exception($"Unsupported Type: '{type.FullName}' type);");
            }
        }
    }
}