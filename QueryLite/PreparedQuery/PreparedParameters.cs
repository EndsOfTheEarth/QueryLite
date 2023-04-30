using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace QueryLite.PreparedQuery {

    public interface IParameter<ITEM> {

        string? Name { get; set; }

        public void PopulateParameter(ITEM item, DbParameter parameter);
    }

    public interface IParameter<VALUE, ITEM> : IParameter<ITEM> {

    }

    public class GuidParameter<ITEM> : IParameter<Guid, ITEM> {

        public GuidParameter(Func<ITEM, Guid> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Guid> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableGuidParameter<ITEM> : IParameter<Guid?, ITEM> {

        public NullableGuidParameter(Func<ITEM, Guid?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Guid?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;

            Guid? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class StringParameter<ITEM> : IParameter<string, ITEM> {

        public StringParameter(Func<ITEM, string> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, string> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.NVarChar;

            string value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class ShortParameter<ITEM> : IParameter<short, ITEM> {

        public ShortParameter(Func<ITEM, short> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, short> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableShortParameter<ITEM> : IParameter<short?, ITEM> {

        public NullableShortParameter(Func<ITEM, short?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, short?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.SmallInt;

            short? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class IntParameter<ITEM> : IParameter<int, ITEM> {

        public IntParameter(Func<ITEM, int> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, int> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Int;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableIntParameter<ITEM> : IParameter<int?, ITEM> {

        public NullableIntParameter(Func<ITEM, int?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, int?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Int;

            int? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class LongParameter<ITEM> : IParameter<long, ITEM> {

        public LongParameter(Func<ITEM, long> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, long> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableLongParameter<ITEM> : IParameter<long?, ITEM> {

        public NullableLongParameter(Func<ITEM, long?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, long?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.BigInt;

            long? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class BoolParameter<ITEM> : IParameter<bool, ITEM> {

        public BoolParameter(Func<ITEM, bool> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, bool> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.TinyInt;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableBoolParameter<ITEM> : IParameter<bool?, ITEM> {

        public NullableBoolParameter(Func<ITEM, bool?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, bool?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.TinyInt;

            bool? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class BitParameter<ITEM> : IParameter<Bit, ITEM> {

        public BitParameter(Func<ITEM, Bit> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Bit> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Bit;
            sqlParameter.Value = Func(item).Value;
        }
    }

    public class NullableBitParameter<ITEM> : IParameter<Bit?, ITEM> {

        public NullableBitParameter(Func<ITEM, Bit?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, Bit?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Bit;

            Bit? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value.Value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class DecimalParameter<ITEM> : IParameter<decimal, ITEM> {

        public DecimalParameter(Func<ITEM, decimal> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, decimal> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Decimal;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableDecimalParameter<ITEM> : IParameter<decimal?, ITEM> {

        public NullableDecimalParameter(Func<ITEM, decimal?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, decimal?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Decimal;

            decimal? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class FloatParameter<ITEM> : IParameter<float, ITEM> {

        public FloatParameter(Func<ITEM, float> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, float> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Real;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableFloatParameter<ITEM> : IParameter<float?, ITEM> {

        public NullableFloatParameter(Func<ITEM, float?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, float?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Real;

            float? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class DoubleParameter<ITEM> : IParameter<double, ITEM> {

        public DoubleParameter(Func<ITEM, double> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, double> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Float;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableDoubleParameter<ITEM> : IParameter<double?, ITEM> {

        public NullableDoubleParameter(Func<ITEM, double?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, double?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Float;

            double? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class ByteArrayParameter<ITEM> : IParameter<byte[], ITEM> {

        public ByteArrayParameter(Func<ITEM, byte[]> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, byte[]> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Binary;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableByteArrayParameter<ITEM> : IParameter<byte[]?, ITEM> {

        public NullableByteArrayParameter(Func<ITEM, byte[]?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, byte[]?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Binary;

            byte[]? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class DateTimeParameter<ITEM> : IParameter<DateTime, ITEM> {

        public DateTimeParameter(Func<ITEM, DateTime> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTime> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTime;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableDateTimeParameter<ITEM> : IParameter<DateTime?, ITEM> {

        public NullableDateTimeParameter(Func<ITEM, DateTime?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTime?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTime;

            DateTime? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class DateTimeOffsetParameter<ITEM> : IParameter<DateTimeOffset, ITEM> {

        public DateTimeOffsetParameter(Func<ITEM, DateTimeOffset> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTimeOffset> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTimeOffset;
            sqlParameter.Value = Func(item);
        }
    }

    public class NullableDateTimeOffsetParameter<ITEM> : IParameter<DateTimeOffset?, ITEM> {

        public NullableDateTimeOffsetParameter(Func<ITEM, DateTimeOffset?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateTimeOffset?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.DateTimeOffset;

            DateTimeOffset? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value;
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class DateOnlyParameter<ITEM> : IParameter<DateOnly, ITEM> {

        public DateOnlyParameter(Func<ITEM, DateOnly> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateOnly> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Date;
            sqlParameter.Value = Func(item).ToDateTime(TimeOnly.MinValue);
        }
    }

    public class NullableDateOnlyParameter<ITEM> : IParameter<DateOnly?, ITEM> {

        public NullableDateOnlyParameter(Func<ITEM, DateOnly?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, DateOnly?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Date;

            DateOnly? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value.ToDateTime(TimeOnly.MinValue);
            }
            else {
                sqlParameter.Value = DBNull.Value;
            }
        }
    }

    public class TimeOnlyParameter<ITEM> : IParameter<TimeOnly, ITEM> {

        public TimeOnlyParameter(Func<ITEM, TimeOnly> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, TimeOnly> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Time;
            sqlParameter.Value = Func(item).ToTimeSpan();
        }
    }

    public class NullableTimeOnlyParameter<ITEM> : IParameter<TimeOnly?, ITEM> {

        public NullableTimeOnlyParameter(Func<ITEM, TimeOnly?> func) {
            Func = func;
        }
        public string? Name { get; set; }
        public Func<ITEM, TimeOnly?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            SqlParameter sqlParameter = (SqlParameter)parameter;

            sqlParameter.SqlDbType = System.Data.SqlDbType.Time;

            TimeOnly? value = Func(item);

            if(value != null) {
                sqlParameter.Value = value.Value.ToTimeSpan();
            }
            else {
                sqlParameter.Value = DBNull.Value;
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

    public class EnumParameter<ITEM, ENUM> where ENUM : Enum, IParameter<ENUM, ITEM> {

        public EnumParameter(Func<ITEM, ENUM> func) {
            Func = func;
            _IntegerType = EnumHelper.GetNumericType<ENUM>();
        }
        private NumericType _IntegerType;
        public string? Name { get; set; }
        public Func<ITEM, ENUM> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {
            EnumHelper.SetParameterValue<ENUM>(value: Func(item), parameter: (SqlParameter)parameter, _IntegerType);
        }
    }

    public class NullableEnumParameter<ITEM, ENUM> where ENUM : Enum, IParameter<ENUM?, ITEM> {

        public NullableEnumParameter(Func<ITEM, ENUM?> func) {
            Func = func;
            _IntegerType = EnumHelper.GetNumericType<ENUM>();
        }
        private NumericType _IntegerType;
        public string? Name { get; set; }
        public Func<ITEM, ENUM?> Func { get; private set; }

        public void PopulateParameter(ITEM item, DbParameter parameter) {

            ENUM? value = Func(item);

            if(value != null) {
                EnumHelper.SetParameterValue<ENUM>(value: value, parameter: (SqlParameter)parameter, _IntegerType);
            }
            else {
                EnumHelper.SetNullParameterValue<ENUM>(parameter: (SqlParameter)parameter, _IntegerType);
            }
        }
    }
}