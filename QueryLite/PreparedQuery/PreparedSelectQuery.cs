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
        public ICompiledDistinct<ITEM, RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc) {
            return new SqlServerQueryBuilder<ITEM, RESULT>(selectFunc);
        }
    }



    //public interface ICompileSelect<ITEM> {

    //    ICompiledDistinct<ITEM, RESULT> Select<RESULT>(Func<IResultRow, RESULT> selectFunc);
    //}

    public interface ICompiledDistinct<ITEM, RESULT> : ICompiledTop<ITEM, RESULT> {

        ICompiledTop<ITEM, RESULT> Distinct { get; }
    }

    public interface ICompiledTop<ITEM, RESULT> : ICompiledFrom<ITEM, RESULT> {

        /// <summary>
        /// Return TOP n rows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        ICompiledFrom<ITEM, RESULT> Top(int rows);
    }
    public interface ICompiledFrom<ITEM, RESULT> {

        /// <summary>
        /// From table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        ICompiledHint<ITEM, RESULT> From(ITable table);
    }

    public interface ICompiledHint<ITEM, RESULT> : ICompiledJoin<ITEM, RESULT> {

        /// <summary>
        /// The 'With' option only works on sql server. For other databases the query will ignore these table hints and execute without them.
        /// </summary>
        /// <param name="hints"></param>
        /// <returns></returns>
        public ICompiledJoin<ITEM, RESULT> With(params SqlServerTableHint[] hints);
    }


    public interface ICompiledJoin<ITEM, RESULT> : ICompiledWhere<ITEM, RESULT> {

        /// <summary>
        /// Join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        ICompiledJoinOn<ITEM, RESULT> Join(ITable table);

        /// <summary>
        /// Left join table clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        ICompiledJoinOn<ITEM, RESULT> LeftJoin(ITable table);
    }

    public interface ICompiledJoinOn<ITEM, RESULT> {

        /// <summary>
        /// Join condition
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        ICompiledJoin<ITEM, RESULT> On(ICondition on);
    }

    public interface ICompiledWhere<ITEM, RESULT> : ICompiledGroupBy<ITEM, RESULT> {

        /// <summary>
        /// Where condition clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ICompiledGroupBy<ITEM, RESULT> Where(ACompiledCondition<ITEM>? condition);
    }

    public interface ICompiledGroupBy<ITEM, RESULT> : ICompiledHaving<ITEM, RESULT> {

        /// <summary>
        /// Group by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        ICompiledHaving<ITEM, RESULT> GroupBy(params ISelectable[] columns);
    }

    public interface ICompiledHaving<ITEM, RESULT> : ICompiledOrderBy<ITEM, RESULT> {

        /// <summary>
        /// Having clause
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ICompiledOrderBy<ITEM, RESULT> Having(ICondition condition);
    }

    public interface ICompiledOrderBy<ITEM, RESULT> : ICompiledFor<ITEM, RESULT> {

        /// <summary>
        /// Order by clause
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        ICompiledFor<ITEM, RESULT> OrderBy(params IOrderByColumn[] columns);

        /// <summary>
        /// Union query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        ICompiledDistinct<ITEM, RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc);

        /// <summary>
        /// Union all query
        /// </summary>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        ICompiledDistinct<ITEM, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc);
    }

    public interface ICompiledFor<ITEM, RESULT> : ICompiledOption<ITEM, RESULT> {

        /// <summary>
        /// FOR caluse. PostgreSql only
        /// </summary>
        /// <param name="forType"></param>
        /// <param name="ofTables"></param>
        /// <param name="waitType"></param>
        /// <returns></returns>
        ICompiledOption<ITEM, RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType);
    }

    public interface ICompiledOption<ITEM, RESULT> : ICompileQuery<ITEM, RESULT> {

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

        ICompiledQueryExecute<ITEM, RESULT> Build();
    }

    

    

    public class SqlServerQueryBuilder<ITEM, RESULT> : ICompiledDistinct<ITEM, RESULT>, ICompiledTop<ITEM, RESULT>, ICompiledFrom<ITEM, RESULT>, ICompiledHint<ITEM, RESULT>,
                                                        ICompiledJoin<ITEM, RESULT>, ICompiledJoinOn<ITEM, RESULT>, ICompiledWhere<ITEM, RESULT>, ICompiledGroupBy<ITEM, RESULT>,
                                                        ICompiledHaving<ITEM, RESULT>, ICompiledOrderBy<ITEM, RESULT>, ICompiledFor<ITEM, RESULT>, ICompiledOption<ITEM, RESULT>,
                                                        ICompileQuery<ITEM, RESULT>, ICompiledQueryExecute<ITEM, RESULT> {

        private Func<IResultRow, RESULT> _selectFunc;
        private StringBuilder _sql = new StringBuilder();

        private string? _sqlQuery;

        private List<IParameter<ITEM>> _parameters = new List<IParameter<ITEM>>();


        public SqlServerQueryBuilder(Func<IResultRow, RESULT> selectFunc) {
            _selectFunc = selectFunc;
            _sql.Append("SELECT");
        }

        public ICompiledTop<ITEM, RESULT> Distinct {
            get {
                _sql.Append(" DISTINCT");
                return this;
            }
        }

        public ICompiledHint<ITEM, RESULT> From(ITable table) {

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

        public ICompiledQueryExecute<ITEM, RESULT> Build() {
            _sqlQuery = _sql.ToString();
            _sql.Clear();
            return this;
        }

        public ICompiledOption<ITEM, RESULT> FOR(ForType forType, ITable[] ofTables, WaitType waitType) {
            throw new NotImplementedException();
        }

        public ICompiledHaving<ITEM, RESULT> GroupBy(params ISelectable[] columns) {
            throw new NotImplementedException();
        }

        public ICompiledOrderBy<ITEM, RESULT> Having(ICondition condition) {
            throw new NotImplementedException();
        }

        public ICompiledJoinOn<ITEM, RESULT> Join(ITable table) {
            throw new NotImplementedException();
        }

        public ICompiledJoinOn<ITEM, RESULT> LeftJoin(ITable table) {
            throw new NotImplementedException();
        }

        public ICompileQuery<ITEM, RESULT> Option(params SqlServerQueryOption[] options) {
            throw new NotImplementedException();
        }

        public ICompileQuery<ITEM, RESULT> Option(string labelName, params SqlServerQueryOption[] options) {
            throw new NotImplementedException();
        }

        public ICompiledFor<ITEM, RESULT> OrderBy(params IOrderByColumn[] columns) {
            throw new NotImplementedException();
        }

        public ICompiledFrom<ITEM, RESULT> Top(int rows) {
            _sql.Append(" TOP ").Append(rows);
            return this;
        }

        public ICompiledDistinct<ITEM, RESULT> UnionAllSelect(Func<IResultRow, RESULT> selectFunc) {
            throw new NotImplementedException();
        }

        public ICompiledDistinct<ITEM, RESULT> UnionSelect(Func<IResultRow, RESULT> selectFunc) {
            throw new NotImplementedException();
        }

        public ICompiledGroupBy<ITEM, RESULT> Where(ACompiledCondition<ITEM>? condition) {

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

        public ICompiledJoin<ITEM, RESULT> With(params SqlServerTableHint[] hints) {
            throw new NotImplementedException();
        }

        public ICompiledJoin<ITEM, RESULT> On(ICondition on) {
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

    public interface ICompiledQueryExecute<ITEM, RESULT> {

        List<RESULT> Execute(ITEM item, SqlConnection connection);
    }
}