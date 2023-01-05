using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlInsertQueryGenerator : IInsertQueryGenerator {

        string IInsertQueryGenerator.GetSql(InsertQueryTemplate template, IDatabase database, IParameters? parameters) {

            StringBuilder sql = new StringBuilder("INSERT INTO ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append('(');

            {
                bool first = true;

                foreach(SetValue insertSet in template.SetValues) {

                    if(!first) {
                        sql.Append(',');
                    }
                    else {
                        first = false;
                    }
                    PostgreSqlHelper.AppendColumnName(sql, insertSet.Column);
                }
            }

            sql.Append(") VALUES(");

            {

                bool first = true;

                foreach(SetValue insertSet in template.SetValues) {

                    if(!first) {
                        sql.Append(',');
                    }
                    else {
                        first = false;
                    }

                    if(insertSet.Value is IColumn rightColumn) {
                        sql.Append(rightColumn.Table.Alias).Append('.');
                        PostgreSqlHelper.AppendColumnName(sql, rightColumn);
                    }
                    else if(insertSet.Value is IFunction rightFunction) {
                        sql.Append(rightFunction.GetSql(database, useAlias: true, parameters));
                    }
                    else if(parameters == null) {

                        if(insertSet.Value != null) {
                            sql.Append(database.ConvertToSql(insertSet.Value));
                        }
                        else {
                            sql.Append(" NULL");
                        }
                    }
                    else {
                        parameters.Add(database, insertSet.Column.Type, insertSet.Value, out string paramName);
                        sql.Append(paramName);
                    }
                }
            }

            sql.Append(')');

            {
                if(template.ReturningFields != null && template.ReturningFields.Count > 0) {

                    sql.Append(" RETURNING ");

                    bool first = true;

                    foreach(IColumn column in template.ReturningFields) {

                        if(!first) {
                            sql.Append(',');
                        }
                        else {
                            first = false;
                        }
                        PostgreSqlHelper.AppendColumnName(sql, column);
                    }
                }
            }
            return sql.ToString();
        }
    }
}