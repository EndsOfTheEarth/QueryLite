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

namespace QueryLite {

    public interface ICondition {

        void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters);

        public static ICondition And(ICondition? conditionA, ICondition conditionB) {
            return conditionA != null ? new AndOrCondition(conditionA, isAnd: true, conditionB) : conditionB;
        }
        public static ICondition Or(ICondition? conditionA, ICondition conditionB) {
            return conditionA != null ? new AndOrCondition(conditionA, isAnd: false, conditionB) : conditionB;
        }
        public static ICondition operator &(ICondition? conditionA, ICondition conditionB) {

            if(conditionA == null) {
                return conditionB;
            }
            return new AndOrCondition(conditionA, isAnd: true, conditionB);
        }
        public static ICondition operator |(ICondition? conditionA, ICondition conditionB) {

            if(conditionA == null) {
                return conditionB;
            }
            return new AndOrCondition(conditionA, isAnd: false, conditionB);
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

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {
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
    internal sealed class AndOrConditionColumnCondition : ICondition {

        public ICondition ConditionA { get; private set; }
        public bool IsAnd { get; private set; }
        public ICondition ConditionB { get; private set; }

        public AndOrConditionColumnCondition(ICondition conditionA, bool isAnd, ICondition conditionB) {
            ConditionA = conditionA;
            IsAnd = isAnd;
            ConditionB = conditionB;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            sql.Append('(');
            ConditionA.GetSql(sql, database, useAlias, parameters);
            sql.Append(IsAnd ? " AND " : " OR ");
            ConditionB.GetSql(sql, database, useAlias, parameters);
            sql.Append(')');
        }
    }

    internal sealed class InNotInCondition<TYPE> : ICondition where TYPE : notnull {

        private AColumn<TYPE> Left { get; }
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

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

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
                    parameters.AddParameter(database, Left.Type, value: item, out string paramName);
                    sql.Append(paramName);
                }
            }
            sql.Append(')');
        }
    }

    internal sealed class InNotInNestedQueryCondition<TYPE, RESULT> : ICondition where TYPE : notnull {

        private AColumn<TYPE> Left { get; }
        private bool IsIn { get; }
        private IExecute<RESULT> NestedQuery { get; }

        public InNotInNestedQueryCondition(AColumn<TYPE> left, bool isIn, IExecute<RESULT> nestedQuery) {
            Left = left;
            IsIn = isIn;
            NestedQuery = nestedQuery;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(useAlias) {
                sql.Append(Left.Table.Alias).Append('.');
            }
            sql.Append(Left.ColumnName).Append(IsIn ? " IN(" : " NOT IN(");
            sql.Append(NestedQuery.GetSql(database, parameters));
            sql.Append(')');
        }
    }

    internal sealed class NullNotNullCondition<TYPE> : ICondition where TYPE : notnull {

        private IColumn Left { get; }
        private bool IsNull { get; }

        public NullNotNullCondition(IColumn left, bool isNull) {
            Left = left;
            IsNull = isNull;
        }
        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {
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
        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            sql.Append(Left.GetSql(database, useAlias, parameters)).Append(IsNull ? " IS NULL" : " IS NOT NULL");
        }
    }

    internal sealed class GenericCondition : ICondition {

        public IField Left { get; private set; }
        public Operator Operator { get; private set; }
        public object Right { get; private set; }

        public GenericCondition(IField left, Operator @operator, object right) {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            ConditionHelper.AppendSqlValue(Left, sql, database, useAlias: useAlias, parameters);

            sql.Append(Operator switch {
                Operator.EQUALS => " = ",
                Operator.NOT_EQUALS => " != ",
                Operator.GREATER_THAN => " > ",
                Operator.GREATER_THAN_OR_EQUAL => " >= ",
                Operator.LESS_THAN => " < ",
                Operator.LESS_THAN_OR_EQUAL => " <= ",
                _ => throw new Exception($"Unknown join operator. {nameof(Operator)} == {Operator}")
            });
            ConditionHelper.AppendSqlValue(Right, sql, database, useAlias: useAlias, parameters);
        }
    }

    internal sealed class LikeCondition<TYPE> : ICondition {

        public IField Left { get; private set; }
        public ILike<TYPE> Like { get; private set; }

        public LikeCondition(IField left, ILike<TYPE> like) {
            Left = left;
            Like = like;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

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
            sql.Append(database.LikeSqlConditionGenerator.GetSql(Like, database));
        }
    }

    internal sealed class BetweenCondition<TYPE> : ICondition where TYPE : notnull {
        
        public bool Not { get; }
        public IField Left { get; private set; }
        public object ValueA { get; private set; }
        public object ValueB { get; private set; }

        public BetweenCondition(bool not, IField left, TYPE valueA, TYPE valueB) {
            Not = not;
            Left = left;
            ValueA = valueA;
            ValueB = valueB;
        }

        public BetweenCondition(bool not, IField left, ISelectable<TYPE> valueA, TYPE valueB) {
            Not = not;
            Left = left;
            ValueA = valueA;
            ValueB = valueB;
        }

        public BetweenCondition(bool not, IField left, ISelectable<TYPE> valueA, ISelectable<TYPE> valueB) {
            Not = not;
            Left = left;
            ValueA = valueA;
            ValueB = valueB;
        }

        public BetweenCondition(bool not, IField left, TYPE valueA, ISelectable<TYPE> valueB) {
            Not = not;
            Left = left;
            ValueA = valueA;
            ValueB = valueB;
        }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

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
            if(Not) {
                sql.Append(" NOT BETWEEN ");
            }
            else {
                sql.Append(" BETWEEN ");
            }
            ConditionHelper.AppendSqlValue(ValueA, sql, database, useAlias: useAlias, parameters);
            sql.Append(" AND ");
            ConditionHelper.AppendSqlValue(ValueB, sql, database, useAlias: useAlias, parameters);
        }
    }

    internal static class ConditionHelper {

        public static void AppendSqlValue(object value, StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(value is IColumn rightColumn) {

                if(useAlias) {
                    sql.Append(rightColumn.Table.Alias).Append('.');
                }
                sql.Append(rightColumn.ColumnName);
            }
            else if(value is IFunction rightFunction) {
                sql.Append(rightFunction.GetSql(database, useAlias: useAlias, parameters));
            }
            else if(value is SqlText text) {
                sql.Append(text.AsEscapedText());
            }
            else {

                if(parameters == null) {
                    sql.Append(database.ConvertToSql(value));
                }
                else {
                    parameters.AddParameter(database, value.GetType(), value: value, out string paramName);
                    sql.Append(paramName);
                }
            }
        }
    }

    public enum Operator {
        EQUALS,
        NOT_EQUALS,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL
    }
}