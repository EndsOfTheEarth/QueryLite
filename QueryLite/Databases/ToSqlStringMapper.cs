using QueryLite.Utility;
using System;
using System.Collections.Generic;

namespace QueryLite.Databases {

    /// <summary>
    /// Functions to convert supported data types into sql string values.
    /// </summary>
    public interface IToSqlStringFunctions {

        string ToSqlString(bool value);
        string ToSqlString(Bit value);
        string ToSqlString(byte[] value);
        string ToSqlString(byte value);
        string ToSqlString(DateTimeOffset value);
        string ToSqlString(DateTime value);
        string ToSqlString(TimeOnly value);
        string ToSqlString(DateOnly value);
        string ToSqlString(decimal value);
        string ToSqlString(double value);
        string ToSqlString(float value);
        string ToSqlString(Guid value);
        string ToSqlString(short value);
        string ToSqlString(int value);
        string ToSqlString(long value);
        string ToSqlString(string value);
        string ToSqlString(IGuidType value);
        string ToSqlString(IStringType value);
        string ToSqlString(IInt16Type value);
        string ToSqlString(IInt32Type value);
        string ToSqlString(IInt64Type value);
        string ToSqlString(IBoolType value);

        string? GetCSharpCodeSet(Type dotNetType);
    }

    public delegate string ToSqlStringDelegate(object value);

    public static class ToSqlStringMapper {

        public static ToSqlStringDelegate GetMapping(object value, IToSqlStringFunctions toSql) {

            ToSqlStringDelegate? toSqlStringDelegate = null;

            if(value is IGuidType guidType) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IGuidType)value).Value);
            }
            else if(value is IStringType stringType) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IStringType)value).Value);
            }
            else if(value is IInt16Type int16Type) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IInt16Type)value).Value);
            }
            else if(value is IInt32Type int32Type) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IInt32Type)value).Value);
            }
            else if(value is IInt64Type int64Type) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IInt64Type)value).Value);
            }
            else if(value is IBoolType boolType) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IBoolType)value).Value);
            }
            else if(value is IValue<Guid> guidIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<Guid>)value).Value);
            }
            else if(value is IValue<short> shortIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<short>)value).Value);
            }
            else if(value is IValue<int> intIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<int>)value).Value);
            }
            else if(value is IValue<long> longIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<long>)value).Value);
            }
            else if(value is IValue<string> stringIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<string>)value).Value);
            }
            else if(value is IValue<bool> boolIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<bool>)value).Value);
            }
            else if(value is IValue<decimal> decimalIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<decimal>)value).Value);
            }
            else if(value is IValue<DateTime> dateTimeIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<DateTime>)value).Value);
            }
            else if(value is IValue<DateTimeOffset> dateTimeOffsetIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<DateTimeOffset>)value).Value);
            }
            else if(value is IValue<DateOnly> dateOnlyIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<DateOnly>)value).Value);
            }
            else if(value is IValue<TimeOnly> timeOnlyIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<TimeOnly>)value).Value);
            }
            else if(value is IValue<float> floatIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<float>)value).Value);
            }
            else if(value is IValue<double> doubleIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<double>)value).Value);
            }
            else if(value is IValue<Bit> bitIValue) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<Bit>)value).Value);
            }
            else {

                if(value.GetType().IsEnum) {
                    toSqlStringDelegate = value => ((int)value).ToString()!;
                }
            }

            if(toSqlStringDelegate == null) {
                throw new Exception($"Unknown parameter type '{value.GetType().FullName}'");
            }
            return toSqlStringDelegate;
        }
    }

    /// <summary>
    /// Map csharp type to Database type
    /// </summary>
    public interface ITypeMap<DBTYPE> {

        DBTYPE Guid { get; }
        DBTYPE String { get; }
        DBTYPE Boolean { get; }
        DBTYPE ByteArray { get; }
        DBTYPE Byte { get; }
        DBTYPE DateTimeOffset { get; }
        DBTYPE DateTime { get; }
        DBTYPE TimeOnly { get; }
        DBTYPE DateOnly { get; }
        DBTYPE Decimal { get; }
        DBTYPE Double { get; }
        DBTYPE Float { get; }
        DBTYPE Short { get; }
        DBTYPE Integer { get; }
        DBTYPE Long { get; }
        DBTYPE Bit { get; }


        private DBTYPE AddDbType(Type type, DBTYPE dbType) {

            lock(DbTypeLookup) {
                DbTypeLookup.TryAdd(type, dbType);
            }
            return dbType;
        }

        internal DBTYPE GetDbTypeProtected(Type type) {

            if(DbTypeLookup.TryGetValue(type, out DBTYPE? dbType)) {
                return dbType;
            }

            /*
             *  Map Key Types
             */
            if(type.IsAssignableTo(typeof(IGuidType))) {
                return AddDbType(type, Guid);
            }
            if(type.IsAssignableTo(typeof(IStringType))) {
                return AddDbType(type, String);
            }
            if(type.IsAssignableTo(typeof(IInt16Type))) {
                return AddDbType(type, Short);
            }
            if(type.IsAssignableTo(typeof(IInt32Type))) {
                return AddDbType(type, Integer);
            }
            if(type.IsAssignableTo(typeof(IInt64Type))) {
                return AddDbType(type, Long);
            }
            if(type.IsAssignableTo(typeof(IBoolType))) {
                return AddDbType(type, Boolean);
            }

            /*
             *  Map Custom Types
             */
            if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                return AddDbType(type, Guid);
            }
            if(type.IsAssignableTo(typeof(IValue<short>))) {
                return AddDbType(type, Short);
            }
            if(type.IsAssignableTo(typeof(IValue<int>))) {
                return AddDbType(type, Integer);
            }
            if(type.IsAssignableTo(typeof(IValue<long>))) {
                return AddDbType(type, Long);
            }
            if(type.IsAssignableTo(typeof(IValue<string>))) {
                return AddDbType(type, String);
            }
            if(type.IsAssignableTo(typeof(IValue<bool>))) {
                return AddDbType(type, Boolean);
            }
            if(type.IsAssignableTo(typeof(IValue<decimal>))) {
                return AddDbType(type, Decimal);
            }
            if(type.IsAssignableTo(typeof(IValue<DateTime>))) {
                return AddDbType(type, DateTime);
            }
            if(type.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                return AddDbType(type, DateTimeOffset);
            }
            if(type.IsAssignableTo(typeof(IValue<DateOnly>))) {
                return AddDbType(type, DateOnly);
            }
            if(type.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                return AddDbType(type, TimeOnly);
            }
            if(type.IsAssignableTo(typeof(IValue<float>))) {
                return AddDbType(type, Float);
            }
            if(type.IsAssignableTo(typeof(IValue<double>))) {
                return AddDbType(type, Double);
            }
            if(type.IsAssignableTo(typeof(IValue<Bit>))) {
                return AddDbType(type, Bit);
            }

            Type? underlyingType = Nullable.GetUnderlyingType(type);

            if(underlyingType != null) {

                /*
                 *  Map Nullable Key Types
                 */
                if(underlyingType.IsAssignableTo(typeof(IGuidType))) {
                    return AddDbType(underlyingType, Guid);
                }
                if(underlyingType.IsAssignableTo(typeof(IStringType))) {
                    return AddDbType(underlyingType, String);
                }
                if(underlyingType.IsAssignableTo(typeof(IInt16Type))) {
                    return AddDbType(underlyingType, Short);
                }
                if(underlyingType.IsAssignableTo(typeof(IInt32Type))) {
                    return AddDbType(underlyingType, Integer);
                }
                if(underlyingType.IsAssignableTo(typeof(IInt64Type))) {
                    return AddDbType(underlyingType, Long);
                }
                if(underlyingType.IsAssignableTo(typeof(IBoolType))) {
                    return AddDbType(underlyingType, Boolean);
                }

                /*
                 *  Map Nullable Custom Types
                 */
                if(underlyingType.IsAssignableTo(typeof(IValue<Guid>))) {
                    return AddDbType(underlyingType, Guid);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<short>))) {
                    return AddDbType(underlyingType, Short);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<int>))) {
                    return AddDbType(underlyingType, Integer);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<long>))) {
                    return AddDbType(underlyingType, Long);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<string>))) {
                    return AddDbType(underlyingType, String);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<bool>))) {
                    return AddDbType(underlyingType, Boolean);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<decimal>))) {
                    return AddDbType(underlyingType, Decimal);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateTime>))) {
                    return AddDbType(underlyingType, DateTime);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                    return AddDbType(underlyingType, DateTimeOffset);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<DateOnly>))) {
                    return AddDbType(underlyingType, DateOnly);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                    return AddDbType(underlyingType, TimeOnly);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<float>))) {
                    return AddDbType(underlyingType, Float);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<double>))) {
                    return AddDbType(underlyingType, Double);
                }
                if(underlyingType.IsAssignableTo(typeof(IValue<Bit>))) {
                    return AddDbType(underlyingType, Bit);
                }
            }

            /*
             * Map Enum Types
             */
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
                    return AddDbType(type, Short);
                }
                else if(integerType == NumericType.Short) {
                    return AddDbType(type, Short);
                }
                else if(integerType == NumericType.UInt) {
                    return AddDbType(type, Integer);
                }
                else if(integerType == NumericType.Int) {
                    return AddDbType(type, Integer);
                }
                else if(integerType == NumericType.ULong) {
                    return AddDbType(type, Long);
                }
                else if(integerType == NumericType.Long) {
                    return AddDbType(type, Long);
                }
                else if(integerType == NumericType.SByte) {
                    return AddDbType(type, Byte);
                }
                else if(integerType == NumericType.Byte) {
                    return AddDbType(type, Byte);
                }
                else {
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
                }
            }
            throw new Exception($"Unknown PostgreSql parameter type '{type.FullName}'");
        }

        Dictionary<Type, DBTYPE> DbTypeLookup { get; }

        public static Dictionary<Type, DBTYPE> GetDbTypeLookup(ITypeMap<DBTYPE> TypeMapper) {

            Dictionary<Type, DBTYPE> lookup = new Dictionary<Type, DBTYPE>() {
                { typeof(Guid), TypeMapper.Guid },
                { typeof(Guid?), TypeMapper.Guid },
                { typeof(bool), TypeMapper.Boolean },
                { typeof(bool?), TypeMapper.Boolean },
                { typeof(Bit), TypeMapper.Bit },
                { typeof(Bit?), TypeMapper.Bit },
                { typeof(byte[]), TypeMapper.ByteArray },
                { typeof(byte?[]), TypeMapper.ByteArray },
                { typeof(byte), TypeMapper.Byte },
                { typeof(DateTimeOffset), TypeMapper.DateTimeOffset },
                { typeof(DateTimeOffset?), TypeMapper.DateTimeOffset },
                { typeof(DateTime), TypeMapper.DateTime },
                { typeof(DateTime?), TypeMapper.DateTime },
                { typeof(TimeOnly), TypeMapper.TimeOnly },
                { typeof(TimeOnly?), TypeMapper.TimeOnly },
                { typeof(DateOnly), TypeMapper.DateOnly },
                { typeof(DateOnly?), TypeMapper.DateOnly },
                { typeof(decimal), TypeMapper.Decimal },
                { typeof(decimal?), TypeMapper.Decimal },
                { typeof(double), TypeMapper.Double },
                { typeof(double?), TypeMapper.Double },
                { typeof(float), TypeMapper.Float },
                { typeof(float?), TypeMapper.Float },
                { typeof(short), TypeMapper.Short },
                { typeof(short?), TypeMapper.Short },
                { typeof(int), TypeMapper.Integer },
                { typeof(int?), TypeMapper.Integer },
                { typeof(long), TypeMapper.Long },
                { typeof(long?), TypeMapper.Long },
                { typeof(string), TypeMapper.String }
            };
            return lookup;
        }
    }
}