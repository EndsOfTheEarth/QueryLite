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
using QueryLite.PreparedQuery;

namespace QueryLite {

    public interface IPreparedUpdateSet<PARAMETERS> {

        IPreparedUpdateFrom<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values);
    }

    public interface IPreparedUpdateFrom<PARAMETERS> : IPreparedUpdateWhere<PARAMETERS> {

        public IPreparedUpdateWhere<PARAMETERS> From(ITable table, params ITable[] tables);
    }

    public interface IPreparedUpdateWhere<PARAMETERS> : IPreparedUpdateSet<PARAMETERS> {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IPreparedUpdateBuild<PARAMETERS> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> condition);

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used
        /// when there is no where clause on an update query.
        /// </summary>
        /// <returns></returns>
        public IPreparedUpdateBuild<PARAMETERS> NoWhereCondition();
    }

    public interface IPreparedUpdateBuild<PARAMETERS> {

        IPreparedUpdateQuery<PARAMETERS> Build();
        IPreparedUpdateQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> returningFunc);
    }

    public interface IPreparedUpdateQuery<PARAMETERS> {

        void Initialize(IDatabase database);

        NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");

        Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? ct = null,
                                            QueryTimeout? timeout = null, string debugName = "");
    }

    public interface IPreparedUpdateQuery<PARAMETERS, RESULT> {

        void Initialize(IDatabase database);

        QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");

        Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? ct = null,
                                               QueryTimeout? timeout = null, string debugName = "");

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
        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? ct = null,
                                                  QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, IDatabase database, CancellationToken? ct = null,
                                                  QueryTimeout? timeout = null, string debugName = "");
    }
}