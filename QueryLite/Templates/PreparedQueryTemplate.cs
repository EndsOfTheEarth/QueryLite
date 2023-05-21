using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;

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
        public ICondition? HavingCondition { get; private set; }
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

        public IPreparedJoin<PARAMETERS, RESULT> Join(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            PreparedJoin<PARAMETERS, RESULT> join = new PreparedJoin<PARAMETERS, RESULT>(JoinType.Join, table, this);

            if(Joins == null) {
                Joins = new List<IPreparedJoin<PARAMETERS>>(1);
            }
            Joins.Add(join);
            return this;
        }

        public IPreparedJoin<PARAMETERS, RESULT> LeftJoin(ITable table) {

            ArgumentNullException.ThrowIfNull(table);

            PreparedJoin<PARAMETERS, RESULT> join = new PreparedJoin<PARAMETERS, RESULT>(JoinType.LeftJoin, table, this);

            if(Joins == null) {
                Joins = new List<IPreparedJoin<PARAMETERS>>(1);
            }
            Joins.Add(join);
            return this;
        }

        public IPreparedGroupBy<PARAMETERS, RESULT> Where(APreparedCondition<PARAMETERS>? condition) {

            WhereCondition = condition;
            return this;
        }

        public IPreparedHaving<PARAMETERS, RESULT> GroupBy(params ISelectable[] columns) {

            ArgumentNullException.ThrowIfNull(columns);

            GroupByFields = columns;
            return this;
        }

        public IPreparedOrderBy<PARAMETERS, RESULT> Having(ICondition condition) {

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

            PreparedQueryTemplate<PARAMETERS, RESULT> template = new PreparedQueryTemplate<PARAMETERS, RESULT>(selectFunc);
            template.ParentUnion = this;
            ChildUnion = template;
            ChildUnionType = UnionType.Union;
            return template;
        }

        public IPreparedDistinct<PARAMETERS, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {
            ArgumentNullException.ThrowIfNull(selectFunc);

            PreparedQueryTemplate<PARAMETERS, RESULT> template = new PreparedQueryTemplate<PARAMETERS, RESULT>(selectFunc);
            template.ParentUnion = this;
            ChildUnion = template;
            ChildUnionType = UnionType.UnionAll;
            return template;
        }

        public IPreparedQueryExecute<PARAMETERS, RESULT> Build() {
            return new PreparedQuery<PARAMETERS, RESULT>(this);
        }
    }

    internal sealed class PreparedQuery<PARAMETERS, RESULT> : IPreparedQueryExecute<PARAMETERS, RESULT> {

        private PreparedQueryTemplate<PARAMETERS, RESULT> QueryTemplate { get; }

        private readonly string?[] _sql;    //Store the sql for each database type in an array that is indexed by the database type integer value (For performance)

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
            _sql = new string?[max];
        }

        public List<RESULT> Execute(PARAMETERS parameters, IDatabase database, QueryTimeout? timeout = null, Parameters useParameters = Parameters.Default, string debugName = "") {

            int dbTypeIndex = (int)database.DatabaseType;

            if(_sql[dbTypeIndex] == null) {

                database.PreparedQueryGenerator.GetSql(QueryTemplate, database, out List<IParameter<PARAMETERS>> paramList);

            }
            throw new NotImplementedException();
        }
    }
}