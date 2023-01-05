
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class TableConstraintsTable : ATable {

        public static readonly TableConstraintsTable Instance = new TableConstraintsTable();

        public Column<string> Constraint_catalog { get; }
        public Column<string> Constraint_schema { get; }
        public Column<string> Constraint_name { get; }
        public Column<string> Table_catalog { get; }
        public Column<StringKey<ISchemaName>> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<string> Constraint_type { get; }
        public Column<string> Is_deferrable { get; }
        public Column<string> Initially_deferred { get; }

        public TableConstraintsTable() : base(tableName: "table_constraints", schemaName: "information_schema") {

            Constraint_catalog = new Column<string>(table: this, columnName: "constraint_catalog");
            Constraint_schema = new Column<string>(table: this, columnName: "constraint_schema");
            Constraint_name = new Column<string>(table: this, columnName: "constraint_name");
            Table_catalog = new Column<string>(table: this, columnName: "table_catalog");
            Table_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            Table_name = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            Constraint_type = new Column<string>(table: this, columnName: "constraint_type");
            Is_deferrable = new Column<string>(table: this, columnName: "is_deferrable");
            Initially_deferred = new Column<string>(table: this, columnName: "initially_deferred");
        }
    }
}