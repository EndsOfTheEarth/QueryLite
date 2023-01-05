using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerInsertQueryGenerator : IInsertQueryGenerator {

        string IInsertQueryGenerator.GetSql(InsertQueryTemplate template, IDatabase database, IParameters? parameters) {

            StringBuilder sql = new StringBuilder("INSERT INTO ", capacity: 256);

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            SqlServerHelper.AppendEnclose(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(" (");

            {
                bool first = true;

                foreach(SetValue insertSet in template.SetValues) {

                    if(!first) {
                        sql.Append(',');
                    }
                    first = false;
                    SqlServerHelper.AppendColumnName(sql, insertSet.Column);
                }
            }

            sql.Append(')');

            {
                if(template.ReturningFields != null && template.ReturningFields.Count > 0) {

                    sql.Append(" OUTPUT ");

                    bool first = true;

                    foreach(IColumn column in template.ReturningFields) {

                        if(!first) {
                            sql.Append(',');
                        }
                        else {
                            first = false;
                        }
                        sql.Append(" INSERTED.");
                        SqlServerHelper.AppendColumnName(sql, column);
                    }
                }
            }

            sql.Append(" VALUES (");

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
                        SqlServerHelper.AppendColumnName(sql, rightColumn);
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
            return sql.ToString();
        }
    }
}