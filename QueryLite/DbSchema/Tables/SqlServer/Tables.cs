
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class TablesTable : ATable {

        public static readonly TablesTable Instance = new TablesTable();

        public Column<string> TABLE_CATALOG { get; }
        public Column<StringKey<ISchemaName>> TABLE_SCHEMA { get; }
        public Column<StringKey<ITableName>> TABLE_NAME { get; }
        public Column<string> TABLE_TYPE { get; }

        public TablesTable() : base(tableName: "tables", schemaName: "information_schema") {

            TABLE_CATALOG = new Column<string>(table: this, columnName: "table_catalog");
            TABLE_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            TABLE_NAME = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            TABLE_TYPE = new Column<string>(table: this, columnName: "table_type");
        }
    }
}