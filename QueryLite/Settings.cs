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
using System;

namespace QueryLite {

    public static class Settings {

        /// <summary>
        /// Turns query parameters on or off. ie. False = use plain sql text instead of parameter in sql query. This is good for debugging purposes.
        /// </summary>
        public static bool UseParameters { get; set; } = true;

        /// <summary>
        /// If true this causes code to stop on a break point when a select query is executed in debug mode. This is a debugging feature.
        /// </summary>
        public static bool BreakOnSelectQuery { get; set; }

        /// <summary>
        /// If true this causes code to stop on a break point when a insert query is executed in debug mode. This is a debugging feature.
        /// </summary>
        public static bool BreakOnInsertQuery { get; set; }

        /// <summary>
        /// If true this causes code to stop on a break point when a update query is executed in debug mode. This is a debugging feature.
        /// </summary>
        public static bool BreakOnUpdateQuery { get; set; }

        /// <summary>
        /// If true this causes code to stop on a break point when a delete query is executed in debug mode. This is a debugging feature.
        /// </summary>
        public static bool BreakOnDeleteQuery { get; set; }

        /// <summary>
        /// If true this causes code to stop on a break point when a truncate query is executed in debug mode. This is a debugging feature.
        /// </summary>
        public static bool BreakOnTruncateQuery { get; set; }

        internal static bool HasEvents => QueryExecuting != null || QueryPerformed != null;

        public delegate void QueryExecutingDelegate(IDatabase database, string sql, QueryType queryType, DateTimeOffset? start, System.Data.IsolationLevel isolationLevel, ulong? transactionId);

        /// <summary>
        /// Event fired when a query begins execution
        /// </summary>
        public static event QueryExecutingDelegate? QueryExecuting;

        internal static void FireQueryExecutingEvent(IDatabase database, string sql, QueryType queryType, DateTimeOffset? start, System.Data.IsolationLevel isolationLevel, ulong? transactionId) {
            try {

                QueryExecuting?.Invoke(
                    database: database,
                    sql: sql,
                    queryType: queryType,
                    start: start,
                    isolationLevel: isolationLevel,
                    transactionId: transactionId
                );
            }
            catch { }
        }

        public delegate void QueryPerformedDelegate(IDatabase database, string sql, int rows, int rowsEffected, QueryType queryType, DateTimeOffset? start, DateTimeOffset? end, TimeSpan? elapsedTime, Exception? exception, System.Data.IsolationLevel isolationLevel, ulong? transactionId);

        /// <summary>
        /// Event fired when a query completes execution or throws an exception
        /// </summary>
        public static event QueryPerformedDelegate? QueryPerformed;

        internal static void FireQueryPerformedEvent(IDatabase database, string sql, int rows, int rowsEffected, QueryType queryType, DateTimeOffset? start, DateTimeOffset? end, TimeSpan? elapsedTime, Exception? exception, System.Data.IsolationLevel isolationLevel, ulong? transactionId) {
            try {


                QueryPerformed?.Invoke(
                    database: database,
                    sql: sql,
                    rows: rows,
                    rowsEffected: rowsEffected,
                    queryType: queryType,
                    start: start,
                    end: end,
                    elapsedTime: elapsedTime,
                    exception: exception,
                    isolationLevel: isolationLevel,
                    transactionId: transactionId
                );
            }
            catch { }
        }
    }
}