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
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    public interface IPreparedUpdateSet<PARAMETERS> where PARAMETERS : notnull {

        IPreparedUpdateJoin<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values);
    }

    public interface IPreparedUpdateJoin<PARAMETERS> : IPreparedUpdateWhere<PARAMETERS> where PARAMETERS : notnull {

        /// <summary>
        /// Join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedUpdateJoinOn<PARAMETERS> Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedUpdateJoinOn<PARAMETERS> LeftJoin(ITable table);
    }

    public interface IPreparedUpdateJoinOn<PARAMETERS> where PARAMETERS : notnull {

        // <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IPreparedUpdateJoin<PARAMETERS> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on);
    }

    internal sealed class PreparedUpdateJoin<PARAMETERS> : IPreparedUpdateJoinOn<PARAMETERS> where PARAMETERS : notnull {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        public APreparedCondition<PARAMETERS>? _condition;

        public APreparedCondition<PARAMETERS> Condition {
            get { return _condition!; }
        }

        private readonly PreparedUpdateTemplate<PARAMETERS> Template;

        internal PreparedUpdateJoin(JoinType joinType, ITable table, PreparedUpdateTemplate<PARAMETERS> template) {
            JoinType = joinType;
            Table = table;
            Template = template;
        }

        public IPreparedUpdateJoin<PARAMETERS> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on) {
            _condition = on(new EmptyPreparedCondition<PARAMETERS>());
            return Template;
        }
    }

    public interface IPreparedUpdateWhere<PARAMETERS> : IPreparedUpdateSet<PARAMETERS> where PARAMETERS : notnull {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IPreparedUpdateBuild<PARAMETERS> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> condition);

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used when there is no where clause on an update query.
        /// </summary>
        /// <returns></returns>
        public IPreparedUpdateBuild<PARAMETERS> NoWhereCondition();
    }

    public interface IPreparedUpdateBuild<PARAMETERS> where PARAMETERS : notnull {

        IPreparedUpdateQuery<PARAMETERS> Build();
        IPreparedUpdateQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> returningFunc);

        /// <summary>
        /// Get update sql
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        //string GetSql(IDatabase database);

        //NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        //QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");

        //Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        //Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }

    public interface IPreparedUpdateQuery<PARAMETERS> {

        void Initilize(IDatabase database);

        NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }

    public interface IPreparedUpdateQuery<PARAMETERS, RESULT> {

        void Initilize(IDatabase database);

        QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }
}