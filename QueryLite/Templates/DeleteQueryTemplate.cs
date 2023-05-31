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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal sealed class DeleteQueryTemplate : IDeleteJoin, IDeleteWhere, IDeleteExecute {

        public ITable Table { get; }
        public IList<IJoin>? Joins { get; private set; }
        public ICondition? WhereCondition { get; private set; }
        public IList<IColumn>? ReturningColumns { get; private set; }

        public DeleteQueryTemplate(ITable table) {

            ArgumentNullException.ThrowIfNull(table);
            Table = table;
        }
        public DeleteQueryTemplate(ITable table, ICondition whereCondition) {

            ArgumentNullException.ThrowIfNull(table);
            ArgumentNullException.ThrowIfNull(whereCondition);

            Table = table;
            WhereCondition = whereCondition;
        }

        public IDeleteJoinOn Join(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            if(Joins == null) {
                Joins = new List<IJoin>(1);
            }
            DeleteJoin join = new DeleteJoin(JoinType.Join, table, this);
            Joins.Add(join);
            return join;
        }

        public IDeleteJoinOn LeftJoin(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            if(Joins == null) {
                Joins = new List<IJoin>(1);
            }
            DeleteJoin join = new DeleteJoin(JoinType.LeftJoin, table, this);
            Joins.Add(join);
            return join;
        }

        public IDeleteExecute Where(ICondition condition) {

            ArgumentNullException.ThrowIfNull(condition);

            if(WhereCondition != null) {
                throw new Exception($"Where condition has already been set");
            }
            WhereCondition = condition;
            return this;
        }

        public IDeleteExecute NoWhereCondition() {
            WhereCondition = null;
            return this;
        }

        public string GetSql(IDatabase database) {

            ArgumentNullException.ThrowIfNull(database);

            return database.DeleteGenerator.GetSql(this, database, parameters : null);
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            NonQueryResult result = QueryExecutor.ExecuteNonQuery(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Delete,
                debugName: debugName);

            return result;
        }


        public QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(func);
            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningColumns = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Update,
                debugName: debugName
            );
            return result;
        }

        public Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            Task<NonQueryResult> result = QueryExecutor.ExecuteNonQueryAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Delete,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(func);
            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningColumns = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            Task<QueryResult<RESULT>> result = QueryExecutor.ExecuteAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Update,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }
    }
}