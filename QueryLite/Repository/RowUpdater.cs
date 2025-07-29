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
using QueryLite.Databases;
using System.Data.Common;
using System.Text;

namespace QueryLite {

    public interface IRowUpdaterCollection { }

    public class RowUpdaterCollection<TABLE, ROW> : IRowUpdaterCollection where TABLE : ATable where ROW : IEquatable<ROW> {

        //SQL Server and PostgreSql are the only two supported databases for now so it is quicker to have two fields than
        //manage a collection and implement locking etc.
        private RowUpdater<TABLE, ROW>? _sqlServerUpdater;
        private RowUpdater<TABLE, ROW>? _postgreSqlUpdater;

        public RowUpdater<TABLE, ROW>? GetUpdater(DatabaseType databaseType) {

            if(databaseType == DatabaseType.SqlServer) {
                return _sqlServerUpdater;
            }
            else if(databaseType == DatabaseType.PostgreSql) {
                return _postgreSqlUpdater;
            }
            throw new Exception($"Unsupported database type '{databaseType}'");
        }

        public void SetUpdater(DatabaseType databaseType, RowUpdater<TABLE, ROW> rowUpdaters) {

            if(databaseType == DatabaseType.SqlServer) {
                _sqlServerUpdater = rowUpdaters;
            }
            else if(databaseType == DatabaseType.PostgreSql) {
                _postgreSqlUpdater = rowUpdaters;
            }
            else {
                throw new Exception($"Unsupported database type '{databaseType}'");
            }
        }
    }

    public class RowUpdater<TABLE, ROW> where TABLE : ATable where ROW : IEquatable<ROW> {

        public string InsertSql { get; }
        public string UpdateSql { get; }
        public string DeleteSql { get; }

        private List<Func<ROW, DbParameter>> InsertParameterCreators { get; }
        private List<Func<ROW, DbParameter>> UpdateParameterCreators { get; }
        private List<Func<ROW, DbParameter>> WhereClauseParameterCreators { get; }


        public RowUpdater(TABLE table, List<ColumnAndSetter<ROW>> insertColumnAndSetters,
                           List<ColumnAndSetter<ROW>> updateColumnAndSetters,
                           List<ColumnAndSetter<ROW>> whereColumns,
                           IDatabase database) {

            int count = 0;

            foreach(ColumnAndSetter<ROW> cs in insertColumnAndSetters) {
                cs.ParameterName = ParamNameCache.GetName(count++);
            }
            foreach(ColumnAndSetter<ROW> cs in updateColumnAndSetters) {
                cs.ParameterName = ParamNameCache.GetName(count++);
            }
            foreach(ColumnAndSetter<ROW> cs in whereColumns) {
                cs.ParameterName = ParamNameCache.GetName(count++);
            }

            InsertSql = CreateInsertSql(database, table, insertColumnAndSetters);
            InsertParameterCreators = GenerateParameterCreators(insertColumnAndSetters, database.ParameterMapper);

            UpdateSql = CreateUpdateSql(database, table, updateColumnAndSetters, whereColumns);
            UpdateParameterCreators = GenerateParameterCreators(updateColumnAndSetters, database.ParameterMapper);
            WhereClauseParameterCreators = GenerateParameterCreators(whereColumns, database.ParameterMapper);

            DeleteSql = CreateDeleteSql(database, table, whereColumns);
        }

        /// <summary>
        /// Creates a list of create parameter functions for the provided list of columns.
        /// </summary>
        private static List<Func<ROW, DbParameter>> GenerateParameterCreators(List<ColumnAndSetter<ROW>> columnAndSetters, IPreparedParameterMapper parameterMapper) {

            List<Func<ROW, DbParameter>> list = new List<Func<ROW, DbParameter>>(columnAndSetters.Count);

            foreach(ColumnAndSetter<ROW> cs in columnAndSetters) {
                CreateParameterDelegate createParameter = parameterMapper!.GetCreateParameterDelegate(cs.Column.Type);
                list.Add(row => createParameter(cs.ParameterName!, cs.Setter(row)));
            }
            return list;
        }

        private static void SetParameters(ROW row, DbParameterCollection parameters, List<Func<ROW, DbParameter>> parameterCreators) {

            foreach(Func<ROW, DbParameter> creator in parameterCreators) {
                parameters.Add(creator(row));
            }
        }

        public async Task<int> InsertAsync(ROW newRow, Transaction transaction, QueryTimeout? timeout, CancellationToken cancellationToken) {

            using DbCommand command = transaction.CreateCommand(timeout ?? TimeoutLevel.ShortInsert);

            command.CommandText = InsertSql;

            SetParameters(newRow, command.Parameters, InsertParameterCreators);

            int rowsEffected = await command.ExecuteNonQueryAsync(cancellationToken);

            return rowsEffected;
        }

        public async Task<int> UpdateAsync(ROW oldRow, ROW newRow, Transaction transaction, QueryTimeout? timeout, CancellationToken cancellationToken) {

            using DbCommand command = transaction.CreateCommand(timeout ?? TimeoutLevel.ShortUpdate);

            command.CommandText = UpdateSql;

            SetParameters(newRow, command.Parameters, UpdateParameterCreators);

            SetParameters(oldRow, command.Parameters, WhereClauseParameterCreators);

            int rowsEffected = await command.ExecuteNonQueryAsync(cancellationToken);

            return rowsEffected;
        }

        public async Task<int> DeleteAsync(ROW existingRow, Transaction transaction, QueryTimeout? timeout, CancellationToken cancellationToken) {

            using DbCommand command = transaction.CreateCommand(timeout ?? TimeoutLevel.ShortDelete);

            command.CommandText = DeleteSql;

            SetParameters(existingRow, command.Parameters, WhereClauseParameterCreators);

            int rowsEffected = await command.ExecuteNonQueryAsync(cancellationToken);

            return rowsEffected;
        }

        public static string CreateInsertSql(IDatabase database, TABLE table, List<ColumnAndSetter<ROW>> insertColumns) {

            StringBuilder sql = StringBuilderCache.Acquire();

            sql.Append("INSERT INTO ");

            string schemaName = database.SchemaMap(table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                sql.Append('.');
            }
            SqlHelper.AppendEncloseTableName(sql, table);            

            sql.Append('(');

            for(int index = 0; index < insertColumns.Count; index++) {

                if(index > 0) {
                    sql.Append(',');
                }
                ColumnAndSetter<ROW> cs = insertColumns[index];
                SqlHelper.AppendEncloseColumnName(sql, cs.Column);
            }

            sql.Append(")VALUES(");

            for(int index = 0; index < insertColumns.Count; index++) {

                if(index > 0) {
                    sql.Append(',');
                }
                ColumnAndSetter<ROW> cs = insertColumns[index];
                sql.Append(cs.ParameterName);
            }
            sql.Append(')');

            string insertSql =  sql.ToString();

            StringBuilderCache.Release(sql);
            return insertSql;
        }

        public static string CreateUpdateSql(IDatabase database, TABLE table, List<ColumnAndSetter<ROW>> updateColumns, List<ColumnAndSetter<ROW>> whereClauseColumns) {

            StringBuilder sql = StringBuilderCache.Acquire();

            sql.Append("UPDATE ");

            string schemaName = database.SchemaMap(table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                sql.Append('.');
            }
            
            SqlHelper.AppendEncloseTableName(sql, table);
            
            sql.Append(" SET ");

            int columnCount = 0;

            for(int index = 0; index < updateColumns.Count; index++) {

                ColumnAndSetter<ROW> cs = updateColumns[index];

                if(columnCount > 0) {
                    sql.Append(',');
                }
                columnCount++;
                SqlHelper.AppendEncloseColumnName(sql, cs.Column);
                sql.Append('=').Append(cs.ParameterName);
            }
            GenerateWhereClause(whereClauseColumns, sql);

            string updateSql = sql.ToString();

            StringBuilderCache.Release(sql);
            return updateSql;
        }

        private static void GenerateWhereClause(List<ColumnAndSetter<ROW>> whereClauseColumns, StringBuilder sql) {

            sql.Append(" WHERE ");

            for(int index = 0; index < whereClauseColumns.Count; index++) {

                if(index > 0) {
                    sql.Append(" AND ");
                }
                ColumnAndSetter<ROW> cs = whereClauseColumns[index];
                GenerateWhereClauseCondition(sql, cs);
            }
        }

        private static void GenerateWhereClauseCondition(StringBuilder sql, ColumnAndSetter<ROW> cs) {

            bool isFloatingPoint =
                cs.Column.UnderlyingType == typeof(float) ||
                cs.Column.UnderlyingType == typeof(float?) ||
                cs.Column.UnderlyingType == typeof(double) ||
                cs.Column.UnderlyingType == typeof(double?);

            if(cs.Column.IsNullable) {
                sql.Append("((");
                SqlHelper.AppendEncloseColumnName(sql, cs.Column);
                sql.Append(" IS NULL AND ").Append(cs.ParameterName).Append(" IS NULL) OR (");
            }

            // Floating point comparison is not reliable so we need to take that into consideration
            if(isFloatingPoint) {
                sql.Append("ABS(");
                SqlHelper.AppendEncloseColumnName(sql, cs.Column);
                sql.Append('-').Append(cs.ParameterName).Append(") < 0.0000001");   //TODO: Check this value - Maybe different for float and double
            }
            else {
                SqlHelper.AppendEncloseColumnName(sql, cs.Column);
                sql.Append('=').Append(cs.ParameterName);
            }

            if(cs.Column.IsNullable) {
                sql.Append("))");
            }
        }

        public static string CreateDeleteSql(IDatabase database, TABLE table, List<ColumnAndSetter<ROW>> whereClauseColumns) {

            StringBuilder sql = StringBuilderCache.Acquire();

            sql.Append("DELETE FROM ");

            string schemaName = database.SchemaMap(table.SchemaName);

            if(!string.IsNullOrWhiteSpace(schemaName)) {
                SqlHelper.AppendEncloseSchemaName(sql, schemaName);
                sql.Append('.');
            }

            SqlHelper.AppendEncloseTableName(sql, table);

            GenerateWhereClause(whereClauseColumns, sql);

            string deleteSql = sql.ToString();

            StringBuilderCache.Release(sql);
            return deleteSql;
        }
    }
}