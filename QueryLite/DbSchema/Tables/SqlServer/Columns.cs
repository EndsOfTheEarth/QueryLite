/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class ColumnsTable : ATable {

        public static readonly ColumnsTable Instance = new();

        public Column<SchemaName, string> TableSchema { get; }
        public Column<TableName, string> TableName_ { get; }
        public Column<ColumnName, string> ColumnName { get; }
        public NullableColumn<int> OrdinalPosition { get; }
        public Column<string> Columndefault { get; }
        public Column<string> IsNullable { get; }
        public Column<string> DataType { get; }
        public NullableColumn<int> CharacterMaximumLength { get; }
        public NullableColumn<int> CharacterOctetLength { get; }
        public NullableColumn<byte> NumericPrecision { get; }
        public NullableColumn<short> NumericPrecisionRadix { get; }
        public NullableColumn<int> NumericScale { get; }
        public NullableColumn<short> DatetimePrecision { get; }
        public Column<string> CharacterSetCatalog { get; }
        public Column<string> CharacterSetSchema { get; }
        public Column<string> CharacterSetName { get; }
        public Column<string> CollationCatalog { get; }
        public Column<string> CollationSchema { get; }
        public Column<string> CollationName { get; }
        public Column<string> DomainCatalog { get; }
        public Column<string> DomainSchema { get; }
        public Column<string> DomainName { get; }

        public ColumnsTable() : base(tableName: "columns", schemaName: "information_schema") {

            TableSchema = new Column<SchemaName, string>(table: this, columnName: "table_schema");
            TableName_ = new Column<TableName, string>(table: this, columnName: "table_name");
            ColumnName = new Column<ColumnName, string>(table: this, columnName: "column_name");
            OrdinalPosition = new NullableColumn<int>(table: this, columnName: "ordinal_position");
            Columndefault = new Column<string>(table: this, columnName: "column_default");
            IsNullable = new Column<string>(table: this, columnName: "is_nullable");
            DataType = new Column<string>(table: this, columnName: "data_type");
            CharacterMaximumLength = new NullableColumn<int>(table: this, columnName: "character_maximum_length");
            CharacterOctetLength = new NullableColumn<int>(table: this, columnName: "character_octet_length");
            NumericPrecision = new NullableColumn<byte>(table: this, columnName: "numeric_precision");
            NumericPrecisionRadix = new NullableColumn<short>(table: this, columnName: "numeric_precision_radix");
            NumericScale = new NullableColumn<int>(table: this, columnName: "numeric_scale");
            DatetimePrecision = new NullableColumn<short>(table: this, columnName: "datetime_precision");
            CharacterSetCatalog = new Column<string>(table: this, columnName: "character_set_catalog");
            CharacterSetSchema = new Column<string>(table: this, columnName: "character_set_schema");
            CharacterSetName = new Column<string>(table: this, columnName: "character_set_name");
            CollationCatalog = new Column<string>(table: this, columnName: "collation_catalog");
            CollationSchema = new Column<string>(table: this, columnName: "collation_schema");
            CollationName = new Column<string>(table: this, columnName: "collation_name");
            DomainCatalog = new Column<string>(table: this, columnName: "domain_catalog");
            DomainSchema = new Column<string>(table: this, columnName: "domain_schema");
            DomainName = new Column<string>(table: this, columnName: "domain_name");
        }
    }

    public sealed class ColumnsRow {

        public ColumnsRow() { }

        public ColumnsRow(IResultRow result, ColumnsTable table) {
            Table_schema = result.Get(table.TableSchema);
            Table_name = result.Get(table.TableName_);
            Column_name = result.Get(table.ColumnName);
            Ordinal_position = result.Get(table.OrdinalPosition);
            Column_default = result.Get(table.Columndefault);
            Is_nullable = result.Get(table.IsNullable);
            Data_type = result.Get(table.DataType);
            Character_maximum_length = result.Get(table.CharacterMaximumLength);
            Character_octet_length = result.Get(table.CharacterOctetLength);
            Numeric_precision = result.Get(table.NumericPrecision);
            Numeric_precision_radix = result.Get(table.NumericPrecisionRadix);
            Numeric_scale = result.Get(table.NumericScale);
            Datetime_precision = result.Get(table.DatetimePrecision);
            Character_set_catalog = result.Get(table.CharacterSetCatalog);
            Character_set_schema = result.Get(table.CharacterSetSchema);
            Character_set_name = result.Get(table.CharacterSetName);
            Collation_catalog = result.Get(table.CollationCatalog);
            Collation_schema = result.Get(table.CollationSchema);
            Collation_name = result.Get(table.CollationName);
            Domain_catalog = result.Get(table.DomainCatalog);
            Domain_schema = result.Get(table.DomainSchema);
            Domain_name = result.Get(table.DomainName);
        }

        public SchemaName Table_schema { get; set; }

        public TableName Table_name { get; set; }

        public ColumnName Column_name { get; set; }

        public int? Ordinal_position { get; set; }

        public string Column_default { get; set; } = "";

        public string Is_nullable { get; set; } = "";

        public string Data_type { get; set; } = "";

        public int? Character_maximum_length { get; set; }

        public int? Character_octet_length { get; set; }

        public int? Numeric_precision { get; set; }

        public int? Numeric_precision_radix { get; set; }

        public int? Numeric_scale { get; set; }

        public int? Datetime_precision { get; set; }

        public string Character_set_catalog { get; set; } = "";

        public string Character_set_schema { get; set; } = "";

        public string Character_set_name { get; set; } = "";

        public string Collation_catalog { get; set; } = "";

        public string Collation_schema { get; set; } = "";

        public string Collation_name { get; set; } = "";

        public string Domain_catalog { get; set; } = "";

        public string Domain_schema { get; set; } = "";

        public string Domain_name { get; set; } = "";
    }
}