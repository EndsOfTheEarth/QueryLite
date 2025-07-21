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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace QueryLite {

    public interface IRow<TABLE, ROW> where TABLE : ATable where ROW : class, IEquatable<ROW> {

        abstract static ROW CloneRow(ROW row);

        abstract static List<(IColumn, Func<ROW, object?>)> GetColumnMap(TABLE table);
        abstract static ROW LoadRow(TABLE table, IResultRow resultRow);
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
        IDataTableWhere<TABLE, ROW> SelectRows { get; }

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
        Task<int> UpdateAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken);
    }

    public abstract class ARepository<TABLE, ROW> where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        public TABLE Table { get; }

        private bool ConcurrencyCheck { get; }

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
        /// <param name="concurrencyCheck">Forces row updates and deletes to compare all column values and fail if they are different or the record is missing.</param>
        protected ARepository(TABLE table, bool concurrencyCheck) {
            Table = table;
            ConcurrencyCheck = concurrencyCheck;
        }

        private RowUpdater<TABLE, ROW> GetOrBuildRowUpdater(IDatabase database) {

            RowUpdater<TABLE, ROW>? updaters = GetUpdater(database.DatabaseType);

            if(updaters != null) {
                return updaters;
            }

            List<ColumnAndSetter<ROW>> insertAndUpdateColumnAndSetters = GetInsertAndUpdateColumnsAndSettersMap();

            List<ColumnAndSetter<ROW>> whereColumnAndSetters = GetWhereClauseColumnsAndSettersMap();

            updaters = new RowUpdater<TABLE, ROW>(Table, insertAndUpdateColumnAndSetters, insertAndUpdateColumnAndSetters, whereColumnAndSetters, database);

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

        protected List<ColumnAndSetter<ROW>> CreateColumnsAndSettersMap(TABLE table) {
            return [.. ROW.GetColumnMap(table).Select(cm => new ColumnAndSetter<ROW>(cm.Item1, cm.Item2))];
        }

        private List<ColumnAndSetter<ROW>> GetInsertAndUpdateColumnsAndSettersMap() {
            List<ColumnAndSetter<ROW>> list = CreateColumnsAndSettersMap(Table);
            return [.. list.Where(cs => !cs.Column.IsAutoGenerated)];
        }

        private List<ColumnAndSetter<ROW>> GetWhereClauseColumnsAndSettersMap() {

            List<ColumnAndSetter<ROW>> list = CreateColumnsAndSettersMap(Table);  //Get second set of parameter creators with different parameter names for use in the where clause

            if(ConcurrencyCheck || Table.PrimaryKey == null) {
                return list;
            }

            Dictionary<string, ColumnAndSetter<ROW>> lookup = list.ToDictionary(cs => cs.Column.ColumnName);

            return [.. Table.PrimaryKey.Columns.Select(pkColumn => lookup[pkColumn.ColumnName])];
        }

        public IDataTableWhere<TABLE, ROW> SelectRows {
            get { return new DataTableQueryTemplate(this, Table); }
        }

        protected void ClearRows() {
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
        public async Task<int> UpdateAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    totalRowsEffected += await PerformInsertAsync(transaction, timeout, newState, updater, row, cancellationToken);
                }
                else if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        totalRowsEffected += await PerformUpdateAsync(transaction, timeout, newState, updater, row, cancellationToken);
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
                else if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }
                    totalRowsEffected += await PerformDeleteAsync(transaction, timeout, updater, row, cancellationToken);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        private static async Task<int> PerformInsertAsync(Transaction transaction, QueryTimeout timeout, List<RowState> newState,
                                                    RowUpdater<TABLE, ROW> updater,
                                                    RowState rowState, CancellationToken cancellationToken) {

            int rowsEffected = await updater.InsertAsync(rowState.NewRow, transaction, timeout, cancellationToken);

            if(rowsEffected != 1) {
                throw new Exception($"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            newState.Add(new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(rowState.NewRow), newRow: rowState.NewRow));
            return rowsEffected;
        }


        private static async Task<int> PerformUpdateAsync(Transaction transaction, QueryTimeout timeout,
                                                  List<RowState> newState, RowUpdater<TABLE, ROW> updater,
                                                  RowState row, CancellationToken cancellationToken) {

            int rowsEffected = await updater.UpdateAsync(oldRow: row.OldRow!, newRow: row.NewRow, transaction, timeout, cancellationToken);

            if(rowsEffected == 0) {
                throw new Exception("Concurrency violation. Maybe row was changed in the database?");
            }
            if(rowsEffected != 1) {
                throw new Exception($"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            newState.Add(new RowState(state: RowUpdateState.Existing, oldRow: ROW.CloneRow(row.NewRow), newRow: row.NewRow));
            return rowsEffected;
        }

        private static async Task<int> PerformDeleteAsync(Transaction transaction, QueryTimeout timeout,
                                                          RowUpdater<TABLE, ROW> updater, RowState row,
                                                          CancellationToken cancellationToken) {

            int rowsEffected = await updater.DeleteAsync(existingRow: row.OldRow!, transaction, timeout, cancellationToken);

            if(rowsEffected != 1) {
                throw new Exception($"Delete failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}.");
            }
            return rowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    totalRowsEffected += await PerformInsertAsync(transaction, timeout, newState, updater, row, cancellationToken);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        totalRowsEffected += await PerformUpdateAsync(transaction, timeout, newState, updater, row, cancellationToken);
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
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken) {

            List<RowState> newState = new List<RowState>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database);

            int totalRowsEffected = 0;

            foreach(RowState row in Rows) {

                if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception($"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug.");
                    }
                    totalRowsEffected += await PerformDeleteAsync(transaction, timeout, updater, row, cancellationToken);
                }
            }
            StateLookup = newState.ToDictionary(rowState => new RefCompare<ROW>(rowState.NewRow));
            Rows = newState;
            return totalRowsEffected;
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

        protected class DataTableQueryTemplate : IDataTableWhere<TABLE, ROW>, IDataTableOrderBy<TABLE, ROW>, IDateTableExecute<TABLE, ROW> {

            private ARepository<TABLE, ROW> Repository { get; }
            private TABLE Table { get; }

            public DataTableQueryTemplate(ARepository<TABLE, ROW> repository, TABLE table) {
                Repository = repository;
                Table = table;
            }

            public ICondition? Condition { get; private set; }
            public IOrderByColumn[]? OrderByColumns { get; private set; }

            public IDataTableOrderBy<TABLE, ROW> Where(ICondition? condition) {
                Condition = condition;
                return this;
            }

            public IDateTableExecute<TABLE, ROW> OrderBy(params IOrderByColumn[] columns) {
                OrderByColumns = columns;
                return this;
            }

            public async Task ExecuteAsync(IDatabase database, CancellationToken cancellationToken) {

                Repository.ClearRows();

                IGroupBy<ROW> q1 = Query
                    .Select(row => ROW.LoadRow(Table, row))
                    .From(Table)
                    .Where(Condition);

                IFor<ROW> q2 = OrderByColumns != null ? q1.OrderBy(OrderByColumns) : q1;

                QueryResult<ROW> result = await q2.ExecuteAsync(database, cancellationToken);

                Repository.PopulateWithExistingRows(result.Rows);
            }
        }
    }

    public enum RowUpdateState {
        PendingAdd,
        PendingDelete,
        Existing,
        Deleted
    }

    public interface IDataTableWhere<TABLE, ROW> : IDataTableOrderBy<TABLE, ROW> where TABLE : ATable where ROW : class, IEquatable<ROW> {

        IDataTableOrderBy<TABLE, ROW> Where(ICondition? condition);
    }
    public interface IDataTableOrderBy<TABLE, ROW> : IDateTableExecute<TABLE, ROW> where TABLE : ATable where ROW : class, IEquatable<ROW> {

        IDateTableExecute<TABLE, ROW> OrderBy(params IOrderByColumn[] columns);
    }
    public interface IDateTableExecute<TABLE, ROW> where TABLE : ATable where ROW : class, IEquatable<ROW> {

        Task ExecuteAsync(IDatabase database, CancellationToken cancellationToken);
    }

    public class ColumnAndSetter<ROW> {

        public ColumnAndSetter(IColumn column, Func<ROW, object?> setter) {
            Column = column;
            Setter = setter;
        }
        public string? ParameterName { get; internal set; }
        public IColumn Column { get; }
        public Func<ROW, object?> Setter { get; }
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