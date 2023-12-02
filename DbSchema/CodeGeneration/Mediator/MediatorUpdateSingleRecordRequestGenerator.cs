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

    public static class MediatorUpdateSingleRecordRequestGenerator {

        public static string GetUpdateRequest(DatabaseTable table, CodeGeneratorSettings settings) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetUpdateRequestName( name);

            string code = $@"
    public sealed class {requestName} : IRequest<Response> {{

        public {name} {name} {{ get; set; }}

        public {requestName}({name} {name.ToLower()}) {{
            {name} = {name.ToLower()};
        }}
    }}
";
            return code;
        }

        private static string GetUpdateRequestName(string name) {
            return $"Update{name}Request";
        }
        private static string GetUpdateHandlerName(string name) {
            return $"Update{name}Handler";
        }

        public static string GetUpdateHandlerCode(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings) {

            if(settings.UsePreparedQueries) {
                return GetUpdateHandlerCodeWithCompiledQuery(table, prefix, settings);
            }
            else {
                return GetUpdateHandlerCodeNonCompiledQuery(table, prefix);
            }
        }

        private static string GetUpdateHandlerCodeWithCompiledQuery(DatabaseTable table, TablePrefix prefix, CodeGeneratorSettings settings) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetUpdateRequestName(name);
            string handlerName = GetUpdateHandlerName(name);

            StringBuilder whereClause = new StringBuilder();

            StringBuilder setValues = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

                if(column.IsPrimaryKey) {

                    string propertyName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                    if(whereClause.Length > 0) {
                        whereClause.Append(" & ");
                    }
                    whereClause.Append($"where.EQUALS(table.{propertyName}, request => request.{propertyName})");
                }

                if(setValues.Length > 0) {
                    setValues.Append(Environment.NewLine);
                }                
                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);
                setValues.Append($"                    .Set(table.{columnName}, info => info.{columnName})");
            }

            if(whereClause.Length == 0) {
                whereClause.Append("??? No Primary Key Columns Exist ???");
            }

            string code = $@"
    public sealed class {handlerName}: IRequestHandler<{requestName}, Response> {{

        private static readonly {name}Validator _validator = new {name}Validator(isNew: false);

        private readonly static IPreparedUpdateQuery<{name}> _query;

        static {handlerName}() {{

            {name}Table table = {name}Table.Instance;

            _query = Query
                .Prepare<{name}>()
                .Update(table)
                .Values(values => values
{setValues}
                )
                .Where(where => {whereClause})
                .Build();
        }}

        private readonly IDatabase _database;

        public {handlerName}(IDatabase database) {{
            _database = database;
        }}

        public async Task<Response> Handle({requestName} request, CancellationToken cancellationToken) {{

            FluentValidation.Results.ValidationResult validation = _validator.Validate(request.{name});

            if(!validation.IsValid) {{
                return Response.Failure(validation);
            }}

            using(Transaction transaction = new Transaction(_database)) {{

                NonQueryResult result = await _query.ExecuteAsync(parameters: request.{name}, transaction, cancellationToken);

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

        private static string GetUpdateHandlerCodeNonCompiledQuery(DatabaseTable table, TablePrefix prefix) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetUpdateRequestName(name);
            string handlerName = GetUpdateHandlerName(name);

            StringBuilder setValues = new StringBuilder();

            StringBuilder whereClause = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                string tableClassName = CodeHelper.GetTableName(table, includePostFix: true);

                if(column.IsPrimaryKey) {

                    string propertyName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);

                    if(whereClause.Length > 0) {
                        whereClause.Append(" & ");
                    }
                    whereClause.Append($"table.{propertyName} == info.{propertyName}");
                }

                if(setValues.Length > 0) {
                    setValues.Append(Environment.NewLine);
                }
                string columnName = prefix.GetColumnName(column.ColumnName.Value, className: tableClassName);
                setValues.Append($"                        .Set(table.{columnName}, info.{columnName})");
            }

            if(whereClause.Length == 0) {
                whereClause.Append("??? No Primary Key Columns Exist ???");
            }

            string code = $@"
    public sealed class {handlerName}: IRequestHandler<{requestName}, Response> {{

        private static readonly {name}Validator _validator = new {name}Validator(isNew: false);

        private readonly IDatabase _database;

        public {handlerName}(IDatabase database) {{
            _database = database;
        }}

        public async Task<Response> Handle({requestName} request, CancellationToken cancellationToken) {{

            FluentValidation.Results.ValidationResult validation = _validator.Validate(request.{name});

            if(!validation.IsValid) {{
                return Response.Failure(validation);
            }}

            {name}Table table = {name}Table.Instance;

            {name} info = request.{name};

            using(Transaction transaction = new Transaction(_database)) {{

                NonQueryResult result = await Query
                    .Update(table)
                    .Values(values => values
{setValues}
                    )
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