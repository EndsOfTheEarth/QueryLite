using BenchmarkDotNet.Attributes;
using Benchmarks.Tables;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class UpdateSingleRowBenchmarks {

        public int Id { get; private set; }
        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private readonly IPreparedUpdateQuery<UpdateSingleRowBenchmarks> _preparedUpdateQuery;

        public UpdateSingleRowBenchmarks() {

            Test01Table table = Test01Table.Instance;

            _preparedUpdateQuery = Query
                .Prepare<UpdateSingleRowBenchmarks>()
                .Update(table)
                .Values(values => values
                    .Set(table.Message, info => "New Message")
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
                        .Set(table.Message, _message)
                        .Set(table.Date, _date)
                    )
                    .Execute(returning => returning.Get(table.Id), transaction);

                Id = result.Rows[0];
                transaction.Commit();
            }
        }

        private int _iterations = 2000;

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
                command.Parameters.Add(new NpgsqlParameter(parameterName: "@1", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = "New Message" });
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

                var parameters = new { message = "New Message", date = DateTime.Now, row_guid = _guid };

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

                using Transaction transaction = new(Databases.TestDatabase);

                repository
                    .SelectRows
                    .Where(repository.Table.Row_guid == _guid)
                    .Execute(Databases.TestDatabase);

                Test01Row row = repository[0];

                row.Message = "New Message";
                row.Date = DateTime.Now;

                repository.Update(transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Repository_Static_Update() {

            for(int index = 0; index < _iterations; index++) {

                using Transaction transaction = new(Databases.TestDatabase);

                Test01Row row = new(
                    id: Id,
                    row_guid: _guid,
                    message: "New Message",
                    date: DateTime.Now
                );

                Test01RowRepository.ExecuteUpdate(row, Test01Table.Instance, transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void EF_Core_Single_Row_Update() {

            Test01Row_EfCore row = new(
                id: Id,
                row_guid: _guid,
                message: "New Message",
                date: DateTime.Now.ToUniversalTime()
            );

            for(int index = 0; index < _iterations; index++) {

                using TestContext context = new(Databases.ConnectionString);

                using IDbContextTransaction transaction = context.Database.BeginTransaction();

                row.Date = DateTime.Now.ToUniversalTime();

                context.Update(row);

                context.SaveChanges();

                transaction.Commit();
            }
        }
    }
}