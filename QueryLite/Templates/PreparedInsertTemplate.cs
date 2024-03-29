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
using QueryLite.Databases;
using System;
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

        public IPreparedInsertQuery<PARAMETERS> Build() {
            return new PreparedInsertQuery<PARAMETERS>(this);
        }

        public IPreparedInsertQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> outputFunc) {
            return new PreparedInsertQuery<PARAMETERS, RESULT>(this, outputFunc);
        }
    }

    internal sealed class PreparedInsertQuery<PARAMETERS> : IPreparedInsertQuery<PARAMETERS> {

        private readonly PreparedInsertTemplate<PARAMETERS> _template;

        private readonly PreparedSqlAndParameters<PARAMETERS>?[] _insertDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)

        public PreparedInsertQuery(PreparedInsertTemplate<PARAMETERS> template) {

            _template = template;

            DatabaseType[] values = Enum.GetValues<DatabaseType>();

            int max = 0;

            foreach(DatabaseType value in values) {

                int valueAsInt = (int)value;

                if(valueAsInt > max) {
                    max = valueAsInt;
                }
            }
            _insertDetails = new PreparedSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public void Initialize(IDatabase database) {
            _ = GetInsertQuery(database);
        }
        private PreparedSqlAndParameters<PARAMETERS> GetInsertQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedSqlAndParameters<PARAMETERS> insertDetail;

            if(_insertDetails[dbTypeIndex] == null) {

                string sql = database.PreparedInsertGenerator.GetSql<PARAMETERS, bool>(_template, database, out PreparedParameterList<PARAMETERS> insertParameters, outputFunc: null);

                insertDetail = new PreparedSqlAndParameters<PARAMETERS>(sql, insertParameters);
                _insertDetails[dbTypeIndex] = insertDetail;
            }
            else {
                insertDetail = _insertDetails[dbTypeIndex]!;
            }
            return insertDetail;
        }

        public NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            NonQueryResult result = PreparedQueryExecutor.ExecuteNonQuery(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
            return result;
        }

        public async Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            NonQueryResult result = await PreparedQueryExecutor.ExecuteNonQueryAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }
    }

    internal sealed class PreparedInsertQuery<PARAMETERS, RESULT> : IPreparedInsertQuery<PARAMETERS, RESULT> {

        private readonly PreparedInsertTemplate<PARAMETERS> _template;
        private readonly PreparedSqlAndParameters<PARAMETERS>?[] _insertDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)
        private Func<IResultRow, RESULT> _outputFunc;

        public PreparedInsertQuery(PreparedInsertTemplate<PARAMETERS> template, Func<IResultRow, RESULT> outputFunc) {

            _template = template;
            _outputFunc = outputFunc;

            DatabaseType[] values = Enum.GetValues<DatabaseType>();

            int max = 0;

            foreach(DatabaseType value in values) {

                int valueAsInt = (int)value;

                if(valueAsInt > max) {
                    max = valueAsInt;
                }
            }
            _insertDetails = new PreparedSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public void Initialize(IDatabase database) {
            _ = GetInsertQuery(database);
        }
        private PreparedSqlAndParameters<PARAMETERS> GetInsertQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedSqlAndParameters<PARAMETERS> insertDetail;

            if(_insertDetails[dbTypeIndex] == null) {

                string sql = database.PreparedInsertGenerator.GetSql<PARAMETERS, RESULT>(_template, database, out PreparedParameterList<PARAMETERS> insertParameters, outputFunc: _outputFunc);

                insertDetail = new PreparedSqlAndParameters<PARAMETERS>(sql, insertParameters);
                _insertDetails[dbTypeIndex] = insertDetail;
            }
            else {
                insertDetail = _insertDetails[dbTypeIndex]!;
            }
            return insertDetail;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            QueryResult<RESULT> result = PreparedQueryExecutor.Execute(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            QueryResult<RESULT> result = await PreparedQueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public RESULT? SingleOrDefault(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            return PreparedQueryExecutor.SingleOrDefault(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
        }

        public RESULT? SingleOrDefault(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(database);

            return PreparedQueryExecutor.SingleOrDefault(
                database: database,
                transaction: null,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName
            );
        }

        public async Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            RESULT? result = await PreparedQueryExecutor.SingleOrDefaultAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public async Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(database);

            RESULT? result = await PreparedQueryExecutor.SingleOrDefaultAsync(
                database: database,
                transaction: null,
                timeout: timeout ?? TimeoutLevel.ShortInsert,
                parameters: parameters,
                setParameters: insertDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: insertDetail.Sql,
                queryType: QueryType.Insert,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }
    }
}