using System.Text;

namespace QueryLite.Databases.Functions {

    public class Case<TYPE> : Function<TYPE> where TYPE : notnull {

        private CaseTemplate<TYPE> Template { get; }

        internal Case(CaseTemplate<TYPE> template) : base(name: "CASE") {
            Template = template;
        }
        /// <summary>
        /// Begin with an expression for a Simple CASE statement. e.g.
        ///   CASE column_name
        ///     WHEN 'a' THEN 'b'
        ///     WHEN 'c' THEN 'd'
        ///   END
        /// </summary>
        /// <param name="selectable"></param>
        /// <returns></returns>
        public static ICaseWhen<TYPE> Simple(ISelectable selectable) {
            CaseTemplate<TYPE> template = new(simpleExpression: selectable);
            return template;
        }
        public static ICaseWhen<TYPE> Simple(IGetSelectSql nestedQuery) {
            CaseTemplate<TYPE> template = new(simpleExpression: nestedQuery);
            return template;
        }
        public static ICaseWhen<TYPE> SimpleValue(object value) {
            CaseTemplate<TYPE> template = new(simpleExpression: value);
            return template;
        }
        public static ICaseThen<TYPE> When(ICondition condition) {
            CaseTemplate<TYPE> template = new(simpleExpression: null);
            return template.When(condition);
        }
        public static ICaseThen<TYPE> When(ISelectable selectable) {
            CaseTemplate<TYPE> template = new(simpleExpression: null);
            return template.When(selectable);
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return Template.GetSql(database, useAlias: useAlias, parameters);
        }
    }

    internal class CaseTemplate<TYPE> : ICaseWhen<TYPE> where TYPE : notnull {

        private object? SimpleExpression { get; }

        public CaseTemplate(object? simpleExpression) {
            SimpleExpression = simpleExpression;
        }

        private List<CaseValue<TYPE>> Values { get; } = [];

        private object? ElseClause { get; set; }

        public ICaseThen<TYPE> When(ICondition condition) {
            CaseValue<TYPE> value = new(template: this, when: condition);
            Values.Add(value);
            return value;
        }
        public ICaseThen<TYPE> When(ISelectable selectable) {
            CaseValue<TYPE> value = new(template: this, when: selectable);
            Values.Add(value);
            return value;
        }
        public ICaseThen<TYPE> WhenValue(object value) {
            CaseValue<TYPE> val = new(template: this, when: value);
            Values.Add(val);
            return val;
        }

        public ICaseEnd<TYPE> Else(TYPE then) {
            ElseClause = then;
            return this;
        }

        public Case<TYPE> End() {
            return new Case<TYPE>(this);
        }

        public string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();

            sql.Append("CASE");

            if(SimpleExpression != null) {
                sql.Append('(');
                ConditionHelper.AppendSqlValue(SimpleExpression, sql, database, useAlias, parameters);
                sql.Append(')');
            }
            foreach(CaseValue<TYPE> value in Values) {

                sql.Append(" WHEN ");
                ConditionHelper.AppendSqlValue(value.When, sql, database, useAlias, parameters);

                if(value.Then != null) {
                    sql.Append(" THEN ");
                    ConditionHelper.AppendSqlValue(value.Then, sql, database, useAlias, parameters);
                }
            }

            if(ElseClause != null) {
                sql.Append(" ELSE ");
                ConditionHelper.AppendSqlValue(ElseClause, sql, database, useAlias, parameters);
            }
            sql.Append(" END");
            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }

    internal class CaseValue<TYPE> : ICaseThen<TYPE> where TYPE : notnull {

        private CaseTemplate<TYPE> Template { get; }

        public CaseValue(CaseTemplate<TYPE> template, object when) {
            Template = template;
            When = when;
        }
        public object When { get; }
        public object? Then { get; set; }

        ICaseWhen<TYPE> ICaseThen<TYPE>.Then(TYPE then) {
            Then = then;
            return Template;
        }
    }

    public interface ICaseThen<TYPE> where TYPE : notnull {

        public ICaseWhen<TYPE> Then(TYPE then);
    }

    public interface ICaseWhen<TYPE> : ICaseEnd<TYPE> where TYPE : notnull {

        public ICaseThen<TYPE> When(ICondition condition);
        public ICaseThen<TYPE> When(ISelectable selectable);
        public ICaseThen<TYPE> WhenValue(object value);

        public ICaseEnd<TYPE> Else(TYPE then);
    }

    public interface ICaseEnd<TYPE> where TYPE : notnull {
        public Case<TYPE> End();
    }
}