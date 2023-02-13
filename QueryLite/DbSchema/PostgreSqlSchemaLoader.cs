﻿/*
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
using System;
using QueryLite.DbSchema.Tables.PostgreSql;
using System.Collections.Generic;
using QueryLite.DbSchema.Tables;

namespace QueryLite.DbSchema {

    public sealed class PostgreSqlSchemaLoader {

        public static List<DatabaseTable> LoadTables(IDatabase database) {

            List<DatabaseTable> tableList = new List<DatabaseTable>();
            Dictionary<StringKey<ITableName>, DatabaseTable> tableLookup = new Dictionary<StringKey<ITableName>, DatabaseTable>();
            Dictionary<string, Type?> typelookup = new Dictionary<string, Type?>();

            /*
             * Load tables and columns
             **/
            TablesTable tablesTable = TablesTable.Instance;
            ColumnsTable columnsTable = ColumnsTable.Instance;

            var query = Query
                .Select(
                    result => new {
                        Table_schema = result.Get(tablesTable.Table_schema),
                        Table_name = result.Get(tablesTable.Table_name),
                        Table_type = result.Get(tablesTable.Table_type),
                        ColumnsRow = new ColumnsRow(result, columnsTable)
                    }
                )
                .From(tablesTable)
                .Join(columnsTable).On(tablesTable.Table_schema == columnsTable.Table_schema & tablesTable.Table_name == columnsTable.Table_name)
                .OrderBy(columnsTable.Ordinal_position)
                .Execute(database);

            for(int index = 0; index < query.Rows.Count; index++) {

                var row = query.Rows[index];

                if(!tableLookup.TryGetValue(row.Table_name, out DatabaseTable? databaseTable)) {
                    databaseTable = new DatabaseTable(schema: row.Table_schema, tableName: row.Table_name, isView: row.Table_type == "VIEW");
                    tableList.Add(databaseTable);
                    tableLookup.Add(databaseTable.TableName, databaseTable);
                }

                ColumnsRow columnRow = row.ColumnsRow;

                if(!typelookup.TryGetValue(columnRow.Data_type, out Type? dotNetType)) {
                    dotNetType = PostgreSqlTypes.GetDotNetType(columnRow.Data_type);
                    typelookup.Add(columnRow.Data_type, dotNetType);
                }

                DataType dataType = new DataType(name: columnRow.Data_type, dotNetType: (dotNetType ?? typeof(StringKey<IUnknownType>)));

                int? length = null;

                if(columnRow.Character_octet_length != null) {
                    length = columnRow.Character_octet_length.Value;
                }

                if(columnRow.Character_maximum_length != null) {
                    length = columnRow.Character_maximum_length.Value;
                }

                bool isNullable = string.Compare(columnRow.Is_nullable, "YES", true) == 0;

                bool isAutoGenerated = (columnRow.Column_default ?? string.Empty).StartsWith("nextval");

                DatabaseColumn databaseColumn = new DatabaseColumn(
                    table: databaseTable,
                    columnName: columnRow.Column_name,
                    dataType: dataType,
                    length: length,
                    isNullable: isNullable,
                    isAutoGenerated: isAutoGenerated
                );
                databaseTable!.Columns.Add(databaseColumn);
            }

            SetPrimaryKeys(tableList, database);
            SetForeignKeys(tableList, database);

            return tableList;
        }

        private static void SetPrimaryKeys(List<DatabaseTable> tableList, IDatabase database) {

            Dictionary<TableKey, DatabaseTable> dbTableLookup = new Dictionary<TableKey, DatabaseTable>();

            foreach(DatabaseTable table in tableList) {
                dbTableLookup.Add(new TableKey(table.Schema, table.TableName), table);
            }

            TableConstraintsTable tableConstraints = TableConstraintsTable.Instance;
            KeyColumnUsageTable keyColumnUsage = KeyColumnUsageTable.Instance;

            var result = Query
                .Select(
                    row => new {
                        Table_schema = row.Get(keyColumnUsage.Table_schema),
                        Table_name = row.Get(keyColumnUsage.Table_name),
                        Column_name = row.Get(keyColumnUsage.Column_name),
                        Constraint_Name = row.Get(tableConstraints.Constraint_name)
                    }
                )
                .From(tableConstraints)
                .Join(keyColumnUsage).On(
                    tableConstraints.Table_schema == keyColumnUsage.Table_schema &
                    tableConstraints.Table_name == keyColumnUsage.Table_name &
                    tableConstraints.Constraint_name == keyColumnUsage.Constraint_name
                )
                .Where(tableConstraints.Constraint_type == "PRIMARY KEY" & keyColumnUsage.Ordinal_position.IsNotNull)
                .OrderBy(keyColumnUsage.Ordinal_position)
                .Execute(database, TimeoutLevel.ShortSelect);

            foreach(var row in result.Rows) {

                DatabaseTable table = dbTableLookup[new TableKey(row.Table_schema, row.Table_name)];

                if(table.PrimaryKey == null) {
                    table.PrimaryKey = new DatabasePrimaryKey(constraintName: row.Constraint_Name);
                }
                table.PrimaryKey.ColumnNames.Add(row.Column_name.Value);

                foreach(DatabaseColumn dbColumn in table.Columns) {

                    if(string.Compare(dbColumn.ColumnName.Value, row.Column_name.Value, ignoreCase: true) == 0) {
                        dbColumn.IsPrimaryKey = true;
                    }
                }
            }
        }

        private static void SetForeignKeys(List<DatabaseTable> tableList, IDatabase database) {

            Dictionary<TableKey, DatabaseTable> tableLookup = new Dictionary<TableKey, DatabaseTable>();
            Dictionary<TableColumnKey, DatabaseColumn> columnLookup = new Dictionary<TableColumnKey, DatabaseColumn>();

            foreach(DatabaseTable table in tableList) {

                tableLookup.Add(new TableKey(table.Schema, table.TableName), table);

                foreach(DatabaseColumn column in table.Columns) {

                    columnLookup.Add(new TableColumnKey(table.Schema, table.TableName, column.ColumnName), column);
                }
            }

            TableConstraintsTable tcTable = TableConstraintsTable.Instance;
            KeyColumnUsageTable kcuTable = KeyColumnUsageTable.Instance;
            ConstraintColumnUsageTable ccuTable = ConstraintColumnUsageTable.Instance;

            var result = Query
                .Select(
                    result => new {
                        Table_schema = result.Get(kcuTable.Table_schema),
                        kcuTable_name = result.Get(kcuTable.Table_name),
                        Column_name = result.Get(kcuTable.Column_name),
                        Constraint_schema = result.Get(ccuTable.Constraint_schema),
                        Constraint_Name = result.Get(kcuTable.Constraint_name),
                        ccuTable_name = result.Get(ccuTable.Table_name)
                    }
                )
                .From(tcTable)
                .Join(kcuTable).On(tcTable.Table_schema == kcuTable.Table_schema & tcTable.Table_name == kcuTable.Table_name & tcTable.Constraint_name == kcuTable.Constraint_name)
                .Join(ccuTable).On(tcTable.Constraint_schema == ccuTable.Constraint_schema & tcTable.Constraint_name == ccuTable.Constraint_name)
                .Where(tcTable.Constraint_type == "FOREIGN KEY")
                .OrderBy(kcuTable.Ordinal_position)
                .Execute(database, TimeoutLevel.ShortSelect);

            for(int index = 0; index < result.Rows.Count; index++) {

                var row = result.Rows[index];

                TableColumnKey columnKey = new TableColumnKey(row.Table_schema, row.kcuTable_name, row.Column_name);

                DatabaseColumn column = columnLookup[columnKey];

                TableKey tableKey = new TableKey(row.Constraint_schema, row.ccuTable_name);
                DatabaseTable foreignKeyTable = tableLookup[tableKey];

                //if(column.IsForeignKey != false || column.ForeignKeyTable != null) {
                //    throw new Exception($"{nameof(column)} already has a foreign key set. This might be a bug. Column Name {column}");
                //}
                column.IsForeignKey = true;

                column.IsForeignKey = true;
                column.ForeignKeys.Add(new ForeignKey_(row.Constraint_Name, foreignKeyTable));
            }
        }

        private sealed class TableKey {

            private readonly StringKey<ISchemaName> SchemaName;
            private readonly StringKey<ITableName> TableName;

            public TableKey(StringKey<ISchemaName> schemaName, StringKey<ITableName> tableName) {
                SchemaName = schemaName;
                TableName = tableName;
            }

            public override bool Equals(object? obj) {

                if(obj is TableKey key) {
                    return SchemaName.Value == key.SchemaName.Value && TableName.Value == key.TableName.Value;
                }
                else {
                    return false;
                }
            }
            public override int GetHashCode() {
                return (SchemaName.Value + "^" + TableName.Value).GetHashCode();
            }
        }

        private sealed class TableColumnKey {

            private readonly StringKey<ISchemaName> SchemaName;
            private readonly StringKey<ITableName> TableName;
            private readonly StringKey<IColumnName> ColumnName;

            public TableColumnKey(StringKey<ISchemaName> schemaName, StringKey<ITableName> tableName, StringKey<IColumnName> columnName) {
                SchemaName = schemaName;
                TableName = tableName;
                ColumnName = columnName;
            }

            public override bool Equals(object? obj) {

                if(obj is TableColumnKey key) {
                    return SchemaName.Value == key.SchemaName.Value && TableName.Value == key.TableName.Value && ColumnName.Value == key.ColumnName.Value;
                }
                else {
                    return false;
                }
            }
            public override int GetHashCode() {
                return (SchemaName.Value + "^" + TableName.Value + "^" + ColumnName.Value).GetHashCode();
            }
        }
    }

    public static class PostgreSqlTypes {

        private static readonly Dictionary<string, Type?> _Lookup = new Dictionary<string, Type?>();

        static PostgreSqlTypes() {

            _Lookup.Add("anyarray", null);
            _Lookup.Add("inet", null);
            _Lookup.Add("_text", typeof(string));
            _Lookup.Add("xid", null);
            _Lookup.Add("_char", typeof(string));
            _Lookup.Add("name", null);
            _Lookup.Add("oidvector", null);
            _Lookup.Add("_aclitem", null);
            _Lookup.Add("int2vector", null);
            _Lookup.Add("_int2", typeof(short));
            _Lookup.Add("interval", null);
            _Lookup.Add("abstime", null);
            _Lookup.Add("regproc", null);
            _Lookup.Add("_regtype", null);
            _Lookup.Add("ARRAY", null);
            _Lookup.Add("bytea", typeof(byte[]));
            _Lookup.Add("date", typeof(DateOnly));
            _Lookup.Add("time", typeof(TimeOnly));
            _Lookup.Add("timestamptz", typeof(DateTimeOffset));
            _Lookup.Add("integer", typeof(int));
            _Lookup.Add("int4", typeof(int));
            _Lookup.Add("int8", typeof(long));
            _Lookup.Add("uuid", typeof(Guid));
            _Lookup.Add("_float4", typeof(float));
            _Lookup.Add("char", typeof(string));
            _Lookup.Add("\"char\"", typeof(string));
            _Lookup.Add("bool", typeof(bool));
            _Lookup.Add("float4", typeof(float));
            _Lookup.Add("float8", typeof(double));
            _Lookup.Add("int2", typeof(short));
            _Lookup.Add("_oid", typeof(uint));
            _Lookup.Add("oid", typeof(uint));
            _Lookup.Add("varchar", typeof(string));
            _Lookup.Add("bpchar", typeof(string));
            _Lookup.Add("text", typeof(string));
            _Lookup.Add("timestamp", typeof(DateTime));
            _Lookup.Add("numeric", typeof(decimal));
            _Lookup.Add("boolean", typeof(bool));
            _Lookup.Add("smallint", typeof(short));
            _Lookup.Add("time without time zone", typeof(TimeOnly));
            _Lookup.Add("timestamp with time zone", typeof(DateTimeOffset));
            _Lookup.Add("timestamp without time zone", typeof(DateTime));
            _Lookup.Add("real", typeof(float));
            _Lookup.Add("bigint", typeof(long));
            _Lookup.Add("double precision", typeof(double));
            _Lookup.Add("character varying", typeof(string));
        }
        public static Type? GetDotNetType(string typeName) {

            if(!_Lookup.TryGetValue(typeName, out Type? dotNetType)) {
                return null;
            }
            return dotNetType;
        }
    }
}