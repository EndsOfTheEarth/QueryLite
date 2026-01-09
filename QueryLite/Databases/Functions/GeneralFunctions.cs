using QueryLite.Databases;
using System.Text;

namespace QueryLite.Functions {

    /// <summary>
    /// COUNT(*) function
    /// </summary>
    public sealed class Count : Function<int> {

        public Count() : base(name: "COUNT(*)") { }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return "COUNT(*)";
        }
    }

    public sealed class Min<TYPE> : Function<TYPE> where TYPE : notnull {

        private ISelectable<TYPE> Selectable { get; }

        public Min(ISelectable<TYPE> selectable) : base(name: "MIN") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"MIN(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class NMin<TYPE> : NFunction<TYPE> where TYPE : notnull {

        private ISelectable<TYPE> Selectable { get; }

        public NMin(ISelectable<TYPE> selectable) : base(name: "MIN") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"MIN(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class Max<TYPE> : Function<TYPE> where TYPE : notnull {

        private ISelectable<TYPE> Selectable { get; }

        public Max(ISelectable<TYPE> selectable) : base(name: "MAX") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"MAX(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class NMax<TYPE> : NFunction<TYPE> where TYPE : notnull {

        private ISelectable<TYPE> Selectable { get; }

        public NMax(ISelectable<TYPE> selectable) : base(name: "MAX") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"MAX(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class Sum<TYPE> : Function<TYPE> where TYPE : notnull {

        private ISelectable<TYPE> Selectable { get; }

        public Sum(ISelectable<TYPE> selectable) : base(name: "SUM") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"SUM(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class NSum<TYPE> : NFunction<TYPE> where TYPE : notnull {

        private ISelectable<TYPE> Selectable { get; }

        public NSum(ISelectable<TYPE> selectable) : base(name: "SUM") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"SUM(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class Avg<TYPE> : Function<TYPE> where TYPE : notnull {

        private ISelectable Selectable { get; }

        public Avg(ISelectable selectable) : base(name: "AVG") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"AVG(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    public sealed class NAvg<TYPE> : NFunction<TYPE> where TYPE : notnull {

        private ISelectable Selectable { get; }

        public NAvg(ISelectable selectable) : base(name: "AVG") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();
            sql.Append($"AVG(");

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}