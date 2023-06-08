using BenchmarkDotNet.Attributes;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class UpdateSingleRowBenchmarks {

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        //private IPreparedQueryExecute<UpdateSingleRowBenchmarks, Test01> _preparedUpdateQuery;

        public UpdateSingleRowBenchmarks() {

            //Tables.Test01Table table = Tables.Test01Table.Instance;

            //_preparedUpdateQuery = Query
            //    .PrepareWithParameters<UpdateSingleRowBenchmarks>()
            //    .Select(
            //        row => new Test01(table, row)
            //    )
            //    .From(table)
            //    .Where(where => where.EQUALS(table.Row_guid, info => info._guid))
            //    .Build();

            //_preparedUpdateQuery.Initialize(Databases.TestDatabase);
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

        [Benchmark]
        public void Ado_Single_Row_Update() {

            using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

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

        [Benchmark]
        public void Dapper_Single_Row_Update() {

            using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

            connection.Open();

            using NpgsqlTransaction transaction = connection.BeginTransaction();

            var parameters = new { message = "New Message", date = DateTime.Now, row_guid = _guid };

            int rows = connection.Execute(sql: "UPDATE Test01 SET message=@message,date=@date WHERE row_guid=@row_guid", parameters, transaction: transaction);

            transaction.Commit();
        }

        //[Benchmark]
        //public void QueryLite_Single_Row_Prepared_Select() {

        //    QueryResult<Test01> result = _preparedSelectQuery.Execute(parameterValues: this, Databases.TestDatabase);
        //}

        [Benchmark]
        public void QueryLite_Single_Row_Dynamic_Update() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            using Transaction transaction = new Transaction(Databases.TestDatabase);

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
}