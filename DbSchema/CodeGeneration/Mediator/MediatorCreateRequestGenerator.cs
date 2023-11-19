using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;
using System;
using System.Text;

namespace DbSchema.CodeGeneration {

    public static class MediatorCreateRequestGenerator {

        public static string GetCreateRequest(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string code = $@"
public sealed class Create{name}Request : IRequest<Response> {{

    public {name} {name} {{ get; set; }}

    public Create{name}Request({name} {name.ToLower()}) {{
        {name} = {name.ToLower()};
    }}
}}
";
            return code;
        }

        public static string GetCreateHandlerCode(DatabaseTable table, CodeGeneratorSettings settings) {

            if(settings.UsePreparedQueries) {
                return GetCreateHandlerCodeWithCompiledQuery(table);
            }
            else {
                return GetCreateHandlerCodeNonCompiledQuery(table);
            }
        }

        private static string GetCreateHandlerCodeWithCompiledQuery(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            StringBuilder setValues = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(setValues.Length > 0) {
                    setValues.Append(Environment.NewLine);
                }
                string columnName = column.ColumnName.Value.FirstLetterUpperCase();
                setValues.Append($"                values.Set(table.{columnName}, info => info.{columnName});");
            }
            string code = $@"
public sealed class Create{name}Handler : IRequestHandler<Create{name}Request, Response> {{

    private static readonly {name}Validator _validator = new {name}Validator(isNew: true);

    private static readonly IPreparedInsertQuery<{name}> _insertQuery;

    static Create{name}Handler() {{

        {name}Table table = {name}Table.Instance;

        _insertQuery = Query.Prepare<{name}>()
            .Insert(table)
            .Values(values => {{
{setValues}
            }}
            ).Build();
    }}

    private readonly IDatabase _database;

    public Create{name}Handler(IDatabase database) {{
        _database = database;
    }}

    public async Task<Response> Handle(Create{name}Request request, CancellationToken cancellationToken) {{

        FluentValidation.Results.ValidationResult validation = _validator.Validate(request.{name});

        if(!validation.IsValid) {{
            return Response.Failure(validation);
        }}

        using(Transaction transaction = new Transaction(_database)) {{

            NonQueryResult result = await _insertQuery.ExecuteAsync(request.{name}, transaction, cancellationToken, TimeoutLevel.ShortInsert);

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

        private static string GetCreateHandlerCodeNonCompiledQuery(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            StringBuilder setValues = new StringBuilder();

            foreach(DatabaseColumn column in table.Columns) {

                if(setValues.Length > 0) {
                    setValues.Append(Environment.NewLine);
                }
                string columnName = column.ColumnName.Value.FirstLetterUpperCase();
                setValues.Append($"                    values.Set(table.{columnName}, info.{columnName});");
            }
            string code = $@"
public sealed class Create{name}Handler : IRequestHandler<Create{name}Request, Response> {{

    private static readonly {name}Validator _validator = new {name}Validator(isNew: true);

    private readonly IDatabase _database;

    public Create{name}Handler(IDatabase database) {{
        _database = database;
    }}

    public async Task<Response> Handle(Create{name}Request request, CancellationToken cancellationToken) {{

        FluentValidation.Results.ValidationResult validation = _validator.Validate(request.{name});

        if(!validation.IsValid) {{
            return Response.Failure(validation);
        }}

        using(Transaction transaction = new Transaction(_database)) {{

            {name}Table table = {name}Table.Instance;

            {name} info = request.{name};

            NonQueryResult result = await Query
                .Insert(table)
                .Values(values => {{
{setValues}
                }}
                ).ExecuteAsync(transaction, cancellationToken, timeout: TimeoutLevel.ShortInsert);

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