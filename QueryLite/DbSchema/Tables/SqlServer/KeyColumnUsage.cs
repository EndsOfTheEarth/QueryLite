/*
 * MIT License
 *
 * Copyright (c) 2023 EndsOfTheEarth
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

namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class KeyColumnUsageTable : ATable {

        public static readonly KeyColumnUsageTable Instance = new KeyColumnUsageTable();
        public static readonly KeyColumnUsageTable Instance2 = new KeyColumnUsageTable();

        public Column<string> CONSTRAINT_CATALOG { get; }
        public Column<StringKey<ISchemaName>> CONSTRAINT_SCHEMA { get; }
        public Column<string> CONSTRAINT_NAME { get; }
        public Column<string> TABLE_CATALOG { get; }
        public Column<StringKey<ISchemaName>> TABLE_SCHEMA { get; }
        public Column<StringKey<ITableName>> TABLE_NAME { get; }
        public Column<StringKey<IColumnName>> COLUMN_NAME { get; }
        public NullableColumn<int> ORDINAL_POSITION { get; }

        public KeyColumnUsageTable() : base(tableName: "key_column_usage", schemaName: "information_schema") {

            CONSTRAINT_CATALOG = new Column<string>(table: this, columnName: "constraint_catalog");
            CONSTRAINT_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "constraint_schema");
            CONSTRAINT_NAME = new Column<string>(table: this, columnName: "constraint_name");
            TABLE_CATALOG = new Column<string>(table: this, columnName: "table_catalog");
            TABLE_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            TABLE_NAME = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            COLUMN_NAME = new Column<StringKey<IColumnName>>(table: this, columnName: "column_name");
            ORDINAL_POSITION = new NullableColumn<int>(this, columnName: "ordinal_position");
        }
    }
}