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
namespace QueryLite {

    /// <summary>
    /// Query timeout period measured in seconds
    /// </summary>
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
        public override string ToString() {
            return $"{Seconds} seconds";
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