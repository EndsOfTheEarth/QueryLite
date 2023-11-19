﻿/*
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

    public static class MediatorDeleteSingleRecordRequestGenerator {

        public static string GetDeleteRequest(DatabaseTable table, CodeGeneratorSettings settings) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetDeleteRequestName(table, name);

            StringBuilder parametersText = new StringBuilder();
            StringBuilder settersText = new StringBuilder();
            StringBuilder propertiesText = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsPrimaryKey) {

                    CodeHelper.GetColumnName(table, column, useIdentifiers: settings.UseIdentifiers, dotNetType: out Type dotNetType, columnTypeName: out string columnTypeName, out bool isKeyColumn);

                    if(parametersText.Length > 0) {
                        parametersText.Append(',');
                    }

                    string propertyName = column.ColumnName.Value.FirstLetterUpperCase();
                    string parameterName = column.ColumnName.Value.FirstLetterLowerCase();

                    parametersText.Append($"{columnTypeName} {parameterName}");

                    if(settersText.Length > 0) {
                        settersText.Append(',');
                    }

                    if(propertyName != parameterName) {
                        settersText.Append($"        {propertyName} = {parameterName};");
                    }
                    else {
                        settersText.Append($"        this.{propertyName} = {parameterName};");
                    }

                    if(propertiesText.Length > 0) {
                        propertiesText.Append(Environment.NewLine);
                    }
                    propertiesText.Append($"    public {columnTypeName} {propertyName} {{ get; set; }}");
                }
            }

            string code = $@"
public sealed class {requestName} : IRequest<Response> {{

    public {requestName}({parametersText}) {{
{settersText}
    }}
{propertiesText}
}}
";
            return code;
        }

        private static string GetDeleteRequestName(DatabaseTable table, string name) {
            return $"Delete{name}Request";
        }
        private static string GetDeleteHandlerName(DatabaseTable table, string name) {
            return $"Delete{name}Handler";
        }

        public static string GetDeleteHandlerCode(DatabaseTable table, CodeGeneratorSettings settings) {

            if(settings.UsePreparedQueries) {
                return GetDeleteHandlerCodeWithCompiledQuery(table, settings);
            }
            else {
                return GetDeleteHandlerCodeNonCompiledQuery(table);
            }
        }

        private static string GetDeleteHandlerCodeWithCompiledQuery(DatabaseTable table, CodeGeneratorSettings settings) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetDeleteRequestName(table, name);
            string handlerName = GetDeleteHandlerName(table, name);

            StringBuilder whereClause = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsPrimaryKey) {

                    string propertyName = column.ColumnName.Value.FirstLetterUpperCase();

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
public sealed class {handlerName}: IRequestHandler<{requestName}, Response> {{

    private readonly static IPreparedDeleteQuery<{requestName}> _query;

    static {handlerName}() {{

        {name}Table table = {name}Table.Instance;

        _query = Query
            .Prepare<{requestName}> ()
            .Delete(table)
            .Where(where => {whereClause})
            .Build();
    }}

    private readonly IDatabase _database;

    public {handlerName}(IDatabase database) {{
        _database = database;
    }}

    public async Task<Response> Handle({requestName} request, CancellationToken cancellationToken) {{

        using(Transaction transaction = new Transaction(_database)) {{

            NonQueryResult result = await _query.ExecuteAsync(parameters: request, transaction, cancellationToken);
        
            if(result.RowsEffected != 1) {{
                throw new Exception($""Record not found. {{nameof(result.RowsEffected)}} != 1. Value = {{result.RowsEffected}}"");
            }}
            transaction.Commit();
        }}        
        return Response.Success;
    }}
}}
";
            return code;
        }

        private static string GetDeleteHandlerCodeNonCompiledQuery(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetDeleteRequestName(table, name);
            string handlerName = GetDeleteHandlerName(table, name);


            StringBuilder whereClause = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(column.IsPrimaryKey) {

                    string propertyName = column.ColumnName.Value.FirstLetterUpperCase();

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
public sealed class {handlerName}: IRequestHandler<{requestName}, Response> {{

    private readonly IDatabase _database;

    public {handlerName}(IDatabase database) {{
        _database = database;
    }}

    public async Task<Response> Handle({requestName} request, CancellationToken cancellationToken) {{

        {name}Table table = {name}Table.Instance;

        using(Transaction transaction = new Transaction(_database)) {{

            NonQueryResult result = await Query
                .Delete(table)
                .Where({whereClause})
                .ExecuteAsync(transaction, cancellationToken);

            if(result.RowsEffected != 1) {{
                throw new Exception($""Record not found. {{nameof(result.RowsEffected)}} != 1. Value = {{result.RowsEffected}}"");
            }}
            transaction.Commit();
        }}
        return Response.Success;
    }}
}}
";
            return code;
        }
    }
}