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
using System;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerReturningFieldCollector : IResultRow {

        private bool _isDelete;
        private StringBuilder? _sql;

        private int _counter;

        public SqlServerReturningFieldCollector(bool isDelete, StringBuilder sql) {
            _isDelete = isDelete;
            _sql = sql;
        }

        public void Reset(bool isDelete, StringBuilder sql) {
            _isDelete = isDelete;
            _sql = sql;
            _counter = 0;
        }

        public void Clear() {
            _sql = null;
            _counter = 0;
        }

        void IResultRow.Reset() {

        }

        private VALUE Add<VALUE>(AColumn<VALUE> column) where VALUE : notnull {

            if(_counter > 0) {
                _sql!.Append(',');
            }
            else {
                _counter++;
            }

            if(_isDelete) {
                _sql!.Append("DELETED.");
            }
            else {
                _sql!.Append("INSERTED.");  //Note: for sql server 'INSERTED' is used for both insert and update queries
            }

            SqlHelper.AppendEncloseColumnName(_sql!, column);

#pragma warning disable CS8603 // Possible null reference return.
            return default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private VALUE Add<VALUE>(AFunction<VALUE> function) where VALUE : notnull {

            throw new NotSupportedException();
        }

        public string Get(Column<string> column) {
            return Add(column);
        }

        public string? Get(NullableColumn<string> column) {
            return Add(column);
        }

        public Guid Get(Column<Guid> column) {
            return Add(column);
        }

        public Guid? Get(NullableColumn<Guid> column) {
            return Add(column);
        }

        public bool Get(Column<bool> column) {
            return Add(column);
        }

        public bool? Get(NullableColumn<bool> column) {
            return Add(column);
        }

        public Bit Get(Column<Bit> column) {
            return Add(column);
        }

        public Bit? Get(NullableColumn<Bit> column) {
            return Add(column);
        }

        public decimal Get(Column<decimal> column) {
            return Add(column);
        }

        public decimal? Get(NullableColumn<decimal> column) {
            return Add(column);
        }

        public short Get(Column<short> column) {
            return Add(column);
        }

        public short? Get(NullableColumn<short> column) {
            return Add(column);
        }

        public int Get(Column<int> column) {
            return Add(column);
        }

        public int? Get(NullableColumn<int> column) {
            return Add(column);
        }

        public long Get(Column<long> column) {
            return Add(column);
        }

        public long? Get(NullableColumn<long> column) {
            return Add(column);
        }

        public float Get(Column<float> column) {
            return Add(column);
        }

        public float? Get(NullableColumn<float> column) {
            return Add(column);
        }

        public double Get(Column<double> column) {
            return Add(column);
        }

        public double? Get(NullableColumn<double> column) {
            return Add(column);
        }

        public TimeOnly Get(Column<TimeOnly> column) {
            return Add(column);
        }

        public TimeOnly? Get(NullableColumn<TimeOnly> column) {
            return Add(column);
        }

        public DateTime Get(Column<DateTime> column) {
            return Add(column);
        }

        public DateTime? Get(NullableColumn<DateTime> column) {
            return Add(column);
        }

        public DateOnly Get(Column<DateOnly> column) {
            return Add(column);
        }

        public DateOnly? Get(NullableColumn<DateOnly> column) {
            return Add(column);
        }

        public DateTimeOffset Get(Column<DateTimeOffset> column) {
            return Add(column);
        }

        public DateTimeOffset? Get(NullableColumn<DateTimeOffset> column) {
            return Add(column);
        }

        public byte Get(Column<byte> column) {
            return Add(column);
        }

        public byte? Get(NullableColumn<byte> column) {
            return Add(column);
        }

        public byte[] Get(Column<byte[]> column) {
            return Add(column);
        }

        public byte[]? Get(NullableColumn<byte[]> column) {
            return Add(column);
        }

        public StringKey<TYPE> Get<TYPE>(Column<StringKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public StringKey<TYPE>? Get<TYPE>(NullableColumn<StringKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public GuidKey<TYPE> Get<TYPE>(Column<GuidKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public GuidKey<TYPE>? Get<TYPE>(NullableColumn<GuidKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public ShortKey<TYPE> Get<TYPE>(Column<ShortKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public ShortKey<TYPE>? Get<TYPE>(NullableColumn<ShortKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public IntKey<TYPE> Get<TYPE>(Column<IntKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public IntKey<TYPE>? Get<TYPE>(NullableColumn<IntKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public LongKey<TYPE> Get<TYPE>(Column<LongKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public LongKey<TYPE>? Get<TYPE>(NullableColumn<LongKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public BoolValue<TYPE> Get<TYPE>(Column<BoolValue<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public BoolValue<TYPE>? Get<TYPE>(NullableColumn<BoolValue<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public string Get(Function<string> column) {
            return Add(column);
        }

        public string? Get(NullableFunction<string> column) {
            return Add(column);
        }

        public Guid Get(Function<Guid> column) {
            return Add(column);
        }

        public Guid? Get(NullableFunction<Guid> column) {
            return Add(column);
        }

        public bool Get(Function<bool> column) {
            return Add(column);
        }

        public bool? Get(NullableFunction<bool> column) {
            return Add(column);
        }

        public Bit Get(Function<Bit> column) {
            return Add(column);
        }

        public Bit? Get(NullableFunction<Bit> column) {
            return Add(column);
        }

        public short Get(Function<short> column) {
            return Add(column);
        }

        public short? Get(NullableFunction<short> column) {
            return Add(column);
        }

        public int Get(Function<int> column) {
            return Add(column);
        }

        public int? Get(NullableFunction<int> column) {
            return Add(column);
        }

        public long Get(Function<long> column) {
            return Add(column);
        }

        public long? Get(NullableFunction<long> column) {
            return Add(column);
        }

        public float Get(Function<float> column) {
            return Add(column);
        }

        public float? Get(NullableFunction<float> column) {
            return Add(column);
        }

        public double Get(Function<double> column) {
            return Add(column);
        }

        public double? Get(NullableFunction<double> column) {
            return Add(column);
        }

        public DateTime Get(Function<DateTime> column) {
            return Add(column);
        }

        public DateTime? Get(NullableFunction<DateTime> column) {
            return Add(column);
        }

        public DateTimeOffset Get(Function<DateTimeOffset> column) {
            return Add(column);
        }

        public DateTimeOffset? Get(NullableFunction<DateTimeOffset> column) {
            return Add(column);
        }

        public DateOnly Get(Function<DateOnly> column) {
            return Add(column);
        }

        public DateOnly? Get(NullableFunction<DateOnly> column) {
            return Add(column);
        }

        public TimeOnly Get(Function<TimeOnly> column) {
            return Add(column);
        }

        public TimeOnly? Get(NullableFunction<TimeOnly> column) {
            return Add(column);
        }

        public byte Get(Function<byte> column) {
            return Add(column);
        }

        public byte? Get(NullableFunction<byte> column) {
            return Add(column);
        }

        public byte[] Get(Function<byte[]> column) {
            return Add(column);
        }

        public byte[]? Get(NullableFunction<byte[]> column) {
            return Add(column);
        }

        public StringKey<TYPE> Get<TYPE>(Function<StringKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public StringKey<TYPE>? Get<TYPE>(NullableFunction<StringKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public GuidKey<TYPE> Get<TYPE>(Function<GuidKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public GuidKey<TYPE>? Get<TYPE>(NullableFunction<GuidKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public ShortKey<TYPE> Get<TYPE>(Function<ShortKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public ShortKey<TYPE>? Get<TYPE>(NullableFunction<ShortKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public IntKey<TYPE> Get<TYPE>(Function<IntKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public IntKey<TYPE>? Get<TYPE>(NullableFunction<IntKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public LongKey<TYPE> Get<TYPE>(Function<LongKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public LongKey<TYPE>? Get<TYPE>(NullableFunction<LongKey<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public BoolValue<TYPE> Get<TYPE>(Function<BoolValue<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public BoolValue<TYPE>? Get<TYPE>(NullableFunction<BoolValue<TYPE>> column) where TYPE : notnull {
            return Add(column);
        }

        public ENUM Get<ENUM>(Column<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public ENUM? Get<ENUM>(NullableColumn<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public ENUM Get<ENUM>(Function<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public ENUM? Get<ENUM>(NullableFunction<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public CUSTOM_TYPE GetGuid<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<Guid, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetGuid<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<Guid, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetShort<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<short, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetShort<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<short, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetInt<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<int, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetInt<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<int, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetLong<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<long, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetLong<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<long, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetString<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<string, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetString<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<string, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetBool<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<bool, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetBool<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<bool, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetDecimal<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<decimal, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? GetDecimal<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<decimal, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetDateTime<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTime, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetDateTime<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTime, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetDateTimeOffset<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTimeOffset, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetDateTimeOffset<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTimeOffset, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetDateOnly<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetDateOnly<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetTimeOnly<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<TimeOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetTimeOnly<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<TimeOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetFloat<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<float, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetFloat<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<float, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetDouble<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<double, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetDouble<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<double, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE GetBit<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<Bit, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? GetBit<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<Bit, CUSTOM_TYPE> {
            return Add(column);
        }
    }
}