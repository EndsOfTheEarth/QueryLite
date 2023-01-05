using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlTruncateQueryGenerator : ITruncateQueryGenerator {

        string ITruncateQueryGenerator.GetSql(TruncateQueryTemplate template, IDatabase database, IParameters? parameters) {

            StringBuilder sql = new StringBuilder("TRUNCATE TABLE ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);

            return sql.ToString();
        }
    }
}