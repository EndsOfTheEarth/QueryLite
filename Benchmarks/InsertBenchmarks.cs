using BenchmarkDotNet.Attributes;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class InsertBenchmarks {

        private IPreparedInsertQuery<InsertBenchmarks> _preparedInsertQuery;

        public InsertBenchmarks() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedInsertQuery = Query
                .PreparedInsert<InsertBenchmarks>(table)
                .Values(values => values
                    .Set(table.Row_guid, info => info._guid)
                    .Set(table.Message, info => info._message)
                    .Set(table.Date, info => info._date)
                )
                .Build();

            _preparedInsertQuery.Initilize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            using(QueryLite.Transaction transaction = new QueryLite.Transaction(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                transaction.Commit();
            }
        }

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        [Benchmark]
        public void Ado_Single_Insert() {

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

        [Benchmark]
        public void Dapper_Single_Insert() {

            using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

            connection.Open();

            using NpgsqlTransaction transaction = connection.BeginTransaction();

            var parameters = new { A = _guid, B = _message, C = _date };

            int rows = connection.Execute(sql: "INSERT INTO Test01 (row_guid,message,date) VALUES(@A, @B, @C)", parameters, transaction: transaction);

            transaction.Commit();
        }

        [Benchmark]
        public void QueryLite_Single_Compiled_Insert() {

            using QueryLite.Transaction transaction = new Transaction(Databases.TestDatabase);

            NonQueryResult result = _preparedInsertQuery.Execute(parameters: this, transaction);

            transaction.Commit();
        }

        [Benchmark]
        public void QueryLite_Single_Dynamic_Insert() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            using QueryLite.Transaction transaction = new Transaction(Databases.TestDatabase);

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
}