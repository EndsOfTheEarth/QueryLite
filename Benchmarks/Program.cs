using BenchmarkDotNet.Running;

namespace Benchmarks {

    internal class Program {

        static async Task Main(string[] args) {

            //BenchmarkRunner.Run<InsertBenchmarks>();
            //BenchmarkRunner.Run<UpdateSingleRowBenchmarks>();
            BenchmarkRunner.Run<DeleteSingleRowBenchmarks>();

            //BenchmarkRunner.Run<SelectSingleRowBenchmarks>();
            //BenchmarkRunner.Run<SelectTenRowBenchmarks>();
            //BenchmarkRunner.Run<SelectTenRowWithWhereClauseBenchmarks>();
            //BenchmarkRunner.Run<SelectOneHundredRowBenchmarks>();
            //BenchmarkRunner.Run<SelectOneThousandRowBenchmarks>();

            //BenchmarkRunner.Run<SelectSingleRowAsyncBenchmarks>();
            //BenchmarkRunner.Run<SelectTenRowAsyncBenchmarks>();
            //BenchmarkRunner.Run<SelectOneHundredRowAsyncBenchmarks>();
            //BenchmarkRunner.Run<SelectOneThousandRowAsyncBenchmarks>();

            //await new SelectOneThousandRowAsyncBenchmarks().Ado_One_Thousand_Row_SelectAsync();

            //await new SelectOneThousandRowAsyncBenchmarks().Ado_One_Thousand_Row_SelectAsync();
            //await new SelectOneThousandRowAsyncBenchmarks().QueryLite_One_Thousand_Row_Prepared_SelectAsync();

            //SelectOneThousandRowBenchmarks benchmark = new SelectOneThousandRowBenchmarks();
            //benchmark.Setup();
            //benchmark.QueryLite_One_Thousand_Row_Repository_Select();

            //await benchmark.Ado_One_Hundred_Row_SelectAsync();
            Console.Read();
        }
    }

    public static class Databases {

        public readonly static string ConnectionString = "Server=127.0.0.1;Port=5432;Database=Benchmarks;User Id=postgres;Password=1;";

        public readonly static QueryLite.IDatabase TestDatabase = new QueryLite.Databases.PostgreSql.PostgreSqlDatabase(
            name: "Benchmarks",
            connectionString: ConnectionString
        );

        public static void ResetTable() {

            string sql = @"
DROP TABLE Test01;

CREATE TABLE IF NOT EXISTS Test01 (
	
	id SERIAL NOT NULL PRIMARY KEY,
	row_guid UUID NOT NULL,
	message VARCHAR(100) NOT NULL,
	date TIMESTAMP NOT NULL
);";
            QueryLite.Query.ExecuteNonQuery(sql: sql, TestDatabase);
        }
    }
}