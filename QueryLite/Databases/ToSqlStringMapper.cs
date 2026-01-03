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
using System.Data.Common;

namespace QueryLite.Databases {

    /// <summary>
    /// Functions to convert supported data types into sql string values.
    /// </summary>
    public abstract class AToSqlStringFunctions {

        public abstract string ToSqlString(bool value);
        public abstract string ToSqlString(Bit value);
        public abstract string ToSqlString(byte[] value);
        public abstract string ToSqlString(byte value);
        public abstract string ToSqlString(DateTimeOffset value);
        public abstract string ToSqlString(DateTime value);
        public abstract string ToSqlString(TimeOnly value);
        public abstract string ToSqlString(DateOnly value);
        public abstract string ToSqlString(decimal value);
        public abstract string ToSqlString(double value);
        public abstract string ToSqlString(float value);
        public abstract string ToSqlString(Guid value);
        public abstract string ToSqlString(short value);
        public abstract string ToSqlString(int value);
        public abstract string ToSqlString(long value);
        public abstract string ToSqlString(string value);
        public abstract string ToSqlString(Json value);
        public abstract string ToSqlString(Jsonb value);

        public abstract string? GetCSharpCodeSet(Type dotNetType);

        private Dictionary<Type, ToSqlStringDelegate> ToSqlStringLookup { get; }

        protected AToSqlStringFunctions() {
            ToSqlStringLookup = GetSqlStringDelegateLookup(this);
        }

        public string ConvertToSql(object value) {

            Type type = value.GetType();

            if(ToSqlStringLookup.TryGetValue(type, out ToSqlStringDelegate? toSqlStringDelegate)) {
                return toSqlStringDelegate(value);
            }

            toSqlStringDelegate = ToSqlStringMapper.GetMapping(value, this);

            lock(ToSqlStringLookup) {
                ToSqlStringLookup.TryAdd(type, toSqlStringDelegate);
            }
            return toSqlStringDelegate(value);
        }

        private static Dictionary<Type, ToSqlStringDelegate> GetSqlStringDelegateLookup(AToSqlStringFunctions toSql) {

            Dictionary<Type, ToSqlStringDelegate> lookup = new Dictionary<Type, ToSqlStringDelegate>() {
                { typeof(bool), value => toSql.ToSqlString((bool)value) },
                { typeof(Bit), value => toSql.ToSqlString((Bit)value) },
                { typeof(byte[]), value => toSql.ToSqlString((byte[])value) },
                { typeof(byte), value => toSql.ToSqlString((byte)value) },
                { typeof(DateTimeOffset), value => toSql.ToSqlString((DateTimeOffset)value) },
                { typeof(DateTime), value => toSql.ToSqlString((DateTime)value) },
                { typeof(TimeOnly), value => toSql.ToSqlString((TimeOnly)value) },
                { typeof(DateOnly), value => toSql.ToSqlString((DateOnly)value) },
                { typeof(decimal), value => toSql.ToSqlString((decimal)value) },
                { typeof(double), value => toSql.ToSqlString((double)value) },
                { typeof(float), value => toSql.ToSqlString((float)value) },
                { typeof(Guid), value => toSql.ToSqlString((Guid)value) },
                { typeof(short), value => toSql.ToSqlString((short)value) },
                { typeof(int), value => toSql.ToSqlString((int)value) },
                { typeof(long), value => toSql.ToSqlString((long)value) },
                { typeof(string), value => toSql.ToSqlString((string)value) },
                { typeof(Json), value => toSql.ToSqlString((Json)value) },
                { typeof(Jsonb), value => toSql.ToSqlString((Jsonb)value) }
            };
            return lookup;
        }
    }

    public delegate string ToSqlStringDelegate(object value);

    public static class ToSqlStringMapper {

        public static ToSqlStringDelegate GetMapping(object value, AToSqlStringFunctions toSql) {

            ToSqlStringDelegate? toSqlStringDelegate = null;

            if(value is IValue<Guid>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<Guid>)value).Value);
            }
            else if(value is IValue<short>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<short>)value).Value);
            }
            else if(value is IValue<int>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<int>)value).Value);
            }
            else if(value is IValue<long>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<long>)value).Value);
            }
            else if(value is IValue<string>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<string>)value).Value);
            }
            else if(value is IValue<bool>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<bool>)value).Value);
            }
            else if(value is IValue<decimal>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<decimal>)value).Value);
            }
            else if(value is IValue<DateTime>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<DateTime>)value).Value);
            }
            else if(value is IValue<DateTimeOffset>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<DateTimeOffset>)value).Value);
            }
            else if(value is IValue<DateOnly>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<DateOnly>)value).Value);
            }
            else if(value is IValue<TimeOnly>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<TimeOnly>)value).Value);
            }
            else if(value is IValue<float>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<float>)value).Value);
            }
            else if(value is IValue<double>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<double>)value).Value);
            }
            else if(value is IValue<Bit>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<Bit>)value).Value);
            }
            else if(value is IValue<Json>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<Json>)value).Value);
            }
            else if(value is IValue<Jsonb>) {
                toSqlStringDelegate = value => toSql.ToSqlString(((IValue<Jsonb>)value).Value);
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
    public abstract class ATypeMap<DBTYPE> where DBTYPE : struct {

        public abstract DBTYPE Guid { get; }
        public abstract DBTYPE String { get; }
        public abstract DBTYPE Boolean { get; }
        public abstract DBTYPE ByteArray { get; }
        public abstract DBTYPE SByte { get; }
        public abstract DBTYPE Byte { get; }
        public abstract DBTYPE DateTimeOffset { get; }
        public abstract DBTYPE DateTime { get; }
        public abstract DBTYPE TimeOnly { get; }
        public abstract DBTYPE DateOnly { get; }
        public abstract DBTYPE Decimal { get; }
        public abstract DBTYPE Double { get; }
        public abstract DBTYPE Float { get; }
        public abstract DBTYPE Short { get; }
        public abstract DBTYPE Integer { get; }
        public abstract DBTYPE Long { get; }
        public abstract DBTYPE Bit { get; }
        public abstract DBTYPE Json { get; }
        public abstract DBTYPE JsonB { get; }

        private Dictionary<Type, DBTYPE> DbTypeLookup { get; }

        protected ATypeMap() {
            DbTypeLookup = GetDbTypeLookup(this);
        }

        private DBTYPE AddDbType(Type type, DBTYPE dbType) {

            lock(DbTypeLookup) {
                DbTypeLookup.TryAdd(type, dbType);
            }
            return dbType;
        }

        public DBTYPE GetDbType(Type type) {

            if(DbTypeLookup.TryGetValue(type, out DBTYPE dbt)) {
                return dbt;
            }

            DBTYPE? dbType = TryGetDbTypeForCustomType(type);

            if(dbType != null) {
                return dbType.Value;
            }

            Type? underlyingType = Nullable.GetUnderlyingType(type);

            if(underlyingType != null) {

                /*
                 *  Map Nullable Custom Types
                 */
                dbType = TryGetDbTypeForCustomType(underlyingType);

                if(dbType != null) {
                    return dbType.Value;
                }
            }

            /*
             * Map Enum Types
             */
            dbType = TryGetDbTypeForEnumType(type, underlyingType);

            if(dbType != null) {
                return dbType.Value;
            }
            throw new Exception($"Unknown parameter type '{type.FullName}'");
        }

        private DBTYPE? TryGetDbTypeForEnumType(Type type, Type? underlyingType) {

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
            return null;
        }
        private DBTYPE? TryGetDbTypeForCustomType(Type type) {

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
            if(type.IsAssignableTo(typeof(IValue<Json>))) {
                return AddDbType(type, Json);
            }
            if(type.IsAssignableTo(typeof(IValue<Jsonb>))) {
                return AddDbType(type, JsonB);
            }
            return null;
        }

        /// <summary>
        /// Returns default csharp / DB Type enum.
        /// </summary>
        private static Dictionary<Type, DBTYPE> GetDbTypeLookup(ATypeMap<DBTYPE> TypeMapper) {

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
                { typeof(string), TypeMapper.String },
                { typeof(Json), TypeMapper.Json },
                { typeof(Jsonb), TypeMapper.JsonB }
            };
            return lookup;
        }
    }

    /// <summary>
    /// Abstract class for creating sql parameters for the supported csharp types.
    /// </summary>
    public abstract class AParameterMap<PARAMETER, DBTYPE> where PARAMETER : DbParameter where DBTYPE : struct {

        private Dictionary<Type, CreateParameterDelegate> CreateParameterDelegateLookup { get; }

        protected ATypeMap<DBTYPE> TypeMap { get; }

        protected AParameterMap(ATypeMap<DBTYPE> typeMap) {
            TypeMap = typeMap;
            CreateParameterDelegateLookup = LoadCreateParameterDelegateLookup();
        }

        protected abstract PARAMETER CreateParameter(string name, Guid? value);
        protected abstract PARAMETER CreateParameter(string name, string? value);

        protected abstract PARAMETER CreateParameter(string name, short? value);
        protected abstract PARAMETER CreateParameter(string name, int? value);
        protected abstract PARAMETER CreateParameter(string name, long? value);

        protected abstract PARAMETER CreateParameter(string name, bool? value);
        protected abstract PARAMETER CreateParameter(string name, Bit? value);

        protected abstract PARAMETER CreateParameter(string name, byte? value);
        protected abstract PARAMETER CreateParameter(string name, sbyte? value);

        protected abstract PARAMETER CreateParameter(string name, decimal? value);
        protected abstract PARAMETER CreateParameter(string name, float? value);
        protected abstract PARAMETER CreateParameter(string name, double? value);

        protected abstract PARAMETER CreateParameter(string name, byte[]? value);

        protected abstract PARAMETER CreateParameter(string name, DateTime? value);
        protected abstract PARAMETER CreateParameter(string name, DateTimeOffset? value);

        protected abstract PARAMETER CreateParameter(string name, DateOnly? value);
        protected abstract PARAMETER CreateParameter(string name, TimeOnly? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<Guid>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<string>? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<short>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<int>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<long>? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<bool>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<Bit>? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<decimal>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<float>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<double>? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<byte[]>? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<DateTime>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<DateTimeOffset>? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<DateOnly>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<TimeOnly>? value);

        protected abstract PARAMETER CreateParameter(string name, Json? value);
        protected abstract PARAMETER CreateParameter(string name, Jsonb? value);

        protected abstract PARAMETER CreateParameter(string name, IValue<Json>? value);
        protected abstract PARAMETER CreateParameter(string name, IValue<Jsonb>? value);

        /// <summary>
        /// Returns default type / create parameter delegate mappings.
        /// </summary>
        protected Dictionary<Type, CreateParameterDelegate> LoadCreateParameterDelegateLookup() {

            Dictionary<Type, CreateParameterDelegate> lookup = new Dictionary<Type, CreateParameterDelegate>() {
                { typeof(Guid), (name, value) => CreateParameter(name: name, value: (Guid?)value) },
                { typeof(Guid?), (name, value) => CreateParameter(name: name, value: (Guid?)value) },
                { typeof(string), (name, value) => CreateParameter(name: name, value: value != null ? (string)value : null) },
                { typeof(short), (name, value) => CreateParameter(name: name, value: (short?)value) },
                { typeof(short?), (name, value) => CreateParameter(name: name, value: (short?)value) },
                { typeof(int), (name, value) => CreateParameter(name: name, value: (int?)value) },
                { typeof(int?), (name, value) => CreateParameter(name: name, value: (int?)value) },
                { typeof(long), (name, value) => CreateParameter(name: name, value: (long?)value) },
                { typeof(long?), (name, value) => CreateParameter(name: name, value: (long?)value) },
                { typeof(bool), (name, value) => CreateParameter(name: name, value: (bool?)value) },
                { typeof(bool?), (name, value) => CreateParameter(name: name, value: (bool?)value) },
                { typeof(Bit), (name, value) => CreateParameter(name: name, value: (Bit?)value) },
                { typeof(Bit?), (name, value) => CreateParameter(name: name, value: (Bit?)value) },
                { typeof(decimal), (name, value) => CreateParameter(name: name, value: (decimal?)value) },
                { typeof(decimal?), (name, value) => CreateParameter(name: name, value: (decimal?)value) },
                { typeof(float), (name, value) => CreateParameter(name: name, value: (float?)value) },
                { typeof(float?), (name, value) => CreateParameter(name: name, value: (float?)value) },
                { typeof(double), (name, value) => CreateParameter(name: name, value: (double?)value) },
                { typeof(double?), (name, value) => CreateParameter(name: name, value: (double?)value) },
                { typeof(byte[]), (name, value) => CreateParameter(name: name, value: (byte[]?) value) },
                { typeof(DateTime), (name, value) => CreateParameter(name: name, value: (DateTime?)value) },
                { typeof(DateTime?), (name, value) => CreateParameter(name: name, value: (DateTime?)value) },
                { typeof(DateTimeOffset), (name, value) => CreateParameter(name: name, value: (DateTimeOffset?) value) },
                { typeof(DateTimeOffset?), (name, value) => CreateParameter(name: name, value: (DateTimeOffset?) value) },
                { typeof(DateOnly), (name, value) => CreateParameter(name: name, value: (DateOnly?)value) },
                { typeof(DateOnly?), (name, value) => CreateParameter(name: name, value: (DateOnly?)value) },
                { typeof(TimeOnly), (name, value) => CreateParameter(name: name, value: (TimeOnly?)value) },
                { typeof(TimeOnly?), (name, value) => CreateParameter(name: name, value: (TimeOnly?)value) },
                { typeof(Json), (name, value) => CreateParameter(name: name, value: (Json?)value) },
                { typeof(Json?), (name, value) => CreateParameter(name: name, value: (Json?)value) },
                { typeof(Jsonb), (name, value) => CreateParameter(name: name, value: (Jsonb?)value) },
                { typeof(Jsonb?), (name, value) => CreateParameter(name: name, value: (Jsonb?)value) }
            };
            return lookup;
        }

        private CreateParameterDelegate AddParameterDelegate(Type type, CreateParameterDelegate @delegate) {

            lock(CreateParameterDelegateLookup) {
                CreateParameterDelegateLookup.TryAdd(type, @delegate);
            }
            return @delegate;
        }

        public CreateParameterDelegate GetCreateParameterDelegate(Type type) {

            if(CreateParameterDelegateLookup.TryGetValue(type, out CreateParameterDelegate? createParameterDelegate)) {
                return createParameterDelegate!;
            }

            /*
             * Map Custom Types
             */
            createParameterDelegate = TryGetCustomTypeCreateParameterDelegate(type);

            if(createParameterDelegate != null) {
                return createParameterDelegate;
            }


            Type? underlyingType = Nullable.GetUnderlyingType(type);

            /*
             * Map Nullable Custom Types
             */
            if(underlyingType != null) {

                createParameterDelegate = TryGetCustomTypeCreateParameterDelegate(underlyingType);

                if(createParameterDelegate != null) {
                    return createParameterDelegate;
                }
            }

            /*
             * Map Enum Types
             */
            createParameterDelegate = TryGetEnumCreateParameterDelegate(type, underlyingType);

            if(createParameterDelegate != null) {
                return createParameterDelegate;
            }
            throw new Exception($"Unsupported Type: '{type.FullName}' type);");
        }

        /// <summary>
        /// Returns a create parameter delegate if 'type' is an enum.
        /// </summary>
        private CreateParameterDelegate? TryGetEnumCreateParameterDelegate(Type type, Type? underlyingType) {

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

                if(integerType == NumericType.UShort || integerType == NumericType.Short) {
                    return AddParameterDelegate(type, (name, value) => CreateParameter(name, value != null ? (short)value : null));
                }
                else if(integerType == NumericType.UInt || integerType == NumericType.Int) {
                    return AddParameterDelegate(type, (name, value) => CreateParameter(name, value != null ? (int)value : null));
                }
                else if(integerType == NumericType.ULong || integerType == NumericType.Long) {
                    return AddParameterDelegate(type, (name, value) => CreateParameter(name, value != null ? (long)value : null));
                }
                else if(integerType == NumericType.SByte) {
                    return AddParameterDelegate(type, (name, value) => CreateParameter(name, value != null ? (sbyte)value : null));
                }
                else if(integerType == NumericType.Byte) {
                    return AddParameterDelegate(type, (name, value) => CreateParameter(name, value != null ? (byte)value : null));
                }
                else {
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a create parameter delegate if 'type' is a custom 'IValue<>' type.
        /// </summary>
        private CreateParameterDelegate? TryGetCustomTypeCreateParameterDelegate(Type type) {

            if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<Guid>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<short>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<short>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<int>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<int>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<long>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<long>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<string>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<string>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<bool>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<bool>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<decimal>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<decimal>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<DateTime>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<DateTime>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<DateTimeOffset>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<DateTimeOffset>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<DateOnly>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<DateOnly>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<TimeOnly>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<TimeOnly>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<float>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<float>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<double>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<double>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<Bit>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<Bit>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<Json>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<Json>?)value));
            }
            if(type.IsAssignableTo(typeof(IValue<Jsonb>))) {
                return AddParameterDelegate(type, (name, value) => CreateParameter(name, (IValue<Jsonb>?)value));
            }

            return null;
        }
    }
}