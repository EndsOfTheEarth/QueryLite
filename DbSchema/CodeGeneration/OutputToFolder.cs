using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class OutputToFolder {

        public static void Output(List<DatabaseTable> tables, Namespaces namespaces, CodeGeneratorSettings settings, string folder, bool singleFiles, IDatabase database) {

            if(string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)) {
                throw new ArgumentException($"{nameof(folder)} = '{folder}' does not exist");
            }

            StringBuilder classesText = new StringBuilder();
            StringBuilder fluentValidationText = new StringBuilder();

            string tablesFolder = Path.Combine(folder, "Tables");
            string classesFolder = Path.Combine(folder, "Classes");
            string validationFolder = Path.Combine(folder, "Validation");

            if(singleFiles) {
                Directory.CreateDirectory(tablesFolder);
                Directory.CreateDirectory(classesFolder);
                Directory.CreateDirectory(validationFolder);
            }

            foreach(DatabaseTable table in tables) {


                if(classesText.Length > 0) {
                    classesText.Append(Environment.NewLine).Append(Environment.NewLine);
                }
                TablePrefix prefix = new TablePrefix(table);

                CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(database, table, prefix, settings);

                classesText.Append(classCode.ToString());

                if(!table.IsView) {
                    CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(database, table, prefix, settings.UseIdentifiers, namespaces);
                    fluentValidationText.Append(validationCode.ToString());
                }

                if(singleFiles) {

                    CodeBuilder tableCode = TableCodeGenerator.Generate(new List<DatabaseTable>() { table }, settings);

                    string tableFileName = Path.Combine(tablesFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Table.cs");
                    File.WriteAllText(tableFileName, tableCode.ToString());

                    string classesFileName = Path.Combine(classesFolder, CodeHelper.GetTableName(table, includePostFix: false) + ".cs");
                    File.WriteAllText(classesFileName, classesText.ToString());
                    classesText.Clear();

                    if(!table.IsView) {
                        string validationFileName = Path.Combine(validationFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Validation.cs");
                        File.WriteAllText(validationFileName, fluentValidationText.ToString());
                        fluentValidationText.Clear();
                    }
                }
                else {
                    fluentValidationText.Append(Environment.NewLine).Append(Environment.NewLine);
                }
            }

            if(!singleFiles) {

                CodeBuilder tableCode = TableCodeGenerator.Generate(tables, settings);

                File.WriteAllText(Path.Combine(folder, "Tables.cs"), tableCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Classes.cs"), classesText.ToString());
                File.WriteAllText(Path.Combine(folder, "Validation.cs"), fluentValidationText.ToString().TrimEnd());
            }
        }
    }
}