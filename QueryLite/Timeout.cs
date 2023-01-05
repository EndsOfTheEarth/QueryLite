using System;

namespace QueryLite {

    public readonly struct QueryTimeout { //Decided to not call it Timeout as there is a Timeout class in the System.Threading namespace

        /// <summary>
        /// Number of seconds until query is timed out
        /// </summary>
        public int Seconds { get; private init; }

        public QueryTimeout() {
            Seconds = 60;
        }
        public QueryTimeout(int seconds) {

            if(seconds <= 0) {
                throw new Exception($"{nameof(seconds)} must be > 0");
            }
            Seconds = seconds;
        }
    }

    public static class TimeoutLevel {

        /// <summary>
        /// Short select query timeout defaults to 60 seconds
        /// </summary>
        public static QueryTimeout ShortSelect { get; set; } = new QueryTimeout(seconds: 60);

        /// <summary>
        /// Short insert query timeout defaults to 60 seconds
        /// </summary>
        public static QueryTimeout ShortInsert { get; set; } = new QueryTimeout(seconds: 60);

        /// <summary>
        /// Short update query timeout defaults to 60 seconds
        /// </summary>
        public static QueryTimeout ShortUpdate { get; set; } = new QueryTimeout(seconds: 60);

        /// <summary>
        /// Short delete query timeout defaults to 60 seconds
        /// </summary>
        public static QueryTimeout ShortDelete { get; set; } = new QueryTimeout(seconds: 60);

        /// <summary>
        /// Medium select query timeout defaults to 300 seconds
        /// </summary>
        public static QueryTimeout MediumSelect { get; set; } = new QueryTimeout(seconds: 300);

        /// <summary>
        /// Medium insert query timeout defaults to 300 seconds
        /// </summary>
        public static QueryTimeout MediumInsert { get; set; } = new QueryTimeout(seconds: 300);

        /// <summary>
        /// Medium update query timeout defaults to 300 seconds
        /// </summary>
        public static QueryTimeout MediumUpdate { get; set; } = new QueryTimeout(seconds: 300);

        /// <summary>
        /// Medium delete query timeout defaults to 300 seconds
        /// </summary>
        public static QueryTimeout MediumDelete { get; set; } = new QueryTimeout(seconds: 300);

        /// <summary>
        /// Long select query timeout defaults to 1800 seconds
        /// </summary>
        public static QueryTimeout LongSelect { get; set; } = new QueryTimeout(seconds: 1800);

        /// <summary>
        /// Long insert query timeout defaults to 1800 seconds
        /// </summary>
        public static QueryTimeout LongInsert { get; set; } = new QueryTimeout(seconds: 1800);

        /// <summary>
        /// Long update query timeout defaults to 1800 seconds
        /// </summary>
        public static QueryTimeout LongUpdate { get; set; } = new QueryTimeout(seconds: 1800);

        /// <summary>
        /// Long delete query timeout defaults to 1800 seconds
        /// </summary>
        public static QueryTimeout LongDelete { get; set; } = new QueryTimeout(seconds: 1800);
    }
}