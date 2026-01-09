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
using QueryLite.Databases.SqlServer.Collectors;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerDeleteQueryGenerator : IDeleteQueryGenerator {

        string IDeleteQueryGenerator.GetSql<RESULT>(DeleteQueryTemplate template, IDatabase database, IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc) {


            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            //
            //  Note: The OUPUT clause changes goes before the 'FROM' clause when using aliasing and after the 'FROM' clause when not
            //
            if(template.FromTables != null) {

                sql.Append("DELETE FROM ").Append(template.Table.Alias);

                GenerateOutputClause(sql, outputFunc);
            }
            else {

                sql.Append("DELETE FROM ");

                string schemaName = database.SchemaMap(template.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName, EncloseWith.SquareBracket);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, template.Table, EncloseWith.SquareBracket);

                GenerateOutputClause(sql, outputFunc);
            }

            bool useAlias = template.FromTables != null;

            if(template.FromTables != null) {

                sql.Append(" FROM ");

                string schemaName = database.SchemaMap(template.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName, EncloseWith.SquareBracket);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, template.Table, EncloseWith.SquareBracket);

                sql.Append(" AS ").Append(template.Table.Alias).Append(' ');

                for(int index = 0; index < template.FromTables.Count; index++) {

                    sql.Append(',');

                    ITable usingTable = template.FromTables[index];

                    string usingTableSchemaName = database.SchemaMap(template.Table.SchemaName);

                    if(!string.IsNullOrWhiteSpace(usingTableSchemaName)) {
                        SqlHelper.AppendEncloseSchemaName(sql, usingTableSchemaName, EncloseWith.SquareBracket);
                        sql.Append('.');
                    }

                    SqlHelper.AppendEncloseTableName(sql, usingTable, EncloseWith.SquareBracket);

                    sql.Append(' ');

                    SqlHelper.AppendEncloseAlias(sql, usingTable.Alias, EncloseWith.SquareBracket);
                }
            }

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: useAlias, parameters);
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }

        private static void GenerateOutputClause<RESULT>(StringBuilder sql, Func<IResultRow, RESULT>? outputFunc) {

            if(outputFunc != null) {

                SqlServerReturningFieldCollector collector = SqlServerReturningCollectorCache.Acquire(isDelete: true, sql);

                sql.Append(" OUTPUT ");

                outputFunc(collector);

                SqlServerReturningCollectorCache.Release(collector);
            }
        }
    }
}