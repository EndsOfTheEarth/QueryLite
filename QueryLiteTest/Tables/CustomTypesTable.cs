using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class CustomTypesTable : ATable {

        public static readonly CustomTypesTable Instance = new CustomTypesTable();

        public Column<CustomGuid> Guid { get; }
        public Column<CustomShort> Short { get; }
        public Column<CustomInt> Int { get; }
        public Column<CustomLong> Long { get; }
        public Column<CustomString> String { get; }
        public Column<CustomBool> Bool { get; }
        public Column<CustomDecimal> Decimal { get; }

        public NullableColumn<CustomGuid> NGuid { get; }
        public NullableColumn<CustomShort> NShort { get; }
        public NullableColumn<CustomInt> NInt { get; }
        public NullableColumn<CustomLong> NLong { get; }
        public NullableColumn<CustomString> NString { get; }
        public NullableColumn<CustomBool> NBool { get; }
        public NullableColumn<CustomDecimal> NDecimal { get; }

        private CustomTypesTable() : base(tableName: "CustomTypes", schemaName: "dbo") {

            Guid = new Column<CustomGuid>(this, columnName: "ctGuid");
            Short = new Column<CustomShort>(this, columnName: "ctShort");
            Int = new Column<CustomInt>(this, columnName: "ctInt");
            Long = new Column<CustomLong>(this, columnName: "ctLong");
            String = new Column<CustomString>(this, columnName: "ctString", length: 100);
            Bool = new Column<CustomBool>(this, columnName: "ctBool");
            Decimal = new Column<CustomDecimal>(this, columnName: "ctDecimal");

            NGuid = new NullableColumn<CustomGuid>(this, columnName: "ctNGuid");
            NShort = new NullableColumn<CustomShort>(this, columnName: "ctNShort");
            NInt = new NullableColumn<CustomInt>(this, columnName: "ctNInt");
            NLong = new NullableColumn<CustomLong>(this, columnName: "ctNLong");
            NString = new NullableColumn<CustomString>(this, columnName: "ctNString", length: 100);
            NBool = new NullableColumn<CustomBool>(this, columnName: "ctNBool");
            NDecimal = new NullableColumn<CustomDecimal>(this, columnName: "ctNDecimal");
        }
    }

    public readonly struct CustomGuid : IValueOf<Guid, CustomGuid>, IValue<Guid>, IEquatable<CustomGuid> {

        public Guid Value { get; }

        public CustomGuid(Guid value) {
            Value = value;
        }
        public static CustomGuid ValueOf(Guid value) {
            return new CustomGuid(value);
        }
        public bool Equals(CustomGuid other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomGuid? pA, CustomGuid? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomGuid? pA, CustomGuid? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomGuid identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct CustomShort : IValueOf<short, CustomShort>, IValue<short>, IEquatable<CustomShort> {

        public short Value { get; }

        public CustomShort(short value) {
            Value = value;
        }
        public static CustomShort ValueOf(short value) {
            return new CustomShort(value);
        }
        public bool Equals(CustomShort other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomShort? pA, CustomShort? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomShort? pA, CustomShort? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomShort identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct CustomInt : IValueOf<int, CustomInt>, IValue<int>, IEquatable<CustomInt> {

        public int Value { get; }

        public CustomInt(int value) {
            Value = value;
        }
        public static CustomInt ValueOf(int value) {
            return new CustomInt(value);
        }
        public bool Equals(CustomInt other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomInt? pA, CustomInt? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomInt? pA, CustomInt? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomInt identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct CustomLong : IValueOf<long, CustomLong>, IValue<long>, IEquatable<CustomLong> {

        public long Value { get; }

        public CustomLong(long value) {
            Value = value;
        }
        public static CustomLong ValueOf(long value) {
            return new CustomLong(value);
        }
        public bool Equals(CustomLong other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomLong? pA, CustomLong? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomLong? pA, CustomLong? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomLong identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct CustomString : IValueOf<string, CustomString>, IValue<string>, IEquatable<CustomString> {

        public string Value { get; }

        public CustomString(string value) {
            Value = value;
        }
        public static CustomString ValueOf(string value) {
            return new CustomString(value);
        }
        public bool Equals(CustomString other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomString? pA, CustomString? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomString? pA, CustomString? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomString identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct CustomBool : IValueOf<bool, CustomBool>, IValue<bool>, IEquatable<CustomBool> {

        public bool Value { get; }

        public CustomBool(bool value) {
            Value = value;
        }
        public static CustomBool ValueOf(bool value) {
            return new CustomBool(value);
        }
        public bool Equals(CustomBool other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomBool? pA, CustomBool? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomBool? pA, CustomBool? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomBool identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }

    public readonly struct CustomDecimal : IValueOf<decimal, CustomDecimal>, IValue<decimal>, IEquatable<CustomDecimal> {

        public decimal Value { get; }

        public CustomDecimal(decimal value) {
            Value = value;
        }
        public static CustomDecimal ValueOf(decimal value) {
            return new CustomDecimal(value);
        }
        public bool Equals(CustomDecimal other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomDecimal? pA, CustomDecimal? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomDecimal? pA, CustomDecimal? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomDecimal identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? string.Empty;
        }
    }
}