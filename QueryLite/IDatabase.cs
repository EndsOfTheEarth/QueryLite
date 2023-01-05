using System;
using System.Data.Common;

namespace QueryLite {

    public interface IDatabase : IInternalConnection {
        /// <summary>
        /// Descriptive name of database
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Database type
        /// </summary>
        public DatabaseType DatabaseType { get; }

        /// <summary>
        /// Function to map between schema names when using more than one type of database. For example: the Sql Server schema might be 'dbo' but it is 'public' in PostgreSql.
        /// </summary>
        Func<string, string> SchemaMap { get; }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns></returns>
        DbConnection GetNewConnection();
        public string ConvertToSql(object value);
        public IParameters CreateParameters();
        string? GetCSharpCodeSet(Type dotNetType);
    }

    public enum DatabaseType {
        SqlServer = 1,
        PostgreSql = 2
    }

    public interface IInternalConnection {

        internal IQueryGenerator QueryGenerator { get; }
        internal IInsertQueryGenerator InsertGenerator { get; }
        internal IUpdateQueryGenerator UpdateGenerator { get; }
        internal IDeleteQueryGenerator DeleteGenerator { get; }
        internal ITruncateQueryGenerator TruncateGenerator { get; }
    }
}