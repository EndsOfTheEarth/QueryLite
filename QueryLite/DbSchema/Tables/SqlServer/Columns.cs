
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class ColumnsTable : ATable {

        public static readonly ColumnsTable Instance = new ColumnsTable();

        public Column<StringKey<ISchemaName>> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<StringKey<IColumnName>> Column_name { get; }
        public NullableColumn<int> Ordinal_position { get; }
        public Column<string> Column_default { get; }
        public Column<string> Is_nullable { get; }
        public Column<string> Data_type { get; }
        public NullableColumn<int> Character_maximum_length { get; }
        public NullableColumn<int> Character_octet_length { get; }
        public NullableColumn<byte> Numeric_precision { get; }
        public NullableColumn<short> Numeric_precision_radix { get; }
        public NullableColumn<int> Numeric_scale { get; }
        public NullableColumn<short> Datetime_precision { get; }
        public Column<string> Character_set_catalog { get; }
        public Column<string> Character_set_schema { get; }
        public Column<string> Character_set_name { get; }
        public Column<string> Collation_catalog { get; }
        public Column<string> Collation_schema { get; }
        public Column<string> Collation_name { get; }
        public Column<string> Domain_catalog { get; }
        public Column<string> Domain_schema { get; }
        public Column<string> Domain_name { get; }

        public ColumnsTable() : base(tableName: "columns", schemaName: "information_schema") {

            Table_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            Table_name = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            Column_name = new Column<StringKey<IColumnName>>(table: this, columnName: "column_name");
            Ordinal_position = new NullableColumn<int>(table: this, columnName: "ordinal_position");
            Column_default = new Column<string>(table: this, columnName: "column_default");
            Is_nullable = new Column<string>(table: this, columnName: "is_nullable");
            Data_type = new Column<string>(table: this, columnName: "data_type");
            Character_maximum_length = new NullableColumn<int>(table: this, columnName: "character_maximum_length");
            Character_octet_length = new NullableColumn<int>(table: this, columnName: "character_octet_length");
            Numeric_precision = new NullableColumn<byte>(table: this, columnName: "numeric_precision");
            Numeric_precision_radix = new NullableColumn<short>(table: this, columnName: "numeric_precision_radix");
            Numeric_scale = new NullableColumn<int>(table: this, columnName: "numeric_scale");
            Datetime_precision = new NullableColumn<short>(table: this, columnName: "datetime_precision");
            Character_set_catalog = new Column<string>(table: this, columnName: "character_set_catalog");
            Character_set_schema = new Column<string>(table: this, columnName: "character_set_schema");
            Character_set_name = new Column<string>(table: this, columnName: "character_set_name");
            Collation_catalog = new Column<string>(table: this, columnName: "collation_catalog");
            Collation_schema = new Column<string>(table: this, columnName: "collation_schema");
            Collation_name = new Column<string>(table: this, columnName: "collation_name");
            Domain_catalog = new Column<string>(table: this, columnName: "domain_catalog");
            Domain_schema = new Column<string>(table: this, columnName: "domain_schema");
            Domain_name = new Column<string>(table: this, columnName: "domain_name");
        }
    }

    public sealed class ColumnsRow {

        public ColumnsRow() { }

        public ColumnsRow(IResultRow result, ColumnsTable table) {
            Table_schema = result.Get(table.Table_schema);
            Table_name = result.Get(table.Table_name);
            Column_name = result.Get(table.Column_name);
            Ordinal_position = result.Get(table.Ordinal_position);
            Column_default = result.Get(table.Column_default);
            Is_nullable = result.Get(table.Is_nullable);
            Data_type = result.Get(table.Data_type);
            Character_maximum_length = result.Get(table.Character_maximum_length);
            Character_octet_length = result.Get(table.Character_octet_length);
            Numeric_precision = result.Get(table.Numeric_precision);
            Numeric_precision_radix = result.Get(table.Numeric_precision_radix);
            Numeric_scale = result.Get(table.Numeric_scale);
            Datetime_precision = result.Get(table.Datetime_precision);
            Character_set_catalog = result.Get(table.Character_set_catalog);
            Character_set_schema = result.Get(table.Character_set_schema);
            Character_set_name = result.Get(table.Character_set_name);
            Collation_catalog = result.Get(table.Collation_catalog);
            Collation_schema = result.Get(table.Collation_schema);
            Collation_name = result.Get(table.Collation_name);
            Domain_catalog = result.Get(table.Domain_catalog);
            Domain_schema = result.Get(table.Domain_schema);
            Domain_name = result.Get(table.Domain_name);
        }

        public StringKey<ISchemaName> Table_schema { get; set; }

        public StringKey<ITableName> Table_name { get; set; }

        public StringKey<IColumnName> Column_name { get; set; }

        public int? Ordinal_position { get; set; }

        public string Column_default { get; set; } = string.Empty;

        public string Is_nullable { get; set; } = string.Empty;

        public string Data_type { get; set; } = string.Empty;

        public int? Character_maximum_length { get; set; }

        public int? Character_octet_length { get; set; }

        public int? Numeric_precision { get; set; }

        public int? Numeric_precision_radix { get; set; }

        public int? Numeric_scale { get; set; }

        public int? Datetime_precision { get; set; }

        public string Character_set_catalog { get; set; } = string.Empty;

        public string Character_set_schema { get; set; } = string.Empty;

        public string Character_set_name { get; set; } = string.Empty;

        public string Collation_catalog { get; set; } = string.Empty;

        public string Collation_schema { get; set; } = string.Empty;

        public string Collation_name { get; set; } = string.Empty;

        public string Domain_catalog { get; set; } = string.Empty;

        public string Domain_schema { get; set; } = string.Empty;

        public string Domain_name { get; set; } = string.Empty;
    }
}