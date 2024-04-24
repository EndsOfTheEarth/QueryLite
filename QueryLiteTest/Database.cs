using QueryLite;
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;

namespace QueryLiteTest {

    public static class TestDatabase {

        //public static IDatabase Database { get; set; } = new SqlServerDatabase(name: "QueryLiteTest", connectionString: "Server=localhost;Database=QueryLiteTesting;Trusted_Connection=True;");
        public static IDatabase Database { get; set; } = new PostgreSqlDatabase(name: "QueryLiteTest", connectionString: "Server=127.0.0.1;Port=5432;Database=QueryLiteTesting;User Id=postgres;Password=1;", schemaMap: schema => schema == "dbo" ? "public" : schema);
    }
}