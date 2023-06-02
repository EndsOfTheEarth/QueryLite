using System;
using System.Text;

namespace QueryLite.Databases.PostgreSql.Collectors {

    internal static class PostgreSqlReturningCollectorCache {

        [ThreadStatic]
        private static PostgreSqlReturningFieldCollector? Instance;

        public static PostgreSqlReturningFieldCollector Acquire(StringBuilder sql) {

            if(Settings.EnableCollectorCaching) {

                PostgreSqlReturningFieldCollector? instance = Instance;

                if(instance != null) {
                    Instance = null;
                    instance.Reset(sql);
                    return instance;
                }
            }
            return new PostgreSqlReturningFieldCollector(sql);
        }

        public static void Release(PostgreSqlReturningFieldCollector instance) {

            if(Settings.EnableCollectorCaching) {
                Instance = instance;
            }
        }
    }
}