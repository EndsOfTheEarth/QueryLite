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
using System.Collections.Generic;
using System.Text;

namespace QueryLite.PreparedQuery {

    public static class ColumnExtension {

        public static IPreparedCondition<PARAMETERS> EQUALS<TYPE, PARAMETERS>(this Column<TYPE> column, IParameter<TYPE, PARAMETERS> parameter) where TYPE : notnull {
            return new PreparedParameterCondition<PARAMETERS>(column, "=", parameter);
        }
        public static IPreparedCondition<PARAMETERS> NOT_EQUALS<TYPE, PARAMETERS>(this Column<TYPE> column, IParameter<TYPE, PARAMETERS> parameter) where TYPE : notnull {
            return new PreparedParameterCondition<PARAMETERS>(column, "!=", parameter);
        }

        public static IPreparedCondition<PARAMETERS> IS_NULL<PARAMETERS, TYPE>(this Column<TYPE> column) where TYPE : notnull {
            return new PreparedIsNullCondition<PARAMETERS>(column, @operator: "IS NULL");
        }
        public static IPreparedCondition<PARAMETERS> IS_NOT_NULL<PARAMETERS, TYPE>(this Column<TYPE> column) where TYPE : notnull {
            return new PreparedIsNullCondition<PARAMETERS>(column, @operator: "IS NOT NULL");
        }
    }

    public interface IParameterCollector<PARAMETERS> {

        public void Add(IParameter<PARAMETERS> parameter);
    }

    public class ParameterCollector<PARAMETERS> : IParameterCollector<PARAMETERS> {

        public List<IParameter<PARAMETERS>> Parameters { get; } = new List<IParameter<PARAMETERS>>();

        public void Add(IParameter<PARAMETERS> parameter) {
            parameter.Name = $"@{Parameters.Count}";
            Parameters.Add(parameter);
        }
    }


    public interface IPreparedCondition<PARAMETERS> {

        public void GetSql(StringBuilder sql, IParameterCollector<PARAMETERS> paramCollector, bool useAlias);

        public IPreparedCondition<PARAMETERS> AND(IPreparedCondition<PARAMETERS> condition) {
            return new PreparedAndOrCondition<PARAMETERS>(this, @operator: "AND", condition);
        }
        public IPreparedCondition<PARAMETERS> OR(IPreparedCondition<PARAMETERS> condition) {
            return new PreparedAndOrCondition<PARAMETERS>(this, @operator: "AND", condition);
        }
    }

    public class PreparedParameterCondition<PARAMETERS> : IPreparedCondition<PARAMETERS> {

        public PreparedParameterCondition(IColumn column, string @operator, IParameter<PARAMETERS>? parameter) {
            Column = column;
            Operator = @operator;
            Parameter = parameter;
        }
        public IColumn Column { get; }
        public string Operator { get; }
        public IParameter<PARAMETERS>? Parameter { get; }

        public void GetSql(StringBuilder sql, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            if(Parameter != null) {
                paramCollector.Add(Parameter);
            }

            if(useAlias) {
                sql.Append(Column.Table.Alias).Append('.');
            }
            sql.Append(Column.ColumnName).Append(' ').Append(Operator);

            if(Parameter != null) {
                sql.Append(' ').Append(Parameter.Name!);
            }
        }
    }

    public class PreparedAndOrCondition<PARAMETERS> : IPreparedCondition<PARAMETERS> {

        public PreparedAndOrCondition(IPreparedCondition<PARAMETERS> conditionA, string @operator, IPreparedCondition<PARAMETERS> conditionB) {
            ConditionA = conditionA;
            Operator = @operator;
            ConditionB = conditionB;
        }

        public IPreparedCondition<PARAMETERS> ConditionA { get; }
        public string Operator { get; }
        public IPreparedCondition<PARAMETERS> ConditionB { get; }

        public void GetSql(StringBuilder sql, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            sql.Append('(');
            ConditionA.GetSql(sql, paramCollector, useAlias: useAlias);
            sql.Append(' ').Append(Operator).Append(' ');
            ConditionB.GetSql(sql, paramCollector, useAlias: useAlias);
            sql.Append(')');
        }
    }

    public class PreparedIsNullCondition<PARAMETERS> : IPreparedCondition<PARAMETERS> {

        public PreparedIsNullCondition(IColumn column, string @operator) {
            Column = column;
            Operator = @operator;
        }
        public IColumn Column { get; }
        public string Operator { get; }

        public void GetSql(StringBuilder sql, IParameterCollector<PARAMETERS> paramCollector, bool useAlias) {

            if(useAlias) {
                sql.Append(Column.Table.Alias).Append('.');
            }
            sql.Append(Column.ColumnName).Append(' ').Append(Operator);
        }
    }
}