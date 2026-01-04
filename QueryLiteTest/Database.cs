using QueryLite;
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.Sqlite;
using QueryLite.Databases.SqlServer;
using System;
using System.IO;
using System.Text.Json;

namespace QueryLiteTest {

    public static class TestDatabase {

        public static IDatabase Database { get; set; }

        //  Example test settings file

        /*  
            {
                "DatabaseType": "SqlServer",
                //"DatabaseType": "PostgreSql",
                "SqlServerConnectionString": "Server=localhost;Database=QueryLiteTesting;Trusted_Connection=True;TrustServerCertificate=True",
                "PostgreSqlConnectionString": "Server=127.0.0.1;Port=5432;Database=QueryLiteTesting;User Id=;Password=;"
            }
         */

        static TestDatabase() {

            string filePath = Environment.GetEnvironmentVariable("QueryLiteTestSettingsFilePath")!; //Note: Requires an environment variable called "QueryLiteTestSettingsFilePath" populated with the TestSettings file path. Visual studio may need to be restarted to read any newly created value.

            TestSettings? settings = JsonSerializer.Deserialize<TestSettings>(File.ReadAllText(filePath), options: new JsonSerializerOptions() { ReadCommentHandling = JsonCommentHandling.Skip });

            string dbType = settings!.DatabaseType;

            DatabaseType databaseType;

            if(string.Equals(dbType, "SqlServer", StringComparison.OrdinalIgnoreCase)) {
                databaseType = DatabaseType.SqlServer;
            }
            else if(string.Equals(dbType, "PostgreSql", StringComparison.OrdinalIgnoreCase)) {
                databaseType = DatabaseType.PostgreSql;
            }
            else if(string.Equals(dbType, "SqlLite", StringComparison.OrdinalIgnoreCase)) {
                databaseType = DatabaseType.Sqlite;
            }
            else {
                throw new Exception($"Unknown {nameof(dbType)} setting. Value = '{dbType}'");
            }

            if(databaseType == DatabaseType.SqlServer) {
                Database = new SqlServerDatabase(name: "QueryLiteTest", connectionString: settings!.SqlServerConnectionString);
            }
            else if(databaseType == DatabaseType.PostgreSql) {
                Database = new PostgreSqlDatabase(name: "QueryLiteTest", connectionString: settings!.PostgreSqlConnectionString, schemaMap: schema => schema == "dbo" ? "public" : schema);
            }
            else if(databaseType == DatabaseType.Sqlite) {

                string path = settings!.SqliteConnectionString.Replace("Data Source=", "");

                if(File.Exists(path) && !string.IsNullOrWhiteSpace(path)) {
                    File.Delete(path);
                }
                Database = new SqliteDatabase(name: "QueryLiteTest", connectionString: settings!.SqliteConnectionString, schemaMap: schema => "");
                Query.ExecuteNonQuery(sql: SqliteSchema.Sql, database: Database);
            }
            else {
                throw new Exception($"Unknown {nameof(databaseType)}. Value = '{databaseType}'");
            }
        }
    }

    public static class SqliteSchema {

        public static string Sql =>
@"CREATE TABLE AllTypes (	
	taId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    taGuid BLOB NOT NULL,
    taString TEXT NOT NULL,
    taSmallInt INTEGER NOT NULL,
    taInt INTEGER NOT NULL,
    taBigInt INTEGER NOT NULL,
    taDecimal NUMERIC NOT NULL,
    taFloat REAL NOT NULL,
    taDouble REAL NOT NULL,
    taBoolean INTEGER NOT NULL,
    taBytes BLOB NOT NULL,
    taDateTime TEXT NOT NULL,
    taDateTimeOffset TEXT NOT NULL,
    taEnum INTEGER NOT NULL,
    taDateOnly TEXT NOT NULL,
    taTimeOnly TEXT NOT NULL
);
";
    }

    public class TestSettings {

        public string DatabaseType { get; set; } = "";
        public string SqlServerConnectionString { get; set; } = "";
        public string PostgreSqlConnectionString { get; set; } = "";
        public string SqliteConnectionString { get; set; } = "";
    }
}