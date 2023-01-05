using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace QueryLite.Databases.SqlServer {

    public class SqlServerDatabase : IDatabase {

        public string Name { get; }
        string IDatabase.Name => Name;
        public Func<string, string> SchemaMap { get; }
        private string ConnectionString { get; }

        IQueryGenerator IInternalConnection.QueryGenerator { get; } = new SqlServerSelectQueryGenerator();
        IInsertQueryGenerator IInternalConnection.InsertGenerator { get; } = new SqlServerInsertQueryGenerator();
        IUpdateQueryGenerator IInternalConnection.UpdateGenerator { get; } = new SqlServerUpdateQueryGenerator();
        IDeleteQueryGenerator IInternalConnection.DeleteGenerator { get; } = new SqlServerDeleteQueryGenerator();
        ITruncateQueryGenerator IInternalConnection.TruncateGenerator { get; } = new SqlServerTruncateQueryGenerator();

        public DatabaseType DatabaseType => DatabaseType.SqlServer;

        public SqlServerDatabase(string name, string connectionString, Func<string, string>? schemaMap = null) {

            ArgumentNullException.ThrowIfNullOrEmpty(connectionString, paramName: nameof(connectionString));

            Name = name ?? string.Empty;
            SchemaMap = schemaMap ?? ((schema) => schema);

            ConnectionString = connectionString;
        }

        public DbConnection GetNewConnection() {

            SqlConnection? connection = null;

            try {
                connection = new SqlConnection(ConnectionString);
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
            return SqlServerSqlTypeMappings.ConvertToSql(value);
        }
        public IParameters CreateParameters() {
            return new SqlServerParameters();
        }
        public string? GetCSharpCodeSet(Type dotNetType) {
            return SqlServerSqlTypeMappings.GetCSharpCodeSet(dotNetType);
        }
    }
}