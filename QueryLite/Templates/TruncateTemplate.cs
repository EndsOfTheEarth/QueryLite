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

    internal sealed class TruncateQueryTemplate : ITruncate {

        public ITable Table { get; }

        public TruncateQueryTemplate(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            Table = table;
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            string sql = transaction.Database.TruncateGenerator.GetSql(this, transaction.Database, parameters: null);

            return QueryExecutor.ExecuteNonQuery(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: null,
                sql: sql,
                queryType: QueryType.Truncate,
                debugName: debugName
            );
        }

        public Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortDelete;
            }

            string sql = transaction.Database.TruncateGenerator.GetSql(this, transaction.Database, parameters: null);

            return QueryExecutor.ExecuteNonQueryAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: null,
                sql: sql,
                queryType: QueryType.Truncate,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
        }
    }
}