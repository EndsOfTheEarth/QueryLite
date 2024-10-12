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

    public sealed class CheckConstraintsView : ATable {

        public static readonly CheckConstraintsView Instance = new CheckConstraintsView();

        public Column<StringKey<IConstraintCatalog>> ConstraintCatalog { get; }
        public Column<StringKey<ISchemaName>> ConstraintSchema { get; }
        public Column<StringKey<IConstraintName>> ConstraintName { get; }
        public Column<string> CheckClause { get; }

        private CheckConstraintsView() : base(tableName: "check_constraints", schemaName: "information_schema", isView: true) {

            ConstraintCatalog = new Column<StringKey<IConstraintCatalog>>(this, columnName: "constraint_catalog");
            ConstraintSchema = new Column<StringKey<ISchemaName>>(this, columnName: "constraint_schema");
            ConstraintName = new Column<StringKey<IConstraintName>>(this, columnName: "constraint_name");
            CheckClause = new Column<string>(this, columnName: "check_clause", length: 1073741824);
        }
    }
}