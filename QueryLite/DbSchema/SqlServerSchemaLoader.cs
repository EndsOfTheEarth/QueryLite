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
using QueryLite.DbSchema.Tables;
using QueryLite.DbSchema.Tables.SqlServer;
using System;
using System.Collections.Generic;

namespace QueryLite.DbSchema {

    using TableColumnKey = Key<StringKey<ISchemaName>, StringKey<ITableName>, StringKey<IColumnName>>;
    using TableKey = Key<StringKey<ISchemaName>, StringKey<ITableName>>;

    public sealed class SqlServerSchemaLoader {

        public sealed class COLUMNPROPERTY : NullableFunction<int> {

            private ColumnsTable Table { get; }

            public COLUMNPROPERTY(ColumnsTable table) : base("COLUMNPROPERTY") {
                Table = table;
            }
            public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
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
            Dictionary<TableKey, DatabaseTable> tableLookup = new Dictionary<TableKey, DatabaseTable>();
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

                TableKey tableKey = new TableKey(row.TABLE_SCHEMA, row.TABLE_NAME);

                if(!tableLookup.TryGetValue(tableKey, out DatabaseTable? databaseTable)) {
                    databaseTable = new DatabaseTable(schema: row.TABLE_SCHEMA, tableName: row.TABLE_NAME, isView: row.TABLE_TYPE == "VIEW");
                    tableList.Add(databaseTable);
                    tableLookup.Add(tableKey, databaseTable);
                }

                ColumnsRow columnRow = row.Column;

                if(!typelookup.TryGetValue(columnRow.Data_type, out Type? dotNetType)) {
                    dotNetType = SqlServerTypes.GetDotNetType(columnRow.Data_type);
                    typelookup.Add(columnRow.Data_type, dotNetType);
                }

                DataType dataType = new DataType(name: columnRow.Data_type, dotNetType: (dotNetType ?? typeof(IUnknownType)));

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

                //If the column is populated by a sequence
                if(columnRow.Column_default != null && columnRow.Column_default.Contains("NEXT VALUE FOR", StringComparison.OrdinalIgnoreCase)) {
                    isAutoGenerated = true;
                }

                DatabaseColumn databaseColumn = new DatabaseColumn(
                    table: databaseTable,
                    columnName: columnRow.Column_name,
                    dataType: dataType,
                    sqlDataTypeName: columnRow.Data_type,
                    length: length,
                    isNullable: isNullable,
                    isAutoGenerated: isAutoGenerated
                );
                databaseTable!.Columns.Add(databaseColumn);
            }

            SetPrimaryKeys(tableList, database);
            SetForeignKeys(tableList, database);

            LoadCommentMetaData(tableList, database);

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

            ReferentialConstraintsTable rcTable = ReferentialConstraintsTable.Instance;
            KeyColumnUsageTable foreignKcuTable = KeyColumnUsageTable.Instance;
            KeyColumnUsageTable uniqueKcuTable = KeyColumnUsageTable.Instance2;

            var result = Query
                .Select(
                    row => new {
                        FOREIGN_KEY_TABLE_SCHEMA = row.Get(foreignKcuTable.TABLE_SCHEMA),
                        FOREIGN_KEY_TABLE_NAME = row.Get(foreignKcuTable.TABLE_NAME),
                        FOREIGN_KEY_CONSTRAINT_NAME = row.Get(rcTable.CONSTRAINT_NAME),
                        FOREIGN_KEY_COLUMN_NAME = row.Get(foreignKcuTable.COLUMN_NAME),
                        UNIQUE_KEY_TABLE_SCHEMA = row.Get(uniqueKcuTable.TABLE_SCHEMA),
                        UNIQUE_KEY_TABLE_NAME = row.Get(uniqueKcuTable.TABLE_NAME),
                        UNIQUE_KEY_COLUMN_NAME = row.Get(uniqueKcuTable.COLUMN_NAME)
                    }
                )
                .From(rcTable)
                .Join(foreignKcuTable).On(
                    rcTable.CONSTRAINT_CATALOG == foreignKcuTable.CONSTRAINT_CATALOG &
                    rcTable.CONSTRAINT_SCHEMA == foreignKcuTable.CONSTRAINT_SCHEMA &
                    rcTable.CONSTRAINT_NAME == foreignKcuTable.CONSTRAINT_NAME
                )
                .Join(uniqueKcuTable).On(
                    rcTable.UNIQUE_CONSTRAINT_CATALOG == uniqueKcuTable.CONSTRAINT_CATALOG &
                    rcTable.UNIQUE_CONSTRAINT_SCHEMA == uniqueKcuTable.CONSTRAINT_SCHEMA &
                    rcTable.UNIQUE_CONSTRAINT_NAME == uniqueKcuTable.CONSTRAINT_NAME &
                    foreignKcuTable.ORDINAL_POSITION == uniqueKcuTable.ORDINAL_POSITION
                )
                .OrderBy(rcTable.CONSTRAINT_NAME, foreignKcuTable.ORDINAL_POSITION)
                .Execute(database);

            Dictionary<ForeignK, DatabaseForeignKey> dbForeignKeyLookup = new Dictionary<ForeignK, DatabaseForeignKey>();

            foreach(var row in result.Rows) {

                ForeignK fk = new ForeignK(row.FOREIGN_KEY_TABLE_SCHEMA, row.FOREIGN_KEY_TABLE_NAME, row.FOREIGN_KEY_CONSTRAINT_NAME);

                if(!dbForeignKeyLookup.TryGetValue(fk, out DatabaseForeignKey? foreignKey)) {

                    DatabaseTable foreignKeyTable = tableLookup[new TableKey(row.FOREIGN_KEY_TABLE_SCHEMA, row.FOREIGN_KEY_TABLE_NAME)];

                    foreignKey = new DatabaseForeignKey(row.FOREIGN_KEY_CONSTRAINT_NAME, foreignKeyTable);

                    foreignKeyTable.ForeignKeys.Add(foreignKey);
                    dbForeignKeyLookup.Add(fk, foreignKey);
                }

                TableColumnKey foreignKeyColumnKey = new TableColumnKey(row.FOREIGN_KEY_TABLE_SCHEMA, row.FOREIGN_KEY_TABLE_NAME, row.FOREIGN_KEY_COLUMN_NAME);
                DatabaseColumn foreignKeyColumn = columnLookup[foreignKeyColumnKey];

                TableColumnKey referencedColumnKey = new TableColumnKey(row.UNIQUE_KEY_TABLE_SCHEMA, row.UNIQUE_KEY_TABLE_NAME, row.UNIQUE_KEY_COLUMN_NAME);
                DatabaseColumn primaryKeyColumn = columnLookup[referencedColumnKey];

                foreignKey.References.Add(new DatabaseForeignKeyReference(foreignKeyColumn: foreignKeyColumn, primaryKeyColumn: primaryKeyColumn));
            }
        }

        private static void LoadCommentMetaData(List<DatabaseTable> tableList, IDatabase database) {

            Dictionary<TableKey, DatabaseTable> tableLookup = new Dictionary<TableKey, DatabaseTable>();
            Dictionary<TableColumnKey, DatabaseColumn> columnLookup = new Dictionary<TableColumnKey, DatabaseColumn>();

            foreach(DatabaseTable table in tableList) {

                tableLookup.Add(new TableKey(table.Schema, table.TableName), table);

                foreach(DatabaseColumn column in table.Columns) {

                    columnLookup.Add(new TableColumnKey(table.Schema, table.TableName, column.ColumnName), column);
                }
            }

            ExtendedPropertiesView extPropView = ExtendedPropertiesView.Instance;
            TablesView tablesView = TablesView.Instance;
            ColumnsView columnsView = ColumnsView.Instance;
            SchemasView schemasView = SchemasView.Instance;

            var result = Query
                .Select(
                    rows => new {
                        SchemaName = rows.Get(schemasView.Schema_Name),
                        TableName = rows.Get(tablesView.Table_Name),
                        ColumnName = rows.Get(columnsView.Column_Name),
                        Value = rows.Get(extPropView.Value)
                    }
                )
                .From(extPropView)
                .Join(tablesView).On(extPropView.Major_Id == tablesView.Object_Id)
                .LeftJoin(columnsView).On(extPropView.Major_Id == columnsView.Object_Id & extPropView.Minor_Id == columnsView.Column_Id)
                .Join(schemasView).On(tablesView.Schema_Id == schemasView.Schema_Id)
                .Where(extPropView.Class == 1 & extPropView.Name == "MS_Description")
                .Execute(database);

            foreach(var row in result.Rows) {

                if(string.IsNullOrEmpty(row.ColumnName.Value)) {

                    TableKey tableKey = new TableKey(row.SchemaName, row.TableName);

                    if(tableLookup.TryGetValue(tableKey, out DatabaseTable? dbTable)) {

                        dbTable.Description = row.Value ?? string.Empty;
                    }
                }
                else {

                    TableColumnKey columnKey = new TableColumnKey(row.SchemaName, row.TableName, row.ColumnName);

                    if(columnLookup.TryGetValue(columnKey, out DatabaseColumn? dbColumn)) {

                        dbColumn.Description = row.Value ?? string.Empty;
                    }
                }
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

        private static readonly Dictionary<string, Type?> _Lookup = new Dictionary<string, Type?>(StringComparer.OrdinalIgnoreCase);

        static SqlServerTypes() {

            _Lookup.Add("int", typeof(int));
            _Lookup.Add("smallint", typeof(short));
            _Lookup.Add("bigint", typeof(long));
            _Lookup.Add("uniqueidentifier", typeof(Guid));
            _Lookup.Add("nvarchar", typeof(string));
            _Lookup.Add("varchar", typeof(string));
            _Lookup.Add("xml", typeof(string));
            _Lookup.Add("nchar", typeof(string));
            _Lookup.Add("decimal", typeof(decimal));
            _Lookup.Add("numeric", typeof(decimal));
            _Lookup.Add("money", typeof(decimal));
            _Lookup.Add("smallmoney", typeof(decimal));
            _Lookup.Add("real", typeof(float));
            _Lookup.Add("float", typeof(double));
            _Lookup.Add("tinyint", typeof(bool));
            _Lookup.Add("bit", typeof(Bit));
            _Lookup.Add("varbinary", typeof(byte[]));
            _Lookup.Add("datetime", typeof(DateTime));
            _Lookup.Add("datetime2", typeof(DateTime));
            _Lookup.Add("smalldatetime", typeof(DateTime));
            _Lookup.Add("datetimeoffset", typeof(DateTimeOffset));
            _Lookup.Add("date", typeof(DateOnly));
            _Lookup.Add("time", typeof(TimeOnly));
            _Lookup.Add("geography", typeof(IGeography_UnsupportedType));
        }
        public static Type? GetDotNetType(string typeName) {

            if(!_Lookup.TryGetValue(typeName, out Type? dotNetType)) {
                return null;
            }
            return dotNetType;
        }
    }
}