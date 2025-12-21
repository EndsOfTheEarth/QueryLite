using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Benchmarks.Tables;
using Dapper;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectOneThousandRowAsyncBenchmarks {

        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedQueryExecute<SelectOneThousandRowAsyncBenchmarks, Test01> _preparedSelectQuery;

        public SelectOneThousandRowAsyncBenchmarks() {

            Tables.Test01Table table = Tables.Test01Table.Instance;

            _preparedSelectQuery = Query
                .Prepare<SelectOneThousandRowAsyncBenchmarks>()
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

            using(Transaction transaction = new Transaction(Databases.TestDatabase)) {

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

        private const int _iterations = 2000;

        [Benchmark]
        public async Task Ado_One_Thousand_Row_SelectAsync() {

            SemaphoreSlim _semaphore = new SemaphoreSlim(0, _iterations);

            for(int index = 0; index < _iterations; index++) {

                _ = Task.Run(async () => {

                    await using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                    await connection.OpenAsync().ConfigureAwait(false);

                    using NpgsqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT id,row_guid,message,date FROM Test01";

                    await using NpgsqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                    List<Test01> list = new List<Test01>();

                    while(await reader.ReadAsync().ConfigureAwait(false)) {

                        list.Add(
                            new Test01(
                                id: reader.GetInt32(0),
                                row_guid: reader.GetGuid(1),
                                message: reader.GetString(2),
                                date: reader.GetDateTime(3)
                            )
                        );
                    }
                    _semaphore.Release();
                });
            }

            for(int index = 0; index < _iterations; index++) {
                await _semaphore.WaitAsync();
            }
        }

        [Benchmark]
        public async Task Dapper_One_Thousand_Row_SelectAsync() {

            SemaphoreSlim _semaphore = new SemaphoreSlim(0, _iterations);

            for(int index = 0; index < _iterations; index++) {

                _ = Task.Run(async () => {

                    await using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                    await connection.OpenAsync();

                    IEnumerable<Test01>? result = await connection.QueryAsync<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01");
                    _semaphore.Release();
                });
            }
            for(int index = 0; index < _iterations; index++) {
                await _semaphore.WaitAsync();
            }
        }

        [Benchmark]
        public async Task QueryLite_One_Thousand_Row_Prepared_SelectAsync() {

            SemaphoreSlim _semaphore = new SemaphoreSlim(0, _iterations);

            for(int index = 0; index < _iterations; index++) {

                _ = Task.Run(async () => {

                    QueryResult<Test01> result = await _preparedSelectQuery.ExecuteAsync(parameters: this, Databases.TestDatabase);
                    _semaphore.Release();
                });
            }
            for(int index = 0; index < _iterations; index++) {
                await _semaphore.WaitAsync();
            }
        }

        [Benchmark]
        public async Task QueryLite_One_Thousand_Row_Dynamic_SelectAsync() {

            SemaphoreSlim _semaphore = new SemaphoreSlim(0, _iterations);

            for(int index = 0; index < _iterations; index++) {

                _ = Task.Run(async () => {

                    Tables.Test01Table table = Tables.Test01Table.Instance;

                    QueryResult<Test01> result = await Query
                        .Select(
                            row => new Test01(table, row)
                        )
                        .From(table)
                        .ExecuteAsync(Databases.TestDatabase);

                    _semaphore.Release();
                });
            }
            for(int index = 0; index < _iterations; index++) {
                await _semaphore.WaitAsync();
            }
        }

        [Benchmark]
        public async Task QueryLite_One_Thousand_Row_Repository_SelectAsync() {

            SemaphoreSlim _semaphore = new SemaphoreSlim(0, _iterations);

            for(int index = 0; index < _iterations; index++) {

                _ = Task.Run(async () => {

                    Test01RowRepository repository = new Test01RowRepository();

                    repository.SelectRows.Execute(Databases.TestDatabase);

                    _semaphore.Release();
                });
            }
            for(int index = 0; index < _iterations; index++) {
                await _semaphore.WaitAsync();
            }
        }
    }
}