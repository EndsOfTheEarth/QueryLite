using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Benchmarks {

    internal class Program {

        static void Main(string[] args) {

            //BenchmarkRunner.Run<SelectSingleRowBenchmarks>();

            //BenchmarkRunner.Run<SelectTenRowBenchmarks>();

            //new SelectTenRowBenchmarks().QueryLite_Ten_Row_Prepared_Select();

            BenchmarkRunner.Run<SelectOneHundredRowBenchmarks>();

            //BenchmarkRunner.Run<SelectOneThousandRowBenchmarks>();

            //BenchmarkRunner.Run<InsertBenchmarks>();

            //new SelectSingleRowBenchmarks().QueryLite_Single_Row_Select();

            //new InsertBenchmarks().Ado_Single_Insert();
            //new InsertBenchmarks().QueryLite_Single_Insert();
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