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
using DbSchema.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;

namespace QueryLite.DbSchema.CodeGeneration {

    public static class OutputToFolder {

        public static void Output(List<DatabaseTable> tables, Namespaces namespaces, CodeGeneratorSettings settings, string folder, bool singleFiles, IDatabase database) {

            if(string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)) {
                throw new ArgumentException($"{nameof(folder)} = '{folder}' does not exist");
            }

            string tablesFolder = Path.Combine(folder, "Tables");
            string classesFolder = Path.Combine(folder, "Classes");
            string validationFolder = Path.Combine(folder, "Validation");
            string requestsFolder = Path.Combine(folder, "Requests");

            if(singleFiles) {

                Directory.CreateDirectory(tablesFolder);
                Directory.CreateDirectory(classesFolder);
                Directory.CreateDirectory(validationFolder);
                Directory.CreateDirectory(requestsFolder);

                foreach(DatabaseTable table in tables) {

                    TablePrefix prefix = new TablePrefix(table);

                    CodeBuilder tableCode = TableCodeGenerator.Generate(new List<DatabaseTable>() { table }, settings);

                    string tableFileName = Path.Combine(tablesFolder, CodeHelper.GetTableName(table, includePostFix: true) + ".cs");
                    File.WriteAllText(tableFileName, tableCode.ToString());

                    CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(database, table, prefix, settings, includeUsings: true);

                    string classesFileName = Path.Combine(classesFolder, CodeHelper.GetTableName(table, includePostFix: false) + (table.IsView ? "View" : string.Empty) + ".cs");
                    File.WriteAllText(classesFileName, classCode.ToString());

                    if(!table.IsView) {
                        
                        CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(table, prefix, settings, includeUsings: true);
                        string validationFileName = Path.Combine(validationFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Validation.cs");
                        File.WriteAllText(validationFileName, validationCode.ToString());

                        CodeBuilder requestsCode = MediatorCodeGenerator.GenerateMediatorRequestsCode(table, prefix, settings, includeUsings: true);
                        string requestsFileName = Path.Combine(requestsFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Requests.cs");
                        File.WriteAllText(requestsFileName, requestsCode.ToString());
                    }
                }
            }
            else {

                CodeBuilder tableCode = TableCodeGenerator.Generate(tables, settings);
                CodeBuilder classCode = ClassCodeGenerator.Generate(database, tables, settings);
                CodeBuilder validationCode = FluentValidationGenerator.Generate(tables, settings);
                CodeBuilder requestsCode = MediatorCodeGenerator.GenerateMediatorRequestsCode(tables, settings, includeUsings: true);

                File.WriteAllText(Path.Combine(folder, "Tables.cs"), tableCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Classes.cs"), classCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Validation.cs"), validationCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Requests.cs"), requestsCode.ToString());
            }
        }
    }
}