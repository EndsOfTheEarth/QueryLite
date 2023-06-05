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
using QueryLite.Databases.SqlServer.Collectors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite
{

    public enum QueryType {
        Select,
        Insert,
        Update,
        Delete,
        Truncate
    }

    internal static class QueryExecutor {

        public static QueryResult<RESULT> Execute<RESULT>(
            IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            IParametersBuilder? parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;    //Using this flag to speed up code when there are no subscribed events

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                if(parameters != null) {

                    for(int index = 0; index < parameters.ParameterList.Count; index++) {
                        command.Parameters.Add(parameters.ParameterList[index]);
                    }
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = command.ExecuteReader();

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                List<RESULT> rowList = new List<RESULT>();

                while(reader.Read()) {
                    rowList.Add(func(resultRow));
                    resultRow.Reset();
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                //Note: Reader must be closed for RecordsAffected to be populated
                QueryResult<RESULT> result = new QueryResult<RESULT>(rowList, sql, rowsEffected: (reader.RecordsAffected != -1 ? reader.RecordsAffected : 0));

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: result.Rows.Count,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static NonQueryResult ExecuteNonQuery(
            IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            IParametersBuilder? parameters,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                if(parameters != null) {

                    for(int index = 0; index < parameters.ParameterList.Count; index++) {
                        command.Parameters.Add(parameters.ParameterList[index]);
                    }
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                int rowsEffected = command.ExecuteNonQuery();
                NonQueryResult result = new NonQueryResult(sql, rowsEffected);

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: 0,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }


        public static RESULT? SingleOrDefault<RESULT>(IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            IParametersBuilder? parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                if(parameters != null) {

                    for(int index = 0; index < parameters.ParameterList.Count; index++) {
                        command.Parameters.Add(parameters.ParameterList[index]);
                    }
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = command.ExecuteReader();

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                bool isFirst = true;

                RESULT? result = default;

                while(reader.Read()) {

                    if(!isFirst) {
                        throw new Exception("More than one record exists in result");
                    }
                    result = func(resultRow);
                    isFirst = false;
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    List<RESULT> resultRows = result != null ? new List<RESULT> { result! } : new List<RESULT>();

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: !isFirst ? 1 : 0,
                        rowsEffected: 0,
                        queryType: queryType,
                        result: new QueryResult<RESULT>(resultRows, sql, rowsEffected: 0),
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static async Task<RESULT?> SingleOrDefaultAsync<RESULT>(IDatabase database,
            Transaction? transaction,
            CancellationToken cancellationToken,
            QueryTimeout timeout,
            IParametersBuilder? parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                if(parameters != null) {

                    for(int index = 0; index < parameters.ParameterList.Count; index++) {
                        command.Parameters.Add(parameters.ParameterList[index]);
                    }
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                bool isFirst = true;

                RESULT? result = default;

                while(reader.Read()) {

                    if(!isFirst) {
                        throw new Exception("More than one record exists in result");
                    }
                    result = func(resultRow);
                    isFirst = false;
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    List<RESULT> resultRows = result != null ? new List<RESULT> { result! } : new List<RESULT>();

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: !isFirst ? 1 : 0,
                        rowsEffected: 0,
                        queryType: queryType,
                        result: new QueryResult<RESULT>(resultRows, sql, rowsEffected: 0),
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static async Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(
            IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            IParametersBuilder? parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName,
            CancellationToken cancellationToken) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                if(parameters != null) {

                    for(int index = 0; index < parameters.ParameterList.Count; index++) {
                        command.Parameters.Add(parameters.ParameterList[index]);
                    }
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                List<RESULT> rowList = new List<RESULT>();

                while(reader.Read()) {
                    rowList.Add(func(resultRow));
                    resultRow.Reset();
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                //Note: Reader must be closed for RecordsAffected to be populated
                QueryResult<RESULT> result = new QueryResult<RESULT>(rowList, sql, rowsEffected: (reader.RecordsAffected != -1 ? reader.RecordsAffected : 0));

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: result.Rows.Count,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static async Task<NonQueryResult> ExecuteNonQueryAsync(
            IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            IParametersBuilder? parameters,
            string sql,
            QueryType queryType,
            string debugName,
            CancellationToken cancellationToken) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                if(parameters != null) {

                    for(int index = 0; index < parameters.ParameterList.Count; index++) {
                        command.Parameters.Add(parameters.ParameterList[index]);
                    }
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                int rowsEffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                NonQueryResult result = new NonQueryResult(sql, rowsEffected);

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: 0,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }
    }

    internal static class PreparedQueryExecutor {

        public static QueryResult<RESULT> Execute<PARAMETERS, RESULT>(
            IDatabase database,
            PARAMETERS paramValue,
            Transaction? transaction,
            QueryTimeout timeout,
            IList<QueryParameter<PARAMETERS>> parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;    //Using this flag to speed up code when there are no subscribed events

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                for(int paramIndex = 0; paramIndex < parameters.Count; paramIndex++) {
                    command.Parameters.Add(parameters[paramIndex].CreateParameter(paramValue));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = command.ExecuteReader();

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                List<RESULT> rowList = new List<RESULT>();

                while(reader.Read()) {
                    rowList.Add(func(resultRow));
                    resultRow.Reset();
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                //Note: Reader must be closed for RecordsAffected to be populated
                QueryResult<RESULT> result = new QueryResult<RESULT>(rowList, sql, rowsEffected: (reader.RecordsAffected != -1 ? reader.RecordsAffected : 0));

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: result.Rows.Count,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static NonQueryResult ExecuteNonQuery<PARAMETERS>(
            IDatabase database,
            PARAMETERS paramValue,
            Transaction? transaction,
            QueryTimeout timeout,
            IList<QueryParameter<PARAMETERS>> parameters,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                for(int paramIndex = 0; paramIndex < parameters.Count; paramIndex++) {
                    command.Parameters.Add(parameters[paramIndex].CreateParameter(paramValue));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                int rowsEffected = command.ExecuteNonQuery();
                NonQueryResult result = new NonQueryResult(sql, rowsEffected);

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: 0,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static RESULT? SingleOrDefault<PARAMETERS, RESULT>(
            IDatabase database,
            PARAMETERS paramValue,
            Transaction? transaction,
            QueryTimeout timeout,
            IList<QueryParameter<PARAMETERS>> parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                for(int paramIndex = 0; paramIndex < parameters.Count; paramIndex++) {
                    command.Parameters.Add(parameters[paramIndex].CreateParameter(paramValue));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = command.ExecuteReader();

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                bool isFirst = true;

                RESULT? result = default;

                while(reader.Read()) {

                    if(!isFirst) {
                        throw new Exception("More than one record exists in result");
                    }
                    result = func(resultRow);
                    isFirst = false;
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    List<RESULT> resultRows = result != null ? new List<RESULT> { result! } : new List<RESULT>();

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: !isFirst ? 1 : 0,
                        rowsEffected: 0,
                        queryType: queryType,
                        result: new QueryResult<RESULT>(resultRows, sql, rowsEffected: 0),
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static async Task<RESULT?> SingleOrDefaultAsync<PARAMETERS, RESULT>(
            IDatabase database,
            PARAMETERS paramValue,
            Transaction? transaction,
            CancellationToken cancellationToken,
            QueryTimeout timeout,
            IList<QueryParameter<PARAMETERS>> parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                for(int paramIndex = 0; paramIndex < parameters.Count; paramIndex++) {
                    command.Parameters.Add(parameters[paramIndex].CreateParameter(paramValue));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                bool isFirst = true;

                RESULT? result = default;

                while(reader.Read()) {

                    if(!isFirst) {
                        throw new Exception("More than one record exists in result");
                    }
                    result = func(resultRow);
                    isFirst = false;
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    List<RESULT> resultRows = result != null ? new List<RESULT> { result! } : new List<RESULT>();

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: !isFirst ? 1 : 0,
                        rowsEffected: 0,
                        queryType: queryType,
                        result: new QueryResult<RESULT>(resultRows, sql, rowsEffected: 0),
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static async Task<QueryResult<RESULT>> ExecuteAsync<PARAMETERS, RESULT>(
            IDatabase database,
            PARAMETERS paramValue,
            Transaction? transaction,
            QueryTimeout timeout,
            IList<QueryParameter<PARAMETERS>> parameters,
            Func<IResultRow, RESULT> func,
            string sql,
            QueryType queryType,
            //IList<IField> selectFields,
            //FieldCollector fieldCollector,
            string debugName,
            CancellationToken cancellationToken) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                for(int paramIndex = 0; paramIndex < parameters.Count; paramIndex++) {
                    command.Parameters.Add(parameters[paramIndex].CreateParameter(paramValue));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                List<RESULT> rowList = new List<RESULT>();

                while(reader.Read()) {
                    rowList.Add(func(resultRow));
                    resultRow.Reset();
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                //Note: Reader must be closed for RecordsAffected to be populated
                QueryResult<RESULT> result = new QueryResult<RESULT>(rowList, sql, rowsEffected: (reader.RecordsAffected != -1 ? reader.RecordsAffected : 0));

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: result.Rows.Count,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }

        public static async Task<NonQueryResult> ExecuteNonQueryAsync<PARAMETERS>(
            IDatabase database,
            PARAMETERS paramValue,
            Transaction? transaction,
            QueryTimeout timeout,
            IList<QueryParameter<PARAMETERS>> parameters,
            string sql,
            QueryType queryType,
            string debugName,
            CancellationToken cancellationToken) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;

                for(int paramIndex = 0; paramIndex < parameters.Count; paramIndex++) {
                    command.Parameters.Add(parameters[paramIndex].CreateParameter(paramValue));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                int rowsEffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                NonQueryResult result = new NonQueryResult(sql, rowsEffected);

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: 0,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }







        public static QueryResult<RESULT> Execute<PARAMETERS, RESULT>(
            IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            PARAMETERS parameters,
            List<ISetParameter<PARAMETERS>> setParameters,
            Func<IResultRow, RESULT> outputFunc,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;    //Using this flag to speed up code when there are no subscribed events

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;


                for(int index = 0; index < setParameters.Count; index++) {

                    ISetParameter<PARAMETERS> param = setParameters[index];

                    command.Parameters.Add(param.CreateParameter(parameters));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = command.ExecuteReader();

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                List<RESULT> rowList = new List<RESULT>();

                while(reader.Read()) {
                    rowList.Add(outputFunc(resultRow));
                    resultRow.Reset();
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                //Note: Reader must be closed for RecordsAffected to be populated
                QueryResult<RESULT> result = new QueryResult<RESULT>(rowList, sql, rowsEffected: (reader.RecordsAffected != -1 ? reader.RecordsAffected : 0));

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: result.Rows.Count,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }





        public static async Task<QueryResult<RESULT>> ExecuteAsync<PARAMETERS, RESULT>(
            IDatabase database,
            Transaction? transaction,
            CancellationToken cancellationToken,
            QueryTimeout timeout,
            PARAMETERS parameters,
            List<ISetParameter<PARAMETERS>> setParameters,
            Func<IResultRow, RESULT> outputFunc,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;    //Using this flag to speed up code when there are no subscribed events

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;


                for(int index = 0; index < setParameters.Count; index++) {

                    ISetParameter<PARAMETERS> param = setParameters[index];

                    command.Parameters.Add(param.CreateParameter(parameters));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

                IResultRow resultRow = SelectCollectorCache.Acquire(database.DatabaseType, reader);

                List<RESULT> rowList = new List<RESULT>();

                while(reader.Read()) {
                    rowList.Add(outputFunc(resultRow));
                    resultRow.Reset();
                }

                SelectCollectorCache.Release(database.DatabaseType, resultRow);

                reader.Close();

                //Note: Reader must be closed for RecordsAffected to be populated
                QueryResult<RESULT> result = new QueryResult<RESULT>(rowList, sql, rowsEffected: (reader.RecordsAffected != -1 ? reader.RecordsAffected : 0));

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: result.Rows.Count,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }








        public static NonQueryResult ExecuteNonQuery<PARAMETERS>(
            IDatabase database,
            Transaction? transaction,
            QueryTimeout timeout,
            PARAMETERS parameters,
            List<ISetParameter<PARAMETERS>> setParameters,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;    //Using this flag to speed up code when there are no subscribed events

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    dbConnection.Open();
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        dbConnection.Open();
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;


                for(int index = 0; index < setParameters.Count; index++) {

                    ISetParameter<PARAMETERS> param = setParameters[index];

                    command.Parameters.Add(param.CreateParameter(parameters));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                int rowsEffected = command.ExecuteNonQuery();

                NonQueryResult result = new NonQueryResult(sql, rowsEffected);

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: 0,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }




        public static async Task<NonQueryResult> ExecuteNonQueryAsync<PARAMETERS>(
            IDatabase database,
            Transaction? transaction,
            CancellationToken cancellationToken,
            QueryTimeout timeout,
            PARAMETERS parameters,
            List<ISetParameter<PARAMETERS>> setParameters,
            string sql,
            QueryType queryType,
            string debugName) {

            DbConnection? dbConnection = null;

            bool closeConnection = false;

            bool hasEvents = Settings.HasEvents;    //Using this flag to speed up code when there are no subscribed events

            DateTimeOffset? start = hasEvents ? DateTimeOffset.Now : null;

            long? startTicks = hasEvents ? Stopwatch.GetTimestamp() : null;

            try {

                if(hasEvents) {

                    Settings.FireQueryExecutingEvent(
                        database: database,
                        sql: sql,
                        queryType: queryType,
                        start: start,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }

                if(transaction == null) {
                    closeConnection = true;
                    dbConnection = database.GetNewConnection();
                    await dbConnection.OpenAsync(cancellationToken);
                }
                else {

                    DbTransaction? dbTransaction = transaction.GetTransaction(database);

                    if(dbTransaction == null) {
                        dbConnection = database.GetNewConnection();
                        await dbConnection.OpenAsync(cancellationToken);
                        transaction.SetTransaction(dbConnection, dbConnection.BeginTransaction(transaction.IsolationLevel));
                    }
                    else {
                        dbConnection = dbTransaction.Connection;
                    }
                    closeConnection = false;
                }

                using DbCommand command = dbConnection!.CreateCommand();

                command.CommandText = sql;
                command.Transaction = transaction != null ? transaction.GetTransaction(database)! : null;

                command.CommandTimeout = timeout.Seconds;


                for(int index = 0; index < setParameters.Count; index++) {

                    ISetParameter<PARAMETERS> param = setParameters[index];

                    command.Parameters.Add(param.CreateParameter(parameters));
                }

                if((queryType == QueryType.Select && Settings.BreakOnSelectQuery) || (queryType == QueryType.Insert && Settings.BreakOnInsertQuery) || (queryType == QueryType.Update && Settings.BreakOnUpdateQuery) || (queryType == QueryType.Delete && Settings.BreakOnDeleteQuery) || (queryType == QueryType.Truncate && Settings.BreakOnTruncateQuery)) {

                    if(Debugger.IsAttached) {
                        Debugger.Break();
                    }
                }

                int rowsEffected = await command.ExecuteNonQueryAsync();

                NonQueryResult result = new NonQueryResult(sql, rowsEffected);

                if(closeConnection) {
                    dbConnection.Close();
                }

                if(hasEvents) {

                    Settings.FireQueryPerformedEvent(
                        database: database,
                        sql: sql,
                        rows: 0,
                        rowsEffected: result.RowsEffected,
                        queryType: queryType,
                        result: result,
                        start: start,
                        end: DateTimeOffset.Now,
                        elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                        exception: null,
                        isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                        transactionId: transaction?.TransactionId,
                        debugName: debugName
                    );
                }
                return result;
            }
            catch(Exception ex) {

                Settings.FireQueryPerformedEvent(
                    database: database,
                    sql: sql,
                    rows: 0,
                    rowsEffected: 0,
                    queryType: queryType,
                    result: null,
                    start: start,
                    end: DateTimeOffset.Now,
                    elapsedTime: startTicks != null ? Stopwatch.GetElapsedTime(startTicks.Value) : null,
                    exception: ex,
                    isolationLevel: transaction != null ? transaction.IsolationLevel : IsolationLevel.ReadCommitted,
                    transactionId: transaction?.TransactionId,
                    debugName: debugName
                );
                throw;
            }
            finally {
                if(closeConnection && dbConnection != null && dbConnection.State != ConnectionState.Closed) {
                    dbConnection.Dispose();
                }
            }
        }
    }
}