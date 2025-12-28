using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Benchmarks.Tables;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectSingleRowBenchmarks {

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private readonly IPreparedQueryExecute<SelectSingleRowBenchmarks, Test01> _preparedSelectQuery;

        public SelectSingleRowBenchmarks() {

            Test01Table table = Test01Table.Instance;

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

            Databases.ResetTable();

            Test01Table table = Test01Table.Instance;

            using(Transaction transaction = new(Databases.TestDatabase)) {

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

        private readonly int _iterations = 2000;

        [Benchmark]
        public void Ado_Single_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new(Databases.ConnectionString);

                connection.Open();

                using NpgsqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0";

                command.Parameters.Add(new NpgsqlParameter(parameterName: "@0", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = _guid });

                using NpgsqlDataReader reader = command.ExecuteReader();

                reader.Read();

                Test01 test01 = new Test01(
                    id: reader.GetInt32(0),
                    row_guid: reader.GetGuid(1),
                    message: reader.GetString(2),
                    date: reader.GetDateTime(3)
                );
            }
        }

        [Benchmark]
        public void Dapper_Single_Row_Select() {

            for(int index = 0; index < _iterations; index++) {

                using NpgsqlConnection connection = new(Databases.ConnectionString);

                connection.Open();

                Test01 result = connection.QuerySingle<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@_guid", new { _guid });
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Prepared_Select() {

            for(int index = 0; index < _iterations; index++) {

                Test01? result = _preparedSelectQuery.SingleOrDefault(parameters: this, Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Dynamic_Select() {

            for(int index = 0; index < _iterations; index++) {

                Test01Table table = Test01Table.Instance;

                Test01? result = Query
                    .Select(
                        row => new Test01(table, row)
                    )
                    .From(table)
                    .Where(table.Row_guid == _guid)
                    .SingleOrDefault(Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void QueryLite_Single_Row_Repository_Select() {

            for(int index = 0; index < _iterations; index++) {

                Test01RowRepository repository = new();

                repository
                    .SelectRows
                    .Where(repository.Table.Row_guid == _guid)
                    .Execute(Databases.TestDatabase);
            }
        }

        [Benchmark]
        public void EF_Core_Single_Row_Select() {

            using TestContext context = new(Databases.ConnectionString);

            for(int index = 0; index < _iterations; index++) {

                Test01Row_EfCore? result = context.TestRows
                    .Select(row => row)
                    .Where(row => row.Row_guid == _guid)
                    .SingleOrDefault();
            }
        }
    }
}