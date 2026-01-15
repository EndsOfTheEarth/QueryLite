# Query Lite

Query Lite is a typesafe SQL query library for .net. It has support for Sql Server, PostgreSql and Sqlite.

The library supports common queries like `select`, `update`, `delete` and `truncate`. And more complex syntax like left joins,
unions, nested queries, query hints and row locking. It also has a row repository feature which is similar in concept to EF Core's context CRUD pattern.
 
## Example Select Query (Dynamic)
``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

OrderTable orderTable = OrderTable.Instance;
OrderStatusTable orderStatusTable = OrderStatusTable.Instance;
CustomerTable customerTable = CustomerTable.Instance;

var result = Query
    .Select(
        row => new {
            OrderId = row.Get(orderTable.OrderId),
            CustomerId = row.Get(customerTable.CustomerId),
            CompanyName = row.Get(customerTable.CompanyName),
            OrderStatus = row.GetAsNull(orderStatusTable.Status)
        }
    )
    .From(orderTable)
    .Join(customerTable).On(orderTable.CustomerId == customerTable.CustomerId)
    .LeftJoin(orderStatusTable).On(orderTable.StatusId == orderStatusTable.Id)    
    .Where(
        orderTable.OrderDate < DateTime.Now &
        customerTable.ContactName == "Jane"
    )
    .OrderBy(orderTable.OrderId.ASC)
    .Execute(DB.Northwind);

string sql = result.Sql;    //Generated sql is available on the result

foreach(var row in result.Rows) {

    OrderId orderId = row.OrderId;  //Note: OrderId and CustomerId are custom types that implement ICustomType<TYPE, CUSTOM_TYPE> and can be source generated.
    CustomerId customerId = row.CustomerId;
    string companyName = row.CompanyName;
    OrderStatus? status = row.OrderStatus;
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

[Additional - Notes on Sqlite Support](Sqlite.md)

## Nuget Package

[QueryLite.net Nuget Package](https://www.nuget.org/packages/QueryLite.net/)

	Install-Package QueryLite.net

## Core features

* Typesafe queries in code.
  - Designed to work with nullable reference types and .net 10.0.
* Sql Select, insert, update, delete and truncate queries.
* Supports sql syntax e.g. Join, Left Join, Where, Order By, Group By, Union and Nested Queries.
* Does not do any client side filtering.
* Query syntax clear and easy to read.
* Dynamic queries have performance similar to Dapper.
* Prepared queries have performance similar to raw Ado.net sql.
* Efficient memory allocation.
* No slow context start up times.
* Debugging features that show sql queries being executed.
  - Events Like - QueryExecuting, QueryPerformed.
  - Query results contain the executed sql.
  - Breakpoint on query.
* Code generation tool for table definitions.
* Table definition validator.
* Supports both Sql Server, PostgreSql and Sqlite.
* Schema description attributes to allow documentation generation.

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

CustomerId customerId = CustomerId.ValueOf("ABC");

using(Transaction transaction = new(DB.Northwind)) {

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

CustomerId customerId = CustomerId.ValueOf("ABC");

using(Transaction transaction = new(DB.Northwind)) {

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

CustomerId customerId = CustomerId.ValueOf("ABC");

using(Transaction transaction = new(DB.Northwind)) {

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

public sealed class CustomerTable : ATable {

    public static readonly CustomerTable Instance = new();
    
    public Column<CustomerId, string> CustomerId { get; }
    public Column<string> CompanyName { get; }
    public NColumn<string> ContactName { get; }
    public NColumn<string> ContactTitle { get; }
    public NColumn<string> Address { get; }
    public NColumn<string> City { get; }
    public NColumn<string> Region { get; }
    public NColumn<string> PostalCode { get; }
    public NColumn<string> Country { get; }
    public NColumn<string> Phone { get; }
    public NColumn<string> Fax { get; }

    private CustomerTable() : base(name:"Customer", schemaName: "dbo") {
        CustomerId = new Column<CustomerId, string>(this, name: "CustomerId", length: new(5));
        CompanyName = new Column<string>(this, name: "CompanyName", length: new(40));
        ContactName = new NColumn<string>(this, name: "ContactName", length: new(30));
        ContactTitle = new NColumn<string>(this, name: "ContactTitle", length: new(30));
        Address = new NColumn<string>(this, name: "Address", length: new(60));
        City = new NColumn<string>(this, name: "City", length: new(15));
        Region = new NColumn<string>(this, name: "Region", length: new(15));
        PostalCode = new NColumn<string>(this, name: "PostalCode", length: new(10));
        Country = new NColumn<string>(this, name: "Country", length: new(15));
        Phone = new NColumn<string>(this, name: "Phone", length: new(24));
        Fax = new NColumn<string>(this, name: "Fax", length: new(24));
    }
}

public sealed class OrderTable : ATable {

    public static readonly OrderTable Instance = new();
    
    public Column<OrderId, int> OrderId { get; }
    public NColumn<CustomerId, string> CustomerId { get; }
    public NColumn<EmployeeId, int> EmployeeId { get; }
    public NColumn<DateTime> OrderDate { get; }
    public NColumn<DateTime> RequiredDate { get; }
    public NColumn<DateTime> ShippedDate { get; }
    public NColumn<ShipperId, int> ShipVia { get; }
    public NColumn<decimal> Freight { get; }
    public NColumn<string> ShipName { get; }
    public NColumn<string> ShipAddress { get; }
    public NColumn<string> ShipCity { get; }
    public NColumn<string> ShipRegion { get; }
    public NColumn<string> ShipPostalCode { get; }
    public NColumn<string> ShipCountry { get; }

    private OrderTable() : base(name:"Order", schemaName: "dbo", enclose: true) {
        OrderId = new Column<OrderId, int>(this, name: "OrderId", isAutoGenerated: true);
        CustomerId = new NColumn<CustomerId, string>(this, name: "CustomerId", length: new(5));
        EmployeeId = new NColumn<EmployeeId, int>(this, name: "EmployeeId");
        OrderDate = new NColumn<DateTime>(this, name: "OrderDate");
        RequiredDate = new NColumn<DateTime>(this, name: "RequiredDate");
        ShippedDate = new NColumn<DateTime>(this, name: "ShippedDate");
        ShipVia = new NColumn<ShipperId, int>(this, name: "ShipVia");
        Freight = new NColumn<decimal>(this, name: "Freight");
        ShipName = new NColumn<string>(this, name: "ShipName", length: new(40));
        ShipAddress = new NColumn<string>(this, name: "ShipAddress", length: new(60));
        ShipCity = new NColumn<string>(this, name: "ShipCity", length: new(15));
        ShipRegion = new NColumn<string>(this, name: "ShipRegion", length: new(15));
        ShipPostalCode = new NColumn<string>(this, name: "ShipPostalCode", length: new(10));
        ShipCountry = new NColumn<string>(this, name: "ShipCountry", length: new(15));
    }
}
```

Here is an example of a custom type defintion. Please note: These can be source generated using a third party library if needed.

```C#

public readonly struct CustomerId : ICustomType<string, CustomerId>, IEquatable<CustomerId> {

    public string Value { get; }

    public CustomerId(string value) {
        Value = value;
    }
    public static CustomerId ValueOf(string value) {
        return new CustomerId(value);
    }
    public bool Equals(CustomerId other) {
        return Value == other.Value;
    }

    public static bool operator ==(CustomerId? pA, CustomerId? pB) {

        if(pA is null && pB is null) {
            return true;
        }
        if(pA is not null) {
            return pA.Equals(pB);
        }
        return false;
    }
    public static bool operator !=(CustomerId? pA, CustomerId? pB) {

        if(pA is null && pB is null) {
            return false;
        }
        if(pA is not null) {
            return !pA.Equals(pB);
        }
        return true;
    }

    public override bool Equals(object? obj) {

        if(obj is CustomerId identifier) {
            return Value.CompareTo(identifier.Value) == 0;
        }
        return false;
    }
    public override int GetHashCode() {
        return Value.GetHashCode();
    }
    public override string ToString() {
        return Value.ToString() ?? "";
    }
}
```

## Code Generator Tool

The code generator tool can be used to generate database schema definitions. The application CodeGenerator.exe can be downloaded from the releases page: [https://github.com/EndsOfTheEarth/QueryLite/releases](https://github.com/EndsOfTheEarth/QueryLite/releases)

![QueryLite Core Generator](https://github.com/user-attachments/assets/f257be9c-bb6c-455e-baaf-8f11fc15ce40)

## Query Monitoring Events

Currently executing queries can be monitored by subscribing the to the `QueryExecuting` or `QueryPerformed` events.

``` C#
QueryLite.Settings.QueryExecuting += Settings_QueryExecuting;
QueryLite.Settings.QueryPerformed += Settings_QueryPerformed;

//Called when an sql query is about to be executed
void Settings_QueryExecuting(QueryExecutingDetail queryDetail) {

    IDatabase database = queryDetail.Database;
    string sql = queryDetail.Sql;
    QueryType queryType = queryDetail.QueryType;
    DateTimeOffset? start = queryDetail.Start;
    System.Data.IsolationLevel isolationLevel = queryDetail.IsolationLevel;
    ulong? transactionId = queryDetail.TransactionId;
    QueryTimeout timeout = queryDetail.QueryTimeout;
    string debugName = queryDetail.DebugName;
}

//Called when an sql query is executed
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
    QueryTimeout timeout = queryDetail.QueryTimeout;
    string debugName = queryDetail.DebugName;
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
- ARepository queries uses cached sql queries which lowers memory use. But the track row changes functionality adds additional memory usage.
- EF Core - Select queries allocate more memory than the other queries except ARepository (Which is similar). Select queries with a 'where' clause can allocate significantly more memory.

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
* `QueryLite_Single_Row_Repository_Select` = Query Lite select using ARepository. (Note: This uses additional memory to track row changes).
* `EF_Core_Single_Row_Select` = EF Core query. (Note: This uses additional memory to track row changes)

These tests are running for 2000 sequential iterations. So the results should be divided by 2000 to give 'per query' values.

These tests measure sequential execution times (non async). Executing queries as async in a multi-request environment will yield much higher throughput (Total queries per second) for all query methods.

### Select Single Row To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01 WHERE row_guid=@0
```
| Method                                 | Mean     | Error   | StdDev  | Gen0      | Allocated |
|--------------------------------------- |---------:|--------:|--------:|----------:|----------:|
| Ado_Single_Row_Select                  | 154.5 ms | 1.20 ms | 1.06 ms |         - |   2.84 MB |
| Dapper_Single_Row_Select               | 152.2 ms | 1.42 ms | 1.33 ms |         - |   3.19 MB |
| QueryLite_Single_Row_Prepared_Select   | 158.2 ms | 2.05 ms | 1.82 ms |         - |   2.93 MB |
| QueryLite_Single_Row_Dynamic_Select    | 159.6 ms | 2.28 ms | 2.02 ms |         - |   3.89 MB |
| QueryLite_Single_Row_Repository_Select | 190.8 ms | 2.10 ms | 1.86 ms |         - |   4.72 MB |
| EF_Core_Single_Row_Select              | 318.9 ms | 2.40 ms | 2.13 ms | 6000.0000 | 108.16 MB |

 - Repository has higher memory allocation because of change tracking.
 - For EF Core it appears the WHERE caluse evaluation increases memory allocation significantly.

### Select Ten Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```
| Method                              | Mean     | Error   | StdDev  | Gen0      | Allocated |
|------------------------------------ |---------:|--------:|--------:|----------:|----------:|
| Ado_Ten_Row_Select                  | 139.2 ms | 1.77 ms | 1.48 ms |         - |   4.43 MB |
| Dapper_Ten_Row_Select               | 137.7 ms | 2.14 ms | 1.90 ms |         - |   6.76 MB |
| QueryLite_Ten_Row_Prepared_Select   | 143.2 ms | 2.14 ms | 1.89 ms |         - |   4.43 MB |
| QueryLite_Ten_Row_Dynamic_Select    | 143.9 ms | 1.60 ms | 1.50 ms |         - |   5.16 MB |
| QueryLite_Ten_Row_Repository_Select | 186.2 ms | 2.33 ms | 2.18 ms |         - |   7.44 MB |
| EF_Core_Ten_Row_Select              | 268.0 ms | 2.50 ms | 2.34 ms | 7000.0000 | 114.13 MB |

- Without a WHERE clause, EF Core has similar memory allocation to the Repository.

### Select Ten Rows To List With WHERE clause (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01 WHERE date < '9999-12-31'
```
| Method                              | Mean     | Error   | StdDev  | Gen0      | Gen1      | Allocated |
|------------------------------------ |---------:|--------:|--------:|----------:|----------:|----------:|
| Ado_Ten_Row_Select                  | 165.3 ms | 2.98 ms | 4.46 ms |         - |         - |   4.43 MB |
| Dapper_Ten_Row_Select               | 160.6 ms | 1.48 ms | 1.32 ms |         - |         - |   6.76 MB |
| QueryLite_Ten_Row_Prepared_Select   | 161.6 ms | 2.11 ms | 1.87 ms |         - |         - |   5.71 MB |
| QueryLite_Ten_Row_Dynamic_Select    | 163.9 ms | 2.76 ms | 2.95 ms |         - |         - |   6.65 MB |
| QueryLite_Ten_Row_Repository_Select | 203.3 ms | 3.98 ms | 4.26 ms |         - |         - |   8.96 MB |
| EF_Core_Ten_Row_Select              | 313.1 ms | 2.41 ms | 1.88 ms | 7000.0000 | 1000.0000 | 119.32 MB |

- With a WHERE clause, EF Core increases memory allocation significantly.

### Select One Hundred Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```
| Method                                      | Mean     | Error   | StdDev  | Gen0       | Gen1      | Allocated |
|-------------------------------------------- |---------:|--------:|--------:|-----------:|----------:|----------:|
| Ado_One_Hundred_Row_Select                  | 178.9 ms | 1.53 ms | 1.36 ms |  1000.0000 |         - |  29.95 MB |
| Dapper_One_Hundred_Row_Select               | 179.1 ms | 1.86 ms | 1.65 ms |  2000.0000 |         - |  46.02 MB |
| QueryLite_One_Hundred_Row_Prepared_Select   | 184.4 ms | 1.38 ms | 1.22 ms |  1000.0000 |         - |  29.95 MB |
| QueryLite_One_Hundred_Row_Dynamic_Select    | 187.2 ms | 3.74 ms | 3.67 ms |  1000.0000 |         - |  30.69 MB |
| QueryLite_One_Hundred_Row_Repository_Select | 261.6 ms | 2.51 ms | 2.35 ms |  3000.0000 |         - |  49.45 MB |
| EF_Core_One_Hundred_Row_Select              | 377.5 ms | 4.33 ms | 3.61 ms | 17000.0000 | 3000.0000 | 273.67 MB |

- Dapper memory allocation increases well above QueryLite dynamic as the number of rows returns increases.
- EF Core memory allocation drops below the repository pattern.

### Select One Thousand Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```

| Method                                       | Mean       | Error   | StdDev  | Gen0        | Gen1       | Allocated  |
|--------------------------------------------- |-----------:|--------:|--------:|------------:|-----------:|-----------:|
| Ado_One_Thousand_Row_Select                  |   526.1 ms | 1.48 ms | 1.15 ms |  17000.0000 |  5000.0000 |  277.16 MB |
| Dapper_One_Thousand_Row_Select               |   526.1 ms | 3.76 ms | 3.33 ms |  26000.0000 |  8000.0000 |  430.56 MB |
| QueryLite_One_Thousand_Row_Prepared_Select   |   537.8 ms | 2.27 ms | 1.90 ms |  17000.0000 |  8000.0000 |  277.16 MB |
| QueryLite_One_Thousand_Row_Dynamic_Select    |   535.4 ms | 1.87 ms | 1.46 ms |  17000.0000 |  9000.0000 |  277.89 MB |
| QueryLite_One_Thousand_Row_Repository_Select |   924.2 ms | 7.41 ms | 6.93 ms |  28000.0000 | 18000.0000 |  461.45 MB |
| EF_Core_One_Thousand_Row_Select              | 1,147.1 ms | 4.63 ms | 4.33 ms | 116000.0000 | 53000.0000 | 1861.03 MB |

- Dapper memory allocation increases well above QueryLite dynamic as the number of rows returns increases.
- EF Core memory allocation increases well above the repository pattern.

### Insert Single Row (2000 Sequential Iterations)

```SQL
INSERT INTO Test01 (row_guid,message,date) VALUES(@0, @1, @2)
```
| Method                                    | Mean     | Error   | StdDev  | Gen0      | Allocated |
|------------------------------------------ |---------:|--------:|--------:|----------:|----------:|
| Ado_Single_Insert                         | 358.0 ms | 3.76 ms | 3.51 ms |         - |   3.37 MB |
| Dapper_Single_Insert                      | 356.3 ms | 2.57 ms | 2.40 ms |         - |   3.49 MB |
| QueryLite_Single_Compiled_Insert          | 362.4 ms | 2.39 ms | 2.24 ms |         - |   3.59 MB |
| QueryLite_Single_Dynamic_Insert           | 362.8 ms | 2.24 ms | 1.99 ms |         - |   4.49 MB |
| QueryLite_Single_Repository_Insert        | 380.6 ms | 1.78 ms | 1.58 ms |         - |   4.58 MB |
| QueryLite_Single_Repository_Static_Insert | 377.7 ms | 2.02 ms | 1.89 ms |         - |   3.88 MB |
| EF_Core_Single_Insert                     | 649.0 ms | 3.30 ms | 2.92 ms | 7000.0000 | 114.65 MB |

- EF Core allocates significantly more memory.

### Update Single Row (2000 Sequential Iterations)

```SQL
UPDATE Test01 SET message=@1,date=@2 WHERE row_guid=@0
```

| Method                                 | Mean     | Error   | StdDev  | Gen0      | Gen1      | Allocated |
|--------------------------------------- |---------:|--------:|--------:|----------:|----------:|----------:|
| Ado_Single_Row_Update                  | 431.1 ms | 1.45 ms | 1.21 ms |         - |         - |   3.36 MB |
| Dapper_Single_Row_Update               | 430.6 ms | 3.08 ms | 2.88 ms |         - |         - |   3.56 MB |
| QueryLite_Single_Row_Prepared_Update   | 436.8 ms | 3.10 ms | 2.90 ms |         - |         - |   3.57 MB |
| QueryLite_Single_Row_Dynamic_Update    | 436.5 ms | 8.49 ms | 8.72 ms |         - |         - |   4.71 MB |
| QueryLite_Single_Row_Repository_Update | 674.3 ms | 2.47 ms | 2.31 ms |         - |         - |   8.28 MB |
| EF_Core_Single_Row_Update              | 781.1 ms | 4.30 ms | 3.81 ms | 7000.0000 | 1000.0000 |  126.2 MB |

- EF Core allocates significantly more memory.

### Delete Single Row (2000 Sequential Iterations)

```SQL
DELETE FROM Test01 WHERE row_guid=@0
```

| Method                                 | Mean     | Error   | StdDev  | Gen0      | Allocated |
|--------------------------------------- |---------:|--------:|--------:|----------:|----------:|
| Ado_Single_Row_Delete                  | 221.4 ms | 3.41 ms | 2.85 ms |         - |   2.43 MB |
| Dapper_Single_Row_Delete               | 221.1 ms | 4.28 ms | 5.56 ms |         - |   2.64 MB |
| QueryLite_Single_Row_Prepared_Delete   | 228.0 ms | 3.22 ms | 2.86 ms |         - |   2.62 MB |
| QueryLite_Single_Row_Dynamic_Delete    | 227.0 ms | 3.96 ms | 3.51 ms |         - |   3.27 MB |
| QueryLite_Single_Row_Repository_Delete | 429.1 ms | 2.80 ms | 2.62 ms |         - |      6 MB |
| EF_Core_Single_Row_Delete              | 654.1 ms | 6.28 ms | 5.88 ms | 7000.0000 | 115.46 MB |

- EF Core allocates significantly more memory.