using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    public interface IDeleteJoin : IDeleteWhere {

        /// <summary>
        /// Delete join syntax. Please Note: This syntax is only supported on Sql Server
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IDeleteJoinOn Join(ITable table);

        /// <summary>
        /// Delete left join syntax. Please Note: This syntax is only supported on Sql Server
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IDeleteJoinOn LeftJoin(ITable table);
    }
    public interface IDeleteJoinOn {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IDeleteJoin On(ICondition on);
    }

    public interface IDeleteWhere {

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IDeleteExecute Where(ICondition condition);

        /// <summary>
        /// Explicitly state that there is no where clause. For code safety purposes the 'NoWhereCondition()' method must be used when there is no where clause on a delete query.
        /// </summary>
        /// <returns></returns>
        public IDeleteExecute NoWhereCondition();
    }

    internal sealed class DeleteJoin : IJoin, IDeleteJoinOn {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        public ICondition? _condition;

        public ICondition Condition {
            get { return _condition!; }
        }

        private readonly DeleteQueryTemplate Template;

        internal DeleteJoin(JoinType joinType, ITable table, DeleteQueryTemplate tempate) {
            JoinType = joinType;
            Table = table;
            Template = tempate;
        }

        public IDeleteJoin On(ICondition on) {
            _condition = on;
            return Template;
        }
    }

    public interface IDeleteExecute {

        /// <summary>
        /// Get delete sql
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        string GetSql(IDatabase database);

        NonQueryResult Execute(Transaction transaction, QueryTimeout? timeout = null,  Parameters useParameters = Parameters.Default);
        QueryResult<RESULT> Execute<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);

        Task<NonQueryResult> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        Task<QueryResult<RESULT>> ExecuteAsync<RESULT>(Func<IResultRow, RESULT> func, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
    }
}