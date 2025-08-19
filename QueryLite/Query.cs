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

    public static class Query {

        /// <summary>
        /// Nested query
        /// </summary>
        public static IDistinct<FIELD> NestedSelect<FIELD>(FIELD field) where FIELD : IField {
            return new SelectQueryTemplate<FIELD>(new List<IField>() { field });
        }

        /// <summary>
        /// Sql select query
        /// </summary>
        public static IDistinct<RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc) => new SelectQueryTemplate<RESULT>(selectFunc);

        /// <summary>
        /// Create a prepared query
        /// </summary>
        /// <typeparam name="PARAMETERS"></typeparam>
        /// <returns></returns>
        public static IPreparedOption<PARAMETERS> Prepare<PARAMETERS>() => new PreparedOption<PARAMETERS>();

        public interface IPreparedOption<PARAMETERS> {

            IPreparedDistinct<PARAMETERS, RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc);
            IPreparedInsertSet<PARAMETERS> Insert(ITable table);
            IPreparedUpdateSet<PARAMETERS> Update(ITable table);
            IPreparedDeleteFrom<PARAMETERS> Delete(ITable table);
        }

        internal sealed class PreparedOption<PARAMETERS> : IPreparedOption<PARAMETERS> {

            public IPreparedDistinct<PARAMETERS, RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc) => new PreparedQueryTemplate<PARAMETERS, RESULT>(selectFunc);

            public IPreparedInsertSet<PARAMETERS> Insert(ITable table) => new PreparedInsertTemplate<PARAMETERS>(table);

            public IPreparedUpdateSet<PARAMETERS> Update(ITable table) => new PreparedUpdateTemplate<PARAMETERS>(table);

            public IPreparedDeleteFrom<PARAMETERS> Delete(ITable table) => new PreparedDeleteQueryTemplate<PARAMETERS>(table);
        }

        /// <summary>
        /// Sql insert query
        /// </summary>
        public static IInsertSet Insert(ITable table) {
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
        public static IDeleteFrom Delete(ITable table) {
            return new DeleteQueryTemplate(table);
        }

        /// <summary>
        /// Sql truncate query
        /// </summary>
        public static ITruncate Truncate(ITable table) {
            return new TruncateQueryTemplate(table);
        }
    }
}