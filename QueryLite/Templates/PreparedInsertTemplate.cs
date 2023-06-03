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
using QueryLite.Databases;
using QueryLite.Databases.SqlServer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal sealed class PreparedInsertTemplate<PARAMETERS> : IPreparedInsertSet<PARAMETERS>, IPreparedInsertBuild<PARAMETERS> {

        public ITable Table { get; }
        public Action<IPreparedSetValuesCollector<PARAMETERS>>? SetValues { get; private set; }

        public PreparedInsertTemplate(ITable table) {
            Table = table;
        }

        public IPreparedInsertBuild<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values) {
            SetValues = values;
            return this;
        }

        public IPreparedInsertQuery<PARAMETERS> Build(IDatabase database) {

            string sql = new SqlServerPreparedInsertQueryGenerator().GetSql<PARAMETERS, bool>(this, database, out List<ISetParameter<PARAMETERS>> insertParameters, outputFunc: null);

            return new SqlServerPreparedInsertQuery<PARAMETERS>(sql, insertParameters);
        }

        public IPreparedInsertQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> outputFunc, IDatabase database) {

            string sql = new SqlServerPreparedInsertQueryGenerator().GetSql(this, database, out List<ISetParameter<PARAMETERS>> insertParameters, outputFunc: outputFunc);

            return new SqlServerPreparedInsertQuery<PARAMETERS, RESULT>(sql, insertParameters, outputFunc);
        }
    }

    internal sealed class SqlServerPreparedInsertQuery<PARAMETERS> : IPreparedInsertQuery<PARAMETERS> {

        private readonly string _sql;
        private readonly List<ISetParameter<PARAMETERS>> _insertParameters;

        public SqlServerPreparedInsertQuery(string sql, List<ISetParameter<PARAMETERS>> insertParameters) {
            _sql = sql;
            _insertParameters = insertParameters;
        }

        public NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            NonQueryResult result = PreparedQueryExecutor.ExecuteNonQuery(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: _insertParameters,
                sql: _sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
            return result;
        }

        public async Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            NonQueryResult result = await PreparedQueryExecutor.ExecuteNonQueryAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: _insertParameters,
                sql: _sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
            return result;
        }
    }

    internal sealed class SqlServerPreparedInsertQuery<PARAMETERS, RESULT> : IPreparedInsertQuery<PARAMETERS, RESULT> {

        private string _sql;
        private List<ISetParameter<PARAMETERS>> _insertParameters;
        private Func<IResultRow, RESULT> _outputFunc;

        public SqlServerPreparedInsertQuery(string sql, List<ISetParameter<PARAMETERS>> insertParameters, Func<IResultRow, RESULT> outputFunc) {
            _sql = sql;
            _insertParameters = insertParameters;
            _outputFunc = outputFunc;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            QueryResult<RESULT> result = PreparedQueryExecutor.Execute(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: _insertParameters,
                outputFunc: _outputFunc,
                sql: _sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            QueryResult<RESULT> result = await PreparedQueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                cancellationToken: cancellationToken ?? CancellationToken.None,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: _insertParameters,
                outputFunc: _outputFunc,
                sql: _sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
            return result;
        }
    }
}