# Prepared Queries

Query lite has two types of queries, `dynamic` and `prepared`.

Prepared queries generate their sql query string once and reuse that object for every execution. Whereas dynamic queries generate a new sql query for every execution.

These are the pros and cons of the two query types:

## Dynamic Queries

Pros:

* Simpler C# syntax
* Allow for dynamic conditions
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

## Syntax Not Supported By Prepared Queries

The following syntax is currently not supported by prepared queries:
 * Nested queries
 * `WHERE column IN(....)` expressions
 * Functions that contain parameters e.g. `SELECT LEN(@0)`

# What Query Type Is Best?

Dynamic queries are a good choice as the C# syntax is easier to read and understand. In general a dynamic queries have good performance.

Prepared queries have performance and memory allocation characteristics near equivalent to direct ado.net. So, if a query is on the 'hot path' and memory allocation pressure on the garbage collector is an issue, then use a prepared query.

# Examples

## The Prepare Clause

To begin a Prepared query you must first call the `Query.Prepare<>()` method. This has a generic argument called `PARAMETERS` which defines type of object that can be used to set parameters within the query.

Here is an example using the three parameter values 'GivenName', 'Surname' and 'Id' in the `WHERE` clause:

```C#
var query = Query
    .Prepare<Person>()  //Define the query 'PARAMETERS' type as 'Person'
    .Select(row => table.Id)
    .From(table)
    .Where(where =>
        where.EQUALS(table.GivenName, person => person.GivenName) &  //Use the Person parameter in each condition
        where.EQUALS(table.Surname, person => person.Surname) &
        where.NOT_EQUALS(table.Id, person => person.Id)
    )
    .Build();

Person person = new Person() { Id = 25, GivenName = "Glen", Surname = "Smith" };

var result = query.Execute(parameters: person, _database);  //Provide the person class when executing query
```

## Prepared Select Query Example

```C#
public sealed class ProductCostHandler {

    /*
     *  The prepared query is defined to receive a parameter of type 'ProductId' on every call
     *  to execute query and return 'rows' of the type 'ProductCostHistory'
     */
    private readonly static IPreparedQueryExecute<ProductId, ProductCostHistory> _loadCostHistoryQuery;

    static ProductCostHandler() {

        /*
         * Create the prepared query is the static constructor
         */
        ProductTable productTable = ProductTable.Instance;
        ProductCostHistoryTable costHistoryTable = ProductCostHistoryTable.Instance;

        _loadCostHistoryQuery = Query
            .Prepare<ProductId>()        //This line defines that ProductId is a query parameter type.
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
            .Where(where => where.EQUALS(productTable.ProductID, id => id))    //The function id => id returns the provided 'ProductId' for the underlying sql parameter
            .OrderBy(costHistoryTable.StartDate, costHistoryTable.EndDate)
            .Build();
    }

    private readonly IDatabase _database;

    public ProductCostHandler(IDatabase database) {
        _database = database;
    }

    public async Task<IList<ProductCostHistory>> LoadProductCostHistory(ProductId productId, CancellationToken cancellationToken) {

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

    private readonly static IPreparedInsertQuery<Product, ProductId> _insertProductQuery;

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

    public async Task<ProductId> AddProduct(Product product, CancellationToken cancellationToken) {
        
        /*
         *  Execute the prepared query and passing 'product' in as a parameter
         */
        using Transaction transaction = new Transaction(_database);

        QueryResult<ProductId> result = await _insertProductQuery.ExecuteAsync(parameters: product, transaction, cancellationToken);

        transaction.Commit();

        ProductId productId = result.Rows[0];

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

    private readonly static IPreparedDeleteQuery<ProductId> _deleteProductQuery;

    static DeleteProductHandler() {

        ProductTable table = ProductTable.Instance;

        _deleteProductQuery = Query
            .Prepare<ProductId>()
            .Delete(table)
            .Where(where => where.EQUALS(table.ProductID, productId => productId))
            .Build();
    }

    private readonly IDatabase _database;

    public DeleteProductHandler(IDatabase database) {
        _database = database;
    }

    public async Task DeleteProduct(ProductId productId, CancellationToken cancellationToken) {

        using Transaction transaction = new Transaction(_database);

        NonQueryResult result = await _deleteProductQuery.ExecuteAsync(parameters: productId, transaction, cancellationToken);

        if(result.RowsEffected != 1) {
            throw new Exception($"Expected {nameof(result.RowsEffected)} == 1. Actual value == {result.RowsEffected}");
        }
        transaction.Commit();
    }
}
```