/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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

    internal sealed class SqlServerUpdateQueryGenerator : IUpdateQueryGenerator {

        string IUpdateQueryGenerator.GetSql<RESULT>(UpdateQueryTemplate template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc) {

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            sql.Append("UPDATE ");

            bool useAlias = template.FromTables != null;

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

            if(useParameters == Parameters.On || (useParameters == Parameters.Default && Settings.UseParameters)) {

                SqlServerSetValuesParameterCollector valuesCollector = new SqlServerSetValuesParameterCollector(sql, paramSql: null, database, CollectorMode.Update, useAlias: useAlias);

                sql.Append(" SET ");
                template.ValuesCollector!(valuesCollector); //Note: This outputs sql to the sql string builder

                parameters = valuesCollector.Parameters;
            }
            else {

                SqlServerSetValuesCollector valuesCollector = new SqlServerSetValuesCollector(sql, database, CollectorMode.Update, useAlias: useAlias);

                sql.Append(" SET ");
                template.ValuesCollector!(valuesCollector); //Note: This outputs sql to the sql string builder

                parameters = null;
            }

            if(outputFunc != null) {

                SqlServerReturningFieldCollector collector = SqlServerReturningCollectorCache.Acquire(isDelete: false, sql);

                sql.Append(" OUTPUT ");

                outputFunc(collector);

                SqlServerReturningCollectorCache.Release(collector);
            }

            if(template.FromTables != null) {

                sql.Append(" FROM ");

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, template.Table);
                sql.Append(" as ").Append(template.Table.Alias);

                for(int index = 0; index < template.FromTables.Count; index++) {

                    ITable fromTable = template.FromTables[index];

                    if(fromTable == template.Table) {
                        throw new Exception("The update table must not be included in the 'FROM' table list");
                    }

                    sql.Append(',');

                    string fromSchemaName = database.SchemaMap(fromTable.SchemaName);

                    if(!string.IsNullOrWhiteSpace(fromSchemaName)) {
                        SqlHelper.AppendEncloseSchemaName(sql, fromSchemaName);
                        sql.Append('.');
                    }
                    SqlHelper.AppendEncloseTableName(sql, fromTable);
                    sql.Append(" as ").Append(fromTable.Alias);
                }
            }

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: useAlias, parameters);
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}