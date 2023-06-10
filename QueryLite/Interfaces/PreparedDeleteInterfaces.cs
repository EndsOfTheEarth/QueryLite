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
using QueryLite.PreparedQuery;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace QueryLite {

    public interface IPreparedDeleteUsing<PARAMETERS> : IPreparedDeleteJoin<PARAMETERS> where PARAMETERS : notnull {

        /// <summary>
        /// Delete using syntax. Please Note: This syntax is only supported by PostgreSql
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        IPreparedDeleteWhere<PARAMETERS> Using(params ITable[] tables);
    }
    public interface IPreparedDeleteJoin<PARAMETERS> : IPreparedDeleteWhere<PARAMETERS> where PARAMETERS : notnull {

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
    public interface IPreparedDeleteJoinOn<PARAMETERS> where PARAMETERS : notnull {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IPreparedDeleteJoin<PARAMETERS> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on);
    }

    public interface IPreparedDeleteWhere<PARAMETERS> where PARAMETERS : notnull {

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

    internal sealed class PreparedDeleteJoin<PARAMETERS> : IPreparedDeleteJoinOn<PARAMETERS> where PARAMETERS : notnull {

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

        NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }

    public interface IPreparedDeleteQuery<PARAMETERS, RETURNING> {

        void Initialize(IDatabase database);

        QueryResult<RETURNING> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<QueryResult<RETURNING>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }
}