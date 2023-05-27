using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal class SqlServerSetValuesParameterCollector : ISetValuesCollector {

        public List<DbParameter> Parameters { get; } = new List<DbParameter>(1);

        public StringBuilder ValuesSql = new StringBuilder();
        public StringBuilder ParamSql = new StringBuilder();

        private IDatabase _database;

        private int _counter;

        public SqlServerSetValuesParameterCollector(IDatabase database) {
            _database = database;
        }

        private ISetValuesCollector AddParameter(IColumn column, SqlDbType dbType, object? value) {

            if(_counter > 0) {
                ValuesSql.Append(',');
                ParamSql.Append(',');
            }
            SqlServerHelper.AppendEnclose(ValuesSql, column.ColumnName, forceEnclose: false);

            string name = $"@{_counter++}";

            ParamSql.Append(name);

            if(value == null) {
                value = DBNull.Value;
            }
            Parameters.Add(new SqlParameter(parameterName: name, value) { SqlDbType = dbType });
            return this;
        }

        private ISetValuesCollector AddFunction(IColumn column, SqlDbType dbType, IFunction function) {

            if(_counter > 0) {
                ValuesSql.Append(',');
                ParamSql.Append(',');
            }
            _counter++;
            SqlServerHelper.AppendEnclose(ValuesSql, column.ColumnName, forceEnclose: false);
            ParamSql.Append(function.GetSql(_database, useAlias: true, parameters: null));
            return this;
        }

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.NVarChar, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.NVarChar, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.SmallInt, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.SmallInt, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Int, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Int, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.BigInt, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.BigInt, value?.Value);
        }

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Bit, value.Value);
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull {
            return AddParameter(column, SqlDbType.Bit, value?.Value);
        }

        private SqlDbType GetEnumDbType<ENUM>() where ENUM : notnull, Enum {

            NumericType integerType = EnumHelper.GetNumericType<ENUM>();

            switch(integerType) {
                case NumericType.UShort:
                case NumericType.Short:
                    return SqlDbType.SmallInt;
                case NumericType.Int:
                case NumericType.UInt:
                    return SqlDbType.Int;
                case NumericType.Long:
                case NumericType.ULong:
                    return SqlDbType.BigInt;
                case NumericType.Byte:
                case NumericType.SByte:
                    return SqlDbType.TinyInt;
                default:
                    throw new Exception($"Unknown {nameof(integerType)} type. Value = '{integerType}');");
            }
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), value);
        }

        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : notnull, Enum {
            return AddParameter(column, GetEnumDbType<ENUM>(), value);
        }

        public ISetValuesCollector Set(Column<string> column, string value) {
            return AddParameter(column, SqlDbType.NVarChar, value);
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {
            return AddParameter(column, SqlDbType.NVarChar, value);
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value);
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {
            return AddParameter(column, SqlDbType.UniqueIdentifier, value);
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return AddParameter(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {
            return AddParameter(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return AddParameter(column, SqlDbType.Bit, value.Value);
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {
            return AddParameter(column, SqlDbType.Bit, value?.Value);
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return AddParameter(column, SqlDbType.Decimal, value);
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {
            return AddParameter(column, SqlDbType.Decimal, value);
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return AddParameter(column, SqlDbType.Int, value);
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {
            return AddParameter(column, SqlDbType.Int, value);
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return AddParameter(column, SqlDbType.BigInt, value);
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {
            return AddParameter(column, SqlDbType.BigInt, value);
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return AddParameter(column, SqlDbType.Real, value);
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {
            return AddParameter(column, SqlDbType.Real, value);
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return AddParameter(column, SqlDbType.Float, value);
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {
            return AddParameter(column, SqlDbType.Float, value);
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return AddParameter(column, SqlDbType.Time, value.ToTimeSpan());
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {
            return AddParameter(column, SqlDbType.Time, value?.ToTimeSpan());
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return AddParameter(column, SqlDbType.DateTime, value);
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {
            return AddParameter(column, SqlDbType.DateTime, value);
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return AddParameter(column, SqlDbType.Date, value.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            return AddParameter(column, SqlDbType.Date, value?.ToDateTime(TimeOnly.MinValue));
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return AddParameter(column, SqlDbType.DateTimeOffset, value);
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {
            return AddParameter(column, SqlDbType.DateTimeOffset, value);
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {
            return AddParameter(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return AddParameter(column, SqlDbType.Binary, value);
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {
            return AddParameter(column, SqlDbType.Binary, value);
        }

        public ISetValuesCollector Set(AColumn<string> column, AFunction<string> value) {
            return AddFunction(column, SqlDbType.NVarChar, value);
        }

        public ISetValuesCollector Set(AColumn<Guid> column, AFunction<Guid> value) {
            return AddFunction(column, SqlDbType.UniqueIdentifier, value);
        }

        public ISetValuesCollector Set(AColumn<bool> column, AFunction<bool> value) {
            return AddFunction(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(AColumn<Bit> column, AFunction<Bit> value) {
            return AddFunction(column, SqlDbType.Bit, value);
        }

        public ISetValuesCollector Set(AColumn<decimal> column, AFunction<decimal> value) {
            return AddFunction(column, SqlDbType.Decimal, value);
        }

        public ISetValuesCollector Set(AColumn<short> column, AFunction<short> value) {
            return AddFunction(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(AColumn<int> column, AFunction<int> value) {
            return AddFunction(column, SqlDbType.Int, value);
        }

        public ISetValuesCollector Set(AColumn<long> column, AFunction<long> value) {
            return AddFunction(column, SqlDbType.BigInt, value);
        }

        public ISetValuesCollector Set(AColumn<float> column, AFunction<float> value) {
            return AddFunction(column, SqlDbType.Real, value);
        }

        public ISetValuesCollector Set(AColumn<double> column, AFunction<double> value) {
            return AddFunction(column, SqlDbType.Float, value);
        }

        public ISetValuesCollector Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return AddFunction(column, SqlDbType.Time, value);
        }

        public ISetValuesCollector Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return AddFunction(column, SqlDbType.DateTime, value);
        }

        public ISetValuesCollector Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return AddFunction(column, SqlDbType.Date, value);
        }

        public ISetValuesCollector Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return AddFunction(column, SqlDbType.DateTimeOffset, value);
        }

        public ISetValuesCollector Set(AColumn<byte> column, AFunction<byte> value) {
            return AddFunction(column, SqlDbType.SmallInt, value);
        }

        public ISetValuesCollector Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return AddFunction(column, SqlDbType.Binary, value);
        }
    }

    internal class SqlServerSetValuesCollector : ISetValuesCollector {

        private IDatabase _database;

        public SqlServerSetValuesCollector(IDatabase database) {
            _database = database;
        }

        public StringBuilder ValuesSql = new StringBuilder();
        public StringBuilder ParamsSql = new StringBuilder();

        private ISetValuesCollector SetValue(IColumn column, string value) {

            if(ValuesSql.Length > 0) {
                ValuesSql.Append(',');
                ParamsSql.Append(',');
            }
            SqlServerHelper.AppendEnclose(ValuesSql, column.ColumnName, forceEnclose: false);

            ParamsSql.Append(value);
            return this;
        }
        public ISetValuesCollector Set(Column<string> column, string value) {
            
            ArgumentNullException.ThrowIfNull(value);

            return SetValue(column, $"'{Helpers.EscapeForSql(value)}'");
        }

        public ISetValuesCollector Set(NullableColumn<string> column, string? value) {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value)}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Guid> column, Guid value) {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.ToString())}'");
        }

        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value) {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.ToString())}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<bool> column, bool value) {
            return SetValue(column, value ? "1" : "0");
        }

        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value) {

            if(value != null) {
                return SetValue(column, value.Value ? "1" : "0");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<Bit> column, Bit value) {
            return SetValue(column, value.Value ? "1" : "0");
        }

        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value) {

            if(value != null) {
                return SetValue(column, value.Value.Value ? "1" : "0");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<decimal> column, decimal value) {
            return SetValue(column, Helpers.EscapeForSql(value.ToString()));
        }

        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value) {

            if(value != null) {
                return SetValue(column, Helpers.EscapeForSql(value.Value.ToString()));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<short> column, short value) {
            return SetValue(column, value.ToString());
        }

        public ISetValuesCollector Set(NullableColumn<short> column, short? value) {

            if(value != null) {
                return SetValue(column, value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<int> column, int value) {
            return SetValue(column, value.ToString());
        }

        public ISetValuesCollector Set(NullableColumn<int> column, int? value) {

            if(value != null) {
                return SetValue(column, value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<long> column, long value) {
            return SetValue(column, value.ToString());
        }

        public ISetValuesCollector Set(NullableColumn<long> column, long? value) {

            if(value != null) {
                return SetValue(column, value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<float> column, float value) {
            return SetValue(column, Helpers.EscapeForSql(value.ToString()));
        }

        public ISetValuesCollector Set(NullableColumn<float> column, float? value) {

            if(value != null) {
               return SetValue(column, Helpers.EscapeForSql(value.Value.ToString()));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<double> column, double value) {
            return SetValue(column, Helpers.EscapeForSql(value.ToString()));
        }

        public ISetValuesCollector Set(NullableColumn<double> column, double? value) {

            if(value != null) {
                return SetValue(column, Helpers.EscapeForSql(value.Value.ToString()));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value) {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.ToString("HH:mm:ss.fffffff"))}'");
        }

        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value) {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.ToString("HH:mm:ss.fffffff"))}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value) {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'");
        }

        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value) {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"))}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value) {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd"))}'");
        }

        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value) {
            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.ToString("yyyy-MM-dd"))}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value) {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'");
        }

        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value) {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz"))}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte> column, byte value) {
            return SetValue(column, value.ToString());
        }

        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value) {

            if(value != null) {
                return SetValue(column, value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value) {
            return SetValue(column, "0x" + (BitConverter.ToString(value)).Replace("-", string.Empty));
        }

        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value) {

            if(value != null) {
                return SetValue(column, "0x" + (BitConverter.ToString(value)).Replace("-", string.Empty));
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum {
            return SetValue(column, ((int)(object)value).ToString());   //TODO: Find way to convert enum to integer without allocating an object on the heap 
        }

        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : notnull, Enum {

            if(value != null) {
                return SetValue(column, ((int)(object)value).ToString());   //TODO: Find way to convert enum to integer without allocating an object on the heap 
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.Value)}'");
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.Value)}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, $"'{Helpers.EscapeForSql(value.ToString())}'");
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, $"'{Helpers.EscapeForSql(value.Value.ToString())}'");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, value.Value.ToString());
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, value.Value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, value.Value.ToString());
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, value.Value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull {
            return SetValue(column, value.Value.ToString());
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, value.Value.Value.ToString());
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull {
            return SetValue(column, value.Value ? "1" : "0");
        }

        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull {

            if(value != null) {
                return SetValue(column, value.Value.Value ? "1" : "0");
            }
            return SetValue(column, "null");
        }

        public ISetValuesCollector Set(AColumn<string> column, AFunction<string> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<Guid> column, AFunction<Guid> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<bool> column, AFunction<bool> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<Bit> column, AFunction<Bit> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<decimal> column, AFunction<decimal> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<short> column, AFunction<short> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<int> column, AFunction<int> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<long> column, AFunction<long> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<float> column, AFunction<float> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<double> column, AFunction<double> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<byte> column, AFunction<byte> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }

        public ISetValuesCollector Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return SetValue(column, value.GetSql(_database, useAlias: false, parameters: null));
        }
    }
}