using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectTenRowBenchmarks {

        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedQueryExecute<SelectTenRowBenchmarks, Test01> _preparedSelectQuery;

        public SelectTenRowBenchmarks() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedSelectQuery = Query
                .Prepare<SelectTenRowBenchmarks>()
                .Select(
                    row => new Test01(table, row)
                )
                .From(table)
                .Build();

            _preparedSelectQuery.Initialize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            using(Transaction transaction = new Transaction(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                for(int index = 0; index < 10; index++) {

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
        public void Ado_Ten_Row_Select() {

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
        public void Dapper_Ten_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                IEnumerable<Test01> result = connection.Query<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01");
            }
        }

        [Benchmark]
        public void QueryLite_Ten_Row_Prepared_Select() {

            for(int index = 0; index < _iterations; index++) {

                QueryResult<Test01> result = _preparedSelectQuery.Execute(parameters: this, Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void QueryLite_Ten_Row_Dynamic_Select() {

            for(int index = 0; index < _iterations; index++) {

                Tables.Test01Table table = Tables.Test01Table.Instance;

                QueryResult<Test01> result = Query
                    .Select(
                        row => new Test01(table, row)
                    )
                    .From(table)
                    .Execute(Databases.TestDatabase);
            }
        }
    }
}