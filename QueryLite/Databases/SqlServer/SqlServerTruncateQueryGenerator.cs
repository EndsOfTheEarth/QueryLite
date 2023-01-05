using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerTruncateQueryGenerator : ITruncateQueryGenerator {

        string ITruncateQueryGenerator.GetSql(TruncateQueryTemplate template, IDatabase database, IParameters? parameters) {

            StringBuilder sql = new StringBuilder("TRUNCATE TABLE ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            return sql.ToString();
        }
    }
}