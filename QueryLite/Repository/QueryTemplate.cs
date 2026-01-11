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

namespace QueryLite.Repository {

    internal class RepositoryQueryTemplate<TABLE, ROW> : IRepositoryWith,
                                                IRepositoryJoin, IRepositoryWhere,
                                                IRepositoryOrderBy,
                                                IRepositoryFor, IRepositoryExecute
                                                where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        private ARepository<TABLE, ROW> Repository { get; }
        private TABLE Table { get; }

        private SelectQueryTemplate<RowState<ROW>> QueryTemplate { get; }

        public RepositoryQueryTemplate(ARepository<TABLE, ROW> repository, TABLE table) {
            Repository = repository;
            Table = table;
            QueryTemplate = new SelectQueryTemplate<RowState<ROW>>(
                row => {

                    ROW r = ROW.LoadRow(Table, row);

                    return new RowState<ROW>(
                        state: RowUpdateState.Existing,
                        oldRow: ROW.CloneRow(r),
                        newRow: r
                    );
                }
            );
            QueryTemplate.Distinct.From(table);
        }

        public IRepositoryJoin With(params SqlServerTableHint[] hints) {
            QueryTemplate.With(hints);
            return this;
        }

        public IRepositoryJoinOn Join(ITable table) {
            return new RepositoryQueryJoinOn<RowState<ROW>>(this, QueryTemplate.Join(table));
        }

        public IRepositoryJoinOn LeftJoin(ITable table) {
            return new RepositoryQueryJoinOn<RowState<ROW>>(this, QueryTemplate.LeftJoin(table));
        }

        public IRepositoryExecute FOR(ForType forType, ITable[] ofTables, WaitType waitType) {
            QueryTemplate.FOR(forType, ofTables, waitType);
            return this;
        }

        public IRepositoryOrderBy Where(ICondition? condition) {
            QueryTemplate.Where(condition);
            return this;
        }

        public IRepositoryFor OrderBy(params IOrderByColumn[] columns) {
            QueryTemplate.OrderBy(columns);
            return this;
        }

        public void Execute(IDatabase database) {
            Execute(database, TimeoutLevel.ShortSelect, debugName: "");
        }

        public void Execute(IDatabase database, QueryTimeout timeout) {
            Execute(database, timeout, debugName: "");
        }

        public void Execute(IDatabase database, QueryTimeout timeout, string debugName) {

            ArgumentNullException.ThrowIfNull(database);

            Repository.ClearRows();

            QueryResult<RowState<ROW>> result = QueryTemplate.Execute(database, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }

        public void Execute(Transaction transaction) {
            Execute(transaction, TimeoutLevel.ShortSelect, debugName: "");
        }

        public void Execute(Transaction transaction, QueryTimeout timeout) {
            Execute(transaction, timeout, debugName: "");
        }

        public void Execute(Transaction transaction, QueryTimeout timeout, string debugName) {

            ArgumentNullException.ThrowIfNull(transaction);

            Repository.ClearRows();

            QueryResult<RowState<ROW>> result = QueryTemplate.Execute(transaction, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }

        public async Task ExecuteAsync(IDatabase database, CancellationToken ct) {
            await ExecuteAsync(database, TimeoutLevel.ShortSelect, debugName: "", ct);
        }

        public async Task ExecuteAsync(IDatabase database, QueryTimeout timeout, CancellationToken ct) {
            await ExecuteAsync(database, timeout, debugName: "", ct);
        }

        public async Task ExecuteAsync(IDatabase database, QueryTimeout timeout, string debugName, CancellationToken ct) {

            ArgumentNullException.ThrowIfNull(database);

            Repository.ClearRows();

            QueryResult<RowState<ROW>> result = await QueryTemplate.ExecuteAsync(database, ct, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }

        public async Task ExecuteAsync(Transaction transaction, CancellationToken ct) {
            await ExecuteAsync(transaction, TimeoutLevel.ShortSelect, debugName: "", ct);
        }

        public async Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct) {
            await ExecuteAsync(transaction, timeout, debugName: "", ct);
        }

        public async Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct) {

            ArgumentNullException.ThrowIfNull(transaction);

            Repository.ClearRows();

            QueryResult<RowState<ROW>> result = await QueryTemplate.ExecuteAsync(transaction, ct, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }
    }

    internal class RepositoryQueryJoinOn<RESULT> : IRepositoryJoinOn {

        private IRepositoryJoin Template { get; }
        private IJoinOn<RESULT> JoinOn { get; }

        public RepositoryQueryJoinOn(IRepositoryJoin template, IJoinOn<RESULT> joinOn) {
            Template = template;
            JoinOn = joinOn;
        }
        public IRepositoryJoin On(ICondition on) {
            JoinOn.On(on);
            return Template;
        }
    }
}