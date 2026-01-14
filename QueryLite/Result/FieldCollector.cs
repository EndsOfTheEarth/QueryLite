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

namespace QueryLite {

    internal sealed class FieldCollector : IResultRow {

        void IResultRow.Reset() {
            Fields.Clear();
        }
        public List<IField> Fields { get; } = [];

        public string Get(Column<string> column) {
            Fields.Add(column);
            return "";
        }
        public string? Get(NColumn<string> column) {
            Fields.Add(column);
            return default;
        }
        public string? GetAsNull(Column<string> column) {
            Fields.Add(column);
            return default;
        }

        public Guid Get(Column<Guid> column) {
            Fields.Add(column);
            return default;
        }
        public Guid? Get(NColumn<Guid> column) {
            Fields.Add(column);
            return default;
        }
        public Guid? GetAsNull(Column<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public bool Get(Column<bool> column) {
            Fields.Add(column);
            return default;
        }
        public bool? Get(NColumn<bool> column) {
            Fields.Add(column);
            return default;
        }
        public bool? GetAsNull(Column<bool> column) {
            Fields.Add(column);
            return default;
        }

        public Bit Get(Column<Bit> column) {
            Fields.Add(column);
            return default;
        }
        public Bit? Get(NColumn<Bit> column) {
            Fields.Add(column);
            return default;
        }
        public Bit? GetAsNull(Column<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public decimal Get(Column<decimal> column) {
            Fields.Add(column);
            return default;
        }
        public decimal? Get(NColumn<decimal> column) {
            Fields.Add(column);
            return default;
        }
        public decimal? GetAsNull(Column<decimal> column) {
            Fields.Add(column);
            return default;
        }

        public short Get(Column<short> column) {
            Fields.Add(column);
            return default;
        }
        public short? Get(NColumn<short> column) {
            Fields.Add(column);
            return default;
        }
        public short? GetAsNull(Column<short> column) {
            Fields.Add(column);
            return default;
        }

        public int Get(Column<int> column) {
            Fields.Add(column);
            return default;
        }

        public int? Get(NColumn<int> column) {
            Fields.Add(column);
            return default;
        }

        public long Get(Column<long> column) {
            Fields.Add(column);
            return default;
        }
        public long? Get(NColumn<long> column) {
            Fields.Add(column);
            return default;
        }
        public long? GetAsNull(Column<long> column) {
            Fields.Add(column);
            return default;
        }

        public float Get(Column<float> column) {
            Fields.Add(column);
            return default;
        }
        public float? Get(NColumn<float> column) {
            Fields.Add(column);
            return default;
        }
        public float? GetAsNull(Column<float> column) {
            Fields.Add(column);
            return default;
        }

        public double Get(Column<double> column) {
            Fields.Add(column);
            return default;
        }
        public double? Get(NColumn<double> column) {
            Fields.Add(column);
            return default;
        }
        public double? GetAsNull(Column<double> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime Get(Column<DateTime> column) {
            Fields.Add(column);
            return default;
        }
        public DateTime? Get(NColumn<DateTime> column) {
            Fields.Add(column);
            return default;
        }
        public DateTime? GetAsNull(Column<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly Get(Column<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }
        public TimeOnly? Get(NColumn<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }
        public TimeOnly? GetAsNull(Column<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly Get(Column<DateOnly> column) {
            Fields.Add(column);
            return default;
        }
        public DateOnly? Get(NColumn<DateOnly> column) {
            Fields.Add(column);
            return default;
        }
        public DateOnly? GetAsNull(Column<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset Get(Column<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }
        public DateTimeOffset? Get(NColumn<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }
        public DateTimeOffset? GetAsNull(Column<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public byte Get(Column<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte? Get(NColumn<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte[] Get(Column<byte[]> column) {
            Fields.Add(column);
            return [];
        }

        public byte[]? Get(NColumn<byte[]> column) {
            Fields.Add(column);
            return default;
        }

        public string Get(Function<string> column) {
            Fields.Add(column);
            return "";
        }

        public string? Get(NFunction<string> column) {
            Fields.Add(column);
            return default;
        }

        public Guid Get(Function<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public Guid? Get(NFunction<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public bool Get(Function<bool> column) {
            Fields.Add(column);
            return default;
        }

        public bool? Get(NFunction<bool> column) {
            Fields.Add(column);
            return default;
        }

        public Bit Get(Function<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public Bit? Get(NFunction<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public short Get(Function<short> column) {
            Fields.Add(column);
            return default;
        }

        public short? Get(NFunction<short> column) {
            Fields.Add(column);
            return default;
        }

        public int Get(Function<int> column) {
            Fields.Add(column);
            return default;
        }

        public int? Get(NFunction<int> column) {
            Fields.Add(column);
            return default;
        }

        public long Get(Function<long> column) {
            Fields.Add(column);
            return default;
        }

        public long? Get(NFunction<long> column) {
            Fields.Add(column);
            return default;
        }

        public float Get(Function<float> column) {
            Fields.Add(column);
            return default;
        }

        public float? Get(NFunction<float> column) {
            Fields.Add(column);
            return default;
        }

        public double Get(Function<double> column) {
            Fields.Add(column);
            return default;
        }

        public double? Get(NFunction<double> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime Get(Function<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime? Get(NFunction<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset Get(Function<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset? Get(NFunction<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly Get(Function<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly? Get(NFunction<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly Get(Function<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly? Get(NFunction<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public byte Get(Function<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte? Get(NFunction<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte[] Get(Function<byte[]> column) {
            Fields.Add(column);
            return [];
        }

        public byte[]? Get(NFunction<byte[]> column) {
            Fields.Add(column);
            return default;
        }

        public ENUM Get<ENUM>(Column<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default!;
        }

        public ENUM? Get<ENUM>(NColumn<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default;
        }

        public ENUM Get<ENUM>(Function<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default!;
        }

        public ENUM? Get<ENUM>(NFunction<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public TYPE LoadFromReader<TYPE>(Column<TYPE> column, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Fields.Add(column);
            return @default;
        }
        public TYPE? LoadFromReader<TYPE>(NColumn<TYPE> column, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE LoadFromReader<CUSTOM_TYPE, TYPE>(Column<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue, CUSTOM_TYPE @default)
                                                      where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                      where TYPE : notnull {
            Fields.Add(column);
            return @default;
        }

        public CUSTOM_TYPE? LoadFromReader<CUSTOM_TYPE, TYPE>(NColumn<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue)
                                                       where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE>
                                                       where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public TYPE LoadFromReader<TYPE>(Function<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Fields.Add(function);
            return @default;
        }

        public TYPE? LoadFromReader<TYPE>(NFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Fields.Add(function);
            return default;
        }

        public TYPE LoadFromReader<TYPE>(RawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull {
            Fields.Add(function);
            return @default;
        }

        public TYPE? LoadFromReader<TYPE>(NRawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull {
            Fields.Add(function);
            return default;
        }

        public Json Get(Function<Json> function) {
            Fields.Add(function);
            return Json.Empty;
        }

        public Json? Get(NFunction<Json> function) {
            Fields.Add(function);
            return null;
        }

        public Jsonb Get(Function<Jsonb> column) {
            Fields.Add(column);
            return Jsonb.Empty;
        }

        public Jsonb? Get(NFunction<Jsonb> column) {
            Fields.Add(column);
            return null;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE> {
            Fields.Add(column);
            return default;
        }

        public Json Get(Column<Json> column) {
            Fields.Add(column);
            return default;
        }
        public Json? Get(NColumn<Json> column) {
            Fields.Add(column);
            return default;
        }

        public Jsonb Get(Column<Jsonb> column) {
            Fields.Add(column);
            return default;
        }
        public Jsonb? Get(NColumn<Jsonb> column) {
            Fields.Add(column);
            return default;
        }
    }
}