using BenchmarkDotNet.Attributes;
using Benchmarks.Tables;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class UpdateSingleRowBenchmarks {

        public int Id { get; private set; }
        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message1 = "message1";
        private readonly string _message2 = "message2";
        private readonly DateTime _date = DateTime.Now;

        private readonly IPreparedUpdateQuery<UpdateSingleRowBenchmarks> _preparedUpdateQuery;

        public UpdateSingleRowBenchmarks() {

            Test01Table table = Test01Table.Instance;

            _preparedUpdateQuery = Query
                .Prepare<UpdateSingleRowBenchmarks>()
                .Update(table)
                .Values(values => values
                    .Set(table.Message, info => _message2)
                    .Set(table.Date, info => DateTime.Now)
                )
                .Where(where => where.EQUALS(table.Row_guid, info => info._guid))
                .Build();

            _preparedUpdateQuery.Initialize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Databases.ResetTable();

            Test01Table table = Test01Table.Instance;

            using(Transaction transaction = new(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                QueryResult<int> result = Query
                    .Insert(table)
                    .Values(values => values
                        .Set(table.Row_guid, _guid)
                        .Set(table.Message, _message1)
                        .Set(table.Date, _date)
                    )
                    .Execute(returning => returning.Get(table.Id), transaction);

                Id = result.Rows[0];
                transaction.Commit();
            }
            using TestContext context = new(Databases.ConnectionString);
            Test01Row_EfCore row = context.TestRows.Where(test => test.Row_guid == _guid).First();
        }

        private readonly int _iterations = 2000;

        [Benchmark]
        public void Ado_Single_Row_Update() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new(Databases.ConnectionString);

                connection.Open();

                using NpgsqlTransaction transaction = connection.BeginTransaction();

                using NpgsqlCommand command = connection.CreateCommand();

                command.Transaction = transaction;

                command.CommandText = "UPDATE Test01 SET message=@1,date=@2 WHERE row_guid=@0";

                command.Parameters.Add(new NpgsqlParameter(parameterName: "@0", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = _guid });
                command.Parameters.Add(new NpgsqlParameter(parameterName: "@1", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = index % 2 == 0 ? _message2 : _message1 });
                command.Parameters.Add(new NpgsqlParameter(parameterName: "@2", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = DateTime.Now });

                int rows = command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        [Benchmark]
        public void Dapper_Single_Row_Update() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new(Databases.ConnectionString);

                connection.Open();

                using NpgsqlTransaction transaction = connection.BeginTransaction();

                var parameters = new { message = index % 2 == 0 ? _message2 : _message1, date = DateTime.Now, row_guid = _guid };

                int rows = connection.Execute(sql: "UPDATE Test01 SET message=@message,date=@date WHERE row_guid=@row_guid", parameters, transaction: transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Prepared_Update() {

            for(int index = 0; index < _iterations; index++) {

                using Transaction transaction = new(Databases.TestDatabase);

                NonQueryResult result = _preparedUpdateQuery.Execute(parameters: this, transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Dynamic_Update() {

            for(int index = 0; index < _iterations; index++) {

                Test01Table table = Test01Table.Instance;

                using Transaction transaction = new(Databases.TestDatabase);

                NonQueryResult result = Query
                    .Update(table)
                    .Values(values => values
                        .Set(table.Message, "New Message")
                        .Set(table.Date, DateTime.Now)
                    )
                    .Where(table.Row_guid == _guid)
                    .Execute(transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Repository_Update() {

            for(int index = 0; index < _iterations; index++) {

                Test01RowRepository repository = new();

                repository
                    .SelectRows
                    .Where(repository.Table.Row_guid == _guid)
                    .Execute(Databases.TestDatabase);

                Test01Row row = repository[0];

                row.Message = row.Message == _message1 ? _message2 : _message2; //Alternate messages
                row.Date = DateTime.Now;

                repository.SaveChanges(Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void EF_Core_Single_Row_Update() {

            for(int index = 0; index < _iterations; index++) {

                using TestContext context = new(Databases.ConnectionString);

                Test01Row_EfCore row = context
                    .TestRows
                    .Where(test => test.Row_guid == _guid)
                    .First();

                row.Message = row.Message == _message1 ? _message2 : _message2; //Alternate messages
                row.Date = DateTime.Now.ToUniversalTime();

                context.Update(row);
                context.SaveChanges();
            }
        }
    }
}