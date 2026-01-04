
namespace QueryLite.DbSchema.Tables.Sqlite {

    public sealed class SchemaTable : ATable {

        public static readonly SchemaTable Instance = new();

        public Column<string> Type { get; }
        public Column<string> Name { get; }
        public Column<string> TblName { get; }
        public Column<int> RootPage { get; }
        public Column<string> Sql { get; }

        public SchemaTable() : base(tableName: "sqlite_schema", schemaName: "") {

            Type = new Column<string>(table: this, columnName: "type");
            Name = new Column<string>(table: this, columnName: "name");
            TblName = new Column<string>(table: this, columnName: "tbl_name");
            RootPage = new Column<int>(table: this, columnName: "rootpage");
            Sql = new Column<string>(table: this, columnName: "sql");
        }
    }
}