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
using System.Data.Common;

namespace QueryLite {

    /// <summary>
    /// Delegate to read a value from a DbDataReader.
    /// </summary>
    public delegate TYPE ReadValueDelegate<TYPE>(DbDataReader reader, int ordinal);

    public interface IResultRow {

        internal void Reset();

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public string Get(Column<string> column);
        public string? Get(NColumn<string> column);
        public string? GetAsNull(Column<string> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Guid Get(Column<Guid> column);
        public Guid? Get(NColumn<Guid> column);
        public Guid? GetAsNull(Column<Guid> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public bool Get(Column<bool> column);
        public bool? Get(NColumn<bool> column);
        public bool? GetAsNull(Column<bool> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Bit Get(Column<Bit> column);
        public Bit? Get(NColumn<Bit> column);
        public Bit? GetAsNull(Column<Bit> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public decimal Get(Column<decimal> column);
        public decimal? Get(NColumn<decimal> column);
        public decimal? GetAsNull(Column<decimal> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public short Get(Column<short> column);
        public short? Get(NColumn<short> column);
        public short? GetAsNull(Column<short> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public int Get(Column<int> column);
        public int? Get(NColumn<int> column);
        public int? GetAsNull(Column<int> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public long Get(Column<long> column);
        public long? Get(NColumn<long> column);
        public long? GetAsNull(Column<long> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public float Get(Column<float> column);
        public float? Get(NColumn<float> column);
        public float? GetAsNull(Column<float> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public double Get(Column<double> column);
        public double? Get(NColumn<double> column);
        public double? GetAsNull(Column<double> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public TimeOnly Get(Column<TimeOnly> column);
        public TimeOnly? Get(NColumn<TimeOnly> column);
        public TimeOnly? GetAsNull(Column<TimeOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTime Get(Column<DateTime> column);
        public DateTime? Get(NColumn<DateTime> column);
        public DateTime? GetAsNull(Column<DateTime> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateOnly Get(Column<DateOnly> column);
        public DateOnly? Get(NColumn<DateOnly> column);
        public DateOnly? GetAsNull(Column<DateOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTimeOffset Get(Column<DateTimeOffset> column);
        public DateTimeOffset? Get(NColumn<DateTimeOffset> column);
        public DateTimeOffset? GetAsNull(Column<DateTimeOffset> column);

        public byte Get(Column<byte> column);
        public byte? Get(NColumn<byte> column);
        public byte? GetAsNull(Column<byte> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public byte[] Get(Column<byte[]> column);
        public byte[]? Get(NColumn<byte[]> column);
        public byte[]? GetAsNull(Column<byte[]> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Json Get(Column<Json> column);
        public Json? Get(NColumn<Json> column);
        public Json? GetAsNull(Column<Json> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Jsonb Get(Column<Jsonb> column);
        public Jsonb? Get(NColumn<Jsonb> column);
        public Jsonb? GetAsNull(Column<Jsonb> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public ENUM Get<ENUM>(Column<ENUM> column) where ENUM : struct, Enum;
        public ENUM? Get<ENUM>(NColumn<ENUM> column) where ENUM : struct, Enum;
        public ENUM? GetAsNull<ENUM>(Column<ENUM> column) where ENUM : struct, Enum;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public string Get(Function<string> column);
        public string? Get(NFunction<string> column);
        public string? GetAsNull(Function<string> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Guid Get(Function<Guid> column);
        public Guid? Get(NFunction<Guid> column);
        public Guid? GetAsNull(Function<Guid> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public bool Get(Function<bool> column);
        public bool? Get(NFunction<bool> column);
        public bool? GetAsNull(Function<bool> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Bit Get(Function<Bit> column);
        public Bit? Get(NFunction<Bit> column);
        public Bit? GetAsNull(Function<Bit> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public short Get(Function<short> column);
        public short? Get(NFunction<short> column);
        public short? GetAsNull(Function<short> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public int Get(Function<int> column);
        public int? Get(NFunction<int> column);
        public int? GetAsNull(Function<int> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public long Get(Function<long> column);
        public long? Get(NFunction<long> column);
        public long? GetAsNull(Function<long> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public float Get(Function<float> column);
        public float? Get(NFunction<float> column);
        public float? GetAsNull(Function<float> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public double Get(Function<double> column);
        public double? Get(NFunction<double> column);
        public double? GetAsNull(Function<double> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTime Get(Function<DateTime> column);
        public DateTime? Get(NFunction<DateTime> column);
        public DateTime? GetAsNull(Function<DateTime> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateTimeOffset Get(Function<DateTimeOffset> column);
        public DateTimeOffset? Get(NFunction<DateTimeOffset> column);
        public DateTimeOffset? GetAsNull(Function<DateTimeOffset> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public DateOnly Get(Function<DateOnly> column);
        public DateOnly? Get(NFunction<DateOnly> column);
        public DateOnly? GetAsNull(Function<DateOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public TimeOnly Get(Function<TimeOnly> column);
        public TimeOnly? Get(NFunction<TimeOnly> column);
        public TimeOnly? GetAsNull(Function<TimeOnly> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public byte Get(Function<byte> column);
        public byte? Get(NFunction<byte> column);
        public byte? GetAsNull(Function<byte> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public byte[] Get(Function<byte[]> column);
        public byte[]? Get(NFunction<byte[]> column);
        public byte[]? GetAsNull(Function<byte[]> column);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Json Get(Function<Json> function);
        public Json? Get(NFunction<Json> function);
        public Json? GetAsNull(Function<Json> function);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public Jsonb Get(Function<Jsonb> function);
        public Jsonb? Get(NFunction<Jsonb> function);
        public Jsonb? GetAsNull(Function<Jsonb> function);

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public ENUM Get<ENUM>(Function<ENUM> column) where ENUM : struct, Enum;
        public ENUM? Get<ENUM>(NFunction<ENUM> column) where ENUM : struct, Enum;
        public ENUM? GetAsNull<ENUM>(Function<ENUM> column) where ENUM : struct, Enum;

        /// <summary>
        /// Returns the selected value if it exists otherwise the default value for the type.
        /// </summary>
        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetAsNull<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Guid> column) where CUSTOM_TYPE : struct, ICustomType<Guid, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetAsNull<CUSTOM_TYPE>(Column<CUSTOM_TYPE, short> column) where CUSTOM_TYPE : struct, ICustomType<short, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetAsNull<CUSTOM_TYPE>(Column<CUSTOM_TYPE, int> column) where CUSTOM_TYPE : struct, ICustomType<int, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetAsNull<CUSTOM_TYPE>(Column<CUSTOM_TYPE, long> column) where CUSTOM_TYPE : struct, ICustomType<long, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE>;
        public CUSTOM_TYPE? GetAsNull<CUSTOM_TYPE>(Column<CUSTOM_TYPE, string> column) where CUSTOM_TYPE : struct, ICustomType<string, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, bool> column) where CUSTOM_TYPE : struct, ICustomType<bool, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, decimal> column) where CUSTOM_TYPE : struct, ICustomType<decimal, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTime> column) where CUSTOM_TYPE : struct, ICustomType<DateTime, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateTimeOffset> column) where CUSTOM_TYPE : struct, ICustomType<DateTimeOffset, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, DateOnly> column) where CUSTOM_TYPE : struct, ICustomType<DateOnly, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, TimeOnly> column) where CUSTOM_TYPE : struct, ICustomType<TimeOnly, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, float> column) where CUSTOM_TYPE : struct, ICustomType<float, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, double> column) where CUSTOM_TYPE : struct, ICustomType<double, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Bit> column) where CUSTOM_TYPE : struct, ICustomType<Bit, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Json> column) where CUSTOM_TYPE : struct, ICustomType<Json, CUSTOM_TYPE>;

        public CUSTOM_TYPE Get<CUSTOM_TYPE>(Column<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE>;
        public CUSTOM_TYPE? Get<CUSTOM_TYPE>(NColumn<CUSTOM_TYPE, Jsonb> column) where CUSTOM_TYPE : struct, ICustomType<Jsonb, CUSTOM_TYPE>;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public TYPE LoadFromReader<TYPE>(Column<TYPE> column, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public TYPE? LoadFromReader<TYPE>(NColumn<TYPE> column, ReadValueDelegate<TYPE> readValue) where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public CUSTOM_TYPE LoadFromReader<CUSTOM_TYPE, TYPE>(Column<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue, CUSTOM_TYPE @default) where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE> where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public CUSTOM_TYPE? LoadFromReader<CUSTOM_TYPE, TYPE>(NColumn<CUSTOM_TYPE, TYPE> column, ReadValueDelegate<CUSTOM_TYPE> readValue) where CUSTOM_TYPE : struct, ICustomType<TYPE, CUSTOM_TYPE> where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public TYPE LoadFromReader<TYPE>(Function<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public TYPE? LoadFromReader<TYPE>(NFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public TYPE LoadFromReader<TYPE>(RawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue, TYPE @default) where TYPE : notnull;

        /// <summary>
        /// Read value directly from DbDataReader.
        /// </summary>
        public TYPE? LoadFromReader<TYPE>(NRawSqlFunction<TYPE> function, ReadValueDelegate<TYPE> readValue) where TYPE : notnull;
    }
}