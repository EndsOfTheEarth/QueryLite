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
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace QueryLite.PreparedQuery {

    
    public interface IParameter<PARAMETERS> {

        string? Name { get; set; }

        public DbParameter CreateParameter(PARAMETERS item, DbCommand command);
    }

    public static class ParameterFactory {

        public static IParameter<Guid, ITEM> CreateParameter<ITEM>(Func<ITEM, Guid> func) {
            return new GuidParameter<ITEM>(func);
        }
        public static IParameter<Guid?, ITEM> CreateParameter<ITEM>(Func<ITEM, Guid?> func) {
            return new NullableGuidParameter<ITEM>(func);
        }
        public static IParameter<string, ITEM> CreateParameter<ITEM>(Func<ITEM, string> func) {
            return new StringParameter<ITEM>(func);
        }

        public static IParameter<short, ITEM> CreateParameter<ITEM>(Func<ITEM, short> func) {
            return new ShortParameter<ITEM>(func);
        }
        public static IParameter<short?, ITEM> CreateParameter<ITEM>(Func<ITEM, short?> func) {
            return new NullableShortParameter<ITEM>(func);
        }

        public static IParameter<int, ITEM> CreateParameter<ITEM>(Func<ITEM, int> func) {
            return new IntParameter<ITEM>(func);
        }
        public static IParameter<int?, ITEM> CreateParameter<ITEM>(Func<ITEM, int?> func) {
            return new NullableIntParameter<ITEM>(func);
        }

        public static IParameter<long, ITEM> CreateParameter<ITEM>(Func<ITEM, long> func) {
            return new LongParameter<ITEM>(func);
        }
        public static IParameter<long?, ITEM> CreateParameter<ITEM>(Func<ITEM, long?> func) {
            return new NullableLongParameter<ITEM>(func);
        }

        public static IParameter<bool, ITEM> CreateParameter<ITEM>(Func<ITEM, bool> func) {
            return new BoolParameter<ITEM>(func);
        }
        public static IParameter<bool?, ITEM> CreateParameter<ITEM>(Func<ITEM, bool?> func) {
            return new NullableBoolParameter<ITEM>(func);
        }

        public static IParameter<Bit, ITEM> CreateParameter<ITEM>(Func<ITEM, Bit> func) {
            return new BitParameter<ITEM>(func);
        }
        public static IParameter<Bit?, ITEM> CreateParameter<ITEM>(Func<ITEM, Bit?> func) {
            return new NullableBitParameter<ITEM>(func);
        }

        public static IParameter<decimal, ITEM> CreateParameter<ITEM>(Func<ITEM, decimal> func) {
            return new DecimalParameter<ITEM>(func);
        }
        public static IParameter<decimal?, ITEM> CreateParameter<ITEM>(Func<ITEM, decimal?> func) {
            return new NullableDecimalParameter<ITEM>(func);
        }

        public static IParameter<float, ITEM> CreateParameter<ITEM>(Func<ITEM, float> func) {
            return new FloatParameter<ITEM>(func);
        }
        public static IParameter<float?, ITEM> CreateParameter<ITEM>(Func<ITEM, float?> func) {
            return new NullableFloatParameter<ITEM>(func);
        }

        public static IParameter<double, ITEM> CreateParameter<ITEM>(Func<ITEM, double> func) {
            return new DoubleParameter<ITEM>(func);
        }
        public static IParameter<double?, ITEM> CreateParameter<ITEM>(Func<ITEM, double?> func) {
            return new NullableDoubleParameter<ITEM>(func);
        }

        //public static IParameter<byte[], ITEM> CreateParameter<ITEM>(Func<ITEM, byte[]> func) {
        //    return new ByteArrayParameter<ITEM>(func);
        //}
        public static IParameter<byte[]?, ITEM> CreateParameter<ITEM>(Func<ITEM, byte[]?> func) {
            return new NullableByteArrayParameter<ITEM>(func);
        }

        public static IParameter<DateTime, ITEM> CreateParameter<ITEM>(Func<ITEM, DateTime> func) {
            return new DateTimeParameter<ITEM>(func);
        }
        public static IParameter<DateTime?, ITEM> CreateParameter<ITEM>(Func<ITEM, DateTime?> func) {
            return new NullableDateTimeParameter<ITEM>(func);
        }

        public static IParameter<DateTimeOffset, ITEM> CreateParameter<ITEM>(Func<ITEM, DateTimeOffset> func) {
            return new DateTimeOffsetParameter<ITEM>(func);
        }
        public static IParameter<DateTimeOffset?, ITEM> CreateParameter<ITEM>(Func<ITEM, DateTimeOffset?> func) {
            return new NullableDateTimeOffsetParameter<ITEM>(func);
        }

        public static IParameter<DateOnly, ITEM> CreateParameter<ITEM>(Func<ITEM, DateOnly> func) {
            return new DateOnlyParameter<ITEM>(func);
        }
        public static IParameter<DateOnly?, ITEM> CreateParameter<ITEM>(Func<ITEM, DateOnly?> func) {
            return new NullableDateOnlyParameter<ITEM>(func);
        }

        public static IParameter<TimeOnly, ITEM> CreateParameter<ITEM>(Func<ITEM, TimeOnly> func) {
            return new TimeOnlyParameter<ITEM>(func);
        }
        public static IParameter<TimeOnly?, ITEM> CreateParameter<ITEM>(Func<ITEM, TimeOnly?> func) {
            return new NullableTimeOnlyParameter<ITEM>(func);
        }

        public static IParameter<ENUM, ITEM> CreateParameter<ITEM, ENUM>(Func<ITEM, ENUM> func) where ENUM : struct, Enum {
            return new EnumParameter<ITEM, ENUM>(func);
        }
        public static IParameter<ENUM?, ITEM> CreateParameter<ITEM, ENUM>(Func<ITEM, ENUM?> func) where ENUM : struct, Enum {
            return new NullableEnumParameter<ITEM, ENUM>(func);
        }


        public static IParameter<GuidKey<TYPE>, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, GuidKey<TYPE>> func) {
            return new GuidKeyParameter<ITEM, TYPE>(func);
        }
        public static IParameter<GuidKey<TYPE>?, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, GuidKey<TYPE>?> func) {
            return new NullableGuidKeyParameter<ITEM, TYPE>(func);
        }

        public static IParameter<StringKey<TYPE>, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, StringKey<TYPE>> func) {
            return new StringKeyParameter<ITEM, TYPE>(func);
        }
        public static IParameter<StringKey<TYPE>?, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, StringKey<TYPE>?> func) {
            return new NullableStringKeyParameter<ITEM, TYPE>(func);
        }


        public static IParameter<ShortKey<TYPE>, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, ShortKey<TYPE>> func) {
            return new ShortKeyParameter<ITEM, TYPE>(func);
        }
        public static IParameter<ShortKey<TYPE>?, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, ShortKey<TYPE>?> func) {
            return new NullableShortKeyParameter<ITEM, TYPE>(func);
        }

        public static IParameter<IntKey<TYPE>, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, IntKey<TYPE>> func) {
            return new IntKeyParameter<ITEM, TYPE>(func);
        }
        public static IParameter<IntKey<TYPE>?, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, IntKey<TYPE>?> func) {
            return new NullableIntKeyParameter<ITEM, TYPE>(func);
        }

        public static IParameter<LongKey<TYPE>, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, LongKey<TYPE>> func) {
            return new LongKeyParameter<ITEM, TYPE>(func);
        }
        public static IParameter<LongKey<TYPE>?, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, LongKey<TYPE>?> func) {
            return new NullableLongKeyParameter<ITEM, TYPE>(func);
        }

        public static IParameter<BoolValue<TYPE>, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, BoolValue<TYPE>> func) {
            return new BoolValueParameter<ITEM, TYPE>(func);
        }
        public static IParameter<BoolValue<TYPE>?, ITEM> CreateParameter<ITEM, TYPE>(Func<ITEM, BoolValue<TYPE>?> func) {
            return new NullableBoolValueParameter<ITEM, TYPE>(func);
        }
    }

    public interface IParameter<VALUE, ITEM> : IParameter<ITEM> {

    }

    public class GuidParameter<ITEM> : IParameter<Guid, ITEM> {

        public GuidParameter(Func<ITEM, Guid> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Guid> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableGuidParameter<ITEM> : IParameter<Guid?, ITEM> {

        public NullableGuidParameter(Func<ITEM, Guid?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Guid?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;

            Guid? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class StringParameter<ITEM> : IParameter<string, ITEM> {

        public StringParameter(Func<ITEM, string> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, string> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.NVarChar;

            string value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class ShortParameter<ITEM> : IParameter<short, ITEM> {

        public ShortParameter(Func<ITEM, short> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, short> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableShortParameter<ITEM> : IParameter<short?, ITEM> {

        public NullableShortParameter(Func<ITEM, short?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, short?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.SmallInt;

            short? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class IntParameter<ITEM> : IParameter<int, ITEM> {

        public IntParameter(Func<ITEM, int> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, int> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Int;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableIntParameter<ITEM> : IParameter<int?, ITEM> {

        public NullableIntParameter(Func<ITEM, int?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, int?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Int;

            int? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class LongParameter<ITEM> : IParameter<long, ITEM> {

        public LongParameter(Func<ITEM, long> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, long> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableLongParameter<ITEM> : IParameter<long?, ITEM> {

        public NullableLongParameter(Func<ITEM, long?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, long?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;

            long? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class BoolParameter<ITEM> : IParameter<bool, ITEM> {

        public BoolParameter(Func<ITEM, bool> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, bool> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.TinyInt;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableBoolParameter<ITEM> : IParameter<bool?, ITEM> {

        public NullableBoolParameter(Func<ITEM, bool?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, bool?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.TinyInt;

            bool? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class BitParameter<ITEM> : IParameter<Bit, ITEM> {

        public BitParameter(Func<ITEM, Bit> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Bit> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableBitParameter<ITEM> : IParameter<Bit?, ITEM> {

        public NullableBitParameter(Func<ITEM, Bit?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Bit?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Bit;

            Bit? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class DecimalParameter<ITEM> : IParameter<decimal, ITEM> {

        public DecimalParameter(Func<ITEM, decimal> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, decimal> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Decimal;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableDecimalParameter<ITEM> : IParameter<decimal?, ITEM> {

        public NullableDecimalParameter(Func<ITEM, decimal?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, decimal?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Decimal;

            decimal? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class FloatParameter<ITEM> : IParameter<float, ITEM> {

        public FloatParameter(Func<ITEM, float> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, float> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Real;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableFloatParameter<ITEM> : IParameter<float?, ITEM> {

        public NullableFloatParameter(Func<ITEM, float?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, float?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Real;

            float? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class DoubleParameter<ITEM> : IParameter<double, ITEM> {

        public DoubleParameter(Func<ITEM, double> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, double> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Float;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableDoubleParameter<ITEM> : IParameter<double?, ITEM> {

        public NullableDoubleParameter(Func<ITEM, double?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, double?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Float;

            double? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    //public class ByteArrayParameter<ITEM> : IParameter<byte[], ITEM> {

    //    public ByteArrayParameter(Func<ITEM, byte[]> func) {
    //        Func = func;
    //    }
    //    public string? Name { get; set; }
    //    public Func<ITEM, byte[]> Func { get; private set; }

    //    public DbParameter CreateParameter(ITEM item, DbCommand command) {

    //        SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

    //        sqlParameter.SqlDbType = System.Data.SqlDbType.Binary;
    //        sqlParameter.Value = Func(item);
    //    }
    //}

    public class NullableByteArrayParameter<ITEM> : IParameter<byte[]?, ITEM> {

        public NullableByteArrayParameter(Func<ITEM, byte[]?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, byte[]?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Binary;

            byte[]? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class DateTimeParameter<ITEM> : IParameter<DateTime, ITEM> {

        public DateTimeParameter(Func<ITEM, DateTime> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTime> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTime;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableDateTimeParameter<ITEM> : IParameter<DateTime?, ITEM> {

        public NullableDateTimeParameter(Func<ITEM, DateTime?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTime?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTime;

            DateTime? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class DateTimeOffsetParameter<ITEM> : IParameter<DateTimeOffset, ITEM> {

        public DateTimeOffsetParameter(Func<ITEM, DateTimeOffset> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTimeOffset> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTimeOffset;
            sqlParameter.Value = Func(item);

            return sqlParameter;
        }
    }

    public class NullableDateTimeOffsetParameter<ITEM> : IParameter<DateTimeOffset?, ITEM> {

        public NullableDateTimeOffsetParameter(Func<ITEM, DateTimeOffset?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTimeOffset?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTimeOffset;

            DateTimeOffset? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class DateOnlyParameter<ITEM> : IParameter<DateOnly, ITEM> {

        public DateOnlyParameter(Func<ITEM, DateOnly> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateOnly> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Date;
            sqlParameter.Value = Func(item).ToDateTime(TimeOnly.MinValue);

            return sqlParameter;
        }
    }

    public class NullableDateOnlyParameter<ITEM> : IParameter<DateOnly?, ITEM> {

        public NullableDateOnlyParameter(Func<ITEM, DateOnly?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateOnly?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Date;

            DateOnly? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value.ToDateTime(TimeOnly.MinValue);
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class TimeOnlyParameter<ITEM> : IParameter<TimeOnly, ITEM> {

        public TimeOnlyParameter(Func<ITEM, TimeOnly> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, TimeOnly> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Time;
            sqlParameter.Value = Func(item).ToTimeSpan();

            return sqlParameter;
        }
    }

    public class NullableTimeOnlyParameter<ITEM> : IParameter<TimeOnly?, ITEM> {

        public NullableTimeOnlyParameter(Func<ITEM, TimeOnly?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, TimeOnly?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Time;

            TimeOnly? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value.ToTimeSpan();
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
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

        public static void SetNullParameterValue<ENUM>(SqlParameter parameter, NumericType integerType) where ENUM : Enum {

            if(integerType == NumericType.UShort) {
                parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.Short) {
                parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.UInt) {
                parameter.SqlDbType = System.Data.SqlDbType.Int;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.Int) {
                parameter.SqlDbType = System.Data.SqlDbType.Int;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.ULong) {
                parameter.SqlDbType = System.Data.SqlDbType.BigInt;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.Long) {
                parameter.SqlDbType = System.Data.SqlDbType.BigInt;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.SByte) {
                parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
                parameter.Value = DBNull.Value;
            }
            else if(integerType == NumericType.Byte) {
                parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
                parameter.Value = DBNull.Value;
            }
            else {
                throw new Exception($"Unkonwn {nameof(integerType)} type. Value = '{integerType}');");
            }
        }

        public static void SetParameterValue<ENUM>(ENUM value, SqlParameter parameter, NumericType integerType) where ENUM : Enum {

            if(integerType == NumericType.UShort) {
                parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                parameter.Value = EnumHelper.UnsafeConvertToUShort(value);
            }
            else if(integerType == NumericType.Short) {
                parameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                parameter.Value = EnumHelper.UnsafeConvertToShort(value);
            }
            else if(integerType == NumericType.UInt) {
                parameter.SqlDbType = System.Data.SqlDbType.Int;
                parameter.Value = EnumHelper.UnsafeConvertToUShort(value);
            }
            else if(integerType == NumericType.Int) {
                parameter.SqlDbType = System.Data.SqlDbType.Int;
                parameter.Value = EnumHelper.UnsafeConvertToShort(value);
            }
            else if(integerType == NumericType.ULong) {
                parameter.SqlDbType = System.Data.SqlDbType.BigInt;
                parameter.Value = EnumHelper.UnsafeConvertToULong(value);
            }
            else if(integerType == NumericType.Long) {
                parameter.SqlDbType = System.Data.SqlDbType.BigInt;
                parameter.Value = EnumHelper.UnsafeConvertToLong(value);
            }
            else if(integerType == NumericType.SByte) {
                parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
                parameter.Value = EnumHelper.UnsafeConvertToULong(value);
            }
            else if(integerType == NumericType.Byte) {
                parameter.SqlDbType = System.Data.SqlDbType.TinyInt;
                parameter.Value = EnumHelper.UnsafeConvertToLong(value);
            }
            else {
                throw new Exception($"Unkonwn {nameof(integerType)} type. Value = '{integerType}');");
            }
        }
    }

    public class EnumParameter<ITEM, ENUM> : IParameter<ENUM, ITEM>  where ENUM : struct, Enum {

        public EnumParameter(Func<ITEM, ENUM> func) {
            Func = func;
            _IntegerType = EnumHelper.GetNumericType<ENUM>();
        }
        private NumericType _IntegerType;
        public string? Name { get; set; }
        public Func<ITEM, ENUM> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;

            EnumHelper.SetParameterValue<ENUM>(value: Func(item), parameter: sqlParameter, _IntegerType);

            return sqlParameter;
        }
    }

    public class NullableEnumParameter<ITEM, ENUM> : IParameter<ENUM?, ITEM> where ENUM : struct, Enum {

        public NullableEnumParameter(Func<ITEM, ENUM?> func) {
            Func = func;
            _IntegerType = EnumHelper.GetNumericType<ENUM>();
        }
        private NumericType _IntegerType;
        public string? Name { get; set; }
        public Func<ITEM, ENUM?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;

            ENUM? value = Func(item);

            if(value != null) {
                EnumHelper.SetParameterValue<ENUM>(value: value.Value, parameter: sqlParameter, _IntegerType);
            }
            else {
                EnumHelper.SetNullParameterValue<ENUM>(parameter: sqlParameter, _IntegerType);
            }
            return sqlParameter;
        }
    }

    public class GuidKeyParameter<ITEM, TYPE> : IParameter<GuidKey<TYPE>, ITEM> {

        public GuidKeyParameter(Func<ITEM, GuidKey<TYPE>> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, GuidKey<TYPE>> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableGuidKeyParameter<ITEM, TYPE> : IParameter<GuidKey<TYPE>?, ITEM> {

        public NullableGuidKeyParameter(Func<ITEM, GuidKey<TYPE>?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, GuidKey<TYPE>?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;

            GuidKey<TYPE>? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class StringKeyParameter<ITEM, TYPE> : IParameter<StringKey<TYPE>, ITEM> {

        public StringKeyParameter(Func<ITEM, StringKey<TYPE>> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, StringKey<TYPE>> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.NVarChar;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableStringKeyParameter<ITEM, TYPE> : IParameter<StringKey<TYPE>?, ITEM> {

        public NullableStringKeyParameter(Func<ITEM, StringKey<TYPE>?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, StringKey<TYPE>?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.NVarChar;

            StringKey<TYPE>? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class ShortKeyParameter<ITEM, TYPE> : IParameter<ShortKey<TYPE>, ITEM> {

        public ShortKeyParameter(Func<ITEM, ShortKey<TYPE>> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, ShortKey<TYPE>> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableShortKeyParameter<ITEM, TYPE> : IParameter<ShortKey<TYPE>?, ITEM> {

        public NullableShortKeyParameter(Func<ITEM, ShortKey<TYPE>?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, ShortKey<TYPE>?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.SmallInt;

            ShortKey<TYPE>? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class IntKeyParameter<ITEM, TYPE> : IParameter<IntKey<TYPE>, ITEM> {

        public IntKeyParameter(Func<ITEM, IntKey<TYPE>> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, IntKey<TYPE>> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Int;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableIntKeyParameter<ITEM, TYPE> : IParameter<IntKey<TYPE>?, ITEM> {

        public NullableIntKeyParameter(Func<ITEM, IntKey<TYPE>?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, IntKey<TYPE>?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.Int;

            IntKey<TYPE>? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class LongKeyParameter<ITEM, TYPE> : IParameter<LongKey<TYPE>, ITEM> {

        public LongKeyParameter(Func<ITEM, LongKey<TYPE>> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, LongKey<TYPE>> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableLongKeyParameter<ITEM, TYPE> : IParameter<LongKey<TYPE>?, ITEM> {

        public NullableLongKeyParameter(Func<ITEM, LongKey<TYPE>?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, LongKey<TYPE>?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;

            LongKey<TYPE>? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }

    public class BoolValueParameter<ITEM, TYPE> : IParameter<BoolValue<TYPE>, ITEM> {

        public BoolValueParameter(Func<ITEM, BoolValue<TYPE>> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, BoolValue<TYPE>> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.TinyInt;
            sqlParameter.Value = Func(item).Value;

            return sqlParameter;
        }
    }

    public class NullableBoolValueParameter<ITEM, TYPE> : IParameter<BoolValue<TYPE>?, ITEM> {

        public NullableBoolValueParameter(Func<ITEM, BoolValue<TYPE>?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, BoolValue<TYPE>?> Func { get; private set; }

        public DbParameter CreateParameter(ITEM item, DbCommand command) {

            SqlParameter sqlParameter = ((SqlCommand)command).CreateParameter();

            sqlParameter.ParameterName = Name;
            sqlParameter.SqlDbType = System.Data.SqlDbType.TinyInt;

            BoolValue<TYPE>? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
            return sqlParameter;
        }
    }
}