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
using QueryLite.Databases;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryLite.PreparedQuery {

    public abstract class APreparedCondition<PARAMETERS> {

        public APreparedCondition<PARAMETERS> EQUALS<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedCondition<PARAMETERS>(columnA, Operator.EQUALS, columnB);
        }
        public APreparedCondition<PARAMETERS> EQUALS<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueCondition<PARAMETERS, TYPE>(column, Operator.EQUALS, func);
        }

        public APreparedCondition<PARAMETERS> NOT_EQUALS<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedCondition<PARAMETERS>(columnA, Operator.NOT_EQUALS, columnB);
        }
        public APreparedCondition<PARAMETERS> NOT_EQUALS<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueCondition<PARAMETERS, TYPE>(column, Operator.NOT_EQUALS, func);
        }

        public APreparedCondition<PARAMETERS> LESS_THAN<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedCondition<PARAMETERS>(columnA, Operator.LESS_THAN, columnB);
        }
        public APreparedCondition<PARAMETERS> LESS_THAN<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueCondition<PARAMETERS, TYPE>(column, Operator.LESS_THAN, func);
        }

        public APreparedCondition<PARAMETERS> LESS_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedCondition<PARAMETERS>(columnA, Operator.LESS_THAN_OR_EQUAL, columnB);
        }
        public APreparedCondition<PARAMETERS> LESS_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueCondition<PARAMETERS, TYPE>(column, Operator.LESS_THAN_OR_EQUAL, func);
        }

        public APreparedCondition<PARAMETERS> GREATER_THAN<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedCondition<PARAMETERS>(columnA, Operator.GREATER_THAN, columnB);
        }
        public APreparedCondition<PARAMETERS> GREATER_THAN<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueCondition<PARAMETERS, TYPE>(column, Operator.GREATER_THAN, func);
        }

        public APreparedCondition<PARAMETERS> GREATER_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedCondition<PARAMETERS>(columnA, Operator.GREATER_THAN_OR_EQUAL, columnB);
        }

        public APreparedCondition<PARAMETERS> GREATER_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueCondition<PARAMETERS, TYPE>(column, Operator.GREATER_THAN_OR_EQUAL, func);
        }

        public APreparedCondition<PARAMETERS> IS_NULL<TYPE>(AColumn<TYPE> column) where TYPE : notnull {
            return new PreparedNullCondition<PARAMETERS>(column, isNull: true);
        }
        public APreparedCondition<PARAMETERS> IS_NOT_NULL<TYPE>(AColumn<TYPE> column) where TYPE : notnull {
            return new PreparedNullCondition<PARAMETERS>(column, isNull: false);
        }

        public static APreparedCondition<PARAMETERS> operator &(APreparedCondition<PARAMETERS> conditionA, APreparedCondition<PARAMETERS> conditionB) {
            return new PreparedAndOrCondition<PARAMETERS>(conditionA, isAnd: true, conditionB);
        }
        public static APreparedCondition<PARAMETERS> operator |(APreparedCondition<PARAMETERS> conditionA, APreparedCondition<PARAMETERS> conditionB) {
            return new PreparedAndOrCondition<PARAMETERS>(conditionA, isAnd: false, conditionB);
        }

        internal abstract void GetSql(StringBuilder sql, IDatabase database, PreparedParameterList<PARAMETERS> parameters, bool useAlias);
    }

    internal sealed class EmptyPreparedCondition<PARAMETERS> : APreparedCondition<PARAMETERS> {

        internal override void GetSql(StringBuilder sql, IDatabase database, PreparedParameterList<PARAMETERS> parameters, bool useAlias) {
            throw new InvalidOperationException("An empty condition cannot be used in a query");
        }
    }

    internal sealed class PreparedAndOrCondition<PARAMETERS> : APreparedCondition<PARAMETERS> {

        private readonly APreparedCondition<PARAMETERS> _conditionA;
        private readonly bool _isAnd;
        private readonly APreparedCondition<PARAMETERS> _conditionB;

        public PreparedAndOrCondition(APreparedCondition<PARAMETERS> conditionA, bool isAnd, APreparedCondition<PARAMETERS> conditionB) {
            _conditionA = conditionA;
            _isAnd = isAnd;
            _conditionB = conditionB;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, PreparedParameterList<PARAMETERS> parameters, bool useAlias) {

            _conditionA.GetSql(sql, database, parameters, useAlias: useAlias);

            sql.Append(_isAnd ? " AND " : " OR ");

            _conditionB.GetSql(sql, database, parameters, useAlias: useAlias);
        }
    }

    internal sealed class PreparedCondition<PARAMETERS> : APreparedCondition<PARAMETERS> {

        private readonly IColumn _columnA;
        private readonly Operator _operator;
        private readonly IColumn _columnB;

        public PreparedCondition(IColumn columnA, Operator @operator, IColumn columnB) {
            _columnA = columnA;
            _operator = @operator;
            _columnB = columnB;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, PreparedParameterList<PARAMETERS> parameters, bool useAlias) {

            if(useAlias) {
                sql.Append(_columnA.Table.Alias).Append('.');
            }
            sql.Append(_columnA.ColumnName);

            sql.Append(_operator switch {
                Operator.EQUALS => " = ",
                Operator.NOT_EQUALS => " != ",
                Operator.GREATER_THAN => " > ",
                Operator.GREATER_THAN_OR_EQUAL => " >= ",
                Operator.LESS_THAN => " < ",
                Operator.LESS_THAN_OR_EQUAL => " <= ",
                //Operator.LIKE => " LIKE ",
                //Operator.NOT_LIKE => " NOT LIKE ",
                _ => throw new Exception($"Unsupported join operator. {nameof(Operator)} == {_operator}")
            });

            if(useAlias) {
                sql.Append(_columnB.Table.Alias).Append('.');
            }
            sql.Append(_columnB.ColumnName);
        }
    }

    internal sealed class PreparedValueCondition<PARAMETERS, TYPE> : APreparedCondition<PARAMETERS> {

        private readonly IColumn _column;
        private readonly Operator _operator;
        private readonly Func<PARAMETERS, TYPE> _func;

        public PreparedValueCondition(IColumn column, Operator @operator, Func<PARAMETERS, TYPE> func) {
            _column = column;
            _operator = @operator;
            _func = func;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, PreparedParameterList<PARAMETERS> parameters, bool useAlias) {

            if(useAlias) {
                SqlHelper.AppendEncloseAlias(sql, _column.Table.Alias);
                sql.Append('.');
            }
            SqlHelper.AppendEncloseColumnName(sql, _column);

            sql.Append(_operator switch {
                Operator.EQUALS => " = ",
                Operator.NOT_EQUALS => " != ",
                Operator.GREATER_THAN => " > ",
                Operator.GREATER_THAN_OR_EQUAL => " >= ",
                Operator.LESS_THAN => " < ",
                Operator.LESS_THAN_OR_EQUAL => " <= ",
                //Operator.LIKE => " LIKE ",
                //Operator.NOT_LIKE => " NOT LIKE ",
                _ => throw new Exception($"Unsupported join operator. {nameof(Operator)} == {_operator}")
            });

            string paramName = parameters.GetNextParameterName();
            
            parameters.Add(new PreparedParameter<PARAMETERS, TYPE>(name: paramName, _func, database.ParameterMapper.GetCreateParameterDelegate(_column.Type)));

            sql.Append(paramName);
        }
    }

    internal sealed class PreparedNullCondition<PARAMETERS> : APreparedCondition<PARAMETERS> {

        private readonly IColumn _column;
        private readonly bool _isNull;

        public PreparedNullCondition(IColumn column, bool isNull) {
            _column = column;
            _isNull = isNull;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, PreparedParameterList<PARAMETERS> parameters, bool useAlias) {

            if(useAlias) {
                sql.Append(_column.Table.Alias).Append('.');
            }
            sql.Append(_column.ColumnName);

            sql.Append(_isNull ? " IS NULL" : " IS NOT NULL");
        }
    }
}