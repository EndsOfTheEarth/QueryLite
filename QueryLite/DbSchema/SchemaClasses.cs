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
using QueryLite.Utility;
using System;
using System.Collections.Generic;

namespace QueryLite.DbSchema {

    public sealed class DatabaseSchema {

        public List<DatabaseTable> Tables { get; }

        public DatabaseSchema(List<DatabaseTable> tables) {
            Tables = tables;
        }
    }

    public sealed class DatabaseTable {

        public StringKey<ISchemaName> Schema { get; }
        public StringKey<ITableName> TableName { get; }
        public bool IsView { get; }
        public string Description { get; set; } = string.Empty;
        public List<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        public DatabasePrimaryKey? PrimaryKey { get; set; }
        public List<DatabaseUniqueConstraint> UniqueConstraints { get; set; } = new List<DatabaseUniqueConstraint>();
        public List<DatabaseForeignKey> ForeignKeys { get; } = new List<DatabaseForeignKey>();

        public DatabaseTable(StringKey<ISchemaName> schema, StringKey<ITableName> tableName, bool isView) {
            Schema = schema;
            TableName = tableName;
            IsView = isView;
        }

        public override string ToString() {
            return Schema.ToString() + "." + TableName.ToString();
        }
    }

    public sealed class DatabaseUniqueConstraint {

        public DatabaseUniqueConstraint(string constraintName) {
            ConstraintName = constraintName;
        }
        public string ConstraintName { get; }
        public List<StringKey<IColumnName>> ColumnNames { get; } = new List<StringKey<IColumnName>>();
    }

    public sealed class DatabasePrimaryKey {

        public DatabasePrimaryKey(string constraintName) {
            ConstraintName = constraintName;
        }
        public string ConstraintName { get; }
        public List<string> ColumnNames { get; } = new List<string>();
    }

    public sealed class DatabaseColumn {

        public DatabaseTable Table { get; }
        public StringKey<IColumnName> ColumnName { get; }
        public DataType DataType { get; }
        public string SqlDataTypeName { get; }
        public bool IsPrimaryKey { get; internal set; }
        public int? Length { get; }
        public bool IsNullable { get; }
        public string Description { get; set; } = string.Empty;

        public bool IsAutoGenerated { get; }

        public DatabaseColumn(DatabaseTable table, StringKey<IColumnName> columnName, DataType dataType, string sqlDataTypeName, int? length, bool isNullable, bool isAutoGenerated) {
            Table = table;
            ColumnName = columnName;
            SqlDataTypeName = sqlDataTypeName;
            DataType = dataType;
            Length = length;
            IsNullable = isNullable;
            IsAutoGenerated = isAutoGenerated;
        }
        public override string ToString() {
            return Table.TableName.ToString() + "." + ColumnName + $" ({(DataType != null ? DataType.ToString() : "?")})";
        }
    }

    public sealed class DatabaseForeignKey {

        public string ConstraintName { get; }
        public DatabaseTable ForeignKeyTable { get; }
        public List<DatabaseForeignKeyReference> References { get; } = new List<DatabaseForeignKeyReference>();

        public DatabaseForeignKey(string constraintName, DatabaseTable foreignKeyTable) {
            ConstraintName = constraintName;
            ForeignKeyTable = foreignKeyTable;
        }
    }

    public sealed class DatabaseForeignKeyReference {

        public DatabaseForeignKeyReference(DatabaseColumn foreignKeyColumn, DatabaseColumn primaryKeyColumn) {
            ForeignKeyColumn = foreignKeyColumn;
            PrimaryKeyColumn = primaryKeyColumn;
        }

        public DatabaseColumn ForeignKeyColumn { get; }
        public DatabaseColumn PrimaryKeyColumn { get; }
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