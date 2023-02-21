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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    public enum JoinType {
        Join, LeftJoin
    }

    public interface IJoin {

        JoinType JoinType { get; }
        ITable Table { get; }
        ICondition Condition { get; }
    }
    internal sealed class Join<RESULT> : IJoin, IJoinOn<RESULT> {

        public JoinType JoinType { get; private set; }
        public ITable Table { get; private set; }

        private ICondition? _condition;

        public ICondition Condition {
            get { return _condition!; }
        }

        private readonly SelectQueryTemplate<RESULT> Template;

        internal Join(JoinType joinType, ITable table, SelectQueryTemplate<RESULT> tempate) {
            JoinType = joinType;
            Table = table;
            Template = tempate;
        }
        public IJoin<RESULT> On(ICondition on) {
            _condition = on;
            return Template;
        }
    }

    public interface ITop<RESULT> : IFrom<RESULT> {

        /// <summary>
        /// Return TOP n rows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IFrom<RESULT> Top(int rows);
    }
    public interface IFrom<RESULT> {

        /// <summary>
        /// From table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IHint<RESULT> From(ITable table);
    }

    public interface IHint<RESULT> : IJoin<RESULT> {

        /// <summary>
        /// The 'With' option only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public IJoin<RESULT> With(params SqlServerTableHint[] hints);
    }

    public enum SqlServerTableHint {
        KEEPIDENTITY,
        KEEPDEFAULTS,
        HOLDLOCK,
        IGNORE_CONSTRAINTS,
        IGNORE_TRIGGERS,
        NOLOCK,
        NOWAIT,
        PAGLOCK,
        READCOMMITTED,
        READCOMMITTEDLOCK,
        READPAST,
        REPEATABLEREAD,
        ROWLOCK,
        SERIALIZABLE,
        SNAPSHOT,
        TABLOCK,
        TABLOCKX,
        UPDLOCK,
        XLOCK
    }
    public interface IJoin<RESULT> : IWhere<RESULT> {

        /// <summary>
        /// Join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IJoinOn<RESULT> Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IJoinOn<RESULT> LeftJoin(ITable table);
    }

    public interface IJoinOn<RESULT> {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IJoin<RESULT> On(ICondition on);
    }

    public interface IWhere<RESULT> : IGroupBy<RESULT> {

        /// <summary>
        /// Where condition clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IGroupBy<RESULT> Where(ICondition? condition);
    }

    public interface IGroupBy<RESULT> : IHaving<RESULT> {

        /// <summary>
        /// Group by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IHaving<RESULT> GroupBy(params ISelectable[] columns);
    }

    public interface IHaving<RESULT> : IOrderBy<RESULT> {

        /// <summary>
        /// Having clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IOrderBy<RESULT> Having(ICondition condition);
    }

    public interface IOrderBy<RESULT> : IFor<RESULT> {

        /// <summary>
        /// Order by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IFor<RESULT> OrderBy(params IOrderByColumn[] columns);

        /// <summary>
        /// Union query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        ITop<RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc);

        /// <summary>
        /// Union all query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        ITop<RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc);
    }

    public interface IFor<RESULT> : IOption<RESULT> {

        /// <summary>
        /// FOR caluse. PostgreSql only
        /// </summary>
        /// <param name="forType"></param>
        /// <param name="ofTables"></param>
        /// <param name="waitType"></param>
        /// <returns></returns>
        IOption<RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType);
    }

    public interface IOption<RESULT> : IExecute<RESULT> {

        /// <summary>
        /// Sql server OPTION syntax. Note: 'Option' only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        IExecute<RESULT> Option(params SqlServerQueryOption[] options);

        /// <summary>
        /// Sql server OPTION syntax. Note: 'Option' only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        IExecute<RESULT> Option(string labelName, params SqlServerQueryOption[] options);
    }

    public enum ForType {
        UPDATE,
        NO_KEY_UPDATE,
        SHARE,
        KEY_SHARE
    }
    public enum WaitType {
        WAIT,
        NOWAIT,
        SKIP_LOCKED
    }
    public enum UnionType {
        Union,
        UnionAll
    }

    public enum SqlServerQueryOption {
        HASH_JOIN,
        LOOP_JOIN,
        MERGE_JOIN,
        FORCE_ORDER,
        FORCE_EXTERNALPUSHDOWN,
        DISABLE_EXTERNALPUSHDOWN
    }

    public interface IExecute<RESULT> {

        /// <summary>
        /// Get select query sql
        /// </summary>
        /// <param name="database"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GetSql(IDatabase database, IParameters? parameters = null);

        public QueryResult<RESULT> Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        public QueryResult<RESULT> Execute(IDatabase database, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);

        public Task<QueryResult<RESULT>> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
        public Task<QueryResult<RESULT>> ExecuteAsync(IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default);
    }

    public sealed class QueryResult<RESULT> {

        /// <summary>
        /// Returned rows
        /// </summary>
        public IList<RESULT> Rows { get; }

        /// <summary>
        /// Sql of query
        /// </summary>
        public string Sql { get; }

        /// <summary>
        /// Rows effected by a change
        /// </summary>
        public int RowsEffected { get; }

        public QueryResult(IList<RESULT> rows, string sql, int rowsEffected) {
            Rows = rows;
            Sql = sql;
            RowsEffected = rowsEffected;
        }
        public override string ToString() {
            return $"Rows: {Rows.Count}, Rows Effected: {RowsEffected}";
        }
    }
    public sealed class NonQueryResult {

        /// <summary>
        /// Sql of query
        /// </summary>
        public string Sql { get; }

        /// <summary>
        /// Rows effected by a change
        /// </summary>
        public int RowsEffected { get; }

        public NonQueryResult(string sql, int rowsEffected) {
            Sql = sql;
            RowsEffected = rowsEffected;
        }
    }
}