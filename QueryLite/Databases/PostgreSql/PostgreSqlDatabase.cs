using Npgsql;
using System;
using System.Data.Common;

namespace QueryLite.Databases.PostgreSql {

    public class PostgreSqlDatabase : IDatabase {

        public string Name { get; }
        string IDatabase.Name => Name;
        public Func<string, string> SchemaMap { get; }
        private string ConnectionString { get; }

        IQueryGenerator IInternalConnection.QueryGenerator { get; } = new PostgreSqlSelectQueryGenerator();
        IInsertQueryGenerator IInternalConnection.InsertGenerator { get; } = new PostgreSqlInsertQueryGenerator();
        IUpdateQueryGenerator IInternalConnection.UpdateGenerator { get; } = new PostgreSqlUpdateQueryGenerator();
        IDeleteQueryGenerator IInternalConnection.DeleteGenerator { get; } = new PostgreSqlDeleteQueryGenerator();
        ITruncateQueryGenerator IInternalConnection.TruncateGenerator { get; } = new PostgreSqlTruncateQueryGenerator();

        public DatabaseType DatabaseType => DatabaseType.PostgreSql;

        public PostgreSqlDatabase(string name, string connectionString, Func<string, string>? schemaMap = null) {

            ArgumentNullException.ThrowIfNullOrEmpty(connectionString, paramName: nameof(connectionString));

            Name = name ?? string.Empty;
            SchemaMap = schemaMap ?? ((schema) => schema);

            ConnectionString = connectionString;
        }

        public DbConnection GetNewConnection() {

            NpgsqlConnection? connection = null;

            try {
                connection = new NpgsqlConnection(ConnectionString);
            }
            catch {
                connection?.Dispose();
                throw;
            }
            return connection;
        }
        DbConnection IDatabase.GetNewConnection() {
            return GetNewConnection();
        }

        public string ConvertToSql(object value) {
            return PostgreSqlTypeMappings.ConvertToSql(value);
        }
        public IParameters CreateParameters() {
            return new PostgreSqlParameters();
        }

        public string? GetCSharpCodeSet(Type dotNetType) {
            return PostgreSqlTypeMappings.GetCSharpCodeSet(dotNetType);
        }
    }
}