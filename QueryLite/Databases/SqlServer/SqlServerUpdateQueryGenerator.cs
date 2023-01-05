using System;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerUpdateQueryGenerator : IUpdateQueryGenerator {

        string IUpdateQueryGenerator.GetSql(UpdateQueryTemplate template, IDatabase database, IParameters? parameters) {

            StringBuilder sql = new StringBuilder("UPDATE ", capacity: 256);

            sql.Append(template.Table.Alias);

            {
                sql.Append(" SET ");

                bool first = true;

                foreach(SetValue setValue in template.SetValues) {

                    if(!first) {
                        sql.Append(',');
                    }
                    else {
                        first = false;
                    }

                    SqlServerHelper.AppendColumnName(sql, setValue.Column);
                    sql.Append('=');

                    if(setValue.Value is IColumn rightColumn) {
                        sql.Append(rightColumn.Table.Alias).Append('.');
                        SqlServerHelper.AppendColumnName(sql, rightColumn);
                    }
                    else if(setValue.Value is IFunction rightFunction) {
                        sql.Append(rightFunction.GetSql(database, useAlias: true, parameters));
                    }
                    else if(parameters == null) {

                        if(setValue.Value != null) {
                            sql.Append(database.ConvertToSql(setValue.Value));
                        }
                        else {
                            sql.Append(" NULL");
                        }
                    }
                    else {
                        parameters.Add(database, setValue.Column.Type, setValue.Value, out string paramName);
                        sql.Append(paramName);
                    }
                }
            }

            if(template.ReturningColumns?.Count > 0) {

                sql.Append(" OUTPUT ");

                for(int index = 0; index < template.ReturningColumns.Count; index++) {

                    IColumn column = template.ReturningColumns[index];

                    if(index > 0) {
                        sql.Append(',');
                    }
                    sql.Append(" INSERTED.");     //Note INSERTED returns the updated value
                    SqlServerHelper.AppendColumnName(sql, column);
                }
            }

            sql.Append(" FROM ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(" AS ").Append(template.Table.Alias);

            if(template.Joins != null) {

                foreach(IJoin join in template.Joins) {

                    sql.Append(join.JoinType switch {
                        JoinType.Join => " JOIN ",
                        JoinType.LeftJoin => " LEFT JOIN ",
                        _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                    });

                    string joinSchemaName = database.SchemaMap(join.Table.SchemaName);

                    if(!string.IsNullOrWhiteSpace(joinSchemaName)) {
                        SqlServerHelper.AppendEnclose(sql, joinSchemaName, forceEnclose: false);
                        sql.Append('.');
                    }
                    SqlServerHelper.AppendEnclose(sql, join.Table.TableName, forceEnclose: join.Table.Enclose);
                    sql.Append(" AS ").Append(join.Table.Alias).Append(" ON ");
                    join.Condition.GetSql(sql, database, useAlias: true, parameters);
                }   
            }
            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: true, parameters);
            }
            return sql.ToString();
        }
    }
}