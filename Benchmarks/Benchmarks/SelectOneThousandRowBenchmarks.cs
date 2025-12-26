using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Benchmarks.Tables;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectOneThousandRowBenchmarks {

        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedQueryExecute<SelectOneThousandRowBenchmarks, Test01> _preparedSelectQuery;

        public SelectOneThousandRowBenchmarks() {

            Test01Table table = Test01Table.Instance;

            _preparedSelectQuery = Query
                .Prepare<SelectOneThousandRowBenchmarks>()
                .Select(
                    row => new Test01(table, row)
                )
                .From(table)
                .Build();

            _preparedSelectQuery.Initialize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Test01Table table = Test01Table.Instance;

            using(Transaction transaction = new Transaction(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                for(int index = 0; index < 1000; index++) {

                    NonQueryResult result = Query
                        .Insert(table)
                        .Values(values => values
                            .Set(table.Row_guid, Guid.NewGuid())
                            .Set(table.Message, _message)
                            .Set(table.Date, _date)
                        )
                        .Execute(transaction);
                }
                transaction.Commit();
            }
        }

        private int _iterations = 2000;

        [Benchmark]
        public void Ado_One_Thousand_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                using NpgsqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT id,row_guid,message,date FROM Test01";

                using NpgsqlDataReader reader = command.ExecuteReader();

                List<Test01> list = new List<Test01>();

                while(reader.Read()) {

                    list.Add(
                        new Test01(
                            id: reader.GetInt32(0),
                            row_guid: reader.GetGuid(1),
                            message: reader.GetString(2),
                            date: reader.GetDateTime(3)
                        )
                    );
                }
            }
        }

        [Benchmark]
        public void Dapper_One_Thousand_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                IEnumerable<Test01> result = connection.Query<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01");
            }
        }

        [Benchmark]
        public void QueryLite_One_Thousand_Row_Prepared_Select() {

            for(int index = 0; index < _iterations; index++) {

                QueryResult<Test01> result = _preparedSelectQuery.Execute(parameters: this, Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void QueryLite_One_Thousand_Row_Dynamic_Select() {

            for(int index = 0; index < _iterations; index++) {

                Test01Table table = Test01Table.Instance;

                QueryResult<Test01> result = Query
                    .Select(
                        row => new Test01(table, row)
                    )
                    .From(table)
                    .Execute(Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void QueryLite_One_Thousand_Row_Repository_Select() {

            for(int index = 0; index < _iterations; index++) {

                Test01RowRepository repository = new Test01RowRepository();

                repository.SelectRows.Execute(Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void EF_Core_One_Thousand_Row_Select() {

            using TestContext context = new TestContext(Databases.ConnectionString);

            for(int index = 0; index < _iterations; index++) {

                List<Test01Row_EfCore> list = context.TestRows.ToList();
            }
        }
    }
}