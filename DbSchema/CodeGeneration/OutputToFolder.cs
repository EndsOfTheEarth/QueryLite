/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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

        public static void Output(List<DatabaseTable> tables, CodeGeneratorSettings settings, string folder, bool singleFiles, IDatabase database) {

            if(string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)) {
                throw new ArgumentException($"{nameof(folder)} = '{folder}' does not exist");
            }

            if(singleFiles) {

                foreach(DatabaseTable table in tables) {

                    string tablesFolder = Path.Combine(folder, "Tables");
                    //string validationFolder = Path.Combine(folder, "Validation");
                    string requestsFolder = Path.Combine(folder, "Requests");
                    string classesFolder = Path.Combine(folder, "Classes");
                    string handlersFolder = Path.Combine(folder, "Handlers");

                    Directory.CreateDirectory(tablesFolder);
                    Directory.CreateDirectory(classesFolder);
                    //Directory.CreateDirectory(validationFolder);
                    Directory.CreateDirectory(requestsFolder);
                    Directory.CreateDirectory(handlersFolder);

                    TablePrefix prefix = new TablePrefix(table);

                    OutputTableFile(settings, tablesFolder, table);
                    OutputClassCodeFile(settings, database, classesFolder, table, prefix);

                    if(!table.IsView) {

                        //OutputValidationFile(settings, validationFolder, table, prefix);
                        OutputRequestFile(settings, requestsFolder, table, prefix);
                        OutputHandlersAndValidationToFile(settings, handlersFolder, table, prefix);
                    }
                }
            }
            else {

                CodeBuilder tableCode = TableCodeGenerator.Generate(tables, settings);
                CodeBuilder classCode = ClassCodeGenerator.Generate(database, tables, settings);
                CodeBuilder validationCode = FluentValidationGenerator.Generate(tables, settings);
                CodeBuilder requestsCode = MediatorCodeGenerator.GenerateMediatorRequestsCode(tables, settings);
                CodeBuilder handlersCode = MediatorCodeGenerator.GenerateMediatorHandlersCode(tables, settings);

                File.WriteAllText(Path.Combine(folder, "Tables.cs"), tableCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Classes.cs"), classCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Validation.cs"), validationCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Requests.cs"), requestsCode.ToString());
                File.WriteAllText(Path.Combine(folder, "Handlers.cs"), handlersCode.ToString());
            }
        }

        private static void OutputHandlersAndValidationToFile(CodeGeneratorSettings settings, string handlersFolder, DatabaseTable table, TablePrefix prefix) {

            string handlersAndSchemaFolder = Path.Combine(handlersFolder, (table.Schema.Value ?? ""));

            if(!Directory.Exists(handlersAndSchemaFolder)) {
                Directory.CreateDirectory(handlersAndSchemaFolder);
            }

            CodeBuilder handlersCode = MediatorCodeGenerator.GenerateMediatorHandlersCode(table, prefix, settings, includeUsings: true);
            CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(table, prefix, settings, includeUsings: true);

            handlersCode.EndLine().EndLine();
            handlersCode.Append(validationCode.ToString()); //Add the validation code to the handlers code

            string handlersFileName = Path.Combine(handlersAndSchemaFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Handlers.cs");

            File.WriteAllText(handlersFileName, handlersCode.ToString());
        }

        private static void OutputRequestFile(CodeGeneratorSettings settings, string requestsFolder, DatabaseTable table, TablePrefix prefix) {

            string requestsAndSchemaFolder = Path.Combine(requestsFolder, (table.Schema.Value ?? ""));

            if(!Directory.Exists(requestsAndSchemaFolder)) {
                Directory.CreateDirectory(requestsAndSchemaFolder);
            }

            CodeBuilder requestsCode = MediatorCodeGenerator.GenerateMediatorRequestsCode(table, prefix, settings, includeUsings: true);
            string requestsFileName = Path.Combine(requestsAndSchemaFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Requests.cs");
            File.WriteAllText(requestsFileName, requestsCode.ToString());
        }

        //private static void OutputValidationFile(CodeGeneratorSettings settings, string validationFolder, DatabaseTable table, TablePrefix prefix) {

        //    string validationAndSchemaFolder = Path.Combine(validationFolder, (table.Schema.Value ?? ""));

        //    if(!Directory.Exists(validationAndSchemaFolder)) {
        //        Directory.CreateDirectory(validationAndSchemaFolder);
        //    }

        //    CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(table, prefix, settings, includeUsings: true);
        //    string validationFileName = Path.Combine(validationAndSchemaFolder, CodeHelper.GetTableName(table, includePostFix: false) + "Validation.cs");
        //    File.WriteAllText(validationFileName, validationCode.ToString());
        //}

        private static void OutputClassCodeFile(CodeGeneratorSettings settings, IDatabase database, string classesFolder, DatabaseTable table, TablePrefix prefix) {

            string classesAndSchemaFolder = Path.Combine(classesFolder, (table.Schema.Value ?? ""));

            if(!Directory.Exists(classesAndSchemaFolder)) {
                Directory.CreateDirectory(classesAndSchemaFolder);
            }

            CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(database, table, prefix, settings, includeUsings: true);

            string classesFileName = Path.Combine(classesAndSchemaFolder, CodeHelper.GetTableName(table, includePostFix: false) + (table.IsView ? "View" : "") + ".cs");
            File.WriteAllText(classesFileName, classCode.ToString());
        }

        private static void OutputTableFile(CodeGeneratorSettings settings, string tablesFolder, DatabaseTable table) {

            string tablesAndSchemaFolder = Path.Combine(tablesFolder, (table.Schema.Value ?? ""));

            if(!Directory.Exists(tablesAndSchemaFolder)) {
                Directory.CreateDirectory(tablesAndSchemaFolder);
            }

            CodeBuilder tableCode = TableCodeGenerator.Generate([table], settings);

            string tableFileName = Path.Combine(tablesAndSchemaFolder, CodeHelper.GetTableName(table, includePostFix: true) + ".cs");
            File.WriteAllText(tableFileName, tableCode.ToString());
        }
    }
}