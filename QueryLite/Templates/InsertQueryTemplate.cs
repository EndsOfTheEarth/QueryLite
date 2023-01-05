﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal sealed class InsertQueryTemplate : IInsertSet, IInsertSetNext, IInsertExecute {

        public ITable Table { get; }
        public IList<SetValue> SetValues { get; } = new List<SetValue>();
        public IList<IColumn>? ReturningFields { get; private set; }

        public InsertQueryTemplate(ITable table) {
            Table = table;
        }

        public IInsertSetNext Set<TYPE>(Column<TYPE> column, TYPE value) where TYPE : notnull {
            SetValues.Add(new SetValue(column, value));
            return this;
        }

        public IInsertSetNext Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : class {
            SetValues.Add(new SetValue(column, value));
            return this;
        }

        public IInsertSetNext Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : struct {
            SetValues.Add(new SetValue(column, value));
            return this;
        }

        public IInsertSetNext Set<TYPE>(Column<TYPE> column, AFunction<TYPE> function) where TYPE : notnull {
            SetValues.Add(new SetValue(column, function));
            return this;
        }

        public string GetSql(IDatabase database) {

            IParameters parameters = database.CreateParameters();
            return database.InsertGenerator.GetSql(this, database, parameters);
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortInsert;
            }

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.InsertGenerator.GetSql(this, database, parameters);

            NonQueryResult result = QueryExecutor.ExecuteNonQuery(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Insert);

            return result;
        }

        public QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortInsert;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningFields = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.InsertGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Insert,
                selectFields: fieldCollector.Fields,
                fieldCollector: fieldCollector);

            return result;
        }

        public Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortInsert;
            }

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.InsertGenerator.GetSql(this, database, parameters);

            Task<NonQueryResult> result = QueryExecutor.ExecuteNonQueryAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                sql: sql,
                queryType: QueryType.Insert,
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortInsert;
            }

            FieldCollector fieldCollector = new FieldCollector();

            func(fieldCollector);

            ReturningFields = fieldCollector.GetFieldsAsColumns();

            IDatabase database = transaction.Database;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.InsertGenerator.GetSql(this, database, parameters);

            Task<QueryResult<RESULT>> result = QueryExecutor.ExecuteAsync(
                database: database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: func,
                sql: sql,
                queryType: QueryType.Insert,
                selectFields: fieldCollector.Fields,
                fieldCollector: fieldCollector,
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }
    }

    internal sealed class SetValue {

        public IColumn Column { get; }
        public object? Value { get; }

        public SetValue(IColumn column, object? value) {
            Column = column;
            Value = value;
        }
    }
}