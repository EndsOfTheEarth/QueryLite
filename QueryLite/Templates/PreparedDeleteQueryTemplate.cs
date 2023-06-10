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
using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace QueryLite {

    internal sealed class PreparedDeleteQueryTemplate<PARAMETERS> : IPreparedDeleteUsing<PARAMETERS>, IPreparedDeleteJoin<PARAMETERS>, IPreparedDeleteWhere<PARAMETERS>, IPreparedDeleteBuild<PARAMETERS> where PARAMETERS : notnull {

        public ITable Table { get; }
        public IList<ITable>? Usings { get; private set; }
        public IList<PreparedDeleteJoin<PARAMETERS>>? Joins { get; private set; }
        public APreparedCondition<PARAMETERS>? WhereCondition { get; private set; }

        public PreparedDeleteQueryTemplate(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            Table = table;
        }

        public IPreparedDeleteWhere<PARAMETERS> Using(params ITable[] tables) {
            ArgumentNullException.ThrowIfNull(tables);
            Usings = new List<ITable>(tables);
            return this;
        }

        public IPreparedDeleteJoinOn<PARAMETERS> Join(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            PreparedDeleteJoin<PARAMETERS> join = new PreparedDeleteJoin<PARAMETERS>(JoinType.Join, table, this);

            if(Joins == null) {
                Joins = new List<PreparedDeleteJoin<PARAMETERS>>();
            }
            Joins.Add(join);
            return join;
        }

        public IPreparedDeleteJoinOn<PARAMETERS> LeftJoin(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            PreparedDeleteJoin<PARAMETERS> join = new PreparedDeleteJoin<PARAMETERS>(JoinType.LeftJoin, table, this);

            if(Joins == null) {
                Joins = new List<PreparedDeleteJoin<PARAMETERS>>();
            }
            Joins.Add(join);
            return join;
        }

        public IPreparedDeleteBuild<PARAMETERS> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> condition) {

            ArgumentNullException.ThrowIfNull(condition);
            WhereCondition = condition(new EmptyPreparedCondition<PARAMETERS>());
            return this;
        }

        public IPreparedDeleteBuild<PARAMETERS> NoWhereCondition() {
            WhereCondition = null;
            return this;
        }

        public IPreparedDeleteQuery<PARAMETERS> Build() {
            return new PreparedDeleteQuery<PARAMETERS>(this);
        }

        public IPreparedDeleteQuery<PARAMETERS, RETURNING> Build<RETURNING>(Func<IResultRow, RETURNING> func) {
            return new PreparedDeleteQuery<PARAMETERS, RETURNING>(this, func);
        }
    }

    internal sealed class PreparedDeleteQuery<PARAMETERS> : IPreparedDeleteQuery<PARAMETERS> where PARAMETERS : notnull {

        private readonly PreparedDeleteQueryTemplate<PARAMETERS> _template;
        
        private readonly PreparedSqlAndParameters<PARAMETERS>?[] _deleteDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)

        public PreparedDeleteQuery(PreparedDeleteQueryTemplate<PARAMETERS> template) {

            _template = template;

            DatabaseType[] values = Enum.GetValues<DatabaseType>();

            int max = 0;

            foreach(DatabaseType value in values) {

                int valueAsInt = (int)value;

                if(valueAsInt > max) {
                    max = valueAsInt;
                }
            }
            _deleteDetails = new PreparedSqlAndParameters<PARAMETERS>?[max + 1];
        }

        public void Initialize(IDatabase database) {
            _ = GetDeleteQuery(database);
        }
        private PreparedSqlAndParameters<PARAMETERS> GetDeleteQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedSqlAndParameters<PARAMETERS> updateDetail;

            if(_deleteDetails[dbTypeIndex] == null) {

                string sql = database.PreparedDeleteQueryGenerator.GetSql<PARAMETERS, bool>(_template, database, out PreparedParameterList<PARAMETERS> parameters, outputFunc: null);

                updateDetail = new PreparedSqlAndParameters<PARAMETERS>(sql, parameters);
                _deleteDetails[dbTypeIndex] = updateDetail;
            }
            else {
                updateDetail = _deleteDetails[dbTypeIndex]!;
            }
            return updateDetail;
        }

        public NonQueryResult Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetDeleteQuery(transaction.Database);

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

        public Task<NonQueryResult> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetDeleteQuery(transaction.Database);

            return PreparedQueryExecutor.ExecuteNonQueryAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortUpdate,
                parameters: parameters,
                setParameters: UpdateDetail.SetParameters,
                sql: UpdateDetail.Sql,
                queryType: QueryType.Update,
                debugName: debugName
,
                cancellationToken: cancellationToken ?? CancellationToken.None);
        }
    }

    internal sealed class PreparedDeleteQuery<PARAMETERS, RESULT> : IPreparedDeleteQuery<PARAMETERS, RESULT> where PARAMETERS : notnull {

        private readonly PreparedDeleteQueryTemplate<PARAMETERS> _template;
        private readonly PreparedSqlAndParameters<PARAMETERS>?[] _updateDetails;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)
        private Func<IResultRow, RESULT> _outputFunc;

        public PreparedDeleteQuery(PreparedDeleteQueryTemplate<PARAMETERS> template, Func<IResultRow, RESULT> outputFunc) {

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
            _ = GetDeleteQuery(database);
        }
        private PreparedSqlAndParameters<PARAMETERS> GetDeleteQuery(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedSqlAndParameters<PARAMETERS> updateDetail;

            if(_updateDetails[dbTypeIndex] == null) {

                string sql = database.PreparedDeleteQueryGenerator.GetSql(_template, database, out PreparedParameterList<PARAMETERS> UpdateParameters, outputFunc: _outputFunc);

                updateDetail = new PreparedSqlAndParameters<PARAMETERS>(sql, UpdateParameters);
                _updateDetails[dbTypeIndex] = updateDetail;
            }
            else {
                updateDetail = _updateDetails[dbTypeIndex]!;
            }
            return updateDetail;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetDeleteQuery(transaction.Database);

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

        public Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> UpdateDetail = GetDeleteQuery(transaction.Database);

            return PreparedQueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout ?? TimeoutLevel.ShortUpdate,
                parameters: parameters,
                setParameters: UpdateDetail.SetParameters,
                outputFunc: _outputFunc,
                sql: UpdateDetail.Sql,
                queryType: QueryType.Update,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None);
        }

        public RESULT? SingleOrDefault(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetDeleteQuery(transaction.Database);

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

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetDeleteQuery(database);

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

        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetDeleteQuery(transaction.Database);

            return PreparedQueryExecutor.SingleOrDefaultAsync(
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
        }

        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedSqlAndParameters<PARAMETERS> insertDetail = GetDeleteQuery(database);

            return PreparedQueryExecutor.SingleOrDefaultAsync(
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
        }
    }
}