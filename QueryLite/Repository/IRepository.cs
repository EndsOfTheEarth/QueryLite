using QueryLite.Repository;
using System.Diagnostics.CodeAnalysis;

namespace QueryLite {

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
        IRepositoryWith SelectRows { get; }

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
        int SaveChanges(IDatabase database);

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
        Task<int> SaveChangesAsync(IDatabase database, CancellationToken ct);

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
}