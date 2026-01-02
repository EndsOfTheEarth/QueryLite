/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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
namespace QueryLite.DbSchema {

    public interface IUnknownType { }

    public readonly partial struct SchemaName : ICustomType<string, SchemaName>, IEquatable<SchemaName>, IComparable<SchemaName> {

        public static SchemaName Empty { get; } = new SchemaName("");

        private readonly string _value = "";

        public string Value => _value ?? "";

        public SchemaName(string value) {
            _value = value ?? "";
        }
        public static SchemaName ValueOf(string value) {
            return new SchemaName(value);
        }
        public bool Equals(SchemaName other) {
            return Value == other.Value;
        }

        public bool IsEmpty => Value != "";

        public int Length => Value.Length;

        public static bool operator ==(SchemaName? pA, SchemaName? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(SchemaName? pA, SchemaName? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(SchemaName other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is SchemaName identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(SchemaName a, SchemaName b, StringComparison comparison) {
            return string.Equals(a.Value, b.Value, comparison);
        }
        public static int Compare(SchemaName a, SchemaName b, bool ignoreCase) {
            return string.Compare(a.Value, b.Value, ignoreCase);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }

    public readonly partial struct TableName : ICustomType<string, TableName>, IEquatable<TableName>, IComparable<TableName> {

        public static TableName Empty { get; } = new TableName("");

        private readonly string _value = "";

        public string Value => _value ?? "";

        public TableName(string value) {
            _value = value ?? "";
        }
        public static TableName ValueOf(string value) {
            return new TableName(value);
        }
        public bool Equals(TableName other) {
            return Value == other.Value;
        }

        public bool IsEmpty => Value != "";

        public int Length => Value.Length;

        public static bool operator ==(TableName? pA, TableName? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(TableName? pA, TableName? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(TableName other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is TableName identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(TableName a, TableName b, StringComparison comparison) {
            return string.Equals(a.Value, b.Value, comparison);
        }
        public static int Compare(TableName a, TableName b, bool ignoreCase) {
            return string.Compare(a.Value, b.Value, ignoreCase);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }

    public readonly partial struct ColumnName : ICustomType<string, ColumnName>, IEquatable<ColumnName>, IComparable<ColumnName> {

        public static ColumnName Empty { get; } = new ColumnName("");

        private readonly string _value = "";

        public string Value => _value ?? "";

        public ColumnName(string value) {
            _value = value ?? "";
        }
        public static ColumnName ValueOf(string value) {
            return new ColumnName(value);
        }
        public bool Equals(ColumnName other) {
            return Value == other.Value;
        }

        public bool IsEmpty => Value != "";

        public int Length => Value.Length;

        public static bool operator ==(ColumnName? pA, ColumnName? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(ColumnName? pA, ColumnName? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(ColumnName other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is ColumnName identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(ColumnName a, ColumnName b, StringComparison comparison) {
            return string.Equals(a.Value, b.Value, comparison);
        }
        public static int Compare(ColumnName a, ColumnName b, bool ignoreCase) {
            return string.Compare(a.Value, b.Value, ignoreCase);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }
}