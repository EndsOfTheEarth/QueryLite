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

    internal sealed class DeleteQueryTemplate : IDeleteFrom, IDeleteNoWhere, IDeleteWhere, IDeleteExecute {

        public ITable Table { get; }
        public List<ITable>? FromTables { get; private set; }
        public ICondition? WhereCondition { get; private set; }

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

        public IDeleteWhere From(ITable table, params ITable[] tables) {
            Using(table, tables);
            return this;
        }

        public IDeleteWhere Using(ITable table, params ITable[] tables) {

            ArgumentNullException.ThrowIfNull(table);

            FromTables = new List<ITable>(tables.Length + 1);

            FromTables.Add(table);

            if(tables.Length > 0) {
                FromTables.AddRange(tables);
            }
            return this;
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

            return database.DeleteGenerator.GetSql<bool>(this, database, parameters: null, outputFunc: null);
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null,
                                      Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) ||
                                             (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql<bool>(this, database, parameters, outputFunc: null);

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


        public QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction,
                                                   QueryTimeout? timeout = null,
                                                   Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(func);
            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) ||
                                             (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters, func);

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

        public async Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? ct = null,
                                                       QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default,
                                                       string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) ||
                                             (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql<bool>(this, database, parameters, outputFunc: null);

            NonQueryResult result = await QueryExecutor.ExecuteNonQueryAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Delete,
                debugName: debugName,
                ct: ct ?? CancellationToken.None
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? ct = null,
                                                                    QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(func);
            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParametersBuilder? parameters = (useParameters == Parameters.On) ||
                                             (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters, func);

            QueryResult<RESULT> result = await QueryExecutor.ExecuteAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Update,
                debugName: debugName,
                ct: ct ?? CancellationToken.None
            );
            return result;
        }
    }
}