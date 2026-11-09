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

namespace QueryLite.Repository; 

public interface IRepositoryWith : IRepositoryWhere {

    /// <summary>
    /// The 'With' option only works on sql server. For other databases the query will ignore
    /// these table hints and execute without them.
    /// </summary>
    public IRepositoryJoin With(params SqlServerTableHint[] hints);
}

public interface IRepositoryJoin : IRepositoryWhere {

    /// <summary>
    /// Join table clause
    /// </summary>
    IRepositoryJoinOn Join(ITable table);

    /// <summary>
    /// Left join table clause
    /// </summary>
    IRepositoryJoinOn LeftJoin(ITable table);
}

public interface IRepositoryJoinOn {

    /// <summary>
    /// Join condition
    /// </summary>
    IRepositoryJoin On(ICondition on);
}

public interface IRepositoryWhere : IRepositoryOrderBy {

    /// <summary>
    /// Where condition clause
    /// </summary>
    IRepositoryOrderBy Where(ICondition? condition);
}

public interface IRepositoryOrderBy : IRepositoryFor {

    /// <summary>
    /// Order by clause
    /// </summary>
    IRepositoryFor OrderBy(params IOrderByColumn[] columns);
}

public interface IRepositoryFor : IRepositoryExecute {

    /// <summary>
    /// FOR clause. PostgreSql only
    /// </summary>
    IRepositoryExecute FOR(ForType forType, ITable[] ofTables, WaitType waitType);
}

public interface IRepositoryExecute {

    void Execute(IDatabase database);
    void Execute(IDatabase database, QueryTimeout timeout);
    void Execute(IDatabase database, QueryTimeout timeout, string debugName);

    void Execute(Transaction transaction);
    void Execute(Transaction transaction, QueryTimeout timeout);
    void Execute(Transaction transaction, QueryTimeout timeout, string debugName);

    Task ExecuteAsync(IDatabase database, CancellationToken ct);
    Task ExecuteAsync(IDatabase database, QueryTimeout timeout, CancellationToken ct);
    Task ExecuteAsync(IDatabase database, QueryTimeout timeout, string debugName, CancellationToken ct);

    Task ExecuteAsync(Transaction transaction, CancellationToken ct);
    Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, CancellationToken ct);
    Task ExecuteAsync(Transaction transaction, QueryTimeout timeout, string debugName, CancellationToken ct);
}