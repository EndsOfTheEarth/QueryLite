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
namespace QueryLite.DbSchema.Tables.SqlServer {

    public sealed class ReferentialConstraintsTable : ATable {

        public static readonly ReferentialConstraintsTable Instance = new();

        public NullableColumn<string> CONSTRAINT_CATALOG { get; }
        public NullableColumn<SchemaName, string> CONSTRAINT_SCHEMA { get; }
        public Column<string> CONSTRAINT_NAME { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_CATALOG { get; }
        public NullableColumn<SchemaName, string> UNIQUE_CONSTRAINT_SCHEMA { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_NAME { get; }
        public NullableColumn<string> MATCH_OPTION { get; }
        public NullableColumn<string> UPDATE_RULE { get; }
        public NullableColumn<string> DELETE_RULE { get; }

        public ReferentialConstraintsTable() : base(tableName: "REFERENTIAL_CONSTRAINTS", schemaName: "information_schema") {

            CONSTRAINT_CATALOG = new NullableColumn<string>(this, name: "CONSTRAINT_CATALOG", length: new(128));
            CONSTRAINT_SCHEMA = new NullableColumn<SchemaName, string>(this, name: "CONSTRAINT_SCHEMA", length: new(128));
            CONSTRAINT_NAME = new Column<string>(this, name: "CONSTRAINT_NAME", length: new(128));
            UNIQUE_CONSTRAINT_CATALOG = new NullableColumn<string>(this, name: "UNIQUE_CONSTRAINT_CATALOG", length: new(128));
            UNIQUE_CONSTRAINT_SCHEMA = new NullableColumn<SchemaName, string>(this, name: "UNIQUE_CONSTRAINT_SCHEMA", length: new(128));
            UNIQUE_CONSTRAINT_NAME = new NullableColumn<string>(this, name: "UNIQUE_CONSTRAINT_NAME", length: new(128));
            MATCH_OPTION = new NullableColumn<string>(this, name: "MATCH_OPTION", length: new(7));
            UPDATE_RULE = new NullableColumn<string>(this, name: "UPDATE_RULE", length: new(11));
            DELETE_RULE = new NullableColumn<string>(this, name: "DELETE_RULE", length: new(11));
        }
    }
}