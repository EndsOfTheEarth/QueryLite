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

namespace QueryLite.DbSchema.Tables.PostgreSql {

    public sealed class TablesTable : ATable {

        public static readonly TablesTable Instance = new TablesTable();

        public Column<string> Table_catalog { get; }
        public Column<StringKey<ISchemaName>> Table_schema { get; }
        public Column<StringKey<ITableName>> Table_name { get; }
        public Column<string> Table_type { get; }
        public Column<string> Self_referencing_column_name { get; }
        public Column<string> Reference_generation { get; }
        public Column<string> User_defined_type_catalog { get; }
        public Column<string> User_defined_type_schema { get; }
        public Column<string> User_defined_type_name { get; }
        public Column<string> Is_insertable_into { get; }
        public Column<string> Is_typed { get; }
        public Column<string> Commit_action { get; }

        public TablesTable() : base(tableName: "tables", schemaName: "information_schema", isView: true) {

            Table_catalog = new Column<string>(table: this, columnName: "table_catalog");
            Table_schema = new Column<StringKey<ISchemaName>>(table: this, columnName: "table_schema");
            Table_name = new Column<StringKey<ITableName>>(table: this, columnName: "table_name");
            Table_type = new Column<string>(table: this, columnName: "table_type");
            Self_referencing_column_name = new Column<string>(table: this, columnName: "self_referencing_column_name");
            Reference_generation = new Column<string>(table: this, columnName: "reference_generation");
            User_defined_type_catalog = new Column<string>(table: this, columnName: "user_defined_type_catalog");
            User_defined_type_schema = new Column<string>(table: this, columnName: "user_defined_type_schema");
            User_defined_type_name = new Column<string>(table: this, columnName: "user_defined_type_name");
            Is_insertable_into = new Column<string>(table: this, columnName: "is_insertable_into");
            Is_typed = new Column<string>(table: this, columnName: "is_typed");
            Commit_action = new Column<string>(table: this, columnName: "commit_action");
        }
    }
}