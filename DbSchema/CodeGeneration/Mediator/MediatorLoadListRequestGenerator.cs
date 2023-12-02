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
using QueryLite;
using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;

namespace DbSchema.CodeGeneration {

    public static class MediatorLoadListRequestGenerator {

        public static string GetLoadListRequest(DatabaseTable table) {

            string name = table.TableName.Value.FirstLetterUpperCase();

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

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(table, name);
            string handlerName = GetLoadListHandlerName(table, name);

            string code = $@"
    public sealed class {handlerName}: IRequestHandler<{requestName}, IList<{name}>> {{

        private readonly static IPreparedQueryExecute<{requestName}, {name}> _query;

        static {handlerName}() {{

            {name}Table table = {name}Table.Instance;

            _query = Query
                .Prepare<{requestName}>()
                .Select(row => new {name}(table, row))
                .From(table)
                .Build();
        }}

        private readonly __IDatabase__ _database;

        public {handlerName}(__IDatabase__ database) {{
            _database = database;
        }}

        public async Task<IList<{name}>> Handle({requestName} request, CancellationToken cancellationToken) {{

            QueryResult<{name}> list = await _query.ExecuteAsync(parameters: request, _database, cancellationToken);

            return list.Rows;
        }}
    }}
";
            return code;
        }

        private static string GetLoadListHandlerCodeNonCompiledQuery(DatabaseTable table) {

            string name = table.TableName.Value.FirstLetterUpperCase();

            string requestName = GetLoadListRequestName(table, name);
            string handlerName = GetLoadListHandlerName(table, name);

            string code = $@"
    public sealed class {handlerName}: IRequestHandler<{requestName}, IList<{name}>> {{

        private readonly __IDatabase__ _database;

        public {handlerName}(__IDatabase__ database) {{
            _database = database;
        }}

        public async Task<IList<{name}>> Handle({requestName} request, CancellationToken cancellationToken) {{

            {name}Table table = {name}Table.Instance;

            QueryResult<{name}> list = await Query
                .Select(
                    row => new {name}(table, row)
                )
                .From(table)
                .ExecuteAsync(_database, cancellationToken);

            return list.Rows;
        }}
    }}
";
            return code;
        }
    }
}