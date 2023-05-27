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
using System;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerDeleteQueryGenerator : IDeleteQueryGenerator {

        string IDeleteQueryGenerator.GetSql(DeleteQueryTemplate template, IDatabase database, IParametersBuilder? parameters) {

            StringBuilder? outputClause = null;

            if(template.ReturningColumns?.Count > 0) {

                outputClause = new StringBuilder();

                outputClause.Append(" OUTPUT ");

                for(int index = 0; index < template.ReturningColumns.Count; index++) {

                    IColumn column = template.ReturningColumns[index];

                    if(index > 0) {
                        outputClause.Append(',');
                    }
                    outputClause.Append("DELETED.");
                    SqlServerHelper.AppendColumnName(outputClause, column);
                }
            }

            StringBuilder sql = new StringBuilder(capacity: 256);

            bool useAlias = template.Joins?.Count != 0;

            //
            //  Note: The OUPUT clase changes goes before the 'FROM' caluse when using aliasing and after the 'FROM' clause when not
            //
            if(useAlias) {

                sql.Append("DELETE ").Append(template.Table.Alias);

                if(outputClause != null) {
                    sql.Append(outputClause);
                }
                sql.Append(" FROM ");

                string schemaName = database.SchemaMap(template.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                    sql.Append('.');
                }
                SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);

                sql.Append(" AS ").Append(template.Table.Alias).Append(' ');                
            }
            else {

                sql.Append("DELETE FROM ");

                string schemaName = database.SchemaMap(template.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                    sql.Append('.');
                }
                SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);


                if(outputClause != null) {
                    sql.Append(outputClause);
                }
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
                        SqlServerHelper.AppendEnclose(sql, joinSchemaName, forceEnclose: false);
                        sql.Append('.');
                    }
                    SqlServerHelper.AppendEnclose(sql, join.Table.TableName, forceEnclose: join.Table.Enclose);
                    sql.Append(" AS ").Append(join.Table.Alias).Append(" ON ");
                    join.Condition.GetSql(sql, database, useAlias: true, parameters);
                }
            }

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: useAlias, parameters);
            }
            return sql.ToString();
        }
    }
}