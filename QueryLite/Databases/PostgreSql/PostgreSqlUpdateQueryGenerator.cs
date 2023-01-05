﻿using System;
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlUpdateQueryGenerator : IUpdateQueryGenerator {

        string IUpdateQueryGenerator.GetSql(UpdateQueryTemplate template, IDatabase database, IParameters? parameters) {

            StringBuilder sql = new StringBuilder("UPDATE ");

            string schemaName = database.SchemaMap(template.Table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(" AS ").Append(template.Table.Alias).Append(' ');

            {
                sql.Append(" SET ");

                bool first = true;

                foreach(SetValue setValue in template.SetValues) {

                    if(!first) {
                        sql.Append(',');
                    }
                    first = false;

                    PostgreSqlHelper.AppendColumnName(sql, setValue.Column);
                    sql.Append(" = ");

                    if(setValue.Value is IColumn rightColumn) {
                        sql.Append(rightColumn.Table.Alias).Append('.');
                        PostgreSqlHelper.AppendColumnName(sql, rightColumn);
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

            sql.Append(" FROM ");

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                PostgreSqlHelper.AppendEncase(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }
            PostgreSqlHelper.AppendEncase(sql, template.Table.TableName, forceEnclose: template.Table.Enclose);
            sql.Append(' ');

            if(template.Joins != null) {

                foreach(IJoin join in template.Joins) {

                    string joinType = join.JoinType switch {
                        JoinType.Join => "JOIN",
                        JoinType.LeftJoin => "LEFT JOIN",
                        _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                    };
                    sql.Append(' ').Append(joinType).Append(' ');

                    string joinSchemaName = database.SchemaMap(join.Table.SchemaName);

                    if(!string.IsNullOrWhiteSpace(joinSchemaName)) {
                        PostgreSqlHelper.AppendEncase(sql, joinSchemaName, forceEnclose: false);
                        sql.Append('.');
                    }
                    PostgreSqlHelper.AppendEncase(sql, join.Table.TableName, forceEnclose: join.Table.Enclose);
                    sql.Append(" AS ").Append(join.Table.Alias).Append(" ON ");
                    join.Condition.GetSql(sql, database, useAlias: true, parameters);
                }
            }

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: true, parameters);
            }

            if(template.ReturningColumns != null && template.ReturningColumns.Count > 0) {

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