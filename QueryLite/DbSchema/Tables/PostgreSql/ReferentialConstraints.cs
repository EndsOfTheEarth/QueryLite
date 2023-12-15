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

    public sealed class ReferentialConstraintsTable : ATable {

        public static readonly ReferentialConstraintsTable Instance = new ReferentialConstraintsTable();

        public NullableColumn<string> CONSTRAINT_CATALOG { get; }
        public NullableColumn<StringKey<ISchemaName>> CONSTRAINT_SCHEMA { get; }
        public Column<string> CONSTRAINT_NAME { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_CATALOG { get; }
        public NullableColumn<StringKey<ISchemaName>> UNIQUE_CONSTRAINT_SCHEMA { get; }
        public NullableColumn<string> UNIQUE_CONSTRAINT_NAME { get; }
        public NullableColumn<string> MATCH_OPTION { get; }
        public NullableColumn<string> UPDATE_RULE { get; }
        public NullableColumn<string> DELETE_RULE { get; }

        public ReferentialConstraintsTable() : base(tableName: "REFERENTIAL_CONSTRAINTS", schemaName: "information_schema") {

            CONSTRAINT_CATALOG = new NullableColumn<string>(this, columnName: "CONSTRAINT_CATALOG", length: 128);
            CONSTRAINT_SCHEMA = new NullableColumn<StringKey<ISchemaName>>(this, columnName: "CONSTRAINT_SCHEMA", length: 128);
            CONSTRAINT_NAME = new Column<string>(this, columnName: "CONSTRAINT_NAME", length: 128);
            UNIQUE_CONSTRAINT_CATALOG = new NullableColumn<string>(this, columnName: "UNIQUE_CONSTRAINT_CATALOG", length: 128);
            UNIQUE_CONSTRAINT_SCHEMA = new NullableColumn<StringKey<ISchemaName>>(this, columnName: "UNIQUE_CONSTRAINT_SCHEMA", length: 128);
            UNIQUE_CONSTRAINT_NAME = new NullableColumn<string>(this, columnName: "UNIQUE_CONSTRAINT_NAME", length: 128);
            MATCH_OPTION = new NullableColumn<string>(this, columnName: "MATCH_OPTION", length: 7);
            UPDATE_RULE = new NullableColumn<string>(this, columnName: "UPDATE_RULE", length: 11);
            DELETE_RULE = new NullableColumn<string>(this, columnName: "DELETE_RULE", length: 11);
        }
    }
}