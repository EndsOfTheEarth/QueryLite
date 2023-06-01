using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;
using System;
using System.Data.Common;

namespace QueryLite.Databases {

    internal static class SelectCollectorCache {

        [ThreadStatic]
        private static SqlServerResultRow? SqlServerInstance;

        [ThreadStatic]
        private static PostgreSqlResultRow? PostgreSqlInstance;

        public static IResultRow Acquire(DatabaseType databaseType, DbDataReader reader) {

            if(databaseType == DatabaseType.SqlServer) {

                if(Settings.EnableCollectorCaching) {

                    SqlServerResultRow? resultRow = SqlServerInstance;

                    if(resultRow != null) {
                        SqlServerInstance = null;
                        ((IResultRow)resultRow).Reset(reader);
                        return resultRow;
                    }
                }
                return new SqlServerResultRow(reader);
            }
            else if(databaseType == DatabaseType.PostgreSql) {

                if(Settings.EnableCollectorCaching) {

                    PostgreSqlResultRow? resultRow = PostgreSqlInstance;

                    if(resultRow != null) {
                        PostgreSqlInstance = null;
                        ((IResultRow)resultRow).Reset(reader);
                        return resultRow;
                    }
                }
                return new PostgreSqlResultRow(reader);
            }
            else {
                throw new Exception($"Unknown {nameof(DatabaseType)}. Value = '{databaseType}'");
            }
        }

        public static void Release(DatabaseType databaseType, IResultRow resultRow) {

            if(databaseType == DatabaseType.SqlServer) {
                SqlServerInstance = (SqlServerResultRow?)resultRow;
            }
            else if(databaseType == DatabaseType.PostgreSql) {
                PostgreSqlInstance = (PostgreSqlResultRow?)resultRow;
            }
            else {
                throw new Exception($"Unknown {nameof(DatabaseType)}. Value = '{databaseType}'");
            }
        }
    }
}