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
using QueryLite.PreparedQuery;
using System;
using System.Data.Common;

namespace QueryLite {

    public interface IDatabase : IInternalConnection {
        /// <summary>
        /// Descriptive name of database
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Database type
        /// </summary>
        public DatabaseType DatabaseType { get; }

        /// <summary>
        /// Function to map between schema names when using more than one type of database. For example: the Sql Server schema might be 'dbo' but it is 'public' in PostgreSql.
        /// </summary>
        Func<string, string> SchemaMap { get; }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns></returns>
        DbConnection GetNewConnection();
        public string ConvertToSql(object value);
        public IParametersBuilder CreateParameters(int initParams);
        string? GetCSharpCodeSet(Type dotNetType);
    }

    public enum DatabaseType {
        SqlServer = 0,  //Note: These integer values are used by the prepared query functionality
        PostgreSql = 1
    }

    public interface IInternalConnection {

        internal IQueryGenerator QueryGenerator { get; }
        internal IInsertQueryGenerator InsertGenerator { get; }
        internal IPreparedInsertQueryGenerator PreparedInsertGenerator { get; }
        internal IUpdateQueryGenerator UpdateGenerator { get; }
        internal IDeleteQueryGenerator DeleteGenerator { get; }
        internal ITruncateQueryGenerator TruncateGenerator { get; }
        internal IPreparedQueryGenerator PreparedQueryGenerator { get; }
        internal IParameterMapper ParameterMapper { get; }
    }
}