using System;
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlDeleteQueryGenerator : IDeleteQueryGenerator {

        string IDeleteQueryGenerator.GetSql(DeleteQueryTemplate template, IDatabase database, IParameters? parameters) {

            if(template.Joins != null) {
                throw new Exception("Delete join syntax is not supported by PostgreSql");
            }
            StringBuilder sql = new StringBuilder("DELETE FROM ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(" AS ").Append(template.Table.Alias);

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: false, parameters);
            }

            if(template.ReturningColumns?.Count > 0) {

                sql.Append(" RETURNING ");

                for(int index = 0; index < template.ReturningColumns.Count; index++) {

                    IColumn column = template.ReturningColumns[index];

                    if(index > 0) {
                        sql.Append(',');
                    }
                    sql.Append(template.Table.Alias).Append('.');
                    PostgreSqlHelper.AppendColumnName(sql, column);
                }
            }
            return sql.ToString();
        }
    }
}