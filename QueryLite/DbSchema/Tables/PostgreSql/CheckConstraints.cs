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

    public sealed class CheckConstraintsTable : ATable {

        public static readonly CheckConstraintsTable Instance = new();

        public Column<string> ConstraintCatalog { get; }
        public Column<SchemaName, string> ConstraintSchema { get; }
        public Column<string> ConstraintName { get; }
        public Column<string> CheckCaluse { get; }

        public CheckConstraintsTable() : base(tableName: "check_constraints", schemaName: "information_schema") {
            ConstraintCatalog = new Column<string>(table: this, name: "constraint_catalog");
            ConstraintSchema = new Column<SchemaName, string>(table: this, name: "constraint_schema");
            ConstraintName = new Column<string>(table: this, name: "constraint_name");
            CheckCaluse = new Column<string>(table: this, name: "check_clause");
        }
    }
}