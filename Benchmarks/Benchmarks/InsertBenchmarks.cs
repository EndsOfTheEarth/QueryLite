using BenchmarkDotNet.Attributes;
using Benchmarks.Tables;
using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class InsertBenchmarks {

        private IPreparedInsertQuery<InsertBenchmarks> _preparedInsertQuery;

        public InsertBenchmarks() {

            Test01Table table = Test01Table.Instance;

            _preparedInsertQuery = Query
                .Prepare<InsertBenchmarks>()
                .Insert(table)
                .Values(values => values
                    .Set(table.Row_guid, info => info._guid)
                    .Set(table.Message, info => info._message)
                    .Set(table.Date, info => info._date)
                )
                .Build();

            _preparedInsertQuery.Initialize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Test01Table table = Test01Table.Instance;

            using(Transaction transaction = new Transaction(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                transaction.Commit();
            }
            Query.ExecuteNonQuery(sql: $"VACUUM {table.SchemaName}.{table.TableName};", Databases.TestDatabase);
        }

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private int _iterations = 2000;

        [Benchmark]
        public void Ado_Single_Insert() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                using NpgsqlTransaction transaction = connection.BeginTransaction();

                using NpgsqlCommand command = connection.CreateCommand();

                command.CommandText = "INSERT INTO Test01 (row_guid,message,date) VALUES(@0, @1, @2)";

                command.Transaction = transaction;

                command.Parameters.Add(new NpgsqlParameter(parameterName: "@0", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = _guid });
                command.Parameters.Add(new NpgsqlParameter(parameterName: "@1", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = _message });
                command.Parameters.Add(new NpgsqlParameter(parameterName: "@2", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = _date });

                int rows = command.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        [Benchmark]
        public void Dapper_Single_Insert() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                using NpgsqlTransaction transaction = connection.BeginTransaction();

                var parameters = new { A = _guid, B = _message, C = _date };

                int rows = connection.Execute(sql: "INSERT INTO Test01 (row_guid,message,date) VALUES(@A, @B, @C)", parameters, transaction: transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Compiled_Insert() {

            for(int index = 0; index < _iterations; index++) {

                using Transaction transaction = new Transaction(Databases.TestDatabase);

                NonQueryResult result = _preparedInsertQuery.Execute(parameters: this, transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Dynamic_Insert() {

            for(int index = 0; index < _iterations; index++) {

                Test01Table table = Test01Table.Instance;

                using Transaction transaction = new Transaction(Databases.TestDatabase);

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

        [Benchmark]
        public void QueryLite_Single_Repository_Insert() {

            for(int index = 0; index < _iterations; index++) {

                Test01RowRepository repository = new Test01RowRepository();

                Test01Row row = new Test01Row(
                    id: 0,
                    row_guid: _guid,
                    message: _message,
                    date: _date
                );
                repository.AddNewRow(row);

                using Transaction transaction = new Transaction(Databases.TestDatabase);

                repository.Update(transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void QueryLite_Single_Repository_Static_Insert() {

            for(int index = 0; index < _iterations; index++) {

                Test01Row row = new Test01Row(
                    id: 0,
                    row_guid: _guid,
                    message: _message,
                    date: _date
                );

                using Transaction transaction = new Transaction(Databases.TestDatabase);

                Test01RowRepository.ExecuteInsert(row, Test01Table.Instance, transaction);

                transaction.Commit();
            }
        }

        [Benchmark]
        public void EF_Core_Single_Insert() {

            for(int index = 0; index < _iterations; index++) {

                using TestContext context = new TestContext(Databases.ConnectionString);

                Test01Row_EfCore row = new Test01Row_EfCore(
                    id: 0,
                    row_guid: _guid,
                    message: _message,
                    date: _date.ToUniversalTime()
                );

                using IDbContextTransaction transaction = context.Database.BeginTransaction();

                context.TestRows.Add(row);

                context.SaveChanges();

                transaction.Commit();
            }
        }
    }
}