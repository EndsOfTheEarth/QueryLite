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

    public sealed class ExtendedPropertiesView : ATable {

        public static readonly ExtendedPropertiesView Instance = new ExtendedPropertiesView();

        public Column<short> Class { get; }
        public NullableColumn<string> Class_Desc { get; }
        public Column<int> Major_Id { get; }
        public Column<int> Minor_Id { get; }
        public Column<string> Name { get; }
        public NullableColumn<string> Value { get; }

        public ExtendedPropertiesView() : base(tableName: "extended_properties", schemaName: "sys") {

            Class = new Column<short>(this, columnName: "class");
            Class_Desc = new NullableColumn<string>(this, columnName: "class_desc", length: 60);
            Major_Id = new Column<int>(this, columnName: "major_id");
            Minor_Id = new Column<int>(this, columnName: "minor_id");
            Name = new Column<string>(this, columnName: "name", length: 128);
            Value = new NullableColumn<string>(this, columnName: "value");
        }
    }

    public sealed class TablesView : ATable {

        public static readonly TablesView Instance = new TablesView();

        public Column<int> Object_Id { get; }
        public Column<int> Schema_Id { get; }
        public Column<StringKey<ITableName>> Table_Name { get; }

        public TablesView() : base(tableName: "tables", schemaName: "sys") {

            Object_Id = new Column<int>(this, columnName: "object_id");
            Schema_Id = new Column<int>(this, columnName: "schema_id");
            Table_Name = new Column<StringKey<ITableName>>(this, columnName: "name", length: 128);
        }
    }

    public sealed class ColumnsView : ATable {

        public static readonly ColumnsView Instance = new ColumnsView();

        public Column<int> Object_Id { get; }
        public Column<int> Column_Id { get; }
        public Column<StringKey<IColumnName>> Column_Name { get; }

        public ColumnsView() : base(tableName: "columns", schemaName: "sys") {

            Object_Id = new Column<int>(this, columnName: "object_id");
            Column_Id = new Column<int>(this, columnName: "column_id");
            Column_Name = new Column<StringKey<IColumnName>>(this, columnName: "name", length: 128);
        }
    }

    public sealed class SchemasView : ATable {

        public static readonly SchemasView Instance = new SchemasView();

        public Column<int> Schema_Id { get; }
        public Column<StringKey<ISchemaName>> Schema_Name { get; }

        public SchemasView() : base(tableName: "schemas", schemaName: "sys") {

            Schema_Id = new Column<int>(this, columnName: "schema_id");
            Schema_Name = new Column<StringKey<ISchemaName>>(this, columnName: "name", length: 128);
        }
    }
}