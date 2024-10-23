/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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
using QueryLite.Databases.PostgreSql.Collectors;
using System;
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlDeleteQueryGenerator : IDeleteQueryGenerator {

        string IDeleteQueryGenerator.GetSql<RESULT>(DeleteQueryTemplate template, IDatabase database, IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc) {

            StringBuilder sql = StringBuilderCache.Acquire();

            sql.Append("DELETE FROM ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                sql.Append('.');
            }

            SqlHelper.AppendEncloseTableName(sql, template.Table);
            sql.Append(" AS ").Append(template.Table.Alias);

            bool useAliases = template.FromTables != null;

            if(template.FromTables != null) {

                sql.Append(" USING ");

                for(int index = 0; index < template.FromTables.Count; index++) {

                    if(index > 0) {
                        sql.Append(',');
                    }
                    ITable usingTable = template.FromTables[index];

                    string usingTableSchemaName = database.SchemaMap(template.Table.SchemaName);

                    if(!string.IsNullOrWhiteSpace(usingTableSchemaName)) {
                        SqlHelper.AppendEncloseSchemaName(sql, usingTableSchemaName);
                        sql.Append('.');
                    }

                    SqlHelper.AppendEncloseTableName(sql, usingTable);

                    sql.Append(' ');

                    SqlHelper.AppendEncloseAlias(sql, usingTable.Alias);
                }
            }

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: useAliases, parameters);
            }
            if(outputFunc != null) {

                PostgreSqlReturningFieldCollector collector = PostgreSqlReturningCollectorCache.Acquire(sql, useAlias: true);

                sql.Append(" RETURNING ");

                outputFunc(collector);

                PostgreSqlReturningCollectorCache.Release(collector);
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}