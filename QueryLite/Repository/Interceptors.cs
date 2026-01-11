
namespace QueryLite.Repository {

    public enum Change {
        Insert, Update, Delete
    }
    public abstract class RepositorySavingChangesInterceptor {

        public virtual void SavingChanges(Change change, object? oldRow, object? newRow) { }
        public virtual Task SavingChangesAsync(Change change, object? oldRow, object? newRow) => Task.CompletedTask;
    }
}