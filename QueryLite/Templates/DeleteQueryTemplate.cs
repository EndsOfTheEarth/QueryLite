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
            Table = table;
        }
        public DeleteQueryTemplate(ITable table, ICondition whereCondition) {
            Table = table;
            WhereCondition = whereCondition;
        }

        public IDeleteJoinOn Join(ITable table) {

            if(Joins == null) {
                Joins = new List<IJoin>(1);
            }
            DeleteJoin join = new DeleteJoin(JoinType.Join, table, this);
            Joins.Add(join);
            return join;
        }

        public IDeleteJoinOn LeftJoin(ITable table) {

            if(Joins == null) {
                Joins = new List<IJoin>(1);
            }
            DeleteJoin join = new DeleteJoin(JoinType.LeftJoin, table, this);
            Joins.Add(join);
            return join;
        }

        public IDeleteExecute Where(ICondition condition) {

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

            IParameters parameters = database.CreateParameters();
            return database.DeleteGenerator.GetSql(this, database, parameters);
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            NonQueryResult result = QueryExecutor.ExecuteNonQuery(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Delete);

            return result;
        }


        public QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningColumns = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Update,
                selectFields: fieldCollector.Fields,
                fieldCollector: fieldCollector);

            return result;
        }

        public Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

            Task<NonQueryResult> result = QueryExecutor.ExecuteNonQueryAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Delete,
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningColumns = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.DeleteGenerator.GetSql(this, database, parameters);

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
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }
    }
}