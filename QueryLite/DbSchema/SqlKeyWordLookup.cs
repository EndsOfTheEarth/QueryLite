﻿/*
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
using System.Collections.Generic;

namespace QueryLite.DbSchema {

    /// <summary>
    /// Stores a list of sql keywords for Sql Server and PostgreSql databases
    /// </summary>
    public static class SqlKeyWordLookup {

        private static readonly HashSet<string> KeyWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "ADD",
            "ALL",
            "ALTER",
            "ANALYSE",
            "ANALYZE",
            "AND",
            "ANY",
            "ARRAY",
            "AS",
            "ASC",
            "ASYMMETRIC",
            "AUTHORIZATION",
            "BACKUP",
            "BEGIN",
            "BETWEEN",
            "BINARY",
            "BOTH",
            "BREAK",
            "BROWSE",
            "BULK",
            "BY",
            "CASCADE",
            "CASE",
            "CAST",
            "CHECK",
            "CHECKPOINT",
            "CLOSE",
            "CLUSTERED",
            "COALESCE",
            "COLLATE",
            "COLLATION",
            "COLUMN",
            "COMMIT",
            "COMPUTE",
            "CONCURRENTLY",
            "CONSTRAINT",
            "CONTAINS",
            "CONTAINSTABLE",
            "CONTINUE",
            "CONVERT",
            "CREATE",
            "CROSS",
            "CURRENT",
            "CURRENT_DATE",
            "CURRENT_ROLE",
            "CURRENT_SCHEMA",
            "CURRENT_TIME",
            "CURRENT_TIMESTAMP",
            "CURRENT_USER",
            "CURSOR",
            "DATABASE",
            "DAY",
            "DBCC",
            "DEALLOCATE",
            "DECLARE",
            "DEFAULT",
            "DEFERRABLE",
            "DELETE",
            "DENY",
            "DESC",
            "DISK",
            "DISTINCT",
            "DISTRIBUTED",
            "DO",
            "DOUBLE",
            "DROP",
            "DUMP",
            "ELSE",
            "END",
            "ERRLVL",
            "ESCAPE",
            "EXCEPT",
            "EXEC",
            "EXECUTE",
            "EXISTS",
            "EXIT",
            "EXTERNAL",
            "FALSE",
            "FETCH",
            "FETCH",
            "FILE",
            "FILLFACTOR",
            "FOR",
            "FOREIGN",
            "FREETEXT",
            "FREETEXTTABLE",
            "FREEZE",
            "FROM",
            "FULL",
            "FUNCTION",
            "GOTO",
            "GRANT",
            "GROUP",
            "HAVING",
            "HOLDLOCK",
            "IDENTITY",
            "IDENTITY_INSERT",
            "IDENTITYCOL",
            "IF",
            "ILIKE",
            "IN",
            "INDEX",
            "INITIALLY",
            "INNER",
            "INSERT",
            "INTERSECT",
            "INTO",
            "IS",
            "ISNULL",
            "JOIN",
            "KEY",
            "KILL",
            "LATERAL",
            "LEADING",
            "LEFT",
            "LIKE",
            "LIMIT",
            "LINENO",
            "LOAD",
            "LOCALTIME",
            "LOCALTIMESTAMP",
            "MERGE",
            "MONTH",
            "NATIONAL",
            "NATURAL",
            "NOCHECK",
            "NONCLUSTERED",
            "NOT",
            "NOTNULL",
            "NULL",
            "NULLIF",
            "OF",
            "OFF",
            "OFFSET",
            "OFFSETS",
            "ON",
            "ONLY",
            "OPEN",
            "OPENDATASOURCE",
            "OPENQUERY",
            "OPENROWSET",
            "OPENXML",
            "OPTION",
            "OR",
            "ORDER",
            "OUTER",
            "OVER",
            "OVERLAPS",
            "PERCENT",
            "PIVOT",
            "PLACING",
            "PLAN",
            "PRECISION",
            "PRIMARY",
            "PRINT",
            "PROC",
            "PROCEDURE",
            "PUBLIC",
            "RAISERROR",
            "READ",
            "READTEXT",
            "RECONFIGURE",
            "REFERENCES",
            "REPLICATION",
            "RESTORE",
            "RESTRICT",
            "RETURN",
            "RETURNING",
            "REVERT",
            "REVOKE",
            "RIGHT",
            "ROLLBACK",
            "ROWCOUNT",
            "ROWGUIDCOL",
            "RULE",
            "SAVE",
            "SCHEMA",
            "SECURITYAUDIT",
            "SELECT",
            "SEMANTICKEYPHRASETABLE",
            "SEMANTICSIMILARITYDETAILSTABLE",
            "SEMANTICSIMILARITYTABLE",
            "SESSION_USER",
            "SET",
            "SETUSER",
            "SHUTDOWN",
            "SIMILAR",
            "SOME",
            "STATISTICS",
            "SYMMETRIC",
            "SYSTEM_USER",
            "TABLE",
            "TABLESAMPLE",
            "TEXTSIZE",
            "THEN",
            "TO",
            "TOP",
            "TRAILING",
            "TRAN",
            "TRANSACTION",
            "TRIGGER",
            "TRUE",
            "TRUNCATE",
            "TRY_CONVERT",
            "TSEQUAL",
            "UNION",
            "UNIQUE",
            "UNPIVOT",
            "UPDATE",
            "UPDATETEXT",
            "USE",
            "USER",
            "USING",
            "VALUES",
            "VARIADIC",
            "VARYING",
            "VERBOSE",
            "VIEW",
            "WAITFOR",
            "WHEN",
            "WHERE",
            "WHILE",
            "WINDOW",
            "WITH",
            "WITHIN GROUP",
            "WRITETEXT",
            "YEAR"
        };

        public static bool IsKeyWord(string text) {
            return KeyWords.Contains(text);
        }
    }
}