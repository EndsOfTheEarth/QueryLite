using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;
using System;
using System.Text;

namespace DbSchema.CodeGeneration {

    public static class MediatorLoadSingleRecordRequestGenerator {

        public static string GetLoadRequest(DatabaseTable table, CodeGeneratorSettings settings) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(table, name);

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
public sealed class {requestName} : IRequest<{name}> {{

    public {requestName}({parametersText}) {{
{settersText}
    }}
{propertiesText}
}}
";
            return code;
        }

        private static string GetLoadListRequestName(DatabaseTable table, string name) {
            return $"Load{name}Request";
        }
        private static string GetLoadListHandlerName(DatabaseTable table, string name) {
            return $"Load{name}Handler";
        }

        public static string GetLoadListHandlerCode(DatabaseTable table, CodeGeneratorSettings settings) {

            if(settings.UsePreparedQueries) {
                return GetLoadListHandlerCodeWithCompiledQuery(table, settings);
            }
            else {
                return GetLoadListHandlerCodeNonCompiledQuery(table);
            }
        }

        private static string GetLoadListHandlerCodeWithCompiledQuery(DatabaseTable table, CodeGeneratorSettings settings) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(table, name);
            string handlerName = GetLoadListHandlerName(table, name);

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
public sealed class {handlerName}: IRequestHandler<{requestName}, {name}> {{

    private readonly static IPreparedQueryExecute<{requestName}, {name}> _query;

    static {handlerName}() {{

        {name}Table table = {name}Table.Instance;

        _query = Query
            .Prepare<{requestName}> ()
            .Select(row => new {name}(table, row))
            .From(table)
            .Where(where => {whereClause})
            .Build();
    }}

    private readonly IDatabase _database;

    public {handlerName}(IDatabase database) {{
        _database = database;
    }}

    public async Task<{name}> Handle({requestName} request, CancellationToken cancellationToken) {{

        QueryResult<{name}> result = await _query.ExecuteAsync(parameters: request, _database, cancellationToken, TimeoutLevel.ShortSelect);

        if(result.Rows.Count != 1) {{
            throw new Exception($""Record not found. {{nameof(result.Rows)}} != 1. Value = {{result.Rows}}"");
        }}
        return result.Rows[0];
    }}
}}
";
            return code;
        }

        private static string GetLoadListHandlerCodeNonCompiledQuery(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(table, name);
            string handlerName = GetLoadListHandlerName(table, name);


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
public sealed class {handlerName}: IRequestHandler<{requestName}, {name}> {{

    private readonly IDatabase _database;

    public {handlerName}(IDatabase database) {{
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
            .ExecuteAsync(_database, cancellationToken, TimeoutLevel.ShortSelect);

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