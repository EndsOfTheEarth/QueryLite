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
using QueryLite.PreparedQuery;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace QueryLite {

    public interface IPreparedDeleteUsing<PARAMETERS> : IPreparedDeleteJoin<PARAMETERS> {

        /// <summary>
        /// Delete using syntax. Please Note: This syntax is only supported by PostgreSql
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        IPreparedDeleteWhere<PARAMETERS> Using(params ITable[] tables);
    }
    public interface IPreparedDeleteJoin<PARAMETERS> : IPreparedDeleteWhere<PARAMETERS> {

        /// <summary>
        /// Delete join syntax. Please Note: This syntax is only supported on Sql Server
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedDeleteJoinOn<PARAMETERS> Join(ITable table);

        /// <summary>
        /// Delete left join syntax. Please Note: This syntax is only supported on Sql Server
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedDeleteJoinOn<PARAMETERS> LeftJoin(ITable table);
    }
    public interface IPreparedDeleteJoinOn<PARAMETERS> {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IPreparedDeleteJoin<PARAMETERS> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on);
    }

    public interface IPreparedDeleteWhere<PARAMETERS> {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IPreparedDeleteBuild<PARAMETERS> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> condition);

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used when there is no where clause on a delete query.
        /// </summary>
        /// <returns></returns>
        public IPreparedDeleteBuild<PARAMETERS> NoWhereCondition();
    }

    internal sealed class PreparedDeleteJoin<PARAMETERS> : IPreparedDeleteJoinOn<PARAMETERS> {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        public APreparedCondition<PARAMETERS>? _condition;

        public APreparedCondition<PARAMETERS> Condition {
            get { return _condition!; }
        }

        private readonly PreparedDeleteQueryTemplate<PARAMETERS> Template;

        internal PreparedDeleteJoin(JoinType joinType, ITable table, PreparedDeleteQueryTemplate<PARAMETERS> template) {
            JoinType = joinType;
            Table = table;
            Template = template;
        }

        public IPreparedDeleteJoin<PARAMETERS> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on) {
            _condition = on(new EmptyPreparedCondition<PARAMETERS>());
            return Template;
        }
    }

    public interface IPreparedDeleteBuild<PARAMETERS> {

        public IPreparedDeleteQuery<PARAMETERS> Build();
        public IPreparedDeleteQuery<PARAMETERS, RETURNING> Build<RETURNING>(Func<IResultRow, RETURNING> func);
    }

    public interface IPreparedDeleteQuery<PARAMETERS> {

        void Initialize(IDatabase database);

        NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");
        Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");
    }

    public interface IPreparedDeleteQuery<PARAMETERS, RESULT> {

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
}