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

namespace QueryLite.PreparedQuery {

    public static class ColumnExtension {

        public static APreparedCondition<ITEM> EQUALS<TYPE, ITEM>(this Column<TYPE> column, IParameter<TYPE, ITEM> parameter) where TYPE : notnull {
            return new PreparedParameterCondition<ITEM>(column, "=", parameter);
        }
        public static APreparedCondition<ITEM> NOT_EQUALS<TYPE, ITEM>(this Column<TYPE> column, IParameter<TYPE, ITEM> parameter) where TYPE : notnull {
            return new PreparedParameterCondition<ITEM>(column, "!=", parameter);
        }

        public static APreparedCondition<TYPE> IS_NULL<TYPE>(this Column<TYPE> column) where TYPE : notnull {
            return new PreparedIsNullCondition<TYPE>(column, @operator: "IS NULL");
        }
        public static APreparedCondition<TYPE> IS_NOT_NULL<TYPE>(this Column<TYPE> column) where TYPE : notnull {
            return new PreparedIsNullCondition<TYPE>(column, @operator: "IS NOT NULL");
        }
    }

    public abstract class APreparedCondition<ITEM> {

        public abstract string GetSql();
        public abstract void CollectParameters(List<IParameter<ITEM>> parameters);

        public APreparedCondition<ITEM> AND(APreparedCondition<ITEM> condition) {
            return new PreparedAndOrCondition<ITEM>(this, @operator: "AND", condition);
        }
        public APreparedCondition<ITEM> OR(APreparedCondition<ITEM> condition) {
            return new PreparedAndOrCondition<ITEM>(this, @operator: "AND", condition);
        }
    }

    public class PreparedParameterCondition<ITEM> : APreparedCondition<ITEM> {

        public PreparedParameterCondition(IColumn column, string @operator, IParameter<ITEM>? parameter) {
            Column = column;
            Operator = @operator;
            Parameter = parameter;
        }
        public IColumn Column { get; }

        public string Operator { get; }
        public IParameter<ITEM>? Parameter { get; }

        public override void CollectParameters(List<IParameter<ITEM>> parameters) {

            if(Parameter != null) {
                parameters.Add(Parameter);
            }
        }

        public override string GetSql() {

            if(Parameter != null) {
                return $"{Column.Table.Alias}.{Column.ColumnName} {Operator} {Parameter.Name!}";
            }
            else {
                return $"{Column.Table.Alias}.{Column.ColumnName} {Operator}";
            }
        }
    }

    public class PreparedAndOrCondition<ITEM> : APreparedCondition<ITEM> {

        public PreparedAndOrCondition(APreparedCondition<ITEM> conditionA, string @operator, APreparedCondition<ITEM> conditionB) {
            ConditionA = conditionA;
            Operator = @operator;
            ConditionB = conditionB;
        }

        public APreparedCondition<ITEM> ConditionA { get; }
        public string Operator { get; }
        public APreparedCondition<ITEM> ConditionB { get; }

        public override void CollectParameters(List<IParameter<ITEM>> parameters) {

            ConditionA.CollectParameters(parameters);
            ConditionB.CollectParameters(parameters);
        }
        public override string GetSql() {
            return $"({ConditionA.GetSql()} {Operator} {ConditionB.GetSql()})";
        }
    }

    public class PreparedIsNullCondition<ITEM> : APreparedCondition<ITEM> {

        public PreparedIsNullCondition(IColumn column, string @operator) {
            Column = column;
            Operator = @operator;
        }
        public IColumn Column { get; }
        public string Operator { get; }

        public override void CollectParameters(List<IParameter<ITEM>> parameters) {

        }
        public override string GetSql() {
            return $"{Column.Table.Alias}.{Column.ColumnName}{Operator}";
        }
    }
}