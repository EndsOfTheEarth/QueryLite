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
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    public interface IInsertSet {

        IInsertExecute Values(Action<ISetValuesCollector> values);
    }

    public interface IInsertExecute {

        string GetSql(IDatabase database);

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }

    public interface ISetValuesCollector {

        public ISetValuesCollector Set(Column<string> column, string value);
        public ISetValuesCollector Set(NullableColumn<string> column, string? value);

        public ISetValuesCollector Set(Column<Guid> column, Guid value);
        public ISetValuesCollector Set(NullableColumn<Guid> column, Guid? value);

        public ISetValuesCollector Set(Column<bool> column, bool value);
        public ISetValuesCollector Set(NullableColumn<bool> column, bool? value);

        public ISetValuesCollector Set(Column<Bit> column, Bit value);
        public ISetValuesCollector Set(NullableColumn<Bit> column, Bit? value);

        public ISetValuesCollector Set(Column<decimal> column, decimal value);
        public ISetValuesCollector Set(NullableColumn<decimal> column, decimal? value);

        public ISetValuesCollector Set(Column<short> column, short value);
        public ISetValuesCollector Set(NullableColumn<short> column, short? value);

        public ISetValuesCollector Set(Column<int> column, int value);
        public ISetValuesCollector Set(NullableColumn<int> column, int? value);

        public ISetValuesCollector Set(Column<long> column, long value);
        public ISetValuesCollector Set(NullableColumn<long> column, long? value);

        public ISetValuesCollector Set(Column<float> column, float value);
        public ISetValuesCollector Set(NullableColumn<float> column, float? value);

        public ISetValuesCollector Set(Column<double> column, double value);
        public ISetValuesCollector Set(NullableColumn<double> column, double? value);

        public ISetValuesCollector Set(Column<TimeOnly> column, TimeOnly value);
        public ISetValuesCollector Set(NullableColumn<TimeOnly> column, TimeOnly? value);

        public ISetValuesCollector Set(Column<DateTime> column, DateTime value);
        public ISetValuesCollector Set(NullableColumn<DateTime> column, DateTime? value);

        public ISetValuesCollector Set(Column<DateOnly> column, DateOnly value);
        public ISetValuesCollector Set(NullableColumn<DateOnly> column, DateOnly? value);

        public ISetValuesCollector Set(Column<DateTimeOffset> column, DateTimeOffset value);
        public ISetValuesCollector Set(NullableColumn<DateTimeOffset> column, DateTimeOffset? value);

        public ISetValuesCollector Set(Column<byte> column, byte value);
        public ISetValuesCollector Set(NullableColumn<byte> column, byte? value);

        public ISetValuesCollector Set(Column<byte[]> column, byte[] value);
        public ISetValuesCollector Set(NullableColumn<byte[]> column, byte[]? value);

        public ISetValuesCollector Set<ENUM>(Column<ENUM> column, ENUM value) where ENUM : notnull, Enum;
        public ISetValuesCollector Set<ENUM>(NullableColumn<ENUM> column, ENUM? value) where ENUM : notnull, Enum;

        public ISetValuesCollector Set<TYPE>(Column<StringKey<TYPE>> column, StringKey<TYPE> value) where TYPE : notnull;
        public ISetValuesCollector Set<TYPE>(NullableColumn<StringKey<TYPE>> column, StringKey<TYPE>? value) where TYPE : notnull;

        public ISetValuesCollector Set<TYPE>(Column<GuidKey<TYPE>> column, GuidKey<TYPE> value) where TYPE : notnull;
        public ISetValuesCollector Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, GuidKey<TYPE>? value) where TYPE : notnull;

        public ISetValuesCollector Set<TYPE>(Column<ShortKey<TYPE>> column, ShortKey<TYPE> value) where TYPE : notnull;
        public ISetValuesCollector Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, ShortKey<TYPE>? value) where TYPE : notnull;

        public ISetValuesCollector Set<TYPE>(Column<IntKey<TYPE>> column, IntKey<TYPE> value) where TYPE : notnull;
        public ISetValuesCollector Set<TYPE>(NullableColumn<IntKey<TYPE>> column, IntKey<TYPE>? value) where TYPE : notnull;

        public ISetValuesCollector Set<TYPE>(Column<LongKey<TYPE>> column, LongKey<TYPE> value) where TYPE : notnull;
        public ISetValuesCollector Set<TYPE>(NullableColumn<LongKey<TYPE>> column, LongKey<TYPE>? value) where TYPE : notnull;

        public ISetValuesCollector Set<TYPE>(Column<BoolValue<TYPE>> column, BoolValue<TYPE> value) where TYPE : notnull;
        public ISetValuesCollector Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, BoolValue<TYPE>? value) where TYPE : notnull;

        public ISetValuesCollector Set(AColumn<string> column, AFunction<string> value);

        public ISetValuesCollector Set(AColumn<Guid> column, AFunction<Guid> value);

        public ISetValuesCollector Set(AColumn<bool> column, AFunction<bool> value);

        public ISetValuesCollector Set(AColumn<Bit> column, AFunction<Bit> value);

        public ISetValuesCollector Set(AColumn<decimal> column, AFunction<decimal> value);

        public ISetValuesCollector Set(AColumn<short> column, AFunction<short> value);

        public ISetValuesCollector Set(AColumn<int> column, AFunction<int> value);

        public ISetValuesCollector Set(AColumn<long> column, AFunction<long> value);

        public ISetValuesCollector Set(AColumn<float> column, AFunction<float> value);

        public ISetValuesCollector Set(AColumn<double> column, AFunction<double> value);

        public ISetValuesCollector Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value);

        public ISetValuesCollector Set(AColumn<DateTime> column, AFunction<DateTime> value);

        public ISetValuesCollector Set(AColumn<DateOnly> column, AFunction<DateOnly> value);

        public ISetValuesCollector Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value);

        public ISetValuesCollector Set(AColumn<byte> column, AFunction<byte> value);

        public ISetValuesCollector Set(AColumn<byte[]> column, AFunction<byte[]> value);
    }
}