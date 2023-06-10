# Prepared Queries

Query lite has two types of queries, `dynamic` and `prepared`.

Prepared queries generate their sql query string once and reused that object on every execution. Whereas dynamic queries will generate a new sql query on every execution.

These are the pros and cons of the two query types

## Dynamic Queries

Pros:

* Simpiler C# syntax
    * Type safe queries
* Allow for dynanic condtions
    * i.e. The `WHERE` clause can change between executions
* Supports query features that are dynamic in nature
    * e.g. in list `IN(@0,@1,@3....)` (As the number of items in the list can change)
* High performance loading results when returning a list of objects
    * Equivalent to a direct ado.net query (That loads to a list)

Cons:

* More memory allocations
    - Generating the sql query on each execution requires additional memory allocations compared to using a static sql string

## Prepared Queries

Pros:

* Type safe queries
* High performance
    * Very similar cpu and memory allocation as using direct ado.net queries

Cons:

* Syntax is more difficult to understand and can look a bit messey for complicated queries
    * The C# generics required to make prepared queries type safe add complexity to the syntax
    * C# currently does not support generic parameters for custom operators. This means condition syntax (e.g. the `WHERE` clause) cannot use operators like `==` and `!=`
* Some query features are not supported
    * e.g. In list `IN(@0, @1, @2)` as the number of items in the list can change between executions


# What Query Type Is Best?

Generally, dynamic queries are a better choice as the C# syntax is easier to write and understand. In general a dynamic query will have equivalent or better performance to Dapper when returning a list of objects as the result. Query Lite is more memory efficient in loading result classes than dapper and that often balances out the initial overhead of dynamically generating a query.

If a query is on the 'hot path' and you want to minimise memory allocations then use a prepared query. As a general rule, prepared queries will use almost the equivalent CPU and memory allocation as a direct ado.net query.

## Benchmarks

Benchmarks against PostgreSql are located in the Benchmarks project.

Here are some results:

### Select Single Row

Running the query `SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0` and returning a single row to a List<>.

Example Ado Code:

```C#
private int _iterations = 2000;

[Benchmark]
public void Ado_Ten_Row_Select() {

    for(int index = 0; index < _iterations; index++) {

        using NpgsqlConnection connection = new NpgsqlConnection(Databases.ConnectionString);

        connection.Open();

        using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = "SELECT id,row_guid,message,date FROM Test01";

        using NpgsqlDataReader reader = command.ExecuteReader();

        List<Test01> list = new List<Test01>();

        while(reader.Read()) {

            list.Add(
                new Test01(
                    id: reader.GetInt32(0),
                    row_guid: reader.GetGuid(1),
                    message: reader.GetString(2),
                    date: reader.GetDateTime(3)
                )
            );
        }
    }
}
public sealed class Test01 {

    public Test01(int id, Guid row_guid, string message, DateTime date) {
        Id = id;
        Row_guid = row_guid;
        Message = message;
        Date = date;
    }
    public int Id { get; set; }
    public Guid Row_guid { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
```


|                               Method |     Mean |   Error |  StdDev | Allocated | Total Queries |
|------------------------------------- |---------:|--------:|--------:|----------:|----------: |
|                Ado_Single_Row_Select | 122.7 ms | 2.43 ms | 3.79 ms |   3.13 MB | 2000 queries |
|             Dapper_Single_Row_Select | 123.9 ms | 2.41 ms | 2.96 ms |   3.88 MB | 2000 queries |
| QueryLite_Single_Row_Prepared_Select | 121.2 ms | 2.23 ms | 1.98 ms |   3.16 MB | 2000 queries |
|  QueryLite_Single_Row_Dynamic_Select | 123.4 ms | 1.25 ms | 1.10 ms |   4.23 MB | 2000 queries |

* `Ado_Single_Row_Select` = Direct ado query
* `Dapper_Single_Row_Select` = Dapper query
* `QueryLite_Single_Row_Prepared_Select` = Prepared query
* `QueryLite_Single_Row_Dynamic_Select` = Dynamic query

### Select Ten Rows

Running the query `SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0` and returning tens rows to a List<>.

|                            Method |     Mean |   Error |  StdDev |   Median | Allocated | Total Queries |
|---------------------------------- |---------:|--------:|--------:|---------:|----------:|----------: |
|                Ado_Ten_Row_Select | 107.3 ms | 2.11 ms | 2.67 ms | 105.8 ms |   5.22 MB | 2000 queries |
|             Dapper_Ten_Row_Select | 115.4 ms | 2.29 ms | 3.05 ms | 114.5 ms |   7.31 MB | 2000 queries |
| QueryLite_Ten_Row_Prepared_Select | 109.4 ms | 2.15 ms | 3.35 ms | 107.6 ms |   5.25 MB | 2000 queries |
|  QueryLite_Ten_Row_Dynamic_Select | 110.0 ms | 1.66 ms | 1.56 ms | 109.5 ms |   6.09 MB | 2000 queries |


### Select One Hundred Rows

Running the query `SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0` and returning one hundred rows to a List<>.

|                                    Method |     Mean |   Error |  StdDev |      Gen0 |      Gen1 | Allocated | Total Queries |
|------------------------------------------ |---------:|--------:|--------:|----------:|----------:|----------:|----------: |
|                Ado_One_Hundred_Row_Select | 152.4 ms | 1.30 ms | 1.22 ms | 1000.0000 | 1000.0000 |  30.75 MB | 2000 queries |
|             Dapper_One_Hundred_Row_Select | 199.8 ms | 0.96 ms | 0.80 ms | 2000.0000 | 1000.0000 |  46.57 MB | 2000 queries |
| QueryLite_One_Hundred_Row_Prepared_Select | 157.5 ms | 0.51 ms | 0.45 ms | 1000.0000 | 1000.0000 |  30.78 MB | 2000 queries |
|  QueryLite_One_Hundred_Row_Dynamic_Select | 161.0 ms | 1.72 ms | 1.53 ms | 1000.0000 | 1000.0000 |  31.62 MB | 2000 queries |


### Select One Thousand Rows

Running the query `SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0` and returning one thousand rows to a List<>.

|                                     Method |     Mean |   Error |  StdDev |       Gen0 |      Gen1 | Allocated | Total Queries |
|------------------------------------------- |---------:|--------:|--------:|-----------:|----------:|----------:|----------: |
|                Ado_One_Thousand_Row_Select | 474.1 ms | 1.18 ms | 1.10 ms | 17000.0000 | 5000.0000 | 277.95 MB | 2000 queries |
|             Dapper_One_Thousand_Row_Select | 807.4 ms | 1.85 ms | 1.73 ms | 27000.0000 | 8000.0000 | 431.11 MB | 2000 queries |
| QueryLite_One_Thousand_Row_Prepared_Select | 474.1 ms | 1.51 ms | 1.26 ms | 17000.0000 | 8000.0000 | 277.99 MB | 2000 queries |
|  QueryLite_One_Thousand_Row_Dynamic_Select | 477.5 ms | 1.75 ms | 1.55 ms | 17000.0000 | 8000.0000 | 278.82 MB | 2000 queries |