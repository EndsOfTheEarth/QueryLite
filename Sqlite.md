# Sqlite Support

Sqlite databases can be queried using QueryLite but there are a number of limitations.

## Limitations

In `STRICT` mode `Sqlite` only supports 4 data types. This means there are data types supported by QueryLite
that do not correlate with a Sqlite data type. These types only map directly to a small set of .NET types.

### Non-Converted Type Mappings

These are the type mappings where the .NET type is not converted to a different storage type by QueryLite.

| Sqlite Type | Query Lite Type        |
|-------------|------------------------|
| INTEGER     | byte, short, int, long |
| REAL        | float, double          |
| TEXT        | string                 |
| BLOB        | byte[]                 |


For all the other supported .NET types, QueryLite converts those to one of the 4 types.

These are the type conversions:

### INTEGER Type

These are all the types that are sorted as an INTEGER.

| Query Lite Type  | Sqlite Type |
|------------------|-------------|
| short, int, long | INTEGER     |
| byte             | INTEGER     |
| bool             | INTEGER     |
| BIT              | INTEGER     |

### TEXT Type

These are all the types that are sorted as a TEXT.

| Query Lite Type  | Sqlite Type |
|------------------|-------------|
| string           | TEXT        |
| decimal          | TEXT        |
| Json, Jsonb      | TEXT        |
| DateTimeOffset   | TEXT        |
| DateTime         | TEXT        |
| DateOnly         | TEXT        |
| TimeOnly         | TEXT        |

### REAL Type

These are all the types that are sorted as a REAL.

| Query Lite Type  | Sqlite Type |
|------------------|-------------|
| float            | REAL        |
| double            | REAL       |

### BLOB Type

These are all the types that are sorted as a BLOB.

| Query Lite Type  | Sqlite Type |
|------------------|-------------|
| byte[]           | BLOB        |
| Guid             | REAL        |

## Validation

Sqlite has a limited API to query schema information. This means code gneration and 
table validation is missing features.

Information that is missing from Sqlite:
 - Primary key name
 - Column is auto generated
 - Column length
 - Unique keys
 - Foreign keys
 - Check constraints

When validating the schema, these should not be included in the validation settings.

Example:
```C#
SchemaValidationSettings settings = new() {
    ValidatePrimaryKeys = PrimaryKeyValidation.Columns, //Primary keys do not have names
    ValidateUniqueConstraints = false,                  
    ValidateForeignKeys = false,
    ValidateCheckConstraintNames = false,
    ValidateMissingCodeTables = true,
    ValidateColumnLengths = false,                      //Column lengths cannot be obtained
    ValidateColumnAutoGeneration = false                //Column auto generated cannot be obtained
};
ValidationResult result = SchemaValidator.ValidateTables(TestDatabase.Database, tables, settings);
```