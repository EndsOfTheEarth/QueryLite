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
namespace QueryLite.Databases.Functions {

    public static class SqlMath {

        public static Add<VALUE> Add<VALUE>(Column<VALUE> column, VALUE value) where VALUE : notnull {
            return new Add<VALUE>(column, value);
        }
        public static Subtract<VALUE> Subtract<VALUE>(Column<VALUE> column, VALUE value) where VALUE : notnull {
            return new Subtract<VALUE>(column, value);
        }
    }

    public sealed class Add<VALUE> : Function<VALUE> where VALUE : notnull {

        private Column<VALUE> Column { get; }
        private VALUE Value { get; }

        public Add(Column<VALUE> column, VALUE value) : base(name: "ADD") {
            Column = column;
            Value = value;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"({Column.Table.Alias}.{SqlHelper.EncloseColumnName(Column)} + {Value})" : $"({SqlHelper.EncloseColumnName(Column)} + {Value})";
        }
    }

    public sealed class Subtract<VALUE> : Function<VALUE> where VALUE : notnull {

        private Column<VALUE> Column { get; }
        private VALUE Value { get; }

        public Subtract(Column<VALUE> column, VALUE value) : base(name: "ADD") {
            Column = column;
            Value = value;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"({Column.Table.Alias}.{SqlHelper.EncloseColumnName(Column)} - {Value})" : $"({SqlHelper.EncloseColumnName(Column)} - {Value})";
        }
    }
}