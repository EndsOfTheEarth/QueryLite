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
using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;
using System;
using System.Collections.Generic;

namespace DbSchema.CodeGeneration {

    public static class MediatorCodeGenerator {

        public static CodeBuilder GenerateMediatorRequestsCode(List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            CodeBuilder code = new CodeBuilder();

            code.Append("using QueryLite;").EndLine();
            code.Append($"using {settings.Namespaces.TableNamespace};").EndLine();


            code.Append(@$"
namespace {settings.Namespaces.BaseNamespace} {{

    public struct Response {{

        public static Response Success => new Response(successful: true, validation: null);
        public static Response Failure(FluentValidation.Results.ValidationResult validation) => new Response(successful: false, validation);

        public bool Successful {{ get; }}
        public FluentValidation.Results.ValidationResult? Validation {{ get; }}

        public Response(bool successful, FluentValidation.Results.ValidationResult? validation) {{
            Successful = successful;
            Validation = validation;
        }}
    }}

    public interface IRequest<RESPONSE> {{

    }}
    public interface IRequestHandler<REQUEST, RESPONSE> {{

        Task<RESPONSE> Handle(REQUEST request, CancellationToken ct);
    }}
    
    public interface IMyDatabase : IDatabase {{ }}

    public class MyDatabase : QueryLite.Databases.SqlServer.SqlServerDatabase, IMyDatabase {{

        public MyDatabase(string name, string connectionString, Func<string, string>? schemaMap = null) : base(name, connectionString, schemaMap) {{

        }}
    }}
}}
");
            List<SchemaName> schemaNames = [];

            foreach(DatabaseTable table in tables) {

                if(!table.IsView && !schemaNames.Contains(table.Schema)) {
                    schemaNames.Add(table.Schema);
                }
            }

            schemaNames.Sort((a, b) => a.Value.CompareTo(b.Value));

            int count = 0;

            foreach(SchemaName schema in schemaNames) {

                if(count > 0) {
                    code.EndLine();
                }
                count++;
                code.EndLine().Append($"namespace {settings.Namespaces.GetRequestsNamespace(schema)} {{").EndLine();

                string tableNamespace = settings.Namespaces.GetTableNamespace(schema);

                if(settings.Namespaces.TableNamespace != tableNamespace) {  //If this table exist in a non default schema it will have a different namespace
                    code.EndLine().Indent(1).Append($"using {tableNamespace};").EndLine();
                }

                foreach(DatabaseTable table in tables) {

                    if(!table.IsView && string.Equals(table.Schema.Value, schema.Value, StringComparison.OrdinalIgnoreCase)) {

                        TablePrefix prefix = new TablePrefix(table);

                        code.Append(GenerateMediatorRequestsCode(table, prefix, settings, includeUsings: false).ToString());
                    }
                }
                code.Append("}");
            }
            return code;
        }

        public static CodeBuilder GenerateMediatorRequestsCode(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings, bool includeUsings) {

            CodeBuilder code = new CodeBuilder();

            if(includeUsings) {

                code.Append($"namespace {settings.Namespaces.GetRequestsNamespace(table.Schema)} {{").EndLine().EndLine();

                code.Indent(1).Append("using QueryLite;").EndLine();
                code.Indent(1).Append($"using {settings.Namespaces.TableNamespace};").EndLine();

                string tableNamespace = settings.Namespaces.GetTableNamespace(table.Schema);

                if(settings.Namespaces.TableNamespace != tableNamespace) {  //If this table exist in a non default schema it will have a different namespace
                    code.Indent(1).Append($"using {tableNamespace};").EndLine();
                }
            }

            code.Append(MediatorLoadSingleRecordRequestGenerator.GetLoadRequest(table, prefix, settings));
            code.Append(MediatorLoadListRequestGenerator.GetLoadListRequest(table));
            code.Append(MediatorCreateRequestGenerator.GetCreateRequest(table));
            code.Append(MediatorUpdateSingleRecordRequestGenerator.GetUpdateRequest(table, settings));
            code.Append(MediatorDeleteSingleRecordRequestGenerator.GetDeleteRequest(table, prefix, settings));

            if(includeUsings) {
                code.Append("}");
            }
            return code;
        }

        public static CodeBuilder GenerateMediatorHandlersCode(List<DatabaseTable> tables, CodeGeneratorSettings settings) {

            CodeBuilder code = new CodeBuilder();

            code.Append("using FluentValidation;").EndLine();
            code.Append("using QueryLite;").EndLine();
            code.Append($"using {settings.Namespaces.TableNamespace};").EndLine();

            List<SchemaName> schemaNames = [];

            foreach(DatabaseTable table in tables) {

                if(!table.IsView && !schemaNames.Contains(table.Schema)) {
                    schemaNames.Add(table.Schema);
                }
            }

            schemaNames.Sort((a, b) => a.Value.CompareTo(b.Value));

            int count = 0;

            foreach(SchemaName schema in schemaNames) {

                if(count > 0) {
                    code.EndLine();
                }
                count++;
                code.EndLine().Append($"namespace {settings.Namespaces.GetHandlersNamespace(schema)} {{").EndLine();

                string tableNamespace = settings.Namespaces.GetTableNamespace(schema);

                if(settings.Namespaces.TableNamespace != tableNamespace) {  //If this table exist in a non default schema it will have a different namespace
                    code.EndLine().Indent(1).Append($"using {tableNamespace};").EndLine();
                }

                foreach(DatabaseTable table in tables) {

                    if(!table.IsView && string.Equals(table.Schema.Value, schema.Value, StringComparison.OrdinalIgnoreCase)) {

                        TablePrefix prefix = new TablePrefix(table);

                        code.Append(GenerateMediatorHandlersCode(table, prefix, settings, includeUsings: false).ToString());
                    }
                }
                code.Append("}");
            }
            return code;
        }

        public static CodeBuilder GenerateMediatorHandlersCode(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings, bool includeUsings) {

            CodeBuilder code = new CodeBuilder();

            if(includeUsings) {

                code.Append($"namespace {settings.Namespaces.GetHandlersNamespace(table.Schema)} {{").EndLine().EndLine();

                code.Indent(1).Append("using QueryLite;").EndLine();

                string tableNamespace = settings.Namespaces.GetTableNamespace(table.Schema);

                if(settings.Namespaces.TableNamespace != tableNamespace) {  //If this table exist in a non default schema it will have a different namespace
                    code.Indent(1).Append($"using {tableNamespace};").EndLine();
                }
            }
            code.Append(MediatorLoadSingleRecordRequestGenerator.GetLoadListHandlerCode(table, prefix, settings));
            code.Append(MediatorLoadListRequestGenerator.GetLoadListHandlerCode(table, settings));
            code.Append(MediatorCreateRequestGenerator.GetCreateHandlerCode(table, prefix, settings));
            code.Append(MediatorUpdateSingleRecordRequestGenerator.GetUpdateHandlerCode(table, prefix, settings));
            code.Append(MediatorDeleteSingleRecordRequestGenerator.GetDeleteHandlerCode(table, prefix, settings));

            if(includeUsings) {
                code.Append("}");
            }
            return code;
        }
    }
}