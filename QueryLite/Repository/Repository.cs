/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace QueryLite {

    public interface IRow<TABLE, ROW> where TABLE : ATable where ROW : class, IEquatable<ROW> {

        [EditorBrowsable(EditorBrowsableState.Never)]
        int InternalIndex { get; set; }

        abstract static ROW CloneRow(ROW row);
        abstract static List<GetSetMap<ROW>> GetColumnMap(TABLE table);
        abstract static ROW LoadRow(TABLE table, IResultRow resultRow);
    }

    public sealed class GetSetMap<ROW>(IColumn column, Func<ROW, object?> get,
                                       Action<ROW, IResultRow> set,
                                       Func<ROW, ROW, bool> valuesEqual) {

        public IColumn Column { get; } = column;
        public Func<ROW, object?> Get { get; } = get;
        public Action<ROW, IResultRow> Set { get; } = set;

        /// <summary>
        /// Returns true if the column in row A has the same value in row B.
        /// </summary>
        public Func<ROW, ROW, bool> ValuesEqual { get; } = valuesEqual;
    }

    public static class Repositories {

        internal static volatile List<RepositorySavingChangesInterceptor> SaveInterceptors = [];

        public static void ClearInterceptors() {
            SaveInterceptors = [];
        }
        public static void AddInterceptor(RepositorySavingChangesInterceptor interceptor) {

            List<RepositorySavingChangesInterceptor> interceptors = [.. SaveInterceptors];  //Work on a copy

            if(!interceptors.Contains(interceptor)) {
                interceptors.Add(interceptor);
            }
            SaveInterceptors = interceptors;
        }
        public static void RemoveInterceptor(RepositorySavingChangesInterceptor interceptor) {

            List<RepositorySavingChangesInterceptor> interceptors = [.. SaveInterceptors];  //Work on a copy
            interceptors.Remove(interceptor);
            SaveInterceptors = interceptors;
        }
    }

    public abstract partial class ARepository<TABLE, ROW> where TABLE : ATable
                                                          where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        public TABLE Table { get; }

        private MatchOn MatchOn { get; }

        /*
         *  Note: We use RefCompare<ROW> to compare the ROW by reference rather than equality.
         *  Otherwise, if a property value on a ROW instance changes, the ROW cannot be found
         *  in the Dictionary. 
         */
        //private Dictionary<RefCompare<ROW>, RowState> StateLookup { get; set; } = [];
        private IList<RowState<ROW>> Rows { get; set; } = [];

        /// <summary>
        /// ARepository constructor.
        /// </summary>
        /// <param name="matchOn">Forces row updates and deletes to compare all column values and
        /// fail if they are different or the record is missing.</param>
        protected ARepository(TABLE table, MatchOn matchOn) {

            if(matchOn != MatchOn.PrimaryKey && matchOn != MatchOn.AllColumns) {
                throw new ArgumentException(
                    $"Invalid {nameof(matchOn)} value. Must be either {MatchOn.PrimaryKey} or {MatchOn.AllColumns}"
                );
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

            updaters = new RowUpdater<TABLE, ROW>(
                table: table,
                insertColumnAndSetters: insertAndUpdateColumnAndSetters,
                updateColumnAndSetters: insertAndUpdateColumnAndSetters,
                whereColumns: whereColumnAndSetters,
                database: database
            );
            SetUpdater(database.DatabaseType, updaters);
            return updaters;
        }

        private static readonly Lock Lock = new();

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
            return Rows[row.InternalIndex].State;
        }

        /// <summary>
        /// Returns true if changes to the row require the records to be inserted, updated or deleted.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool RequiresUpdate(ROW row) {

            RowState<ROW> rowState = Rows[row.InternalIndex];

            return rowState.State switch {
                RowUpdateState.PendingAdd or RowUpdateState.PendingDelete => true,
                RowUpdateState.Existing => rowState.OldRow != null &&
                                           !rowState.OldRow.Equals(rowState.NewRow),
                _ => false,
            };
        }

        public bool TryGetRowState(ROW row, [MaybeNullWhen(false)] out RowUpdateState? state) {

            state = null;

            if(row.InternalIndex < Rows.Count) {
                state = Rows[row.InternalIndex].State;
                return true;
            }
            return false;
        }

        protected static List<ColumnAndSetter<ROW>> CreateColumnsAndSettersMap(TABLE table) {

            return ROW.GetColumnMap(table)
                      .Select(
                          cm => new ColumnAndSetter<ROW>(
                              column: cm.Column,
                              getter: cm.Get,
                              setter: cm.Set,
                              valuesEqual: cm.ValuesEqual
                          )
                      ).ToList();
        }

        private static List<ColumnAndSetter<ROW>> GetInsertAndUpdateColumnsAndSettersMap(TABLE table) {
            
            List<ColumnAndSetter<ROW>> list = CreateColumnsAndSettersMap(table);

            return list.Where(cs => !cs.Column.IsAutoGenerated).ToList();
        }

        private static List<ColumnAndSetter<ROW>> GetWhereClauseColumnsAndSettersMap(TABLE table, MatchOn matchOn) {

            //Get second set of parameter creators with different parameter names for use in the where clause
            List<ColumnAndSetter<ROW>> list = CreateColumnsAndSettersMap(table);

            if(matchOn != MatchOn.PrimaryKey) {
                return list;
            }

            //Most primary key properties allocate a new object on every call.
            //So we hold it as a variable to reduce allocations.
            PrimaryKey? primaryKey = table.PrimaryKey;

            if(primaryKey == null) {
                return list;
            }

            Dictionary<string, ColumnAndSetter<ROW>> lookup = list.ToDictionary(cs => cs.Column.ColumnName);

            return primaryKey
                .Columns
                .Select(pkColumn => lookup[pkColumn.ColumnName])
                .ToList();
        }

        public IRepositoryWith SelectRows {
            get { return new RepositoryQueryTemplate<TABLE, ROW>(this, Table); }
        }

        /// <summary>
        /// Throws an exception if the number of rows in the Repository does not equal rows.
        /// </summary>
        public void AssertRowCount(int rows) {

            if(rows < 0) {
                throw new ArgumentException(
                    message: $"{nameof(rows)} must be a positive number. {nameof(rows)} == {rows}"
                );
            }

            if(Rows.Count != rows) {
                throw new Exception(
                    message: $"Expected {rows} row{(rows != 1 ? "s" : "")} but {Rows.Count} rows were returned."
                );
            }
        }

        internal void ClearRows() {
            Rows.Clear();
        }

        public void PopulateWithExistingRows(IEnumerable<ROW> rows) {
            Rows = [
                    .. Rows,
                    .. rows.Select(
                           row => new RowState<ROW>(
                               state: RowUpdateState.Existing,
                               oldRow: ROW.CloneRow(row),
                               newRow: row
                           )
                        )
                   ];
            ReindexRows(Rows);
        }

        internal void PopulateWithExistingRows(IList<RowState<ROW>> rows) {

            if(Rows.Count == 0) {
                ReindexRows(rows);
                Rows = rows;
            }
            else {
                Rows = [.. Rows, .. rows];
                ReindexRows(Rows);
            }
        }

        public ROW AddNewRow(ROW row) {
            
            row.InternalIndex = Rows.Count;
            
            Rows.Add(
                new RowState<ROW>(
                    state: RowUpdateState.PendingAdd,
                    oldRow: null,
                    newRow: row
                )
            );
            return row;
        }

        /// <summary>
        /// Flags row to be deleted when repository is updated.
        /// </summary>
        public void DeleteRow(ROW row) {

            if(row.InternalIndex >= Rows.Count) {
                throw new Exception(message: "Row does not exist in data table");
            }

            RowState<ROW> rowState = Rows[row.InternalIndex];

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
        public int SaveChanges(IDatabase database) {

            using Transaction transaction = new(database);

            int result = SaveChanges(transaction, timeout: null);

            transaction.Commit();

            return result;
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

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {

                    foreach(RepositorySavingChangesInterceptor interceptor in Repositories.SaveInterceptors) {
                        interceptor.SavingChanges(Change.Insert, oldRow: null, newRow: row.NewRow);
                    }
                    totalRowsEffected += PerformInsert(
                        transaction: transaction,
                        timeout: timeout,
                        newState: newState,
                        updater: updater,
                        rowState: row,
                        debugName: debugName
                    );
                }
                else if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row has a " +
                                     $"{nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {

                        foreach(RepositorySavingChangesInterceptor interceptor in Repositories.SaveInterceptors) {
                            interceptor.SavingChanges(Change.Update, oldRow: row.OldRow, newRow: row.NewRow);
                        }
                        //It is possible for the interceptor(s) to modify the row so that it no longer has any changes.
                        //So we check for changes a second time before saving
                        rowHasChanged = !row.OldRow.Equals(row.NewRow);

                        if(rowHasChanged) {
                            
                            totalRowsEffected += PerformUpdate(
                                transaction: transaction,
                                timeout: timeout,
                                newState: newState,
                                updater: updater,
                                row: row,
                                debugName: debugName
                            );
                        }
                    }

                    if(!rowHasChanged) {
                        newState.Add(row);  //Add unchanged row
                    }
                }
                else if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row has a " +
                                     $"{nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }
                    foreach(RepositorySavingChangesInterceptor interceptor in Repositories.SaveInterceptors) {
                        
                        interceptor.SavingChanges(
                            change: Change.Delete,
                            oldRow: row.OldRow,
                            newRow: null
                        );
                    }
                    totalRowsEffected += PerformDelete(
                        transaction: transaction,
                        timeout: timeout,
                        updater: updater,
                        row: row,
                        debugName: debugName
                    );
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist all changes to database. Inserts, updates and deletes in row order.
        /// </summary>
        public async Task<int> SaveChangesAsync(IDatabase database, CancellationToken ct) {

            using Transaction transaction = new(database);

            int result = await SaveChangesAsync(transaction, timeout: null, ct);

            await transaction.CommitAsync(ct);

            return result;
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
        public async Task<int> SaveChangesAsync(Transaction transaction, QueryTimeout? timeout,
                                                string debugName, CancellationToken ct) {

            List<RowState<ROW>> newState = new List<RowState<ROW>>(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {

                    foreach(RepositorySavingChangesInterceptor interceptor in Repositories.SaveInterceptors) {
                        
                        await interceptor.SavingChangesAsync(
                            change: Change.Insert,
                            oldRow: null,
                            newRow: row.NewRow
                        );
                    }
                    totalRowsEffected += await PerformInsertAsync(
                        transaction: transaction,
                        timeout: timeout,
                        newState: newState,
                        updater: updater,
                        rowState: row,
                        debugName: debugName,
                        ct: ct
                    );
                }
                else if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row has a " +
                                     $"{nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {

                        foreach(RepositorySavingChangesInterceptor interceptor in Repositories.SaveInterceptors) {
                            
                            await interceptor.SavingChangesAsync(
                                change: Change.Update,
                                oldRow: row.OldRow,
                                newRow: row.NewRow
                            );
                        }
                        totalRowsEffected += await PerformUpdateAsync(
                            transaction: transaction,
                            timeout: timeout,
                            newState: newState,
                            updater: updater,
                            row: row,
                            debugName: debugName,
                            ct: ct
                         );
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
                else if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row has a " +
                                     $"{nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }
                    foreach(RepositorySavingChangesInterceptor interceptor in Repositories.SaveInterceptors) {
                        
                        await interceptor.SavingChangesAsync(
                            change: Change.Delete,
                            oldRow: row.OldRow,
                            newRow: null
                        );
                    }
                    totalRowsEffected += await PerformDeleteAsync(
                        transaction: transaction,
                        timeout: timeout,
                        updater: updater,
                        row: row,
                        debugName: debugName,
                        ct: ct
                    );
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        private static int PerformInsert(Transaction transaction, QueryTimeout? timeout,
                                         List<RowState<ROW>> newState, RowUpdater<TABLE, ROW> updater,
                                         RowState<ROW> rowState, string debugName) {

            int rowsEffected = updater.Insert(
                newRow: rowState.NewRow,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName
            );

            if(rowsEffected != 1) {
                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
            newState.Add(
                item: new RowState<ROW>(
                    state: RowUpdateState.Existing,
                    oldRow: ROW.CloneRow(rowState.NewRow),
                    newRow: rowState.NewRow
                )
            );
            return rowsEffected;
        }

        private static async Task<int> PerformInsertAsync(Transaction transaction, QueryTimeout? timeout,
                                                          List<RowState<ROW>> newState, RowUpdater<TABLE, ROW> updater,
                                                          RowState<ROW> rowState, string debugName, CancellationToken ct) {

            int rowsEffected = await updater.InsertAsync(
                newRow: rowState.NewRow,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName,
                ct: ct
            );

            if(rowsEffected != 1) {
                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
            newState.Add(
                new RowState<ROW>(
                    state: RowUpdateState.Existing,
                    oldRow: ROW.CloneRow(rowState.NewRow),
                    newRow: rowState.NewRow
                )
            );
            return rowsEffected;
        }

        private int PerformUpdate(Transaction transaction, QueryTimeout? timeout, List<RowState<ROW>> newState,
                                  RowUpdater<TABLE, ROW> updater, RowState<ROW> row, string debugName) {

            int rowsEffected = updater.Update(
                oldRow: row.OldRow!,
                newRow: row.NewRow,
                table: Table,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName
            );

            if(rowsEffected == 0) {
                throw new Exception(
                    message: "Concurrency violation. Maybe row was changed in the database?"
                );
            }
            if(rowsEffected != 1) {
                throw new Exception(
                    message: $"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
            newState.Add(
                new RowState<ROW>(
                    state: RowUpdateState.Existing,
                    oldRow: ROW.CloneRow(row.NewRow),
                    newRow: row.NewRow
                )
            );
            return rowsEffected;
        }

        private async Task<int> PerformUpdateAsync(Transaction transaction, QueryTimeout? timeout,
                                                  List<RowState<ROW>> newState, RowUpdater<TABLE, ROW> updater,
                                                  RowState<ROW> row, string debugName, CancellationToken ct) {

            int rowsEffected = await updater.UpdateAsync(
                oldRow: row.OldRow!,
                newRow: row.NewRow,
                table: Table,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName,
                ct: ct
            );

            if(rowsEffected == 0) {
                throw new Exception(
                    message: "Concurrency violation. Maybe row was changed in the database?"
                );
            }
            if(rowsEffected != 1) {
                throw new Exception(
                    message: $"Update failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
            newState.Add(
                new RowState<ROW>(
                    state: RowUpdateState.Existing,
                    oldRow: ROW.CloneRow(row.NewRow),
                    newRow: row.NewRow
                )
            );
            return rowsEffected;
        }

        private static int PerformDelete(Transaction transaction, QueryTimeout? timeout,
                                         RowUpdater<TABLE, ROW> updater, RowState<ROW> row,
                                         string debugName) {

            int rowsEffected = updater.Delete(
                existingRow: row.OldRow!,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName
            );

            if(rowsEffected != 1) {
                throw new Exception(
                    message: $"Delete failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
            return rowsEffected;
        }

        private static async Task<int> PerformDeleteAsync(Transaction transaction, QueryTimeout? timeout,
                                                          RowUpdater<TABLE, ROW> updater, RowState<ROW> row,
                                                          string debugName, CancellationToken ct) {

            int rowsEffected = await updater.DeleteAsync(
                existingRow: row.OldRow!,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName,
                ct: ct
            );

            if(rowsEffected != 1) {
                
                throw new Exception(
                    message: $"Delete failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
            return rowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations
        /// where the order ofinserts, update and deletes is important.
        /// </summary>
        public int PersistInsertsOnly(Transaction transaction) {
            return PersistInsertsOnly(transaction, timeout: TimeoutLevel.ShortInsert);
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistInsertsOnly(Transaction transaction, QueryTimeout timeout) {
            return PersistInsertsOnly(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistInsertsOnly(Transaction transaction, QueryTimeout timeout, string debugName) {

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(
                database: transaction.Database,
                table: Table,
                matchOn: MatchOn
            );

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    
                    totalRowsEffected += PerformInsert(
                        transaction: transaction,
                        timeout: timeout,
                        newState: newState,
                        updater: updater,
                        rowState: row,
                        debugName: debugName
                    );
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        private static void InvalidateRowIndexes(IEnumerable<RowState<ROW>> rows) {

            foreach(RowState<ROW> state in rows) {

                state.OldRow?.InternalIndex = int.MaxValue;
                state.NewRow.InternalIndex = int.MaxValue;
            }
        }
        private static void ReindexRows(IEnumerable<RowState<ROW>> rows) {

            int index = 0;

            foreach(RowState<ROW> state in rows) {

                state.OldRow?.InternalIndex = index;
                state.NewRow.InternalIndex = index;
                index++;
            }
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, CancellationToken ct) {
            return await PersistInsertsOnlyAsync(transaction, timeout: TimeoutLevel.ShortInsert, ct);
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct) {
            return await PersistInsertsOnlyAsync(transaction, timeout, debugName: "", ct);
        }

        /// <summary>
        /// Persist only records that need to be inserted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistInsertsOnlyAsync(Transaction transaction, QueryTimeout timeout,
                                                       string debugName, CancellationToken ct) {

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.PendingAdd) {
                    
                    totalRowsEffected += await PerformInsertAsync(
                        transaction: transaction,
                        timeout: timeout,
                        newState: newState,
                        updater: updater,
                        rowState: row,
                        debugName: debugName,
                        ct: ct
                    );
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistUpdatesOnly(Transaction transaction) {
            return PersistUpdatesOnly(transaction, timeout: TimeoutLevel.ShortUpdate);
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistUpdatesOnly(Transaction transaction, QueryTimeout timeout) {
            return PersistUpdatesOnly(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistUpdatesOnly(Transaction transaction, QueryTimeout timeout, string debugName) {

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row has a {nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        
                        totalRowsEffected += PerformUpdate(
                            transaction: transaction,
                            timeout: timeout,
                            newState: newState,
                            updater: updater,
                            row: row,
                            debugName: debugName
                        );
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, CancellationToken ct) {
            return await PersistUpdatesOnlyAsync(transaction, timeout: TimeoutLevel.ShortUpdate, ct);
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct) {
            return await PersistUpdatesOnlyAsync(transaction, timeout, debugName: "", ct);
        }

        /// <summary>
        /// Persist only records that need to be updated. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistUpdatesOnlyAsync(Transaction transaction, QueryTimeout timeout,
                                                       string debugName, CancellationToken ct) {

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.Existing) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row " +
                                     $"has a {nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }

                    bool rowHasChanged = !row.OldRow.Equals(row.NewRow);

                    if(rowHasChanged) {
                        
                        totalRowsEffected += await PerformUpdateAsync(
                            transaction: transaction,
                            timeout: timeout,
                            newState: newState,
                            updater: updater,
                            row: row,
                            debugName: debugName,
                            ct: ct
                        );
                    }
                    else {
                        newState.Add(row);  //Add unchanged row
                    }
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }


        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistDeletesOnly(Transaction transaction) {
            return PersistDeletesOnly(transaction, timeout: TimeoutLevel.ShortDelete);
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistDeletesOnly(Transaction transaction, QueryTimeout timeout) {
            return PersistDeletesOnly(transaction, timeout, debugName: "");
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public int PersistDeletesOnly(Transaction transaction, QueryTimeout timeout, string debugName) {

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {

                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when row " +
                                     $"has a {nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }
                    totalRowsEffected += PerformDelete(
                        transaction: transaction,
                        timeout: timeout,
                        updater: updater,
                        row: row,
                        debugName: debugName
                    );
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, CancellationToken ct) {
            return await PersistDeletesOnlyAsync(transaction, timeout: TimeoutLevel.ShortDelete, ct);
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout,
                                                       CancellationToken ct) {
            return await PersistDeletesOnlyAsync(transaction, timeout, debugName: "", ct);
        }

        /// <summary>
        /// Persist only records that need to be deleted. This is useful for situations
        /// where the order of inserts, update and deletes is important.
        /// </summary>
        public async Task<int> PersistDeletesOnlyAsync(Transaction transaction, QueryTimeout timeout,
                                                       string debugName, CancellationToken ct) {

            List<RowState<ROW>> newState = new(Rows.Count);

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, Table, MatchOn);

            int totalRowsEffected = 0;

            foreach(RowState<ROW> row in Rows) {

                if(row.State == RowUpdateState.PendingDelete) {

                    if(row.OldRow == null) {
                        throw new Exception(
                            message: $"{nameof(row.OldRow)} should not be null when " +
                                     $"row has a {nameof(row.State)} == {row.State}. This is a bug."
                        );
                    }
                    totalRowsEffected += await PerformDeleteAsync(
                        transaction: transaction,
                        timeout: timeout,
                        updater: updater,
                        row: row,
                        debugName: debugName,
                        ct: ct
                    );
                }
            }
            InvalidateRowIndexes(Rows);
            ReindexRows(newState);
            Rows = newState;
            return totalRowsEffected;
        }

        public static void ExecuteInsert(ROW row, TABLE table, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Insert(
                newRow: row,
                transaction: transaction,
                timeout: null,
                debugName: ""
            );

            if(rowsEffected != 1) {

                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
        }

        public static void ExecuteInsert(ROW row, TABLE table, QueryTimeout? timeout, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Insert(
                newRow: row,
                transaction: transaction,
                timeout: timeout,
                debugName: ""
            );

            if(rowsEffected != 1) {

                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
        }

        public static void ExecuteInsert(ROW row, TABLE table, QueryTimeout? timeout,
                                         string debugName, Transaction transaction) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = updater.Insert(
                newRow: row,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName
            );

            if(rowsEffected != 1) {
                
                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
        }

        public static async Task ExecuteInsertAsync(ROW row, TABLE table, Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.InsertAsync(
                newRow: row,
                transaction: transaction,
                timeout: null,
                debugName: "",
                ct: ct
            );

            if(rowsEffected != 1) {
                
                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
        }

        public static async Task ExecuteInsertAsync(ROW row, TABLE table, QueryTimeout? timeout,
                                                    Transaction transaction, CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.InsertAsync(
                newRow: row,
                transaction: transaction,
                timeout: timeout,
                debugName: "",
                ct: ct
            );

            if(rowsEffected != 1) {
                
                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
        }
        public static async Task ExecuteInsertAsync(ROW row, TABLE table, QueryTimeout? timeout,
                                                    string debugName, Transaction transaction,
                                                    CancellationToken ct) {

            RowUpdater<TABLE, ROW> updater = GetOrBuildRowUpdater(transaction.Database, table, MatchOn.PrimaryKey);

            int rowsEffected = await updater.InsertAsync(
                newRow: row,
                transaction: transaction,
                timeout: timeout,
                debugName: debugName,
                ct: ct
            );

            if(rowsEffected != 1) {
                
                throw new Exception(
                    message: $"Insert failed. {nameof(rowsEffected)} should have == 1. Instead == {rowsEffected}."
                );
            }
        }
    }

    public sealed class RowState<ROW>(RowUpdateState state, ROW? oldRow, ROW newRow) {

        public RowUpdateState State { get; set; } = state;
        public ROW? OldRow { get; set; } = oldRow;
        public ROW NewRow { get; set; } = newRow;
    }

    public enum RowUpdateState : byte {
        PendingAdd,
        PendingDelete,
        Existing,
        Deleted
    }

    public sealed class ColumnAndSetter<ROW>(IColumn column, Func<ROW, object?> getter,
                                             Action<ROW, IResultRow> setter,
                                             Func<ROW, ROW, bool> valuesEqual) {

        public string? ParameterName { get; internal set; }
        public IColumn Column { get; } = column;
        public Func<ROW, object?> Getter { get; } = getter;
        public Action<ROW, IResultRow> Setter { get; } = setter;

        /// <summary>
        /// Returns true if the column in row A has the same value in row B.
        /// </summary>
        public Func<ROW, ROW, bool> ValuesEqual { get; } = valuesEqual;
    }

    /// <summary>
    /// RefCompare is a struct that compares records by instance rather than equality.
    /// </summary>
    //public readonly struct RefCompare<TYPE> where TYPE : class {

    //    private TYPE Row { get; }

    //    public RefCompare(TYPE row) {
    //        Row = row;
    //    }
    //    public override bool Equals([NotNullWhen(true)] object? obj) {

    //        if(obj is RefCompare<TYPE> refCompare) {
    //            return object.ReferenceEquals(Row, refCompare.Row);
    //        }
    //        return false;
    //    }
    //    public override int GetHashCode() {
    //        return RuntimeHelpers.GetHashCode(Row);
    //    }
    //    public static bool operator ==(RefCompare<TYPE> left, RefCompare<TYPE> right) {
    //        return left.Equals(right);
    //    }
    //    public static bool operator !=(RefCompare<TYPE> left, RefCompare<TYPE> right) {
    //        return !(left == right);
    //    }
    //}
}