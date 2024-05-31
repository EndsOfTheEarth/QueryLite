using QueryLite;
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;
using System;
using System.IO;
using System.Text.Json;

namespace QueryLiteTest {

    public static class TestDatabase {

        public static IDatabase Database { get; set; }

        static TestDatabase() {

            string filePath = Environment.GetEnvironmentVariable("QueryLiteTestSettingsFilePath")!; //Note: Requires an environment variable called "QueryLiteTestSettingsFilePath" populated with the TestSettings file path. Visual studio may need to be restarted to read any newly created value.

            TestSettings? settings = JsonSerializer.Deserialize<TestSettings>(File.ReadAllText(filePath));

            DatabaseType databaseType = DatabaseType.SqlServer;

            if(databaseType == DatabaseType.SqlServer) {
                Database = new SqlServerDatabase(name: "QueryLiteTest", connectionString: settings!.SqlServerConnectionString);
            }
            else {
                Database = new PostgreSqlDatabase(name: "QueryLiteTest", connectionString: settings!.PostgreSqlConnectionString, schemaMap: schema => schema == "dbo" ? "public" : schema);
            }
        }
    }

    public class TestSettings {

        public string SqlServerConnectionString { get; set; } = string.Empty;
        public string PostgreSqlConnectionString { get; set; } = string.Empty;
    }
}