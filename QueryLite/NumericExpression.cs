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

    public sealed class NumericExpression : ICondition {

        internal NumericExpression(NumericExpression left, string @operator, object right) {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        internal NumericExpression(ISelectable left, string @operator, object right) {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        private object Left { get; init; }
        private string Operator { get; init; }
        private object Right { get; init; }

        public void GetSql(StringBuilder sql, IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            sql.Append('(');

            if(Left is NumericExpression leftExpr) {
                leftExpr.GetSql(sql, database, useAlias: useAlias, parameters);
            }
            else {
                ConditionHelper.AppendSqlValue(Left, sql, database, useAlias: useAlias, parameters);
            }

            sql.Append(' ').Append(Operator).Append(' ');

            if(Right is NumericExpression rightExpr) {
                rightExpr.GetSql(sql, database, useAlias: useAlias, parameters);
            }
            else {
                ConditionHelper.AppendSqlValue(Right, sql, database, useAlias: useAlias, parameters);
            }
            sql.Append(')');
        }

        #region Equals

        public static ICondition operator ==(NumericExpression expr, ISelectable value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "=", value);
        }
        public static ICondition operator ==(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "=", value);
        }
        #endregion

        #region Not Equals

        public static ICondition operator !=(NumericExpression expr, ISelectable value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        public static ICondition operator !=(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "!=", value);
        }
        #endregion

        #region Greater Than Equals

        public static ICondition operator >=(NumericExpression expr, ISelectable value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        public static ICondition operator >=(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, ">=", value);
        }
        #endregion

        #region Less Than Equals

        public static ICondition operator <=(NumericExpression expr, ISelectable value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        public static ICondition operator <=(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "<=", value);
        }
        #endregion

        #region Plus

        public static NumericExpression operator +(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "+", value);
        }
        #endregion

        #region Minus

        public static NumericExpression operator -(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "-", value);
        }
        #endregion

        #region Multiply

        public static NumericExpression operator *(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "*", value);
        }
        #endregion

        #region Divide

        public static NumericExpression operator /(NumericExpression expr, NumericExpression value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, short value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, int value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, long value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, decimal value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, float value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(NumericExpression expr, double value) {
            return new NumericExpression(left: expr, "/", value);
        }
        #endregion

        public override bool Equals(object? obj) {

            if(ReferenceEquals(this, obj)) {
                return true;
            }
            if(obj is null) {
                return false;
            }
            return false;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}