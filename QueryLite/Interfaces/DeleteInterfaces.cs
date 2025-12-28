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
namespace QueryLite {

    public interface IDeleteFrom : IDeleteNoWhere {

        /// <summary>
        /// Use additional tables in the delete query. Note: The 'From' and 'Using' methods are just aliases for the same action.
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        IDeleteWhere From(ITable table, params ITable[] tables);

        /// <summary>
        /// Use additional tables in the delete query. Note: The 'From' and 'Using' methods are just aliases for the same action.
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        IDeleteWhere Using(ITable table, params ITable[] tables);
    }

    public interface IDeleteNoWhere : IDeleteWhere {

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used when there is no where clause on a delete query.
        /// </summary>
        /// <returns></returns>
        public IDeleteExecute NoWhereCondition();
    }

    public interface IDeleteWhere {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IDeleteExecute Where(ICondition condition);
    }

    public interface IDeleteExecute {

        /// <summary>
        /// Get delete sql
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        string GetSql(IDatabase database);

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default,
                               string debugName = "");

        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction,
                                            QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default,
                                            string debugName = "");

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null,
                                          QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default,
                                          string debugName = "");

        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction,
                                                       CancellationToken? cancellationToken = null, QueryTimeout? timeout = null,
                                                       Parameters useParameters = Parameters.Default, string debugName = "");
    }
}