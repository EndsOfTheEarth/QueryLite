
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class ConstraintColumnUsageTable : ATable {

        public static readonly ConstraintColumnUsageTable Instance = new ConstraintColumnUsageTable();

        public Column<string> TABLE_CATALOG { get; }
        public Column<StringKey<ISchemaName>> TABLE_SCHEMA { get; }
        public Column<StringKey<ITableName>> TABLE_NAME { get; }
        public Column<StringKey<IColumnName>> COLUMN_NAME { get; }
        public Column<string> CONSTRAINT_CATALOG { get; }
        public Column<StringKey<ISchemaName>> CONSTRAINT_SCHEMA { get; }
        public Column<string> CONSTRAINT_NAME { get; }


        public ConstraintColumnUsageTable() : base(tableName: "constraint_column_usage", schemaName: "information_schema") {

            TABLE_CATALOG = new Column<string>(table: this, columnName: "table_catalog");
            TABLE_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            TABLE_NAME = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            COLUMN_NAME = new Column<StringKey<IColumnName>>(table: this, columnName: "column_name");
            CONSTRAINT_CATALOG = new Column<string>(table: this, columnName: "constraint_catalog");
            CONSTRAINT_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "constraint_schema");
            CONSTRAINT_NAME = new Column<string>(table: this, columnName: "constraint_name");
        }
    }
}