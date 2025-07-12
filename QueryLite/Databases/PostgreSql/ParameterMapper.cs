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
using Npgsql;
using NpgsqlTypes;
using QueryLite.Utility;
using System;
using System.Collections.Generic;
#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace QueryLite.Databases.SqlServer {

    public sealed class PostgreSqlParameterMapper : IPreparedParameterMapper {

#if NET9_0_OR_GREATER
        private static readonly Lock _lock = new Lock();
#else
        private static readonly object _lock = new object();
#endif

        private readonly static Dictionary<Type, CreateParameterDelegate> CreateParameterDelegateLookup = new Dictionary<Type, CreateParameterDelegate>() {
            { typeof(Guid), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) { Value = value } },
            { typeof(Guid?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) { Value = value ?? DBNull.Value } },
            { typeof(string), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Varchar) { Value = value ?? DBNull.Value } },
            { typeof(short), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) { Value = value } },
            { typeof(short?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) { Value = value ?? DBNull.Value } },
            { typeof(int), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) { Value = value } },
            { typeof(int?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) { Value = value ?? DBNull.Value } },
            { typeof(long), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) { Value = value } },
            { typeof(long?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) { Value = value ?? DBNull.Value } },
            { typeof(bool), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) { Value = value } },
            { typeof(bool?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) { Value = value ?? DBNull.Value } },
            { typeof(Bit), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) { Value = ((Bit)value!).Value } },
            { typeof(Bit?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) { Value = value != null ? ((Bit?)value).Value.Value : DBNull.Value } },
            { typeof(decimal), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Numeric) { Value = value } },
            { typeof(decimal?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Numeric) { Value = value ?? DBNull.Value } },
            { typeof(float), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Real) { Value = value } },
            { typeof(float?),  (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Real) { Value = value ?? DBNull.Value } },
            { typeof(double), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Double) { Value = value } },
            { typeof(double?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Double) { Value = value ?? DBNull.Value } },
            { typeof(byte[]), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bytea) { Value = value ?? DBNull.Value } },
            { typeof(DateTime), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Timestamp) { Value = value } },
            { typeof(DateTime?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Timestamp) { Value = value ?? DBNull.Value } },
            { typeof(DateTimeOffset), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.TimestampTz) { Value = value != null ? new DateTimeOffset(((DateTimeOffset)value).UtcDateTime) : DBNull.Value } },
            { typeof(DateTimeOffset?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.TimestampTz) { Value = value != null ? new DateTimeOffset(((DateTimeOffset)value).UtcDateTime) : DBNull.Value } },
            { typeof(DateOnly), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Date) { Value = ((DateOnly)value!).ToDateTime(TimeOnly.MinValue) } },
            { typeof(DateOnly?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Date) { Value = value != null ? ((DateOnly?)value).Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value } },
            { typeof(TimeOnly), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Time) { Value = ((TimeOnly)value!).ToTimeSpan() } },
            { typeof(TimeOnly?), (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Time) { Value = value != null ? ((TimeOnly?)value).Value.ToTimeSpan() : DBNull.Value } }
        };

        private static CreateParameterDelegate AddParameterDelegate(Type type, CreateParameterDelegate @delegate) {

            lock(_lock) {
                CreateParameterDelegateLookup.TryAdd(type, @delegate);
            }
            return @delegate;
        }

        public CreateParameterDelegate GetCreateParameterDelegate(Type type) {

            if(CreateParameterDelegateLookup.TryGetValue(type, out CreateParameterDelegate? @delegate)) {
                return @delegate!;
            }

            /*
             * Map Custom Types
             */
            if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Uuid) {
                    Value = value != null ? ((IValue<Guid>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<short>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Smallint) {
                    Value = value != null ? ((IValue<short>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<int>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Integer) {
                    Value = value != null ? ((IValue<int>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<long>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Bigint) {
                    Value = value != null ? ((IValue<long>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<string>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Varchar) {
                    Value = value != null ? ((IValue<string>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<bool>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Boolean) {
                    Value = value != null ? ((IValue<bool>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<decimal>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Numeric) {
                    Value = value != null ? ((IValue<decimal>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<DateTime>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Timestamp) {
                    Value = value != null ? ((IValue<DateTime>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.TimestampTz) {
                    Value = value != null ? new DateTimeOffset(((IValue<DateTimeOffset>)value).Value.UtcDateTime) : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<DateOnly>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Date) {
                    Value = value != null ? ((IValue<DateOnly>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Time) {
                    Value = value != null ? ((IValue<TimeOnly>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<float>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Real) {
                    Value = value != null ? ((IValue<float>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<double>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Double) {
                    Value = value != null ? ((IValue<double>)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IValue<Bit>))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Boolean) {
                    Value = value != null ? ((IValue<Bit>)value).Value : DBNull.Value
                });
            }

            Type? underlyingType = Nullable.GetUnderlyingType(type);

            /*
             * Map Nullable Custom Types
             */
            if(underlyingType != null) {

                if(underlyingType.IsAssignableTo(typeof(IValue<Guid>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Uuid) {
                        Value = value != null ? ((IValue<Guid>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<short>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Smallint) {
                        Value = value != null ? ((IValue<short>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<int>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Integer) {
                        Value = value != null ? ((IValue<int>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<long>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Bigint) {
                        Value = value != null ? ((IValue<long>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<string>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Varchar) {
                        Value = value != null ? ((IValue<string>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<bool>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Boolean) {
                        Value = value != null ? ((IValue<bool>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<decimal>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Numeric) {
                        Value = value != null ? ((IValue<decimal>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateTime>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Timestamp) {
                        Value = value != null ? ((IValue<DateTime>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.TimestampTz) {
                        Value = value != null ? new DateTimeOffset(((IValue<DateTimeOffset>)value).Value.UtcDateTime) : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateOnly>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Date) {
                        Value = value != null ? ((IValue<DateOnly>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Time) {
                        Value = value != null ? ((IValue<TimeOnly>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<float>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Real) {
                        Value = value != null ? ((IValue<float>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<double>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Double) {
                        Value = value != null ? ((IValue<double>)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<Bit>))) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, NpgsqlDbType.Boolean) {
                        Value = value != null ? ((IValue<Bit>)value).Value : DBNull.Value
                    });
                }
            }

            /*
             * Map Key Types
             */
            if(type.IsAssignableTo(typeof(IGuidType))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) {
                    Value = value != null ? ((IGuidType)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Varchar) {
                    Value = value != null ? ((IStringType)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                    Value = value != null ? ((IInt16Type)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                    Value = value != null ? ((IInt32Type)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                    Value = value != null ? ((IInt64Type)value).Value : DBNull.Value
                });
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                    Value = value != null ? ((IBoolType)value).Value : DBNull.Value
                });
            }

            /*
             * Map Nullable Key Types
             */
            if(underlyingType != null) {

                if(underlyingType.IsAssignableTo(typeof(IGuidType))) {
                    return AddParameterDelegate(underlyingType, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Uuid) {
                        Value = value != null ? ((IGuidType)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IStringType))) {
                    return AddParameterDelegate(underlyingType, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Varchar) {
                        Value = value != null ? ((IStringType)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IInt16Type))) {
                    return AddParameterDelegate(underlyingType, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? ((IInt16Type)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IInt32Type))) {
                    return AddParameterDelegate(underlyingType, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                        Value = value != null ? ((IInt32Type)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IInt64Type))) {
                    return AddParameterDelegate(underlyingType, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                        Value = value != null ? ((IInt64Type)value).Value : DBNull.Value
                    });
                }
                if(underlyingType.IsAssignableTo(typeof(IBoolType))) {
                    return AddParameterDelegate(underlyingType, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Boolean) {
                        Value = value != null ? ((IBoolType)value).Value : DBNull.Value
                    });
                }
            }

            Type? enumType = null;

            if(type.IsEnum) {
                enumType = type;
            }
            else {  //Check to see if this is a nullable enum type

                if(underlyingType != null && underlyingType.IsEnum) {
                    enumType = underlyingType;
                }
            }

            if(enumType != null) {

                NumericType integerType = EnumHelper.GetNumericType(enumType);

                if(integerType == NumericType.UShort) {
                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? (short)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.Short) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? (short)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.UInt) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                        Value = value != null ? (int)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.Int) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Integer) {
                        Value = value != null ? (int)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.ULong) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                        Value = value != null ? (long)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.Long) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bigint) {
                        Value = value != null ? (long)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.SByte) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Bit) {
                        Value = value != null ? (short)value : DBNull.Value
                    });
                }
                else if(integerType == NumericType.Byte) {

                    return AddParameterDelegate(type, (name, value) => new NpgsqlParameter(parameterName: name, parameterType: NpgsqlDbType.Smallint) {
                        Value = value != null ? (byte)value : DBNull.Value
                    });
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