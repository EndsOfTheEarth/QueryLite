/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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
using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal sealed class PreparedQueryTemplate<PARAMETERS, RESULT> : IPreparedDistinct<PARAMETERS, RESULT>, IPreparedTop<PARAMETERS, RESULT>, IPreparedFrom<PARAMETERS, RESULT>, IPreparedHint<PARAMETERS, RESULT>,
                                                        IPreparedJoin<PARAMETERS, RESULT>, IPreparedWhere<PARAMETERS, RESULT>, IPreparedGroupBy<PARAMETERS, RESULT>,
                                                        IPreparedHaving<PARAMETERS, RESULT>, IPreparedOrderBy<PARAMETERS, RESULT>, IPreparedFor<PARAMETERS, RESULT>, IPreparedOption<PARAMETERS, RESULT>,
                                                        ICompileQuery<PARAMETERS, RESULT> {

        public Func<IResultRow, RESULT>? SelectFunction { get; }
        public IList<IField> SelectFields { get; private set; }

        public PreparedQueryTemplate<PARAMETERS, RESULT>? ParentUnion { get; private set; }
        public PreparedQueryTemplate<PARAMETERS, RESULT>? ChildUnion { get; private set; }
        public UnionType? ChildUnionType { get; private set; }

        public bool IsDistinct { get; private set; }
        public int? TopRows { get; private set; }
        public ITable? FromTable { get; private set; }
        public SqlServerTableHint[]? Hints { get; private set; }
        public IList<IPreparedJoin<PARAMETERS>>? Joins { get; private set; }
        public APreparedCondition<PARAMETERS>? WhereCondition { get; private set; }
        public ISelectable[]? GroupByFields { get; private set; }
        public APreparedCondition<PARAMETERS>? HavingCondition { get; private set; }
        public IOrderByColumn[]? OrderByFields { get; private set; }

        public ForType? ForType { get; private set; } = null;
        public ITable[]? OfTables { get; private set; } = null;
        public WaitType? WaitType { get; private set; } = null;

        public string? OptionLabelName { get; private set; } = null;
        public SqlServerQueryOption[]? Options { get; private set; } = null;

        public PreparedQueryTemplate(Func<IResultRow, RESULT> selectFunction) {

            ArgumentNullException.ThrowIfNull(selectFunction);

            SelectFunction = selectFunction;
            SelectFields = new List<IField>();
        }
        public IPreparedTop<PARAMETERS, RESULT> Distinct {
            get {
                IsDistinct = true;
                return this;
            }
        }

        public IPreparedFrom<PARAMETERS, RESULT> Top(int rows) {

            if(rows <= 0) {
                throw new ArgumentException($"{nameof(rows)} must be greater than zero");
            }
            TopRows = rows;
            return this;
        }

        public IPreparedHint<PARAMETERS, RESULT> From(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            FromTable = table;
            return this;
        }


        public IPreparedJoin<PARAMETERS, RESULT> With(params SqlServerTableHint[] hints) {

            ArgumentNullException.ThrowIfNull(hints);

            Hints = hints;
            return this;
        }

        public IPreparedJoinOn<PARAMETERS, RESULT> Join(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            PreparedJoin<PARAMETERS, RESULT> join = new PreparedJoin<PARAMETERS, RESULT>(JoinType.Join, table, this);

            if(Joins == null) {
                Joins = new List<IPreparedJoin<PARAMETERS>>();
            }
            Joins.Add(join);
            return join;
        }

        public IPreparedJoinOn<PARAMETERS, RESULT> LeftJoin(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            PreparedJoin<PARAMETERS, RESULT> join = new PreparedJoin<PARAMETERS, RESULT>(JoinType.LeftJoin, table, this);

            if(Joins == null) {
                Joins = new List<IPreparedJoin<PARAMETERS>>();
            }
            Joins.Add(join);
            return join;
        }

        public IPreparedGroupBy<PARAMETERS, RESULT> Where(Func<APreparedCondition<PARAMETERS>, APreparedCondition<PARAMETERS>>? condition) {

            if(condition != null) {
                WhereCondition = condition(new EmptyPreparedCondition<PARAMETERS>());
            }
            else {
                WhereCondition = null;
            }
            return this;
        }

        public IPreparedHaving<PARAMETERS, RESULT> GroupBy(params ISelectable[] columns) {

            ArgumentNullException.ThrowIfNull(columns);

            GroupByFields = columns;
            return this;
        }

        public IPreparedOrderBy<PARAMETERS, RESULT> Having(APreparedCondition<PARAMETERS> condition) {

            ArgumentNullException.ThrowIfNull(condition);

            HavingCondition = condition;
            return this;
        }

        public IPreparedFor<PARAMETERS, RESULT> OrderBy(params IOrderByColumn[] columns) {

            ArgumentNullException.ThrowIfNull(columns);

            OrderByFields = columns;
            return this;
        }

        public IPreparedOption<PARAMETERS, RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {

            ArgumentNullException.ThrowIfNull(ofTables);

            ForType = forType;
            OfTables = ofTables;
            WaitType = waitType;
            return this;
        }

        public ICompileQuery<PARAMETERS, RESULT> Option(params SqlServerQueryOption[] options) {

            ArgumentNullException.ThrowIfNull(options);

            if(options.Length == 0) {
                throw new ArgumentException($"{nameof(options)} cannot be empty");
            }
            Options = options;
            return this;
        }

        public ICompileQuery<PARAMETERS, RESULT> Option(string labelName, params SqlServerQueryOption[] options) {

            ArgumentException.ThrowIfNullOrEmpty(labelName);
            ArgumentNullException.ThrowIfNull(options);

            if(options.Length == 0) {
                throw new ArgumentException($"{nameof(options)} cannot be empty");
            }
            OptionLabelName = labelName;
            Options = options;
            return this;
        }

        public IPreparedDistinct<PARAMETERS, RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc) {

            ArgumentNullException.ThrowIfNull(selectFunc);

            PreparedQueryTemplate<PARAMETERS, RESULT> template = new PreparedQueryTemplate<PARAMETERS, RESULT>(selectFunc) {
                ParentUnion = this
            };
            ChildUnion = template;
            ChildUnionType = UnionType.Union;
            return template;
        }

        public IPreparedDistinct<PARAMETERS, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {
            ArgumentNullException.ThrowIfNull(selectFunc);

            PreparedQueryTemplate<PARAMETERS, RESULT> template = new PreparedQueryTemplate<PARAMETERS, RESULT>(selectFunc) {
                ParentUnion = this
            };
            ChildUnion = template;
            ChildUnionType = UnionType.UnionAll;
            return template;
        }

        public IPreparedQueryExecute<PARAMETERS, RESULT> Build() {

            PreparedQueryTemplate<PARAMETERS, RESULT> template = this;

            while(template.ParentUnion != null) {
                template = template.ParentUnion;
            }

            while(true) {

                FieldCollector fieldCollector = new FieldCollector();

                template.SelectFunction!(fieldCollector);

                template.SelectFields = fieldCollector.Fields;

                if(template.ChildUnion == null) {
                    break;
                }
                template = template.ChildUnion;
            }
            return new PreparedQuery<PARAMETERS, RESULT>(this);
        }
    }

    internal sealed class PreparedQuery<PARAMETERS, RESULT> : IPreparedQueryExecute<PARAMETERS, RESULT> {

        private PreparedQueryTemplate<PARAMETERS, RESULT> QueryTemplate { get; }

        private readonly PreparedQueryDetail<PARAMETERS>?[] _queries;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)

        public PreparedQuery(PreparedQueryTemplate<PARAMETERS, RESULT> template) {

            QueryTemplate = template;

            DatabaseType[] values = Enum.GetValues<DatabaseType>();

            int max = 0;

            foreach(DatabaseType value in values) {

                int valueAsInt = (int)value;

                if(valueAsInt > max) {
                    max = valueAsInt;
                }
            }
            _queries = new PreparedQueryDetail<PARAMETERS>?[max + 1];
        }

        public void Initialize(IDatabase database) {
            _ = GetQueryDetail(database);
        }

        public PreparedQueryDetail<PARAMETERS> GetQueryDetail(IDatabase database) {

            int dbTypeIndex = (int)database.DatabaseType;

            PreparedQueryDetail<PARAMETERS> queryDetail;

            if(_queries[dbTypeIndex] == null) {

                PreparedParameterList<PARAMETERS> parameters = new PreparedParameterList<PARAMETERS>();

                string sql = database.PreparedQueryGenerator.GetSql(QueryTemplate, database, parameters);

                queryDetail = new PreparedQueryDetail<PARAMETERS>(sql, parameters);

                _queries[dbTypeIndex] = queryDetail;
            }
            else {
                queryDetail = _queries[dbTypeIndex]!;
            }
            return queryDetail;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "") {

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            QueryResult<RESULT> result = PreparedQueryExecutor.Execute(
                database: database,
                paramValue: parameters,
                transaction: null,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public QueryResult<RESULT> Execute(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            IDatabase database = transaction.Database;

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            QueryResult<RESULT> result = PreparedQueryExecutor.Execute(
                database: database,
                paramValue: parameters,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, IDatabase database, CancellationToken cancellationToken, QueryTimeout? timeout = null, string debugName = "") {

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            QueryResult<RESULT> result = await PreparedQueryExecutor.ExecuteAsync(
                database: database,
                paramValue: parameters,
                transaction: null,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName,
                cancellationToken: cancellationToken
            );
            return result;
        }

        public async Task<QueryResult<RESULT>> ExecuteAsync(PARAMETERS parameters, Transaction transaction, CancellationToken cancellationToken, QueryTimeout? timeout = null, string debugName = "") {

            IDatabase database = transaction.Database;

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            QueryResult<RESULT> result = await PreparedQueryExecutor.ExecuteAsync(
                database: database,
                paramValue: parameters,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName,
                cancellationToken: cancellationToken
            );
            return result;
        }

        public RESULT? SingleOrDefault(PARAMETERS parameters, Transaction transaction, QueryTimeout? timeout = null, string debugName = "") {

            IDatabase database = transaction.Database;

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            RESULT? result = PreparedQueryExecutor.SingleOrDefault(
                database: database,
                paramValue: parameters,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public RESULT? SingleOrDefault(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, string debugName = "") {

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            RESULT? result = PreparedQueryExecutor.SingleOrDefault(
                database: database,
                paramValue: parameters,
                transaction: null,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName
            );
            return result;
        }

        public async Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            IDatabase database = transaction.Database;

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            RESULT? result = await PreparedQueryExecutor.SingleOrDefaultAsync(
                database: database,
                paramValue: parameters,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public async Task<RESULT?> SingleOrDefaultAsync(PARAMETERS parameters, IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, string debugName = "") {

            PreparedQueryDetail<PARAMETERS> queryDetail = GetQueryDetail(database);

            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            RESULT? result = await PreparedQueryExecutor.SingleOrDefaultAsync(
                database: database,
                paramValue: parameters,
                transaction: null,
                timeout: timeout.Value,
                parameters: queryDetail.QueryParameters,
                func: QueryTemplate.SelectFunction!,
                sql: queryDetail.Sql,
                queryType: QueryType.Select,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }
    }

    internal sealed class PreparedQueryDetail<PARAMETERS> {

        public PreparedQueryDetail(string sql, PreparedParameterList<PARAMETERS> queryParameters) {
            Sql = sql;
            QueryParameters = queryParameters;
        }
        public string Sql { get; }
        public PreparedParameterList<PARAMETERS> QueryParameters { get; }
    }
}