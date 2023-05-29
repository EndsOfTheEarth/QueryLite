using System;
using System.Text;

namespace QueryLite.Databases {

    internal static class StringBuilderCache {

        [ThreadStatic]
        private static StringBuilder? InstanceA;

        [ThreadStatic]
        private static StringBuilder? InstanceB;

        public static StringBuilder Acquire(int capacity = 16) {

            if(Settings.EnableStringBuilderCaching &&  capacity <= Settings.StringBuilderCacheMaxCharacters) {

                StringBuilder? builder = InstanceA;

                if(builder != null && capacity <= builder.Capacity) {
                    InstanceA = null;
                    builder.Clear();
                    return builder;
                }

                builder = InstanceB;

                if(builder != null && capacity <= builder.Capacity) {
                    InstanceB = null;
                    builder.Clear();
                    return builder;
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder builder) {

            if(Settings.EnableStringBuilderCaching && builder.Capacity <= Settings.StringBuilderCacheMaxCharacters) {
                InstanceA = InstanceB;
                InstanceB = builder;
            }
        }
        public static string ToStringAndRelease(StringBuilder builder) {
            string result = builder.ToString();
            Release(builder);
            return result;
        }
    }
}
