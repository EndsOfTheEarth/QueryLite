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
using System.Collections.Generic;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class TableCodeGenerator {

        public static CodeBuilder Generate(List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            CodeBuilder code = new CodeBuilder();

            code.EndLine().Append($"namespace {settings.Namespaces.TableNamespace} {{").EndLine().EndLine();

            code.Indent(1).Append("using System;").EndLine();
            code.Indent(1).Append("using QueryLite;").EndLine();

            code.EndLine();

            foreach(DatabaseTable table in tables) {
                string tableName = CodeHelper.GetTableName(table, includePostFix: false);
                code.Indent(1).Append($"public interface I{tableName} {{}}").EndLine();
            }

            foreach(DatabaseTable table in tables) {
                TablePrefix prefix = new TablePrefix(table);
                code.Append(Generate(table, prefix, settings, includeUsings: false).ToString());
            }
            code.Append("}");
            return code;
        }

        public static CodeBuilder Generate(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings, bool includeUsings) {

            CodeBuilder code = new CodeBuilder();

            if(includeUsings) {

                code.Append($"namespace {settings.Namespaces.TableNamespace} {{").EndLine().EndLine();

                code.Indent(1).Append("using System;").EndLine();
                code.Indent(1).Append("using QueryLite;").EndLine();

                code.EndLine();
            }

            string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

            code.EndLine();

            if(settings.IncludeDescriptions) {
                code.Indent(1).Append("[Description(\"\")]").EndLine();
            }
            code.Indent(1).Append($"public sealed class {tableClassName} : ATable {{").EndLine().EndLine();

            code.Indent(2).Append($"public static readonly {tableClassName} Instance = new {tableClassName}();").EndLine();
            code.Indent(2).Append($"public static readonly {tableClassName} Instance2 = new {tableClassName}();").EndLine();
            code.Indent(2).Append($"public static readonly {tableClassName} Instance3 = new {tableClassName}();").EndLine();

            List<string> lines = new List<string>();

            code.EndLine();

            int count = 0;

            foreach(DatabaseColumn column in table.Columns) {

                CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, out string columnTypeName, out bool isKeyColumn);

                string columnClass = !column.IsNullable ? "Column" : "NullableColumn";

                string columnName = prefix.GetColumnName(column.ColumnName.Value);

                if(count > 0 && (settings.IncludeDescriptions || settings.IncludeKeyAttributes)) {
                    code.EndLine();
                }

                if(settings.IncludeDescriptions) {    
                    code.Indent(2).Append("[Description(\"\")]").EndLine();
                }

                if(column.IsPrimaryKey && settings.IncludeKeyAttributes) {
                    code.Indent(2).Append($"[PrimaryKey(\"{column.PrimaryKeyConstraintName}\")]").EndLine();
                }

                if(column.IsForeignKey && settings.IncludeKeyAttributes) {
                    string pkTableTypeName = CodeHelper.GetTableName(column.ForeignKeyTable!, includePostFix: true);
                    code.Indent(2).Append($"[ForeignKey<{pkTableTypeName}>(\"{column.ForeignKeyConstraintName}\")]").EndLine();
                }
                count++;
                code.Indent(2).Append($"public {columnClass}<{columnTypeName}> {columnName} {{ get; }}").EndLine();

                string columnLengthParameter = string.Empty;

                if(column.Length != null) {
                    columnLengthParameter = ", length: " + (column.Length != -1 ? column.Length.Value.ToString() : "int.MaxValue");
                }
                else if(dotNetType == typeof(byte[])) {
                    columnLengthParameter = ", length: int.MaxValue";
                }
                //dotNetType
                lines.Add($"{columnName} = new {columnClass}<{columnTypeName}>(this, columnName: \"{column.ColumnName.Value}\"{(column.IsPrimaryKey ? ", isPrimaryKey: true" : "")}{(column.IsAutoGenerated ? ", isAutoGenerated: true" : "")}{columnLengthParameter});");
            }

            code.EndLine();

            string encloseTableName = SqlKeyWordLookup.IsKeyWord(table.TableName.Value) ? ", enclose: true" : "";

            code.Indent(2).Append($"private {tableClassName}() : base(tableName:\"{table.TableName.Value}\", schemaName: \"{table.Schema}\"{encloseTableName}) {{").EndLine().EndLine(); 

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