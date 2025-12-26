using BenchmarkDotNet.Attributes;
using Benchmarks.Classes;
using Benchmarks.Tables;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using QueryLite;

namespace Benchmarks {

    [MemoryDiagnoser]
    public class SelectTenRowAsyncBenchmarks {

        private readonly string _message = "this is my new message";
        private readonly DateTime _date = DateTime.Now;

        private IPreparedQueryExecute<SelectTenRowAsyncBenchmarks, Test01> _preparedSelectQuery;

        public SelectTenRowAsyncBenchmarks() {

            Test01Table table = Test01Table.Instance;

            _preparedSelectQuery = Query
                .Prepare<SelectTenRowAsyncBenchmarks>()
                .Select(
                    row => new Test01(table, row)
                )
                .From(table)
                .Build();

            _preparedSelectQuery.Initialize(Databases.TestDatabase);
        }

        [IterationSetup]
        public void Setup() {

            Databases.ResetTable();

            Test01Table table = Test01Table.Instance;

            using(Transaction transaction = new Transaction(Databases.TestDatabase)) {

                Query.Truncate(table).Execute(transaction);

                for(int index = 0; index < 10; index++) {

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

        private int _iterations = 2000;

        [Benchmark]
        public async Task Ado_Ten_Row_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

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
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task Dapper_Ten_Row_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    await using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

                    await connection.OpenAsync();

                    IEnumerable<Test01> result = await connection.QueryAsync<Test01>(sql: "SELECT id,row_guid,message,date FROM Test01");
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task QueryLite_Ten_Row_Prepared_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    QueryResult<Test01> result = await _preparedSelectQuery.ExecuteAsync(parameters: this, Databases.TestDatabase, CancellationToken.None);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task QueryLite_Ten_Row_Dynamic_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    Test01Table table = Test01Table.Instance;

                    QueryResult<Test01> result = await Query
                        .Select(
                            row => new Test01(table, row)
                        )
                        .From(table)
                        .ExecuteAsync(Databases.TestDatabase);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task QueryLite_Ten_Row_Repository_SelectAsync() {

            List<Task> tasks = new List<Task>(_iterations);

            for(int index = 0; index < _iterations; index++) {

                Task task = Task.Run(async () => {

                    Test01RowRepository repository = new Test01RowRepository();

                    await repository
                        .SelectRows
                        .ExecuteAsync(Databases.TestDatabase, CancellationToken.None);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task EF_Core_Ten_Row_SelectAsync() {

            using(TestContext context = new TestContext(Databases.ConnectionString)) {

                List<Task> tasks = new List<Task>(_iterations);

                for(int index = 0; index < _iterations; index++) {

                    Task task = Task.Run(async () => {

                        List<Test01Row_EfCore> result = await context.TestRows.ToListAsync(CancellationToken.None);
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
            }
        }
    }
}