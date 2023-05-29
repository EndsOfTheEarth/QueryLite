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
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlInsertQueryGenerator : IInsertQueryGenerator {

        string IInsertQueryGenerator.GetSql(InsertQueryTemplate template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            
            sql.Append("INSERT INTO ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);

            if(useParameters == Parameters.On || (useParameters == Parameters.Default && Settings.UseParameters)) {

                StringBuilder paramSql = StringBuilderCache.Acquire();

                PostgreSqlSetValuesParameterCollector valuesCollector = new PostgreSqlSetValuesParameterCollector(sql, paramSql: paramSql, database, CollectorMode.Insert);

                sql.Append('(');
                template.ValuesCollector!(valuesCollector);
                sql.Append(") VALUES(");

                parameters = valuesCollector.Parameters;

                sql.Append(paramSql);
                sql.Append(')');

                StringBuilderCache.Release(paramSql);
            }
            else {

                PostgreSqlSetValuesCollector valuesCollector = new PostgreSqlSetValuesCollector(sql, database, CollectorMode.Insert);

                sql.Append('(');
                template.ValuesCollector!(valuesCollector);
                sql.Append(") VALUES(");

                parameters = null;

                sql.Append(valuesCollector.ParamsSql);
                sql.Append(')');
            }

            {
                if(template.ReturningFields != null && template.ReturningFields.Count > 0) {

                    sql.Append(" RETURNING ");

                    bool first = true;

                    foreach(IColumn column in template.ReturningFields) {

                        if(!first) {
                            sql.Append(',');
                        }
                        else {
                            first = false;
                        }
                        PostgreSqlHelper.AppendColumnName(sql, column);
                    }
                }
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}