/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
using System.Text;

namespace QueryLite.Databases.Sqlite {

    internal sealed class SqlitePreparedSelectQueryGenerator : IPreparedQueryGenerator {

        string IPreparedQueryGenerator.GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, PreparedParameterList<PARAMETERS> parameters) {
            return GetSql(template, database, parameters);
        }

        internal static string GetSql<PARAMETERS, RESULT>(PreparedQueryTemplate<PARAMETERS, RESULT> template, IDatabase database, PreparedParameterList<PARAMETERS> parameters) {

            //We need to start with the first query template
            while(template.ParentUnion != null) {
                template = template.ParentUnion;
            }

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            while(true) {

                sql.Append("SELECT");

                if(template.IsDistinct) {
                    sql.Append(" DISTINCT");
                }

                bool useAliases = template.Joins != null && template.Joins.Count > 0;

                GenerateSelectClause(sql, template, useAliases: useAliases, database);
                GenerateFromClause(sql, template, useAliases: useAliases, database);

                if(template.Joins != null) {
                    GenerateJoins(sql, template, useAliases: useAliases, database, parameters);
                }
                GenerateWhereClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateGroupByClause(sql, template, useAliases: useAliases);
                GenerateHavingClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateOrderByClause(sql, template, useAliases: useAliases, database);
                GenerateLimitClause(sql, template);
                GenerateForClause(sql, template, useAliases: useAliases);

                if(template.ChildUnion != null) {

                    if(template.ChildUnionType == UnionType.Union) {
                        sql.Append(" UNION ");
                    }
                    else if(template.ChildUnionType == UnionType.UnionAll) {
                        sql.Append(" UNION ALL ");
                    }
                    else {
                        throw new Exception($"Unknown {nameof(template.ChildUnionType)} type. Value {template.ChildUnionType}");
                    }
                    template = template.ChildUnion;
                }
                else {
                    break;
                }
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }

        private static void GenerateSelectClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database) {

            sql.Append(' ');

            bool isFirst = true;

            foreach(IField field in template.SelectFields) {

                if(field is IColumn column) {

                    if(!isFirst) {
                        sql.Append(',');
                    }
                    else {
                        isFirst = false;
                    }
                    if(useAliases) {
                        sql.Append(column.Table.Alias).Append('.');
                    }
                    SqlHelper.AppendEncloseColumnName(sql, column, EncloseWith.SquareBracket);
                }
                else if(field is IFunction function) {

                    if(!isFirst) {
                        sql.Append(',');
                    }
                    else {
                        isFirst = false;
                    }
                    sql.Append(function.GetSql(database, useAlias: useAliases, parameters: null));
                }
                else {
                    throw new Exception($"Unknown field type. Type = {field}");
                }
            }
        }

        private static void GenerateFromClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database) {

            if(template.FromTable == null) {
                throw new Exception($"From table is null. Please check query");
            }
            sql.Append(" FROM ");

            string schemaName = database.SchemaMap(template.FromTable.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName, EncloseWith.SquareBracket);
                sql.Append('.');
            }

            SqlHelper.AppendEncloseTableName(sql, template.FromTable, EncloseWith.SquareBracket);

            if(useAliases) {
                sql.Append(" AS ").Append(template.FromTable.Alias);
            }
        }

        private static void GenerateJoins<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, PreparedParameterList<PARAMETERS> parameters) {

            if(template.Joins == null) {
                return;
            }

            for(int index = 0; index < template.Joins.Count; index++) {

                IPreparedJoin<PARAMETERS> join = template.Joins[index];

                sql.Append(join.JoinType switch {
                    JoinType.Join => " JOIN ",
                    JoinType.LeftJoin => " LEFT JOIN ",
                    _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                });

                string schemaName = database.SchemaMap(join.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName, EncloseWith.SquareBracket);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, join.Table, EncloseWith.SquareBracket);

                if(useAliases) {
                    sql.Append(" AS ").Append(join.Table.Alias);
                }
                sql.Append(" ON ");
                join.Condition.GetSql(sql, database, parameters, useAlias: useAliases);
            }
        }

        private static void GenerateWhereClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, PreparedParameterList<PARAMETERS> parameters) {

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, parameters, useAlias: useAliases);
            }
        }

        private static void GenerateGroupByClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases) {

            if(template.GroupByFields != null && template.GroupByFields.Length > 0) {

                sql.Append(" GROUP BY ");

                bool isFirst = true;

                foreach(ISelectable field in template.GroupByFields) {

                    if(field is IColumn column) {

                        if(!isFirst) {
                            sql.Append(',');
                        }
                        else {
                            isFirst = false;
                        }
                        if(useAliases) {
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlHelper.AppendEncloseColumnName(sql, column, EncloseWith.SquareBracket);
                    }
                    else {
                        throw new Exception($"Unknown field type. Type = {field}");
                    }
                }
            }
        }

        private static void GenerateHavingClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database, PreparedParameterList<PARAMETERS> parameters) {

            if(template.HavingCondition != null) {
                sql.Append(" HAVING ");
                template.HavingCondition.GetSql(sql, database, parameters, useAlias: useAliases);
            }
        }

        private static void GenerateOrderByClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases, IDatabase database) {

            if(template.OrderByFields != null && template.OrderByFields.Length > 0) {

                sql.Append(" ORDER BY ");

                bool isFirst = true;

                foreach(IOrderByColumn orderByColumn in template.OrderByFields) {

                    IField field = orderByColumn.Field;

                    if(!isFirst) {
                        sql.Append(',');
                    }
                    else {
                        isFirst = false;
                    }

                    if(field is IColumn column) {

                        if(template.ParentUnion == null && useAliases) {  //Cannot alias in the order by column when this is a union query
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlHelper.AppendEncloseColumnName(sql, column, EncloseWith.SquareBracket);
                    }
                    else if(field is IFunction function) {
                        sql.Append(function.GetSql(database, useAlias: useAliases, null));
                    }
                    else {
                        throw new Exception($"Unknown field type. Type = {field}");
                    }

                    sql.Append(orderByColumn.OrderBy switch {
                        OrderBy.ASC => " ASC",
                        OrderBy.DESC => " DESC",
                        OrderBy.Default => "",
                        _ => throw new Exception($"Unknown {nameof(OrderBy)} type. Type: '{field.OrderBy}'")
                    });
                }
            }
        }
        private static void GenerateLimitClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template) {

            if(template.TopRows != null) {
                sql.Append(" LIMIT ").Append(template.TopRows.Value);
            }
        }

        private static void GenerateForClause<PARAMETERS, RESULT>(StringBuilder sql, PreparedQueryTemplate<PARAMETERS, RESULT> template, bool useAliases) {

            if(template.ForType != null) {

                sql.Append(" FOR ");

                switch(template.ForType.Value) {
                    case ForType.UPDATE:
                        sql.Append("UPDATE");
                        break;
                    case ForType.NO_KEY_UPDATE:
                        sql.Append("NO KEY UPDATE");
                        break;
                    case ForType.SHARE:
                        sql.Append("SHARE");
                        break;
                    case ForType.KEY_SHARE:
                        sql.Append("KEY SHARE");
                        break;
                    default:
                        throw new Exception($"Unknown {nameof(template.ForType)} type {template.ForType}");
                }

                if(template.OfTables != null && template.OfTables.Length > 0) {

                    sql.Append(" OF ");

                    bool isFirst = true;

                    foreach(ITable table in template.OfTables) {

                        if(!isFirst) {
                            sql.Append(',');
                        }
                        else {
                            isFirst = false;
                        }
                        if(useAliases) {
                            sql.Append(table.Alias);
                        }
                    }
                }

                if(template.WaitType != null) {

                    switch(template.WaitType.Value) {
                        case WaitType.WAIT:
                            //Do nothing as this is the default value
                            break;
                        case WaitType.NOWAIT:
                            sql.Append(" NOWAIT");
                            break;
                        case WaitType.SKIP_LOCKED:
                            sql.Append(" SKIP LOCKED");
                            break;
                        default:
                            throw new Exception($"Unknown {nameof(template.WaitType)} type {template.WaitType}");
                    }
                }
            }
        }
    }
}