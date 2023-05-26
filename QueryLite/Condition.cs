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

namespace QueryLite {

    public interface ICondition {

        void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters);

        //public static ICondition And(ICondition? conditionA, ICondition conditionB) {
        //    return conditionA != null ? new AndOrCondition(conditionA, isAnd: true, conditionB) : conditionB;
        //}
        //public static ICondition Or(ICondition? conditionA, ICondition conditionB) {
        //    return conditionA != null ? new AndOrCondition(conditionA, isAnd: false, conditionB) : conditionB;
        //}
        public static ICondition operator &(ICondition pConditionA, ICondition pConditionB) {
            return new AndOrCondition(pConditionA, isAnd: true, pConditionB);
        }
        public static ICondition operator |(ICondition pConditionA, ICondition pConditionB) {
            return new AndOrCondition(pConditionA, isAnd: false, pConditionB);
        }
    }


    public interface IColumnCondition : ICondition {

        IColumnCondition AND(IColumnCondition pConditionB) {
            return new AndOrConditionColumnCondition(this, isAnd: true, pConditionB);
        }
        IColumnCondition OR(IColumnCondition pConditionB) {
            return new AndOrConditionColumnCondition(this, isAnd: false, pConditionB);
        }
        public static IColumnCondition operator &(IColumnCondition pConditionA, IColumnCondition pConditionB) {
            return new AndOrConditionColumnCondition(pConditionA, isAnd: true, pConditionB);
        }
        public static IColumnCondition operator |(IColumnCondition pConditionA, IColumnCondition pConditionB) {
            return new AndOrConditionColumnCondition(pConditionA, isAnd: false, pConditionB);
        }
    }

    internal sealed class AndOrCondition : ICondition {

        public ICondition ConditionA { get; private set; }
        public bool IsAnd { get; private set; }
        public ICondition ConditionB { get; private set; }

        public AndOrCondition(ICondition conditionA, bool isAnd, ICondition conditionB) {
            ConditionA = conditionA;
            IsAnd = isAnd;
            ConditionB = conditionB;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {
            sql.Append('(');
            ConditionA.GetSql(sql, database, useAlias, parameters);
            sql.Append(IsAnd ? " AND " : " OR ");
            ConditionB.GetSql(sql, database, useAlias, parameters);
            sql.Append(')');
        }
    }

    /*
     * These additional condition classes are used to allow conditions between columns to be used in the prepared query functionality but disallow conditions that have fixed values.
     */
    internal sealed class AndOrConditionColumnCondition : IColumnCondition {

        public IColumnCondition ConditionA { get; private set; }
        public bool IsAnd { get; private set; }
        public IColumnCondition ConditionB { get; private set; }

        public AndOrConditionColumnCondition(IColumnCondition conditionA, bool isAnd, IColumnCondition conditionB) {
            ConditionA = conditionA;
            IsAnd = isAnd;
            ConditionB = conditionB;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {
            sql.Append('(');
            ConditionA.GetSql(sql, database, useAlias, parameters);
            sql.Append(IsAnd ? " AND " : " OR ");
            ConditionB.GetSql(sql, database, useAlias, parameters);
            sql.Append(')');
        }
    }

    internal sealed class InNotInCondition<TYPE> : ICondition where TYPE : notnull {

        private IColumn Left { get; }
        private bool IsIn { get; }
        private IEnumerable<TYPE> List { get; }

        public InNotInCondition(AColumn<TYPE> left, bool isIn, IEnumerable<TYPE> list) {

            bool hasAnItem = false;

            foreach(TYPE item in list) {
                hasAnItem = true;
                break;
            }
            if(!hasAnItem) {
                throw new ArgumentException($"{nameof(list)} must have at least one item");
            }
            Left = left;
            IsIn = isIn;
            List = list;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {

            if(useAlias) {
                sql.Append(Left.Table.Alias).Append('.');
            }
            sql.Append(Left.ColumnName).Append(IsIn ? " IN(" : " NOT IN(");

            bool isFirst = true;

            foreach(TYPE item in List) {

                if(!isFirst) {
                    sql.Append(',');
                }
                else {
                    isFirst = false;
                }
                if(parameters == null) {
                    sql.Append(database.ConvertToSql(item));
                }
                else {
                    parameters.Add(database, Left.Type, item, out string paramName);
                    sql.Append(paramName);
                }
            }
            sql.Append(')');
        }
    }

    internal sealed class InNotInNestedQueryCondition<TYPE, RESULT> : ICondition where TYPE : notnull {

        private IColumn Left { get; }
        private bool IsIn { get; }
        private IExecute<RESULT> NestedQuery { get; }

        public InNotInNestedQueryCondition(AColumn<TYPE> left, bool isIn, IExecute<RESULT> nestedQuery) {
            Left = left;
            IsIn = isIn;
            NestedQuery = nestedQuery;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {

            if(useAlias) {
                sql.Append(Left.Table.Alias).Append('.');
            }
            sql.Append(Left.ColumnName).Append(IsIn ? " IN(" : " NOT IN(");
            sql.Append(NestedQuery.GetSql(database, parameters));
            sql.Append(')');
        }
    }

    internal sealed class NullNotNullCondition<TYPE> : IColumnCondition where TYPE : notnull {

        private IColumn Left { get; }
        private bool IsNull { get; }

        public NullNotNullCondition(IColumn left, bool isNull) {
            Left = left;
            IsNull = isNull;
        }
        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {
            sql.Append((useAlias ? Left.Table.Alias + "." : "")).Append(Left.ColumnName).Append(IsNull ? " IS NULL" : " IS NOT NULL");
        }
    }

    internal sealed class NullNotNullFunctionCondition<TYPE> : ICondition where TYPE : notnull {

        private IFunction Left { get; }
        private bool IsNull { get; }

        public NullNotNullFunctionCondition(IFunction left, bool isNull) {
            Left = left;
            IsNull = isNull;
        }
        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {
            sql.Append(Left.GetSql(database, useAlias, parameters)).Append(IsNull ? " IS NULL" : " IS NOT NULL");
        }
    }

    internal sealed class GenericCondition : ICondition {

        public IField Left { get; private set; }
        public Operator Operator { get; private set; }
        public object Right { get; private set; }

        public GenericCondition(IField left, Operator oPerator, object right) {
            Left = left;
            Operator = oPerator;
            Right = right;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {

            if(Left is IColumn leftColumn) {

                if(useAlias) {
                    sql.Append(leftColumn.Table.Alias).Append('.');
                }
                sql.Append(leftColumn.ColumnName);
            }
            else if(Left is IFunction leftFunction) {
                sql.Append(leftFunction.GetSql(database, useAlias: useAlias, parameters));
            }
            else {
                throw new Exception($"Unsupported type: {Left.GetType()}");
            }

            sql.Append(Operator switch {
                Operator.EQUALS => " = ",
                Operator.NOT_EQUALS => " != ",
                Operator.GREATER_THAN => " > ",
                Operator.GREATER_THAN_OR_EQUAL => " >= ",
                Operator.LESS_THAN => " < ",
                Operator.LESS_THAN_OR_EQUAL => " <= ",
                Operator.LIKE => " LIKE ",
                Operator.NOT_LIKE => " NOT LIKE ",
                _ => throw new Exception($"Unknown join operator. {nameof(Operator)} == {Operator}")
            });

            if(Right is IColumn rightColumn) {

                if(useAlias) {
                    sql.Append(rightColumn.Table.Alias).Append('.');
                }
                sql.Append(rightColumn.ColumnName);
            }
            else if(Right is IFunction rightFunction) {
                sql.Append(rightFunction.GetSql(database, useAlias: useAlias, parameters));
            }
            else {

                object value = Right;

                if(parameters == null) {
                    sql.Append(database.ConvertToSql(value));
                }
                else {
                    parameters.Add(database, value.GetType(), value, out string paramName);
                    sql.Append(paramName);
                }
            }
        }
    }



    /// <summary>
    /// Column condition represents a condition that joins either columns or functions. It does not allow conditions with a fix value so that it can be used in prepared queries
    /// </summary>
    internal sealed class ColumnCondition : IColumnCondition {

        public IField Left { get; private set; }
        public Operator Operator { get; private set; }
        public IField Right { get; private set; }

        public ColumnCondition(IField left, Operator oPerator, IField right) {
            Left = left;
            Operator = oPerator;
            Right = right;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParameters? parameters) {

            if(Left is IColumn leftColumn) {

                if(useAlias) {
                    sql.Append(leftColumn.Table.Alias).Append('.');
                }
                sql.Append(leftColumn.ColumnName);
            }
            else if(Left is IFunction leftFunction) {
                sql.Append(leftFunction.GetSql(database, useAlias: useAlias, parameters));
            }
            else {
                throw new Exception($"Unsupported type: {Left.GetType()}");
            }

            sql.Append(Operator switch {
                Operator.EQUALS => " = ",
                Operator.NOT_EQUALS => " != ",
                Operator.GREATER_THAN => " > ",
                Operator.GREATER_THAN_OR_EQUAL => " >= ",
                Operator.LESS_THAN => " < ",
                Operator.LESS_THAN_OR_EQUAL => " <= ",
                Operator.LIKE => " LIKE ",
                Operator.NOT_LIKE => " NOT LIKE ",
                _ => throw new Exception($"Unknown join operator. {nameof(Operator)} == {Operator}")
            });

            if(Right is IColumn rightColumn) {

                if(useAlias) {
                    sql.Append(rightColumn.Table.Alias).Append('.');
                }
                sql.Append(rightColumn.ColumnName);
            }
            else if(Right is IFunction rightFunction) {
                sql.Append(rightFunction.GetSql(database, useAlias: useAlias, parameters));
            }
            else {
                throw new Exception($"Unsupported type '{Right.GetType().FullName}'");
            }
        }
    }

    public enum Operator {
        EQUALS,
        NOT_EQUALS,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        LIKE,
        NOT_LIKE
    }
}