using QueryLite.PreparedQuery;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    public interface IPreparedDistinct<PARAMETERS, RESULT> : IPreparedTop<PARAMETERS, RESULT> {

        IPreparedTop<PARAMETERS, RESULT> Distinct { get; }
    }

    public interface IPreparedTop<PARAMETERS, RESULT> : IPreparedFrom<PARAMETERS, RESULT> {

        /// <summary>
        /// Return TOP n rows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IPreparedFrom<PARAMETERS, RESULT> Top(int rows);
    }
    public interface IPreparedFrom<PARAMETERS, RESULT> {

        /// <summary>
        /// From table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedHint<PARAMETERS, RESULT> From(ITable table);
    }

    public interface IPreparedHint<PARAMETERS, RESULT> : IPreparedJoin<PARAMETERS, RESULT> {

        /// <summary>
        /// The 'With' option only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public IPreparedJoin<PARAMETERS, RESULT> With(params SqlServerTableHint[] hints);
    }

    public interface IPreparedJoin<PARAMETERS, RESULT> : IPreparedWhere<PARAMETERS, RESULT> {

        /// <summary>
        /// Join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedJoinOn<PARAMETERS, RESULT> Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedJoinOn<PARAMETERS, RESULT> LeftJoin(ITable table);
    }

    public interface IPreparedJoin<PARAMETERS> {

        internal JoinType JoinType { get; }
        internal ITable Table { get; }
        internal APreparedCondition<PARAMETERS> Condition { get; }
    }

    public interface IPreparedJoinOn<PARAMETERS, RESULT> : IPreparedJoin<PARAMETERS> {

        IPreparedJoin<PARAMETERS, RESULT> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on);
    }
    internal sealed class PreparedJoin<PARAMETERS, RESULT> : IPreparedJoinOn<PARAMETERS, RESULT> {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        private APreparedCondition<PARAMETERS>? _condition;

        public APreparedCondition<PARAMETERS> Condition {
            get { return _condition!; }
        }

        private readonly PreparedQueryTemplate<PARAMETERS, RESULT> Template;

        internal PreparedJoin(JoinType joinType, ITable table, PreparedQueryTemplate<PARAMETERS, RESULT> template) {
            JoinType = joinType;
            Table = table;
            Template = template;
        }
        public IPreparedJoin<PARAMETERS, RESULT> On(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>> on) {
            _condition = on(new EmptyPreparedCondition<PARAMETERS>());
            return Template;
        }
    }

    public interface IPreparedWhere<PARAMETERS, RESULT> : IPreparedGroupBy<PARAMETERS, RESULT> {

        /// <summary>
        /// Where condition clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IPreparedGroupBy<PARAMETERS, RESULT> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>>? condition);
    }

    public interface IPreparedGroupBy<PARAMETERS, RESULT> : IPreparedHaving<PARAMETERS, RESULT> {

        /// <summary>
        /// Group by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IPreparedHaving<PARAMETERS, RESULT> GroupBy(params ISelectable[] columns);
    }

    public interface IPreparedHaving<PARAMETERS, RESULT> : IPreparedOrderBy<PARAMETERS, RESULT> {

        /// <summary>
        /// Having clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IPreparedOrderBy<PARAMETERS, RESULT> Having(APreparedCondition<PARAMETERS> condition);
    }

    public interface IPreparedOrderBy<PARAMETERS, RESULT> : IPreparedFor<PARAMETERS, RESULT> {

        /// <summary>
        /// Order by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IPreparedFor<PARAMETERS, RESULT> OrderBy(params IOrderByColumn[] columns);

        /// <summary>
        /// Union query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        IPreparedDistinct<PARAMETERS, RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc);

        /// <summary>
        /// Union all query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        IPreparedDistinct<PARAMETERS, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc);
    }

    public interface IPreparedFor<PARAMETERS, RESULT> : IPreparedOption<PARAMETERS, RESULT> {

        /// <summary>
        /// FOR clause. PostgreSql only
        /// </summary>
        /// <param name="forType"></param>
        /// <param name="ofTables"></param>
        /// <param name="waitType"></param>
        /// <returns></returns>
        IPreparedOption<PARAMETERS, RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType);
    }

    public interface IPreparedOption<PARAMETERS, RESULT> : ICompileQuery<PARAMETERS, RESULT> {

        /// <summary>
        /// Sql server OPTION syntax. Note: 'Option' only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        ICompileQuery<PARAMETERS, RESULT> Option(params SqlServerQueryOption[] options);

        /// <summary>
        /// Sql server OPTION syntax. Note: 'Option' only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        ICompileQuery<PARAMETERS, RESULT> Option(string labelName, params SqlServerQueryOption[] options);
    }

    public interface ICompileQuery<PARAMETERS, RESULT> {

        IPreparedQueryExecute<PARAMETERS, RESULT> Build();
    }

    public interface IPreparedQueryExecute<PARAMETERS, RESULT> {

        /// <summary>
        /// Initialize generates the underlying sql query if it do not already exist. This method is mostly used for benchmarking where we want the sql to be generated before the query is used.
        /// </summary>
        /// <param name="database"></param>
        void Initialize(IDatabase database);

        QueryResult<RESULT> Execute(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "");
        QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");

        Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, IDatabase database, CancellationToken cancellationToken, QueryTimeout? timeout = null, string debugName = "");
        Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken cancellationToken, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public RESULT? SingleOrDefault(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public RESULT? SingleOrDefault(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");

        /// <summary>
        /// Returns a value if there is only one row. If there are zero rows the default value is returned. If there is more than one row an exception is thrown
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="timeout"></param>
        /// <param name="useParameters"></param>
        /// <param name="debugName"></param>
        /// <returns></returns>
        public Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "");
    }
}