using System;

namespace QueryLite {

    public interface ITable {

        /// <summary>
        /// Table sql alias used for query generation
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Schema table belongs to
        /// </summary>
        string SchemaName { get; }

        /// <summary>
        /// Name of table in database
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Enclose table name in sql query. For example enclose table name with [] (square brackets) in sql server queries.
        /// </summary>
        bool Enclose { get; }
    }

    /// <summary>
    /// This class is used to generate an alias for each instance of a table class. This alias is used when generating sql queries.
    /// </summary>
    internal static class AliasGenerator {

        private static long counter = 0;
        private readonly static object lock_ = new object();

        public static string GetAlias() {

            long alias;

            lock(lock_) {
                alias = counter++;
            }
            return alias.ToString("X");
        }
    }

    /// <summary>
    /// Represents a database table
    /// </summary>
    public abstract class ATable : ITable {

        public string SchemaName { get; }
        public string TableName { get; }
        public bool Enclose { get; }
        public string Alias { get; }

        /// <summary>
        /// Abstract table
        /// </summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="schemaName">Schema table belongs to</param>
        /// <param name="enclose">Set to true if the table name is a sql keyword. This will stop the key word from causing invalid sql from being created</param>
        /// <exception cref="ArgumentException"></exception>
        protected ATable(string tableName, string schemaName = "", bool enclose = false) {

            ArgumentException.ThrowIfNullOrEmpty(tableName, paramName: nameof(tableName));

            SchemaName = schemaName;
            TableName = !string.IsNullOrWhiteSpace(tableName) ? tableName : throw new ArgumentException($"{nameof(tableName)} cannot be whitespace or empty");
            Enclose = enclose;
            Alias = "_" + AliasGenerator.GetAlias();
        }
    }
}