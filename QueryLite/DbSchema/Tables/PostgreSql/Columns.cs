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
using QueryLite.Utility;

namespace QueryLite.DbSchema.Tables.PostgreSql {

    public sealed class ColumnsTable : ATable {

        public static readonly ColumnsTable Instance = new ColumnsTable();

        public Column<string> Table_catalog { get; }
        public Column<StringKey<ISchemaName>> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<StringKey<IColumnName>> Column_name { get; }
        public NullableColumn<int> Ordinal_position { get; }
        public Column<string> Column_default { get; }
        public Column<string> Is_nullable { get; }
        public Column<string> Data_type { get; }
        public NullableColumn<int> Character_maximum_length { get; }
        public NullableColumn<int> Character_octet_length { get; }
        public NullableColumn<int> Numeric_precision { get; }
        public NullableColumn<int> Numeric_precision_radix { get; }
        public NullableColumn<int> Numeric_scale { get; }
        public NullableColumn<int> Datetime_precision { get; }
        public Column<string> Interval_type { get; }
        public Column<string> Interval_precision { get; }
        public Column<string> Character_set_catalog { get; }
        public Column<string> Character_set_schema { get; }
        public Column<string> Character_set_name { get; }
        public Column<string> Collation_catalog { get; }
        public Column<string> Collation_schema { get; }
        public Column<string> Collation_name { get; }
        public Column<string> Domain_catalog { get; }
        public Column<string> Domain_schema { get; }
        public Column<string> Domain_name { get; }
        public Column<string> Udt_catalog { get; }
        public Column<string> Udt_schema { get; }
        public Column<string> Udt_name { get; }
        public Column<string> Scope_catalog { get; }
        public Column<string> Scope_schema { get; }
        public Column<string> Scope_name { get; }
        public NullableColumn<int> Maximum_cardinality { get; }
        public Column<string> Dtd_identifier { get; }
        public Column<string> Is_self_referencing { get; }
        public Column<string> Is_identity { get; }
        public Column<string> Identity_generation { get; }
        public Column<string> Identity_start { get; }
        public Column<string> Identity_increment { get; }
        public Column<string> Identity_maximum { get; }
        public Column<string> Identity_minimum { get; }
        public Column<string> Identity_cycle { get; }
        public Column<string> Is_generated { get; }
        public Column<string> Generation_expression { get; }
        public Column<string> Is_updatable { get; }

        public ColumnsTable() : base(tableName: "columns", schemaName: "information_schema") {

            Table_catalog = new Column<string>(table: this, columnName: "table_catalog");
            Table_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            Table_name = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            Column_name = new Column<StringKey<IColumnName>>(table: this, columnName: "column_name");
            Ordinal_position = new NullableColumn<int>(table: this, columnName: "ordinal_position");
            Column_default = new Column<string>(table: this, columnName: "column_default");
            Is_nullable = new Column<string>(table: this, columnName: "is_nullable");
            Data_type = new Column<string>(table: this, columnName: "data_type");
            Character_maximum_length = new NullableColumn<int>(table: this, columnName: "character_maximum_length");
            Character_octet_length = new NullableColumn<int>(table: this, columnName: "character_octet_length");
            Numeric_precision = new NullableColumn<int>(table: this, columnName: "numeric_precision");
            Numeric_precision_radix = new NullableColumn<int>(table: this, columnName: "numeric_precision_radix");
            Numeric_scale = new NullableColumn<int>(table: this, columnName: "numeric_scale");
            Datetime_precision = new NullableColumn<int>(table: this, columnName: "datetime_precision");
            Interval_type = new Column<string>(table: this, columnName: "interval_type");
            Interval_precision = new Column<string>(table: this, columnName: "interval_precision");
            Character_set_catalog = new Column<string>(table: this, columnName: "character_set_catalog");
            Character_set_schema = new Column<string>(table: this, columnName: "character_set_schema");
            Character_set_name = new Column<string>(table: this, columnName: "character_set_name");
            Collation_catalog = new Column<string>(table: this, columnName: "collation_catalog");
            Collation_schema = new Column<string>(table: this, columnName: "collation_schema");
            Collation_name = new Column<string>(table: this, columnName: "collation_name");
            Domain_catalog = new Column<string>(table: this, columnName: "domain_catalog");
            Domain_schema = new Column<string>(table: this, columnName: "domain_schema");
            Domain_name = new Column<string>(table: this, columnName: "domain_name");
            Udt_catalog = new Column<string>(table: this, columnName: "udt_catalog");
            Udt_schema = new Column<string>(table: this, columnName: "udt_schema");
            Udt_name = new Column<string>(table: this, columnName: "udt_name");
            Scope_catalog = new Column<string>(table: this, columnName: "scope_catalog");
            Scope_schema = new Column<string>(table: this, columnName: "scope_schema");
            Scope_name = new Column<string>(table: this, columnName: "scope_name");
            Maximum_cardinality = new NullableColumn<int>(table: this, columnName: "maximum_cardinality");
            Dtd_identifier = new Column<string>(table: this, columnName: "dtd_identifier");
            Is_self_referencing = new Column<string>(table: this, columnName: "is_self_referencing");
            Is_identity = new Column<string>(table: this, columnName: "is_identity");
            Identity_generation = new Column<string>(table: this, columnName: "identity_generation");
            Identity_start = new Column<string>(table: this, columnName: "identity_start");
            Identity_increment = new Column<string>(table: this, columnName: "identity_increment");
            Identity_maximum = new Column<string>(table: this, columnName: "identity_maximum");
            Identity_minimum = new Column<string>(table: this, columnName: "identity_minimum");
            Identity_cycle = new Column<string>(table: this, columnName: "identity_cycle");
            Is_generated = new Column<string>(table: this, columnName: "is_generated");
            Generation_expression = new Column<string>(table: this, columnName: "generation_expression");
            Is_updatable = new Column<string>(table: this, columnName: "is_updatable");
        }
    }

    public sealed class ColumnsRow {

        public ColumnsRow() { }

        public ColumnsRow(IResultRow result, ColumnsTable table) {
            Table_catalog = result.Get(table.Table_catalog);
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
            Interval_type = result.Get(table.Interval_type);
            Interval_precision = result.Get(table.Interval_precision);
            Character_set_catalog = result.Get(table.Character_set_catalog);
            Character_set_schema = result.Get(table.Character_set_schema);
            Character_set_name = result.Get(table.Character_set_name);
            Collation_catalog = result.Get(table.Collation_catalog);
            Collation_schema = result.Get(table.Collation_schema);
            Collation_name = result.Get(table.Collation_name);
            Domain_catalog = result.Get(table.Domain_catalog);
            Domain_schema = result.Get(table.Domain_schema);
            Domain_name = result.Get(table.Domain_name);
            Udt_catalog = result.Get(table.Udt_catalog);
            Udt_schema = result.Get(table.Udt_schema);
            Udt_name = result.Get(table.Udt_name);
            Scope_catalog = result.Get(table.Scope_catalog);
            Scope_schema = result.Get(table.Scope_schema);
            Scope_name = result.Get(table.Scope_name);
            Maximum_cardinality = result.Get(table.Maximum_cardinality);
            Dtd_identifier = result.Get(table.Dtd_identifier);
            Is_self_referencing = result.Get(table.Is_self_referencing);
            Is_identity = result.Get(table.Is_identity);
            Identity_generation = result.Get(table.Identity_generation);
            Identity_start = result.Get(table.Identity_start);
            Identity_increment = result.Get(table.Identity_increment);
            Identity_maximum = result.Get(table.Identity_maximum);
            Identity_minimum = result.Get(table.Identity_minimum);
            Identity_cycle = result.Get(table.Identity_cycle);
            Is_generated = result.Get(table.Is_generated);
            Generation_expression = result.Get(table.Generation_expression);
            Is_updatable = result.Get(table.Is_updatable);
        }

        public string Table_catalog { get; set; } = string.Empty;

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

        public string Interval_type { get; set; } = string.Empty;

        public string Interval_precision { get; set; } = string.Empty;

        public string Character_set_catalog { get; set; } = string.Empty;

        public string Character_set_schema { get; set; } = string.Empty;

        public string Character_set_name { get; set; } = string.Empty;

        public string Collation_catalog { get; set; } = string.Empty;

        public string Collation_schema { get; set; } = string.Empty;

        public string Collation_name { get; set; } = string.Empty;

        public string Domain_catalog { get; set; } = string.Empty;

        public string Domain_schema { get; set; } = string.Empty;

        public string Domain_name { get; set; } = string.Empty;

        public string Udt_catalog { get; set; } = string.Empty;

        public string Udt_schema { get; set; } = string.Empty;

        public string Udt_name { get; set; } = string.Empty;

        public string Scope_catalog { get; set; } = string.Empty;

        public string Scope_schema { get; set; } = string.Empty;

        public string Scope_name { get; set; } = string.Empty;

        public int? Maximum_cardinality { get; set; }

        public string Dtd_identifier { get; set; } = string.Empty;

        public string Is_self_referencing { get; set; } = string.Empty;

        public string Is_identity { get; set; } = string.Empty;

        public string Identity_generation { get; set; } = string.Empty;

        public string Identity_start { get; set; } = string.Empty;

        public string Identity_increment { get; set; } = string.Empty;

        public string Identity_maximum { get; set; } = string.Empty;

        public string Identity_minimum { get; set; } = string.Empty;

        public string Identity_cycle { get; set; } = string.Empty;

        public string Is_generated { get; set; } = string.Empty;

        public string Generation_expression { get; set; } = string.Empty;

        public string Is_updatable { get; set; } = string.Empty;
    }
}