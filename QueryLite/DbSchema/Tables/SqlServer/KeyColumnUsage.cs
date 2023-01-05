
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class KeyColumnUsageTable : ATable {

        public static readonly KeyColumnUsageTable Instance = new KeyColumnUsageTable();

        public Column<string> CONSTRAINT_CATALOG { get; }
        public Column<StringKey<ISchemaName>> CONSTRAINT_SCHEMA { get; }
        public Column<string> CONSTRAINT_NAME { get; }
        public Column<string> TABLE_CATALOG { get; }
        public Column<StringKey<ISchemaName>> TABLE_SCHEMA { get; }
        public Column<StringKey<ITableName>> TABLE_NAME { get; }
        public Column<StringKey<IColumnName>> COLUMN_NAME { get; }
        public NullableColumn<int> ORDINAL_POSITION { get; }

        public KeyColumnUsageTable() : base(tableName: "key_column_usage", schemaName: "information_schema") {

            CONSTRAINT_CATALOG = new Column<string>(table: this, columnName: "constraint_catalog");
            CONSTRAINT_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "constraint_schema");
            CONSTRAINT_NAME = new Column<string>(table: this, columnName: "constraint_name");
            TABLE_CATALOG = new Column<string>(table: this, columnName: "table_catalog");
            TABLE_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            TABLE_NAME = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            COLUMN_NAME = new Column<StringKey<IColumnName>>(table: this, columnName: "column_name");
            ORDINAL_POSITION = new NullableColumn<int>(this, columnName: "ordinal_position");
        }
    }
}