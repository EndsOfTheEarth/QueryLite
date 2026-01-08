/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
namespace QueryLite.DbSchema.Tables.PostgreSql {

    public sealed class ConstraintColumnUsageTable : ATable {

        public static readonly ConstraintColumnUsageTable Instance = new();

        public Column<string> TableCatalog { get; }
        public Column<SchemaName, string> TableSchema { get; }
        public Column<TableName, string> TableName_ { get; }
        public Column<ColumnName, string> ColumnName { get; }
        public Column<string> ConstraintCatalog { get; }
        public Column<SchemaName, string> ConstraintSchema { get; }
        public Column<string> ConstraintName { get; }

        public ConstraintColumnUsageTable() : base(tableName: "constraint_column_usage", schemaName: "information_schema") {

            TableCatalog = new Column<string>(table: this, name: "table_catalog");
            TableSchema = new Column<SchemaName, string>(table: this, name: "table_schema");
            TableName_ = new Column<TableName, string>(table: this, name: "table_name");
            ColumnName = new Column<ColumnName, string>(table: this, name: "column_name");
            ConstraintCatalog = new Column<string>(table: this, name: "constraint_catalog");
            ConstraintSchema = new Column<SchemaName, string>(table: this, name: "constraint_schema");
            ConstraintName = new Column<string>(table: this, name: "constraint_name");
        }
    }
}