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
using System.IO;

namespace QueryLite.DbSchema.CodeGeneration {

    public class CodeGeneratorSettings {

        /// <summary>
        /// Include MessagePack attributes on class code properties
        /// </summary>
        public required bool IncludeMessagePackAttributes { get; set; }

        /// <summary>
        /// Include Json attributes on class code properties
        /// </summary>
        public required bool IncludeJsonAttributes { get; set; }

        /// <summary>
        /// Use key structs on primary key and foreign key columns e.g. GuidKey<>, IntKey<> etc.
        /// </summary>
        public required bool UseIdentifiers { get; set; }

        /// <summary>
        /// Include table and column description attributes
        /// </summary>
        public required bool IncludeDescriptions { get; set; }

        /// <summary>
        /// Include primary key, foreign key and unique constraints in table definition
        /// </summary>
        public required bool IncludeConstraints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required int NumberOfInstanceProperties { get; set; }

        public required Namespaces Namespaces { get; set; }
    }

    public sealed class SchemaGenerator {

        public static void GenerateToFiles(string dirPath, IDatabase database, List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            if(Directory.Exists(dirPath)) {
                throw new Exception($"{dirPath} should not exist. This is a safety check to avoid over writing files etc.");
            }

            Directory.CreateDirectory(dirPath);

            string tablesDir = Path.Combine(dirPath, "Tables");
            string logicDir = Path.Combine(dirPath, "Logic");

            Directory.CreateDirectory(tablesDir);
            Directory.CreateDirectory(logicDir);

            foreach(DatabaseTable table in tables) {

                TablePrefix prefix = new TablePrefix(table);

                CodeBuilder code = TableCodeGenerator.Generate(tables: new List<DatabaseTable>(new DatabaseTable[] { table }), settings);

                string tableFilePath = Path.Combine(tablesDir, CodeHelper.GetTableName(table, includePostFix: true) + ".cs");
                File.WriteAllText(tableFilePath, code.ToString());

                CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(database, table, prefix, settings);

                string logicFilePath = Path.Combine(logicDir, CodeHelper.GetTableName(table, includePostFix: false) + ".cs");
                File.WriteAllText(logicFilePath, classCode.ToString());
            }
        }
    }

    public static class CodeHelper {

        public static string ExcapeCSharpString(string text) {
            return !string.IsNullOrEmpty(text) ? text.Replace("\"", "\\\"") : text;
        }

        public static string GetTableName(DatabaseTable table, bool includePostFix) {

            string postFix = !table.IsView ? "Table" : "View";
            string name = table.TableName.Value.Replace(" ", string.Empty) + (includePostFix ? postFix : string.Empty);

            return $"{char.ToUpper(name[0])}{name.Substring(startIndex: 1)}";
        }

        public static void GetColumnName(DatabaseTable table, DatabaseColumn column, bool useIdentifiers, out Type dotNetType, out string columnTypeName, out bool isKeyColumn) {

            string columnType;
            string defaultValue;

            isKeyColumn = false;

            dotNetType = column.DataType.DotNetType;

            if(dotNetType.IsArray && dotNetType.GetElementType() == typeof(byte)) {
                columnType = "byte[]";
                defaultValue = "";
            }
            else if(dotNetType == typeof(string)) {
                columnType = "string";
                defaultValue = "string.Empty";
            }
            else if(dotNetType == typeof(Guid)) {
                columnType = "Guid";
                defaultValue = "Guid.Empty";
            }
            else if(dotNetType == typeof(short)) {
                columnType = "short";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(int)) {
                columnType = "int";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(long)) {
                columnType = "long";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(ushort)) {
                columnType = "ushort";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(uint)) {
                columnType = "uint";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(ulong)) {
                columnType = "ulong";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(bool)) {
                columnType = "bool";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(decimal)) {
                columnType = "decimal";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(float)) {
                columnType = "float";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(double)) {
                columnType = "double";
                defaultValue = "0";
            }
            else if(dotNetType == typeof(DateTime)) {
                columnType = "DateTime";
                defaultValue = "";
            }
            else if(dotNetType == typeof(DateTimeOffset)) {
                columnType = "DateTimeOffset";
                defaultValue = "";
            }
            else if(dotNetType == typeof(DateOnly)) {
                columnType = "DateOnly";
                defaultValue = "";
            }
            else if(dotNetType == typeof(TimeOnly)) {
                columnType = "TimeOnly";
                defaultValue = "";
            }
            else if(dotNetType == typeof(StringKey<IUnknownType>)) {
                columnType = "__UnknownType__";
                defaultValue = "__UnknownType__";
            }
            else {
                columnType = column.DataType.DotNetType.ToString();
                defaultValue = "???";
            }

            DatabaseTable? referencedTable = null;

            foreach(DatabaseForeignKey foreignKey in table.ForeignKeys) {

                foreach(DatabaseForeignKeyReference reference in foreignKey.References) {

                    if(string.Compare(reference.ForeignKeyColumn.ColumnName.Value, column.ColumnName.Value, ignoreCase: true) == 0) {

                        referencedTable = reference.PrimaryKeyColumn.Table;
                        break;
                    }
                }
                if(referencedTable != null) {
                    break;
                }
            }

            if(useIdentifiers && referencedTable != null) {

                isKeyColumn = true;                

                string keyName = $"I{GetTableName(referencedTable, includePostFix: false)}";

                if(dotNetType == typeof(string)) {
                    columnType = $"StringKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(Guid)) {
                    columnType = $"GuidKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(short)) {
                    columnType = $"ShortKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(int)) {
                    columnType = $"IntKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(long)) {
                    columnType = $"LongKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                columnTypeName = columnType;
            }
            else if(useIdentifiers && column.IsPrimaryKey) {

                isKeyColumn = true;

                string keyName = $"I{GetTableName(table, includePostFix: false)}";

                if(dotNetType == typeof(string)) {
                    columnType = $"StringKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(Guid)) {
                    columnType = $"GuidKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(short)) {
                    columnType = $"ShortKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(int)) {
                    columnType = $"IntKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }
                if(dotNetType == typeof(long)) {
                    columnType = $"LongKey<{keyName}>";
                    defaultValue = $"{columnType}.NotValid";
                }

                columnTypeName = columnType;
            }
            else {
                columnTypeName = columnType;
            }
        }

        private readonly static Dictionary<string, object?> _KeyWordLookup = new Dictionary<string, object?>() {
            { "abstract", null },
            { "as", null },
            { "base", null },
            { "bool", null },
            { "break", null },
            { "byte", null },
            { "case", null },
            { "catch", null },
            { "char", null },
            { "checked", null },
            { "class", null },
            { "const", null },
            { "continue", null },
            { "decimal", null },
            { "default", null },
            { "delegate", null },
            { "do", null },
            { "double", null },
            { "else", null },
            { "enum", null },
            { "event", null },
            { "explicit", null },
            { "extern", null },
            { "false", null },
            { "finally", null },
            { "fixed", null },
            { "float", null },
            { "for", null },
            { "foreach", null },
            { "goto", null },
            { "if", null },
            { "implicit", null },
            { "in", null },
            { "int", null },
            { "interface", null },
            { "internal", null },
            { "is", null },
            { "lock", null },
            { "long", null },
            { "namespace", null },
            { "new", null },
            { "null", null },
            { "object", null },
            { "operator", null },
            { "out", null },
            { "override", null },
            { "params", null },
            { "private", null },
            { "protected", null },
            { "public", null },
            { "readonly", null },
            { "ref", null },
            { "return", null },
            { "sbyte", null },
            { "sealed", null },
            { "short", null },
            { "sizeof", null },
            { "stackalloc", null },
            { "static", null },
            { "string", null },
            { "struct", null },
            { "switch", null },
            { "this", null },
            { "throw", null },
            { "true", null },
            { "try", null },
            { "typeof", null },
            { "uint", null },
            { "ulong", null },
            { "unchecked", null },
            { "unsafe", null },
            { "ushort", null },
            { "using", null },
            { "virtual", null },
            { "void", null },
            { "volatile", null },
            { "while", null }
        };

        public static bool IsCSharpKeyword(string text) {
            return _KeyWordLookup.ContainsKey(text.Trim());
        }
    }
}