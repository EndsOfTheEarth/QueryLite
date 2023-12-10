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

    public interface IPreparedInsertSet<PARAMETERS> {

        IPreparedInsertBuild<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values);
    }

    public interface IPreparedInsertBuild<PARAMETERS> {

        IPreparedInsertQuery<PARAMETERS> Build();
        IPreparedInsertQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> returningFunc);
    }

    public interface IPreparedInsertBuild<PARAMETERS, RESULT> {

        IPreparedInsertQuery<PARAMETERS, RESULT> Build();
    }

    public interface IPreparedInsertQuery<PARAMETERS> {

        void Initialize(IDatabase database);

        NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");
        Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");
    }

    public interface IPreparedInsertQuery<PARAMETERS, RESULT> {

        void Initialize(IDatabase database);

        QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");
        Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public RESULT? SingleOrDefault(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public RESULT? SingleOrDefault(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");
    }

    public interface IPreparedSetValuesCollector<PARAMETERS> {

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<string> column, Func<PARAMETERS, string> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<string> column, Func<PARAMETERS, string?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<Guid> column, Func<PARAMETERS, Guid> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<Guid> column, Func<PARAMETERS, Guid?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<bool> column, Func<PARAMETERS, bool> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<bool> column, Func<PARAMETERS, bool?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<Bit> column, Func<PARAMETERS, Bit> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<Bit> column, Func<PARAMETERS, Bit?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<decimal> column, Func<PARAMETERS, decimal> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<decimal> column, Func<PARAMETERS, decimal?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<short> column, Func<PARAMETERS, short> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<short> column, Func<PARAMETERS, short?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<int> column, Func<PARAMETERS, int> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<int> column, Func<PARAMETERS, int?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<long> column, Func<PARAMETERS, long> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<long> column, Func<PARAMETERS, long?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<float> column, Func<PARAMETERS, float> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<float> column, Func<PARAMETERS, float?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<double> column, Func<PARAMETERS, double> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<double> column, Func<PARAMETERS, double?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<TimeOnly> column, Func<PARAMETERS, TimeOnly> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<TimeOnly> column, Func<PARAMETERS, TimeOnly?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateTime> column, Func<PARAMETERS, DateTime> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateTime> column, Func<PARAMETERS, DateTime?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateOnly> column, Func<PARAMETERS, DateOnly> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateOnly> column, Func<PARAMETERS, DateOnly?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateTimeOffset> column, Func<PARAMETERS, DateTimeOffset> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateTimeOffset> column, Func<PARAMETERS, DateTimeOffset?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<byte> column, Func<PARAMETERS, byte> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<byte> column, Func<PARAMETERS, byte?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<byte[]> column, Func<PARAMETERS, byte[]> value);
        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<byte[]> column, Func<PARAMETERS, byte[]?> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set<ENUM>(Column<ENUM> column, Func<PARAMETERS, ENUM> value) where ENUM : struct, Enum;
        public IPreparedSetValuesCollector<PARAMETERS> Set<ENUM>(NullableColumn<ENUM> column, Func<PARAMETERS, ENUM?> value) where ENUM : struct, Enum;

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<StringKey<TYPE>> column, Func<PARAMETERS, StringKey<TYPE>> value) where TYPE : notnull;
        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<StringKey<TYPE>> column, Func<PARAMETERS, StringKey<TYPE>?> value) where TYPE : notnull;

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<GuidKey<TYPE>> column, Func<PARAMETERS, GuidKey<TYPE>> value) where TYPE : notnull;
        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, Func<PARAMETERS, GuidKey<TYPE>?> value) where TYPE : notnull;

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<ShortKey<TYPE>> column, Func<PARAMETERS, ShortKey<TYPE>> value) where TYPE : notnull;
        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, Func<PARAMETERS, ShortKey<TYPE>?> value) where TYPE : notnull;

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<IntKey<TYPE>> column, Func<PARAMETERS, IntKey<TYPE>> value) where TYPE : notnull;
        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<IntKey<TYPE>> column, Func<PARAMETERS, IntKey<TYPE>?> value) where TYPE : notnull;

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<LongKey<TYPE>> column, Func<PARAMETERS, LongKey<TYPE>> value) where TYPE : notnull;
        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<LongKey<TYPE>> column, Func<PARAMETERS, LongKey<TYPE>?> value) where TYPE : notnull;

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<BoolValue<TYPE>> column, Func<PARAMETERS, BoolValue<TYPE>> value) where TYPE : notnull;
        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, Func<PARAMETERS, BoolValue<TYPE>?> value) where TYPE : notnull;

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<string> column, AFunction<string> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<Guid> column, AFunction<Guid> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<bool> column, AFunction<bool> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<Bit> column, AFunction<Bit> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<decimal> column, AFunction<decimal> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<short> column, AFunction<short> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<int> column, AFunction<int> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<long> column, AFunction<long> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<float> column, AFunction<float> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<double> column, AFunction<double> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateTime> column, AFunction<DateTime> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateOnly> column, AFunction<DateOnly> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<byte> column, AFunction<byte> value);

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<byte[]> column, AFunction<byte[]> value);
    }
}