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
using System;
using System.Collections.Generic;

namespace QueryLite {

    internal sealed class FieldCollector : IResultRow {

        void IResultRow.Reset() {
            Fields.Clear();
        }
        public List<IField> Fields { get; } = new List<IField>();

        public List<IColumn> GetFieldsAsColumns() {

            List<IColumn> columns = new List<IColumn>();

            foreach(IField field in Fields) {

                if(field is not IColumn) {
                    throw new Exception($"Field is expected to be of type {typeof(IColumn)}. Other types like functions are not allowed.");
                }
                columns.Add((IColumn)field);
            }
            return columns;
        }

        public string Get(Column<string> column) {
            Fields.Add(column);
            return string.Empty;
        }

        public string? Get(NullableColumn<string> column) {
            Fields.Add(column);
            return default;
        }

        public Guid Get(Column<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public Guid? Get(NullableColumn<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public bool Get(Column<bool> column) {
            Fields.Add(column);
            return default;
        }

        public bool? Get(NullableColumn<bool> column) {
            Fields.Add(column);
            return default;
        }

        public Bit Get(Column<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public Bit? Get(NullableColumn<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public decimal Get(Column<decimal> column) {
            Fields.Add(column);
            return default;
        }

        public decimal? Get(NullableColumn<decimal> column) {
            Fields.Add(column);
            return default;
        }

        public short Get(Column<short> column) {
            Fields.Add(column);
            return default;
        }

        public short? Get(NullableColumn<short> column) {
            Fields.Add(column);
            return default;
        }

        public int Get(Column<int> column) {
            Fields.Add(column);
            return default;
        }

        public int? Get(NullableColumn<int> column) {
            Fields.Add(column);
            return default;
        }

        public long Get(Column<long> column) {
            Fields.Add(column);
            return default;
        }

        public long? Get(NullableColumn<long> column) {
            Fields.Add(column);
            return default;
        }

        public float Get(Column<float> column) {
            Fields.Add(column);
            return default;
        }

        public float? Get(NullableColumn<float> column) {
            Fields.Add(column);
            return default;
        }

        public double Get(Column<double> column) {
            Fields.Add(column);
            return default;
        }

        public double? Get(NullableColumn<double> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime Get(Column<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime? Get(NullableColumn<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly Get(Column<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly? Get(NullableColumn<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly Get(Column<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly? Get(NullableColumn<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset Get(Column<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset? Get(NullableColumn<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public byte Get(Column<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte? Get(NullableColumn<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte[] Get(Column<byte[]> column) {
            Fields.Add(column);
            return Array.Empty<byte>();
        }

        public byte[]? Get(NullableColumn<byte[]> column) {
            Fields.Add(column);
            return default;
        }

        public StringKey<TYPE> Get<TYPE>(Column<StringKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public StringKey<TYPE>? Get<TYPE>(NullableColumn<StringKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public GuidKey<TYPE> Get<TYPE>(Column<GuidKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public GuidKey<TYPE>? Get<TYPE>(NullableColumn<GuidKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public ShortKey<TYPE> Get<TYPE>(Column<ShortKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public ShortKey<TYPE>? Get<TYPE>(NullableColumn<ShortKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public IntKey<TYPE> Get<TYPE>(Column<IntKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public IntKey<TYPE>? Get<TYPE>(NullableColumn<IntKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public LongKey<TYPE> Get<TYPE>(Column<LongKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public LongKey<TYPE>? Get<TYPE>(NullableColumn<LongKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public BoolValue<TYPE> Get<TYPE>(Column<BoolValue<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public BoolValue<TYPE>? Get<TYPE>(NullableColumn<BoolValue<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public string Get(Function<string> column) {
            Fields.Add(column);
            return String.Empty;
        }

        public string? Get(NullableFunction<string> column) {
            Fields.Add(column);
            return default;
        }

        public Guid Get(Function<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public Guid? Get(NullableFunction<Guid> column) {
            Fields.Add(column);
            return default;
        }

        public bool Get(Function<bool> column) {
            Fields.Add(column);
            return default;
        }

        public bool? Get(NullableFunction<bool> column) {
            Fields.Add(column);
            return default;
        }

        public Bit Get(Function<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public Bit? Get(NullableFunction<Bit> column) {
            Fields.Add(column);
            return default;
        }

        public short Get(Function<short> column) {
            Fields.Add(column);
            return default;
        }

        public short? Get(NullableFunction<short> column) {
            Fields.Add(column);
            return default;
        }

        public int Get(Function<int> column) {
            Fields.Add(column);
            return default;
        }

        public int? Get(NullableFunction<int> column) {
            Fields.Add(column);
            return default;
        }

        public long Get(Function<long> column) {
            Fields.Add(column);
            return default;
        }

        public long? Get(NullableFunction<long> column) {
            Fields.Add(column);
            return default;
        }

        public float Get(Function<float> column) {
            Fields.Add(column);
            return default;
        }

        public float? Get(NullableFunction<float> column) {
            Fields.Add(column);
            return default;
        }

        public double Get(Function<double> column) {
            Fields.Add(column);
            return default;
        }

        public double? Get(NullableFunction<double> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime Get(Function<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public DateTime? Get(NullableFunction<DateTime> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset Get(Function<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public DateTimeOffset? Get(NullableFunction<DateTimeOffset> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly Get(Function<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public DateOnly? Get(NullableFunction<DateOnly> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly Get(Function<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public TimeOnly? Get(NullableFunction<TimeOnly> column) {
            Fields.Add(column);
            return default;
        }

        public byte Get(Function<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte? Get(NullableFunction<byte> column) {
            Fields.Add(column);
            return default;
        }

        public byte[] Get(Function<byte[]> column) {
            Fields.Add(column);
            return Array.Empty<byte>();
        }

        public byte[]? Get(NullableFunction<byte[]> column) {
            Fields.Add(column);
            return default;
        }

        public StringKey<TYPE> Get<TYPE>(Function<StringKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public StringKey<TYPE>? Get<TYPE>(NullableFunction<StringKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public GuidKey<TYPE> Get<TYPE>(Function<GuidKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public GuidKey<TYPE>? Get<TYPE>(NullableFunction<GuidKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public ShortKey<TYPE> Get<TYPE>(Function<ShortKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public ShortKey<TYPE>? Get<TYPE>(NullableFunction<ShortKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public IntKey<TYPE> Get<TYPE>(Function<IntKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public IntKey<TYPE>? Get<TYPE>(NullableFunction<IntKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public LongKey<TYPE> Get<TYPE>(Function<LongKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public LongKey<TYPE>? Get<TYPE>(NullableFunction<LongKey<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public ENUM GetEnum<ENUM>(Column<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default!;
        }

        public ENUM? GetEnum<ENUM>(NullableColumn<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default;
        }

        public ENUM GetEnum<ENUM>(Function<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default!;
        }

        public ENUM? GetEnum<ENUM>(NullableFunction<ENUM> column) where ENUM : struct, Enum {
            Fields.Add(column);
            return default;
        }

        public BoolValue<TYPE> Get<TYPE>(Function<BoolValue<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }

        public BoolValue<TYPE>? Get<TYPE>(NullableFunction<BoolValue<TYPE>> column) where TYPE : notnull {
            Fields.Add(column);
            return default;
        }
    }
}