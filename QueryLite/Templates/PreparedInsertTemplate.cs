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
            return new PreparedInsertQuery<PARAMETERS>(this);
        }

        public IPreparedInsertQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> outputFunc, IDatabase database) {
            return new PreparedInsertQuery<PARAMETERS, RESULT>(this, outputFunc);
        }
    }

    internal class InsertSqlAndParameters<PARAMETERS> {

        public InsertSqlAndParameters(string sql, List<ISetParameter<PARAMETERS>> setParameters) {
            Sql = sql;
            SetParameters = setParameters;
        }
        public string Sql { get; }
        public List<ISetParameter<PARAMETERS>> SetParameters { get; }
    }

    internal sealed class PreparedInsertQuery<PARAMETERS> : IPreparedInsertQuery<PARAMETERS> {

        private readonly PreparedInsertTemplate<PARAMETERS> _template;

        private readonly InsertSqlAndParameters<PARAMETERS>?[] _insertDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)

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
            _insertDetails = new InsertSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public InsertSqlAndParameters<PARAMETERS> GetInsertQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            InsertSqlAndParameters<PARAMETERS> insertDetail;

            if(_insertDetails[dbTypeIndex] == null) {

                string sql = database.PreparedInsertGenerator.GetSql<PARAMETERS, bool>(_template, database, out List<ISetParameter<PARAMETERS>> insertParameters, outputFunc: null);

                insertDetail = new InsertSqlAndParameters<PARAMETERS>(sql, insertParameters);
                _insertDetails[dbTypeIndex] = insertDetail;
            }
            else {
                insertDetail = _insertDetails[dbTypeIndex]!;
            }
            return insertDetail;
        }

        public NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            InsertSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

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

        public async Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            InsertSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            NonQueryResult result = await PreparedQueryExecutor.ExecuteNonQueryAsync(
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
    }

    internal sealed class PreparedInsertQuery<PARAMETERS, RESULT> : IPreparedInsertQuery<PARAMETERS, RESULT> {

        private readonly PreparedInsertTemplate<PARAMETERS> _template;
        private readonly InsertSqlAndParameters<PARAMETERS>?[] _insertDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)
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
            _insertDetails = new InsertSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public InsertSqlAndParameters<PARAMETERS> GetInsertQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            InsertSqlAndParameters<PARAMETERS> insertDetail;

            if(_insertDetails[dbTypeIndex] == null) {

                string sql = database.PreparedInsertGenerator.GetSql<PARAMETERS, RESULT>(_template, database, out List<ISetParameter<PARAMETERS>> insertParameters, outputFunc: _outputFunc);

                insertDetail = new InsertSqlAndParameters<PARAMETERS>(sql, insertParameters);
                _insertDetails[dbTypeIndex] = insertDetail;
            }
            else {
                insertDetail = _insertDetails[dbTypeIndex]!;
            }
            return insertDetail;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            InsertSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

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

        public async Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            InsertSqlAndParameters<PARAMETERS> insertDetail = GetInsertQuery(transaction.Database);

            QueryResult<RESULT> result = await PreparedQueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                cancellationToken: cancellationToken ?? CancellationToken.None,
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
    }
}