using BenchmarkDotNet.Running;

namespace Benchmarks {

    internal class Program {

        static async Task Main(string[] args) {

            //BenchmarkRunner.Run<InsertBenchmarks>();
            //BenchmarkRunner.Run<UpdateSingleRowBenchmarks>();
            BenchmarkRunner.Run<DeleteSingleRowBenchmarks>();

            //BenchmarkRunner.Run<SelectSingleRowBenchmarks>();
            //BenchmarkRunner.Run<SelectTenRowBenchmarks>();
            //BenchmarkRunner.Run<SelectOneHundredRowBenchmarks>();
            //BenchmarkRunner.Run<SelectOneThousandRowBenchmarks>();

            //BenchmarkRunner.Run<SelectSingleRowAsyncBenchmarks>();
            //BenchmarkRunner.Run<SelectTenRowAsyncBenchmarks>();
            //BenchmarkRunner.Run<SelectOneHundredRowAsyncBenchmarks>();
            //BenchmarkRunner.Run<SelectOneThousandRowAsyncBenchmarks>();

            //await new SelectOneThousandRowAsyncBenchmarks().Ado_One_Thousand_Row_SelectAsync();

            //await new SelectOneThousandRowAsyncBenchmarks().Ado_One_Thousand_Row_SelectAsync();
            //await new SelectOneThousandRowAsyncBenchmarks().QueryLite_One_Thousand_Row_Prepared_SelectAsync();

            //InsertBenchmarks benchmark = new InsertBenchmarks();
            //benchmark.Setup();

            //await benchmark.Ado_One_Hundred_Row_SelectAsync();
        }
    }

    public static class Databases {

        public readonly static string ConnectionString = "Server=127.0.0.1;Port=5432;Database=Benchmarks;User Id=postgres;Password=1;";

        public static QueryLite.IDatabase TestDatabase = new QueryLite.Databases.PostgreSql.PostgreSqlDatabase(
            name: "Benchmarks",
            connectionString: ConnectionString
        );
    }
}