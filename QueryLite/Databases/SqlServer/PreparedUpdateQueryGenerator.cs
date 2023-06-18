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

    internal sealed class SqlServerPreparedUpdateQueryGenerator : IPreparedUpdateQueryGenerator {

        string IPreparedUpdateQueryGenerator.GetSql<PARAMETERS, RESULT>(PreparedUpdateTemplate<PARAMETERS> template, IDatabase database, out PreparedParameterList<PARAMETERS> parameters, Func<IResultRow, RESULT>? outputFunc) {

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            sql.Append("UPDATE ");

            bool useAlias = template.Joins != null;

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!useAlias) {

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, template.Table);
            }
            else {
                sql.Append(template.Table.Alias);
            }

            {
                SqlServerPreparedSetValuesCollector<PARAMETERS> valuesCollector = new SqlServerPreparedSetValuesCollector<PARAMETERS>(sql, paramSql: null, database, CollectorMode.Update, useAlias: useAlias);

                sql.Append(" SET ");
                template.SetValues!(valuesCollector); //Note: This outputs sql to the sql string builder

                parameters = valuesCollector.Parameters;
            }

            if(outputFunc != null) {

                SqlServerReturningFieldCollector collector = SqlServerReturningCollectorCache.Acquire(isDelete: false, sql);

                sql.Append(" OUTPUT ");

                outputFunc(collector);

                SqlServerReturningCollectorCache.Release(collector);
            }

            if(template.Joins != null) {

                sql.Append(" FROM ");

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                    sql.Append('.');
                }

                SqlHelper.AppendEncloseTableName(sql, template.Table);

                sql.Append(" AS ").Append(template.Table.Alias);

                foreach(PreparedUpdateJoin<PARAMETERS> join in template.Joins) {

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
                    join.Condition.GetSql(sql, database, parameters, useAlias: useAlias);
                }
            }
            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, parameters, useAlias: useAlias);
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}