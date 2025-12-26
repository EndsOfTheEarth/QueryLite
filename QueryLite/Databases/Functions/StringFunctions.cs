using System.Text;

namespace QueryLite.Databases.Functions {

    public sealed class Length : Function<int> {

        private ISelectable<string> Selectable { get; }

        public Length(ISelectable<string> selectable) : base(name: "LENGTH") {
            Selectable = selectable;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            StringBuilder sql = StringBuilderCache.Acquire();

            if(database.DatabaseType == DatabaseType.SqlServer) {
                sql.Append($"LEN(");
            }
            else {
                sql.Append($"LENGTH(");
            }

            ConditionHelper.AppendSqlValue(Selectable, sql, database, useAlias: useAlias, parameters);

            sql.Append(')');

            return StringBuilderCache.ToStringAndRelease(sql);
        }
    }
}