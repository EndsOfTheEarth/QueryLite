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

    internal sealed class UpdateQueryTemplate : IUpdateSet, IUpdateJoin, IUpdateWhere, IUpdateExecute {

        public ITable Table { get; }
        public IList<SetValue> SetValues { get; } = new List<SetValue>();
        public IList<IJoin>? Joins { get; private set; }
        public ICondition? WhereCondition { get; private set; }
        public IList<IColumn>? ReturningColumns { get; private set; }

        public UpdateQueryTemplate(ITable table) {
            Table = table;
        }

        public IUpdateJoin Set<TYPE>(Column<TYPE> column, TYPE value) where TYPE : notnull {
            SetValues.Add(new SetValue(column, value));
            return this;
        }
        public IUpdateJoin Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : class {
            SetValues.Add(new SetValue(column, value));
            return this;
        }
        public IUpdateJoin Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : struct {
            SetValues.Add(new SetValue(column, value));
            return this;
        }
        public IUpdateJoin Set<TYPE>(Column<TYPE> column, AFunction<TYPE> function) where TYPE : notnull {
            SetValues.Add(new SetValue(column, function));
            return this;
        }

        public IUpdateJoinOn Join(ITable table) {

            if(Joins == null) {
                Joins = new List<IJoin>(1);
            }
            UpdateJoin join = new UpdateJoin(JoinType.Join, table, this);
            Joins.Add(join);
            return join;
        }

        public IUpdateJoinOn LeftJoin(ITable table) {

            if(Joins == null) {
                Joins = new List<IJoin>(1);
            }
            UpdateJoin join = new UpdateJoin(JoinType.LeftJoin, table, this);
            Joins.Add(join);
            return join;
        }

        public IUpdateExecute NoWhereCondition() {
            WhereCondition = null;
            return this;
        }
        public IUpdateExecute Where(ICondition condition) {

            if(WhereCondition != null) {
                throw new Exception($"Where condition has already been set");
            }
            WhereCondition = condition;
            return this;
        }
        
        public string GetSql(IDatabase database) {
            IParameters parameters = database.CreateParameters();
            return database.UpdateGenerator.GetSql(this, database, parameters);
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortUpdate;
            }

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.UpdateGenerator.GetSql(this, database, parameters);

            NonQueryResult result = QueryExecutor.ExecuteNonQuery(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Update,
                debugName: debugName
            );
            return result;
        }

        public QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortUpdate;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningColumns = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.UpdateGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Update,
                selectFields: fieldCollector.Fields,
                fieldCollector: fieldCollector,
                debugName: debugName
            );

            return result;
        }

        public Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortUpdate;
            }

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.UpdateGenerator.GetSql(this, database, parameters);

            Task<NonQueryResult> result = QueryExecutor.ExecuteNonQueryAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Update,
                debugName: debugName,
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortUpdate;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningColumns = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.UpdateGenerator.GetSql(this, database, parameters);

            Task<QueryResult<RESULT>> result = QueryExecutor.ExecuteAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Update,
                selectFields: fieldCollector.Fields,
                fieldCollector: fieldCollector,
                debugName: debugName,
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }
    }
}