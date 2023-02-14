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
using QueryLite.DbSchema.Tables;
using QueryLite.DbSchema.Tables.SqlServer;
using System;
using System.Collections.Generic;

namespace QueryLite.DbSchema {

    public sealed class SqlServerSchemaLoader {

        public sealed class COLUMNPROPERTY : NullableFunction<int> {

            private ColumnsTable Table { get; }

            public COLUMNPROPERTY(ColumnsTable table) : base("COLUMNPROPERTY") {
                Table = table;
            }
            public override string GetSql(IDatabase database, bool useAlias, IParameters? parameters) {
                if(useAlias) {
                    return $"COLUMNPROPERTY(object_id({Table.Alias}.{Table.Table_schema.ColumnName} + '.' + {Table.Alias}.{Table.Table_name.ColumnName}), COLUMN_NAME, 'IsIdentity')";
                }
                else {
                    return $"COLUMNPROPERTY(object_id({Table.Table_schema.ColumnName} + '.' + {Table.Table_name.ColumnName}), COLUMN_NAME, 'IsIdentity')";
                }
            }
        }

        public static List<DatabaseTable> LoadTables(IDatabase database) {

            List<DatabaseTable> tableList = new List<DatabaseTable>();
            Dictionary<StringKey<ITableName>, DatabaseTable> tableLookup = new Dictionary<StringKey<ITableName>, DatabaseTable>();
            Dictionary<string, Type?> typelookup = new Dictionary<string, Type?>();

            /*
             * Load tables and columns
             **/
            TablesTable tablesTable = TablesTable.Instance;
            ColumnsTable columnsTable = ColumnsTable.Instance;

            COLUMNPROPERTY columnProperty = new COLUMNPROPERTY(columnsTable);

            var result = Query
                .Select(
                    result => new {
                        TABLE_SCHEMA = result.Get(tablesTable.TABLE_SCHEMA),
                        TABLE_NAME = result.Get(tablesTable.TABLE_NAME),
                        TABLE_TYPE = result.Get(tablesTable.TABLE_TYPE),
                        Column = new ColumnsRow(result, columnsTable),
                        ColumnProperty = result.Get(columnProperty)
                    }
                )
                .From(tablesTable)
                .Join(columnsTable).On(tablesTable.TABLE_SCHEMA == columnsTable.Table_schema & tablesTable.TABLE_NAME == columnsTable.Table_name)
                .Execute(database, TimeoutLevel.ShortSelect);

            for(int index = 0; index < result.Rows.Count; index++) {

                var row = result.Rows[index];

                if(!tableLookup.TryGetValue(row.TABLE_NAME, out DatabaseTable? databaseTable)) {
                    databaseTable = new DatabaseTable(schema: row.TABLE_SCHEMA, tableName: row.TABLE_NAME, isView: row.TABLE_TYPE == "VIEW");
                    tableList.Add(databaseTable);
                    tableLookup.Add(databaseTable.TableName, databaseTable);
                }

                ColumnsRow columnRow = row.Column;

                if(!typelookup.TryGetValue(columnRow.Data_type, out Type? dotNetType)) {
                    dotNetType = SqlServerTypes.GetDotNetType(columnRow.Data_type);
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

                int? isIdentity = row.ColumnProperty;

                bool isAutoGenerated = isIdentity != null && isIdentity == 1;

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
                        TABLE_SCHEMA = row.Get(keyColumnUsage.TABLE_SCHEMA),
                        TABLE_NAME = row.Get(keyColumnUsage.TABLE_NAME),
                        COLUMN_NAME = row.Get(keyColumnUsage.COLUMN_NAME),
                        Constraint_Name = row.Get(tableConstraints.Constraint_name)
                    }
                )
                .From(tableConstraints)
                .Join(keyColumnUsage).On(
                    tableConstraints.Table_schema == keyColumnUsage.TABLE_SCHEMA &
                    tableConstraints.Table_name == keyColumnUsage.TABLE_NAME &
                    tableConstraints.Constraint_name == keyColumnUsage.CONSTRAINT_NAME
                )
                .Where(tableConstraints.Constraint_type == "PRIMARY KEY" & keyColumnUsage.ORDINAL_POSITION.IsNotNull)
                .OrderBy(keyColumnUsage.ORDINAL_POSITION)
                .Execute(database);

            foreach(var row in result.Rows) {

                DatabaseTable table = dbTableLookup[new TableKey(row.TABLE_SCHEMA, row.TABLE_NAME)];

                if(table.PrimaryKey == null) {
                    table.PrimaryKey = new DatabasePrimaryKey(constraintName: row.Constraint_Name);
                }
                table.PrimaryKey.ColumnNames.Add(row.COLUMN_NAME.Value);

                foreach(DatabaseColumn dbColumn in table.Columns) {

                    if(string.Compare(dbColumn.ColumnName.Value, row.COLUMN_NAME.Value, ignoreCase: true) == 0) {
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

            ConstraintColumnUsageTable ccuTable = ConstraintColumnUsageTable.Instance;
            ReferentialConstraintsTable rcuTable = ReferentialConstraintsTable.Instance;
            KeyColumnUsageTable kcuTable = KeyColumnUsageTable.Instance;

            var result = Query
                .Select(
                    result => new {
                        TABLE_SCHEMA = result.Get(ccuTable.TABLE_SCHEMA),
                        FOREIGN_KEY_TABLE_NAME = result.Get(ccuTable.TABLE_NAME),
                        FOREIGN_KEY_COLUMN_NAME = result.Get(ccuTable.COLUMN_NAME),
                        CONSTRAINT_SCHEMA = result.Get(kcuTable.CONSTRAINT_SCHEMA),
                        CONSTRAINT_NAME = result.Get(rcuTable.CONSTRAINT_NAME),
                        PRIMARY_KEY_TABLE_NAME = result.Get(kcuTable.TABLE_NAME),
                        PRIMARY_KEY_COLUMN_NAME = result.Get(kcuTable.COLUMN_NAME)
                    }
                )
                .From(ccuTable)
                .Join(rcuTable).On(ccuTable.CONSTRAINT_CATALOG == rcuTable.CONSTRAINT_CATALOG & ccuTable.CONSTRAINT_SCHEMA == rcuTable.CONSTRAINT_SCHEMA & ccuTable.CONSTRAINT_NAME == rcuTable.CONSTRAINT_NAME)
                .Join(kcuTable).On(rcuTable.CONSTRAINT_CATALOG == kcuTable.CONSTRAINT_CATALOG & rcuTable.CONSTRAINT_SCHEMA == kcuTable.CONSTRAINT_SCHEMA & rcuTable.UNIQUE_CONSTRAINT_NAME == kcuTable.CONSTRAINT_NAME)
                .OrderBy(kcuTable.ORDINAL_POSITION)
                .Execute(database);

            /*
            SELECT _A.table_schema,_A.table_name,_A.column_name,_9.constraint_schema,_B.CONSTRAINT_NAME,_9.table_name
            FROM information_schema.constraint_column_usage AS _A
            JOIN information_schema.REFERENTIAL_CONSTRAINTS AS _B ON ((_A.constraint_catalog = _B.CONSTRAINT_CATALOG AND _A.constraint_schema = _B.CONSTRAINT_SCHEMA) AND _A.constraint_name = _B.CONSTRAINT_NAME)
            JOIN information_schema.key_column_usage AS _9 ON ((_B.CONSTRAINT_CATALOG = _9.constraint_catalog AND _B.CONSTRAINT_SCHEMA = _9.constraint_schema) AND _B.UNIQUE_CONSTRAINT_NAME = _9.constraint_name)
            ORDER BY _9.ordinal_position
            */

            Dictionary<ForeignK, DatabaseForeignKey> dbForeignKeyLookup = new Dictionary<ForeignK, DatabaseForeignKey>();

            for(int index = 0; index < result.Rows.Count; index++) {

                var row = result.Rows[index];

                DatabaseTable foreignKeyTable = tableLookup[new TableKey(row.TABLE_SCHEMA, row.FOREIGN_KEY_TABLE_NAME)];

                ForeignK fk = new ForeignK(row.TABLE_SCHEMA, row.FOREIGN_KEY_TABLE_NAME, row.CONSTRAINT_NAME);

                if(!dbForeignKeyLookup.TryGetValue(fk, out DatabaseForeignKey? foreignKey)) {

                    foreignKey = new DatabaseForeignKey(row.CONSTRAINT_NAME, foreignKeyTable);

                    foreignKeyTable.ForeignKeys.Add(foreignKey);
                    dbForeignKeyLookup.Add(fk, foreignKey);
                }

                TableColumnKey foreignKeyColumnKey = new TableColumnKey(row.TABLE_SCHEMA, row.FOREIGN_KEY_TABLE_NAME, row.FOREIGN_KEY_COLUMN_NAME);
                DatabaseColumn foreignKeyColumn = columnLookup[foreignKeyColumnKey];

                TableColumnKey referencedColumnKey = new TableColumnKey(row.TABLE_SCHEMA, row.PRIMARY_KEY_TABLE_NAME, row.PRIMARY_KEY_COLUMN_NAME);
                DatabaseColumn primaryKeyColumn = columnLookup[referencedColumnKey];

                foreignKey.References.Add(new DatabaseForeignKeyReference(foreignKeyColumn: foreignKeyColumn, primaryKeyColumn: primaryKeyColumn));
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

        private sealed class ForeignK {

            private readonly StringKey<ISchemaName> SchemaName;
            private readonly StringKey<ITableName> TableName;
            private readonly string ConstraintName;

            public ForeignK(StringKey<ISchemaName> schemaName, StringKey<ITableName> tableName, string constraintName) {
                SchemaName = schemaName;
                TableName = tableName;
                ConstraintName = constraintName;
            }

            public override bool Equals(object? obj) {

                if(obj is ForeignK key) {
                    return SchemaName.Value == key.SchemaName.Value && TableName.Value == key.TableName.Value && key.ConstraintName == ConstraintName;
                }
                else {
                    return false;
                }
            }
            public override int GetHashCode() {
                return (SchemaName.Value + "^" + TableName.Value).GetHashCode();
            }
        }
    }

    public static class SqlServerTypes {

        private static readonly Dictionary<string, Type?> _Lookup = new Dictionary<string, Type?>();

        static SqlServerTypes() {

            _Lookup.Add("int", typeof(int));
            _Lookup.Add("smallint", typeof(short));
            _Lookup.Add("bigint", typeof(long));
            _Lookup.Add("uniqueidentifier", typeof(Guid));
            _Lookup.Add("nvarchar", typeof(string));
            _Lookup.Add("varchar", typeof(string));
            _Lookup.Add("nchar", typeof(string));
            _Lookup.Add("decimal", typeof(decimal));
            _Lookup.Add("money", typeof(decimal));
            _Lookup.Add("real", typeof(float));
            _Lookup.Add("float", typeof(double));
            _Lookup.Add("tinyint", typeof(bool));
            _Lookup.Add("bit", typeof(bool));
            _Lookup.Add("varbinary", typeof(byte[]));
            _Lookup.Add("datetime", typeof(DateTime));
            _Lookup.Add("smalldatetime", typeof(DateTime));            
            _Lookup.Add("datetimeoffset", typeof(DateTimeOffset));
            _Lookup.Add("date", typeof(DateOnly));
            _Lookup.Add("time", typeof(TimeOnly));
        }
        public static Type? GetDotNetType(string typeName) {

            if(!_Lookup.TryGetValue(typeName, out Type? dotNetType)) {
                return null;
            }
            return dotNetType;
        }
    }
}