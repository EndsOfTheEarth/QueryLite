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

    public sealed class ExtendedPropertiesView : ATable {

        public static readonly ExtendedPropertiesView Instance = new();

        public Column<short> Class { get; }
        public NullableColumn<string> ClassDesc { get; }
        public Column<int> MajorId { get; }
        public Column<int> MinorId { get; }
        public Column<string> Name { get; }
        public NullableColumn<string> Value { get; }

        public ExtendedPropertiesView() : base(tableName: "extended_properties", schemaName: "sys") {

            Class = new Column<short>(this, name: "class");
            ClassDesc = new NullableColumn<string>(this, name: "class_desc", length: new(60));
            MajorId = new Column<int>(this, name: "major_id");
            MinorId = new Column<int>(this, name: "minor_id");
            Name = new Column<string>(this, name: "name", length: new(128));
            Value = new NullableColumn<string>(this, name: "value");
        }
    }

    public sealed class TablesView : ATable {

        public static readonly TablesView Instance = new();

        public Column<int> Object_Id { get; }
        public Column<int> Schema_Id { get; }
        public Column<TableName, string> Table_Name { get; }

        public TablesView() : base(tableName: "tables", schemaName: "sys") {

            Object_Id = new Column<int>(this, name: "object_id");
            Schema_Id = new Column<int>(this, name: "schema_id");
            Table_Name = new Column<TableName, string>(this, name: "name", length: new(128));
        }
    }

    public sealed class ColumnsView : ATable {

        public static readonly ColumnsView Instance = new();

        public Column<int> Object_Id { get; }
        public Column<int> Column_Id { get; }
        public Column<ColumnName, string> Column_Name { get; }

        public ColumnsView() : base(tableName: "columns", schemaName: "sys") {

            Object_Id = new Column<int>(this, name: "object_id");
            Column_Id = new Column<int>(this, name: "column_id");
            Column_Name = new Column<ColumnName, string>(this, name: "name", length: new(128));
        }
    }

    public sealed class SchemasView : ATable {

        public static readonly SchemasView Instance = new();

        public Column<int> Schema_Id { get; }
        public Column<SchemaName, string> Schema_Name { get; }

        public SchemasView() : base(tableName: "schemas", schemaName: "sys") {

            Schema_Id = new Column<int>(this, name: "schema_id");
            Schema_Name = new Column<SchemaName, string>(this, name: "name", length: new(128));
        }
    }
}