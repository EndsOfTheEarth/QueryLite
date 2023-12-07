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
using System;
using System.Collections.Generic;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class TableCodeGenerator {

        public static CodeBuilder Generate(List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            CodeBuilder code = new CodeBuilder();

            code.Append("using System;").EndLine();
            code.Append("using QueryLite;").EndLine();

            bool skipInterfaces = tables.Count == 1 && tables[0].IsView;

            if(!skipInterfaces) {

                code.EndLine().Append($"namespace {settings.Namespaces.TableNamespace} {{").EndLine().EndLine();

                foreach(DatabaseTable table in tables) {

                    if(!table.IsView) {
                        string tableName = CodeHelper.GetTableName(table, includePostFix: false);
                        code.Indent(1).Append($"public interface I{tableName}Id {{}}").EndLine();
                    }
                }

                code.Append("}").EndLine();
            }

            List<StringKey<ISchemaName>> schemaNames = new List<StringKey<ISchemaName>>();

            foreach(DatabaseTable table in tables) {

                if(!schemaNames.Contains(table.Schema)) {
                    schemaNames.Add(table.Schema);
                }
            }

            schemaNames.Sort((a, b) => a.Value.CompareTo(b.Value));

            int count = 0;

            foreach(StringKey<ISchemaName> schema in schemaNames) {

                if(count > 0) {
                    code.EndLine();
                }
                count++;
                code.EndLine().Append($"namespace {settings.Namespaces.GetTableNamespace(schema)} {{").EndLine();

                foreach(DatabaseTable table in tables) {

                    if(string.Equals(table.Schema.Value, schema.Value, StringComparison.OrdinalIgnoreCase)) {
                        TablePrefix prefix = new TablePrefix(table);
                        code.Append(Generate(table, prefix, settings, includeUsings: false, generateKeyInterface: false).ToString());
                    }
                }
                code.Append("}");
            }
            return code;
        }

        public static CodeBuilder Generate(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings, bool includeUsings, bool generateKeyInterface) {

            CodeBuilder code = new CodeBuilder();

            if(includeUsings) {

                code.Append($"namespace {settings.Namespaces.GetTableNamespace(table.Schema)} {{").EndLine().EndLine();

                code.Indent(1).Append("using System;").EndLine();
                code.Indent(1).Append("using QueryLite;").EndLine();

                code.EndLine();
            }

            if(generateKeyInterface) {
                string tableName = CodeHelper.GetTableName(table, includePostFix: false);
                code.Indent(1).Append($"public interface I{tableName}Id {{}}").EndLine();
            }

            string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

            code.EndLine();

            if(settings.IncludeDescriptions) {
                code.Indent(1).Append("[Description(\"").Append(CodeHelper.EscapeCSharpString(table.Description)).Append("\")]").EndLine();
            }
            code.Indent(1).Append($"public sealed class {tableClassName} : ATable {{").EndLine().EndLine();

            code.Indent(2).Append($"public static readonly {tableClassName} Instance = new {tableClassName}();").EndLine();

            for(int index = 1; index < settings.NumberOfInstanceProperties; index++) {
                code.Indent(2).Append($"public static readonly {tableClassName} Instance{index + 1} = new {tableClassName}();").EndLine();
            }

            List<string> lines = new List<string>();

            code.EndLine();

            int count = 0;

            foreach(DatabaseColumn column in table.Columns) {

                CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, out string columnTypeName, out bool isKeyColumn);

                string columnClass = !column.IsNullable ? "Column" : "NullableColumn";

                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                if(count > 0 && settings.IncludeDescriptions) {
                    code.EndLine();
                }

                if(settings.IncludeDescriptions) {
                    code.Indent(2).Append("[Description(\"").Append(CodeHelper.EscapeCSharpString(column.Description)).Append("\")]").EndLine();
                }

                bool addSuppressAttribute = false;

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IUnsupportedType))) {    //Ignore unsupported types
                    addSuppressAttribute = true;
                    code.EndLine();
                    code.Indent(2).Append("[SuppressColumnTypeValidation] --> ***PLEASE_CHECK_UNSUPPORTED_TYPE***").EndLine();
                }
                count++;
                code.Indent(2).Append($"public {columnClass}<{columnTypeName}> {columnName} {{ get; }}").EndLine();

                if(addSuppressAttribute) {
                    code.EndLine();
                }

                string columnLengthParameter = string.Empty;

                if(column.Length != null) {
                    columnLengthParameter = ", length: " + (column.Length != -1 ? column.Length.Value.ToString() : "int.MaxValue");
                }
                else if(dotNetType == typeof(byte[])) {
                    columnLengthParameter = ", length: int.MaxValue";
                }

                string encloseParameter = SqlKeyWordLookup.IsKeyWord(column.ColumnName.Value) ? ", enclose: true" : string.Empty;

                lines.Add($"{columnName} = new {columnClass}<{columnTypeName}>(this, columnName: \"{column.ColumnName.Value}\"{(column.IsAutoGenerated ? ", isAutoGenerated: true" : "")}{columnLengthParameter}{encloseParameter});");
            }

            if(table.PrimaryKey != null && settings.IncludeConstraints) {

                code.EndLine();
                code.Indent(2).Append($"public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: \"{table.PrimaryKey.ConstraintName}\"");

                foreach(string columnName in table.PrimaryKey.ColumnNames) {

                    foreach(DatabaseColumn column in table.Columns) {

                        if(string.Compare(columnName, column.ColumnName.Value, ignoreCase: true) == 0) {
                            code.Append(", ").Append(prefix.GetColumnName(column.ColumnName.Value, className: tableClassName));
                            break;
                        }
                    }
                }
                code.Append(");").EndLine();
            }

            if(table.UniqueConstraints.Count > 0 && settings.IncludeConstraints) {

                code.EndLine();

                code.Indent(2).Append("public override UniqueConstraint[] UniqueConstraints => new UniqueConstraint[] {").EndLine();

                for(int index = 0; index < table.UniqueConstraints.Count; index++) {

                    DatabaseUniqueConstraint uniqueConstraint = table.UniqueConstraints[index];

                    if(index > 0) {
                        code.Append(",").EndLine();
                    }
                    code.Indent(3).Append($"new UniqueConstraint(this, constraintName: \"{uniqueConstraint.ConstraintName}\"");

                    foreach(StringKey<IColumnName> columnName in uniqueConstraint.ColumnNames) {
                        code.Append(", ").Append(prefix.GetColumnName(columnName.Value, className: tableClassName));
                    }
                    code.Append(")");
                }
                code.EndLine();
                code.Indent(2).Append("};").EndLine();
            }

            if(table.ForeignKeys.Count > 0 && settings.IncludeConstraints) {

                code.EndLine();

                code.Indent(2).Append("public override ForeignKey[] ForeignKeys => new ForeignKey[] {").EndLine();

                for(int index = 0; index < table.ForeignKeys.Count; index++) {

                    DatabaseForeignKey foreignKey = table.ForeignKeys[index];

                    if(index > 0) {
                        code.Append(",").EndLine();
                    }
                    code.Indent(3).Append($"new ForeignKey(this, constraintName: \"{foreignKey.ConstraintName}\")");

                    foreach(DatabaseForeignKeyReference reference in foreignKey.References) {

                        string foreignKeyColumnName = prefix.GetColumnName(reference.ForeignKeyColumn.ColumnName.Value, className: tableClassName);

                        TablePrefix primaryKeyTablePrefix = new TablePrefix(reference.PrimaryKeyColumn.Table);

                        string primaryKeyTable = CodeHelper.GetTableName(reference.PrimaryKeyColumn.Table, includePostFix: true);
                        string primaryKeyColumnName = primaryKeyTablePrefix.GetColumnName(reference.PrimaryKeyColumn.ColumnName.Value, className: null);

                        string primaryKeyTableSchemaName = !Namespaces.IsDefaultSchema(reference.PrimaryKeyColumn.Table.Schema) ? $"{reference.PrimaryKeyColumn.Table.Schema.Value}." : string.Empty;

                        code.Append($".References({foreignKeyColumnName}, {primaryKeyTableSchemaName}{primaryKeyTable}.Instance.{primaryKeyColumnName})");
                    }
                }
                code.EndLine();
                code.Indent(2).Append("};").EndLine();
            }

            code.EndLine();

            string encloseTableName = SqlKeyWordLookup.IsKeyWord(table.TableName.Value) ? ", enclose: true" : "";

            string isViewCode = table.IsView ? ", isView: true" : string.Empty;

            code.Indent(2).Append($"private {tableClassName}() : base(tableName:\"{table.TableName.Value}\", schemaName: \"{table.Schema}\"{encloseTableName}{isViewCode}) {{").EndLine().EndLine();

            foreach(string line in lines) {
                code.Indent(3).Append(line).EndLine();
            }
            code.Indent(2).Append("}").EndLine();
            code.Indent(1).Append("}").EndLine();

            if(includeUsings) {
                code.Append("}");
            }
            return code;
        }
    }
}