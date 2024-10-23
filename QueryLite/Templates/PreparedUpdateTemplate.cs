/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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
using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal sealed class PreparedUpdateTemplate<PARAMETERS> : IPreparedUpdateSet<PARAMETERS>, IPreparedUpdateFrom<PARAMETERS>, IPreparedUpdateWhere<PARAMETERS>, IPreparedUpdateBuild<PARAMETERS> {

        public ITable Table { get; }
        public Action<IPreparedSetValuesCollector<PARAMETERS>>? SetValues { get; private set; }
        public List<ITable>? FromTables { get; private set; }
        public APreparedCondition<PARAMETERS>? WhereCondition { get; private set; }

        public PreparedUpdateTemplate(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            Table = table;
        }

        public IPreparedUpdateFrom<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values) {

            ArgumentNullException.ThrowIfNull(values);
            SetValues = values;
            return this;
        }

        public IPreparedUpdateWhere<PARAMETERS> From(ITable table, params ITable[] tables) {

            ArgumentNullException.ThrowIfNull(table);

            FromTables = new List<ITable>(tables.Length + 1);

            FromTables.Add(table);

            if(tables.Length > 0) {
                FromTables.AddRange(tables);
            }
            return this;
        }

        public IPreparedUpdateBuild<PARAMETERS> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> condition) {

            ArgumentNullException.ThrowIfNull(condition);

            WhereCondition = condition(new EmptyPreparedCondition<PARAMETERS>());
            return this;
        }

        public IPreparedUpdateBuild<PARAMETERS> NoWhereCondition() {

            WhereCondition = null;
            return this;
        }

        public IPreparedUpdateQuery<PARAMETERS> Build() {
            return new PreparedUpdateQuery<PARAMETERS>(this);
        }

        public IPreparedUpdateQuery<PARAMETERS, RESULT> Build<RESULT>(Func<IResultRow, RESULT> returningFunc) {
            return new PreparedUpdateQuery<PARAMETERS, RESULT>(this, returningFunc);
        }
    }

    internal sealed class PreparedSqlAndParameters<PARAMETERS> {

        public PreparedSqlAndParameters(string sql, PreparedParameterList<PARAMETERS> setParameters) {
            Sql = sql;
            SetParameters = setParameters;
        }
        public string Sql { get; }
        public PreparedParameterList<PARAMETERS> SetParameters { get; }
    }

    internal sealed class PreparedUpdateQuery<PARAMETERS> : IPreparedUpdateQuery<PARAMETERS> {

        private readonly PreparedUpdateTemplate<PARAMETERS> _template;

        private readonly PreparedSqlAndParameters<PARAMETERS>?[] _updateDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)

        public PreparedUpdateQuery(PreparedUpdateTemplate<PARAMETERS> template) {

            _template = template;

            DatabaseType[] values = Enum.GetValues<DatabaseType>();

            int max = 0;

            foreach(DatabaseType value in values) {

                int valueAsInt = (int)value;

                if(valueAsInt > max) {
                    max = valueAsInt;
                }
            }
            _updateDetails = new PreparedSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public void Initialize(IDatabase database) {
            _ = GetUpdateQuery(database);
        }
        private PreparedSqlAndParameters<PARAMETERS> GetUpdateQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedSqlAndParameters<PARAMETERS> updateDetail;

            if(_updateDetails[dbTypeIndex] == null) {

                string sql = database.PreparedUpdateGenerator.GetSql<PARAMETERS, bool>(_template, database, out PreparedParameterList<PARAMETERS> parameters, outputFunc: null);

                updateDetail = new PreparedSqlAndParameters<PARAMETERS>(sql, parameters);
                _updateDetails[dbTypeIndex] = updateDetail;
            }
            else {
                updateDetail = _updateDetails[dbTypeIndex]!;
            }
            return updateDetail;
        }

        public NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetUpdateQuery(transaction.Database);

            NonQueryResult result = PreparedQueryExecutor.ExecuteNonQuery(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortUpdate,
                parameters: parameters,
                setParameters: UpdateDetail.SetParameters,
                sql: UpdateDetail.Sql,
                queryType: QueryType.Update,
                debugName: debugName
            );
            return result;
        }

        public async Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetUpdateQuery(transaction.Database);

            NonQueryResult result = await PreparedQueryExecutor.ExecuteNonQueryAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortUpdate,
                parameters: parameters,
                setParameters: UpdateDetail.SetParameters,
                sql: UpdateDetail.Sql,
                queryType: QueryType.Update,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }
    }

    internal sealed class PreparedUpdateQuery<PARAMETERS, RESULT> : IPreparedUpdateQuery<PARAMETERS, RESULT> {

        private readonly PreparedUpdateTemplate<PARAMETERS> _template;
        private readonly PreparedSqlAndParameters<PARAMETERS>?[] _updateDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)
        private Func<IResultRow, RESULT> _outputFunc;

        public PreparedUpdateQuery(PreparedUpdateTemplate<PARAMETERS> template, Func<IResultRow, RESULT> outputFunc) {

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
            _updateDetails = new PreparedSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public void Initialize(IDatabase database) {
            _ = GetUpdateQuery(database);
        }
        private PreparedSqlAndParameters<PARAMETERS> GetUpdateQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedSqlAndParameters<PARAMETERS> updateDetail;

            if(_updateDetails[dbTypeIndex] == null) {

                string sql = database.PreparedUpdateGenerator.GetSql<PARAMETERS, RESULT>(_template, database, out PreparedParameterList<PARAMETERS> UpdateParameters, outputFunc: _outputFunc);

                updateDetail = new PreparedSqlAndParameters<PARAMETERS>(sql, UpdateParameters);
                _updateDetails[dbTypeIndex] = updateDetail;
            }
            else {
                updateDetail = _updateDetails[dbTypeIndex]!;
            }
            return updateDetail;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetUpdateQuery(transaction.Database);

            QueryResult<RESULT> result = PreparedQueryExecutor.Execute(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortUpdate,
                parameters: parameters,
                setParameters: UpdateDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: UpdateDetail.Sql,
                queryType: QueryType.Update,
                debugName: debugName
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetUpdateQuery(transaction.Database);

            QueryResult<RESULT> result = await PreparedQueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortUpdate,
                parameters: parameters,
                setParameters: UpdateDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: UpdateDetail.Sql,
                queryType: QueryType.Update,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public RESULT? SingleOrDefault(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetUpdateQuery(transaction.Database);

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

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetUpdateQuery(database);

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

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetUpdateQuery(transaction.Database);

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

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetUpdateQuery(database);

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