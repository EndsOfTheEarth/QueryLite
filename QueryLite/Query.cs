using System;
using System.Collections.Generic;

namespace QueryLite {

    public static class Query {

        /// <summary>
        /// Nested query
        /// </summary>
        public static ITop<FIELD> NestedSelect<FIELD>(FIELD field) where FIELD : IField {
            return new SelectQueryTemplate<FIELD>(new List<IField>(1) { field });
        }

        /// <summary>
        /// Sql select query
        /// </summary>
        public static ITop<RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc) => new SelectQueryTemplate<RESULT>(selectFunc);

        /// <summary>
        /// Sql insert query
        /// </summary>
        public static IInsertSet InsertInto(ITable table) {
            return new InsertQueryTemplate(table);
        }

        /// <summary>
        /// Sql update query
        /// </summary>
        public static IUpdateSet Update(ITable table) {
            return new UpdateQueryTemplate(table);
        }

        /// <summary>
        /// Sql delete query
        /// </summary>
        public static IDeleteJoin DeleteFrom(ITable table) {
            return new DeleteQueryTemplate(table);
        }

        /// <summary>
        /// Sql truncate query
        /// </summary>
        public static ITruncate TruncateTable(ITable table) {
            return new TruncateQueryTemplate(table);
        }
    }
}