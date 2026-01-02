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
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlSelectFieldCollector : IResultRow {

        private IDatabase? _database;
        private IParametersBuilder? _parameterBuilder;
        private bool _useAlias;
        private StringBuilder? _sql;

        private int _counter;

        public PostgreSqlSelectFieldCollector(IDatabase database, IParametersBuilder? parameterBuilder, bool useAlias, StringBuilder sql) {
            _database = database;
            _parameterBuilder = parameterBuilder;
            _useAlias = useAlias;
            _sql = sql;
        }

        public void Reset(IDatabase database, IParametersBuilder? parameterBuilder, bool useAlias, StringBuilder sql) {
            _database = database;
            _parameterBuilder = parameterBuilder;
            _useAlias = useAlias;
            _sql = sql;
            _counter = 0;
        }

        public void Clear() {
            _database = null;
            _parameterBuilder = null;
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
            if(_useAlias) {
                _sql!.Append(column.Table.Alias).Append('.');
            }

            SqlHelper.AppendEncloseColumnName(_sql!, column);

#pragma warning disable CS8603 // Possible null reference return.
            return default;
#pragma warning restore CS8603 // Possible null reference return.
        }

        private VALUE Add<VALUE>(AFunction<VALUE> function) where VALUE : notnull {

            if(_counter > 0) {
                _sql!.Append(',');
            }
            else {
                _counter++;
            }

            _sql!.Append(function.GetSql(_database!, _useAlias, _parameterBuilder));

#pragma warning disable CS8603 // Possible null reference return.
            return default;
#pragma warning restore CS8603 // Possible null reference return.
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

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return Add(column);
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return Add(column);
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            return Add(column);
        }

        public TYPE LoadFromReader<TYPE>(Column<TYPE> column, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            return Add(column);
        }
        public TYPE? LoadFromReader<TYPE>(NullableColumn<TYPE> column, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            return Add(column);
        }

        public CUSTOM_TYPE LoadFromReader<CUSTOM_TYPE, TYPE>(Column<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue, CUSTOM_TYPE @default)
                                                      where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                      where TYPE : notnull {
            Add(column);
            return @default;
        }

        public CUSTOM_TYPE? LoadFromReader<CUSTOM_TYPE, TYPE>(NullableColumn<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue)
                                                       where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                       where TYPE : notnull {
            Add(column);
            return default;
        }

        public TYPE LoadFromReader<TYPE>(Function<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Add(function);
            return @default;
        }

        public TYPE? LoadFromReader<TYPE>(NullableFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Add(function);
            return default;
        }

        public TYPE LoadFromReader<TYPE>(RawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Add(function);
            return @default;
        }

        public TYPE? LoadFromReader<TYPE>(NullableRawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Add(function);
            return default;
        }
    }
}