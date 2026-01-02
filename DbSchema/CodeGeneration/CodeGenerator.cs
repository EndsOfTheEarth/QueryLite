/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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
using System.IO;
using System.Text;

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
        /// Use key structs on primary key and foreign key columns.
        /// </summary>
        public required IdentifierType UseIdentifiers { get; set; }

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

        public required bool UsePreparedQueries { get; set; }

        /// <summary>
        /// Use repository pattern for CRUD actions.
        /// </summary>
        public required bool UseRepositoryPattern { get; set; }

        public required Namespaces Namespaces { get; set; }
    }

    public enum IdentifierType {
        None,
        Custom
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

                CodeBuilder code = TableCodeGenerator.Generate(tables: [table], settings);

                string tableFilePath = Path.Combine(tablesDir, CodeHelper.GetTableName(table, includePostFix: true) + ".cs");
                File.WriteAllText(tableFilePath, code.ToString());

                CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(database, table, prefix, settings, includeUsings: true);

                string logicFilePath = Path.Combine(logicDir, CodeHelper.GetTableName(table, includePostFix: true) + ".cs");
                File.WriteAllText(logicFilePath, classCode.ToString());
            }
        }
    }

    public static class CodeHelper {

        public static string EscapeCSharpString(string text) {
            return !string.IsNullOrEmpty(text) ? text.Replace("\"", "\\\"") : text;
        }

        public static string GetTableName(DatabaseTable table, bool includePostFix) {

            string postFix = !table.IsView ? "Table" : "View";

            StringBuilder nameText = new StringBuilder(table.TableName.Value);

            nameText[0] = char.ToUpper(nameText[0]);    //Make first character uppercase

            nameText.Replace(" ", "");    //Remove space character

            nameText.Append(includePostFix ? postFix : "");

            char previous = nameText[0];

            for(int index = 1; index < nameText.Length; index++) {  //Replace underscores and make next character uppercase

                char current = nameText[index];

                if(previous == '_' && !char.IsUpper(current)) {
                    nameText[index] = char.ToUpper(current);
                }
                previous = current;
            }
            nameText.Replace("_", "");

            return nameText.ToString();
        }

        public static string FormatNameForClass(string name) {

            StringBuilder nameText = new StringBuilder(name);

            nameText[0] = char.ToUpper(nameText[0]);    //Make first character uppercase

            nameText.Replace(" ", "");    //Remove space character

            char previous = nameText[0];

            for(int index = 1; index < nameText.Length; index++) {  //Replace underscores and make next character uppercase

                char current = nameText[index];

                if(previous == '_' && !char.IsUpper(current)) {
                    nameText[index] = char.ToUpper(current);
                }
                previous = current;
            }
            nameText.Replace("_", "");

            return nameText.ToString();
        }

        public struct ColumnInfo {

            public ColumnInfo() { }

            public required Type DotNetType { get; set; }
            public string ColumnTypeName { get; set; } = "";
            public string UnderlyingTypeName { get; set; } = "";
            public IdentifierType IdentifierType { get; set; }
        }

        public static ColumnInfo GetColumnInfo(DatabaseTable table, DatabaseColumn column, IdentifierType useIdentifiers) {

            ColumnInfo columnInfo = new ColumnInfo {
                IdentifierType = IdentifierType.None,
                DotNetType = column.DataType.DotNetType
            };

            TypeDefaultLookup.TryGetValue(columnInfo.DotNetType, out TypeDefaults? typeDefaults);

            if(columnInfo.DotNetType.IsArray && columnInfo.DotNetType.GetElementType() == typeof(byte)) {
                columnInfo.ColumnTypeName = "byte[]";
            }
            else if(typeDefaults != null) {
                columnInfo.ColumnTypeName = typeDefaults.ColumnType;
            }
            else {
                columnInfo.ColumnTypeName = column.DataType.DotNetType.ToString();
            }

            DatabaseTable? referencedTable = ClassCodeGenerator.GetReferenceTable(table, column);

            bool isPrimaryOrForeignKey = referencedTable != null || column.IsPrimaryKey;

            if(useIdentifiers == IdentifierType.Custom && isPrimaryOrForeignKey) {

                columnInfo.IdentifierType = IdentifierType.Custom;
                columnInfo.UnderlyingTypeName = typeDefaults?.ColumnType ?? "";

                if(typeDefaults != null) {

                    string tableName;

                    if(referencedTable != null) {
                        tableName = GetTableName(referencedTable, includePostFix: false);
                    }
                    else {
                        tableName = GetTableName(table, includePostFix: false);
                    }
                    columnInfo.ColumnTypeName = $"{typeDefaults.TypePrefix}{tableName}{typeDefaults.TypePostfix}";
                }
            }
            else if(column.DataType.DotNetType.IsAssignableTo(typeof(IGeography))) {
                columnInfo.ColumnTypeName = $"{nameof(IGeography)}";
            }
            return columnInfo;
        }

        private readonly static Dictionary<Type, TypeDefaults> TypeDefaultLookup = new Dictionary<Type, TypeDefaults> {

            { typeof(string), new TypeDefaults(columnType: "string", typePrefix: "", typePostfix: "") },
            { typeof(Guid), new TypeDefaults(columnType: "Guid", typePrefix: "", typePostfix: "Guid") },
            { typeof(short), new TypeDefaults(columnType: "short", typePrefix: "", typePostfix: "Id") },
            { typeof(int), new TypeDefaults(columnType: "int", typePrefix: "", typePostfix: "Id") },
            { typeof(long), new TypeDefaults(columnType: "long", typePrefix: "", typePostfix: "Id") },
            { typeof(ushort), new TypeDefaults(columnType: "ushort", typePrefix: "", typePostfix: "Id") },
            { typeof(uint), new TypeDefaults(columnType: "uint", typePrefix: "", typePostfix: "Id") },
            { typeof(ulong), new TypeDefaults(columnType: "ulong", typePrefix: "", typePostfix: "Id") },
            { typeof(bool), new TypeDefaults(columnType: "bool", typePrefix: "", typePostfix: "") },
            { typeof(Bit), new TypeDefaults(columnType: "Bit", typePrefix: "", typePostfix: "") },
            { typeof(decimal), new TypeDefaults(columnType: "decimal", typePrefix: "", typePostfix: "") },
            { typeof(float), new TypeDefaults(columnType: "float", typePrefix: "", typePostfix: "") },
            { typeof(double), new TypeDefaults(columnType: "double", typePrefix: "", typePostfix: "") },
            { typeof(DateTime), new TypeDefaults(columnType: "DateTime", typePrefix: "", typePostfix: "") },
            { typeof(DateTimeOffset), new TypeDefaults(columnType: "DateTimeOffset", typePrefix: "", typePostfix: "") },
            { typeof(DateOnly), new TypeDefaults(columnType: "DateOnly", typePrefix: "", typePostfix: "") },
            { typeof(TimeOnly), new TypeDefaults(columnType: "TimeOnly", typePrefix: "", typePostfix: "") },
            { typeof(IUnknownType), new TypeDefaults(columnType: "__UnknownType__", typePrefix: "", typePostfix: "") }
        };

        public class TypeDefaults {

            public TypeDefaults(string columnType, string typePrefix, string typePostfix) {
                ColumnType = columnType;
                TypePrefix = typePrefix;
                TypePostfix = typePostfix;
            }
            public string ColumnType { get; }
            public string TypePrefix { get; }
            public string TypePostfix { get; }
        }

        private readonly static HashSet<string> _KeyWordLookup = new HashSet<string>() {
            { "abstract" },
            { "as" },
            { "base" },
            { "bool" },
            { "break" },
            { "byte" },
            { "case" },
            { "catch" },
            { "char" },
            { "checked" },
            { "class" },
            { "const" },
            { "continue" },
            { "decimal" },
            { "default" },
            { "delegate" },
            { "do" },
            { "double" },
            { "else" },
            { "enum" },
            { "event" },
            { "explicit" },
            { "extern" },
            { "false" },
            { "finally" },
            { "fixed" },
            { "float" },
            { "for" },
            { "foreach" },
            { "goto" },
            { "if" },
            { "implicit" },
            { "in" },
            { "int" },
            { "interface" },
            { "internal" },
            { "is" },
            { "lock" },
            { "long" },
            { "namespace" },
            { "new" },
            { "null" },
            { "object" },
            { "operator" },
            { "out" },
            { "override" },
            { "params" },
            { "private" },
            { "protected" },
            { "public" },
            { "readonly" },
            { "ref" },
            { "return" },
            { "sbyte" },
            { "sealed" },
            { "short" },
            { "sizeof" },
            { "stackalloc" },
            { "static" },
            { "string" },
            { "struct" },
            { "switch" },
            { "this" },
            { "throw" },
            { "true" },
            { "try" },
            { "typeof" },
            { "uint" },
            { "ulong" },
            { "unchecked" },
            { "unsafe" },
            { "ushort" },
            { "using" },
            { "virtual" },
            { "void" },
            { "volatile" },
            { "while" }
        };

        public static bool IsCSharpKeyword(string text) {
            return _KeyWordLookup.Contains(text.Trim());
        }
    }
}