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
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace QueryLite {

    public enum TransactionState {

    }
    /// <summary>
    /// Database transaction
    /// </summary>
    public sealed class Transaction : IDisposable, IAsyncDisposable {

        /// <summary>
        /// Identifier for transaction. Used for debugging.
        /// </summary>
        public ulong TransactionId { get; }

        public IDatabase Database { get; }

        /// <summary>
        /// Isolation level of transaction
        /// </summary>
        public IsolationLevel IsolationLevel { get; private set; }

        public Transaction(IDatabase database, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) {

            ArgumentNullException.ThrowIfNull(database, paramName: nameof(database));

            Database = database;
            IsolationLevel = isolationLevel;
            TransactionId = GetNextId();
        }

        private DbTransaction? DbTransaction { get; set; }

        internal DbTransaction? GetTransaction(IDatabase database) {

            if(Database != database) {
                throw new Exception("Transaction is being used to query two different connections");
            }
            return DbTransaction;
        }

        internal void SetTransaction(DbTransaction dbTransaction) {

            if(DbTransaction != null) {
                throw new Exception($"Cannot set {nameof(DbTransaction)} twice");
            }
            DbTransaction = dbTransaction;
        }

        /// <summary>
        /// Creates a new ado command from the transactions connection. Please note you will need to correctly dispose of this command object.
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateCommand(QueryTimeout timeout) {

            DbTransaction? dbTransaction = GetTransaction(Database);

            DbConnection dbConnection;

            if(dbTransaction == null) {
                dbConnection = Database.GetNewConnection();
                dbConnection.Open();
                SetTransaction(dbConnection.BeginTransaction(IsolationLevel));
                dbTransaction = DbTransaction;
            }
            else {
                dbConnection = dbTransaction.Connection!;
            }
            DbCommand command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction!;
            command.CommandTimeout = timeout.Seconds;
            return command;
        }

        public async Task<DbCommand> CreateCommandAsync(QueryTimeout timeout, CancellationToken ct) {

            DbTransaction? dbTransaction = GetTransaction(Database);

            DbConnection dbConnection;

            if(dbTransaction == null) {
                dbConnection = Database.GetNewConnection();
                await dbConnection.OpenAsync(ct);
                SetTransaction(await dbConnection.BeginTransactionAsync(IsolationLevel, ct));
                dbTransaction = DbTransaction;
            }
            else {
                dbConnection = dbTransaction.Connection!;
            }
            DbCommand command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction!;
            command.CommandTimeout = timeout.Seconds;
            return command;
        }

        /// <summary>
        /// Counter used to give each transaction a unique id mainly for debugging purposes
        /// </summary>
        private static volatile uint sCounter = 0;

        private static ulong GetNextId() {
            return sCounter++;
        }

        /// <summary>
        /// Dispose transaction. Will roll back transaction if it is not yet committed.
        /// </summary>
        public void Dispose() {

            using(DbConnection? connection = DbTransaction?.Connection) {
                Rollback();
            }
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync() {

            using(DbConnection? connection = DbTransaction?.Connection) {
                await RollbackAsync().ConfigureAwait(false);
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commit transaction in database
        /// </summary>
        public void Commit() {

            if(DbTransaction == null) {
                return;
            }
            try {

                using(DbConnection? connection = DbTransaction.Connection) {

                    using(DbTransaction) {
                        DbTransaction.Commit();
                        ResetIsolationLevel(connection);
                    }
                }
            }
            finally {
                DbTransaction = null;
            }
        }

        /// <summary>
        /// Commit transaction in database
        /// </summary>
        public async Task CommitAsync(CancellationToken ct = default) {

            if(DbTransaction == null) {
                return;
            }
            try {

                await using(DbConnection? connection = DbTransaction.Connection) {

                    await using(DbTransaction) {
                        await DbTransaction.CommitAsync(ct);
                        await ResetIsolationLevelAsync(connection);
                    }
                }
            }
            finally {
                DbTransaction = null;
            }
        }

        /// <summary>
        /// Rollback transaction in database
        /// </summary>
        public void Rollback() {

            if(DbTransaction == null) {
                return;
            }
            try {

                using(DbConnection? connection = DbTransaction.Connection) {

                    using(DbTransaction) {

                        if(connection != null) {

                            bool rollback = connection.State != ConnectionState.Closed &&
                                            connection.State != ConnectionState.Broken;

                            if(rollback) {
                                DbTransaction.Rollback();
                            }
                            ResetIsolationLevel(connection);
                        }
                    }
                }
            }
            finally {
                DbTransaction = null;
            }
        }

        /// <summary>
        /// Rollback transaction in database
        /// </summary>
        public async Task RollbackAsync(CancellationToken ct = default) {

            if(DbTransaction == null) {
                return;
            }
            try {
                await using(DbConnection? connection = DbTransaction.Connection) {

                    await using(DbTransaction) {

                        if(connection != null) {

                            bool rollback = connection.State != ConnectionState.Closed &&
                                            connection.State != ConnectionState.Broken;

                            if(rollback) {
                                await DbTransaction.RollbackAsync(ct);
                            }
                            await ResetIsolationLevelAsync(connection);

                        }
                    }
                }
            }
            finally {
                DbTransaction = null;
            }
        }

        private static void ResetIsolationLevel(DbConnection? connection) {
            //
            //  For Sql Server we need to reset the isolation level
            //
            if(connection is SqlConnection) {

                using(DbCommand command = connection.CreateCommand()) {
                    command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
                    command.ExecuteNonQuery();
                }
            }
        }
        private static async Task ResetIsolationLevelAsync(DbConnection? connection) {
            //
            //  For Sql Server we need to reset the isolation level
            //
            if(connection is SqlConnection) {

                using(DbCommand command = connection.CreateCommand()) {
                    command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}