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
using QueryLite.Databases.SqlServer;
using System;
using System.Data.Common;

namespace QueryLite.Databases {

    internal static class SelectCollectorCache {

        [ThreadStatic]
        private static SqlServerResultRowCollector? SqlServerInstance;

        [ThreadStatic]
        private static PostgreSqlResultRowCollector? PostgreSqlInstance;

        public static IResultRow Acquire(DatabaseType databaseType, DbDataReader reader) {

            if(databaseType == DatabaseType.SqlServer) {

                if(Settings.EnableCollectorCaching) {

                    SqlServerResultRowCollector? resultRow = SqlServerInstance;

                    if(resultRow != null) {
                        SqlServerInstance = null;
                        resultRow.Reset(reader);
                        return resultRow;
                    }
                }
                return new SqlServerResultRowCollector(reader);
            }
            else if(databaseType == DatabaseType.PostgreSql) {

                if(Settings.EnableCollectorCaching) {

                    PostgreSqlResultRowCollector? resultRow = PostgreSqlInstance;

                    if(resultRow != null) {
                        PostgreSqlInstance = null;
                        resultRow.Reset(reader);
                        return resultRow;
                    }
                }
                return new PostgreSqlResultRowCollector(reader);
            }
            else {
                throw new Exception($"Unknown {nameof(DatabaseType)}. Value = '{databaseType}'");
            }
        }

        public static void Release(DatabaseType databaseType, IResultRow resultRow) {

            if(databaseType == DatabaseType.SqlServer) {
                SqlServerInstance = (SqlServerResultRowCollector)resultRow;
                SqlServerInstance.ReleaseReader();
            }
            else if(databaseType == DatabaseType.PostgreSql) {
                PostgreSqlInstance = (PostgreSqlResultRowCollector)resultRow;
                PostgreSqlInstance.ReleaseReader();
            }
            else {
                throw new Exception($"Unknown {nameof(DatabaseType)}. Value = '{databaseType}'");
            }
        }
    }
}