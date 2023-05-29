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

    internal class PreparedInsertTemplate<PARAMETERS> : IPreparedInsertSet<PARAMETERS>, IIPreparedInsertReturning<PARAMETERS> {


        public ITable Table { get; }
        public Action<IPreparedSetValuesCollector<PARAMETERS>>? SetValues { get; private set; }

        public PreparedInsertTemplate(ITable table) {
            Table = table;
        }

        public IIPreparedInsertReturning<PARAMETERS> Values(Action<IPreparedSetValuesCollector<PARAMETERS>> values) {
            SetValues = values;
            return this;
        }

        public IPreparedInsertBuild<RESULT> Returning<RESULT>(Func<IResultRow, RESULT>? returning) {
            return new PreparedInsertTemplate<PARAMETERS, RESULT>(Table, SetValues!, returning);
        }

        public IPreparedInsertQuery<PARAMETERS> Build() {
            throw new NotImplementedException();
        }
    }

    internal class PreparedInsertTemplate<PARAMETERS, RESULT> : IPreparedInsertBuild<RESULT> {

        public ITable Table { get; }
        public Action<IPreparedSetValuesCollector<PARAMETERS>>? SetValues { get; private set; }
        public Func<IResultRow, RESULT>? Returning { get; }

        public PreparedInsertTemplate(ITable table, Action<IPreparedSetValuesCollector<PARAMETERS>>? setValues, Func<IResultRow, RESULT>? returning) {
            Table = table;
            SetValues = setValues;
            Returning = returning;
        }

        public IPreparedInsertQuery<RESULT> Build() {
            throw new NotImplementedException();
        }
    }
}