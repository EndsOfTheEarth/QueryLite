using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectSingleRowBenchmarks {

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedQueryExecute<SelectSingleRowBenchmarks, Test01> _preparedSelectQuery;

        public SelectSingleRowBenchmarks() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedSelectQuery = Query
                .Prepare<SelectSingleRowBenchmarks>()
                .Select(
                    row => new Test01(table, row)
                )
                .From(table)
                .Where(where => where.EQUALS(table.Row_guid, info => info._guid))
                .Build();

            _preparedSelectQuery.Initialize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            using(Transaction transaction = new Transaction(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                NonQueryResult result = Query
                    .Insert(table)
                    .Values(values => values
                        .Set(table.Row_guid, _guid)
                        .Set(table.Message, _message)
                        .Set(table.Date, _date)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }
        }

        private int _iterations = 2000;

        [Benchmark]
        public void Ado_Single_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                using NpgsqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0";

                command.Parameters.Add(new NpgsqlParameter(parameterName: "@0", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = _guid });

                using NpgsqlDataReader reader = command.ExecuteReader();

                reader.Read();

                List<Test01> list = new List<Test01>();

                Test01 test01 = new Test01(
                    id: reader.GetInt32(0),
                    row_guid: reader.GetGuid(1),
                    message: reader.GetString(2),
                    date: reader.GetDateTime(3)
                );
                list.Add(test01);
            }
        }

        [Benchmark]
        public void Dapper_Single_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                IEnumerable<Test01> result = connection.Query<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@_guid", new { _guid });
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Prepared_Select() {

            for(int index = 0; index < _iterations; index++) {

                QueryResult<Test01> result = _preparedSelectQuery.Execute(parameterValues: this, Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Dynamic_Select() {

            for(int index = 0; index < _iterations; index++) {

                Tables.Test01Table table = Tables.Test01Table.Instance;

                QueryResult<Test01> result = Query
                    .Select(
                        row => new Test01(table, row)
                    )
                    .From(table)
                    .Where(table.Row_guid == _guid)
                    .Execute(Databases.TestDatabase);
            }
        }
    }
}