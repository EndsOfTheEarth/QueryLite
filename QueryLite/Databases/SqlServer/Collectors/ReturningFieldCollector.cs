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
using QueryLite.Utility;
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

            SqlHelper.AppendEncloseColumnName(_sql!, column, EncloseWith.SquareBracket);

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

        public string? Get(NColumn<string> column) {
            return Add(column);
        }

        public Guid Get(Column<Guid> column) {
            return Add(column);
        }

        public Guid? Get(NColumn<Guid> column) {
            return Add(column);
        }

        public bool Get(Column<bool> column) {
            return Add(column);
        }

        public bool? Get(NColumn<bool> column) {
            return Add(column);
        }

        public Bit Get(Column<Bit> column) {
            return Add(column);
        }

        public Bit? Get(NColumn<Bit> column) {
            return Add(column);
        }

        public decimal Get(Column<decimal> column) {
            return Add(column);
        }

        public decimal? Get(NColumn<decimal> column) {
            return Add(column);
        }

        public short Get(Column<short> column) {
            return Add(column);
        }

        public short? Get(NColumn<short> column) {
            return Add(column);
        }

        public int Get(Column<int> column) {
            return Add(column);
        }

        public int? Get(NColumn<int> column) {
            return Add(column);
        }

        public long Get(Column<long> column) {
            return Add(column);
        }

        public long? Get(NColumn<long> column) {
            return Add(column);
        }

        public float Get(Column<float> column) {
            return Add(column);
        }

        public float? Get(NColumn<float> column) {
            return Add(column);
        }

        public double Get(Column<double> column) {
            return Add(column);
        }

        public double? Get(NColumn<double> column) {
            return Add(column);
        }

        public TimeOnly Get(Column<TimeOnly> column) {
            return Add(column);
        }

        public TimeOnly? Get(NColumn<TimeOnly> column) {
            return Add(column);
        }

        public DateTime Get(Column<DateTime> column) {
            return Add(column);
        }

        public DateTime? Get(NColumn<DateTime> column) {
            return Add(column);
        }

        public DateOnly Get(Column<DateOnly> column) {
            return Add(column);
        }

        public DateOnly? Get(NColumn<DateOnly> column) {
            return Add(column);
        }

        public DateTimeOffset Get(Column<DateTimeOffset> column) {
            return Add(column);
        }

        public DateTimeOffset? Get(NColumn<DateTimeOffset> column) {
            return Add(column);
        }

        public byte Get(Column<byte> column) {
            return Add(column);
        }

        public byte? Get(NColumn<byte> column) {
            return Add(column);
        }

        public byte[] Get(Column<byte[]> column) {
            return Add(column);
        }

        public byte[]? Get(NColumn<byte[]> column) {
            return Add(column);
        }

        public string Get(Function<string> column) {
            return Add(column);
        }

        public string? Get(NFunction<string> column) {
            return Add(column);
        }

        public Guid Get(Function<Guid> column) {
            return Add(column);
        }

        public Guid? Get(NFunction<Guid> column) {
            return Add(column);
        }

        public bool Get(Function<bool> column) {
            return Add(column);
        }

        public bool? Get(NFunction<bool> column) {
            return Add(column);
        }

        public Bit Get(Function<Bit> column) {
            return Add(column);
        }

        public Bit? Get(NFunction<Bit> column) {
            return Add(column);
        }

        public short Get(Function<short> column) {
            return Add(column);
        }

        public short? Get(NFunction<short> column) {
            return Add(column);
        }

        public int Get(Function<int> column) {
            return Add(column);
        }

        public int? Get(NFunction<int> column) {
            return Add(column);
        }

        public long Get(Function<long> column) {
            return Add(column);
        }

        public long? Get(NFunction<long> column) {
            return Add(column);
        }

        public float Get(Function<float> column) {
            return Add(column);
        }

        public float? Get(NFunction<float> column) {
            return Add(column);
        }

        public double Get(Function<double> column) {
            return Add(column);
        }

        public double? Get(NFunction<double> column) {
            return Add(column);
        }

        public DateTime Get(Function<DateTime> column) {
            return Add(column);
        }

        public DateTime? Get(NFunction<DateTime> column) {
            return Add(column);
        }

        public DateTimeOffset Get(Function<DateTimeOffset> column) {
            return Add(column);
        }

        public DateTimeOffset? Get(NFunction<DateTimeOffset> column) {
            return Add(column);
        }

        public DateOnly Get(Function<DateOnly> column) {
            return Add(column);
        }

        public DateOnly? Get(NFunction<DateOnly> column) {
            return Add(column);
        }

        public TimeOnly Get(Function<TimeOnly> column) {
            return Add(column);
        }

        public TimeOnly? Get(NFunction<TimeOnly> column) {
            return Add(column);
        }

        public byte Get(Function<byte> column) {
            return Add(column);
        }

        public byte? Get(NFunction<byte> column) {
            return Add(column);
        }

        public byte[] Get(Function<byte[]> column) {
            return Add(column);
        }

        public byte[]? Get(NFunction<byte[]> column) {
            return Add(column);
        }

        public ENUM Get<ENUM>(Column<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public ENUM? Get<ENUM>(NColumn<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public ENUM Get<ENUM>(Function<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public ENUM? Get<ENUM>(NFunction<ENUM> column) where ENUM : struct, Enum {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return Add(column);
        }

        public TYPE LoadFromReader<TYPE>(Column<TYPE> column, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            return Add(column);
        }
        public TYPE? LoadFromReader<TYPE>(NColumn<TYPE> column, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            return Add(column);
        }

        public CUSTOM_TYPE LoadFromReader<CUSTOM_TYPE, TYPE>(Column<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue, CUSTOM_TYPE @default)
                                                      where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                      where TYPE : notnull {
            Add(column);
            return @default;
        }

        public CUSTOM_TYPE? LoadFromReader<CUSTOM_TYPE, TYPE>(NColumn<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue)
                                                       where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                       where TYPE : notnull {
           Add(column);
           return default;
        }

        public TYPE LoadFromReader<TYPE>(Function<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Add(function);
            return @default;
        }

        public TYPE? LoadFromReader<TYPE>(NFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Add(function);
            return default;
        }

        public TYPE LoadFromReader<TYPE>(RawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Add(function);
            return @default;
        }

        public TYPE? LoadFromReader<TYPE>(NRawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Add(function);
            return default;
        }

        public Json Get(Function<Json> function) {
            return Add(function);
        }

        public Json? Get(NFunction<Json> function) {
            return Add(function);
        }

        public Jsonb Get(Function<Jsonb> function) {
            return Add(function);
        }

        public Jsonb? Get(NFunction<Jsonb> function) {
            return Add(function);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            return Add(column);
        }

        public Json Get(Column<Json> column) {
            return Add(column);
        }
        public Json? Get(NColumn<Json> column) {
            return Add(column);
        }

        public Jsonb Get(Column<Jsonb> column) {
            return Add(column);
        }
        public Jsonb? Get(NColumn<Jsonb> column) {
            return Add(column);
        }
    }
}