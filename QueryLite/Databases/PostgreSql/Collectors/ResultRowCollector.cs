/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
using QueryLite.Utility;
using System.Data.Common;

namespace QueryLite.Databases.PostgreSql.Collectors {

    internal sealed class PostgreSqlResultRowCollector : IResultRow {

        private NpgsqlDataReader _reader;
        private int _ordinal = -1;

        public PostgreSqlResultRowCollector(DbDataReader reader) {
            _reader = (NpgsqlDataReader)reader;
        }

        void IResultRow.Reset() {
            _ordinal = -1;
        }

        internal void Reset(DbDataReader reader) {
            _reader = (NpgsqlDataReader)reader;
            _ordinal = -1;
        }

        internal void ReleaseReader() {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _reader = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        public string Get(Column<string> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return string.Empty;
            }
            return _reader.GetString(_ordinal);
        }

        public string? Get(NColumn<string> column) {

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
        public Guid? Get(NColumn<Guid> column) {

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
            return _reader.GetBoolean(_ordinal);
        }
        public bool? Get(NColumn<bool> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetBoolean(_ordinal);
        }

        public Bit Get(Column<Bit> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Bit.FALSE;
            }
            return _reader.GetBoolean(_ordinal) ? Bit.TRUE : Bit.FALSE;
        }
        public Bit? Get(NColumn<Bit> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetBoolean(_ordinal) ? Bit.TRUE : Bit.FALSE;
        }

        public decimal Get(Column<decimal> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return 0;
            }
            return _reader.GetDecimal(_ordinal);
        }
        public decimal? Get(NColumn<decimal> column) {

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
        public short? Get(NColumn<short> column) {

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
        public int? Get(NColumn<int> column) {

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
        public long? Get(NColumn<long> column) {

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
        public float? Get(NColumn<float> column) {

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
        public double? Get(NColumn<double> column) {

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
        public DateTime? Get(NColumn<DateTime> column) {

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
        public TimeOnly? Get(NColumn<TimeOnly> column) {

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
        public DateOnly? Get(NColumn<DateOnly> column) {

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
            //PostgreSql returns dates in UTC even though they were saved in a different time zone
            DateTime dateInUtc = _reader.GetDateTime(_ordinal);
            return new DateTimeOffset(dateInUtc);
        }
        public DateTimeOffset? Get(NColumn<DateTimeOffset> column) {

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
        public byte? Get(NColumn<byte> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetByte(_ordinal);
        }

        public byte[] Get(Column<byte[]> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return [];
            }
            return (byte[])_reader.GetValue(_ordinal);
        }
        public byte[]? Get(NColumn<byte[]> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return (byte[])_reader.GetValue(_ordinal);
        }

        public ENUM Get<ENUM>(Column<ENUM> column) where ENUM : struct, Enum {

            _ordinal++;

            Type fieldType = _reader.GetFieldType(_ordinal);

            if(_reader.IsDBNull(_ordinal)) {
                return IntegerToEnum<int, ENUM>.Convert(0);
            }

            if(fieldType == typeof(byte)) {
                return IntegerToEnum<byte, ENUM>.Convert(_reader.GetByte(_ordinal));
            }
            else if(fieldType == typeof(short)) {
                return IntegerToEnum<int, ENUM>.Convert(_reader.GetInt16(_ordinal));
            }
            else if(fieldType == typeof(long)) {
                return IntegerToEnum<long, ENUM>.Convert(_reader.GetInt64(_ordinal));
            }
            else {
                return IntegerToEnum<int, ENUM>.Convert(_reader.GetInt32(_ordinal));
            }
        }

        public ENUM? Get<ENUM>(NColumn<ENUM> column) where ENUM : struct, Enum {

            _ordinal++;

            Type fieldType = _reader.GetFieldType(_ordinal);

            if(_reader.IsDBNull(_ordinal)) {
                return (ENUM?)(object?)null;
            }

            if(fieldType == typeof(byte)) {
                return IntegerToEnum<byte, ENUM>.Convert(_reader.GetByte(_ordinal));
            }
            else if(fieldType == typeof(short)) {
                return IntegerToEnum<int, ENUM>.Convert(_reader.GetInt16(_ordinal));
            }
            else if(fieldType == typeof(long)) {
                return IntegerToEnum<long, ENUM>.Convert(_reader.GetInt64(_ordinal));
            }
            else {
                return IntegerToEnum<int, ENUM>.Convert(_reader.GetInt32(_ordinal));
            }
        }

        public string Get(Function<string> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return string.Empty;
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
            return _reader.GetBoolean(_ordinal);
        }
        public bool? Get(NullableFunction<bool> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return _reader.GetBoolean(_ordinal);
        }


        public Bit Get(Function<Bit> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Bit.ValueOf(false);
            }

            return Bit.ValueOf(_reader.GetBoolean(_ordinal));
        }

        public Bit? Get(NullableFunction<Bit> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return Bit.ValueOf(_reader.GetBoolean(_ordinal));
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

        public DateOnly Get(Function<DateOnly> column) {
            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return DateOnly.MinValue;
            }
            DateTime value = _reader.GetDateTime(_ordinal);
            return DateOnly.FromDateTime(value);
        }
        public DateOnly? Get(NullableFunction<DateOnly> column) {
            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            DateTime value = _reader.GetDateTime(_ordinal);
            return DateOnly.FromDateTime(value);
        }

        public TimeOnly Get(Function<TimeOnly> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return TimeOnly.MinValue;
            }
            TimeSpan value = _reader.GetTimeSpan(_ordinal);
            return TimeOnly.FromTimeSpan(value);
        }
        public TimeOnly? Get(NullableFunction<TimeOnly> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            TimeSpan value = _reader.GetTimeSpan(_ordinal);
            return TimeOnly.FromTimeSpan(value);
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
                return [];
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

        public ENUM Get<ENUM>(Function<ENUM> function) where ENUM : struct, Enum {

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

        public ENUM? Get<ENUM>(NullableFunction<ENUM> function) where ENUM : struct, Enum {

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

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf(Guid.Empty);
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetGuid(_ordinal));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetGuid(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf(0);
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetInt16(_ordinal));
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetInt16(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf(0);
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetInt32(_ordinal));
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetInt32(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf(0);
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetInt64(_ordinal));
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetInt64(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf("");
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetString(_ordinal));
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetString(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf(false);
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetBoolean(_ordinal));
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetBoolean(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return CUSTOM_TYPE.ValueOf(0);
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetDecimal(_ordinal));
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetDecimal(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetDateTime(_ordinal));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetDateTime(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            //PostgreSql returns dates in UTC even though they were saved in a different time zone
            DateTime dateInUtc = _reader.GetDateTime(_ordinal);
            return CUSTOM_TYPE.ValueOf(new DateTimeOffset(dateInUtc));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            //PostgreSql returns dates in UTC even though they were saved in a different time zone
            DateTime dateInUtc = _reader.GetDateTime(_ordinal);
            return CUSTOM_TYPE.ValueOf(new DateTimeOffset(dateInUtc));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            DateTime value = _reader.GetDateTime(_ordinal);
            return CUSTOM_TYPE.ValueOf(DateOnly.FromDateTime(value));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            DateTime value = _reader.GetDateTime(_ordinal);
            return CUSTOM_TYPE.ValueOf(DateOnly.FromDateTime(value));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            TimeSpan value = _reader.GetTimeSpan(_ordinal);
            return CUSTOM_TYPE.ValueOf(TimeOnly.FromTimeSpan(value));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            TimeSpan value = _reader.GetTimeSpan(_ordinal);
            return CUSTOM_TYPE.ValueOf(TimeOnly.FromTimeSpan(value));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetFloat(_ordinal));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetFloat(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetDouble(_ordinal));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetDouble(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetBoolean(_ordinal) ? Bit.TRUE : Bit.FALSE);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(_reader.GetBoolean(_ordinal) ? Bit.TRUE : Bit.FALSE);
        }

        public TYPE LoadFromReader<TYPE>(Column<TYPE> column, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return @default;
            }
            return readValue(_reader, _ordinal);
        }

        public TYPE? LoadFromReader<TYPE>(NColumn<TYPE> column, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return readValue(_reader, _ordinal);
        }

        public CUSTOM_TYPE LoadFromReader<CUSTOM_TYPE, TYPE>(Column<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue, CUSTOM_TYPE @default)
                                                 where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                 where TYPE : notnull {
            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return @default;
            }
            return readValue(_reader, _ordinal);
        }

        public CUSTOM_TYPE? LoadFromReader<CUSTOM_TYPE, TYPE>(NColumn<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue)
                                                  where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                  where TYPE : notnull {
            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return readValue(_reader, _ordinal);
        }

        public TYPE LoadFromReader<TYPE>(Function<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            
            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return @default;
            }
            return readValue(_reader, _ordinal);
        }

        public TYPE? LoadFromReader<TYPE>(NullableFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return readValue(_reader, _ordinal);
        }

        public TYPE LoadFromReader<TYPE>(RawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return @default;
            }
            return readValue(_reader, _ordinal);
        }

        public TYPE? LoadFromReader<TYPE>(NullableRawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return readValue(_reader, _ordinal);
        }

        public Json Get(Function<Json> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Json.Empty;
            }
            return Json.ValueOf(_reader.GetString(_ordinal));
        }

        public Json? Get(NullableFunction<Json> function) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return Json.ValueOf(_reader.GetString(_ordinal));
        }

        public Jsonb Get(Function<Jsonb> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Jsonb.Empty;
            }
            return Jsonb.ValueOf(_reader.GetString(_ordinal));
        }

        public Jsonb? Get(NullableFunction<Jsonb> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return Jsonb.ValueOf(_reader.GetString(_ordinal));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return CUSTOM_TYPE.ValueOf(Json.ValueOf(_reader.GetString(_ordinal)));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(Json.ValueOf(_reader.GetString(_ordinal)));
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return default;
            }
            return CUSTOM_TYPE.ValueOf(Jsonb.ValueOf(_reader.GetString(_ordinal)));
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return CUSTOM_TYPE.ValueOf(Jsonb.ValueOf(_reader.GetString(_ordinal)));
        }

        public Json Get(Column<Json> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Json.Empty;
            }
            return Json.ValueOf(_reader.GetString(_ordinal));
        }

        public Json? Get(NColumn<Json> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return Json.ValueOf(_reader.GetString(_ordinal));
        }

        public Jsonb Get(Column<Jsonb> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return Jsonb.Empty;
            }
            return Jsonb.ValueOf(_reader.GetString(_ordinal));
        }

        public Jsonb? Get(NColumn<Jsonb> column) {

            _ordinal++;

            if(_reader.IsDBNull(_ordinal)) {
                return null;
            }
            return Jsonb.ValueOf(_reader.GetString(_ordinal));
        }
    }
}