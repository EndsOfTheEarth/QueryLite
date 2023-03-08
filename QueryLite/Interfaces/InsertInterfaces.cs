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
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {
    
    public interface IInsertSet {

        IInsertSetNext Set<TYPE>(Column<TYPE> column, TYPE value) where TYPE : notnull;
        IInsertSetNext Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : class;
        IInsertSetNext Set<TYPE>(NullableColumn<TYPE> column, TYPE? value) where TYPE : struct;
        IInsertSetNext Set<TYPE>(Column<TYPE> column, AFunction<TYPE> function) where TYPE : notnull;
    }

    public interface IInsertSetNext : IInsertSet, IInsertExecute {

    }
    public interface IInsertExecute {

        string GetSql(IDatabase database);

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "");
    }
}