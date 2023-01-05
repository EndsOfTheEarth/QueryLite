using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    public interface ITruncate {

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null);
        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null);
    }
}