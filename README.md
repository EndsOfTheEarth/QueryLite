# Query Lite

Query Lite is a typesafe .net sql query library for Sql Server and PostgreSql. In essence it is a `typesafe` `string less` `sql query builder`. Sql queries like `select`, `update`, `delete` and `truncate` are supported.
 
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
            OrderStatus = row.Guid(orderStatusTable.Status)
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

* Typesafe queries in code.
  - Designed to work with nullable reference types and .net 8.0.
* Sql Select, insert, update, delete and truncate queries.
* Supports sql syntax e.g. Join, Left Join, Where, Order By, Group By, Union and Nested Queries.
* Does not do any client side filtering like linq to sql.
* Query syntax clear and easy to read.
* Debugging features that show sql queries being executed.
  - Events Like - QueryExecuting, QueryPerformed.
  - Query results contain the executed sql.
  - Breakpoint on query.
* Code generation tool for table definitions.
* Table definition validator.
* Supports both Sql Server and PostgreSql.
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

CustomerId customerId = CustomerId.ValueOf("ABC");

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

CustomerId customerId = CustomerId.ValueOf("ABC");

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

public sealed class CustomerTable : ATable {

    public static readonly CustomerTable Instance = new CustomerTable();
    
    public Column<CustomerId, string> CustomerId { get; }
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
        CustomerId = new Column<CustomerId, string>(this, columnName: "CustomerId", length: new(5));
        CompanyName = new Column<string>(this, columnName: "CompanyName", length: new(40));
        ContactName = new NullableColumn<string>(this, columnName: "ContactName", length: new(30));
        ContactTitle = new NullableColumn<string>(this, columnName: "ContactTitle", length: new(30));
        Address = new NullableColumn<string>(this, columnName: "Address", length: new(60));
        City = new NullableColumn<string>(this, columnName: "City", length: new(15));
        Region = new NullableColumn<string>(this, columnName: "Region", length: new(15));
        PostalCode = new NullableColumn<string>(this, columnName: "PostalCode", length: new(10));
        Country = new NullableColumn<string>(this, columnName: "Country", length: new(15));
        Phone = new NullableColumn<string>(this, columnName: "Phone", length: new(24));
        Fax = new NullableColumn<string>(this, columnName: "Fax", length: new(24));
    }
}

public sealed class OrderTable : ATable {

    public static readonly OrderTable Instance = new OrderTable();
    
    public Column<OrderId, int> OrderId { get; }
    public NullableColumn<CustomerId, string> CustomerId { get; }
    public NullableColumn<EmployeeId, int> EmployeeId { get; }
    public NullableColumn<DateTime> OrderDate { get; }
    public NullableColumn<DateTime> RequiredDate { get; }
    public NullableColumn<DateTime> ShippedDate { get; }
    public NullableColumn<ShipperId, int> ShipVia { get; }
    public NullableColumn<decimal> Freight { get; }
    public NullableColumn<string> ShipName { get; }
    public NullableColumn<string> ShipAddress { get; }
    public NullableColumn<string> ShipCity { get; }
    public NullableColumn<string> ShipRegion { get; }
    public NullableColumn<string> ShipPostalCode { get; }
    public NullableColumn<string> ShipCountry { get; }

    private OrderTable() : base(tableName:"Order", schemaName: "dbo", enclose: true) {
        OrderId = new Column<OrderId, int>(this, columnName: "OrderId", isAutoGenerated: true);
        CustomerId = new NullableColumn<CustomerId, string>(this, columnName: "CustomerId", length: new(5));
        EmployeeId = new NullableColumn<EmployeeId, int>(this, columnName: "EmployeeId");
        OrderDate = new NullableColumn<DateTime>(this, columnName: "OrderDate");
        RequiredDate = new NullableColumn<DateTime>(this, columnName: "RequiredDate");
        ShippedDate = new NullableColumn<DateTime>(this, columnName: "ShippedDate");
        ShipVia = new NullableColumn<ShipperId, int>(this, columnName: "ShipVia");
        Freight = new NullableColumn<decimal>(this, columnName: "Freight");
        ShipName = new NullableColumn<string>(this, columnName: "ShipName", length: new(40));
        ShipAddress = new NullableColumn<string>(this, columnName: "ShipAddress", length: new(60));
        ShipCity = new NullableColumn<string>(this, columnName: "ShipCity", length: new(15));
        ShipRegion = new NullableColumn<string>(this, columnName: "ShipRegion", length: new(15));
        ShipPostalCode = new NullableColumn<string>(this, columnName: "ShipPostalCode", length: new(10));
        ShipCountry = new NullableColumn<string>(this, columnName: "ShipCountry", length: new(15));
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
| Ado_Single_Row_Select                  | 154.0 ms | 1.14 ms | 1.12 ms |         - |   2.84 MB |
| Dapper_Single_Row_Select               | 149.2 ms | 0.92 ms | 0.81 ms |         - |   3.19 MB |
| QueryLite_Single_Row_Prepared_Select   | 157.4 ms | 2.67 ms | 2.49 ms |         - |   2.93 MB |
| QueryLite_Single_Row_Dynamic_Select    | 158.0 ms | 2.65 ms | 2.35 ms |         - |   3.89 MB |
| QueryLite_Single_Row_Repository_Select | 186.6 ms | 1.66 ms | 1.47 ms |         - |   5.19 MB |
| EF_Core_Single_Row_Select              | 194.8 ms | 1.41 ms | 1.18 ms | 1000.0000 |  17.65 MB |

### Select Ten Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```

| Method                              | Mean     | Error   | StdDev  | Allocated |
|------------------------------------ |---------:|--------:|--------:|----------:|
| Ado_Ten_Row_Select                  | 181.5 ms | 1.51 ms | 1.41 ms |   4.43 MB |
| Dapper_Ten_Row_Select               | 183.3 ms | 3.10 ms | 3.19 ms |   6.76 MB |
| QueryLite_Ten_Row_Prepared_Select   | 184.9 ms | 1.26 ms | 1.11 ms |   4.43 MB |
| QueryLite_Ten_Row_Dynamic_Select    | 186.8 ms | 2.41 ms | 2.14 ms |   5.16 MB |
| QueryLite_Ten_Row_Repository_Select | 240.7 ms | 4.50 ms | 4.82 ms |   9.84 MB |
| EF_Core_Ten_Row_Select              | 209.1 ms | 1.57 ms | 1.47 ms |   9.84 MB |

### Select One Hundred Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```

| Method                                      | Mean     | Error   | StdDev  | Gen0      | Allocated |
|-------------------------------------------- |---------:|--------:|--------:|----------:|----------:|
| Ado_One_Hundred_Row_Select                  | 232.2 ms | 2.59 ms | 2.42 ms | 1000.0000 |  29.95 MB |
| Dapper_One_Hundred_Row_Select               | 234.2 ms | 2.34 ms | 2.19 ms | 2000.0000 |  46.02 MB |
| QueryLite_One_Hundred_Row_Prepared_Select   | 239.7 ms | 1.64 ms | 1.37 ms | 1000.0000 |  29.95 MB |
| QueryLite_One_Hundred_Row_Dynamic_Select    | 240.7 ms | 2.11 ms | 1.97 ms | 1000.0000 |  30.69 MB |
| QueryLite_One_Hundred_Row_Repository_Select | 319.0 ms | 2.01 ms | 1.88 ms | 4000.0000 |  72.95 MB |
| EF_Core_One_Hundred_Row_Select              | 267.2 ms | 3.95 ms | 3.30 ms | 3000.0000 |  56.02 MB |

### Select One Thousand Rows To List (2000 Sequential Iterations)

```SQL
SELECT id,row_guid,message,date FROM Test01
```
| Method                                       | Mean     | Error   | StdDev  | Gen0       | Gen1       | Allocated |
|--------------------------------------------- |---------:|--------:|--------:|-----------:|-----------:|----------:|
| Ado_One_Thousand_Row_Select                  | 530.2 ms | 1.84 ms | 1.53 ms | 17000.0000 |  5000.0000 | 277.16 MB |
| Dapper_One_Thousand_Row_Select               | 530.9 ms | 3.18 ms | 2.82 ms | 26000.0000 |  8000.0000 | 430.56 MB |
| QueryLite_One_Thousand_Row_Prepared_Select   | 533.7 ms | 1.54 ms | 1.37 ms | 17000.0000 |  8000.0000 | 277.16 MB |
| QueryLite_One_Thousand_Row_Dynamic_Select    | 536.9 ms | 3.18 ms | 2.82 ms | 17000.0000 |  9000.0000 | 277.89 MB |
| QueryLite_One_Thousand_Row_Repository_Select | 955.4 ms | 4.04 ms | 3.58 ms | 43000.0000 | 25000.0000 | 687.96 MB |
| EF_Core_One_Thousand_Row_Select              | 579.7 ms | 1.57 ms | 1.31 ms | 31000.0000 |  1000.0000 | 509.79 MB |

### Insert Single Row (2000 Sequential Iterations)

```SQL
INSERT INTO Test01 (row_guid,message,date) VALUES(@0, @1, @2)
```
| Method                                    | Mean     | Error    | StdDev   | Median   | Gen0      | Allocated |
|------------------------------------------ |---------:|---------:|---------:|---------:|----------:|----------:|
| Ado_Single_Insert                         | 397.9 ms |  3.13 ms |  2.93 ms | 398.6 ms |         - |   3.37 MB |
| Dapper_Single_Insert                      | 419.8 ms |  7.19 ms | 10.08 ms | 416.5 ms |         - |   3.49 MB |
| QueryLite_Single_Compiled_Insert          | 428.1 ms |  8.41 ms | 14.28 ms | 427.4 ms |         - |    3.6 MB |
| QueryLite_Single_Dynamic_Insert           | 422.3 ms |  8.16 ms | 21.07 ms | 422.0 ms |         - |    4.5 MB |
| QueryLite_Single_Repository_Insert        | 438.0 ms |  8.69 ms | 23.65 ms | 431.2 ms |         - |   5.36 MB |
| QueryLite_Single_Repository_Static_Insert | 418.7 ms |  4.38 ms |  3.88 ms | 419.0 ms |         - |   3.89 MB |
| EF_Core_Single_Insert                     | 765.8 ms | 14.92 ms | 15.97 ms | 764.7 ms | 7000.0000 | 114.65 MB |

(Note: This benchmark creates a new EF Core context for each insert to simulate unrelated requests).

### Update Single Row (2000 Sequential Iterations)

```SQL
UPDATE Test01 SET message=@1,date=@2 WHERE row_guid=@0
```

| Method                                        | Mean     | Error   | StdDev  | Gen0      | Allocated |
|---------------------------------------------- |---------:|--------:|--------:|----------:|----------:|
| Ado_Single_Row_Update                         | 396.5 ms | 5.51 ms | 5.15 ms |         - |   3.36 MB |
| Dapper_Single_Row_Update                      | 395.8 ms | 5.17 ms | 4.84 ms |         - |   3.57 MB |
| QueryLite_Single_Row_Prepared_Update          | 401.4 ms | 4.43 ms | 4.14 ms |         - |    3.6 MB |
| QueryLite_Single_Row_Dynamic_Update           | 400.9 ms | 4.54 ms | 4.24 ms |         - |   4.74 MB |
| QueryLite_Single_Row_Repository_Update        | 622.0 ms | 5.09 ms | 4.51 ms |         - |  10.03 MB |
| QueryLite_Single_Row_Repository_Static_Update | 411.2 ms | 5.53 ms | 5.17 ms |         - |   4.27 MB |
| EF_Core_Single_Row_Update                     | 673.5 ms | 5.02 ms | 4.69 ms | 7000.0000 |  114.9 MB |

(Note: This benchmark creates a new EF Core context for each update to simulate unrelated requests).

### Delete Single Row (2000 Sequential Iterations)

```SQL
DELETE FROM Test01 WHERE row_guid=@0
```

| Method                                 | Mean     | Error   | StdDev   | Gen0      | Allocated |
|--------------------------------------- |---------:|--------:|---------:|----------:|----------:|
| Ado_Single_Row_Delete                  | 300.0 ms | 5.96 ms | 10.12 ms |         - |   2.43 MB |
| Dapper_Single_Row_Delete               | 292.4 ms | 1.96 ms |  1.83 ms |         - |   2.64 MB |
| QueryLite_Single_Row_Prepared_Delete   | 297.6 ms | 1.99 ms |  1.86 ms |         - |   2.64 MB |
| QueryLite_Single_Row_Dynamic_Delete    | 298.3 ms | 2.44 ms |  2.28 ms |         - |   3.28 MB |
| QueryLite_Single_Row_Repository_Delete | 523.7 ms | 3.21 ms |  3.00 ms |         - |   6.56 MB |
| EF_Core_Single_Row_Delete              | 841.0 ms | 4.21 ms |  3.94 ms | 7000.0000 | 115.46 MB |

(Note: This benchmark creates a new EF Core context for each delete to simulate unrelated requests).