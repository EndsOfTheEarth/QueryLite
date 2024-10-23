# Query Lite

Query Lite is a typesafe .net sql query library for Sql Server and PostgreSql. In essence it is a `typesafe` `string less` `sql query builder`. Sql queries like `select`, `update`, `delete` and `truncate` are supported.
 
## Example Select Query (Dynamic)
``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

OrderTable orderTable = OrderTable.Instance;
CustomerTable customerTable = CustomerTable.Instance;

var result = Query
    .Select(
        row => new {
            OrderId = row.Get(orderTable.OrderID),
            CustomerId = row.Get(customerTable.CustomerID),
            CompanyName = row.Get(customerTable.CompanyName)
        }
    )
    .From(orderTable)
    .Join(customerTable).On(orderTable.CustomerID == customerTable.CustomerID)
    .Where(orderTable.OrderDate < DateTime.Now & customerTable.ContactName == "Jane")
    .OrderBy(orderTable.OrderID.ASC)
    .Execute(DB.Northwind);

string sql = result.Sql;    //Generated sql is available on the result

foreach(var row in result.Rows) {

    IntKey<IOrder> orderId = row.OrderId;
    StringKey<ICustomer> customerId = row.CustomerId;
    string companyName = row.CompanyName;
}
```

## Design Goals

It is designed to achieve these main goals:

1. Typesafe database queries and schema
    - Queries, joins, 'where' conditions, column types and column nullability are enforced by the compiler
2. Runtime validation between code schema and database schema
    - Check for missing tables, missing columns, column type and nullability differences
3. Debuggable
    - Show detailed information about sql queries during and after execution
4. Performance
    - `Prepared` queries are near equivalent performance and memory allocation to direct ado.net code
    - `Dynamic` queries are near equivalent performance and memory allocation to Dapper (Often with significantly lower memory allocation)

## Project State

Query Lite is part of a set of personal projects exploring the optimal design for a query library. The library uses less abstraction for simplicity and performance reasons but the downside is that it has more direct dependencies on Sql Server and PostgreSql.

**Please Note: Query Lite has not been used in a production environment as of yet and I consider the code to be a beta version. So any testing and feedback is appreciated.**

## Getting Started Guide
**[Getting Started Guide is found here](GettingStarted.md)**
## Documentation
**[Documentation is found here](Documentation.md)**

[Additional - Prepared Query Documentation is found here](PreparedQueries.md)

## Nuget Package

[QueryLite.net Nuget Package](https://www.nuget.org/packages/QueryLite.net/)

	Install-Package QueryLite.net

## Core features

* Typesafe queries in code
  - Designed to work with nullable reference types and .net 8.0
* Sql Select, insert, update, delete and truncate queries
* Supports sql syntax e.g. Join, Left Join, Where, Order By, Group By, Union and Nested Queries
* Does not do any client side filtering like linq to sql
* Debugging features that show sql queries being executed
  - Events Like - QueryExecuting, QueryPerformed
  - Query results contain the executed sql
  - Breakpoint on query
* Code generation tool for table definitions
* Table definition validator
* Supports both Sql Server and PostgreSql
* Schema description attributes to allow documentation generation

## Dynamic And Prepared Queries

Query lite implements both dynamic and prepared queries. Prepared queries allocate less memory on the heap during query execution. This gives them very similar performance (And garbage collector load) to lower level ado.net code. The downside is that prepared queries have multiple generic constraints which make them syntactically more complicated to write.

On the other hand, dynamic queries are syntactically simpler but allocate more memory during query execution. This is due to dynamically building the sql query string on every execution. The additional memory allocation makes dynamic queries use more memory than pure ado.net code but similar memory use to libraries like Dapper. See the [benchmarks](#benchmarks) section for PostgreSql memory allocation statistics.

[Prepared Query Documentation is found here](PreparedQueries.md)

## What main problem(s) does Query Lite solve?

Query Lite is suited to code bases that have larger database schemas. It increases the maintainability of larger code bases by providing query level type safety. It removes the need for sql strings in code and error prone mapping between query results and entity object classes.

Here are a number of problems it addresses:

### 1) Renaming schema table columns

Database libraries often embed sql as a string in code. So renaming a table column can be problematic, as it involves the programmer searching for each use of that column in the code base. Missing one of those column references will most likely cause a runtime error.
Query Lite solves this problem by having only one place where the column name needs to be edited (In the table definition class).

### 2) Changing a column data type
Much like renaming a column, changing a column's data type is often an exercise in searching code for uses of that column. Bugs can be introduced if any of those uses are not updated correctly.
Query Lite solves this problem by using type safe column types. The type of the column is defined once in the code. After changing the column type in code, any incorrect use of that column will fail at compile time.

### 3) Schema differences between the database version and code cause runtime errors

Schema differences between the code and the database often cause runtime failures. These can be difficult and time consuming to identify. One use case is where someone creates a new testing environment but forgets to run an sql upgrade script or fails to notice the script failed halfway.

Query Lite addresses this problem with a runtime schema validator. This compares the code schema with the database and outputs any differences. These include missing columns, incorrect data types and nullability differences.

The schema validation could be made available from a webservice endpoint. For example: `https://localhost/ValidateSchema`. Another option is to log the schema validation at startup.

### 4) Difficult to view / debug underlying sql query

Many database access libraries hide away the specific sql queries they are executing and this can lead to hidden performance problems. Query Lite has events that can be subscribed to that provide information like - sql query, execution time, reply size, isolation level e.t.c. These events can be used to show the queries going to the server in real time and make it much easier to spot any obvious issues while developing or testing.

Also the query result classes has a property that stores the sql query.

### 5) Maintaining schema documentation is difficult

Schema documentation is difficult to maintain. Query Lite is able to generate schema documentation using the code table and column description attributes. This allows for the human readable descriptions to be stored and maintained in code.

## Example Insert Query (Dynamic)
``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

StringKey<ICustomer> customerId = new StringKey<ICustomer>("ABC");

using(Transaction transaction = new Transaction(DB.Northwind)) {

    CustomerTable table = CustomerTable.Instance;

    var result = Query.Insert(table)
        .Values(values => values
            .Set(table.CustomerID, customerId)
            .Set(table.CompanyName, "company name")
            .Set(table.ContactName, "contact name")
            .Set(table.ContactTitle, "title")
            .Set(table.Address, "address")
            .Set(table.City, "city")
            .Set(table.Region, "region")
            .Set(table.PostalCode, "12345")
            .Set(table.Country, "somewhere")
            .Set(table.Phone, "312-12312-123")
            .Set(table.Fax, null)
        )
        .Execute(
            inserted => new {   //Return the two updated fields, PostalCode and Address
                PostalCode = inserted.Get(table.PostalCode),
                Address = inserted.Get(table.Address)
            },
            transaction
        );

    var row = result.Rows.First();

    string? postalCode = row.PostalCode;
    string? address = row.Address;

    transaction.Commit();
}
```

## Example Update Query (Dynamic)

``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

StringKey<ICustomer> customerId = new StringKey<ICustomer>("ABC");

using(Transaction transaction = new Transaction(DB.Northwind)) {

    CustomerTable table = CustomerTable.Instance;

    Query.Update(table)
        .Values(values => values
            .Set(table.ContactTitle, "Mrs")
            .Set(table.ContactName, "Mary")
        )
        .Where(table.CustomerID == customerId)
        .Execute(transaction);

    transaction.Commit();
}
```

## Example Delete Query (Dynamic)

``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

StringKey<ICustomer> customerId = new StringKey<ICustomer>("ABC");

using(Transaction transaction = new Transaction(DB.Northwind)) {

    CustomerTable table = CustomerTable.Instance;

    NonQueryResult result = Query
        .Delete(table)
        .Where(table.CustomerID == customerId)
        .Execute(transaction);

    if(result.RowsEffected > 1) {
        throw new Exception($"Deleted {result.RowsEffected} records. Should have only deleted one row.");
    }
    transaction.Commit();
}
```

## Table Definitions

A schema of table definitions must be defined in code. This can be defined either by hand or using the code generator tool.

Here is an example taken from the Northwind database:

```SQL
CREATE TABLE "Customer" (

  "CustomerId" NCHAR(5) NOT NULL ,
  "CompanyName" NVARCHAR(40) NOT NULL ,
  "ContactName" NVARCHAR(30) NULL ,
  "ContactTitle" NVARCHAR(30) NULL ,
  "Address" NVARCHAR(60) NULL ,
  "City" NVARCHAR(15) NULL ,
  "Region" NVARCHAR(15) NULL ,
  "PostalCode" NVARCHAR(10) NULL ,
  "Country" NVARCHAR(15) NULL ,
  "Phone" NVARCHAR(24) NULL ,
  "Fax" NVARCHAR(24) NULL ,
  
  CONSTRAINT "pk_Customer" PRIMARY KEY  CLUSTERED ("CustomerId")
);

CREATE TABLE "Order" (

  "OrderId" INT IDENTITY(1,1) NOT NULL ,
  "CustomerId" NCHAR(5) NULL ,
  "EmployeeId" INT NULL ,
  "OrderDate" DATETIME NULL ,
  "RequiredDate" DATETIME NULL ,
  "ShippedDate" DATETIME NULL ,
  "ShipVia" INT NULL ,
  "Freight" MONEY NULL CONSTRAINT "DF_Orders_Freight" DEFAULT(0),
  "ShipName" NVARCHAR(40) NULL ,
  "ShipAddress" NVARCHAR(60) NULL ,
  "ShipCity" NVARCHAR(15) NULL ,
  "ShipRegion" NVARCHAR(15) NULL ,
  "ShipPostalCode" NVARCHAR(10) NULL ,
  "ShipCountry" NVARCHAR(15) NULL ,
  
  CONSTRAINT "pk_Orders" PRIMARY KEY  CLUSTERED ("OrderId"),
  CONSTRAINT "fk_Orders_Customer" FOREIGN KEY ("CustomerId") REFERENCES "dbo"."Customer" ("CustomerId"),
  CONSTRAINT "fk_Orders_Employees" FOREIGN KEY ("EmployeeId") REFERENCES "dbo"."Employee" ("EmployeeId"),
  CONSTRAINT "fk_Orders_Shipper" FOREIGN KEY ("ShipVia") REFERENCES "dbo"."Shipper" ("ShipperId")
);
```

``` C#

using QueryLite;
using QueryLite.Databases.SqlServer;

public static class DB {

    //Create a database
    public static IDatabase Northwind { get; set; } = new SqlServerDatabase(
        name: "Northwind",
        connectionString: "Server=localhost;Database=Northwind;Trusted_Connection=True;"
    );
}

//Empty interfaces for type safe key columns
public interface ICustomer { }
public interface IOrder { }
public interface IEmployee { }
public interface IShipper { }

public sealed class CustomerTable : ATable {

    public static readonly CustomerTable Instance = new CustomerTable();
    public static readonly CustomerTable Instance2 = new CustomerTable();
    public static readonly CustomerTable Instance3 = new CustomerTable();
    
    public Column<StringKey<ICustomer>> CustomerId { get; }
    public Column<string> CompanyName { get; }
    public NullableColumn<string> ContactName { get; }
    public NullableColumn<string> ContactTitle { get; }
    public NullableColumn<string> Address { get; }
    public NullableColumn<string> City { get; }
    public NullableColumn<string> Region { get; }
    public NullableColumn<string> PostalCode { get; }
    public NullableColumn<string> Country { get; }
    public NullableColumn<string> Phone { get; }
    public NullableColumn<string> Fax { get; }

    private CustomerTable() : base(tableName:"Customer", schemaName: "dbo") {
        CustomerId = new Column<StringKey<ICustomer>>(this, columnName: "CustomerId", length: 5);
        CompanyName = new Column<string>(this, columnName: "CompanyName", length: 40);
        ContactName = new NullableColumn<string>(this, columnName: "ContactName", length: 30);
        ContactTitle = new NullableColumn<string>(this, columnName: "ContactTitle", length: 30);
        Address = new NullableColumn<string>(this, columnName: "Address", length: 60);
        City = new NullableColumn<string>(this, columnName: "City", length: 15);
        Region = new NullableColumn<string>(this, columnName: "Region", length: 15);
        PostalCode = new NullableColumn<string>(this, columnName: "PostalCode", length: 10);
        Country = new NullableColumn<string>(this, columnName: "Country", length: 15);
        Phone = new NullableColumn<string>(this, columnName: "Phone", length: 24);
        Fax = new NullableColumn<string>(this, columnName: "Fax", length: 24);
    }
}

public sealed class OrderTable : ATable {

    public static readonly OrderTable Instance = new OrderTable();
    public static readonly OrderTable Instance2 = new OrderTable();
    public static readonly OrderTable Instance3 = new OrderTable();
    
    public Column<IntKey<IOrder>> OrderId { get; }
    public NullableColumn<StringKey<ICustomer>> CustomerId { get; }
    public NullableColumn<IntKey<IEmployee>> EmployeeId { get; }
    public NullableColumn<DateTime> OrderDate { get; }
    public NullableColumn<DateTime> RequiredDate { get; }
    public NullableColumn<DateTime> ShippedDate { get; }
    public NullableColumn<IntKey<IShipper>> ShipVia { get; }
    public NullableColumn<decimal> Freight { get; }
    public NullableColumn<string> ShipName { get; }
    public NullableColumn<string> ShipAddress { get; }
    public NullableColumn<string> ShipCity { get; }
    public NullableColumn<string> ShipRegion { get; }
    public NullableColumn<string> ShipPostalCode { get; }
    public NullableColumn<string> ShipCountry { get; }

    private OrderTable() : base(tableName:"Order", schemaName: "dbo", enclose: true) {
        OrderId = new Column<IntKey<IOrder>>(this, columnName: "OrderId", isAutoGenerated: true, length: null);
        CustomerId = new NullableColumn<StringKey<ICustomer>>(this, columnName: "CustomerId", length: 5);
        EmployeeId = new NullableColumn<IntKey<IEmployee>>(this, columnName: "EmployeeId", length: null);
        OrderDate = new NullableColumn<DateTime>(this, columnName: "OrderDate", length: null);
        RequiredDate = new NullableColumn<DateTime>(this, columnName: "RequiredDate", length: null);
        ShippedDate = new NullableColumn<DateTime>(this, columnName: "ShippedDate", length: null);
        ShipVia = new NullableColumn<IntKey<IShipper>>(this, columnName: "ShipVia", length: null);
        Freight = new NullableColumn<decimal>(this, columnName: "Freight", length: null);
        ShipName = new NullableColumn<string>(this, columnName: "ShipName", length: 40);
        ShipAddress = new NullableColumn<string>(this, columnName: "ShipAddress", length: 60);
        ShipCity = new NullableColumn<string>(this, columnName: "ShipCity", length: 15);
        ShipRegion = new NullableColumn<string>(this, columnName: "ShipRegion", length: 15);
        ShipPostalCode = new NullableColumn<string>(this, columnName: "ShipPostalCode", length: 10);
        ShipCountry = new NullableColumn<string>(this, columnName: "ShipCountry", length: 15);
    }
}
```

## Code Generator Tool

The code generator tool can be used to generate database schema definitions. The application CodeGenerator.exe can be downloaded from the releases page: [https://github.com/EndsOfTheEarth/QueryLite/releases](https://github.com/EndsOfTheEarth/QueryLite/releases)

![QueryLite Core Generator](https://github.com/EndsOfTheEarth/QueryLite/assets/6175921/898d6de3-f631-42f1-b318-d34acd882e35)

## Query Monitoring Events

Currently executing queries can be monitored by subscribing the to the `QueryExecuting` or `QueryPerformed` events.

``` C#
QueryLite.Settings.QueryExecuting += Settings_QueryExecuting;

//Called when an sql query is about to be executed
void Settings_QueryExecuting(IDatabase database, string sql, QueryType queryType, DateTimeOffset? start, System.Data.IsolationLevel isolationLevel, ulong? transactionId) {
    throw new NotImplementedException();
}

QueryLite.Settings.QueryPerformed += Settings_QueryPerformed;

//Called when an sql query is executed
void Settings_QueryPerformed(IDatabase database, string sql, int rows, int rowsEffected, QueryType queryType, DateTimeOffset? start, DateTimeOffset? end, TimeSpan? elapsedTime, Exception? exception, System.Data.IsolationLevel isolationLevel, ulong? transactionId) {
    
}
```

## Benchmarks

Benchmarks against PostgreSql are located in the Benchmarks project for reference and only cover simple scenarios.

Summary:
- Prepared queries have CPU and memory allocation very similar to directly using an ado.net command.
- Dynamic queries are similar in CPU and memory allocation to Dapper.
- Dapper memory usage increases significantly with the number of rows returned.
    - Possibly due to auto-boxing of value types in the result?
- Dynamic inserts and updates allocate more memory due to object creation in the .Set(..., ...) methods.
    - Using a prepared insert or update might be preferable when memory allocation is an issue

This is the table structure being used in these benchmarks:
```SQL
CREATE TABLE Test01 (	
	id SERIAL NOT NULL PRIMARY KEY,
	row_guid UUID NOT NULL,
	message VARCHAR(100) NOT NULL,
	date TIMESTAMP NOT NULL
);
```

* `Ado_Single_Row_Select` = Direct ado.net query
* `Dapper_Single_Row_Select` = Dapper query
* `QueryLite_Single_Row_Prepared_Select` = Query Lite prepared query
* `QueryLite_Single_Row_Dynamic_Select` = Query Lite dynamic query

These tests are running for 2000 sequential iterations. So the results should be divided by 2000 to give 'per query' values.

These tests measure sequential execution times (non async). Executing queries as async in a multi-request environment will yield much higher throughput (Total queries per second) for all query methods.

### Select Single Row To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0
```
|                               Method |     Mean |   Error |  StdDev | Allocated |
|------------------------------------- |---------:|--------:|--------:|----------:|
|                Ado_Single_Row_Select | 122.7 ms | 2.43 ms | 3.79 ms | 3.13 MB |
|             Dapper_Single_Row_Select | 123.9 ms | 2.41 ms | 2.96 ms | 3.88 MB |
| QueryLite_Single_Row_Prepared_Select | 121.2 ms | 2.23 ms | 1.98 ms | 3.16 MB |
|  QueryLite_Single_Row_Dynamic_Select | 123.4 ms | 1.25 ms | 1.10 ms | 4.23 MB |

### Select Ten Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```
|                            Method |     Mean |   Error |  StdDev |   Median | Allocated |
|---------------------------------- |---------:|--------:|--------:|---------:|----------:|
|                Ado_Ten_Row_Select | 107.3 ms | 2.11 ms | 2.67 ms | 105.8 ms | 5.22 MB |
|             Dapper_Ten_Row_Select | 115.4 ms | 2.29 ms | 3.05 ms | 114.5 ms | 7.31 MB |
| QueryLite_Ten_Row_Prepared_Select | 109.4 ms | 2.15 ms | 3.35 ms | 107.6 ms | 5.25 MB |
|  QueryLite_Ten_Row_Dynamic_Select | 110.0 ms | 1.66 ms | 1.56 ms | 109.5 ms | 6.09 MB |


### Select One Hundred Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```
|                                    Method |     Mean |   Error |  StdDev |      Gen0 |      Gen1 | Allocated |
|------------------------------------------ |---------:|--------:|--------:|----------:|----------:|----------:|
|                Ado_One_Hundred_Row_Select | 152.4 ms | 1.30 ms | 1.22 ms | 1000 | 1000 |  30.75 MB |
|             Dapper_One_Hundred_Row_Select | 199.8 ms | 0.96 ms | 0.80 ms | 2000 | 1000 | 46.57 MB |
| QueryLite_One_Hundred_Row_Prepared_Select | 157.5 ms | 0.51 ms | 0.45 ms | 1000 | 1000 |  30.78 MB |
|  QueryLite_One_Hundred_Row_Dynamic_Select | 161.0 ms | 1.72 ms | 1.53 ms | 1000 | 1000 |  31.62 MB |


### Select One Thousand Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```

|                                     Method |     Mean |   Error |  StdDev |       Gen0 |      Gen1 | Allocated |
|------------------------------------------- |---------:|--------:|--------:|-----------:|----------:|----------:|
|                Ado_One_Thousand_Row_Select | 474.1 ms | 1.18 ms | 1.10 ms | 17000 | 5000 | 277.95 MB |
|             Dapper_One_Thousand_Row_Select | 807.4 ms | 1.85 ms | 1.73 ms | 27000 | 8000 | 431.11 MB |
| QueryLite_One_Thousand_Row_Prepared_Select | 474.1 ms | 1.51 ms | 1.26 ms | 17000 | 8000 | 277.99 MB |
|  QueryLite_One_Thousand_Row_Dynamic_Select | 477.5 ms | 1.75 ms | 1.55 ms | 17000 | 8000 | 278.82 MB |

### Insert Single Row (2000 Sequential Iterations)

```SQL
INSERT INTO Test01 (row_guid,message,date) VALUES(@0, @1, @2)
```

|                           Method |     Mean |   Error |  StdDev |   Median | Allocated |
|--------------------------------- |---------:|--------:|--------:|---------:|----------:|
|                Ado_Single_Insert | 266.1 ms | 2.22 ms | 1.73 ms | 265.7 ms | 3.43 MB |
|             Dapper_Single_Insert | 275.1 ms | 5.49 ms | 8.71 ms | 270.0 ms | 3.56 MB |
| QueryLite_Single_Prepared_Insert | 269.0 ms | 1.50 ms | 1.33 ms | 268.6 ms | 3.56 MB |
|  QueryLite_Single_Dynamic_Insert | 271.3 ms | 2.26 ms | 2.11 ms | 271.1 ms | 4.73 MB |


### Update Single Row (2000 Sequential Iterations)

```SQL
UPDATE Test01 SET message=@1,date=@2 WHERE row_guid=@0
```

|                               Method |     Mean |   Error |  StdDev | Allocated |
|------------------------------------- |---------:|--------:|--------:|----------:|
|                Ado_Single_Row_Update | 300.2 ms | 0.64 ms | 0.50 ms |   3.43 MB |
|             Dapper_Single_Row_Update | 306.6 ms | 1.04 ms | 0.93 ms |   3.63 MB |
| QueryLite_Single_Row_Prepared_Update | 306.6 ms | 1.00 ms | 0.88 ms |   3.56 MB |
|  QueryLite_Single_Row_Dynamic_Update | 310.1 ms | 1.43 ms | 1.27 ms |   4.85 MB |

### Delete Single Row (2000 Sequential Iterations)

```SQL
DELETE FROM Test01 WHERE row_guid=@0
```
|                               Method |     Mean |   Error |  StdDev | Allocated |
|------------------------------------- |---------:|--------:|--------:|----------:|
|                Ado_Single_Row_Delete | 172.8 ms | 1.20 ms | 1.00 ms | 2.55 MB |
|             Dapper_Single_Row_Delete | 175.4 ms | 0.80 ms | 0.63 ms | 2.76 MB |
| QueryLite_Single_Row_Prepared_Delete | 176.3 ms | 1.00 ms | 0.83 ms | 2.70 MB |
|  QueryLite_Single_Row_Dynamic_Delete | 179.4 ms | 1.90 ms | 1.68 ms | 3.36 MB |
