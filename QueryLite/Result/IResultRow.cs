﻿/*
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

namespace QueryLite {

    public interface IResultRow {

        internal void Reset();

        public string Get(Column<string> column);
        public string? Get(NullableColumn<string> column);

        public Guid Get(Column<Guid> column);
        public Guid? Get(NullableColumn<Guid> column);

        public bool Get(Column<bool> column);
        public bool? Get(NullableColumn<bool> column);

        public decimal Get(Column<decimal> column);
        public decimal? Get(NullableColumn<decimal> column);

        public short Get(Column<short> column);
        public short? Get(NullableColumn<short> column);

        public int Get(Column<int> column);
        public int? Get(NullableColumn<int> column);

        public long Get(Column<long> column);
        public long? Get(NullableColumn<long> column);

        public float Get(Column<float> column);
        public float? Get(NullableColumn<float> column);

        public double Get(Column<double> column);
        public double? Get(NullableColumn<double> column);

        public TimeOnly Get(Column<TimeOnly> column);
        public TimeOnly? Get(NullableColumn<TimeOnly> column);

        public DateTime Get(Column<DateTime> column);
        public DateTime? Get(NullableColumn<DateTime> column);

        public DateOnly Get(Column<DateOnly> column);
        public DateOnly? Get(NullableColumn<DateOnly> column);

        public DateTimeOffset Get(Column<DateTimeOffset> column);
        public DateTimeOffset? Get(NullableColumn<DateTimeOffset> column);

        public byte Get(Column<byte> column);
        public byte? Get(NullableColumn<byte> column);

        public byte[] Get(Column<byte[]> column);
        public byte[]? Get(NullableColumn<byte[]> column);

        public ENUM GetEnum<ENUM>(Column<ENUM> column) where ENUM : notnull, Enum;
        public ENUM? GetEnum<ENUM>(NullableColumn<ENUM> column) where ENUM : notnull, Enum;

        public StringKey<TYPE> Get<TYPE>(Column<StringKey<TYPE>> column) where TYPE : notnull;
        public StringKey<TYPE>? Get<TYPE>(NullableColumn<StringKey<TYPE>> column) where TYPE : notnull;

        public GuidKey<TYPE> Get<TYPE>(Column<GuidKey<TYPE>> column) where TYPE : notnull;
        public GuidKey<TYPE>? Get<TYPE>(NullableColumn<GuidKey<TYPE>> column) where TYPE : notnull;

        public ShortKey<TYPE> Get<TYPE>(Column<ShortKey<TYPE>> column) where TYPE : notnull;
        public ShortKey<TYPE>? Get<TYPE>(NullableColumn<ShortKey<TYPE>> column) where TYPE : notnull;

        public IntKey<TYPE> Get<TYPE>(Column<IntKey<TYPE>> column) where TYPE : notnull;
        public IntKey<TYPE>? Get<TYPE>(NullableColumn<IntKey<TYPE>> column) where TYPE : notnull;

        public LongKey<TYPE> Get<TYPE>(Column<LongKey<TYPE>> column) where TYPE : notnull;
        public LongKey<TYPE>? Get<TYPE>(NullableColumn<LongKey<TYPE>> column) where TYPE : notnull;

        public BoolValue<TYPE> Get<TYPE>(Column<BoolValue<TYPE>> column) where TYPE : notnull;
        public BoolValue<TYPE>? Get<TYPE>(NullableColumn<BoolValue<TYPE>> column) where TYPE : notnull;

        public string Get(Function<string> column);
        public string? Get(NullableFunction<string> column);

        public Guid Get(Function<Guid> column);
        public Guid? Get(NullableFunction<Guid> column);

        public bool Get(Function<bool> column);
        public bool? Get(NullableFunction<bool> column);

        public short Get(Function<short> column);
        public short? Get(NullableFunction<short> column);

        public int Get(Function<int> column);
        public int? Get(NullableFunction<int> column);

        public long Get(Function<long> column);
        public long? Get(NullableFunction<long> column);

        public float Get(Function<float> column);
        public float? Get(NullableFunction<float> column);

        public double Get(Function<double> column);
        public double? Get(NullableFunction<double> column);

        public DateTime Get(Function<DateTime> column);
        public DateTime? Get(NullableFunction<DateTime> column);

        public DateTimeOffset Get(Function<DateTimeOffset> column);
        public DateTimeOffset? Get(NullableFunction<DateTimeOffset> column);

        public byte Get(Function<byte> column);
        public byte? Get(NullableFunction<byte> column);

        public byte[] Get(Function<byte[]> column);
        public byte[]? Get(NullableFunction<byte[]> column);

        public ENUM GetEnum<ENUM>(Function<ENUM> column) where ENUM : notnull, Enum;
        public ENUM? GetEnum<ENUM>(NullableFunction<ENUM> column) where ENUM : notnull, Enum;

        public StringKey<TYPE> Get<TYPE>(Function<StringKey<TYPE>> column) where TYPE : notnull;
        public StringKey<TYPE>? Get<TYPE>(NullableFunction<StringKey<TYPE>> column) where TYPE : notnull;

        public GuidKey<TYPE> Get<TYPE>(Function<GuidKey<TYPE>> column) where TYPE : notnull;
        public GuidKey<TYPE>? Get<TYPE>(NullableFunction<GuidKey<TYPE>> column) where TYPE : notnull;

        public ShortKey<TYPE> Get<TYPE>(Function<ShortKey<TYPE>> column) where TYPE : notnull;
        public ShortKey<TYPE>? Get<TYPE>(NullableFunction<ShortKey<TYPE>> column) where TYPE : notnull;

        public IntKey<TYPE> Get<TYPE>(Function<IntKey<TYPE>> column) where TYPE : notnull;
        public IntKey<TYPE>? Get<TYPE>(NullableFunction<IntKey<TYPE>> column) where TYPE : notnull;

        public LongKey<TYPE> Get<TYPE>(Function<LongKey<TYPE>> column) where TYPE : notnull;
        public LongKey<TYPE>? Get<TYPE>(NullableFunction<LongKey<TYPE>> column) where TYPE : notnull;
    }
}