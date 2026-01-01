using System.Text;

namespace QueryLite.Databases.Functions {

    public class COALESCE<TYPE> : AFunction<TYPE> where TYPE : notnull {

        private List<object> List { get; } = [];

        public COALESCE(TYPE[] values, ISelectable<TYPE>[] selectables) : this(selectables, values) { }

        public COALESCE(TYPE[] values) : this(selectables: [], values) { }

        public COALESCE(ISelectable<TYPE>[] selectables) : this(selectables, values: []) { }

        public COALESCE(ISelectable<TYPE>[] selectables, TYPE[] values) : base(name: "COALESCE") {

            ArgumentNullException.ThrowIfNull(selectables);
            ArgumentNullException.ThrowIfNull(values);

            List.AddRange(selectables);
            List.AddRange(values);
        }

        /// <summary>
        /// Gives the ability to include ISelectable columns/functions that have a different TYPE constraint.
        /// </summary>
        /// <param name="selectables"></param>
        public void IncludeIgnoringTypeConstraint(ISelectable[] selectables) {

            ArgumentNullException.ThrowIfNull(selectables);

            List.Add(selectables);
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();

            sql.Append($"COALESCE(");

            for(int index = 0; index < List.Count; index++) {

                if(index > 0) {
                    sql.Append(',');
                }
                object value = List[index];

                if(value is TYPE type) {
                    ConditionHelper.AppendSqlValue(type, sql, database, useAlias: useAlias, parameters);
                }
                else if(value is ISelectable selectable) {
                    ConditionHelper.AppendSqlValue(selectable, sql, database, useAlias: useAlias, parameters);
                }
                else {
                    throw new Exception($"Unexpected type. Type = '{value.GetType().FullName}'");
                }
            }
            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}