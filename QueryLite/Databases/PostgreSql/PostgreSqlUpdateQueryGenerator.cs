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
using QueryLite.Databases.PostgreSql.Collectors;
using System;
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlUpdateQueryGenerator : IUpdateQueryGenerator {

        string IUpdateQueryGenerator.GetSql<RESULT>(UpdateQueryTemplate template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc) {

            StringBuilder sql = StringBuilderCache.Acquire();
            
            sql.Append("UPDATE ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(" AS ").Append(template.Table.Alias).Append(' ');

            if(useParameters == Parameters.On || (useParameters == Parameters.Default && Settings.UseParameters)) {

                PostgreSqlSetValuesParameterCollector valuesCollector = new PostgreSqlSetValuesParameterCollector(sql, database, CollectorMode.Update);

                sql.Append(" SET ");

                template.ValuesCollector!(valuesCollector);

                parameters = valuesCollector.Parameters;
            }
            else {

                PostgreSqlSetValuesCollector valuesCollector = new PostgreSqlSetValuesCollector(sql, database, CollectorMode.Update);

                sql.Append(" SET ");

                template.ValuesCollector!(valuesCollector);

                parameters = null;
            }

            sql.Append(" FROM ");

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(' ');

            if(template.Joins != null) {

                foreach(IJoin join in template.Joins) {

                    string joinType = join.JoinType switch {
                        JoinType.Join => "JOIN",
                        JoinType.LeftJoin => "LEFT JOIN",
                        _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                    };
                    sql.Append(' ').Append(joinType).Append(' ');

                    string joinSchemaName = database.SchemaMap(join.Table.SchemaName);

                    if(!string.IsNullOrWhiteSpace(joinSchemaName)) {
                        PostgreSqlHelper.AppendEncase(sql, joinSchemaName, forceEnclose: false);
                        sql.Append('.');
                    }
                    PostgreSqlHelper.AppendEncase(sql, join.Table.TableName, forceEnclose: join.Table.Enclose);
                    sql.Append(" AS ").Append(join.Table.Alias).Append(" ON ");
                    join.Condition.GetSql(sql, database, useAlias: true, parameters);
                }
            }

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: true, parameters);
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