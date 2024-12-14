using QueryLite;
using QueryLite.Databases.PostgreSql;
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
            else {
                throw new Exception($"Unknown {nameof(dbType)} setting. Value = '{dbType}'");
            }

            if(databaseType == DatabaseType.SqlServer) {
                Database = new SqlServerDatabase(name: "QueryLiteTest", connectionString: settings!.SqlServerConnectionString);
            }
            else if(databaseType == DatabaseType.PostgreSql) {
                Database = new PostgreSqlDatabase(name: "QueryLiteTest", connectionString: settings!.PostgreSqlConnectionString, schemaMap: schema => schema == "dbo" ? "public" : schema);
            }
            else {
                throw new Exception($"Unknown {nameof(databaseType)}. Value = '{databaseType}'");
            }
        }
    }

    public class TestSettings {

        public string DatabaseType { get; set; } = string.Empty;
        public string SqlServerConnectionString { get; set; } = string.Empty;
        public string PostgreSqlConnectionString { get; set; } = string.Empty;
    }
}