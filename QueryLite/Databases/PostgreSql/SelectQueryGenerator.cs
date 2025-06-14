/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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
using System;
using System.Text;

namespace QueryLite.Databases.PostgreSql {

    internal sealed class PostgreSqlSelectQueryGenerator : IQueryGenerator {

        string IQueryGenerator.GetSql<RESULT>(SelectQueryTemplate<RESULT> template, IDatabase database, IParametersBuilder? parameters) {
            return GetSql(template, database, parameters);
        }

        internal static string GetSql<RESULT>(SelectQueryTemplate<RESULT> template, IDatabase database, IParametersBuilder? parameters) {

            //We need to start with the first query template
            while(template.Extras != null && template.Extras.ParentUnion != null) {
                template = template.Extras.ParentUnion;
            }

            StringBuilder sql = StringBuilderCache.Acquire(capacity: 256);

            while(true) {

                sql.Append("SELECT");

                if(template.IsDistinct) {
                    sql.Append(" DISTINCT");
                }

                bool useAliases = template.Joins != null && template.Joins.Count > 0;

                GenerateSelectClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateFromClause(sql, template, useAliases: useAliases, database);

                if(template.Joins != null) {
                    GenerateJoins(sql, template, useAliases: useAliases, database, parameters);
                }
                GenerateWhereClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateGroupByClause(sql, template, useAliases: useAliases);
                GenerateHavingClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateOrderByClause(sql, template, useAliases: useAliases, database, parameters);
                GenerateLimitClause(sql, template);
                GenerateForClause(sql, template, useAliases: useAliases);

                if(template.Extras != null && template.Extras.ChildUnion != null) {

                    if(template.Extras.ChildUnionType == UnionType.Union) {
                        sql.Append(" UNION ");
                    }
                    else if(template.Extras.ChildUnionType == UnionType.UnionAll) {
                        sql.Append(" UNION ALL ");
                    }
                    else {
                        throw new Exception($"Unknown {nameof(template.Extras.ChildUnionType)} type. Value {template.Extras.ChildUnionType}");
                    }
                    template = template.Extras.ChildUnion;
                }
                else {
                    break;
                }
            }
            return StringBuilderCache.ToStringAndRelease(sql);
        }

        [ThreadStatic]
        private static PostgreSqlSelectFieldCollector? _cachedCollectorInstance;

        private static void GenerateSelectClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases, IDatabase database, IParametersBuilder? parameters) {

            if(template.SelectFunction == null && (template.Extras == null || template.Extras.NestedSelectFields == null || template.Extras.NestedSelectFields.Count == 0)) {
                throw new Exception($"{nameof(template.SelectFunction)} and {nameof(template.Extras.NestedSelectFields)} cannot both be null or empty");
            }

            sql.Append(' ');

            if(template.SelectFunction != null) {

                if(Settings.EnableCollectorCaching) {

                    if(_cachedCollectorInstance == null) {
                        _cachedCollectorInstance = new PostgreSqlSelectFieldCollector(database, parameters, useAlias: useAliases, sql);
                    }
                    else {
                        _cachedCollectorInstance.Reset(database, parameters, useAlias: useAliases, sql);
                    }
                    template.SelectFunction(_cachedCollectorInstance);
                    _cachedCollectorInstance.Clear();
                }
                else {
                    template.SelectFunction(new PostgreSqlSelectFieldCollector(database, parameters, useAlias: useAliases, sql));
                }
            }
            else if(template.Extras != null) {

                for(int index = 0; index < template.Extras.NestedSelectFields!.Count; index++) {

                    if(index > 0) {
                        sql.Append(',');
                    }

                    IField field = template.Extras.NestedSelectFields[index];

                    if(field is IColumn column) {

                        if(useAliases) {
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlHelper.AppendEncloseColumnName(sql, column);
                    }
                    else if(field is IFunction function) {

                        sql.Append(function.GetSql(database, useAlias: useAliases, parameters));
                    }
                    else {
                        throw new Exception($"Unknown field type. Type = {field}");
                    }
                }
            }
        }

        private static void GenerateFromClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases, IDatabase database) {

            if(template.FromTable == null) {
                throw new Exception($"From table is null. Please check query");
            }
            sql.Append(" FROM ");

            string schemaName = database.SchemaMap(template.FromTable.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                sql.Append('.');
            }

            SqlHelper.AppendEncloseTableName(sql, template.FromTable);

            if(useAliases) {
                sql.Append(" AS ").Append(template.FromTable.Alias);
            }
        }

        private static void GenerateJoins<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases, IDatabase database, IParametersBuilder? parameters) {

            if(template.Joins == null) {
                return;
            }

            foreach(IJoin join in template.Joins) {

                sql.Append(join.JoinType switch {
                    JoinType.Join => " JOIN ",
                    JoinType.LeftJoin => " LEFT JOIN ",
                    _ => throw new Exception($"Unknown join type. Type = {join.JoinType}")
                });

                string schemaName = database.SchemaMap(join.Table.SchemaName);

                if(!string.IsNullOrWhiteSpace(schemaName)) {
                    SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                    sql.Append('.');
                }
                SqlHelper.AppendEncloseTableName(sql, join.Table);

                if(useAliases) {
                    sql.Append(" AS ").Append(join.Table.Alias);
                }
                sql.Append(" ON ");
                join.Condition.GetSql(sql, database, useAlias: useAliases, parameters);
            }
        }

        private static void GenerateWhereClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases, IDatabase database, IParametersBuilder? parameters) {

            if(template.WhereCondition != null) {
                sql.Append(" WHERE ");
                template.WhereCondition.GetSql(sql, database, useAlias: useAliases, parameters);
            }
        }

        private static void GenerateGroupByClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases) {

            if(template.Extras != null && template.Extras.GroupByFields != null && template.Extras.GroupByFields.Length > 0) {

                sql.Append(" GROUP BY ");

                bool isFirst = true;

                foreach(ISelectable field in template.Extras.GroupByFields) {

                    if(field is IColumn column) {

                        if(!isFirst) {
                            sql.Append(',');
                        }
                        else {
                            isFirst = true;
                        }
                        if(useAliases) {
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlHelper.AppendEncloseColumnName(sql, column);
                    }
                    else {
                        throw new Exception($"Unknown field type. Type = {field}");
                    }
                }
            }
        }

        private static void GenerateHavingClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases, IDatabase database, IParametersBuilder? parameters) {

            if(template.Extras != null && template.Extras.HavingCondition != null) {
                sql.Append(" HAVING ");
                template.Extras.HavingCondition.GetSql(sql, database, useAlias: useAliases, parameters);
            }
        }

        private static void GenerateOrderByClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases, IDatabase database, IParametersBuilder? parameters) {

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

                        if((template.Extras == null || template.Extras.ParentUnion == null) && useAliases) {  //Cannot alias in the order by column when this is a union query
                            sql.Append(column.Table.Alias).Append('.');
                        }
                        SqlHelper.AppendEncloseColumnName(sql, column);
                    }
                    else if(field is IFunction function) {
                        sql.Append(function.GetSql(database, useAlias: useAliases, parameters));
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
        private static void GenerateLimitClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template) {

            if(template.TopRows != null) {
                sql.Append(" LIMIT ").Append(template.TopRows.Value);
            }
        }

        private static void GenerateForClause<RESULT>(StringBuilder sql, SelectQueryTemplate<RESULT> template, bool useAliases) {

            if(template.Extras != null && template.Extras.ForType != null) {

                sql.Append(" FOR ");

                switch(template.Extras.ForType.Value) {
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
                        throw new Exception($"Unknown {nameof(template.Extras.ForType)} type {template.Extras.ForType}");
                }

                if(template.Extras.OfTables != null && template.Extras.OfTables.Length > 0) {

                    sql.Append(" OF ");

                    bool isFirst = true;
                    foreach(ITable table in template.Extras.OfTables) {

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

                if(template.Extras.WaitType != null) {

                    switch(template.Extras.WaitType.Value) {
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
                            throw new Exception($"Unknown {nameof(template.Extras.WaitType)} type {template.Extras.WaitType}");
                    }
                }
            }
        }
    }
}