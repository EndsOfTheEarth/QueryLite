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

namespace QueryLite {

    public interface IFunction : IField {

        /// <summary>
        /// Descriptive name for function
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the function sql. This is used during sql query deneration
        /// </summary>
        /// <param name="database"></param>
        /// <param name="useAlias">Should table aliases be included in sql</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal string GetSql(IDatabase database, bool useAlias, IParameters? parameters);
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
            return new List<IField>(new IField[] { this });
        }
        string IFunction.GetSql(IDatabase database, bool useAlias, IParameters? parameters) {
            return GetSql(database, useAlias, parameters);
        }
        public abstract string GetSql(IDatabase database, bool useAlias, IParameters? parameters);

        public static ICondition operator ==(AFunction<TYPE> function, AFunction<TYPE> columnB) {
            return new GenericCondition(function, Operator.EQUALS, columnB);
        }
        public static ICondition operator !=(AFunction<TYPE> function, AFunction<TYPE> columnB) {
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
    public abstract class NullableFunction<TYPE> : AFunction<TYPE> where TYPE : notnull {

        protected NullableFunction(string name) : base(name) {

        }
    }
}