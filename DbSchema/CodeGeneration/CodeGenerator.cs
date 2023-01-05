using QueryLite.DbSchema.Tables;
using System;
using System.Collections.Generic;
using System.IO;

namespace QueryLite.DbSchema.CodeGeneration {


    public class CodeGeneratorSettings {

        public required bool IncludeMessagePackAttributes { get; set; }
        public required bool IncludeJsonAttributes { get; set; }
        public required bool UseIdentifiers { get; set; }
        public required bool IncludeDescriptions { get; set; }
        public required bool IncludeKeyAttributes { get; set; }
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

        public static string GetTableName(DatabaseTable table, bool includePostFix) {
            string name = table.TableName.Value.Replace(" ", string.Empty) + (includePostFix ? "Table" : string.Empty);
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
            else if(dotNetType == typeof(StringKey<IUnknownType>)) {
                columnType = "__UnknownType__";
                defaultValue = "__UnknownType__";
            }
            else {
                columnType = column.DataType.DotNetType.ToString();
                defaultValue = "???";
            }

            if(useIdentifiers && column.IsForeignKey) {

                isKeyColumn = true;

                string keyName = $"I{GetTableName(column.ForeignKeyTable!, includePostFix: false)}";

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
    }
}