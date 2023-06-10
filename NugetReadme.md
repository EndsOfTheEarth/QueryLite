# Query Lite

Query Lite is a typesafe .net sql query library for Sql Server and PostgreSql.

It is designed to achieve these main goals:

1. Typesafe database queries and schema
    - Queries, joins, 'where' conditions, column types and column nullability are enforced by the compiler
2. Runtime validation between C# code schema and database schema
    - Check for missing tables, missing columns, column type and nullablility differences
3. Debuggable
    - Show detailed information about sql queries during and after execution
4. Performance
    - Prepared queries are near equivalent performance and memory allocation to direct ado.net code
    - Dynamic queries are near equivalent performance and memory allocation to Dapper (Often with significantly lower memory allocation)
## Links

- [Github Repository](https://github.com/EndsOfTheEarth/QueryLite)
- [Documentation](https://github.com/EndsOfTheEarth/QueryLite/blob/main/Documentation.md)
