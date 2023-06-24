# Query Lite Documentation

## Index

- [Introduction](#introduction)
- [Prepared Queries](#prepared-queries)
- [Select Query](#select-query)
- [Defining A Database](#defining-a-database)
- [Database Support](#database-support)
- [Schema mapping](#schema-mapping)
- [Defining A Table Definition](#defining-a-table-definition)
- [Tables And Columns That Use Sql Keywords](#tables-and-columns-that-use-sql-keywords)
- [Column Nullability](#column-nullability)
- [Select Collector](#select-collector)
- [Left Joins](#left-joins)
- [Query Timeout](#query-timeout)
- [Select Union Query](#select-union-query)
- [Select Distinct Query](#select-distinct-query)
- [Select TOP Query](#select-top-query)
- [Nested Query](#nested-query)
- [Insert Query](#insert-query)
- [Update Query](#update-query)
   - [Update From Query](#update-from-query)
- [Delete Query](#delete-query)
   - [Delete From Query](#delete-from-query)
- [Truncate Query](#truncate-query)
- [Supported Operators](#supported-operators)
- [String Like Condition](#string-like-condition)
- [Functions](#functions)
- [Custom Functions](#custom-functions)
- [Supported Data Types](#supported-data-types)
   - [Key Columns](#key-columns)
   - [Geography Types](#geography-types)
   - [Not Supported Data Types](#not-supported-data-types)
- [Transaction Isolation Levels](#transaction-isolation-levels)
- [Query Builder Caching](#query-builder-caching)
- [Executing Custom SQL](#executing-custom-sql)
- [Debugging](#debugging)
- [Breakpoint Debugging](#breakpoint-debugging)
- [Schema Validation](#schema-validation)
- [Database Constraints](#database-constraints)
- [Table Code Generation](#table-code-generation)
- [Documentation Generator](#documentation-generator)

## Introduction

Query lite is C# database query library.

These are the main features:

* Type safe queries
  * Joins, where conditions and selected column types are type safe
* Generate table/schema definitions with code generation tool
* Runtime database schema validation
* Events that return the executing query details
* Supports select, insert, update, delete and truncate queries
  * Syntax - select, distinct, top, from join, left join, table hints, where, group by, order by, returning

## Prepared Queries

Please note that the examples in this document are using `dynamic queries` but the documentation applies to both dynamic and prepared queries apart from the differences in C# query syntax.

Specific documentation and examples for prepared queries is located here => [Prepared Query Documentation](PreparedQueries.md)

## Select Query

Here is a basic example of a query that selects 3 columns and orders by the shippers id

```C#
ShipperTable shipperTable = ShipperTable.Instance;

var result = Query
    .Select(
        row => new {
            Id = row.Get(shipperTable.Id),
            CompanyName = row.Get(shipperTable.CompanyName),
            Phone = row.Get(shipperTable.Phone)
        }
    )
    .From(shipperTable)
    .OrderBy(shipperTable.Id.ASC)
    .Execute(DB.Northwind);

foreach(var row in result.Rows) {

    int id = row.Id;
    string companyName = row.CompanyName;
    string? phone = row.Phone;
}
```

## Defining A Database

In order to query a database object must be created. Both Sql Server and postgresql are supported. This must be passed as a parameter into every query.

```C#
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;

//Sql Server
IDatabase northwind = new SqlServerDatabase(
      name: "Northwind",
      connectionString: "Server=localhost;Database=Northwind;Trusted_Connection=True;"
);

//PostgreSql
IDatabase northwind = new PostgreSqlDatabase(
      name: "Northwind",
      connectionString: "Server=127.0.0.1;Port=5432;Database=Northwind;User Id=postgres;Password=my_password;"
);
```

## Database Support

Query Lite works with Sql Server and PostgreSql databases. A reference to the nuget package System.Data.SqlClient and Npgsql is required for each.

Note: There are some differences in behavour between the two databases behaviour in Query Lite. 

* The two databases have different locking models
* Some date types are stored and returned differently
  - PostgreSqls type - TIMESTAMP WITH TIME ZONE is always stored and returned as UTC time 
  - Sql Servers DATETIMEOFFSET returns in the timezone it was saved as
* Some of the query syntax is database specific
  - Table and query hints eg. WITH(UPDLOCK) and OPTION(...) are specific to Sql Server 
  - FOR(ForType forType, ITable[] ofTables, WaitType waitType) syntax is specific to PostgreSql
  - Delete Join syntax is only supported on Sql Server

## Schema mapping

The C# table definitions only allow one schema name to be set. In the case where the schema name for the same table is different between database servers, a schema mapping function can be used to convert between schema names.

For example: Sql Server's schema name might be defined as "dbo" in C# but the PostgreSql schema is "public".
```C#
IDatabase northwind = new PostgreSqlDatabase(
      name: "Northwind",
      connectionString: "Server=127.0.0.1;Port=5432;Database=Northwind;User Id=postgres;Password=my_password;",
      schemaMap: schema => (schema == "dbo" ? "public" : string.Empty)  //This will convert "dbo" to "public" when using PostgreSql
);
```
## Defining A Table Definition

First we need to start by creating our table in sql.

```Sql
CREATE TABLE Shipper(
    Id INTEGER IDENTITY (1,1) NOT NULL,
    CompanyName NVARCHAR (40) NOT NULL,
    Phone NVARCHAR(24) NULL,
    CONSTRAINT pk_Shipper PRIMARY KEY CLUSTERED(Id)
);
```

Then we create the table class in C#. Nullable columns must use the type NullableColumn&lt;>.

```C#
using QueryLite;

public sealed class ShipperTable : ATable {

    public static readonly ShipperTable Instance = new ShippersTable();

    public Column<int> Id { get; }
    public Column<string> CompanyName { get; }
    public NullableColumn<string> Phone { get; }

    private ShippersTable() : base(tableName:"Shipper", schemaName: "dbo") {

        Id = new Column<int>(this, columnName: "Id", isAutoGenerated: true, length: null);
        CompanyName = new Column<string>(this, columnName: "CompanyName", length: 40);
        Phone = new NullableColumn<string>(this, columnName: "Phone", length: 24);
    }
}
```

Each table has at least one static 'Instance' field that is used as a singleton pattern. Tables classes are immutable so there is no need to create a new table object for every query.

Note: Each instance of a table is assigned a unique table alias when it is created. Generally a single instance of a table is all that is needed but queries that use the same table more than once require a different instance of the table in order for the table aliases to be correct.

For Example:

```C#
  MyTable t1 = MyTable.Instance1; //Each instance of MyTable has a different alias
  MyTable t2 = MyTable.Instance2;

  var result = Query
      .Select(row => row.Get(t1.Text))
      .From(t1)
      .Join(t2).On(t1.Id == t2.ParentId)
```

Is the equivalent of:

```Sql
  SELECT _1.Text
  FROM myTable AS _1
  JOIN myTable AS _2 ON _1.Id = _2.ParentId
```

## Tables And Columns That Use Sql Keywords

If a table name or column name is an sql keyword then the `enclose` parameter on the table or column constructor should be set to `true`. This is so the sql generators know to elcose the name with square brackets i.e. [table_name] or [column_name]. If the table or column is not enclosed it may cause sql syntax errors.

## Column Nullability

Non null columns are represented by the type Column&lt;> and nullable columns by NullableColumn&lt;>

## Select Collector

A column selector function is used for performance and type safety. Columns are mapped to the underlying sql query using their ordinal position in which they are 'collected' from the function. This means that when the sql query is being built, the selector function is run first to determine each column name and ordinal position. Then once the query result is retrieved, the selector function is called for each row in order to populate the result set. Because of this, it is important that the selector function does not contain any conditional logic that can change the ordinal position of a column.

As a rule the collector function should only choose a single column per property and contain no conditional logic.

For Example:

```C#
var result = Query
    .Select(            
        row => new {    //This is good,  The ordinal positions in the query will be ShipperID = 0, CompanyName = 1, Phone = 2
            Id = row.Get(shipperTable.ShipperID),
            CompanyName = row.Get(shipperTable.CompanyName),
            Phone = row.Get(shipperTable.Phone)
        }
    )

var result = Query
    .Select(            
        row => new {
            Id = row.Get(shipperTable.ShipperID),
            //
            //  This is bad (Below) as the column 'CompanyName' could be determined to have
            //  either one or two ordinal positions in the query and could cause an exception
            //  during execution.
            //
            //  So the ordinal postions could be either
            //     -> ShipperID = 0, CompanyName = 1, CompanyName = 2, Phone = 3
            //  or -> ShipperID = 0, CompanyName = 1, Phone = 2
            // 
            CompanyName = row.Get(shipperTable.CompanyName) != null ? row.Get(shipperTable.CompanyName) : "",
            Phone = row.Get(shipperTable.Phone)
        }
    )
```

## Left Joins

When a table is used in a left join its result can be empty / null. In this case the selected columns of the left join table will default in different ways depending on their .net type. Reference types (e.g. string, byte[] and key types) will return null (Even when non-nullable reference types is turned on), value types will default to their default value e.g. short / int / long will default to 0. Nullable value types will default to null.

To detect if a left join result is null, it is recommended to select the primary key column of the table and check the result is not null. Note: That nullable columns are not suitable for this type of check as they can return null regardless of the join used.

For example:

```C#
CustomersTable customersTable = CustomersTable.Instance;
OrdersTable ordersTable = OrdersTable.Instance;

var result = Query
    .Select(
        row => new {
            OrderId = row.Get(ordersTable.OrderID),
            CustomerId = row.Get(customersTable.CustomerID)
        }
    )
    .From(ordersTable)
    .LeftJoin(customersTable).On(ordersTable.CustomerID == customersTable.CustomerID)
    .Execute(DB.Northwind, TimeoutLevel.ShortSelect);

foreach(var row in result.Rows) {

    if(row.CustomerId != null) {    //Check to see if the left join result is not null

    }
}
```

## Query Timeout

**Note: The default timeout for each query type is `Short` -> 60 seconds.**

Default query timeouts are defined in the `TimeoutLevel` class. Each query type has a `Short`, `Medium` and `Long` default value and these can be adjusted where needed.

When a timeout parameter is not passed into an execute method (e.g. null), the timeout will default to a default "short" timeout for that particular query type. For example, a select query will default to `TimeoutLevel.ShortSelect`.

These are the available predefined timeout defaults:

```C#
public static class TimeoutLevel {

    // Short select query timeout defaults to 60 seconds
    public static QueryTimeout ShortSelect { get; set; } = new QueryTimeout(seconds: 60);

    // Short insert query timeout defaults to 60 seconds
    public static QueryTimeout ShortInsert { get; set; } = new QueryTimeout(seconds: 60);

    // Short update query timeout defaults to 60 seconds
    public static QueryTimeout ShortUpdate { get; set; } = new QueryTimeout(seconds: 60);

    // Short delete query timeout defaults to 60 seconds
    public static QueryTimeout ShortDelete { get; set; } = new QueryTimeout(seconds: 60);

    // Medium select query timeout defaults to 300 seconds
    public static QueryTimeout MediumSelect { get; set; } = new QueryTimeout(seconds: 300);

    // Medium insert query timeout defaults to 300 seconds
    public static QueryTimeout MediumInsert { get; set; } = new QueryTimeout(seconds: 300);

    // Medium update query timeout defaults to 300 seconds
    public static QueryTimeout MediumUpdate { get; set; } = new QueryTimeout(seconds: 300);

    // Medium delete query timeout defaults to 300 seconds
    public static QueryTimeout MediumDelete { get; set; } = new QueryTimeout(seconds: 300);

    // Long select query timeout defaults to 1800 seconds
    public static QueryTimeout LongSelect { get; set; } = new QueryTimeout(seconds: 1800);

    // Long insert query timeout defaults to 1800 seconds
    public static QueryTimeout LongInsert { get; set; } = new QueryTimeout(seconds: 1800);

    // Long update query timeout defaults to 1800 seconds
    public static QueryTimeout LongUpdate { get; set; } = new QueryTimeout(seconds: 1800);

    // Long delete query timeout defaults to 1800 seconds
    public static QueryTimeout LongDelete { get; set; } = new QueryTimeout(seconds: 1800);
}
```

Example of passing a timeout parameters:
```C#
var result = Query
    .Select(row => row.Get(shipperTable.Id))
    .From(shipperTable)
    .Execute(DB.Northwind);   //This will default to TimeoutLevel.ShortSelect

var result = Query
    .Select(row => row.Get(shipperTable.Id))
    .From(shipperTable)
    .Execute(DB.Northwind, TimeoutLevel.MediumSelect); //Medium select timeout

var result = Query
    .Insert(shipperTable)
    .Values(values => values
        .Set(shipperTable.CompanyName, "My company name")
        .Set(shipperTable.Phone, "123456789")
    )
    .Execute(transaction);  //This will default to TimeoutLevel.ShortInsert

var result = Query
    .Insert(shipperTable)
    .Values(values => values
        .Set(shipperTable.CompanyName, "My company name")
        .Set(shipperTable.Phone, "123456789")
    )
    .Execute(transaction, TimeoutLevel.MediumInsert);  //Medium insert timeout
```

## Select Union Query

Union queries are supported.

**Note: Please be careful to ensure that each `select` clause has:**

* The same number of columns
* Columns are in the exact same order
* Columns are of the same data type

**Otherwise the query may fail.**

```C#
ShipperTable shipperTable = ShipperTable.Instance;

var result = Query
    .Select(row => new { CompanyName = row.Get(shipperTable.CompanyName), Phone = row.Get(shipperTable.Phone) })
    .From(shipperTable)
    .Where(shipperTable.CompanyName == "")
    .UnionSelect(row => new { CompanyName = row.Get(shipperTable.CompanyName), Phone = row.Get(shipperTable.Phone) })
    .From(shipperTable)
    .Where(shipperTable.Phone.IsNull)
    .Execute(DB.Northwind);
```

## Select Distinct Query

Select DISTINCT is supported.

```C#
var result = Query
    .Select(row => row.Get(shipperTable.Id))
    .Distinct
    .From(shipperTable)
    .Execute(DB.Northwind); 
```

## Select TOP Query

Select TOP is supported. In SqlServer this generates the `TOP` syntax and in PostgreSql this generates the `LIMIT` syntax.

```C#
var result = Query
    .Select(row => row.Get(shipperTable.Id))
    .Top(100)
    .From(shipperTable)
    .Execute(DB.Northwind); 
```

## Nested Query

Nested queries are supported.

```C#
ShipperTable shipperTable = ShipperTable.Instance;
ShipperTable shipperTable2 = ShipperTable.Instance2;    //Get a second instance of the shipper table so the table in the nested query sql uses a different alias

var result = Query
    .Select(row => new { CompanyName = row.Get(shipperTable.CompanyName), Phone = row.Get(shipperTable.Phone) })
    .From(shipperTable)
    .Where(
        shipperTable.CompanyName.In(
            Query.NestedSelect(shipperTable2.CompanyName)
                .From(shipperTable2)
                .GroupBy(shipperTable2.CompanyName)
                .Having(COUNT_ALL.Instance > 1)
        )
    )
    .Execute(DB.Northwind);
```

## Insert Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .Insert(shipperTable)
        .Values(values => values
            .Set(shipperTable.CompanyName, "My company name")
            .Set(shipperTable.Phone, "123456789")
        )
        .Execute(
            //Returns the auto generated SupplierID column
            inserted => new { ShipperID = inserted.Get(shipperTable.ShipperID) },
            transaction
        );

    transaction.Commit();

    IntKey<IShipper> shipperId = result.Rows.First().ShipperID;
}
```

## Update Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .Update(shipperTable)
        .Values(values => values
            .Set(shipperTable.Phone, "")
        )
        .Where(shipperTable.Phone.IsNull)
        .Execute(transaction);

    transaction.Commit();
}
```

## Update Returning Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .Update(shipperTable)
        .Values(values => values
            .Set(shipperTable.Phone, "")
        )
        .Where(shipperTable.Phone.IsNull)
        .Execute(
            updated => updated.Get(shipperTable.ShipperID),
            transaction
        );

    transaction.Commit();
    
    IntKey<IShipper> shipperId = result.Rows.First();
}
```

## Update From Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    OrdersTable orderTable = OrdersTable.Instance;
    CustomersTable customersTable = CustomersTable.Instance;

    NonQueryResult result = Query
        .Update(orderTable)
        .Values(values => values
            .Set(orderTable.ShipVia, null)
        )
        .From(customersTable)
        .Where(orderTable.CustomerID == customersTable.CustomerID & customersTable.Region.IsNull)
        .Execute(transaction);

    transaction.Commit();
}
```

## Delete Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .Delete(shipperTable)
        .Where(shipperTable.ShipperID == new IntKey<IShipper>(100))
        .Execute(transaction);

    transaction.Commit();
}
```

## Delete Returning Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .Delete(shipperTable)
        .Where(shipperTable.ShipperID == new IntKey<IShipper>(100))
        .Execute(
            deleted => deleted.Get(shipperTable.ShipperID),
            transaction
        );

    transaction.Commit();
    
    IntKey<IShipper> shipperId = result.Rows.First();
}
```

## Delete From Query

Note: The `From` and `Using` methods are equivalent syntax for both PostgreSql and Sql Server.

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    OrdersTable orderTable = OrdersTable.Instance;
    CustomersTable customersTable = CustomersTable.Instance;

    NonQueryResult result = Query
        .Delete(orderTable)
        .From(customersTable)
        .Where(orderTable.CustomerID == customersTable.CustomerID & customersTable.Region.IsNull)
        .Execute(transaction);

    transaction.Commit();
}
```

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    OrdersTable orderTable = OrdersTable.Instance;
    CustomersTable customersTable = CustomersTable.Instance;

    NonQueryResult result = Query
        .Delete(orderTable)
        .Using(customersTable)
        .Where(orderTable.CustomerID == customersTable.CustomerID & customersTable.Region.IsNull)
        .Execute(transaction);

    transaction.Commit();
}
```

## Truncate Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    NonQueryResult result = Query
        .Truncate(shipperTable)
        .Execute(transaction);

    transaction.Commit();
}
```

## Supported Operators

| Description | Operator / Method | Example | Notes
| -------- | ----------- | ------- | ---------|
| Equals operator |`==` AND `!=`    | `.Where(orderTable.ShipPostalCode == "abc")` | 
| Equals operator |`==` AND `!=`     | `.LeftJoin(unitMeasureTable).On(productTable.SizeUnitMeasureCode == unitMeasureTable.UnitMeasureCode)` | Join two columns
| Non Type Safe Equals   | Methods `SqlEquals_NonTypeSafe(...)` `SqlNotEquals_NonTypeSafe(...)` | `.Where(orderTable.CustomerID.SqlEquals_NonTypeSafe(10))` | NonTypeSafe methods can be used to work around compile errors caused by the database schema being too complex to be defined correctly in C# |
| Math operators | `<` `<=` `>` `>=` | `.Where(productTable.ListPrice <= 10.0m)` |
| `AND` | `&` | `.Where(productTable.Name == "abc" & productTable.ListPrice > 10.0m)` | |
| `OR`  | `|` | `.Where(productTable.Name == "abc" | productTable.ListPrice > 10.0m)` | |
| `AND` & `OR` | `&` `|` | `.Where((productTable.Name == "abc" | productTable.Name == "efg") & productTable.ListPrice > 10.0m)` | Note: Always surround mixed `AND` and `OR` C# operators with brackets to get the correct sql logic.|
| `IS NULL` | `IsNull` | `.Where(productTable.Name.IsNull)`|
| `IS NOT NULL` | `IsNotNull` | `.Where(productTable.Name.IsNotNull)` |
| `IN(...)` | `In(...)` | `.Where(productTable.Name.In("abc", "efg", "hijk"))` |
| `NOT IN(...)` | `NotIn(...)` | `.Where(productTable.Name.NotIn("abc", "efg", "hijk"))` |
| `LIKE` | `Like(ILike<TYPE> like)` | `.Where(productTable.Name.Like(new StringLike("%abc%"))` |
| `NOT LIKE` | `NotLike(ILike<TYPE> like)` | `.Where(productTable.Name.NotLike(new StringLike("%abc%"))` |



## String Like Condition

```C#
OrdersTable orderTable = OrdersTable.Instance;

var result = Query
    .Select(row => row.Get(orderTable.OrderID)
    .From(orderTable)
    .Where(orderTable.ShipPostalCode.Like(new StringLike("%abc%")))
    .Execute(DB.Northwind);
```

## Functions

Currently only a small set of sql functions are supported. For Sql Server these are `COUNT_ALL`, `GETDATE`, `NEWID` and `SYSDATETIMEOFFSET`. Please note that creating custom sql functions is documented below this section.

Note: Function classes that are immutable may implement a singleton pattern to reduce memory allocations. For example: `COUNT_ALL.Instance`, `GETDATE.Instance`, `NEWID.Instance` and `SYSDATETIMEOFFSET.Instance`.

```C#
ShipperTable shipperTable = ShipperTable.Instance;

COUNT_ALL count = COUNT_ALL.Instance;

QueryResult<int> result = Query
    .Select(
        row => row.Get(count)
    )
    .From(shipperTable)
    .Execute(DB.Northwind);
```

This is the sql that is executed:
```SQL
SELECT COUNT(*) FROM dbo.Shipper
```

## Custom Functions

Here is an example of how to create and use a custom sql function.

Define a custom string length function in C#:

```C#
public sealed class Length : Function<int> {

    private Column<string> Column { get; }

    public Length(Column<string> column) : base(name: "Length") {
        Column = column;
    }

    public override string GetSql(IDatabase database, bool useAlias, IParameters? parameters) {

        if(useAlias) {
            return $"LEN({Column.Table.Alias}.{Column.ColumnName})";
        }
        return $"LEN({Column.ColumnName})";
    }
}
```

This is how to include the custom function in a query:

```C#
ShipperTable shipperTable = ShipperTable.Instance;

Length length = new Length(shipperTable.CompanyName);

var result = Query
   .Select(
       row => new {
           CompanyName = row.Get(shipperTable.CompanyName),
           StringLength = row.Get(length)
       }
   )
   .From(shipperTable)
   .Execute(_northwindDatabase);

foreach(var row in result.Rows) {

    string companyName = row.CompanyName;
    int stringLength = row.StringLength;
}
```

## Sql Server Table And Query Hints

Query Lite supports a subset of the Sql Server table and query hint syntax. Note: This syntax will be ignored if the database is not an instanace of Sql Server.


```C#
ShipperTable shipperTable = ShipperTable.Instance;

var result = Query
   .Select(
       row => CompanyName = row.Get(shipperTable.CompanyName)
   )
   .From(shipperTable)
   .With(SqlServerTableHint.UPDLOCK, SqlServerTableHint.SERIALIZABLE)
   .Option(labelName: "Lable 1", SqlServerQueryOption.FORCE_ORDER)
   .Execute(_northwindDatabase);
```

These are the supported table and query hints:

```C#
public enum SqlServerTableHint {
    KEEPIDENTITY,
    KEEPDEFAULTS,
    HOLDLOCK,
    IGNORE_CONSTRAINTS,
    IGNORE_TRIGGERS,
    NOLOCK,
    NOWAIT,
    PAGLOCK,
    READCOMMITTED,
    READCOMMITTEDLOCK,
    READPAST,
    REPEATABLEREAD,
    ROWLOCK,
    SERIALIZABLE,
    SNAPSHOT,
    TABLOCK,
    TABLOCKX,
    UPDLOCK,
    XLOCK
}
public enum SqlServerQueryOption {
    HASH_JOIN,
    LOOP_JOIN,
    MERGE_JOIN,
    FORCE_ORDER,
    FORCE_EXTERNALPUSHDOWN,
    DISABLE_EXTERNALPUSHDOWN
}
```

## Supported Data Types

| C# Type                       | Sql Server             | PostgreSql                  | Notes  |
| ----------------------------- | ---------------------- | --------------------------- | ------ |
| Column&lt;string>             | NVARCHAR               | VARCHAR                     |        |
| Column&lt;StringKey&lt;TYPE>> | NVARCHAR               | VARCHAR                     |        |
| Column&lt;Guid>               | UNIQUEIDENTIFIER       | UUID                        |        |
| Column&lt;GuidKey&lt;TYPE>>   | UNIQUEIDENTIFIER       | UUID                        |        |
| Columns&lt;short>             | SMALLINT               | SMALLINT                    |        |
| Column&lt;ShortKey&lt;TYPE>>  | SMALLINT               | SMALLINT                    |        |
| Column&lt;int>                | INTEGER                | INTEGER, SERIAL             |        |
| Column&lt;IntKey&lt;TYPE>>    | INTEGER                | INTEGER, SERIAL             |        |
| Column&lt;long>               | BIGINT                 | BIGINT                      |        |
| Column&lt;LongKey&lt;TYPE>>   | BIGINT                 | BIGINT                      |        |
| Columns&lt;bool>              | TINYINT                | BOOLEAN                     |        |
| Columns&lt;Bit>               | BIT                    |                             | Ado returns `BIT` as a `byte` rather than a `bool`. So sharing the `bool` type with both `BIT` and `SMALLINT` would add a conversion step that would reduce result loading preformance. |
| Column&lt;decimal>            | DECIMAL                | DECIMAL                     |        |
| Column&lt;float>              | REAL                   | REAL                        |        |
| Column&lt;double>             | FLOAT                  | DOUBLE PRECISION            |        |
| Column&lt;byte[]>             | VARBINARY              | BYTEA                       |        |
| Column&lt;DateOnly>           | DATE                   | DATE                        |        |
| Column&lt;TimeOnly>           | TIME                   | TIME WITHOUT TIME ZONE      | Precision is only up to microseconds. Nanosecond precision and timezone are not supported. |
| Column&lt;DateTime>           | DATETIME               | TIMESTAMP WITHOUT TIME ZONE | PostgreSql and Sql Server have differences in behaviour. TIMESTAMP WITH TIME ZONE is always stored and returned as UTC time. Sql Server DATETIMEOFFSET returns in the timezone it was populated with. |
| Column&lt;DateTimeOffset>     | DATETIMEOFFSET         | TIMESTAMP WITH TIME ZONE    | PostgreSql and Sql Server have differences in behaviour. TIMESTAMP WITH TIME ZONE is always stored and returned as UTC time. Sql Server DATETIMEOFFSET returns in the timezone it was populated with. |
| Column&lt;Enum>               | TINYINT, SMALLINT, INT | SMALLINT, INT               |        |
| Column&lt;BoolValue&lt;TYPE>> | TINYINT                | BOOLEAN                     |        |

## Key Columns

Key columns can be used for primary and foreign key columns. They help make joins, conditions and values type safe.

| Key Type           |
| ------------------ |
| GuidKey&lt;TYPE>   |
| StringKey&lt;TYPE> |
| ShortKey&lt;TYPE>  |
| IntKey&lt;TYPE>    |
| LongKey&lt;TYPE>   |
| BoolValue&lt;TYPE> |

The generic TYPE can be any type but it is suggested that an empty interface is used for each table.

For example we can change the shipper 'Id' column from an int to an InKey<>

```C#
public interface IShipper {}

using QueryLite;

public sealed class ShipperTable : ATable {

    public static readonly ShipperTable Instance = new ShippersTable();

    public Column<IntKey<IShipper>> Id { get; } //Now an IntKey<> type
    public Column<string> CompanyName { get; }
    public NullableColumn<string> Phone { get; }

    private ShippersTable() : base(tableName:"Shipper", schemaName: "dbo") {

        Id = new Column<IntKey<IShipper>>(this, columnName: "Id", isAutoGenerated: true);
        CompanyName = new Column<string>(this, columnName: "CompanyName", length: 40);
        Phone = new NullableColumn<string>(this, columnName: "Phone", length: 24);
    }
}
```

## Geography Types

Geography types are partially supported. These are complex data types that cannot be returned directly via a query. Instead, a range of functions are used to return infomation about the various geography types.

**Note: Currently Geography functions have only been implemented for Sql Server. Some functions may work on PostgreSql but others are Sql Server Only.**

A custom function can be created if you require one that is not implemented by this library. Tip: View the source code and use the implementation of these functions as a guide on how to create a new function.

These are the geography functions currently implemented:

| Function           | Sql |
| ------------------ | --- |
| GeographyPoint | `geography::Point(...)` |
| STArea | `.STArea()` |
| STEquals | `.STEquals(...)` |
| STAsBinary | `.STAsBinary()` |
| STAsText | `.STAsText()` |
| STContains | `.STContains(...)` |
| STDistance | `.STDistance(...)` |
| STGeomFromText | `geography::STGeomFromText(...)` |
| STPointFromText | `geography::STPointFromText(...)` |
| STLineFromText | `geography::STLineFromText(...)` |
| STPolyFromText | `geography::STPolyFromText(...)` |
| STMPointFromText | `geography::STMPointFromText(...)` |
| STMLineFromText | `geography::STMLineFromText(...)` |
| STMPolyFromText | `geography::STMPolyFromText(...)` |
| STGeomCollFromText | `geography::STGeomCollFromText(...)` |
| STGeomCollFromWKB | `geography::STGeomCollFromWKB(...)` |
| STGeomFromWKB | `geography::STGeomFromWKB(...)` |
| STPointFromWKB | `geography::STPointFromWKB(...)` |
| STLineFromWKB | `geography::STLineFromWKB(...)` |
| STPolyFromWKB | `geography::STPolyFromWKB(...)` |
| STMPointFromWKB | `geography::STMPointFromWKB(...)` |
| STMLineFromWKB | `geography::STMLineFromWKB(...)` |
| STMPolyFromWKB | `geography::STMPolyFromWKB(...)` |
| Longitude | `.Long` |
| Latitude | `.Lat` |
| GeographyParse | `geography::Parse(...)` |

Here is a code example querying the database with a subset of the geography functions listed above:

```C#
GeoTestTable table = GeoTestTable.Instance;

//Define a 'STPointFromText(...)' sql function
STPointFromText stPointFromText = new STPointFromText(kwText: "POINT(-122.34900 47.65100)");

GuidKey<IGeoTest> guid = GuidKey<IGeoTest>.ValueOf(Guid.NewGuid());

using(Transaction transaction = new Transaction(TestDatabase.Database)) {

    //Insert a record into the database
    NonQueryResult insertResult = Query
        .Insert(table)
        .Values(values => values
            .Set(table.Guid, guid)
            .Set(table.Geography, stPointFromText)
        )
        .Execute(transaction);
    /*
        INSERT INTO dbo.GeoTest(gtGuid,gtGeography) VALUES('f876dc6c-4ada-48cd-ade3-e61323d2b416',geography::STPointFromText('POINT(-122.34900 47.65100)', 4326))
    */
    transaction.Commit();
}

//Define a 'geography::Point' sql function
GeographyPoint geographyPoint = new GeographyPoint(latitude: 47.65100, longitude: -122.34900);

//Define a 'STDistance()' sql function
STDistance distance = new STDistance(table.Geography, geographyPoint);

//Define a 'STAsBinary()' sql function
STAsBinary stAsBinary = new STAsBinary(table.Geography);

//Define a 'STAsText()' sql function
STAsText stAsText = new STAsText(table.Geography);

//Define a '.Long' sql property
Longitude longitude = new Longitude(table.Geography);

//Define a '.Lat' sql property
Latitude latitude = new Latitude(table.Geography);

var result = Query
    .Select(
        row => new {
            Guid = row.Get(table.Guid),
            Distance = row.Get(distance),
            Binary = row.Get(stAsBinary),
            Text = row.Get(stAsText),
            Longitude = row.Get(longitude),
            Latitude = row.Get(latitude)
        }
    )
    .From(table)
    .Where(new STEquals(table.Geography, geographyPoint) == 1)
    .Execute(TestDatabase.Database);
/*
    SELECT gtGuid,
        gtGeography.STDistance(geography::Point(47.651,-122.349,4326)),
        gtGeography.STAsBinary(),
        gtGeography.STAsText(),
        gtGeography.Long,
        gtGeography.Lat
    FROM dbo.GeoTest
    WHERE gtGeography.STEquals(geography::Point(47.651,-122.349,4326)) = 1
*/
Assert.AreEqual(result.Rows.Count, 1);

var row = result.Rows[0];

Assert.AreEqual(row.Guid, guid);
Assert.AreEqual(row.Distance, 0);
Assert.AreEqual(BitConverter.ToString(row.Binary!).Replace("-", ""), "01010000007593180456965EC017D9CEF753D34740");
Assert.AreEqual(row.Text, "POINT (-122.349 47.651)");
Assert.AreEqual(row.Longitude, -122.349);
Assert.AreEqual(row.Latitude, 47.651);
```

## Not Supported Data Types

Most non-common or custom database types are not supported. If the type can be read in using a basic .net type (e.g. `int`, `string`, `decimal`) in `ado.net` (e.g. `reader.GetString(ordinal)`) then it should work.

Sql server types like `hierarchyid` are known to not work.


## Transaction Isolation Levels

Isolation levels can be set on transactions. Please note that levels like 'Snapshort' are only supported by Sql Server. If the code returns from the using statement before calling `transaction.Commit()` the transaction will be rolled back.

```C#
using(Transaction transaction = new Transaction(DB.Northwind, IsolationLevel.ReadCommitted)) {
    ...
    transaction.Commit();
}
```


## Query Builder Caching

The query building process uses caching to reduce memory allocation. Objects like StringBuilder and collector classes are cached by default. Caching can be configured in the `Settings` class.

Cached objects are stored for every thread that executes a query and are garbage collected when the thread is no longer referenced.

```C#
//Turns on StringBuilder caching
Settings.EnableStringBuilderCaching = true;

//Max allowed character length of cached StringBuilders
Settings.StringBuilderCacheMaxCharacters = 5000;

//Enable caching of collector classes
Settings.EnableCollectorCaching = true;
```

## Executing Custom SQL

If you need to execute custom SQL / TSQL within an existing transaction, you can create an Ado `DbCommand` object `CreateCommand(...)` on the transaction. The command object will be returned with the tranaction and timeout setting populated.

Note: Remember to correctly dispose of the command object.

```C#
using(Transaction transaction = new Transaction(database)) {

    using DbCommand command = transaction.CreateCommand(timeout: TimeoutLevel.ShortSelect);

    command.CommandText = $"SET IDENTITY_INSERT dbo.MyTable ON";
    command.ExecuteNonQuery();

    //
    //  ....Other SQL etc....
    //
    transaction.Commit();
}
```

## Debugging

There are two events, QueryExecuting and QueryPerformed that can be used to debug queries.

QueryExecuting fires before the query is executed and QueryPerformed fires after execution or when an exception is thrown.

Please Note: The elapsedTime parameter is measured using the stopwatch class. So this will have a more accurate value than subtracting the start and end date parameters.

```C#
QueryLite.Settings.QueryExecuting += Settings_QueryExecuting;
QueryLite.Settings.QueryPerformed += Settings_QueryPerformed;

void Settings_QueryExecuting(QueryExecutingDetail queryDetail) {

    IDatabase database = queryDetail.Database;
    string sql = queryDetail.Sql;
    QueryType queryType = queryDetail.QueryType;
    DateTimeOffset? start = queryDetail.Start;
    System.Data.IsolationLevel isolationLevel = queryDetail.IsolationLevel;
    ulong? transactionId = queryDetail.TransactionId;
    string debugName = queryDetail.DebugName;
}

void Settings_QueryPerformed(QueryDetail queryDetail) {

    IDatabase database = queryDetail.Database;
    string sql = queryDetail.Sql;
    int rows = queryDetail.Rows;
    int rowsEffected = queryDetail.RowsEffected;
    QueryType queryType = queryDetail.QueryType;
    IQueryResult? result = queryDetail.Result;
    DateTimeOffset? start = queryDetail.Start;
    DateTimeOffset? end = queryDetail.End;
    TimeSpan? elapsedTime = queryDetail.ElapsedTime;
    Exception? exception = queryDetail.Exception;
    System.Data.IsolationLevel isolationLevel = queryDetail.IsolationLevel;
    ulong? transactionId = queryDetail.TransactionId;
    string debugName = queryDetail.DebugName;
}
```

## Breakpoint Debugging

The Settings class contains a number of properties that will cause a breakpoint to be hit in Visual Studio when executing a query.

These are the related settings:
```C#
Settings.BreakOnSelectQuery = true;
Settings.BreakOnInsertQuery = true;
Settings.BreakOnUpdateQuery = true;
Settings.BreakOnDeleteQuery = true;
Settings.BreakOnTruncateQuery = true;
```

## Schema Validation

Table definitions can be validated against a database. The validation checks for things like missing columns, correct naming, data types and nullability.

Here is a code example of calling the schema validation.

```C#
SchemaValidationSettings settings = new SchemaValidationSettings() {
    ValidatePrimaryKeys = true,
    ValidateForeignKeys = true,
    ValidateMissingCodeTables = true
};

ValidationResult result = SchemaValidator.ValidateTablesInAssembly(database, Assembly.GetExecutingAssembly(), settings);

//There are other methods like validating table in an Assembly e.g.
//ValidationResult result = SchemaValidator.ValidateTablesInCurrentDomain(database, settings);

StringBuilder output = new StringBuilder();

foreach(TableValidation tableValidation in result.TableValidation) {

    if(tableValidation.HasErrors) {

        string tableSchema = tableValidation.Table?.SchemaName ?? string.Empty;
        string tableName = tableValidation.Table?.TableName ?? string.Empty;

        foreach(string message in tableValidation.ValidationMessages) {
            output.AppendLine($"[{tableSchema}].[{tableName}] =>    {message}");
        }
        output.Append(Environment.NewLine);
    }
}
string text = output.ToString();
```

### Suppressing Column Type Validation

An attribute called `[SuppressColumnTypeValidation]` exists to suppress schema validation errors when a column type does not match the database schema.

In this case `MyColumn` might map to an unsupported database type.

```C#
public sealed class MyTable : ATable {

        public static readonly MyTable Instance = new MyTable();

        //This attribute will suppress any validation errors when the 'object' type does not map to the database schema type
        [SuppressColumnTypeValidation]
        public NullableColumn<IUnsupportedType> MyColumn { get; }

        private EmployeeTable() : base(tableName: "MyTable", schemaName: "dbo") {
            MyColumn = new NullableColumn<IUnsupportedType>(this, columnName: "MyColumn");
        }
}
```

## Database Constraints

Primary and foreign keys can be defined on table definitions. These are useful for schema validation (i.e. Checking those constraints exist in the database) and for generating schema documentation.

The ATable<> class has virtual methods called `PrimaryKey` and `ForeignKeys` that can be overridden to define the constraints. Here is an example below of how to define those constaints in code. (Note: These constraints can be generated using the code generator tool).

Example:

```sql
CREATE TABLE Parent (

	Id UNIQUEIDENTIFIER NOT NULL,
	
	CONSTRAINT pk_Parent PRIMARY KEY(Id),
	CONSTRAINT unq_Parent UNIQUE(Id2)
);

CREATE TABLE Child (

	Id UNIQUEIDENTIFIER NOT NULL,
	ParentId UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT pk_Child PRIMARY KEY(Id),
	CONSTRAINT fk_Child_Parent FOREIGN KEY(ParentId) REFERENCES Parent(Id)
);
```

```C#
using System;
using QueryLite;

namespace Tables {

    public interface IParent {}
    public interface IChild {}

    public sealed class ParentTable : ATable {

        public static readonly ParentTable Instance = new ParentTable();

        public Column<GuidKey<IParent>> Id { get; }
        public Column<Guid> Id2 { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "pk_Parent", Id);

        private ParentTable() : base(tableName:"Parent", schemaName: "dbo") {

            Id = new Column<GuidKey<IParent>>(this, columnName: "Id");
            Id2 = new Column<Guid>(this, columnName: "Id2");
        }
    }

    public sealed class ChildTable : ATable {

        public static readonly ChildTable Instance = new ChildTable();

        public Column<GuidKey<IChild>> Id { get; }
        public Column<GuidKey<IParent>> ParentId { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "pk_Child", Id);

        public override ForeignKey[] ForeignKeys => new ForeignKey[] {
            new ForeignKey(this, constraintName: "fk_Child_Parent").References(ParentId, ParentTable.Instance.Id)
        };

        private ChildTable() : base(tableName:"Child", schemaName: "dbo") {

            Id = new Column<GuidKey<IChild>>(this, columnName: "Id");
            ParentId = new Column<GuidKey<IParent>>(this, columnName: "ParentId");
        }
    }
}
```

## Table Code Generation

Query Lite has a UI application (Windows only) for generating table definitions from existing database schemas. This application can be launched by running `CodeGenerator.exe`.

The code generator works well for common data types and simple key constrains but it struggles with more complex scenarios. So sometimes the generated code may need to be manually corrected.

## Documentation Generator

HTML documentation can be generated by calling `DocumentationGenerator.GenerateForAssembly(...)`. Table classes are loaded from the provided assembly(s) and an html documentation file is generated.

```C#
string htmlDoc = DocumentationGenerator.GenerateForAssembly(new Assembly[] { Assembly.GetExecutingAssembly() });
```
Or documentation can be generated by passing a list of table instances.
```C#
List<Table> tables = new List<Table>();

//...Populate tables list...//

string htmlDoc = DocumentationGenerator.GenerateForTables(tables);
```

`Description` attributes can be added to table classes and table column properties. These descriptions are included in the generated documentation.

For example:
```C#
using QueryLite;

[Description("Stores information about territories")]
public sealed class TerritoryTable : ATable {

    public static readonly TerritoryTable Instance = new TerritoryTable();

    [Description("Territory identifier")]
    public Column<StringKey<ITerritory>> TerritoryId { get; }

    [Description("Territory description")]
    public Column<string> TerritoryDescription { get; }

    [Description("Region that territory belongs to")]
    public Column<IntKey<IRegion>> RegionId { get; }

    private TerritoryTable() : base(tableName:"Territory", schemaName: "dbo") {

        TerritoryId = new Column<StringKey<ITerritory>>(this, columnName: "TerritoryId", length: 20);
        TerritoryDescription = new Column<string>(this, columnName: "TerritoryDescription", length: 50);
        RegionId = new Column<IntKey<IRegion>>(this, columnName: "RegionId");
    }
}
```
