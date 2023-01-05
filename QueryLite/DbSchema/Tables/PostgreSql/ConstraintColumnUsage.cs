
namespace QueryLite.DbSchema.Tables.PostgreSql {

    public sealed class ConstraintColumnUsageTable : ATable {

        public static readonly ConstraintColumnUsageTable Instance = new ConstraintColumnUsageTable();

        public Column<string> Table_catalog { get; }
        public Column<string> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<string> Column_name { get; }
        public Column<string> Constraint_catalog { get; }
        public Column<StringKey<ISchemaName>> Constraint_schema { get; }
        public Column<string> Constraint_name { get; }

        public ConstraintColumnUsageTable() : base(tableName: "constraint_column_usage", schemaName: "information_schema") {

            Table_catalog = new Column<string>(table: this, columnName: "table_catalog");
            Table_schema = new Column<string>(table: this, columnName: "table_schema");
            Table_name = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            Column_name = new Column<string>(table: this, columnName: "column_name");
            Constraint_catalog = new Column<string>(table: this, columnName: "constraint_catalog");
            Constraint_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "constraint_schema");
            Constraint_name = new Column<string>(table: this, columnName: "constraint_name");
        }
    }
}