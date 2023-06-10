﻿using BenchmarkDotNet.Attributes;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class DeleteSingleRowBenchmarks {

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedDeleteQuery<Guid> _preparedDeleteQuery;

        public DeleteSingleRowBenchmarks() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedDeleteQuery = Query
                .Prepare<Guid>()
                .Delete(table)
                .Where(where => where.EQUALS(table.Row_guid, guid => guid))
                .Build();

            _preparedDeleteQuery.Initialize(Databases.TestDatabase);
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
        public void Ado_Single_Row_Delete() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                using NpgsqlTransaction transaction = connection.BeginTransaction();

                using NpgsqlCommand command = connection.CreateCommand();

                command.Transaction = transaction;

                command.CommandText = "DELETE FROM Test01 WHERE row_guid=@0";

                command.Parameters.Add(new NpgsqlParameter(parameterName: "@0", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = _guid });

                int rows = command.ExecuteNonQuery();

                if(rows != 1) {
                    throw new Exception();
                }
                transaction.Rollback(); //Roll back so we can run iterations
            }
        }

        [Benchmark]
        public void Dapper_Single_Row_Delete() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                connection.Open();

                using NpgsqlTransaction transaction = connection.BeginTransaction();

                int rows = connection.Execute(sql: "DELETE FROM Test01 WHERE row_guid=@row_guid", new { row_guid = _guid }, transaction: transaction);

                if(rows != 1) {
                    throw new Exception();
                }
                transaction.Rollback(); //Roll back so we can run iterations
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Prepared_Delete() {

            for(int index = 0; index < _iterations; index++) {

                using Transaction transaction = new Transaction(Databases.TestDatabase);

                NonQueryResult result = _preparedDeleteQuery.Execute(parameters: _guid, transaction);

                if(result.RowsEffected != 1) {
                    throw new Exception();
                }
                transaction.Rollback(); //Roll back so we can run iterations
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Dynamic_Delete() {

            for(int index = 0; index < _iterations; index++) {

                Tables.Test01Table table = Tables.Test01Table.Instance;

                using Transaction transaction = new Transaction(Databases.TestDatabase);

                NonQueryResult result = Query
                    .Delete(table)
                    .Where(table.Row_guid == _guid)
                    .Execute(transaction);

                if(result.RowsEffected != 1) {
                    throw new Exception();
                }
                transaction.Rollback(); //Roll back so we can run iterations
            }
        }
    }
}