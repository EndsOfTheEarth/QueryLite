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

    public interface IDeleteUsing : IDeleteJoin {

        /// <summary>
        /// Delete using syntax. Please Note: This syntax is only supported by PostgreSql
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        IDeleteWhere Using(params ITable[] tables);
    }
    public interface IDeleteJoin : IDeleteWhere {

        /// <summary>
        /// Delete join syntax. Please Note: This syntax is only supported on Sql Server
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IDeleteJoinOn Join(ITable table);

        /// <summary>
        /// Delete left join syntax. Please Note: This syntax is only supported on Sql Server
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IDeleteJoinOn LeftJoin(ITable table);
    }
    public interface IDeleteJoinOn {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IDeleteJoin On(ICondition on);
    }

    public interface IDeleteWhere {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IDeleteExecute Where(ICondition condition);

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used when there is no where clause on a delete query.
        /// </summary>
        /// <returns></returns>
        public IDeleteExecute NoWhereCondition();
    }

    internal sealed class DeleteJoin : IJoin, IDeleteJoinOn {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        public ICondition? _condition;

        public ICondition Condition {
            get { return _condition!; }
        }

        private readonly DeleteQueryTemplate Template;

        internal DeleteJoin(JoinType joinType, ITable table, DeleteQueryTemplate tempate) {
            JoinType = joinType;
            Table = table;
            Template = tempate;
        }

        public IDeleteJoin On(ICondition on) {
            _condition = on;
            return Template;
        }
    }

    public interface IDeleteExecute {

        /// <summary>
        /// Get delete sql
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        string GetSql(IDatabase database);

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null,  Parameters useParameters = Parameters.Default, string debugName = "");
        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }
}