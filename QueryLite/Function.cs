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
namespace QueryLite {

    public interface IFunction : IField {

        /// <summary>
        /// Descriptive name for function
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the function sql. This is used during sql query generation
        /// </summary>
        /// <param name="database"></param>
        /// <param name="useAlias">Should table aliases be included in sql</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters);
    }

    public abstract class AFunction<TYPE> : ISelectable<TYPE>, IFunction where TYPE : notnull {

        public Type Type => typeof(TYPE);

        public string Name { get; }

        public IOrderByColumn ASC => new OrderByColumn(this, OrderBy.ASC);
        public IOrderByColumn DESC => new OrderByColumn(this, OrderBy.DESC);

        public IField Field => this;
        public OrderBy OrderBy => OrderBy.Default;

        protected AFunction(string name) {
            Name = name;
        }

        public IList<IField> GetFields() {
            return new List<IField>([this]);
        }
        string IFunction.GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return GetSql(database, useAlias, parameters);
        }
        public abstract string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters);

        public static ICondition operator ==(AFunction<TYPE> function, ISelectable<TYPE> columnB) {
            return new GenericCondition(function, Operator.EQUALS, columnB);
        }
        public static ICondition operator !=(AFunction<TYPE> function, ISelectable<TYPE> columnB) {
            return new GenericCondition(function, Operator.NOT_EQUALS, columnB);
        }

        public static ICondition operator ==(AFunction<TYPE> function, TYPE value) {
            return new GenericCondition(function, Operator.EQUALS, value);
        }
        public static ICondition operator !=(AFunction<TYPE> function, TYPE value) {
            return new GenericCondition(function, Operator.NOT_EQUALS, value);
        }
        
        public static ICondition operator <(AFunction<TYPE> function, TYPE value) {
            return new GenericCondition(function, Operator.LESS_THAN, value);
        }
        public static ICondition operator <=(AFunction<TYPE> function, TYPE value) {
            return new GenericCondition(function, Operator.LESS_THAN_OR_EQUAL, value);
        }
        public static ICondition operator >(AFunction<TYPE> function, TYPE value) {
            return new GenericCondition(function, Operator.GREATER_THAN, value);
        }
        public static ICondition operator >=(AFunction<TYPE> function, TYPE value) {
            return new GenericCondition(function, Operator.GREATER_THAN_OR_EQUAL, value);
        }

        public static ICondition operator ==(AFunction<TYPE> function, NumericExpression value) {
            return new NumericExpression(left: function, "=", value);
        }
        public static ICondition operator !=(AFunction<TYPE> function, NumericExpression value) {
            return new NumericExpression(left: function, "!=", value);
        }
        public static ICondition operator <(AFunction<TYPE> function, NumericExpression value) {
            return new NumericExpression(left: function, "<", value);
        }
        public static ICondition operator <=(AFunction<TYPE> function, NumericExpression value) {
            return new NumericExpression(left: function, "<=", value);
        }
        public static ICondition operator >(AFunction<TYPE> function, NumericExpression value) {
            return new NumericExpression(left: function, ">", value);
        }
        public static ICondition operator >=(AFunction<TYPE> function, NumericExpression value) {
            return new NumericExpression(left: function, ">=", value);
        }

        public static ICondition operator ==(AFunction<TYPE> function, ISelectable value) {
            return new NumericExpression(left: function, "=", value);
        }
        public static ICondition operator !=(AFunction<TYPE> function, ISelectable value) {
            return new NumericExpression(left: function, "!=", value);
        }
        public static ICondition operator <(AFunction<TYPE> function, ISelectable value) {
            return new NumericExpression(left: function, "<", value);
        }
        public static ICondition operator <=(AFunction<TYPE> function, ISelectable value) {
            return new NumericExpression(left: function, "<=", value);
        }
        public static ICondition operator >(AFunction<TYPE> function, ISelectable value) {
            return new NumericExpression(left: function, ">", value);
        }
        public static ICondition operator >=(AFunction<TYPE> function, ISelectable value) {
            return new NumericExpression(left: function, ">=", value);
        }

        #region Plus

        public static NumericExpression operator +(AFunction<TYPE> expr, NumericExpression value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, short value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, int value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, long value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, decimal value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, float value) {
            return new NumericExpression(left: expr, "+", value);
        }

        public static NumericExpression operator +(AFunction<TYPE> expr, double value) {
            return new NumericExpression(left: expr, "+", value);
        }
        #endregion

        #region Minus

        public static NumericExpression operator -(AFunction<TYPE> expr, NumericExpression value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, short value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, int value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, long value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, decimal value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, float value) {
            return new NumericExpression(left: expr, "-", value);
        }

        public static NumericExpression operator -(AFunction<TYPE> expr, double value) {
            return new NumericExpression(left: expr, "-", value);
        }
        #endregion

        #region Multiply

        public static NumericExpression operator *(AFunction<TYPE> expr, NumericExpression value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, short value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, int value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, long value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, decimal value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, float value) {
            return new NumericExpression(left: expr, "*", value);
        }

        public static NumericExpression operator *(AFunction<TYPE> expr, double value) {
            return new NumericExpression(left: expr, "*", value);
        }
        #endregion

        #region Divide

        public static NumericExpression operator /(AFunction<TYPE> expr, NumericExpression value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, ISelectable<short> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, ISelectable<int> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, ISelectable<long> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, ISelectable<decimal> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, ISelectable<float> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, ISelectable<double> value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, short value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, int value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, long value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, decimal value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, float value) {
            return new NumericExpression(left: expr, "/", value);
        }

        public static NumericExpression operator /(AFunction<TYPE> expr, double value) {
            return new NumericExpression(left: expr, "/", value);
        }
        #endregion




        //
        //  Between conditions
        //
        public ICondition Between(TYPE valueA, TYPE valueB) {
            return new BetweenCondition<TYPE>(not: false, left: this, valueA: valueA, valueB: valueB);
        }
        public ICondition Between(TYPE valueA, ISelectable<TYPE> valueB) {
            return new BetweenCondition<TYPE>(not: false, left: this, valueA: valueA, valueB: valueB);
        }
        public ICondition Between(ISelectable<TYPE> valueA, TYPE valueB) {
            return new BetweenCondition<TYPE>(not: false, left: this, valueA: valueA, valueB: valueB);
        }
        public ICondition Between(ISelectable<TYPE> valueA, ISelectable<TYPE> valueB) {
            return new BetweenCondition<TYPE>(not: false, left: this, valueA: valueA, valueB: valueB);
        }

        public ICondition NotBetween(TYPE valueA, TYPE valueB) {
            return new BetweenCondition<TYPE>(not: true, left: this, valueA: valueA, valueB: valueB);
        }
        public ICondition NotBetween(TYPE valueA, ISelectable<TYPE> valueB) {
            return new BetweenCondition<TYPE>(not: true, left: this, valueA: valueA, valueB: valueB);
        }
        public ICondition NotBetween(ISelectable<TYPE> valueA, TYPE valueB) {
            return new BetweenCondition<TYPE>(not: true, left: this, valueA: valueA, valueB: valueB);
        }
        public ICondition NotBetween(ISelectable<TYPE> valueA, ISelectable<TYPE> valueB) {
            return new BetweenCondition<TYPE>(not: true, left: this, valueA: valueA, valueB: valueB);
        }

        public ICondition IsNull => new NullNotNullFunctionCondition<TYPE>(this, isNull: true);
        public ICondition IsNotNull => new NullNotNullFunctionCondition<TYPE>(this, isNull: false);

        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj) {
            return base.Equals(obj);
        }
    }

    public abstract class Function<TYPE> : AFunction<TYPE> where TYPE : notnull {

        protected Function(string name) : base(name) {

        }
    }
    /// <summary>
    /// Nullable function. A function that can return a null value
    /// </summary>
    /// <typeparam name="TYPE"></typeparam>
    public abstract class NFunction<TYPE> : AFunction<TYPE> where TYPE : notnull {

        protected NFunction(string name) : base(name) {

        }
    }

    public sealed class RawSqlFunction<TYPE> : Function<TYPE> where TYPE : notnull {

        private string Sql { get; }

        public RawSqlFunction(string sql) : base(sql) {
            Sql = sql;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return Sql;
        }
    }

    /// <summary>
    /// Raw Sql Nullable function. A function that can return a null value.
    /// </summary>
    /// <typeparam name="TYPE"></typeparam>
    public sealed class NRawSqlFunction<TYPE> : NFunction<TYPE> where TYPE : notnull {

        private string Sql { get; }

        public NRawSqlFunction(string sql) : base(sql) {
            Sql = sql;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return Sql;
        }
    }
}