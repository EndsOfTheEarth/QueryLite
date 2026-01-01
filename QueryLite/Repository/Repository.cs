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
using QueryLite.Repository;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace QueryLite {

    public interface IRow<TABLE, ROW> where TABLE : ATable where ROW : class, IEquatable<ROW> {

        abstract static ROW CloneRow(ROW row);
        abstract static List<GetSetMap<ROW>> GetColumnMap(TABLE table);
        abstract static ROW LoadRow(TABLE table, IResultRow resultRow);
    }

    public sealed class GetSetMap<ROW> {

        public GetSetMap(IColumn column, Func<ROW, object?> get, Action<ROW, IResultRow> set) {
            Column = column;
            Get = get;
            Set = set;
        }
        public QueryLite.IColumn Column { get; }
        public Func<ROW, object?> Get { get; }
        public Action<ROW, QueryLite.IResultRow> Set { get; }
    }

    public interface IRepository<TABLE, ROW> where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        /// <summary>
        /// Returns number of rows in repository.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Returns row at index.
        /// </summary>
        ROW this[int index] { get; }

        /// <summary>
        /// Returns the row state for given row.
        /// </summary>
        /// <exception cref="Exception">Throws an exception when row does not exist in repository.</exception>
        RowUpdateState GetRowState(ROW row);

        /// <summary>
        /// Returns the row state for given row.
        /// </summary>
        bool TryGetRowState(ROW row, [MaybeNullWhen(false)] out RowUpdateState? state);

        /// <summary>
        /// Select rows from database.
        /// </summary>
        IRepositoryWith<TABLE, ROW> SelectRows { get; }

        /// <summary>
        /// Populate repository with rows that already exist in the database and are unchanged.
        /// </summary>
        void PopulateWithExistingRows(IEnumerable<ROW> rows);

        /// <summary>
        /// Adds row to repository to be inserted when UpdateAsync(...) or PersistInsertsOnlyAsync(...) is called.
        /// </summary>
        ROW AddNewRow(ROW row);

        /// <summary>
        /// Flags row to be deleted when repository is updated.
        /// </summary>
        void DeleteRow(ROW row);

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// Deleted row objects are removed from the repository.
        /// </summary>
        int SaveChanges(Transaction transaction);

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// Deleted row objects are removed from the repository.
        /// </summary>
        int SaveChanges(Transaction transaction, QueryTimeout? timeout);

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// Deleted row objects are removed from the repository.
        /// </summary>
        int SaveChanges(Transaction transaction, QueryTimeout? timeout, string debugName);

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// Deleted row objects are removed from the repository.
        /// </summary>
        Task<int> SaveChangesAsync(Transaction transaction, CancellationToken ct);

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// Deleted row objects are removed from the repository.
        /// </summary>
        Task<int> SaveChangesAsync(Transaction transaction, QueryTimeout? timeout, CancellationToken ct);

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// Deleted row objects are removed from the repository.
        /// </summary>
        Task<int> SaveChangesAsync(Transaction transaction, QueryTimeout? timeout, string debugName, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistInsertsOnly(Transaction transaction);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistInsertsOnly(Transaction transaction, QueryTimeout timeout);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistInsertsOnly(Transaction transaction, QueryTimeout timeout, string debugName);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistInsertsOnlyAsync(Transaction transaction, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistUpdatesOnly(Transaction transaction);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistUpdatesOnly(Transaction transaction, QueryTimeout timeout);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistUpdatesOnly(Transaction transaction, QueryTimeout timeout, string debugName);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistUpdatesOnlyAsync(Transaction transaction, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistDeletesOnly(Transaction transaction);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistDeletesOnly(Transaction transaction, QueryTimeout timeout);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        int PersistDeletesOnly(Transaction transaction, QueryTimeout timeout, string debugName);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistDeletesOnlyAsync(Transaction transaction, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct);
    }

    /// <summary>
    /// When updating or deleting a row, what matching method be used to find the record in a table.
    /// </summary>
    public enum MatchOn {

        /// <summary>
        /// Matching on primary key is the most efficient option, but it allows updating of records
        /// that have changed in the database since loading the row.
        /// </summary> 
        PrimaryKey,

        /// <summary>
        /// Match on all columns is less efficient (produces a larger sql query), and when a record
        /// has changed in the database since loading, an exception will be thrown as the record was
        /// not matched / found.
        /// </summary>
        AllColumns
    }

    public abstract partial class ARepository<TABLE, ROW> where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        public TABLE Table { get; }

        private MatchOn MatchOn { get; }

        /*
         *  Note: We use RefCompare<ROW> to compare the ROW by reference rather than equality.
         *  Otherwise, if a property value on a ROW instance changes, the ROW cannot be found
         *  in the Dictionary. 
         */
        private Dictionary<RefCompare<ROW>, RowState> StateLookup { get; set; } = [];
        private List<RowState> Rows { get; set; } = [];

        /// <summary>
        /// ARepository constructor.
        /// </summary>
        /// <param name="matchOn">Forces row updates and deletes to compare all column values and fail if they are different or the record is missing.</param>
        protected ARepository(TABLE table, MatchOn matchOn) {

            if(matchOn != MatchOn.PrimaryKey && matchOn != MatchOn.AllColumns) {
                throw new ArgumentException($"Invalid {nameof(matchOn)} value. Must be either {MatchOn.PrimaryKey} or {MatchOn.AllColumns}");
            }
            Table = table;
            MatchOn = matchOn;
        }

        private static RowUpdater<TABLE, ROW> GetOrBuildRowUpdater(IDatabase database, TABLE table, MatchOn matchOn) {

            RowUpdater<TABLE, ROW>? updaters = GetUpdater(database.DatabaseType);

            if(updaters != null) {
                return updaters;
            }

            List<ColumnAndSetter<ROW>> insertAndUpdateColumnAndSetters = GetInsertAndUpdateColumnsAndSettersMap(table);

            List<ColumnAndSetter<ROW>> whereColumnAndSetters = GetWhereClauseColumnsAndSettersMap(table, matchOn);

            updaters = new RowUpdater<TABLE, ROW>(table, insertAndUpdateColumnAndSetters, insertAndUpdateColumnAndSetters, whereColumnAndSetters, database);

            SetUpdater(database.DatabaseType, updaters);
            return updaters;
        }
#if NET9_0_OR_GREATER
        private static readonly Lock Lock = new Lock();
#else
        private static readonly object Lock = new object();
#endif

        private static Dictionary<Type, IRowUpdaterCollection> RowUpdaterCache { get; } = [];

        private static RowUpdater<TABLE, ROW>? GetUpdater(DatabaseType databaseType) {

            Type key = typeof(ROW);

            IRowUpdaterCollection? collection;

            lock(Lock) {

                if(!RowUpdaterCache.TryGetValue(key, out collection)) {
                    collection = new RowUpdaterCollection<TABLE, ROW>();
                    RowUpdaterCache.Add(key, collection);
                }
            }
            return ((RowUpdaterCollection<TABLE, ROW>)collection).GetUpdater(databaseType);
        }

        private static void SetUpdater(DatabaseType databaseType, RowUpdater<TABLE, ROW> rowUpdaters) {

            Type key = typeof(ROW);

            IRowUpdaterCollection? collection;

            lock(Lock) {

                if(!RowUpdaterCache.TryGetValue(key, out collection)) {
                    collection = new RowUpdaterCollection<TABLE, ROW>();
                    RowUpdaterCache.Add(key, collection);
                }
            }
            ((RowUpdaterCollection<TABLE, ROW>)collection).SetUpdater(databaseType, rowUpdaters);
        }

        public IEnumerator<ROW> GetEnumerator() {

            for(int index = 0; index < Count; index++) {
                ROW row = this[index];
                yield return row;
            }
        }

        /// <summary>
        /// Number of rows in repository
        /// </summary>
        public int Count => Rows.Count;

        /// <summary>
        /// Get row at index.
        /// </summary>
        public ROW this[int index] => Rows[index].NewRow;

        public RowUpdateState GetRowState(ROW row) {

            if(!StateLookup.TryGetValue(new RefCompare<ROW>(row), out RowState? rowState)) {
                throw new Exception("Row does not exist in data table");
            }
            return rowState.State;
        }

        /// <summary>
        /// Returns true if changes to the row require the records to be inserted, updated or deleted.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool RequiresUpdate(ROW row) {

            if(!StateLookup.TryGetValue(new RefCompare<ROW>(row), out RowState? rowState)) {
                throw new Exception("Row does not exist in data table");
            }
            return rowState.State switch {
                RowUpdateState.PendingAdd or RowUpdateState.PendingDelete => true,
                RowUpdateState.Existing => rowState.OldRow != null && !rowState.OldRow.Equals(rowState.NewRow),
                _ => false,
            };
        }

        public bool TryGetRowState(ROW row, [MaybeNullWhen(false)] out RowUpdateState? state) {

            state = null;

            if(StateLookup.TryGetValue(new RefCompare<ROW>(row), out RowState? rowState)) {
                state = rowState.State;
                return true;
            }
            return false;
        }

        protected static List<ColumnAndSetter<ROW>> CreateColumnsAndSettersMap(TABLE table) {
            return [.. ROW.GetColumnMap(table).Select(cm => new ColumnAndSetter<ROW>(cm.Column, cm.Get, cm.Set))];
        }

        private static List<ColumnAndSetter<ROW>> GetInsertAndUpdateColumnsAndSettersMap(TABLE table) {
            List<ColumnAndSetter<ROW>> list = CreateColumnsAndSettersMap(table);
            return [.. list.Where(cs => !cs.Column.IsAutoGenerated)];
        }

        private static List<ColumnAndSetter<ROW>> GetWhereClauseColumnsAndSettersMap(TABLE table, MatchOn matchOn) {

            List<ColumnAndSetter<ROW>> list = CreateColumnsAndSettersMap(table);  //Get second set of parameter creators with different parameter names for use in the where clause

            if(matchOn != MatchOn.PrimaryKey || table.PrimaryKey == null) {
                return list;
            }

            Dictionary<string, ColumnAndSetter<ROW>> lookup = list.ToDictionary(cs => cs.Column.ColumnName);

            return [.. table.PrimaryKey.Columns.Select(pkColumn => lookup[pkColumn.ColumnName])];
        }

        public IRepositoryWith<TABLE, ROW> SelectRows {
            get { return new RepositoryQueryTemplate<TABLE, ROW>(this, Table); }
        }

        /// <summary>
        /// Throws an exception if the number of rows in the Repository does not equal rows.
        /// </summary>
        public void AssertRowCount(int rows) {

            if(rows < 0) {
                throw new ArgumentException($"{nameof(rows)} must be a positive number. {nameof(rows)} == {rows}");
            }

            if(Rows.Count != rows) {
                throw new Exception($"Expected {rows} row{(rows != 1 ? "s" : "")} but {Rows.Count} rows were returned.");
            }
        }

        internal void ClearRows() {
            StateLookup.Clear();
            Rows.Clear();
        }

        public void PopulateWithExistingRows(IEnumerable<ROW> rows) {

            foreach(ROW row in rows) {
                RowState rowState = new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(row), newRow: row);
                StateLookup.Add(new RefCompare<ROW>(row), rowState);
                Rows.Add(rowState);
            }
        }

        public ROW AddNewRow(ROW row) {
            RowState rowState = new RowState(state: RowUpdateState.PendingAdd, oldRow: null, newRow: row);
            StateLookup.Add(new RefCompare<ROW>(row), rowState);
            Rows.Add(rowState);
            return row;
        }

        /// <summary>
        /// Flags row to be deleted when repository is updated.
        /// </summary>
        public void DeleteRow(ROW row) {

            if(!StateLookup.TryGetValue(new RefCompare<ROW>(row), out RowState? rowState)) {
                throw new Exception("Row does not exist in data table");
            }

            if(rowState.State == RowUpdateState.Existing) {
                rowState.State = RowUpdateState.PendingDelete;
            }
            else if(rowState.State == RowUpdateState.PendingAdd) {
                rowState.State = RowUpdateState.Deleted;
            }
        }

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public int SaveChanges(Transaction transaction) {
            return SaveChanges(transaction, timeout: null);
        }

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public int SaveChanges(Transaction transaction, QueryTimeout? timeout) {
            return SaveChanges(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public int SaveChanges(Transaction transaction, QueryTimeout? timeout, string debugName) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    totalRowsEffected += PerformInsert(transaction, timeout, newState, updater, row, debugName: debugName);
                }
                else if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        totalRowsEffected += PerformUpdate(transaction, timeout, newState, updater, row, debugName: debugName);
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
                else if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }
                    totalRowsEffected += PerformDelete(transaction, timeout, updater, row, debugName: debugName);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public async Task<int> SaveChangesAsync(Transaction transaction, CancellationToken ct) {
            return await SaveChangesAsync(transaction, timeout: null, ct);
        }

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public async Task<int> SaveChangesAsync(Transaction transaction, QueryTimeout? timeout, CancellationToken ct) {
            return await SaveChangesAsync(transaction, timeout, debugName: "", ct);
        }
        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public async Task<int> SaveChangesAsync(Transaction transaction, QueryTimeout? timeout, string debugName, CancellationToken ct) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    totalRowsEffected += await PerformInsertAsync(transaction, timeout, newState, updater, row, debugName: debugName, ct);
                }
                else if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        totalRowsEffected += await PerformUpdateAsync(transaction, timeout, newState, updater, row, debugName: debugName, ct);
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
                else if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }
                    totalRowsEffected += await PerformDeleteAsync(transaction, timeout, updater, row, debugName: debugName, ct);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        private static int PerformInsert(Transaction transaction, QueryTimeout? timeout, List<RowState> newState,
                                                    RowUpdater<TABLE, ROW> updater, RowState rowState, string debugName) {

            int rowsEffected = updater.Insert(rowState.NewRow, transaction, timeout, debugName);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            newState.Add(new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(rowState.NewRow), newRow: rowState.NewRow));
            return rowsEffected;
        }

        private static async Task<int> PerformInsertAsync(Transaction transaction, QueryTimeout? timeout, List<RowState> newState,
                                                    RowUpdater<TABLE, ROW> updater,
                                                    RowState rowState, string debugName, CancellationToken ct) {

            int rowsEffected = await updater.InsertAsync(rowState.NewRow, transaction, timeout, debugName, ct);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            newState.Add(new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(rowState.NewRow), newRow: rowState.NewRow));
            return rowsEffected;
        }

        private static int PerformUpdate(Transaction transaction, QueryTimeout? timeout,
                                         List<RowState> newState, RowUpdater<TABLE, ROW> updater, RowState row, string debugName) {

            int rowsEffected = updater.Update(oldRow: row.OldRow!, newRow: row.NewRow, transaction, timeout, debugName: debugName);

            if(rowsEffected == 0) {
                throw new Exception("Concurrency violation. Maybe row was changed in the database?");
            }
            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            newState.Add(new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(row.NewRow), newRow: row.NewRow));
            return rowsEffected;
        }

        private static async Task<int> PerformUpdateAsync(Transaction transaction, QueryTimeout? timeout,
                                                  List<RowState> newState, RowUpdater<TABLE, ROW> updater,
                                                  RowState row, string debugName, CancellationToken ct) {

            int rowsEffected = await updater.UpdateAsync(oldRow: row.OldRow!, newRow: row.NewRow, transaction, timeout, debugName: debugName, ct);

            if(rowsEffected == 0) {
                throw new Exception("Concurrency violation. Maybe row was changed in the database?");
            }
            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            newState.Add(new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(row.NewRow), newRow: row.NewRow));
            return rowsEffected;
        }

        private static int PerformDelete(Transaction transaction, QueryTimeout? timeout, RowUpdater<TABLE, ROW> updater, RowState row, string debugName) {

            int rowsEffected = updater.Delete(existingRow: row.OldRow!, transaction, timeout, debugName: debugName);

            if(rowsEffected != 1) {
                throw new Exception($"Delete failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            return rowsEffected;
        }

        private static async Task<int> PerformDeleteAsync(Transaction transaction, QueryTimeout? timeout,
                                                          RowUpdater<TABLE, ROW> updater, RowState row, string debugName,
                                                          CancellationToken ct) {

            int rowsEffected = await updater.DeleteAsync(existingRow: row.OldRow!, transaction, timeout, debugName: debugName, ct);

            if(rowsEffected != 1) {
                throw new Exception($"Delete failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            return rowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistInsertsOnly(Transaction transaction) {
            return PersistInsertsOnly(transaction, timeout: TimeoutLevel.ShortInsert);
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistInsertsOnly(Transaction transaction, QueryTimeout timeout) {
            return PersistInsertsOnly(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistInsertsOnly(Transaction transaction, QueryTimeout timeout, string debugName) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    totalRowsEffected += PerformInsert(transaction, timeout, newState, updater, row, debugName: debugName);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, CancellationToken ct) {
            return await PersistInsertsOnlyAsync(transaction, timeout: TimeoutLevel.ShortInsert, ct);
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct) {
            return await PersistInsertsOnlyAsync(transaction, timeout, debugName: "", ct);
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    totalRowsEffected += await PerformInsertAsync(transaction, timeout, newState, updater, row, debugName: debugName, ct);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistUpdatesOnly(Transaction transaction) {
            return PersistUpdatesOnly(transaction, timeout: TimeoutLevel.ShortUpdate);
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistUpdatesOnly(Transaction transaction, QueryTimeout timeout) {
            return PersistUpdatesOnly(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistUpdatesOnly(Transaction transaction, QueryTimeout timeout, string debugName) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        totalRowsEffected += PerformUpdate(transaction, timeout, newState, updater, row, debugName: debugName);
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, CancellationToken ct) {
            return await PersistUpdatesOnlyAsync(transaction, timeout: TimeoutLevel.ShortUpdate, ct);
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct) {
            return await PersistUpdatesOnlyAsync(transaction, timeout, debugName: "", ct);
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        totalRowsEffected += await PerformUpdateAsync(transaction, timeout, newState, updater, row, debugName: debugName, ct);
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }


        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistDeletesOnly(Transaction transaction) {
            return PersistDeletesOnly(transaction, timeout: TimeoutLevel.ShortDelete);
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistDeletesOnly(Transaction transaction, QueryTimeout timeout) {
            return PersistDeletesOnly(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistDeletesOnly(Transaction transaction, QueryTimeout timeout, string debugName) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }
                    totalRowsEffected += PerformDelete(transaction, timeout, updater, row, debugName: debugName);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, CancellationToken ct) {
            return await PersistDeletesOnlyAsync(transaction, timeout: TimeoutLevel.ShortDelete, ct);
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct) {
            return await PersistDeletesOnlyAsync(transaction, timeout, debugName: "", ct);
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }
                    totalRowsEffected += await PerformDeleteAsync(transaction, timeout, updater, row, debugName: debugName, ct);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        public static void ExecuteInsert(ROW row, TABLE table, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Insert(row, transaction, timeout: null, debugName: "");

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static void ExecuteInsert(ROW row, TABLE table, QueryTimeout? timeout, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Insert(row, transaction, timeout, debugName: "");

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static void ExecuteInsert(ROW row, TABLE table, QueryTimeout? timeout, string debugName, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Insert(row, transaction, timeout, debugName);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static async Task ExecuteInsertAsync(ROW row, TABLE table, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.InsertAsync(row, transaction, timeout: null, debugName: "", ct);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static async Task ExecuteInsertAsync(ROW row, TABLE table, QueryTimeout? timeout, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.InsertAsync(row, transaction, timeout, debugName: "", ct);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }
        public static async Task ExecuteInsertAsync(ROW row, TABLE table, QueryTimeout? timeout, string debugName, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.InsertAsync(row, transaction, timeout, debugName, ct);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static void ExecuteUpdate(ROW row, TABLE table, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Update(row, row, transaction, timeout: null, debugName: "");

            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static void ExecuteUpdate(ROW row, TABLE table, QueryTimeout? timeout, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Update(row, row, transaction, timeout, debugName: "");

            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static void ExecuteUpdate(ROW row, TABLE table, QueryTimeout? timeout, string debugName, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Update(row, row, transaction, timeout, debugName);

            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static async Task ExecuteUpdateAsync(ROW row, TABLE table, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.UpdateAsync(row, row, transaction, timeout: null, debugName: "", ct);

            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        public static async Task ExecuteUpdateAsync(ROW row, TABLE table, QueryTimeout? timeout, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.UpdateAsync(row, row, transaction, timeout, debugName: "", ct);

            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }
        public static async Task ExecuteUpdateAsync(ROW row, TABLE table, QueryTimeout? timeout, string debugName, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.UpdateAsync(row, row, transaction, timeout, debugName, ct);

            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
        }

        private class RowState {

            public RowState(RowUpdateState state, ROW? oldRow, ROW newRow) {
                State = state;
                OldRow = oldRow;
                NewRow = newRow;
            }
            public RowUpdateState State { get; set; }
            public ROW? OldRow { get; set; }
            public ROW NewRow { get; set; }
        }
    }

    public enum RowUpdateState {
        PendingAdd,
        PendingDelete,
        Existing,
        Deleted
    }

    public class ColumnAndSetter<ROW> {

        public ColumnAndSetter(IColumn column, Func<ROW, object?> getter, Action<ROW, IResultRow> setter) {
            Column = column;
            Getter = getter;
            Setter = setter;
        }
        public string? ParameterName { get; internal set; }
        public IColumn Column { get; }
        public Func<ROW, object?> Getter { get; }
        public Action<ROW, QueryLite.IResultRow> Setter { get; }
    }

    /// <summary>
    /// RefCompare is a struct that compares records by instance rather than equality.
    /// </summary>
    public readonly struct RefCompare<TYPE> where TYPE : class {

        private TYPE Row { get; }

        public RefCompare(TYPE row) {
            Row = row;
        }
        public override bool Equals([NotNullWhen(true)] object? obj) {

            if(obj is RefCompare<TYPE> refCompare) {
                return object.ReferenceEquals(Row, refCompare.Row);
            }
            return false;
        }
        public override int GetHashCode() {
            return RuntimeHelpers.GetHashCode(Row);
        }
        public static bool operator ==(RefCompare<TYPE> left, RefCompare<TYPE> right) {
            return left.Equals(right);
        }
        public static bool operator !=(RefCompare<TYPE> left, RefCompare<TYPE> right) {
            return !(left == right);
        }
    }
}