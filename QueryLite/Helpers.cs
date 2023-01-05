
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
}