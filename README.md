# Query Lite

Query Lite is a typesafe .net sql query library for Sql Server and PostgreSql

In essence it is a `typesafe` `string less` `sql query builder`.

It is designed to achieve these main goals:

1. Typesafe database queries and schema
    - Queries, joins, 'where' conditions, column types and column nullability are enforced by the compiler
2. Runtime validation between code schema and database schema
    - Check for missing tables, missing columns, column type and nullablility differences
3. Debuggable
    - Show detailed information about sql queries during and after execution
4. Performance
    - `Prepared` queries are near equivalent performance and memory allocation to direct ado.net code
    - `Dynamic` queries are near equivalent performance and memory allocation to Dapper (Often with significantly lower memory allocation)

## Documentation
[Documentation is found here](Documentation.md)

[Prepared Query Documentation is found here](PreparedQueries.md)

## Nuget Package

[QueryLite.net Nuget Package](https://www.nuget.org/packages/QueryLite.net/)

## Example Select Query (Dynamic)
``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

OrdersTable ordersTable = OrdersTable.Instance;
CustomersTable customersTable = CustomersTable.Instance;

var result = Query
    .Select(
        row => new {
            OrderId = row.Get(ordersTable.OrderID),
            CustomerId = row.Get(customersTable.CustomerID),
            CompanyName = row.Get(customersTable.CompanyName)
        }
    )
    .From(ordersTable)
    .Join(customersTable).On(ordersTable.CustomerID == customersTable.CustomerID)
    .Where(ordersTable.OrderDate < DateTime.Now & customersTable.ContactName == "Jane")
    .OrderBy(ordersTable.OrderID.ASC)
    .Execute(DB.Northwind);

string sql = result.Sql;    //Generated sql is available on the result

foreach(var row in result.Rows) {

    IntKey<IOrder> orderId = row.OrderId;
    StringKey<ICustomer> customerId = row.CustomerId;
    string companyName = row.CompanyName;
}
```

## Core features

* Typesafe queries in code
  - Designed to work with nullable reference types and .net 7.0
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

Query lite implements both dynamic and prepared queries. Prepared queries allocate less memory on the heap during query execution. This gives them very similar performance (And garbage collector load) to lower level ado.net code. The downside is that prepared queries have multiple generic constraints which make them syntactically more complicated.

On the other hand, dynamic queries are syntactically simpler but have a higher memory allocation during query execution. This is due to dynamically building the sql query on every execution. The additional memory allocation make dynamic query performance / memory allocation more similar to libaraies like Dapper.

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

CustomersTable customersTable = CustomersTable.Instance;

StringKey<ICustomer> customerId = new StringKey<ICustomer>("ABC");

using(Transaction transaction = new Transaction(DB.Northwind)) {

    var result = Query.Insert(customersTable)
        .Values(values => values
            .Set(customersTable.CustomerID, customerId)
            .Set(customersTable.CompanyName, "company name")
            .Set(customersTable.ContactName, "contact name")
            .Set(customersTable.ContactTitle, "title")
            .Set(customersTable.Address, "address")
            .Set(customersTable.City, "city")
            .Set(customersTable.Region, "region")
            .Set(customersTable.PostalCode, "12345")
            .Set(customersTable.Country, "somewhere")
            .Set(customersTable.Phone, "312-12312-123")
            .Set(customersTable.Fax, null)
        )
        .Execute(
            inserted => new {
                PostalCode = inserted.Get(customersTable.PostalCode),
                Address = inserted.Get(customersTable.Address)
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

CustomersTable customersTable = CustomersTable.Instance;

StringKey<ICustomer> customerId = new StringKey<ICustomer>("ABC");

using(Transaction transaction = new Transaction(DB.Northwind)) {

    Query.Update(customersTable)
        .Values(values => values
            .Set(customersTable.ContactTitle, "Mrs")
            .Set(customersTable.ContactName, "Mary")
        )
        .Where(customersTable.CustomerID == customerId)
        .Execute(transaction);

    transaction.Commit();
}
```

## Example Delete Query (Dynamic)

``` C#
using QueryLite;
using Northwind.Tables;
using Northwind;

CustomersTable customersTable = CustomersTable.Instance;

StringKey<ICustomer> customerId = new StringKey<ICustomer>("ABC");

using(Transaction transaction = new Transaction(DB.Northwind)) {

    Query.Delete(customersTable)
        .Where(customersTable.CustomerID == customerId)
        .Execute(transaction);

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

The code generator tool can be used to generate database schema definitions.

![image](https://user-images.githubusercontent.com/6175921/193362983-c5473a7e-7529-4850-8bdd-25a6e9f071bc.png)

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
