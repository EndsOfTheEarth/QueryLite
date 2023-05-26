/*
 * MIT License
 *
 * Copyright (c) 2023 EndsOfTheEarth
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/
using QueryLite.PreparedQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryLite.Databases.SqlServer {

    internal sealed class SqlServerCompiledSelectQueryGenerator : IPreparedQueryGenerator {

        string IPreparedQueryGenerator.GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, IParameterCollector<PARAMETERS> parameters) {
            return GetSql(template, database, parameters);
        }

        //string IQueryGenerator.GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, IParameterCollector<PARAMETERS> parameters) {
        //    return GetSql(template, database, parameters);
        //}

        internal static string GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, IParameterCollector<PARAMETERS> parameters) {

            //We need to start with the first query template
            while(template.ParentUnion != null) {
                template = template.ParentUnion;
            }

            StringBuilder sql = new StringBuilder(capacity: 256);

            while(true) {

                sql.Append("SELECT");

                if(template.IsDistinct) {
                    sql.Append(" DISTINCT");
                }

                bool useAliases = template.Joins != null && template.Joins.Count > 0;

                GenerateTopClause(sql, template);
                GenerateSelectClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateFromClause(sql, template, useAliases: useAliases, database);

                if(template.Joins != null) {
                    GenerateJoins(sql, template, useAliases: useAliases, database, parameters);
                }
                GenerateWhereClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateGroupByClause(sql, template, useAliases: useAliases);
                GenerateHavingClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateOrderByClause(sql, template, useAliases: useAliases, database, parameters);

                if(template.ChildUnion != null) {

                    if(template.ChildUnionType == UnionType.Union) {
                        sql.Append(" UNION ");
                    }
                    else if(template.ChildUnionType == UnionType.UnionAll) {
                        sql.Append(" UNION ALL ");
                    }
                    else {
                        throw new Exception($"Unknown {nameof(template.ChildUnionType)} type. Value = '{template.ChildUnionType}'");
                    }
                    template = template.ChildUnion;
                }
                else {
                    break;
                }
            }
            return sql.ToString();
        }

        private static void GenerateSelectClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, IParameterCollector<PARAMETERS> parameters) {

            sql.Append(' ');

            bool setComma = false;

            foreach(IField field in template.SelectFields) {

                if(field is IColumn column) {

                    if(setComma) {
                        sql.Append(',');
                    }
                    else {
                        setComma = true;
                    }
                    if(useAliases) {
                        sql.Append(column.Table.Alias).Append('.');
                    }
                    SqlServerHelper.AppendColumnName(sql, column);
                }
                else if(field is IFunction function) {

                    if(setComma) {
                        sql.Append(',');
                    }
                    else {
                        setComma = true;
                    }                    
                    sql.Append(function.GetSql(database, useAlias: useAliases, parameters: null));
                }
                else {
                    throw new Exception($"Unkown field type. Type = {field}");
                }
            }
        }

        private static void GenerateTopClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template) {

            if(template.TopRows != null) {
                sql.Append(" TOP ").Append(template.TopRows.Value);
            }
        }
        private static void GenerateFromClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database) {

            if(template.FromTable == null) {
                throw new Exception($"From table is null. Please check query");
            }
            sql.Append(" FROM ");

            string schemaName = database.SchemaMap(template.FromTable.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                sql.Append('.');
            }

            SqlServerHelper.AppendEnclose(sql, template.FromTable.TableName, forceEnclose: template.FromTable.Enclose);

            if(useAliases) {
                sql.Append(" AS ").Append(template.FromTable.Alias);
            }

            if(template.Hints != null && template.Hints.Length > 0) {

                sql.Append(" WITH(");

                int hintCount = 0;

                foreach(SqlServerTableHint hint in template.Hints) {

                    if(hintCount > 0) {
                        sql.Append(',');
                    }
                    sql.Append(hint.ToString());
                    hintCount++;
                }
                sql.Append(')');
            }
        }

        private static void GenerateJoins<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, IParameterCollector<PARAMETERS> parameters) {

            if(template.Joins == null) {
                return;
            }

            foreach(IPreparedJoin<PARAMETERS> join in template.Joins) {

                sql.Append(join.JoinType switch {
                    JoinType.Join => " JOIN ",
                    JoinType.LeftJoin => " LEFT JOIN ",
                    _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                });

                string schemaName = database.SchemaMap(join.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlServerHelper.AppendEnclose(sql, schemaName, forceEnclose: false);
                    sql.Append('.');
                }
                SqlServerHelper.AppendEnclose(sql, join.Table.TableName, forceEnclose: join.Table.Enclose);

                if(useAliases) {
                    sql.Append(" AS ").Append(join.Table.Alias);
                }
                sql.Append(" ON ");
                join.Condition.GetSql(sql, parameters, useAlias: useAliases);
            }
        }

        private static void GenerateWhereClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, IParameterCollector<PARAMETERS> parameters) {

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, parameters, useAlias: useAliases);
            }
        }

        private static void GenerateGroupByClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases) {

            if(template.GroupByFields != null && template.GroupByFields.Length > 0) {

                sql.Append(" GROUP BY ");

                bool setComma = false;

                foreach(ISelectable field in template.GroupByFields) {

                    if(field is IColumn column) {

                        if(setComma) {
                            sql.Append(',');
                        }
                        else {
                            setComma = true;
                        }
                        if(useAliases) {
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlServerHelper.AppendColumnName(sql, column);
                    }
                    else {
                        throw new Exception($"Unkown field type. Type = {field}");
                    }
                }
            }
        }

        private static void GenerateHavingClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, IParameterCollector<PARAMETERS> parameters) {

            if(template.HavingCondition != null) {
                sql.Append(" HAVING ");
                template.HavingCondition.GetSql(sql, parameters, useAlias: useAliases);
            }
        }
        private static void GenerateOrderByClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, IParameterCollector<PARAMETERS> parameters) {

            if(template.OrderByFields != null && template.OrderByFields.Length > 0) {

                sql.Append(" ORDER BY ");

                bool setComma = false;

                foreach(IOrderByColumn orderByColumn in template.OrderByFields) {

                    IField field = orderByColumn.Field;

                    if(setComma) {
                        sql.Append(',');
                    }
                    else {
                        setComma = true;
                    }

                    if(field is IColumn column) {

                        if(useAliases) {
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlServerHelper.AppendColumnName(sql, column);
                    }
                    else if(field is IFunction function) {
                        sql.Append(function.GetSql(database, useAlias: useAliases, parameters: null));
                    }
                    else {
                        throw new Exception($"Unkown field type. Type = {field}");
                    }
                    sql.Append(orderByColumn.OrderBy switch {
                        OrderBy.ASC => " ASC",
                        OrderBy.DESC => " DESC",
                        OrderBy.Default => "",
                        _ => throw new Exception($"Unknown {nameof(OrderBy)} type. Type: '{field.OrderBy}'")
                    });
                }
            }

            if(template.Options != null && template.Options.Length > 0) {

                sql.Append(" OPTION (");

                bool setComma = false;

                if(!string.IsNullOrEmpty(template.OptionLabelName)) {

                    sql.Append("LABEL = '").Append(Helpers.EscapeForSql(template.OptionLabelName)).Append('\'');
                    setComma = true;
                }

                foreach(SqlServerQueryOption option in template.Options) {

                    if(setComma) {
                        sql.Append(',');
                    }
                    else {
                        setComma = true;
                    }

                    switch(option) {

                        case SqlServerQueryOption.HASH_JOIN:
                            sql.Append("HASH JOIN");
                            break;
                        case SqlServerQueryOption.LOOP_JOIN:
                            sql.Append("LOOP JOIN");
                            break;
                        case SqlServerQueryOption.MERGE_JOIN:
                            sql.Append("MERGE JOIN");
                            break;
                        case SqlServerQueryOption.FORCE_ORDER:
                            sql.Append("FORCE ORDER");
                            break;
                        case SqlServerQueryOption.FORCE_EXTERNALPUSHDOWN:
                            sql.Append("FORCE EXTERNALPUSHDOWN");
                            break;
                        case SqlServerQueryOption.DISABLE_EXTERNALPUSHDOWN:
                            sql.Append("DISABLE EXTERNALPUSHDOWN");
                            break;
                        default:
                            throw new Exception($"Unknown {nameof(SqlServerQueryOption)}. Value = '{option}'");
                    }
                }
                sql.Append(')');
            }
        }

    }

    //public static class SqlServerHelper {

    //    public static void AppendColumnName(StringBuilder sql, IColumn column) {

    //        if(column.Enclose || column.ColumnName.Contains(' ')) {
    //            sql.Append('[').Append(column.ColumnName).Append(']');
    //        }
    //        else {
    //            sql.Append(column.ColumnName);
    //        }
    //    }

    //    public static void AppendEnclose(StringBuilder sql, string value, bool forceEnclose) {

    //        if(forceEnclose || value.Contains(' ')) {
    //            sql.Append('[').Append(value).Append(']');
    //        }
    //        else {
    //            sql.Append(value);
    //        }
    //    }
    //}
}