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