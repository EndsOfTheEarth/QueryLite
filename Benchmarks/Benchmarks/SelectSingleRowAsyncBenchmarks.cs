using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Benchmarks.Tables;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectSingleRowAsyncBenchmarks {

        private readonly Guid _guid = new Guid("{A94E044C-CDE2-40E2-9A81-5803AFB746A2}");
        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedQueryExecute<SelectSingleRowAsyncBenchmarks, Test01> _preparedSelectQuery;

        public SelectSingleRowAsyncBenchmarks() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedSelectQuery = Query
                .Prepare<SelectSingleRowAsyncBenchmarks>()
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
        public async Task Ado_Single_Row_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async() => {

                    await using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                    await connection.OpenAsync();

                    using NpgsqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0";

                    command.Parameters.Add(new NpgsqlParameter(parameterName: "@0", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = _guid });

                    await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    await reader.ReadAsync();

                    List<Test01> list = new List<Test01>();

                    Test01 test01 = new Test01(
                        id: reader.GetInt32(0),
                        row_guid: reader.GetGuid(1),
                        message: reader.GetString(2),
                        date: reader.GetDateTime(3)
                    );
                    list.Add(test01);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task Dapper_Single_Row_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    await using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                    await connection.OpenAsync();

                    IEnumerable<Test01> result = await connection.QueryAsync<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@_guid", new { _guid });
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task QueryLite_Single_Row_Prepared_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    QueryResult<Test01> result = await _preparedSelectQuery.ExecuteAsync(parameters: this, Databases.TestDatabase);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task QueryLite_Single_Row_Dynamic_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    Tables.Test01Table table = Tables.Test01Table.Instance;

                    QueryResult<Test01> result = await Query
                        .Select(
                            row => new Test01(table, row)
                        )
                        .From(table)
                        .Where(table.Row_guid == _guid)
                        .ExecuteAsync(Databases.TestDatabase);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task QueryLite_Single_Row_Repository_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    Test01RowRepository repository = new Test01RowRepository();

                    repository
                        .SelectRows
                        .Where(repository.Table.Row_guid == _guid)
                        .Execute(Databases.TestDatabase);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }
    }
}