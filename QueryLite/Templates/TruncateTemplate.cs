using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal sealed class TruncateQueryTemplate : ITruncate {

        public ITable Table { get; }

        public TruncateQueryTemplate(ITable table) {
            Table = table;
        }

        public NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null) {

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
                queryType: QueryType.Truncate
            );
        }

        public Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null) {

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
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
        }
    }
}