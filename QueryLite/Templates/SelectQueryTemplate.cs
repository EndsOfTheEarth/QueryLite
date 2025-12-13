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
using QueryLite.Databases;

namespace QueryLite {

    internal interface IQueryGenerator {
        internal string GetSql<RESULT>(SelectQueryTemplate<RESULT> template, IDatabase database, IParametersBuilder? parameters);
    }
    internal interface IInsertQueryGenerator {
        internal string GetSql<RESULT>(InsertQueryTemplate template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc);
    }

    internal interface IPreparedInsertQueryGenerator {
        internal string GetSql<PARAMETERS, RESULT>(PreparedInsertTemplate<PARAMETERS> template, IDatabase database, out PreparedParameterList<PARAMETERS> parameters, Func<IResultRow, RESULT>? outputFunc);
    }

    internal interface IUpdateQueryGenerator {
        internal string GetSql<RESULT>(UpdateQueryTemplate template, IDatabase database, Parameters useParameters, out IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc);
    }
    internal interface IPreparedUpdateQueryGenerator {
        internal string GetSql<PARAMETERS, RESULT>(PreparedUpdateTemplate<PARAMETERS> template, IDatabase database, out PreparedParameterList<PARAMETERS> parameters, Func<IResultRow, RESULT>? outputFunc);
    }
    internal interface IDeleteQueryGenerator {
        internal string GetSql<RESULT>(DeleteQueryTemplate template, IDatabase database, IParametersBuilder? parameters, Func<IResultRow, RESULT>? outputFunc);
    }
    internal interface IPreparedDeleteQueryGenerator {
        internal string GetSql<PARAMETERS, RESULT>(PreparedDeleteQueryTemplate<PARAMETERS> template, IDatabase database, out PreparedParameterList<PARAMETERS> parameters, Func<IResultRow, RESULT>? outputFunc);
    }
    internal interface ITruncateQueryGenerator {
        internal string GetSql(TruncateQueryTemplate template, IDatabase database, IParametersBuilder? parameters);
    }

    internal interface IPreparedQueryGenerator {
        internal string GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, PreparedParameterList<PARAMETERS> parameters);
    }

    internal interface ILikeSqlConditionGenerator {
        internal string GetSql<TYPE>(ILike<TYPE> like, IDatabase database);
    }

    internal class TemplateExtra<RESULT> {

        public SelectQueryTemplate<RESULT>? ParentUnion { get; internal set; }
        public SelectQueryTemplate<RESULT>? ChildUnion { get; internal set; }

        public UnionType? ChildUnionType { get; internal set; }

        public IList<IField>? NestedSelectFields { get; internal set; }

        public SqlServerTableHint[]? Hints { get; internal set; }

        public ISelectable[]? GroupByFields { get; internal set; }
        public ICondition? HavingCondition { get; internal set; }

        public ForType? ForType { get; internal set; } = null;
        public ITable[]? OfTables { get; internal set; } = null;
        public WaitType? WaitType { get; internal set; } = null;

        public string? OptionLabelName { get; internal set; } = null;
        public SqlServerQueryOption[]? Options { get; internal set; } = null;
    }
    internal sealed class SelectQueryTemplate<RESULT> : IDistinct<RESULT>, ITop<RESULT>, IFrom<RESULT>, IHint<RESULT>, IJoin<RESULT>, IWhere<RESULT>, IGroupBy<RESULT>, IHaving<RESULT>, IOrderBy<RESULT>, IFor<RESULT>, IExecute<RESULT> {

        public Func<IResultRow, RESULT>? SelectFunction { get; }

        public bool IsDistinct { get; private set; }
        public int? TopRows { get; private set; }
        public ITable? FromTable { get; private set; }
        public IList<IJoin>? Joins { get; private set; }
        public ICondition? WhereCondition { get; private set; }
        public IOrderByColumn[]? OrderByFields { get; private set; }

        public TemplateExtra<RESULT>? Extras { get; private set; }

        public SelectQueryTemplate(Func<IResultRow, RESULT> selectFunction) {

            ArgumentNullException.ThrowIfNull(selectFunction);

            SelectFunction = selectFunction;
        }
        public SelectQueryTemplate(IList<IField> selectFields) {

            ArgumentNullException.ThrowIfNull(selectFields);

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.NestedSelectFields = selectFields;
        }
        public ITop<RESULT> Distinct {
            get {
                IsDistinct = true;
                return this;
            }
        }
        public IFrom<RESULT> Top(int rows) {

            if(rows <= 0) {
                throw new ArgumentException($"{nameof(rows)} must be greater than zero");
            }
            TopRows = rows;
            return this;
        }
        public IHint<RESULT> From(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            FromTable = table;
            return this;
        }

        public IJoin<RESULT> With(params SqlServerTableHint[] hints) {

            ArgumentNullException.ThrowIfNull(hints);

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.Hints = hints;
            return this;
        }

        public IJoinOn<RESULT> Join(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            Join<RESULT> join = new Join<RESULT>(JoinType.Join, table, this);

            if(Joins == null) {
                Joins = new List<IJoin>();
            }
            Joins.Add(join);
            return join;
        }
        public IJoinOn<RESULT> LeftJoin(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            Join<RESULT> join = new Join<RESULT>(JoinType.LeftJoin, table, this);

            if(Joins == null) {
                Joins = new List<IJoin>();
            }
            Joins.Add(join);
            return join;
        }

        public IGroupBy<RESULT> Where(ICondition? condition) {
            WhereCondition = condition;
            return this;
        }
        public IHaving<RESULT> GroupBy(params ISelectable[] groupBy) {

            ArgumentNullException.ThrowIfNull(groupBy);

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.GroupByFields = groupBy;
            return this;
        }

        public IOrderBy<RESULT> Having(ICondition condition) {

            ArgumentNullException.ThrowIfNull(condition);

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.HavingCondition = condition;
            return this;
        }

        public IFor<RESULT> OrderBy(params IOrderByColumn[] orderBy) {

            ArgumentNullException.ThrowIfNull(orderBy);

            OrderByFields = orderBy;
            return this;
        }

        public IOption<RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {

            ArgumentNullException.ThrowIfNull(ofTables);

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.ForType = forType;
            Extras.OfTables = ofTables;
            Extras.WaitType = waitType;
            return this;
        }

        public IExecute<RESULT> Option(params SqlServerQueryOption[] options) {

            ArgumentNullException.ThrowIfNull(options);

            if(options.Length == 0) {
                throw new ArgumentException($"{nameof(options)} cannot be empty");
            }

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.Options = options;
            return this;
        }

        public IExecute<RESULT> Option(string labelName, params SqlServerQueryOption[] options) {

            ArgumentException.ThrowIfNullOrEmpty(labelName);
            ArgumentNullException.ThrowIfNull(options);

            if(options.Length == 0) {
                throw new ArgumentException($"{nameof(options)} cannot be empty");
            }

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.OptionLabelName = labelName;
            Extras.Options = options;
            return this;
        }

        public string GetSql(IDatabase database, IParametersBuilder? parameters = null) {

            ArgumentNullException.ThrowIfNull(database);

            return database.QueryGenerator.GetSql(this, database, parameters);
        }

        public QueryResult<RESULT> Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName);

            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            QueryResult<RESULT> result = await QueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(database);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = await QueryExecutor.ExecuteAsync(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public QueryResult<RESULT> Execute(IDatabase database, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(database);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public RESULT? SingleOrDefault(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            RESULT? result = QueryExecutor.SingleOrDefault(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public RESULT? SingleOrDefault(IDatabase database, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(database);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            RESULT? result = QueryExecutor.SingleOrDefault(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public async Task<RESULT?> SingleOrDefaultAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            RESULT? result = await QueryExecutor.SingleOrDefaultAsync(
                database: transaction.Database,
                cancellationToken: cancellationToken ?? CancellationToken.None,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public async Task<RESULT?> SingleOrDefaultAsync(IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(database);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            IParametersBuilder? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            RESULT? result = await QueryExecutor.SingleOrDefaultAsync(
                database: database,
                cancellationToken: cancellationToken ?? CancellationToken.None,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction!,
                sql: sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public IDistinct<RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc) {

            ArgumentNullException.ThrowIfNull(selectFunc);

            SelectQueryTemplate<RESULT> template = new SelectQueryTemplate<RESULT>(selectFunc) {
                Extras = new TemplateExtra<RESULT> {
                    ParentUnion = this
                }
            };

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.ChildUnion = template;
            Extras.ChildUnionType = UnionType.Union;
            return template;
        }

        public IDistinct<RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {

            ArgumentNullException.ThrowIfNull(selectFunc);

            SelectQueryTemplate<RESULT> template = new SelectQueryTemplate<RESULT>(selectFunc) {
                Extras = new TemplateExtra<RESULT> {
                    ParentUnion = this
                }
            };

            if(Extras == null) {
                Extras = new TemplateExtra<RESULT>();
            }
            Extras.ChildUnion = template;
            Extras.ChildUnionType = UnionType.UnionAll;
            return template;
        }
    }
}