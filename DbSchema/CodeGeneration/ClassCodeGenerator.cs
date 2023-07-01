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
using System.Linq;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class ClassCodeGenerator {

        public static CodeBuilder Generate(IDatabase database, List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            CodeBuilder code = new CodeBuilder();

            code.Append("using System;").EndLine();
            code.Append("using QueryLite;").EndLine();

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
                        code.Append(GenerateClassCode(database, table, prefix, settings, includeUsings: false).ToString());
                    }
                }
                code.Append("}");
            }
            return code;
        }

        public static CodeBuilder GenerateClassCode(IDatabase database, DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings, bool includeUsings) {

            CodeBuilder classCode = new CodeBuilder();

            if(includeUsings) {
                classCode.Append($"namespace {settings.Namespaces.GetClassesNamespace(table.Schema)} {{").EndLine().EndLine();
                classCode.Indent(1).Append("using System;").EndLine();
                classCode.Indent(1).Append("using QueryLite;").EndLine();

                classCode.Indent(1).Append($"using {settings.Namespaces.TableNamespace};").EndLine();
            

            string tableNamespace = settings.Namespaces.GetTableNamespace(table.Schema);

            if(settings.Namespaces.TableNamespace != tableNamespace) {  //If this table exist in a non default schema it will have a different namespace
                classCode.Indent(1).Append($"using {tableNamespace};").EndLine();
            }
            }

            if(settings.IncludeMessagePackAttributes) {
                classCode.Indent(1).Append("using MessagePack;").EndLine();
            }
            if(settings.IncludeJsonAttributes) {
                classCode.Indent(1).Append("using System.Text.Json.Serialization;").EndLine();
            }

            classCode.EndLine();

            string className = CodeHelper.GetTableName(table, includePostFix: false);

            if(table.IsView) {
                className += "_View";
            }

            if(settings.IncludeMessagePackAttributes) {
                classCode.Indent(1).Append("[MessagePackObject]").EndLine();
            }

            classCode.Indent(1).Append($"public sealed class {className} {{").EndLine().EndLine();

            classCode.Indent(2).Append("public ").Append(className).Append("(");

            CodeBuilder constructor = new CodeBuilder();

            int count = 0;
            foreach(DatabaseColumn column in table.Columns) {

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IGeography))) {
                    continue;
                }

                if(count > 0) {
                    classCode.Append(", ");
                }
                count++;

                CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string columnTypeName, out bool isKeyColumn);

                string nullable = column.IsNullable ? "?" : "";
                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: className);

                string paramName = columnName.First().ToString().ToLower() + columnName.Substring(1);

                if(columnName == paramName || CodeHelper.IsCSharpKeyword(paramName)) {
                    paramName = "_" + paramName;
                }

                classCode.Append($"{columnTypeName}{nullable} {paramName}");

                constructor.Indent(3).Append($"{columnName} = {paramName};").EndLine();
            }

            classCode.Append(") {").EndLine();
            classCode.Append(constructor.ToString());
            classCode.Indent(2).Append("}").EndLine();
            classCode.EndLine();

            string tableOrViewClassName = CodeHelper.GetTableName(table, includePostFix: true);

            string tableOrViewParamName = table.IsView ? "view" : "table";

            classCode.Indent(2).Append("public ").Append(className).Append("(").Append(tableOrViewClassName).Append(" ").Append(tableOrViewParamName).Append(", IResultRow row) {").EndLine();

            CodeBuilder constructor2 = new CodeBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IGeography))) {
                    continue;
                }
                
                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: className);
                
                string columnNameForTable = prefix.GetColumnName(column.ColumnName.Value, className: tableOrViewClassName);

                constructor2.Indent(3).Append($"{columnName} = row.Get({tableOrViewParamName}.{columnNameForTable});").EndLine();
            }
            classCode.Append(constructor2.ToString());
            classCode.Indent(2).Append("}").EndLine();

            count = 0;

            foreach(DatabaseColumn column in table.Columns) {

                if(column.DataType.DotNetType.IsAssignableTo(typeof(IGeography))) {
                    continue;
                }

                classCode.EndLine();

                CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string columnTypeName, out bool isKeyColumn);

                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: className);

                if(settings.IncludeMessagePackAttributes || settings.IncludeJsonAttributes) {
                    classCode.EndLine();
                }
                if(settings.IncludeMessagePackAttributes) {
                    classCode.Indent(2).Append($"[Key({count})]").EndLine();
                }

                if(settings.IncludeJsonAttributes) {
                    classCode.Indent(2).Append($"[JsonPropertyName(\"{columnName}\")]").EndLine();
                }

                classCode.Indent(2).Append($"public {columnTypeName}{(column.IsNullable ? "?" : "")} {columnName} {{ get; set; }}");

                if(!column.IsNullable && (!dotNetType.IsPrimitive) && !isKeyColumn) {

                    string? propSet = database.GetCSharpCodeSet(dotNetType);

                    if(propSet != null) {
                        classCode.Append(" = ").Append(propSet).Append(";");
                    }
                }
                count++;
            }
            classCode.EndLine();
            classCode.Indent(1).Append("}").EndLine();

            if(includeUsings) {
                classCode.Append("}");
            }
            return classCode;
        }
    }
}