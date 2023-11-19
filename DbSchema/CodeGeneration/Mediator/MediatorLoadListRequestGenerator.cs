using QueryLite;
using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;

namespace DbSchema.CodeGeneration {

    public static class MediatorLoadListRequestGenerator {

        public static string GetLoadRequest(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string code = $@"
public sealed class {GetLoadListRequestName(table, name)} : IRequest<IList<{name}>> {{

}}
";
            return code;
        }

        private static string GetLoadListRequestName(DatabaseTable table, string name) {
            return $"Load{name}ListRequest";
        }
        private static string GetLoadListHandlerName(DatabaseTable table, string name) {
            return $"Load{name}ListHandler";
        }

        public static string GetLoadListHandlerCode(DatabaseTable table, CodeGeneratorSettings settings) {

            if(settings.UsePreparedQueries) {
                return GetLoadListHandlerCodeWithCompiledQuery(table);
            }
            else {
                return GetLoadListHandlerCodeNonCompiledQuery(table);
            }
        }

        private static string GetLoadListHandlerCodeWithCompiledQuery(DatabaseTable table) {

            string name = table.TableName.Value;

            name = name.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(table, name);
            string handlerName = GetLoadListHandlerName(table, name);

            string code = $@"
public sealed class {handlerName}: IRequestHandler<{requestName}, IList<{name}>> {{

    private readonly static IPreparedQueryExecute<bool, {name}> _query;

    static {handlerName}() {{

        {name}Table table = {name}Table.Instance;

        _query = Query
            .Prepare<bool>()
            .Select(row => new {name}(table, row))
            .From(table)
            .Build();
    }}

    private readonly IDatabase _database;

    public {handlerName}(IDatabase database) {{
        _database = database;
    }}

    public async Task<IList<{name}>> Handle({requestName} request, CancellationToken cancellationToken) {{

        QueryResult<{name}> list = await _query.ExecuteAsync(parameters: true, _database, cancellationToken, TimeoutLevel.ShortSelect);

        return list.Rows;
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

            string code = $@"
public sealed class {handlerName}: IRequestHandler<{requestName}, IList<{name}>> {{

    private readonly IDatabase _database;

    public {handlerName}(IDatabase database) {{
        _database = database;
    }}

    public async Task<IList<{name}>> Handle({requestName} request, CancellationToken cancellationToken) {{

        {name}Table table = {name}Table.Instance;

        QueryResult<{name}> list = await Query
            .Select(
                row => new {name}(table, row)
            )
            .From(table)
            .ExecuteAsync(_database, cancellationToken, TimeoutLevel.ShortSelect);

        return list.Rows;
    }}
}}
";
            return code;
        }
    }
}