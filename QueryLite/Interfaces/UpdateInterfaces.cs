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

    public interface IUpdateSet {
        IUpdateJoin Set<TYPE>(Column<TYPE> column, TYPE value) where TYPE : notnull;
        IUpdateJoin Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : class;
        IUpdateJoin Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : struct;
        IUpdateJoin Set<TYPE>(Column<TYPE> column, AFunction<TYPE> function) where TYPE : notnull;
    }

    public interface IUpdateJoin : IUpdateWhere {

        /// <summary>
        /// Join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IUpdateJoinOn Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IUpdateJoinOn LeftJoin(ITable table);
    }

    public interface IUpdateJoinOn {

        // <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IUpdateJoin On(ICondition on);
    }

    internal sealed class UpdateJoin : IJoin, IUpdateJoinOn {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        public ICondition? _condition;

        public ICondition Condition {
            get { return _condition!; }
        }

        private readonly UpdateQueryTemplate Template;

        internal UpdateJoin(JoinType joinType, ITable table, UpdateQueryTemplate tempate) {
            JoinType = joinType;
            Table = table;
            Template = tempate;
        }

        public IUpdateJoin On(ICondition on) {
            _condition = on;
            return Template;
        }
    }

    public interface IUpdateWhere : IUpdateSet {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IUpdateExecute Where(ICondition condition);

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used when there is no where clause on an update query.
        /// </summary>
        /// <returns></returns>
        public IUpdateExecute NoWhereCondition();
    }

    public interface IUpdateExecute {

        /// <summary>
        /// Get update sql
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        string GetSql(IDatabase database);

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
    }
}