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


using System.Threading;

namespace QueryLite.Repository {

    //internal class RepositoryQueryTemplate<TABLE, ROW> : IRepositoryWith<TABLE, ROW>, IRepositoryWhere<TABLE, ROW>, IRepositoryOrderBy<TABLE, ROW>,
    //                                          IRepositoryFor<TABLE, ROW>, IRepositoryExecute<TABLE, ROW>
    //                                          where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

    //    private ARepository<TABLE, ROW> Repository { get; }
    //    private TABLE Table { get; }

    //    public RepositoryQueryTemplate(ARepository<TABLE, ROW> repository, TABLE table) {
    //        Repository = repository;
    //        Table = table;
    //    }

    //    public ICondition? Condition { get; private set; }
    //    public IOrderByColumn[]? OrderByColumns { get; private set; }
    //    public SqlServerTableHint[]? Hints { get; private set; }

    //    public ForType? ForType { get; internal set; } = null;
    //    public ITable[]? OfTables { get; internal set; } = null;
    //    public WaitType? WaitType { get; internal set; } = null;

    //    public IRepositoryWhere<TABLE, ROW> With(params SqlServerTableHint[] hints) {
    //        ArgumentNullException.ThrowIfNull(hints);
    //        Hints = hints;
    //        return this;
    //    }

    //    public IRepositoryOrderBy<TABLE, ROW> Where(ICondition? condition) {
    //        Condition = condition;
    //        return this;
    //    }

    //    public IRepositoryFor<TABLE, ROW> OrderBy(params IOrderByColumn[] columns) {
    //        ArgumentNullException.ThrowIfNull(columns);
    //        OrderByColumns = columns;
    //        return this;
    //    }

    //    public IRepositoryExecute<TABLE, ROW> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {

    //        ArgumentNullException.ThrowIfNull(ofTables);

    //        ForType = forType;
    //        OfTables = ofTables;
    //        WaitType = waitType;

    //        return this;
    //    }

    //    public void Execute(IDatabase database) {
    //        Execute(database, timeout: TimeoutLevel.ShortSelect, debugName: "");
    //    }

    //    public void Execute(IDatabase database, QueryTimeout timeout) {
    //        Execute(database, timeout, debugName: "");
    //    }

    //    public void Execute(IDatabase database, QueryTimeout timeout, string debugName) {

    //        ArgumentNullException.ThrowIfNull(database);

    //        Repository.ClearRows();

    //        IHint<ROW> q1 = Query
    //            .Select(row => ROW.LoadRow(Table, row))
    //            .From(Table);

    //        IGroupBy<ROW> q2;

    //        if(Hints is not null) {
    //            q2 = q1.With(Hints)
    //                .Where(Condition);
    //        }
    //        else {
    //            q2 = q1.Where(Condition);
    //        }

    //        IFor<ROW> q3 = OrderByColumns != null ? q2.OrderBy(OrderByColumns) : q2;

    //        IExecute<ROW> q4 = ForType is not null ? q3.FOR(ForType.Value, OfTables!, WaitType!.Value) : q3;

    //        QueryResult<ROW> result = q4.Execute(database, timeout, debugName: debugName);

    //        Repository.PopulateWithExistingRows(result.Rows);
    //    }

    //    public void Execute(Transaction transaction) {
    //        Execute(transaction, timeout: TimeoutLevel.ShortSelect, debugName: "");
    //    }

    //    public void Execute(Transaction transaction, QueryTimeout timeout) {
    //        Execute(transaction, timeout, debugName: "");
    //    }

    //    public void Execute(Transaction transaction, QueryTimeout timeout, string debugName) {

    //        ArgumentNullException.ThrowIfNull(transaction);

    //        Repository.ClearRows();

    //        IHint<ROW> q1 = Query
    //            .Select(row => ROW.LoadRow(Table, row))
    //            .From(Table);

    //        IGroupBy<ROW> q2;

    //        if(Hints is not null) {
    //            q2 = q1.With(Hints)
    //                .Where(Condition);
    //        }
    //        else {
    //            q2 = q1.Where(Condition);
    //        }

    //        IFor<ROW> q3 = OrderByColumns != null ? q2.OrderBy(OrderByColumns) : q2;

    //        IExecute<ROW> q4 = ForType is not null ? q3.FOR(ForType.Value, OfTables!, WaitType!.Value) : q3;

    //        QueryResult<ROW> result = q4.Execute(transaction, timeout, debugName: debugName);

    //        Repository.PopulateWithExistingRows(result.Rows);
    //    }

    //    public async Task ExecuteAsync(IDatabase database, CancellationToken cancellationToken) {
    //        await ExecuteAsync(database, timeout: TimeoutLevel.ShortSelect, debugName: "", cancellationToken);
    //    }

    //    public async Task ExecuteAsync(IDatabase database, QueryTimeout timeout, CancellationToken cancellationToken) {
    //        await ExecuteAsync(database, timeout, debugName: "", cancellationToken);
    //    }

    //    public async Task ExecuteAsync(IDatabase database, QueryTimeout timeout, string debugName, CancellationToken cancellationToken) {

    //        ArgumentNullException.ThrowIfNull(database);

    //        Repository.ClearRows();

    //        IHint<ROW> q1 = Query
    //            .Select(row => ROW.LoadRow(Table, row))
    //            .From(Table);

    //        IGroupBy<ROW> q2;

    //        if(Hints is not null) {
    //            q2 = q1.With(Hints)
    //                .Where(Condition);
    //        }
    //        else {
    //            q2 = q1.Where(Condition);
    //        }

    //        IFor<ROW> q3 = OrderByColumns != null ? q2.OrderBy(OrderByColumns) : q2;

    //        IExecute<ROW> q4 = ForType is not null ? q3.FOR(ForType.Value, OfTables!, WaitType!.Value) : q3;

    //        QueryResult<ROW> result = await q4.ExecuteAsync(database, cancellationToken, timeout, debugName: debugName);

    //        Repository.PopulateWithExistingRows(result.Rows);
    //    }

    //    public async Task ExecuteAsync(Transaction transaction, CancellationToken cancellationToken) {
    //        await ExecuteAsync(transaction, timeout: TimeoutLevel.ShortSelect, debugName: "", cancellationToken);
    //    }

    //    public async Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken) {
    //        await ExecuteAsync(transaction, timeout, debugName: "", cancellationToken);
    //    }

    //    public async Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken cancellationToken) {

    //        ArgumentNullException.ThrowIfNull(transaction);

    //        Repository.ClearRows();

    //        IHint<ROW> q1 = Query
    //            .Select(row => ROW.LoadRow(Table, row))
    //            .From(Table);

    //        IGroupBy<ROW> q2;

    //        if(Hints is not null) {
    //            q2 = q1.With(Hints)
    //                .Where(Condition);
    //        }
    //        else {
    //            q2 = q1.Where(Condition);
    //        }

    //        IFor<ROW> q3 = OrderByColumns != null ? q2.OrderBy(OrderByColumns) : q2;

    //        IExecute<ROW> q4 = ForType is not null ? q3.FOR(ForType.Value, OfTables!, WaitType!.Value) : q3;

    //        QueryResult<ROW> result = await q4.ExecuteAsync(transaction, cancellationToken, timeout, debugName: debugName);

    //        Repository.PopulateWithExistingRows(result.Rows);
    //    }
    //}





    internal class RepositoryQueryTemplate<TABLE, ROW> : IRepositoryWith<TABLE, ROW>,
                                                IRepositoryJoin<TABLE, ROW>, IRepositoryWhere<TABLE, ROW>,
                                                IRepositoryOrderBy<TABLE, ROW>,
                                                IRepositoryFor<TABLE, ROW>, IRepositoryExecute<TABLE, ROW>
                                                where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        private ARepository<TABLE, ROW> Repository { get; }
        private TABLE Table { get; }

        private SelectQueryTemplate<ROW> QueryTemplate { get; }

        public RepositoryQueryTemplate(ARepository<TABLE, ROW> repository, TABLE table) {
            Repository = repository;
            Table = table;
            QueryTemplate = new SelectQueryTemplate<ROW>(row => ROW.LoadRow(Table, row));
            QueryTemplate.Distinct.From(table);
        }

        public IRepositoryJoin<TABLE, ROW> With(params SqlServerTableHint[] hints) {
            QueryTemplate.With(hints);
            return this;
        }

        public IRepositoryJoinOn<TABLE, ROW> Join(ITable table) {
            return new RepositoryQueryJoinOn<TABLE, ROW>(this, QueryTemplate.Join(table));
        }

        public IRepositoryJoinOn<TABLE, ROW> LeftJoin(ITable table) {
            return new RepositoryQueryJoinOn<TABLE, ROW>(this, QueryTemplate.LeftJoin(table));
        }

        public IRepositoryExecute<TABLE, ROW> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {
            QueryTemplate.FOR(forType, ofTables, waitType);
            return this;
        }

        public IRepositoryOrderBy<TABLE, ROW> Where(ICondition? condition) {
            QueryTemplate.Where(condition);
            return this;
        }

        public IRepositoryFor<TABLE, ROW> OrderBy(params IOrderByColumn[] columns) {
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

            QueryResult<ROW> result = QueryTemplate.Execute(database, timeout, debugName: debugName);

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

            QueryResult<ROW> result = QueryTemplate.Execute(transaction, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }

        public Task ExecuteAsync(IDatabase database, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(IDatabase database, QueryTimeout timeout, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task ExecuteAsync(IDatabase database, QueryTimeout timeout, string debugName, CancellationToken cancellationToken) {

            ArgumentNullException.ThrowIfNull(database);

            Repository.ClearRows();

            QueryResult<ROW> result = await QueryTemplate.ExecuteAsync(database, cancellationToken, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }

        public Task ExecuteAsync(Transaction transaction, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken cancellationToken) {

            ArgumentNullException.ThrowIfNull(transaction);

            Repository.ClearRows();

            QueryResult<ROW> result = await QueryTemplate.ExecuteAsync(transaction, cancellationToken, timeout, debugName: debugName);

            Repository.PopulateWithExistingRows(result.Rows);
        }
    }

    internal class RepositoryQueryJoinOn<TABLE, ROW> : IRepositoryJoinOn<TABLE, ROW>
                                        where TABLE : ATable where ROW : class, IRow<TABLE, ROW>, IEquatable<ROW> {

        private IRepositoryJoin<TABLE, ROW> Template { get; }
        private IJoinOn<ROW> JoinOn { get; }

        public RepositoryQueryJoinOn(IRepositoryJoin<TABLE, ROW> template, IJoinOn<ROW> joinOn) {
            Template = template;
            JoinOn = joinOn;
        }
        public IRepositoryJoin<TABLE, ROW> On(ICondition on) {
            JoinOn.On(on);
            return Template;
        }
    }
}