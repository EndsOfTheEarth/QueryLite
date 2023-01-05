﻿using QueryLite.DbSchema.Tables;
using System;
using System.Collections.Generic;

namespace QueryLite.DbSchema {

    public sealed class Schema {

        public List<DatabaseTable> Tables { get; }

        public Schema(List<DatabaseTable> tables) {
            Tables = tables;
        }
    }

    public sealed class DatabaseTable {

        public StringKey<ISchemaName> Schema { get; }
        public StringKey<ITableName> TableName { get; }
        public bool IsView { get; }
        public string Description { get; }  //TODO: Populate
        public List<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        public DatabaseTable(StringKey<ISchemaName> schema, StringKey<ITableName> tableName, bool isView, string description) {
            Schema = schema;
            TableName = tableName;
            IsView = isView;
            Description = description;
        }

        public override string ToString() {
            return Schema.ToString() + "." + TableName.ToString();
        }
    }

    public sealed class DatabaseColumn {

        public DatabaseTable Table { get; }
        public StringKey<IColumnName> ColumnName { get; }
        public DataType DataType { get; }
        public bool IsPrimaryKey { get; internal set; }
        public string PrimaryKeyConstraintName { get; internal set; } = string.Empty;
        public int? Length { get; }
        public bool IsNullable { get; }

        public bool IsAutoGenerated { get; }
        public string Description { get; } = string.Empty;  //TODO: Populate
        public bool IsForeignKey { get; internal set; }
        public string ForeignKeyConstraintName { get; internal set; } = string.Empty;
        public DatabaseTable? ForeignKeyTable { get; internal set; }

        public DatabaseColumn(DatabaseTable table, StringKey<IColumnName> columnName, DataType dataType, int? length, bool isNullable, bool isAutoGenerated) {
            Table = table;
            ColumnName = columnName;
            DataType = dataType;
            Length = length;
            IsNullable = isNullable;
            IsAutoGenerated = isAutoGenerated;
        }
        public override string ToString() {
            return Table.TableName.ToString() + "." + ColumnName + $" ({(DataType != null ? DataType.ToString() : "?")})";
        }
    }

    public sealed class DataType {

        public string Name { get; }
        public Type DotNetType { get; }

        public DataType(string name, Type dotNetType) {
            Name = name;
            DotNetType = dotNetType;
        }
        public override string ToString() {
            return $"{Name} {{{DotNetType}}}";
        }
    }
}