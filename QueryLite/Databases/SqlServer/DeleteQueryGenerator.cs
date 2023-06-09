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
using QueryLite.Databases.SqlServer.Collectors;
using System;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerDeleteQueryGenerator : IDeleteQueryGenerator {

        string IDeleteQueryGenerator.GetSql<RESULT>(DeleteQueryTemplate template, IDatabase database, IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc) {

            if(template.Usings != null) {
                throw new Exception("Using syntax is not supported by Sql Server");
            }

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            bool useAlias = template.Joins?.Count != 0;

            //
            //  Note: The OUPUT clause changes goes before the 'FROM' clause when using aliasing and after the 'FROM' clause when not
            //
            if(useAlias) {

                sql.Append("DELETE ").Append(template.Table.Alias);

                GenerateOutputClause(sql, outputFunc);
                
                sql.Append(" FROM ");

                string schemaName = database.SchemaMap(template.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, template.Table);

                sql.Append(" AS ").Append(template.Table.Alias).Append(' ');
            }
            else {

                sql.Append("DELETE FROM ");

                string schemaName = database.SchemaMap(template.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, template.Table);

                GenerateOutputClause(sql, outputFunc);
            }

            if(template.Joins != null) {

                foreach(IJoin join in template.Joins) {

                    sql.Append(join.JoinType switch {
                        JoinType.Join => " JOIN ",
                        JoinType.LeftJoin => " LEFT JOIN ",
                        _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                    });

                    string joinSchemaName = database.SchemaMap(join.Table.SchemaName);

                    if(!string.IsNullOrWhiteSpace(joinSchemaName)) {
                        SqlHelper.AppendEncloseSchemaName(sql, joinSchemaName);
                        sql.Append('.');
                    }
                    SqlHelper.AppendEncloseTableName(sql, join.Table);
                    sql.Append(" AS ").Append(join.Table.Alias).Append(" ON ");
                    join.Condition.GetSql(sql, database, useAlias: true, parameters);
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