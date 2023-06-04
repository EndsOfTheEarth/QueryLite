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
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryLite.PreparedQuery {

    public abstract class APreparedConditionNew<PARAMETERS> {

        public APreparedConditionNew<PARAMETERS> EQUALS<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedConditionNew<PARAMETERS>(columnA, "=", columnB);
        }
        public APreparedConditionNew<PARAMETERS> EQUALS<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueConditionNew<PARAMETERS, TYPE>(column, "=", func);
        }

        public APreparedConditionNew<PARAMETERS> NOT_EQUALS<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedConditionNew<PARAMETERS>(columnA, "!=", columnB);
        }
        public APreparedConditionNew<PARAMETERS> NOT_EQUALS<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueConditionNew<PARAMETERS, TYPE>(column, "!=", func);
        }

        public APreparedConditionNew<PARAMETERS> LESS_THAN<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedConditionNew<PARAMETERS>(columnA, "<", columnB);
        }
        public APreparedConditionNew<PARAMETERS> LESS_THAN<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueConditionNew<PARAMETERS, TYPE>(column, "<", func);
        }

        public APreparedConditionNew<PARAMETERS> LESS_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedConditionNew<PARAMETERS>(columnA, "<=", columnB);
        }
        public APreparedConditionNew<PARAMETERS> LESS_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueConditionNew<PARAMETERS, TYPE>(column, "<=", func);
        }

        public APreparedConditionNew<PARAMETERS> GREATER_THAN<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedConditionNew<PARAMETERS>(columnA, ">", columnB);
        }
        public APreparedConditionNew<PARAMETERS> GREATER_THAN<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueConditionNew<PARAMETERS, TYPE>(column, ">", func);
        }

        public APreparedConditionNew<PARAMETERS> GREATER_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> columnA, AColumn<TYPE> columnB) where TYPE : notnull {
            return new PreparedConditionNew<PARAMETERS>(columnA, "<=", columnB);
        }

        public APreparedConditionNew<PARAMETERS> GREATER_THAN_OR_EQUAL<TYPE>(AColumn<TYPE> column, Func<PARAMETERS, TYPE> func) where TYPE : notnull {
            return new PreparedValueConditionNew<PARAMETERS, TYPE>(column, ">=", func);
        }

        public APreparedConditionNew<PARAMETERS> IS_NULL<TYPE>(AColumn<TYPE> column) where TYPE : notnull {
            return new PreparedNullConditionNew<PARAMETERS>(column, isNull: true);
        }
        public APreparedConditionNew<PARAMETERS> IS_NOT_NULL<TYPE>(AColumn<TYPE> column) where TYPE : notnull {
            return new PreparedNullConditionNew<PARAMETERS>(column, isNull: false);
        }

        public static APreparedConditionNew<PARAMETERS> operator &(APreparedConditionNew<PARAMETERS> conditionA, APreparedConditionNew<PARAMETERS> conditionB) {
            return new PreparedAndOrConditionNew<PARAMETERS>(conditionA, isAnd: true, conditionB);
        }
        public static APreparedConditionNew<PARAMETERS> operator |(APreparedConditionNew<PARAMETERS> conditionA, APreparedConditionNew<PARAMETERS> conditionB) {
            return new PreparedAndOrConditionNew<PARAMETERS>(conditionA, isAnd: false, conditionB);
        }

        internal abstract void GetSql(StringBuilder sql, IDatabase database, IParameterCollector<PARAMETERS> paramCollector, bool useAlias);
    }

    internal sealed class EmptyPreparedConditionNew<PARAMETERS> : APreparedConditionNew<PARAMETERS> {

        internal override void GetSql(StringBuilder sql, IDatabase database, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {
            throw new InvalidOperationException("An empty condition cannot be used in a query");
        }
    }

    internal sealed class PreparedAndOrConditionNew<PARAMETERS> : APreparedConditionNew<PARAMETERS> {

        private readonly APreparedConditionNew<PARAMETERS> _conditionA;
        private readonly bool _isAnd;
        private readonly APreparedConditionNew<PARAMETERS> _conditionB;

        public PreparedAndOrConditionNew(APreparedConditionNew<PARAMETERS> conditionA, bool isAnd, APreparedConditionNew<PARAMETERS> conditionB) {
            _conditionA = conditionA;
            _isAnd = isAnd;
            _conditionB = conditionB;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            _conditionA.GetSql(sql, database, paramCollector, useAlias: useAlias);

            sql.Append(_isAnd ? " AND " : " OR ");

            _conditionB.GetSql(sql, database, paramCollector, useAlias: useAlias);
        }
    }

    internal sealed class PreparedConditionNew<PARAMETERS> : APreparedConditionNew<PARAMETERS> {

        private readonly IColumn _columnA;
        private readonly string _operator;
        private readonly IColumn _columnB;

        public PreparedConditionNew(IColumn columnA, string @operator, IColumn columnB) {
            _columnA = columnA;
            _operator = @operator;
            _columnB = columnB;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            if(useAlias) {
                sql.Append(_columnA.Table.Alias).Append('.');
            }
            sql.Append(_columnA.ColumnName);

            sql.Append(_operator);

            if(useAlias) {
                sql.Append(_columnB.Table.Alias).Append('.');
            }
            sql.Append(_columnB.ColumnName);
        }
    }

    internal sealed class PreparedValueConditionNew<PARAMETERS, TYPE> : APreparedConditionNew<PARAMETERS> {

        private readonly IColumn _column;
        private readonly string _operator;
        private readonly Func<PARAMETERS, TYPE> _func;

        public PreparedValueConditionNew(IColumn column, string @operator, Func<PARAMETERS, TYPE> func) {
            _column = column;
            _operator = @operator;
            _func = func;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            if(useAlias) {
                sql.Append(_column.Table.Alias).Append('.');
            }
            sql.Append(_column.ColumnName);

            sql.Append(_operator);

            string paramName = paramCollector.Add(new Parameter<PARAMETERS, TYPE>(_func));

            sql.Append(paramName);
        }
    }

    internal sealed class PreparedNullConditionNew<PARAMETERS> : APreparedConditionNew<PARAMETERS> {

        private readonly IColumn _column;
        private readonly bool _isNull;

        public PreparedNullConditionNew(IColumn column, bool isNull) {
            _column = column;
            _isNull = isNull;
        }

        internal override void GetSql(StringBuilder sql, IDatabase database, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            if(useAlias) {
                sql.Append(_column.Table.Alias).Append('.');
            }
            sql.Append(_column.ColumnName);

            sql.Append(_isNull ? " IS NULL" : " IS NOT NULL");
        }
    }

    public interface IParameterCollector<PARAMETERS> {

        public string Add(IParameter<PARAMETERS> parameter);
    }

    public class ParameterCollector<PARAMETERS> : IParameterCollector<PARAMETERS> {

        public List<IParameter<PARAMETERS>> Parameters { get; } = new List<IParameter<PARAMETERS>>();

        public string Add(IParameter<PARAMETERS> parameter) {

            parameter.Name = $"@{Parameters.Count}";
            Parameters.Add(parameter);

            return parameter.Name;
        }
    }
}