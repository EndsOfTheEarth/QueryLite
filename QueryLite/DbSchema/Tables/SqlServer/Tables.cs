/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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

    public sealed class TablesTable : ATable {

        public static readonly TablesTable Instance = new TablesTable();

        public Column<string> TABLE_CATALOG { get; }
        public Column<StringKey<ISchemaName>> TABLE_SCHEMA { get; }
        public Column<StringKey<ITableName>> TABLE_NAME { get; }
        public Column<string> TABLE_TYPE { get; }

        public TablesTable() : base(tableName: "tables", schemaName: "information_schema") {

            TABLE_CATALOG = new Column<string>(table: this, columnName: "table_catalog");
            TABLE_SCHEMA = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            TABLE_NAME = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            TABLE_TYPE = new Column<string>(table: this, columnName: "table_type");
        }
    }
}