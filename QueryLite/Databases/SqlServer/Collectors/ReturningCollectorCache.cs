using System;
using System.Text;

namespace QueryLite.Databases.SqlServer.Collectors {

    internal static class SqlServerReturningCollectorCache {

        [ThreadStatic]
        private static SqlServerReturningFieldCollector? Instance;

        public static SqlServerReturningFieldCollector Acquire(bool isDelete, StringBuilder sql) {

            if(Settings.EnableCollectorCaching) {

                SqlServerReturningFieldCollector? resultRow = Instance;

                if(resultRow != null) {
                    Instance = null;
                    resultRow.Reset(isDelete, sql);
                    return resultRow;
                }
            }
            return new SqlServerReturningFieldCollector(isDelete, sql);
        }

        public static void Release(SqlServerReturningFieldCollector instance) {

            if(Settings.EnableCollectorCaching) {
                Instance = instance;
            }
        }
    }
}