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
using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;
using System;
using System.Text;

namespace DbSchema.CodeGeneration {

    public static class MediatorLoadSingleRecordRequestGenerator {

        public static string GetLoadRequest(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(name);

            StringBuilder parametersText = new StringBuilder();
            StringBuilder settersText = new StringBuilder();
            StringBuilder propertiesText = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsPrimaryKey) {

                    CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string columnTypeName, out bool isKeyColumn);

                    if(parametersText.Length > 0) {
                        parametersText.Append(',');
                    }

                    string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

                    string propertyName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);
                    string parameterName = propertyName.FirstLetterLowerCase();

                    parametersText.Append($"{columnTypeName} {parameterName}");

                    if(settersText.Length > 0) {
                        settersText.Append(Environment.NewLine);
                    }

                    if(propertyName != parameterName) {
                        settersText.Append($"            {propertyName} = {parameterName};");
                    }
                    else {
                        settersText.Append($"            this.{propertyName} = {parameterName};");
                    }

                    if(propertiesText.Length > 0) {
                        propertiesText.Append(Environment.NewLine);
                    }
                    propertiesText.Append($"        public {columnTypeName} {propertyName} {{ get; set; }}");
                }
            }

            string code = $@"
    public sealed class {requestName} : IRequest<{name}> {{

        public {requestName}({parametersText}) {{
{settersText}
        }}
{propertiesText}
    }}
";
            return code;
        }

        private static string GetLoadListRequestName(string name) {
            return $"Load{name}Request";
        }
        private static string GetLoadListHandlerName(string name) {
            return $"Load{name}Handler";
        }

        public static string GetLoadListHandlerCode(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings) {

            if(settings.UsePreparedQueries) {
                return GetLoadListHandlerCodeWithCompiledQuery(table, prefix);
            }
            else {
                return GetLoadListHandlerCodeNonCompiledQuery(table, prefix);
            }
        }

        private static string GetLoadListHandlerCodeWithCompiledQuery(DatabaseTable table, TablePrefix prefix) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(name);
            string handlerName = GetLoadListHandlerName(name);

            StringBuilder whereClause = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsPrimaryKey) {

                    string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);
                    string propertyName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                    if(whereClause.Length > 0) {
                        whereClause.Append(" & ");
                    }
                    whereClause.Append($"where.EQUALS(table.{propertyName}, request => request.{propertyName})");
                }
            }

            if(whereClause.Length == 0) {
                whereClause.Append("??? No Primary Key Columns Exist ???");
            }

            string code = $@"
    public sealed class {handlerName}: IRequestHandler<{requestName}, {name}> {{

        private readonly static IPreparedQueryExecute<{requestName}, {name}> _query;

        static {handlerName}() {{

            {name}Table table = {name}Table.Instance;

            _query = Query
                .Prepare<{requestName}>()
                .Select(row => new {name}(table, row))
                .From(table)
                .Where(where => {whereClause})
                .Build();
        }}

        private readonly __IDatabase__ _database;

        public {handlerName}(__IDatabase__ database) {{
            _database = database;
        }}

        public async Task<{name}> Handle({requestName} request, CancellationToken cancellationToken) {{

            QueryResult<{name}> result = await _query.ExecuteAsync(parameters: request, _database, cancellationToken);

            if(result.Rows.Count != 1) {{
                throw new Exception($""Record not found. {{nameof(result.Rows)}} != 1. Value = {{result.Rows}}"");
            }}
            return result.Rows[0];
        }}
    }}
";
            return code;
        }

        private static string GetLoadListHandlerCodeNonCompiledQuery(DatabaseTable table, TablePrefix prefix) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(name);
            string handlerName = GetLoadListHandlerName(name);


            StringBuilder whereClause = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsPrimaryKey) {

                    string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);
                    string propertyName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                    if(whereClause.Length > 0) {
                        whereClause.Append(" & ");
                    }
                    whereClause.Append($"table.{propertyName} == request.{propertyName}");
                }
            }

            if(whereClause.Length == 0) {
                whereClause.Append("??? No Primary Key Columns Exist ???");
            }

            string code = $@"
    public sealed class {handlerName}: IRequestHandler<{requestName}, {name}> {{

        private readonly __IDatabase__ _database;

        public {handlerName}(__IDatabase__ database) {{
            _database = database;
        }}

        public async Task<{name}> Handle({requestName} request, CancellationToken cancellationToken) {{

            {name}Table table = {name}Table.Instance;

            QueryResult<{name}> result = await Query
                .Select(
                    row => new {name}(table, row)
                )
                .From(table)
                .Where({whereClause})
                .ExecuteAsync(_database, cancellationToken);

            if(result.Rows.Count != 1) {{
                throw new Exception($""Record not found. {{nameof(result.Rows)}} != 1. Value = {{result.Rows}}"");
            }}
            return result.Rows[0];
        }}
    }}
";
            return code;
        }
    }
}