using QueryLite.Databases;
using System.Text;
using System.Text.Json;

namespace QueryLite {

    public class Expression<TYPE> : Function<TYPE>, ICondition where TYPE : notnull {

        private readonly List<object> _values = [];

        public Expression(params ISelectable[] values) : base(name: "Expression") {

            foreach(ISelectable value in values) {
                _values.Add(value);
            }
        }
        public Expression(params SqlText[] values) : base(name: "Expression") {

            foreach(SqlText value in values) {
                _values.Add(value);
            }
        }
        public static Expression<TYPE> operator +(Expression<TYPE> expression, ISelectable column) {
            expression._values.Add(column);
            return expression;
        }
        public static Expression<TYPE> operator +(Expression<TYPE> expression, SqlText column) {
            expression._values.Add(column);
            return expression;
        }
        public static Expression<TYPE> operator +(Expression<TYPE> expression, string column) {
            expression._values.Add(SqlText.Raw(column));
            return expression;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();

            for(int index = 0; index < _values.Count; index++) {

                object value = _values[index];

                if(index > 0) {
                    sql.Append(' ');
                }
                ConditionHelper.AppendSqlValue(value, sql, database, useAlias: useAlias, parameters);
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }
        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            sql.Append(GetSql(database, useAlias, parameters));
        }
    }
    
    public readonly struct SqlText {

        public static SqlText Raw(string value) => new(value, surround: false);
        public static SqlText Quoted(string value) => new(value, surround: true);

        public static SqlText QuotedAsJson(object value) => new(JsonSerializer.Serialize(value), surround: true);

        public static implicit operator SqlText(string text) => Raw(text);

        public string AsEscapedText() => _value;

        private readonly string _value;

        private SqlText(string value, bool surround) {

            if(surround) {
                _value = $"'{Helpers.EscapeForSql(value)}'";
            }
            else {
                _value = Helpers.EscapeForSql(value);
            }
        }
    }
}