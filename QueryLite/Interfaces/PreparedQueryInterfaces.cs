using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;

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
        IPreparedJoin<PARAMETERS, RESULT> Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedJoin<PARAMETERS, RESULT> LeftJoin(ITable table);
    }

    public interface IPreparedJoin<PARAMETERS> {

        JoinType JoinType { get; }
        ITable Table { get; }
        IPreparedCondition<PARAMETERS> Condition { get; }
    }
    internal sealed class PreparedJoin<PARAMETERS, RESULT> : IPreparedJoin<PARAMETERS> {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        private IPreparedCondition<PARAMETERS>? _condition;

        public IPreparedCondition<PARAMETERS> Condition {
            get { return _condition!; }
        }

        private readonly PreparedQueryTemplate<PARAMETERS, RESULT> Template;

        internal PreparedJoin(JoinType joinType, ITable table, PreparedQueryTemplate<PARAMETERS, RESULT> tempate) {
            JoinType = joinType;
            Table = table;
            Template = tempate;
        }
        public IPreparedJoin<PARAMETERS, RESULT> On(IPreparedCondition<PARAMETERS> on) {
            _condition = on;
            return Template;
        }
    }

    public interface IPreparedWhere<PARAMETERS, RESULT> : IPreparedGroupBy<PARAMETERS, RESULT> {

        /// <summary>
        /// Where condition clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IPreparedGroupBy<PARAMETERS, RESULT> Where(IPreparedCondition<PARAMETERS>? condition);
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
        IPreparedOrderBy<PARAMETERS, RESULT> Having(IPreparedCondition<PARAMETERS> condition);
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
        /// FOR caluse. PostgreSql only
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

        QueryResult<RESULT> Execute(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "");
    }
}