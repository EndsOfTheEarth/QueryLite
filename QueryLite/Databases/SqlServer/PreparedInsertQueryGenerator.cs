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

    internal sealed class SqlServerPreparedInsertQueryGenerator : IPreparedInsertQueryGenerator {

        public string GetSql<PARAMETERS, RESULT>(PreparedInsertTemplate<PARAMETERS> template, IDatabase database, out PreparedParameterList<PARAMETERS> parameters, Func<IResultRow, RESULT>? outputFunc) {

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            sql.Append("INSERT INTO ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                sql.Append('.');
            }

            SqlHelper.AppendEncloseTableName(sql, template.Table);

            StringBuilder paramSql = StringBuilderCache.Acquire();

            SqlServerPreparedSetValuesCollector<PARAMETERS> valuesCollector = new SqlServerPreparedSetValuesCollector<PARAMETERS>(sql, paramSql: paramSql, database, CollectorMode.Insert, useAlias: false);

            sql.Append('(');

            template.SetValues!(valuesCollector); //Note: This outputs sql to the sql string builder

            sql.Append(')');

            parameters = valuesCollector.Parameters;

            GetReturningSyntax<PARAMETERS, RESULT>(sql, outputFunc);

            sql.Append(" VALUES(").Append(paramSql).Append(')');

            StringBuilderCache.Release(paramSql);

            return StringBuilderCache.ToStringAndRelease(sql);
        }

        private static void GetReturningSyntax<PARAMETERS, RESULT>(StringBuilder sql, Func<IResultRow, RESULT>? outputFunc) {

            if(outputFunc != null) {

                SqlServerReturningFieldCollector collector = SqlServerReturningCollectorCache.Acquire(isDelete: false, sql);

                sql.Append(" OUTPUT ");

                outputFunc(collector);

                SqlServerReturningCollectorCache.Release(collector);
            }
        }
    }
}