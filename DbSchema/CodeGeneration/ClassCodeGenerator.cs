using System.Linq;
using System;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class ClassCodeGenerator {

        public static CodeBuilder GenerateClassCode(IDatabase database, DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings) {

            CodeBuilder classCode = new CodeBuilder();

            classCode.Append($"namespace { settings.Namespaces.ClassNamespace } {{").EndLine().EndLine();
            classCode.Indent(1).Append("using System;").EndLine();

            classCode.Indent(1).Append($"using {settings.Namespaces.TableNamespace };").EndLine();

            if(settings.IncludeMessagePackAttributes) {
                classCode.Indent(1).Append("using MessagePack;").EndLine();
            }
            if(settings.IncludeJsonAttributes) {
                classCode.Indent(1).Append("using System.Text.Json.Serialization;").EndLine();
            }

            classCode.EndLine();

            string className = CodeHelper.GetTableName(table, includePostFix: false);

            if(settings.IncludeMessagePackAttributes) {                
                classCode.Indent(1).Append("[MessagePackObject]").EndLine();
            }

            classCode.Indent(1).Append($"public sealed class { className } {{").EndLine().EndLine();

            classCode.Indent(2).Append("public ").Append(className).Append("(");

            CodeBuilder constructor = new CodeBuilder();

            int count = 0;
            foreach(DatabaseColumn column in table.Columns) {

                if(count > 0) {
                    classCode.Append(", ");
                }
                count++;

                CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string columnTypeName, out bool isKeyColumn);

                string nullable = column.IsNullable ? "?" : "";
                string columnName = prefix.GetColumnName(column.ColumnName.Value);

                string paramName = columnName.First().ToString().ToLower() + columnName.Substring(1);

                if(columnName == paramName) {   //TODO: handle keywords
                    paramName = "_" + paramName;
                }

                classCode.Append($"{ columnTypeName }{ nullable } { paramName }");

                constructor.Indent(3).Append($"{ columnName } = { paramName };").EndLine();
            }

            classCode.Append(") {").EndLine();
            classCode.Append(constructor.ToString());
            classCode.Indent(2).Append("}").EndLine();
            classCode.EndLine();
            classCode.Indent(2).Append("public ").Append(className).Append("(").Append(className).Append("Table table, IResultRow result) {").EndLine();

            CodeBuilder constructor2 = new CodeBuilder();

            foreach(DatabaseColumn column in table.Columns) {
                string columnName = prefix.GetColumnName(column.ColumnName.Value);
                constructor2.Indent(3).Append($"{columnName} = result.Get(table.{columnName});").EndLine();
            }            
            classCode.Append(constructor2.ToString());
            classCode.Indent(2).Append("}").EndLine();

            count = 0;

            foreach(DatabaseColumn column in table.Columns) {

                classCode.EndLine();

                CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string columnTypeName, out bool isKeyColumn);

                string columnName = prefix.GetColumnName(column.ColumnName.Value);

                if(settings.IncludeMessagePackAttributes || settings.IncludeJsonAttributes) {
                    classCode.EndLine();
                }
                if(settings.IncludeMessagePackAttributes) {
                    classCode.Indent(2).Append($"[Key({ count })]").EndLine();                    
                }

                if(settings.IncludeJsonAttributes) {
                    classCode.Indent(2).Append($"[JsonPropertyName(\"{columnName}\")]").EndLine();
                }

                classCode.Indent(2).Append($"public { columnTypeName }{ (column.IsNullable ? "?" : "") } { columnName } {{ get; set; }}");

                if(!column.IsNullable && (!dotNetType.IsPrimitive || (column.IsPrimaryKey || column.IsForeignKey))) {

                    string? propSet = database.GetCSharpCodeSet(dotNetType);

                    if(column.IsPrimaryKey || column.IsForeignKey) {
                        //classCode.Append($" = ").Append(columnTypeName).Append(".NotSet;");
                    }
                    else if(propSet != null) {
                        classCode.Append(" = ").Append(propSet).Append(";");
                    }
                }
                count++;
            }
            classCode.EndLine();
            classCode.Indent(1).Append("}").EndLine();

            classCode.Append("}");
            return classCode;
        }
    }
}