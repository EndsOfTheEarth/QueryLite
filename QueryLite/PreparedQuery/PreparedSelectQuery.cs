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
using QueryLite.Databases.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace QueryLite.PreparedQuery {

    public static class QueryTest {

        public static PreparedSelect<ITEM> Query<ITEM>(DatabaseType databaseType) => new PreparedSelect<ITEM>();
    }

    public class PreparedSelect<ITEM> {
        public IPreparedDistinct<ITEM, RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc) {
            return new SqlServerQueryBuilder<ITEM, RESULT>(selectFunc);
        }
    }



    //public interface ICompileSelect<ITEM> {

    //    IPreparedDistinct<ITEM, RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc);
    //}

    public interface IPreparedDistinct<ITEM, RESULT> : IPreparedTop<ITEM, RESULT> {

        IPreparedTop<ITEM, RESULT> Distinct { get; }
    }

    public interface IPreparedTop<ITEM, RESULT> : IPreparedFrom<ITEM, RESULT> {

        /// <summary>
        /// Return TOP n rows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IPreparedFrom<ITEM, RESULT> Top(int rows);
    }
    public interface IPreparedFrom<ITEM, RESULT> {

        /// <summary>
        /// From table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedHint<ITEM, RESULT> From(ITable table);
    }

    public interface IPreparedHint<ITEM, RESULT> : IPreparedJoin<ITEM, RESULT> {

        /// <summary>
        /// The 'With' option only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public IPreparedJoin<ITEM, RESULT> With(params SqlServerTableHint[] hints);
    }


    public interface IPreparedJoin<ITEM, RESULT> : IPreparedWhere<ITEM, RESULT> {

        /// <summary>
        /// Join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedJoinOn<ITEM, RESULT> Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IPreparedJoinOn<ITEM, RESULT> LeftJoin(ITable table);
    }

    public interface IPreparedJoinOn<ITEM, RESULT> {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        IPreparedJoin<ITEM, RESULT> On(ICondition on);
    }

    public interface IPreparedWhere<ITEM, RESULT> : IPreparedGroupBy<ITEM, RESULT> {

        /// <summary>
        /// Where condition clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IPreparedGroupBy<ITEM, RESULT> Where(APreparedCondition<ITEM>? condition);
    }

    public interface IPreparedGroupBy<ITEM, RESULT> : IPreparedHaving<ITEM, RESULT> {

        /// <summary>
        /// Group by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IPreparedHaving<ITEM, RESULT> GroupBy(params ISelectable[] columns);
    }

    public interface IPreparedHaving<ITEM, RESULT> : IPreparedOrderBy<ITEM, RESULT> {

        /// <summary>
        /// Having clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IPreparedOrderBy<ITEM, RESULT> Having(ICondition condition);
    }

    public interface IPreparedOrderBy<ITEM, RESULT> : IPreparedFor<ITEM, RESULT> {

        /// <summary>
        /// Order by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IPreparedFor<ITEM, RESULT> OrderBy(params IOrderByColumn[] columns);

        /// <summary>
        /// Union query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        IPreparedDistinct<ITEM, RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc);

        /// <summary>
        /// Union all query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        IPreparedDistinct<ITEM, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc);
    }

    public interface IPreparedFor<ITEM, RESULT> : IPreparedOption<ITEM, RESULT> {

        /// <summary>
        /// FOR caluse. PostgreSql only
        /// </summary>
        /// <param name="forType"></param>
        /// <param name="ofTables"></param>
        /// <param name="waitType"></param>
        /// <returns></returns>
        IPreparedOption<ITEM, RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType);
    }

    public interface IPreparedOption<ITEM, RESULT> : ICompileQuery<ITEM, RESULT> {

        /// <summary>
        /// Sql server OPTION syntax. Note: 'Option' only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        ICompileQuery<ITEM, RESULT> Option(params SqlServerQueryOption[] options);

        /// <summary>
        /// Sql server OPTION syntax. Note: 'Option' only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        ICompileQuery<ITEM, RESULT> Option(string labelName, params SqlServerQueryOption[] options);
    }

    public interface ICompileQuery<ITEM, RESULT> {

        IPreparedQueryExecute<ITEM, RESULT> Build();
    }

    

    

    public class SqlServerQueryBuilder<ITEM, RESULT> : IPreparedDistinct<ITEM, RESULT>, IPreparedTop<ITEM, RESULT>, IPreparedFrom<ITEM, RESULT>, IPreparedHint<ITEM, RESULT>,
                                                        IPreparedJoin<ITEM, RESULT>, IPreparedJoinOn<ITEM, RESULT>, IPreparedWhere<ITEM, RESULT>, IPreparedGroupBy<ITEM, RESULT>,
                                                        IPreparedHaving<ITEM, RESULT>, IPreparedOrderBy<ITEM, RESULT>, IPreparedFor<ITEM, RESULT>, IPreparedOption<ITEM, RESULT>,
                                                        ICompileQuery<ITEM, RESULT>, IPreparedQueryExecute<ITEM, RESULT> {

        private Func<IResultRow, RESULT> _selectFunc;
        private StringBuilder _sql = new StringBuilder();

        private string? _sqlQuery;

        private List<IParameter<ITEM>> _parameters = new List<IParameter<ITEM>>();


        public SqlServerQueryBuilder(Func<IResultRow, RESULT> selectFunc) {
            _selectFunc = selectFunc;
            _sql.Append("SELECT");
        }

        public IPreparedTop<ITEM, RESULT> Distinct {
            get {
                _sql.Append(" DISTINCT");
                return this;
            }
        }

        public IPreparedHint<ITEM, RESULT> From(ITable table) {

            //todo: get select columns

            FieldCollector fieldCollector = new FieldCollector();

            _selectFunc(fieldCollector);

            _sql.Append(' ');
            bool first = true;

            foreach(IField field in fieldCollector.Fields) {

                if(!first) {
                    _sql.Append(',');
                }

                first = false;

                if(field is IColumn column) {
                    _sql.Append(column.Table.Alias).Append('.').Append(column.ColumnName);
                }
            }
            _sql.Append(" FROM ").Append(table.SchemaName).Append('.').Append(table.TableName).Append(" ").Append(table.Alias);
            return this;
        }

        public IPreparedQueryExecute<ITEM, RESULT> Build() {
            _sqlQuery = _sql.ToString();
            _sql.Clear();
            return this;
        }

        public IPreparedOption<ITEM, RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {
            throw new NotImplementedException();
        }

        public IPreparedHaving<ITEM, RESULT> GroupBy(params ISelectable[] columns) {
            throw new NotImplementedException();
        }

        public IPreparedOrderBy<ITEM, RESULT> Having(ICondition condition) {
            throw new NotImplementedException();
        }

        public IPreparedJoinOn<ITEM, RESULT> Join(ITable table) {
            throw new NotImplementedException();
        }

        public IPreparedJoinOn<ITEM, RESULT> LeftJoin(ITable table) {
            throw new NotImplementedException();
        }

        public ICompileQuery<ITEM, RESULT> Option(params SqlServerQueryOption[] options) {
            throw new NotImplementedException();
        }

        public ICompileQuery<ITEM, RESULT> Option(string labelName, params SqlServerQueryOption[] options) {
            throw new NotImplementedException();
        }

        public IPreparedFor<ITEM, RESULT> OrderBy(params IOrderByColumn[] columns) {
            throw new NotImplementedException();
        }

        public IPreparedFrom<ITEM, RESULT> Top(int rows) {
            _sql.Append(" TOP ").Append(rows);
            return this;
        }

        public IPreparedDistinct<ITEM, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {
            throw new NotImplementedException();
        }

        public IPreparedDistinct<ITEM, RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc) {
            throw new NotImplementedException();
        }

        public IPreparedGroupBy<ITEM, RESULT> Where(APreparedCondition<ITEM>? condition) {

            if(condition != null) {

                condition.CollectParameters(_parameters);

                for(int index = 0; index < _parameters.Count; index++) {

                    IParameter<ITEM> parameter = _parameters[index];

                    if(string.IsNullOrEmpty(parameter.Name)) {
                        parameter.Name = "@" + index;
                    }
                }
                _sql.Append(" WHERE ").Append(condition.GetSql());
            }
            return this;
        }

        public IPreparedJoin<ITEM, RESULT> With(params SqlServerTableHint[] hints) {
            throw new NotImplementedException();
        }

        public IPreparedJoin<ITEM, RESULT> On(ICondition on) {
            throw new NotImplementedException();
        }

        public List<RESULT> Execute(ITEM item, SqlConnection connection) {

            using SqlCommand command = connection.CreateCommand();

            command.Connection = connection;
            command.CommandText = _sqlQuery!;

            if(_parameters is not null) {

                foreach(IParameter<ITEM> parameter in _parameters) {

                    SqlParameter sqlParameter = new SqlParameter() {
                        ParameterName = parameter.Name
                    };
                    parameter.PopulateParameter(item, sqlParameter);
                    command.Parameters.Add(sqlParameter);
                }
            }
            using SqlDataReader reader = command.ExecuteReader();

            List<RESULT> list = new List<RESULT>();

            SqlServerResultRow sqlServerResultRow = new SqlServerResultRow(reader);

            while(reader.Read()) {
                list.Add(_selectFunc(sqlServerResultRow));
            }
            return list;
        }
    }

    public interface IPreparedQueryExecute<ITEM, RESULT> {

        List<RESULT> Execute(ITEM item, SqlConnection connection);
    }
}