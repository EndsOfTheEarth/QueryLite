﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    /// <summary>
    /// Database transaction
    /// </summary>
    public sealed class Transaction : IDisposable {

        /// <summary>
        /// Identifier for transaction. Used for debugging.
        /// </summary>
        public ulong TransactionId { get; }

        private bool mCommitted = false;
        private bool mRolledBack = false;
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
        private DbConnection? DbConnection { get; set; }

        internal DbTransaction? GetTransaction(IDatabase database) {

            if(Database != database) {
                throw new Exception("Transaction is being used to query two different connections");
            }
            return DbTransaction;
        }

        internal void SetTransaction(DbConnection dbConnection, DbTransaction dbTransaction) {

            if(DbConnection != null) {
                throw new Exception($"Cannot set {nameof(DbConnection)} twice");
            }
            if(DbTransaction != null) {
                throw new Exception($"Cannot set {nameof(DbTransaction)} twice");
            }
            DbConnection = dbConnection;
            DbTransaction = dbTransaction;
        }

        /// <summary>
        /// Counter used to give each transaction a unique id mainly for debugging purposes
        /// </summary>
        private static ulong sCounter = 0;

        private static ulong GetNextId() {

            if(sCounter == ulong.MaxValue) {  //Should never come close to reaching this
                sCounter = ulong.MinValue;
            }
            return sCounter++;  //In theory some transactions could get the same id but this is better than using lock() while potentially being called on async threads
        }

        /// <summary>
        /// Dispose transation. Will roll back transaction if it is not yet committed.
        /// </summary>
        public void Dispose() {

            if(DbTransaction != null) {

                if(!mCommitted && !mRolledBack) {
                    Rollback();
                }
            }
            DbConnection?.Dispose();
        }

        /// <summary>
        /// Commit transaction in database
        /// </summary>
        public void Commit() {

            if(DbTransaction != null) {

                using(DbTransaction) {

                    DbTransaction.Commit();

                    if(DbTransaction.Connection != null) {
                        using DbConnection connection = DbTransaction.Connection;
                        ResetIsolationLevel(connection);
                    }
                }
            }
            if(DbConnection != null) {

                using(DbConnection) {
                    DbConnection.Close();
                }
            }
            mCommitted = true;
        }

        /// <summary>
        /// Commit transaction in database
        /// </summary>
        public async Task CommitAsync(CancellationToken cancellationToken = default) {

            if(DbTransaction != null) {

                using(DbTransaction) {

                    await DbTransaction.CommitAsync(cancellationToken);

                    if(DbTransaction.Connection != null) {
                        using DbConnection connection = DbTransaction.Connection;
                        ResetIsolationLevel(connection);
                    }
                }
            }
            if(DbConnection != null) {

                using(DbConnection) {
                    await DbConnection.CloseAsync();
                }
            }
            mCommitted = true;
        }

        /// <summary>
        /// Rollback transaction in database
        /// </summary>
        public void Rollback() {

            if(DbTransaction != null) {

                using(DbTransaction) {

                    if(DbTransaction.Connection != null) {

                        using DbConnection connection = DbTransaction.Connection;

                        if(DbTransaction.Connection.State != ConnectionState.Closed && DbTransaction.Connection.State != ConnectionState.Broken) {
                            DbTransaction.Rollback();
                        }
                        ResetIsolationLevel(connection);
                    }
                }
                DbTransaction = null;
            }
            if(DbConnection != null) {

                using(DbConnection) {
                    DbConnection.Close();
                }
                DbConnection = null;
            }
            mRolledBack = true;
        }

        /// <summary>
        /// Rollback transaction in database
        /// </summary>
        public async Task RollbackAsync(CancellationToken cancellationToken = default) {

            if(DbTransaction != null) {

                using(DbTransaction) {

                    if(DbTransaction.Connection != null) {

                        using DbConnection connection = DbTransaction.Connection;

                        if(DbTransaction.Connection.State != ConnectionState.Closed && DbTransaction.Connection.State != ConnectionState.Broken) {
                            await DbTransaction.RollbackAsync(cancellationToken);
                        }
                        ResetIsolationLevel(connection);
                    }
                }
                DbTransaction = null;
            }
            if(DbConnection != null) {

                using(DbConnection) {
                    await DbConnection.CloseAsync();
                }
                DbConnection = null;
            }
            mRolledBack = true;
        }

        private static void ResetIsolationLevel(DbConnection connection) {
            //
            //  For sql server we need to reset the isolation level
            //
            if(connection is SqlConnection) {

                using(DbCommand command = connection.CreateCommand()) {
                    command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}