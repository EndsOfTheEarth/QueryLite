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
using System.Diagnostics;

namespace QueryLite {

    public interface ITable {

        /// <summary>
        /// Table sql alias used for query generation
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Schema table belongs to
        /// </summary>
        string SchemaName { get; }

        /// <summary>
        /// Name of table in database
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Is this table class representing a view.
        /// </summary>
        bool IsView { get; }

        /// <summary>
        /// Enclose table name in sql query. For example enclose table name with [] (square brackets) in sql server queries.
        /// </summary>
        bool Enclose { get; }

        PrimaryKey? PrimaryKey { get; }

        UniqueConstraint[] UniqueConstraints { get; }

        ForeignKey[] ForeignKeys { get; }
    }

    /// <summary>
    /// This class is used to generate an alias for each instance of a table class. This alias is used when generating sql queries.
    /// </summary>
    internal static class AliasGenerator {

        private static volatile uint counter = 0;
        private readonly static object lock_ = new object();

        public static string GetAlias() {

            uint alias;

            lock(lock_) {
                alias = counter++;
            }
            return alias.ToString("X");
        }
    }

    /// <summary>
    /// Represents a database table
    /// </summary>
    [DebuggerDisplay("Schema: {SchemaName}, Table Name: {TableName}, Alias: {Alias}")]
    public abstract class ATable : ITable {

        public string SchemaName { get; }
        public string TableName { get; }
        public bool Enclose { get; }
        public string Alias { get; }
        public bool IsView { get; }

        public virtual PrimaryKey? PrimaryKey => null;
        public virtual UniqueConstraint[] UniqueConstraints => Array.Empty<UniqueConstraint>();
        public virtual ForeignKey[] ForeignKeys => Array.Empty<ForeignKey>();

        /// <summary>
        /// Abstract table
        /// </summary>
        /// <param name="tableName">Name of table in database</param>
        /// <param name="schemaName">Schema table belongs to</param>
        /// <param name="enclose">Set to true if the table name is a sql keyword. This will stop the key word from causing invalid sql from being created</param>
        /// <exception cref="ArgumentException"></exception>
        protected ATable(string tableName, string schemaName, bool enclose = false, bool isView = false) {

            ArgumentException.ThrowIfNullOrEmpty(tableName);
            ArgumentException.ThrowIfNullOrEmpty(schemaName);

            SchemaName = schemaName;
            TableName = tableName;
            Enclose = enclose;
            Alias = "_" + AliasGenerator.GetAlias();
            IsView = isView;
        }
    }
}