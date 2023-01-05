
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class ReferentialConstraintsTable : ATable {

        public static readonly ReferentialConstraintsTable Instance = new ReferentialConstraintsTable();

        public NullableColumn<string> CONSTRAINT_CATALOG { get; }
        public NullableColumn<StringKey<ISchemaName>> CONSTRAINT_SCHEMA { get; }
        public Column<string> CONSTRAINT_NAME { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_CATALOG { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_SCHEMA { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_NAME { get; }
        public NullableColumn<string> MATCH_OPTION { get; }
        public NullableColumn<string> UPDATE_RULE { get; }
        public NullableColumn<string> DELETE_RULE { get; }

        public ReferentialConstraintsTable() : base(tableName: "REFERENTIAL_CONSTRAINTS", schemaName: "information_schema") {

            CONSTRAINT_CATALOG = new NullableColumn<string>(this, columnName: "CONSTRAINT_CATALOG", length: 128);
            CONSTRAINT_SCHEMA = new NullableColumn<StringKey<ISchemaName>>(this, columnName: "CONSTRAINT_SCHEMA", length: 128);
            CONSTRAINT_NAME = new Column<string>(this, columnName: "CONSTRAINT_NAME", length: 128);
            UNIQUE_CONSTRAINT_CATALOG = new NullableColumn<string>(this, columnName: "UNIQUE_CONSTRAINT_CATALOG", length: 128);
            UNIQUE_CONSTRAINT_SCHEMA = new NullableColumn<string>(this, columnName: "UNIQUE_CONSTRAINT_SCHEMA", length: 128);
            UNIQUE_CONSTRAINT_NAME = new NullableColumn<string>(this, columnName: "UNIQUE_CONSTRAINT_NAME", length: 128);
            MATCH_OPTION = new NullableColumn<string>(this, columnName: "MATCH_OPTION", length: 7);
            UPDATE_RULE = new NullableColumn<string>(this, columnName: "UPDATE_RULE", length: 11);
            DELETE_RULE = new NullableColumn<string>(this, columnName: "DELETE_RULE", length: 11);

        }
    }
}