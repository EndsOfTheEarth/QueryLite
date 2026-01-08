# Getting Started

## Steps

1) Import Nuget Package
2) Create Table Definition(s) (Using Code Generator)
3) Create Database Instance in code
4) Start Writing Queries

## 1) Import Nuget Package

Import the Nuget package `QueryLite.net` into your project. Either by running the Nuget command `Install-Package QueryLite.net` in the package manager console or by finding the package in the package manager.


## 2) Use Code Generator Tool To Create Table Definitions

Download the CodeGenerator.exe tool from the latest QueryLite release. See [https://github.com/EndsOfTheEarth/QueryLite/releases](https://github.com/EndsOfTheEarth/QueryLite/releases)

The zip file `CodeGenerator.zip` contains the code generator tool `CodeGenerator.exe` and this can be found under the `Assets` section.


Run the code generator, enter in your database connection string and click the `Load' button. This will display all the tables in your database.

Table definitions are generated when a table is selected. Copy the table definition(s) into your project.

![AdventureWorksCodeGenerator](https://github.com/user-attachments/assets/f257be9c-bb6c-455e-baaf-8f11fc15ce40)

## 3) Define A Database Instance

Create a single shared instance of the database. This instance can be cached in the application for queries to share.
```C#
IDatabase database = new SqlServerDatabase(
    name: "MyDatabase",
    connectionString: "Server=localhost;Database=MyDatabase;Trusted_Connection=True;"
);
```

## 4) Start Writing Queries

Full Code Example:
```C#
using QueryLite.Databases.SqlServer;
using QueryLite;
using Northwind.Tables;

/*
 *  Define the database. Note: Only a single instance of class is required for each unique connection string.
 */
IDatabase database = new SqlServerDatabase(
    name: "Northwind",
    connectionString: "Server=localhost;Database=Northwind;Trusted_Connection=True;"
);

CustomerTable customerTable = CustomerTable.Instance;
OrderTable orderTable = OrderTable.Instance;

var result = Query
    .Select(
        row => new {
            CustomerId = row.Get(customerTable.CustomerId),
            Phone = row.Get(customerTable.Phone),
            OrderId = row.Get(orderTable.OrderId),
            ShippedDate = row.Get(orderTable.ShippedDate)
        }
    )
    .From(customerTable)
    .Join(orderTable).On(customerTable.CustomerId == orderTable.CustomerId)
    .Where(customerTable.CustomerId == "NORTS" & customerTable.City.IsNotNull)
    .Execute(database);

foreach(var row in result.Rows) {

    string CustomerId = row.CustomerId;
    string? Phone = row.Phone;
    int OrderId = row.OrderId;
    DateTime? shippedDate = row.ShippedDate;
}

/*
 *  Table Definitions Below - These can be generated using the code generator tool
 */
namespace Northwind.Tables {

    using System;
    using QueryLite;

    public sealed class CustomerTable : ATable {

        public static readonly CustomerTable Instance = new();

        public Column<string> CustomerId { get; }
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

        private CustomerTable() : base(name: "Customer", schemaName: "dbo") {
            CustomerId = new Column<string>(this, name: "CustomerId");
            CompanyName = new Column<string>(this, name: "CompanyName");
            ContactName = new NColumn<string>(this, name: "ContactName");
            ContactTitle = new NColumn<string>(this, name: "ContactTitle");
            Address = new NColumn<string>(this, name: "Address");
            City = new NColumn<string>(this, name: "City");
            Region = new NColumn<string>(this, name: "Region");
            PostalCode = new NColumn<string>(this, name: "PostalCode");
            Country = new NColumn<string>(this, name: "Country");
            Phone = new NColumn<string>(this, name: "Phone");
            Fax = new NColumn<string>(this, name: "Fax");
        }
    }

    public sealed class OrderTable : ATable {

        public static readonly OrderTable Instance = new();

        public Column<int> OrderId { get; }
        public NColumn<string> CustomerId { get; }
        public NColumn<int> EmployeeId { get; }
        public NColumn<DateTime> OrderDate { get; }
        public NColumn<DateTime> RequiredDate { get; }
        public NColumn<DateTime> ShippedDate { get; }
        public NColumn<int> ShipVia { get; }
        public NColumn<decimal> Freight { get; }
        public NColumn<string> ShipName { get; }
        public NColumn<string> ShipAddress { get; }
        public NColumn<string> ShipCity { get; }
        public NColumn<string> ShipRegion { get; }
        public NColumn<string> ShipPostalCode { get; }
        public NColumn<string> ShipCountry { get; }

        private OrderTable() : base(name: "Order", schemaName: "dbo", enclose: true) {

            OrderId = new Column<int>(this, name: "OrderId", isAutoGenerated: true);
            CustomerId = new NColumn<string>(this, name: "CustomerId");
            EmployeeId = new NColumn<int>(this, name: "EmployeeId");
            OrderDate = new NColumn<DateTime>(this, name: "OrderDate");
            RequiredDate = new NColumn<DateTime>(this, name: "RequiredDate");
            ShippedDate = new NColumn<DateTime>(this, name: "ShippedDate");
            ShipVia = new NColumn<int>(this, name: "ShipVia");
            Freight = new NColumn<decimal>(this, name: "Freight");
            ShipName = new NColumn<string>(this, name: "ShipName");
            ShipAddress = new NColumn<string>(this, name: "ShipAddress");
            ShipCity = new NColumn<string>(this, name: "ShipCity");
            ShipRegion = new NColumn<string>(this, name: "ShipRegion");
            ShipPostalCode = new NColumn<string>(this, name: "ShipPostalCode");
            ShipCountry = new NColumn<string>(this, name: "ShipCountry");
        }
    }
}
```