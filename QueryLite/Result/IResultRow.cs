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

namespace QueryLite {

    public interface IResultRow {

        internal void Reset();

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public string Get(Column<string> column);
        public string? Get(NullableColumn<string> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Guid Get(Column<Guid> column);
        public Guid? Get(NullableColumn<Guid> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public bool Get(Column<bool> column);
        public bool? Get(NullableColumn<bool> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Bit Get(Column<Bit> column);
        public Bit? Get(NullableColumn<Bit> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public decimal Get(Column<decimal> column);
        public decimal? Get(NullableColumn<decimal> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public short Get(Column<short> column);
        public short? Get(NullableColumn<short> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public int Get(Column<int> column);
        public int? Get(NullableColumn<int> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public long Get(Column<long> column);
        public long? Get(NullableColumn<long> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public float Get(Column<float> column);
        public float? Get(NullableColumn<float> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public double Get(Column<double> column);
        public double? Get(NullableColumn<double> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public TimeOnly Get(Column<TimeOnly> column);
        public TimeOnly? Get(NullableColumn<TimeOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTime Get(Column<DateTime> column);
        public DateTime? Get(NullableColumn<DateTime> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateOnly Get(Column<DateOnly> column);
        public DateOnly? Get(NullableColumn<DateOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTimeOffset Get(Column<DateTimeOffset> column);
        public DateTimeOffset? Get(NullableColumn<DateTimeOffset> column);

        public byte Get(Column<byte> column);
        public byte? Get(NullableColumn<byte> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public byte[] Get(Column<byte[]> column);
        public byte[]? Get(NullableColumn<byte[]> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public ENUM Get<ENUM>(Column<ENUM> column) where ENUM : struct, Enum;
        public ENUM? Get<ENUM>(NullableColumn<ENUM> column) where ENUM : struct, Enum;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public StringKey<TYPE> Get<TYPE>(Column<StringKey<TYPE>> column) where TYPE : notnull;
        public StringKey<TYPE>? Get<TYPE>(NullableColumn<StringKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public GuidKey<TYPE> Get<TYPE>(Column<GuidKey<TYPE>> column) where TYPE : notnull;
        public GuidKey<TYPE>? Get<TYPE>(NullableColumn<GuidKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public ShortKey<TYPE> Get<TYPE>(Column<ShortKey<TYPE>> column) where TYPE : notnull;
        public ShortKey<TYPE>? Get<TYPE>(NullableColumn<ShortKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public IntKey<TYPE> Get<TYPE>(Column<IntKey<TYPE>> column) where TYPE : notnull;
        public IntKey<TYPE>? Get<TYPE>(NullableColumn<IntKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public LongKey<TYPE> Get<TYPE>(Column<LongKey<TYPE>> column) where TYPE : notnull;
        public LongKey<TYPE>? Get<TYPE>(NullableColumn<LongKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public BoolValue<TYPE> Get<TYPE>(Column<BoolValue<TYPE>> column) where TYPE : notnull;
        public BoolValue<TYPE>? Get<TYPE>(NullableColumn<BoolValue<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public string Get(Function<string> column);
        public string? Get(NullableFunction<string> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Guid Get(Function<Guid> column);
        public Guid? Get(NullableFunction<Guid> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public bool Get(Function<bool> column);
        public bool? Get(NullableFunction<bool> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Bit Get(Function<Bit> column);
        public Bit? Get(NullableFunction<Bit> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public short Get(Function<short> column);
        public short? Get(NullableFunction<short> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public int Get(Function<int> column);
        public int? Get(NullableFunction<int> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public long Get(Function<long> column);
        public long? Get(NullableFunction<long> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public float Get(Function<float> column);
        public float? Get(NullableFunction<float> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public double Get(Function<double> column);
        public double? Get(NullableFunction<double> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTime Get(Function<DateTime> column);
        public DateTime? Get(NullableFunction<DateTime> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTimeOffset Get(Function<DateTimeOffset> column);
        public DateTimeOffset? Get(NullableFunction<DateTimeOffset> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateOnly Get(Function<DateOnly> column);
        public DateOnly? Get(NullableFunction<DateOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public TimeOnly Get(Function<TimeOnly> column);
        public TimeOnly? Get(NullableFunction<TimeOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public byte Get(Function<byte> column);
        public byte? Get(NullableFunction<byte> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public byte[] Get(Function<byte[]> column);
        public byte[]? Get(NullableFunction<byte[]> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public ENUM Get<ENUM>(Function<ENUM> column) where ENUM : struct, Enum;
        public ENUM? Get<ENUM>(NullableFunction<ENUM> column) where ENUM : struct, Enum;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public StringKey<TYPE> Get<TYPE>(Function<StringKey<TYPE>> column) where TYPE : notnull;
        public StringKey<TYPE>? Get<TYPE>(NullableFunction<StringKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public GuidKey<TYPE> Get<TYPE>(Function<GuidKey<TYPE>> column) where TYPE : notnull;
        public GuidKey<TYPE>? Get<TYPE>(NullableFunction<GuidKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public ShortKey<TYPE> Get<TYPE>(Function<ShortKey<TYPE>> column) where TYPE : notnull;
        public ShortKey<TYPE>? Get<TYPE>(NullableFunction<ShortKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public IntKey<TYPE> Get<TYPE>(Function<IntKey<TYPE>> column) where TYPE : notnull;
        public IntKey<TYPE>? Get<TYPE>(NullableFunction<IntKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public LongKey<TYPE> Get<TYPE>(Function<LongKey<TYPE>> column) where TYPE : notnull;
        public LongKey<TYPE>? Get<TYPE>(NullableFunction<LongKey<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public BoolValue<TYPE> Get<TYPE>(Function<BoolValue<TYPE>> column) where TYPE : notnull;
        public BoolValue<TYPE>? Get<TYPE>(NullableFunction<BoolValue<TYPE>> column) where TYPE : notnull;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public CUSTOM_TYPE GetGuid<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<Guid, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetGuid<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<Guid, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetShort<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<short, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetShort<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<short, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetInt<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<int, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetInt<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<int, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetLong<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<long, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetLong<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<long, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetString<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<string, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetString<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<string, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetBool<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<bool, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetBool<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<bool, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetDecimal<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<decimal, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetDecimal<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<decimal, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetDateTime<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTime, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetDateTime<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTime, CUSTOM_TYPE>;

        public CUSTOM_TYPE GetDateTimeOffset<CUSTOM_TYPE>(Column<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTimeOffset, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetDateTimeOffset<CUSTOM_TYPE>(NullableColumn<CUSTOM_TYPE> column) where CUSTOM_TYPE : struct, IValueOf<DateTimeOffset, CUSTOM_TYPE>;
    }
}