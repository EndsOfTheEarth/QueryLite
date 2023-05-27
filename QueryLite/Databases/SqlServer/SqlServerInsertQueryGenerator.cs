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

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerInsertQueryGenerator : IInsertQueryGenerator {

        string IInsertQueryGenerator.GetSql(InsertQueryTemplate template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters) {

            StringBuilder sql = new StringBuilder("INSERT INTO ", capacity: 256);

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);

            if(useParameters == Parameters.On || (useParameters == Parameters.Default && Settings.UseParameters)) {

                SqlServerSetValuesParameterCollector valuesCollector = new SqlServerSetValuesParameterCollector(database, CollectorMode.Insert);

                template.ValuesCollector!(valuesCollector);

                parameters = valuesCollector.Parameters;

                sql.Append("(");
                sql.Append(valuesCollector.ValuesSql);
                sql.Append(')');

                GetReturningSyntax(template, sql);

                sql.Append(" VALUES(").Append(valuesCollector.ParamSql).Append(')');
            }
            else {

                SqlServerSetValuesCollector valuesCollector = new SqlServerSetValuesCollector(database, CollectorMode.Insert);
                template.ValuesCollector!(valuesCollector);

                parameters = null;

                sql.Append("(");
                sql.Append(valuesCollector.ValuesSql);
                sql.Append(')');

                GetReturningSyntax(template, sql);

                sql.Append(" VALUES(").Append(valuesCollector.ParamsSql).Append(')');

            }
            return sql.ToString();
        }

        private static void GetReturningSyntax(InsertQueryTemplate template, StringBuilder sql) {

            if(template.ReturningFields != null && template.ReturningFields.Count > 0) {

                sql.Append(" OUTPUT");

                bool first = true;

                foreach(IColumn column in template.ReturningFields) {

                    if(!first) {
                        sql.Append(',');
                    }
                    else {
                        first = false;
                    }
                    sql.Append(" INSERTED.");
                    SqlServerHelper.AppendColumnName(sql, column);
                }
            }
        }
    }
}