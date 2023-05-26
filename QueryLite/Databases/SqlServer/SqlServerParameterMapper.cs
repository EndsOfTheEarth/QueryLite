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
using QueryLite.PreparedQuery;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QueryLite.Databases.SqlServer {

    public sealed class SqlServerParameterMapper : IParameterMapper {

        public CreateParameterDelegate GetCreateParameterDelegate(Type type) {

            if(type == typeof(Guid)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.UniqueIdentifier) {
                    Value = value
                };
            }
            else if(type == typeof(Guid?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.UniqueIdentifier) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(string)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.NVarChar) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(short)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.SmallInt) {
                    Value = value
                };
            }
            else if(type == typeof(short?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.SmallInt) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(int)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Int) {
                    Value = value
                };
            }
            else if(type == typeof(int?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Int) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(long)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.BigInt) {
                    Value = value
                };
            }
            else if(type == typeof(long?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.BigInt) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(bool)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.TinyInt) {
                    Value = value
                };
            }
            else if(type == typeof(bool?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.TinyInt) {
                    Value = value ?? DBNull.Value
                };
            }
            else if(type == typeof(Bit)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Bit) {
                    Value = ((Bit)value!).Value
                };
            }
            else if(type == typeof(Bit?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Bit) {
                    Value = value != null ? ((Bit?)value).Value.Value : DBNull.Value
                };
            }
            else if(type == typeof(decimal)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Decimal) {
                    Value = value
                };
            }
            else if(type == typeof(decimal?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Decimal) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(float)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Real) {
                    Value = value
                };
            }
            else if(type == typeof(float?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Real) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(double)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Float) {
                    Value = value
                };
            }
            else if(type == typeof(double?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Float) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(byte[])) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Binary) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(DateTime)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.DateTime) {
                    Value = value
                };
            }
            else if(type == typeof(DateTime?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.DateTime) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(DateTimeOffset)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.DateTimeOffset) {
                    Value = value
                };
            }
            else if(type == typeof(DateTimeOffset?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.DateTimeOffset) {
                    Value = value ?? DBNull.Value
                };
            }

            else if(type == typeof(DateOnly)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Date) {
                    Value = ((DateOnly)value!).ToDateTime(TimeOnly.MinValue)
                };
            }
            else if(type == typeof(DateOnly?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Date) {
                    Value = value != null ? ((DateOnly?)value).Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value
                };
            }

            else if(type == typeof(TimeOnly)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Time) {
                    Value = ((TimeOnly)value!).ToTimeSpan()
                };
            }
            else if(type == typeof(TimeOnly?)) {
                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Time) {
                    Value = value != null ? ((TimeOnly?)value).Value.ToTimeSpan() : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IGuidType))) {

                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.UniqueIdentifier) {
                    Value = value != null ? ((IGuidType)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IStringType))) {

                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.NVarChar) {
                    Value = value != null ? ((IStringType)value).Value : DBNull.Value
                };
            }


            else if(type.IsAssignableTo(typeof(IInt16Type))) {

                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.SmallInt) {
                    Value = value != null ? ((IInt16Type)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IInt32Type))) {

                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Int) {
                    Value = value != null ? ((IInt32Type)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IInt64Type))) {

                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.BigInt) {
                    Value = value != null ? ((IInt64Type)value).Value : DBNull.Value
                };
            }

            else if(type.IsAssignableTo(typeof(IBoolType))) {

                return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.TinyInt) {
                    Value = value != null ? ((IBoolType)value).Value : DBNull.Value
                };
            }
            else if(type.IsEnum) {

                NumericType integerType = EnumHelper.GetNumericType(type);

                if(integerType == NumericType.UShort) {
                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.SmallInt) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.Short) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.SmallInt) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.UInt) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Int) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.Int) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.Int) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.ULong) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.BigInt) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.Long) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.BigInt) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.SByte) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.TinyInt) {
                        Value = value ?? DBNull.Value
                    };
                }
                else if(integerType == NumericType.Byte) {

                    return (string name, object? value) => new SqlParameter(parameterName: name, dbType: SqlDbType.TinyInt) {
                        Value = value ?? DBNull.Value
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


        //public static void SetNullParameterValue<ENUM>(SqlParameter parameter, NumericType integerType) where ENUM : Enum {

        //    if(integerType == NumericType.UShort) {
        //        parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.Short) {
        //        parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.UInt) {
        //        parameter.SqlDbType = System.Data.SqlDbType.Int;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.Int) {
        //        parameter.SqlDbType = System.Data.SqlDbType.Int;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.ULong) {
        //        parameter.SqlDbType = System.Data.SqlDbType.BigInt;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.Long) {
        //        parameter.SqlDbType = System.Data.SqlDbType.BigInt;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.SByte) {
        //        parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else if(integerType == NumericType.Byte) {
        //        parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
        //        parameter.Value = DBNull.Value;
        //    }
        //    else {
        //        throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
        //    }
        //}

        //public static void SetParameterValue<ENUM>(ENUM value, SqlParameter parameter, NumericType integerType) where ENUM : Enum {

        //    if(integerType == NumericType.UShort) {
        //        parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
        //        parameter.Value = EnumHelper.UnsafeConvertToUShort(value);
        //    }
        //    else if(integerType == NumericType.Short) {
        //        parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
        //        parameter.Value = EnumHelper.UnsafeConvertToShort(value);
        //    }
        //    else if(integerType == NumericType.UInt) {
        //        parameter.SqlDbType = System.Data.SqlDbType.Int;
        //        parameter.Value = EnumHelper.UnsafeConvertToUShort(value);
        //    }
        //    else if(integerType == NumericType.Int) {
        //        parameter.SqlDbType = System.Data.SqlDbType.Int;
        //        parameter.Value = EnumHelper.UnsafeConvertToShort(value);
        //    }
        //    else if(integerType == NumericType.ULong) {
        //        parameter.SqlDbType = System.Data.SqlDbType.BigInt;
        //        parameter.Value = EnumHelper.UnsafeConvertToULong(value);
        //    }
        //    else if(integerType == NumericType.Long) {
        //        parameter.SqlDbType = System.Data.SqlDbType.BigInt;
        //        parameter.Value = EnumHelper.UnsafeConvertToLong(value);
        //    }
        //    else if(integerType == NumericType.SByte) {
        //        parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
        //        parameter.Value = EnumHelper.UnsafeConvertToULong(value);
        //    }
        //    else if(integerType == NumericType.Byte) {
        //        parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
        //        parameter.Value = EnumHelper.UnsafeConvertToLong(value);
        //    }
        //    else {
        //        throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
        //    }
        //}
    }
}