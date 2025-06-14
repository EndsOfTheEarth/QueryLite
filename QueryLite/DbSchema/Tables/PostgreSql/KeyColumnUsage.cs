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

    public sealed class KeyColumnUsageTable : ATable {

        public static readonly KeyColumnUsageTable Instance = new KeyColumnUsageTable();
        public static readonly KeyColumnUsageTable Instance2 = new KeyColumnUsageTable();

        public Column<string> Constraint_catalog { get; }
        public Column<StringKey<ISchemaName>> Constraint_schema { get; }
        public Column<string> Constraint_name { get; }
        public Column<string> Table_catalog { get; }
        public Column<StringKey<ISchemaName>> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<StringKey<IColumnName>> Column_name { get; }
        public NullableColumn<int> Ordinal_position { get; }
        public NullableColumn<int> Position_in_unique_constraint { get; }

        public KeyColumnUsageTable() : base(tableName: "key_column_usage", schemaName: "information_schema") {

            Constraint_catalog = new Column<string>(table: this, columnName: "constraint_catalog");
            Constraint_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "constraint_schema");
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