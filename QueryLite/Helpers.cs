/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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

    public static class Helpers {

        /// <summary>
        /// Escape string text for use in an sql query
        /// </summary>
        /// <param name="text">Text to be escaped</param>
        /// <returns></returns>
        public static string EscapeForSql(string text) {
            return text.Replace("'", "''");
        }
    }

    public static class StringExtensions {

        /// <summary>
        /// a Equals b, OrdinalIgnoreCase
        /// </summary>
        public static bool EqualsOIC(this string a, string b) {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// a Contains b, OrdinalIgnoreCase
        /// </summary>
        public static bool ContainsOIC(this string a, string b) {
            return a.Contains(b, StringComparison.OrdinalIgnoreCase);
        }
    }

    // The type name only contains lower-cased ascii characters.
    // Naming Styles
    #pragma warning disable CS8981, IDE1006
    public static class str {
    #pragma warning restore IDE1006,CS8981

        /// <summary>
        /// a Equals b, OrdinalIgnoreCase
        /// </summary>
        public static bool EqualsOIC(string a, string b) {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrEmpty(string? value) {
            return string.IsNullOrEmpty(value);
        }
        public static bool IsNullOrWhiteSpace(string? value) {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}   