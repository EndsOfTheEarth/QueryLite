using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
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

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedSelectQuery = Query
                .PrepareWithParameters<SelectOneThousandRowBenchmarks>()
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

            using(QueryLite.Transaction transaction = new QueryLite.Transaction(Databases.TestDatabase)) {

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

        [Benchmark]
        public void Ado_One_Thousand_Row_Select() {

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

        [Benchmark]
        public void Dapper_One_Thousand_Row_Select() {

            using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

            connection.Open();

            IEnumerable<Test01> result = connection.Query<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01");

        }

        [Benchmark]
        public void QueryLite_One_Thousand_Row_Prepared_Select() {

            QueryResult<Test01> result = _preparedSelectQuery.Execute(parameterValues: this, Databases.TestDatabase);
        }

        [Benchmark]
        public void QueryLite_One_Thousand_Row_Dynamic_Select() {

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