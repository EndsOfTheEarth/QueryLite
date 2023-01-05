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

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
    }
}