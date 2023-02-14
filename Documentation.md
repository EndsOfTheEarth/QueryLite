# Query Lite Documentation

## Index

- [Introduction](#introduction)
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
- [Nested Query](#nested-query)
- [Insert Query](#insert-query)
- [Update Query](#update-query)
- [Update Join Query](#update-join-query)
- [Delete Query](#delete-query)
- [Delete Join Query (MSSQL Only)](#delete-join-query)
- [Truncate Query](#truncate-query)
- [String Like Condition](#string-like-condition)
- [Functions](#functions)
- [Custom Functions](#custom-functions)
- [Supported Data Types](#supported-data-types)
- [Key Columns](#key-columns)
- [Transaction Isolation Levels](#transaction-isolation-levels)
- [Debugging](#debugging)
- [Breakpoint Debugging](#breakpoint-debugging)
- [Schema Validation](#schema-validation)
- [Database Constraints](#database-constraints)
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
  * Syntax - select, top, from join, left join, table hints, where, group by, order by, returning

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
  - Table Hints eg. WITH(UPDLOCK) are specific to Sql Server 
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
    .InsertInto(shipperTable)
    .Set(shipperTable.CompanyName, "My company name")
    .Set(shipperTable.Phone, "123456789")
    .Execute(transaction);  //This will default to TimeoutLevel.ShortInsert

var result = Query
    .InsertInto(shipperTable)
    .Set(shipperTable.CompanyName, "My company name")
    .Set(shipperTable.Phone, "123456789")
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
                .Having(new CountAll() > 1)
        )
    )
    .Execute(DB.Northwind);
```

## Insert Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .InsertInto(shipperTable)
        .Set(shipperTable.CompanyName, "My company name")
        .Set(shipperTable.Phone, "123456789")
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
        .Set(shipperTable.Phone, "")
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
        .Set(shipperTable.Phone, "")
        .Where(shipperTable.Phone.IsNull)
        .Execute(
            updated => updated.Get(shipperTable.ShipperID),
            transaction
        );

    transaction.Commit();
    
    IntKey<IShipper> shipperId = result.Rows.First();
}
```

## Update Join Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    OrdersTable orderTable = OrdersTable.Instance;
    CustomersTable customersTable = CustomersTable.Instance;

    NonQueryResult result = Query
        .Update(orderTable)
        .Set(orderTable.ShipVia, null)
        .Join(customersTable).On(orderTable.CustomerID == customersTable.CustomerID)
        .Where(customersTable.Region.IsNull)
        .Execute(transaction);

    transaction.Commit();
}
```

## Delete Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    var result = Query
        .DeleteFrom(shipperTable)
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
        .DeleteFrom(shipperTable)
        .Where(shipperTable.ShipperID == new IntKey<IShipper>(100))
        .Execute(
            deleted => deleted.Get(shipperTable.ShipperID),
            transaction
        );

    transaction.Commit();
    
    IntKey<IShipper> shipperId = result.Rows.First();
}
```

## Delete Join Query

**Please Note: Join syntax is only supported by Sql Server. An exception will be thrown if this is executed on a PostgreSql database.**

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    OrdersTable orderTable = OrdersTable.Instance;
    CustomersTable customersTable = CustomersTable.Instance;

    NonQueryResult result = Query
        .DeleteFrom(orderTable)
        .Join(customersTable).On(orderTable.CustomerID == customersTable.CustomerID)
        .Where(customersTable.Region.IsNull)
        .Execute(transaction);

    transaction.Commit();
}
```

## Truncate Query

```C#
using(Transaction transaction = new Transaction(DB.Northwind)) {

    ShipperTable shipperTable = ShipperTable.Instance;

    NonQueryResult result = Query
        .TruncateTable(shipperTable)
        .Execute(transaction);

    transaction.Commit();
}
```

## String Like Condition

```C#
OrdersTable orderTable = OrdersTable.Instance;

var result = Query
    .Select(row => new { Id = row.Get(orderTable.OrderID) })
    .From(orderTable)
    .Where(orderTable.ShipPostalCode.Like(new StringLike("%abc%")))
    .Execute(DB.Northwind);
```

## Functions

Currently only a small set of sql functions are supported. For Sql Server these are `CountAll`, `GetDate` and `GetDateTimeOffset`. Please note that creating custom sql functions is documented below this section.

```C#
ShipperTable shipperTable = ShipperTable.Instance;

CountAll count = new CountAll();

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

## Transaction Isolation Levels

Isolation levels can be set on transactions. Please note that levels like 'Snapshort' are only supported by Sql Server. If the code returns from the using statement before calling `transaction.Commit()` the transaction will be rolled back.

```C#
using(Transaction transaction = new Transaction(DB.Northwind, IsolationLevel.ReadCommitted)) {
    ...
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

void Settings_QueryExecuting(IDatabase database, string sql, QueryType queryType, DateTimeOffset? start, System.Data.IsolationLevel isolationLevel, ulong? transactionId) {

}
void Settings_QueryPerformed(IDatabase database, string sql, int rows, int rowsEffected, QueryType queryType, DateTimeOffset? start, DateTimeOffset? end, TimeSpan? elapsedTime, Exception? exception, System.Data.IsolationLevel isolationLevel, ulong? transactionId) {

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

ValidationResult result = SchemaValidator.ValidateTablesInCurrentDomain(database, settings);

//There are other methods like validating table in an Assembly e.g.
//List<TableValidation> validation = SchemaValidator.ValidateTablesInAssembly(database, Assembly.GetCallingAssembly(), settings);

StringBuilder output = new StringBuilder();

foreach(TableValidation tableValidation in result.TableValidation) {

    if(tableValidation.HasErrors) {

        string tableName = tableValidation.Table?.TableName ?? string.Empty;

        output.Append("Table Name: ").Append(tableName).Append(Environment.NewLine);

        output.AppendLine("Errors:").Append(Environment.NewLine);

        foreach(string message in tableValidation.ValidationMessages) {
            output.AppendLine(message);
        }
        output.Append(Environment.NewLine);
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