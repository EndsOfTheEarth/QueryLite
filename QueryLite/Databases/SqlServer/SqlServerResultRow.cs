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
using System.Data.Common;
using System;
using System.Data.SqlClient;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerResultRow : IResultRow {

        private readonly SqlDataReader _reader;
        private int _ordinal = -1;

        public SqlServerResultRow(DbDataReader reader) {
            _reader = (SqlDataReader)reader;
        }

        void IResultRow.Reset() {
            _ordinal = -1;
        }

        public string Get(Column<string> column) {

            _ordinal++;

            //In the case of Left Joins a non null column can return as null
            //So we leave it up to the caller to check for a null column or row in that case
            if(_reader.IsDBNull(_ordinal)) {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            return _reader.GetString(_ordinal);
        }

        public string? Get(NullableColumn<string> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetString(_ordinal);
        }

        public Guid Get(Column<Guid> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Guid.Empty;
            }
            return _reader.GetGuid(_ordinal);
        }
        public Guid? Get(NullableColumn<Guid> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetGuid(_ordinal);
        }

        public bool Get(Column<bool> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return false;
            }
            return _reader.GetByte(_ordinal) == 1;
        }
        public bool? Get(NullableColumn<bool> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetByte(_ordinal) == 1;
        }

        public decimal Get(Column<decimal> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetDecimal(_ordinal);
        }
        public decimal? Get(NullableColumn<decimal> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetDecimal(_ordinal);
        }

        public short Get(Column<short> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetInt16(_ordinal);
        }
        public short? Get(NullableColumn<short> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetInt16(_ordinal);
        }

        public int Get(Column<int> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetInt32(_ordinal);
        }
        public int? Get(NullableColumn<int> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetInt32(_ordinal);
        }

        public long Get(Column<long> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetInt64(_ordinal);
        }
        public long? Get(NullableColumn<long> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetInt64(_ordinal);
        }

        public float Get(Column<float> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return float.MinValue;
            }
            return _reader.GetFloat(_ordinal);
        }
        public float? Get(NullableColumn<float> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetFloat(_ordinal);
        }

        public double Get(Column<double> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return double.MinValue;
            }
            return _reader.GetDouble(_ordinal);
        }
        public double? Get(NullableColumn<double> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetDouble(_ordinal);
        }

        public DateTime Get(Column<DateTime> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return DateTime.MinValue;
            }
            return _reader.GetDateTime(_ordinal);
        }
        public DateTime? Get(NullableColumn<DateTime> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetDateTime(_ordinal);
        }

        public TimeOnly Get(Column<TimeOnly> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return TimeOnly.MinValue;
            }
            TimeSpan value = _reader.GetTimeSpan(_ordinal);
            return TimeOnly.FromTimeSpan(value);
        }
        public TimeOnly? Get(NullableColumn<TimeOnly> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            TimeSpan value = _reader.GetTimeSpan(_ordinal);
            return TimeOnly.FromTimeSpan(value);
        }

        public DateOnly Get(Column<DateOnly> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return DateOnly.MinValue;
            }
            DateTime value = _reader.GetDateTime(_ordinal);
            return DateOnly.FromDateTime(value);
        }
        public DateOnly? Get(NullableColumn<DateOnly> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            DateTime value = _reader.GetDateTime(_ordinal);
            return DateOnly.FromDateTime(value);
        }

        public DateTimeOffset Get(Column<DateTimeOffset> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return DateTimeOffset.MinValue;
            }
            return (DateTimeOffset)_reader.GetValue(_ordinal);
        }
        public DateTimeOffset? Get(NullableColumn<DateTimeOffset> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return (DateTimeOffset)_reader.GetValue(_ordinal);
        }

        public byte Get(Column<byte> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return byte.MinValue;
            }
            return _reader.GetByte(_ordinal);
        }
        public byte? Get(NullableColumn<byte> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetByte(_ordinal);
        }

        public byte[] Get(Column<byte[]> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            return (byte[])_reader.GetValue(_ordinal);
        }
        public byte[]? Get(NullableColumn<byte[]> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return (byte[])_reader.GetValue(_ordinal);
        }

        public ENUM GetEnum<ENUM>(Column<ENUM> column) where ENUM : notnull, Enum {

            _ordinal++;

            Type fieldType = _reader.GetFieldType(_ordinal);

            if(_reader.IsDBNull(_ordinal)) {
                return IntegerToEnum<int, ENUM>.Convert(0);
            }

            int value;

            if(fieldType == typeof(byte)) {
                value = _reader.GetByte(_ordinal);
            }
            else if(fieldType == typeof(short)) {
                value = _reader.GetInt16(_ordinal);
            }
            else {
                value = _reader.GetInt32(_ordinal);
            }
            return IntegerToEnum<int, ENUM>.Convert(value);
        }

        public ENUM? GetEnum<ENUM>(NullableColumn<ENUM> column) where ENUM : notnull, Enum {

            _ordinal++;

            Type fieldType = _reader.GetFieldType(_ordinal);

            if(_reader.IsDBNull(_ordinal)) {
                return (ENUM?)(object?)null;
            }

            int value;

            if(fieldType == typeof(byte)) {
                value = _reader.GetByte(_ordinal);
            }
            else if(fieldType == typeof(short)) {
                value = _reader.GetInt16(_ordinal);
            }
            else {
                value = _reader.GetInt32(_ordinal);
            }
            return IntegerToEnum<int, ENUM>.Convert(value);
        }

        public StringKey<TYPE> Get<TYPE>(Column<StringKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return StringKey<TYPE>.NotSet;
            }
            return new StringKey<TYPE>(_reader.GetString(_ordinal));
        }
        public StringKey<TYPE>? Get<TYPE>(NullableColumn<StringKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new StringKey<TYPE>(_reader.GetString(_ordinal));
        }

        public GuidKey<TYPE> Get<TYPE>(Column<GuidKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return GuidKey<TYPE>.NotSet;
            }
            return new GuidKey<TYPE>(_reader.GetGuid(_ordinal));
        }

        public GuidKey<TYPE>? Get<TYPE>(NullableColumn<GuidKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new GuidKey<TYPE>(_reader.GetGuid(_ordinal));
        }

        public ShortKey<TYPE> Get<TYPE>(Column<ShortKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return ShortKey<TYPE>.NotSet;
            }
            return new ShortKey<TYPE>(_reader.GetInt16(_ordinal));

        }
        public ShortKey<TYPE>? Get<TYPE>(NullableColumn<ShortKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new ShortKey<TYPE>(_reader.GetInt16(_ordinal));
        }

        public IntKey<TYPE> Get<TYPE>(Column<IntKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return IntKey<TYPE>.NotSet;
            }
            return new IntKey<TYPE>(_reader.GetInt32(_ordinal));
        }
        public IntKey<TYPE>? Get<TYPE>(NullableColumn<IntKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new IntKey<TYPE>(_reader.GetInt32(_ordinal));
        }

        public LongKey<TYPE> Get<TYPE>(Column<LongKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return LongKey<TYPE>.NotSet;
            }
            return new LongKey<TYPE>(_reader.GetInt64(_ordinal));
        }
        public LongKey<TYPE>? Get<TYPE>(NullableColumn<LongKey<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new LongKey<TYPE>(_reader.GetInt64(_ordinal));
        }

        public BoolValue<TYPE> Get<TYPE>(Column<BoolValue<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return BoolValue<TYPE>.ValueOf(false);
            }
            return new BoolValue<TYPE>(_reader.GetBoolean(_ordinal));
        }
        public BoolValue<TYPE>? Get<TYPE>(NullableColumn<BoolValue<TYPE>> column) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new BoolValue<TYPE>(_reader.GetBoolean(_ordinal));
        }

        public string Get(Function<string> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            return _reader.GetString(_ordinal);
        }

        public string? Get(NullableFunction<string> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetString(_ordinal);
        }

        public Guid Get(Function<Guid> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Guid.Empty;
            }
            return _reader.GetGuid(_ordinal);
        }
        public Guid? Get(NullableFunction<Guid> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetGuid(_ordinal);
        }

        public bool Get(Function<bool> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return false;
            }
            return _reader.GetByte(_ordinal) == 1;
        }
        public bool? Get(NullableFunction<bool> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetByte(_ordinal) == 1;
        }

        public short Get(Function<short> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetInt16(_ordinal);
        }
        public short? Get(NullableFunction<short> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetInt16(_ordinal);
        }

        public int Get(Function<int> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetInt32(_ordinal);
        }
        public int? Get(NullableFunction<int> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetInt32(_ordinal);
        }

        public long Get(Function<long> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetInt64(_ordinal);
        }
        public long? Get(NullableFunction<long> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetInt64(_ordinal);
        }

        public float Get(Function<float> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return float.MinValue;
            }
            return _reader.GetFloat(_ordinal);
        }
        public float? Get(NullableFunction<float> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetFloat(_ordinal);
        }

        public double Get(Function<double> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return double.MinValue;
            }
            return _reader.GetDouble(_ordinal);
        }
        public double? Get(NullableFunction<double> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetDouble(_ordinal);
        }

        public DateTime Get(Function<DateTime> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return DateTime.MinValue;
            }
            return _reader.GetDateTime(_ordinal);
        }
        public DateTime? Get(NullableFunction<DateTime> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetDateTime(_ordinal);
        }

        public DateTimeOffset Get(Function<DateTimeOffset> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return DateTimeOffset.MinValue;
            }
            return (DateTimeOffset)_reader.GetValue(_ordinal);
        }
        public DateTimeOffset? Get(NullableFunction<DateTimeOffset> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return (DateTimeOffset)_reader.GetValue(_ordinal);
        }

        public byte Get(Function<byte> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return byte.MinValue;
            }
            return _reader.GetByte(_ordinal);
        }
        public byte? Get(NullableFunction<byte> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetByte(_ordinal);
        }

        public byte[] Get(Function<byte[]> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
            }
            return (byte[])_reader.GetValue(_ordinal);
        }
        public byte[]? Get(NullableFunction<byte[]> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return (byte[])_reader.GetValue(_ordinal);
        }

        public ENUM GetEnum<ENUM>(Function<ENUM> function) where ENUM : notnull, Enum {

            _ordinal++;

            Type fieldType = _reader.GetFieldType(_ordinal);

            if(_reader.IsDBNull(_ordinal)) {
                return IntegerToEnum<int, ENUM>.Convert(0);
            }

            int value;

            if(fieldType == typeof(byte)) {
                value = _reader.GetByte(_ordinal);
            }
            else if(fieldType == typeof(short)) {
                value = _reader.GetInt16(_ordinal);
            }
            else {
                value = _reader.GetInt32(_ordinal);
            }
            return IntegerToEnum<int, ENUM>.Convert(value);
        }

        public ENUM? GetEnum<ENUM>(NullableFunction<ENUM> function) where ENUM : notnull, Enum {

            _ordinal++;

            Type fieldType = _reader.GetFieldType(_ordinal);

            if(_reader.IsDBNull(_ordinal)) {
                return (ENUM?)(object?)null;
            }

            int value;

            if(fieldType == typeof(byte)) {
                value = _reader.GetByte(_ordinal);
            }
            else if(fieldType == typeof(short)) {
                value = _reader.GetInt16(_ordinal);
            }
            else {
                value = _reader.GetInt32(_ordinal);
            }
            return IntegerToEnum<int, ENUM>.Convert(value);
        }

        public StringKey<TYPE> Get<TYPE>(Function<StringKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return StringKey<TYPE>.NotSet;
            }
            return new StringKey<TYPE>(_reader.GetString(_ordinal));
        }
        public StringKey<TYPE>? Get<TYPE>(NullableFunction<StringKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new StringKey<TYPE>(_reader.GetString(_ordinal));
        }

        public GuidKey<TYPE> Get<TYPE>(Function<GuidKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return GuidKey<TYPE>.NotSet;
            }
            return new GuidKey<TYPE>(_reader.GetGuid(_ordinal));
        }

        public GuidKey<TYPE>? Get<TYPE>(NullableFunction<GuidKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new GuidKey<TYPE>(_reader.GetGuid(_ordinal));
        }

        public ShortKey<TYPE> Get<TYPE>(Function<ShortKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return ShortKey<TYPE>.NotSet;
            }
            return new ShortKey<TYPE>(_reader.GetInt16(_ordinal));

        }
        public ShortKey<TYPE>? Get<TYPE>(NullableFunction<ShortKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new ShortKey<TYPE>(_reader.GetInt16(_ordinal));
        }

        public IntKey<TYPE> Get<TYPE>(Function<IntKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return IntKey<TYPE>.NotSet;
            }
            return new IntKey<TYPE>(_reader.GetInt32(_ordinal));
        }
        public IntKey<TYPE>? Get<TYPE>(NullableFunction<IntKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new IntKey<TYPE>(_reader.GetInt32(_ordinal));
        }

        public LongKey<TYPE> Get<TYPE>(Function<LongKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return LongKey<TYPE>.NotSet;
            }
            return new LongKey<TYPE>(_reader.GetInt64(_ordinal));
        }
        public LongKey<TYPE>? Get<TYPE>(NullableFunction<LongKey<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new LongKey<TYPE>(_reader.GetInt64(_ordinal));
        }

        public BoolValue<TYPE> Get<TYPE>(Function<BoolValue<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return BoolValue<TYPE>.ValueOf(false);
            }
            return new BoolValue<TYPE>(_reader.GetBoolean(_ordinal));
        }
        public BoolValue<TYPE>? Get<TYPE>(NullableFunction<BoolValue<TYPE>> function) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return new BoolValue<TYPE>(_reader.GetBoolean(_ordinal));
        }
    }
}