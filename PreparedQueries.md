# Prepared Queries

Query lite has two types of queries, `dynamic` and `prepared`.

Prepared queries generate their sql query string once and reuse that object for every execution. Whereas dynamic queries generate a new sql query for every execution.

These are the pros and cons of the two query types:

## Dynamic Queries

Pros:

* Simpiler C# syntax
* Allow for dynanic condtions
    * i.e. The `WHERE` clause can change between executions. Good for filtering and reporting queries that have customizable `WHERE` clause criteria.
* Supports sql query features that are dynamic in nature
    * e.g. in list `IN(@0,@1,@3....)` (As the number of items in the list can change)
* High performance loading results
    * Equivalent to a direct ado.net query

Cons:

* Allocates more memory per query
    - The sql query is generated for every execution and this means more memory allocation
    - Note: Select query memory allocation is still generally competitive with Dapper and EF core. Total memory allocations per query can be lower when more than one row is returned, depending on the query complexity and columns selected.

## Prepared Queries

Pros:

* High performance
    * Very similar cpu and memory allocation as using direct ado.net queries

Cons:

* Syntax is more complicated
    * The language generic constraints required to make prepared queries type safe add complexity to the syntax
    * C# currently does not support generic parameters for custom operators. This means condition syntax (e.g. the `WHERE` clause) cannot use operators like `==` and `!=`
* Some query features are not supported as they are dynamic in nature
    * e.g. In list `IN(@0, @1, @2)` as the number of items in the list can change between executions


# What Query Type Is Best?

Dynamic queries are a good choice as the C# syntax is easier to read and understand. In general a dynamic queries have good performance.

Prepared queries have performance and memory allocation characteristics near equivalent to direct ado.net. So, if a query is on the 'hot path' and memory allocation pressure on the garbage collector is an issue, then use a prepared query.

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


## The Prepare Clause

TODO:

## Prepared Conditions

TODO:

## Prepared Select Query Example


```C#
public sealed class ProductCostHandler {

    /*
     *  The prepared query is defined to receive a parameter of type 'IntKey<IProduct>' on every call
     *  to execute query and return 'rows' of the type 'ProductCostHistory'
     */
    private readonly static IPreparedQueryExecute<IntKey<IProduct>, ProductCostHistory> _loadCostHistoryQuery;

    static ProductCostHandler() {

        /*
         * Create the prepared query is the static constructor
         */
        ProductTable productTable = ProductTable.Instance;
        ProductCostHistoryTable costHistoryTable = ProductCostHistoryTable.Instance;

        _loadCostHistoryQuery = Query
            .Prepare<IntKey<IProduct>>()        //This line defines that IntKey<IProduct> is a query parameter type.
            .Select(
                row => new ProductCostHistory(
                    productID: row.Get(productTable.ProductID),
                    startDate: row.Get(costHistoryTable.StartDate),
                    endDate: row.Get(costHistoryTable.EndDate),
                    standardCost: row.Get(costHistoryTable.StandardCost),
                    modifiedDate: row.Get(costHistoryTable.ModifiedDate)
                )
            )
            .From(productTable)
            .Join(costHistoryTable).On(on => on.EQUALS(productTable.ProductID, costHistoryTable.ProductID))
            .Where(where => where.EQUALS(productTable.ProductID, id => id))    //The function id => id returns the provided 'IntKey<IProduct>' for the underlying sql parameter
            .OrderBy(costHistoryTable.StartDate, costHistoryTable.EndDate)
            .Build();
    }

    private readonly IDatabase _database;

    public ProductCostHandler(IDatabase database) {
        _database = database;
    }

    public async Task<IList<ProductCostHistory>> LoadProductCostHistory(IntKey<IProduct> productId, CancellationToken cancellationToken) {

        /*
         *  Execute the prepared query and passing 'productId' in as a parameter
         */
        QueryResult<ProductCostHistory> result = await _loadCostHistoryQuery.ExecuteAsync(parameters: productId, _database, cancellationToken);

        return result.Rows;
    }
}
```

## Prepared Insert Query Example

```C#
public sealed class AddProductHandler {

    private readonly static IPreparedInsertQuery<Product, IntKey<IProduct>> _insertProductQuery;

    /*
     * Create prepared insert query in the static constructor
     */
    static AddProductHandler() {

        ProductTable table = ProductTable.Instance;

        _insertProductQuery = Query
            .Prepare<Product>() //Product is the defined parameter object that is used to populate the sql values
            .Insert(table)
            .Values(values => values
                .Set(table.Name, p => p.Name)
                .Set(table.ProductNumber, p => p.ProductNumber)
                .Set(table.MakeFlag, p => p.MakeFlag)
                .Set(table.FinishedGoodsFlag, p => p.FinishedGoodsFlag)
                .Set(table.Color, p => p.Color)
                .Set(table.SafetyStockLevel, p => p.SafetyStockLevel)
                .Set(table.ReorderPoint, p => p.ReorderPoint)
                .Set(table.StandardCost, p => p.StandardCost)
                .Set(table.ListPrice, p => p.ListPrice)
                .Set(table.Size, p => p.Size)
                .Set(table.SizeUnitMeasureCode, p => p.SizeUnitMeasureCode)
                .Set(table.WeightUnitMeasureCode, p => p.WeightUnitMeasureCode)
                .Set(table.Weight, p => p.Weight)
                .Set(table.DaysToManufacture, p => p.DaysToManufacture)
                .Set(table.ProductLine, p => p.ProductLine)
                .Set(table.Class, p => p.Class)
                .Set(table.Style, p => p.Style)
                .Set(table.ProductSubcategoryID, p => p.ProductSubcategoryID)
                .Set(table.ProductModelID, p => p.ProductModelID)
                .Set(table.SellStartDate, p => p.SellStartDate)
                .Set(table.SellEndDate, p => p.SellEndDate)
                .Set(table.DiscontinuedDate, p => p.DiscontinuedDate)
                .Set(table.Rowguid, p => p.Rowguid)
                .Set(table.ModifiedDate, p => p.ModifiedDate)
            )
            .Build(
                inserted => inserted.Get(table.ProductID)    //Return auto generated id
            );
    }

    private readonly IDatabase _database;

    public AddProductHandler(IDatabase database) {
        _database = database;
    }

    public async Task<IntKey<IProduct>> AddProduct(Product product, CancellationToken cancellationToken) {
        
        /*
         *  Execute the prepared query and passing 'product' in as a parameter
         */
        using Transaction transaction = new Transaction(_database);

        QueryResult<IntKey<IProduct>> result = await _insertProductQuery.ExecuteAsync(parameters: product, transaction, cancellationToken);

        transaction.Commit();

        IntKey<IProduct> productId = result.Rows[0];

        return productId;
    }
}
```

## Prepared Update Query Example

```C#
public class UpdateProductHandler {

    private readonly static IPreparedUpdateQuery<Product> _updateProductQuery;

    /*
     * Create prepared update query in the static constructor
     */
    static UpdateProductHandler() {

        ProductTable table = ProductTable.Instance;

        _updateProductQuery = Query
            .Prepare<Product>() // Product is the defined parameter object that is used to populate the sql values
            .Update(table)
            .Values(values => values
                .Set(table.Name, p => p.Name)
                .Set(table.ProductNumber, p => p.ProductNumber)
                .Set(table.MakeFlag, p => p.MakeFlag)
                .Set(table.FinishedGoodsFlag, p => p.FinishedGoodsFlag)
                .Set(table.Color, p => p.Color)
                .Set(table.SafetyStockLevel, p => p.SafetyStockLevel)
                .Set(table.ReorderPoint, p => p.ReorderPoint)
                .Set(table.StandardCost, p => p.StandardCost)
                .Set(table.ListPrice, p => p.ListPrice)
                .Set(table.Size, p => p.Size)
                .Set(table.SizeUnitMeasureCode, p => p.SizeUnitMeasureCode)
                .Set(table.WeightUnitMeasureCode, p => p.WeightUnitMeasureCode)
                .Set(table.Weight, p => p.Weight)
                .Set(table.DaysToManufacture, p => p.DaysToManufacture)
                .Set(table.ProductLine, p => p.ProductLine)
                .Set(table.Class, p => p.Class)
                .Set(table.Style, p => p.Style)
                .Set(table.ProductSubcategoryID, p => p.ProductSubcategoryID)
                .Set(table.ProductModelID, p => p.ProductModelID)
                .Set(table.SellStartDate, p => p.SellStartDate)
                .Set(table.SellEndDate, p => p.SellEndDate)
                .Set(table.DiscontinuedDate, p => p.DiscontinuedDate)
                .Set(table.Rowguid, p => p.Rowguid)
                .Set(table.ModifiedDate, p => p.ModifiedDate)
            )
            .Where(where => where.EQUALS(table.ProductID, p => p.ProductID))
            .Build();
    }

    private readonly IDatabase _database;

    public UpdateProductHandler(IDatabase database) {
        _database = database;
    }

    public async Task UpdateProduct(Product product, CancellationToken cancellationToken) {

        using Transaction transaction = new Transaction(_database);

        NonQueryResult result = await _updateProductQuery.ExecuteAsync(parameters: product, transaction, cancellationToken);

        if(result.RowsEffected != 1) {
            throw new Exception($"Expected {nameof(result.RowsEffected)} == 1. Actual value == {result.RowsEffected}");
        }
        transaction.Commit();
    }
}
```

## Prepared Delete Query Example

```C#
public sealed class DeleteProductHandler {

    private readonly static IPreparedDeleteQuery<IntKey<IProduct>> _deleteProductQuery;

    static DeleteProductHandler() {

        ProductTable table = ProductTable.Instance;

        _deleteProductQuery = Query
            .Prepare<IntKey<IProduct>>()
            .Delete(table)
            .Where(where => where.EQUALS(table.ProductID, productId => productId))
            .Build();
    }

    private readonly IDatabase _database;

    public DeleteProductHandler(IDatabase database) {
        _database = database;
    }

    public async Task DeleteProduct(IntKey<IProduct> productId, CancellationToken cancellationToken) {

        using Transaction transaction = new Transaction(_database);

        NonQueryResult result = await _deleteProductQuery.ExecuteAsync(parameters: productId, transaction, cancellationToken);

        if(result.RowsEffected != 1) {
            throw new Exception($"Expected {nameof(result.RowsEffected)} == 1. Actual value == {result.RowsEffected}");
        }
        transaction.Commit();
    }
}
```