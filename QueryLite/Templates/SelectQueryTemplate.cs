﻿using System;
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

    internal sealed class SelectQueryTemplate<RESULT> : ITop<RESULT>, IFrom<RESULT>, IHint<RESULT>, IJoin<RESULT>, IWhere<RESULT>, IGroupBy<RESULT>, IHaving<RESULT>, IOrderBy<RESULT>, IFor<RESULT>, IExecute<RESULT> {

        public Func<IResultRow, RESULT>? SelectFunction { get; }
        public SelectQueryTemplate<RESULT>? ParentUnion { get; private set; }
        public SelectQueryTemplate<RESULT>? ChildUnion { get; private set; }
        public UnionType? ChildUnionType { get; private set; }
        public IList<IField> SelectFields { get; private set; }
        public int? TopRows { get; private set; }
        public ITable? FromTable { get; private set; }
        public SqlServerHint[]? Hints { get; private set; }
        public IList<IJoin>? Joins { get; private set; }
        public ICondition? WhereCondition { get; private set; }
        public ISelectable[]? GroupByFields { get; private set; }
        public ICondition? HavingCondition { get; private set; }
        public IOrderByColumn[]? OrderByFields { get; private set; }

        public ForType? ForType { get; private set; } = null;
        public ITable[]? OfTables { get; private set; } = null;
        public WaitType? WaitType { get; private set; } = null;

        public SelectQueryTemplate(Func<IResultRow, RESULT> selectFunction) {
            SelectFunction = selectFunction;
            SelectFields = new List<IField>();
        }
        public SelectQueryTemplate(IList<IField> selectFields) {
            SelectFields = selectFields;
        }
        public IFrom<RESULT> Top(int rows) {

            if(rows <= 0) {
                throw new ArgumentException($"{nameof(rows)} must be greater than zero");
            }
            TopRows = rows;
            return this;
        }
        public IHint<RESULT> From(ITable table) {
            FromTable = table;
            return this;
        }

        public IJoin<RESULT> With(params SqlServerHint[] hints) {
            Hints = hints;
            return this;
        }

        public IJoinOn<RESULT> Join(ITable table) {

            Join<RESULT> join = new Join<RESULT>(JoinType.Join, table, this);

            if(Joins == null) {
                Joins = new List<IJoin>();
            }
            Joins.Add(join);
            return join;
        }
        public IJoinOn<RESULT> LeftJoin(ITable table) {

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
            GroupByFields = groupBy;
            return this;
        }

        public IOrderBy<RESULT> Having(ICondition condition) {
            HavingCondition = condition;
            return this;
        }

        public IFor<RESULT> OrderBy(params IOrderByColumn[] orderBy) {
            OrderByFields = orderBy;
            return this;
        }

        public IExecute<RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {
            ForType = forType;
            OfTables = ofTables;
            WaitType = waitType;
            return this;
        }

        public string GetSql(IDatabase database, IParameters? parameters = null) {

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

        public QueryResult<RESULT> Execute(Transaction transaction, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            FieldCollector fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters() : null;

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
                fieldCollector: fieldCollector);

            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync(Transaction transaction, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            FieldCollector fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? transaction.Database.CreateParameters() : null;

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
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }

        public Task<QueryResult<RESULT>> ExecuteAsync(IDatabase database, CancellationToken? cancellationToken = null, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

            if(timeout == null) {
                timeout = TimeoutLevel.ShortSelect;
            }

            FieldCollector fieldCollector = new FieldCollector();

            SelectFunction!(fieldCollector);

            SelectFields = fieldCollector.Fields;

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

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
                cancellationToken: cancellationToken ?? new CancellationToken()
            );
            return result;
        }

        public QueryResult<RESULT> Execute(IDatabase database, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default) {

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

            IParameters? parameters = (useParameters == Parameters.On) || (useParameters == Parameters.Default && Settings.UseParameters) ? database.CreateParameters() : null;

            string sql = database.QueryGenerator.GetSql(this, database, parameters);

            QueryResult<RESULT> result = QueryExecutor.Execute(
                database: database,
                transaction: null,
                timeout: timeout.Value,
                parameters: parameters,
                func: SelectFunction,
                sql: sql,
                queryType: QueryType.Select,
                fieldCollector: fieldCollector);

            return result;
        }

        public ITop<RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc) {

            SelectQueryTemplate<RESULT> template = new SelectQueryTemplate<RESULT>(selectFunc);
            template.ParentUnion = this;
            ChildUnion = template;
            ChildUnionType = QueryLite.UnionType.Union;
            return template;
        }

        public ITop<RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {

            SelectQueryTemplate<RESULT> template = new SelectQueryTemplate<RESULT>(selectFunc);
            template.ParentUnion = this;
            ChildUnion = template;
            ChildUnionType = UnionType.UnionAll;
            return template;
        }
    }
}