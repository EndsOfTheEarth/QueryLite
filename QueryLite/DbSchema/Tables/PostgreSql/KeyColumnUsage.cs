
namespace QueryLite.DbSchema.Tables.PostgreSql {

    public sealed class KeyColumnUsageTable : ATable {

        public static readonly KeyColumnUsageTable Instance = new KeyColumnUsageTable();

        public Column<string> Constraint_catalog { get; }
        public Column<string> Constraint_schema { get; }
        public Column<string> Constraint_name { get; }
        public Column<string> Table_catalog { get; }
        public Column<StringKey<ISchemaName>> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<StringKey<IColumnName>> Column_name { get; }
        public NullableColumn<int> Ordinal_position { get; }
        public NullableColumn<int> Position_in_unique_constraint { get; }

        public KeyColumnUsageTable() : base(tableName: "key_column_usage", schemaName: "information_schema") {

            Constraint_catalog = new Column<string>(table: this, columnName: "constraint_catalog");
            Constraint_schema = new Column<string>(table: this, columnName: "constraint_schema");
            Constraint_name = new Column<string>(table: this, columnName: "constraint_name");
            Table_catalog = new Column<string>(table: this, columnName: "table_catalog");
            Table_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            Table_name = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            Column_name = new Column<StringKey<IColumnName>>(table: this, columnName: "column_name");
            Ordinal_position = new NullableColumn<int>(this, columnName: "ordinal_position");
            Position_in_unique_constraint = new NullableColumn<int>(this, columnName: "position_in_unique_constraint");
        }
    }
}