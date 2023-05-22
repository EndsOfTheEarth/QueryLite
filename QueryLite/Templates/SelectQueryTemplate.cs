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
using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLite {

    internal interface IQueryGenerator {
        internal string GetSql<RESULT>(SelectQueryTemplate<RESULT> template, IDatabase database, IParameters? parameters);
    }
    internal interface IInsertQueryGenerator {
        internal string GetSql(InsertQueryTemplate template, IDatabase database, IParameters? parameters);
    }
    internal interface IUpdateQueryGenerator {
        internal string GetSql(UpdateQueryTemplate template, IDatabase database, IParameters? parameters);
    }
    internal interface IDeleteQueryGenerator {
        internal string GetSql(DeleteQueryTemplate template, IDatabase database, IParameters? parameters);
    }
    internal interface ITruncateQueryGenerator {
        internal string GetSql(TruncateQueryTemplate template, IDatabase database, IParameters? parameters);
    }

    internal interface IPreparedQueryGenerator {
        internal string GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, IParameterCollector<PARAMETERS> parameters);
    }

    internal sealed class SelectQueryTemplate<RESULT> : IDistinct<RESULT>, ITop<RESULT>, IFrom<RESULT>, IHint<RESULT>, IJoin<RESULT>, IWhere<RESULT>, IGroupBy<RESULT>, IHaving<RESULT>, IOrderBy<RESULT>, IFor<RESULT>, IExecute<RESULT> {

        public Func<IResultRow, RESULT>? SelectFunction { get; }
        public SelectQueryTemplate<RESULT>? ParentUnion { get; private set; }
        public SelectQueryTemplate<RESULT>? ChildUnion { get; private set; }
        public UnionType? ChildUnionType { get; private set; }
        public IList<IField> SelectFields { get; private set; }
        public bool IsDistinct { get; private set; }
        public int? TopRows { get; private set; }
        public ITable? FromTable { get; private set; }
        public SqlServerTableHint[]? Hints { get; private set; }
        public IList<IJoin>? Joins { get; private set; }
        public ICondition? WhereCondition { get; private set; }
        public ISelectable[]? GroupByFields { get; private set; }
        public ICondition? HavingCondition { get; private set; }
        public IOrderByColumn[]? OrderByFields { get; private set; }

        public ForType? ForType { get; private set; } = null;
        public ITable[]? OfTables { get; private set; } = null;
        public WaitType? WaitType { get; private set; } = null;

        public string? OptionLabelName { get; private set; } = null;
        public SqlServerQueryOption[]? Options { get; private set; } = null;

        public SelectQueryTemplate(Func<IResultRow, RESULT> selectFunction) {

            ArgumentNullException.ThrowIfNull(selectFunction);

            SelectFunction = selectFunction;
            SelectFields = new List<IField>();
        }
        public SelectQueryTemplate(IList<IField> selectFields) {

            ArgumentNullException.ThrowIfNull(selectFields);

            SelectFields = selectFields;
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

            Hints = hints;
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

            GroupByFields = groupBy;
            return this;
        }

        public IOrderBy<RESULT> Having(ICondition condition) {

            ArgumentNullException.ThrowIfNull(condition);

            HavingCondition = condition;
            return this;
        }

        public IFor<RESULT> OrderBy(params IOrderByColumn[] orderBy) {

            ArgumentNullException.ThrowIfNull(orderBy);

            OrderByFields = orderBy;
            return this;
        }

        public IOption<RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {

            ArgumentNullException.ThrowIfNull(ofTables);

            ForType = forType;
            OfTables = ofTables;
            WaitType = waitType;
            return this;
        }

        public IExecute<RESULT> Option(params SqlServerQueryOption[] options) {

            ArgumentNullException.ThrowIfNull(options);

            if(options.Length == 0) {
                throw new ArgumentException($"{nameof(options)} cannot be empty");
            }
            Options = options;
            return this;
        }

        public IExecute<RESULT> Option(string labelName, params SqlServerQueryOption[] options) {

            ArgumentException.ThrowIfNullOrEmpty(labelName);
            ArgumentNullException.ThrowIfNull(options);

            if(options.Length == 0) {
                throw new ArgumentException($"{nameof(options)} cannot be empty");
            }
            OptionLabelName = labelName;
            Options = options;
            return this;
        }

        public string GetSql(IDatabase database, IParameters? parameters = null) {

            ArgumentNullException.ThrowIfNull(database);

            FieldCollector fieldCollector = new FieldCollector();

            if(SelectFunction != null) {
                SelectFunction!(fieldCollector);
                SelectFields = fieldCollector.Fields;
            }
            else if(SelectFields == null || SelectFields.Count == 0) {
                throw new Exception($"{nameof(SelectFields)} should not be null or empty at this stage");
            }
            return database.QueryGenerator.GetSql(this, database, parameters);
        }

        public QueryResult<RESULT> Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            FieldCollector fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                selectFields: SelectFields,
                debugName: debugName,
                fieldCollector: fieldCollector);

            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(transaction);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            FieldCollector fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            Task<QueryResult<RESULT>> result = QueryExecutor.ExecuteAsync(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                selectFields: SelectFields,
                fieldCollector: fieldCollector,
                debugName: debugName,
                cancellationToken: cancellationToken ?? CancellationToken.None
            );
            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync(IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            ArgumentNullException.ThrowIfNull(database);
            ArgumentNullException.ThrowIfNull(debugName);

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            FieldCollector fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            Task<QueryResult<RESULT>> result = QueryExecutor.ExecuteAsync(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                selectFields: SelectFields,
                fieldCollector: fieldCollector,
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

            FieldCollector fieldCollector;

            SelectQueryTemplate<RESULT>? template = this;

            while(template.ParentUnion != null) {
                template = template.ParentUnion;
                fieldCollector = new FieldCollector();
                template.SelectFunction!(fieldCollector);
                template.SelectFields = fieldCollector.Fields;
            }

            fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                fieldCollector: fieldCollector,
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

            FieldCollector fieldCollector;

            SelectQueryTemplate<RESULT?>? template = this;

            while(template.ParentUnion != null) {
                template = template.ParentUnion;
                fieldCollector = new FieldCollector();
                template.SelectFunction!(fieldCollector);
                template.SelectFields = fieldCollector.Fields;
            }

            fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            RESULT? result = QueryExecutor.SingleOrDefault(
                database: transaction.Database,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                fieldCollector: fieldCollector,
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

            FieldCollector fieldCollector;

            SelectQueryTemplate<RESULT?>? template = this;

            while(template.ParentUnion != null) {
                template = template.ParentUnion;
                fieldCollector = new FieldCollector();
                template.SelectFunction!(fieldCollector);
                template.SelectFields = fieldCollector.Fields;
            }

            fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            RESULT? result = QueryExecutor.SingleOrDefault(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                fieldCollector: fieldCollector,
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

            FieldCollector fieldCollector;

            SelectQueryTemplate<RESULT?>? template = this;

            while(template.ParentUnion != null) {
                template = template.ParentUnion;
                fieldCollector = new FieldCollector();
                template.SelectFunction!(fieldCollector);
                template.SelectFields = fieldCollector.Fields;
            }

            fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters(initParams: 1) : null;

            string sql = transaction.Database.QueryGenerator.GetSql(this, transaction.Database, parameters);

            RESULT? result = await QueryExecutor.SingleOrDefaultAsync(
                database: transaction.Database,
                cancellationToken: cancellationToken ?? CancellationToken.None,
                transaction: transaction,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                fieldCollector: fieldCollector,
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

            FieldCollector fieldCollector;

            SelectQueryTemplate<RESULT?>? template = this;

            while(template.ParentUnion != null) {
                template = template.ParentUnion;
                fieldCollector = new FieldCollector();
                template.SelectFunction!(fieldCollector);
                template.SelectFields = fieldCollector.Fields;
            }

            fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters(initParams: 1) : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            RESULT? result = await QueryExecutor.SingleOrDefaultAsync(
                database: database,
                cancellationToken: cancellationToken ?? CancellationToken.None,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                fieldCollector: fieldCollector,
                debugName: debugName
            );
            return result;
        }

        public IDistinct<RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc) {

            ArgumentNullException.ThrowIfNull(selectFunc);

            SelectQueryTemplate<RESULT> template = new SelectQueryTemplate<RESULT>(selectFunc);
            template.ParentUnion = this;
            ChildUnion = template;
            ChildUnionType = UnionType.Union;
            return template;
        }

        public IDistinct<RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {

            ArgumentNullException.ThrowIfNull(selectFunc);

            SelectQueryTemplate<RESULT> template = new SelectQueryTemplate<RESULT>(selectFunc);
            template.ParentUnion = this;
            ChildUnion = template;
            ChildUnionType = UnionType.UnionAll;
            return template;
        }
    }
}