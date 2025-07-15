using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class CustomTypesTable : ATable {

        public static readonly CustomTypesTable Instance = new CustomTypesTable();

        public Column<CustomGuid, Guid> Guid { get; }
        public Column<CustomShort, short> Short { get; }
        public Column<CustomInt, int> Int { get; }
        public Column<CustomLong, long> Long { get; }
        public Column<CustomString, string> String { get; }
        public Column<CustomBool, bool> Bool { get; }
        public Column<CustomDecimal, decimal> Decimal { get; }
        public Column<CustomDateTime, DateTime> DateTime { get; }
        public Column<CustomDateTimeOffset, DateTimeOffset> DateTimeOffset { get; }
        public Column<CustomDateOnly, DateOnly> DateOnly { get; }
        public Column<CustomTimeOnly, TimeOnly> TimeOnly { get; }
        public Column<CustomFloat, float> Float { get; }
        public Column<CustomDouble, double> Double { get; }

        public NullableColumn<CustomGuid, Guid> NGuid { get; }
        public NullableColumn<CustomShort, short> NShort { get; }
        public NullableColumn<CustomInt, int> NInt { get; }
        public NullableColumn<CustomLong, long> NLong { get; }
        public NullableColumn<CustomString, string> NString { get; }
        public NullableColumn<CustomBool, bool> NBool { get; }
        public NullableColumn<CustomDecimal, decimal> NDecimal { get; }
        public NullableColumn<CustomDateTime, DateTime> NDateTime { get; }
        public NullableColumn<CustomDateTimeOffset, DateTimeOffset> NDateTimeOffset { get; }
        public NullableColumn<CustomDateOnly, DateOnly> NDateOnly { get; }
        public NullableColumn<CustomTimeOnly, TimeOnly> NTimeOnly { get; }
        public NullableColumn<CustomFloat, float> NFloat { get; }
        public NullableColumn<CustomDouble, double> NDouble { get; }

        private CustomTypesTable() : base(tableName: "CustomTypes", schemaName: "dbo") {

            Guid = new Column<CustomGuid, Guid>(this, columnName: "ctGuid");
            Short = new Column<CustomShort, short>(this, columnName: "ctShort");
            Int = new Column<CustomInt, int>(this, columnName: "ctInt");
            Long = new Column<CustomLong, long>(this, columnName: "ctLong");
            String = new Column<CustomString, string>(this, columnName: "ctString", length: 100);
            Bool = new Column<CustomBool, bool>(this, columnName: "ctBool");
            Decimal = new Column<CustomDecimal, decimal>(this, columnName: "ctDecimal");
            DateTime = new Column<CustomDateTime, DateTime>(this, columnName: "ctDateTime");
            DateTimeOffset = new Column<CustomDateTimeOffset, DateTimeOffset>(this, columnName: "ctDateTimeOffset");
            DateOnly = new Column<CustomDateOnly, DateOnly>(this, columnName: "ctDateOnly");
            TimeOnly = new Column<CustomTimeOnly, TimeOnly>(this, columnName: "ctTimeOnly");
            Float = new Column<CustomFloat, float>(this, columnName: "ctFloat");
            Double = new Column<CustomDouble, double>(this, columnName: "ctDouble");

            NGuid = new NullableColumn<CustomGuid, Guid>(this, columnName: "ctNGuid");
            NShort = new NullableColumn<CustomShort, short>(this, columnName: "ctNShort");
            NInt = new NullableColumn<CustomInt, int>(this, columnName: "ctNInt");
            NLong = new NullableColumn<CustomLong, long>(this, columnName: "ctNLong");
            NString = new NullableColumn<CustomString, string>(this, columnName: "ctNString", length: 100);
            NBool = new NullableColumn<CustomBool, bool>(this, columnName: "ctNBool");
            NDecimal = new NullableColumn<CustomDecimal, decimal>(this, columnName: "ctNDecimal");
            NDateTime = new NullableColumn<CustomDateTime, DateTime>(this, columnName: "ctNDateTime");
            NDateTimeOffset = new NullableColumn<CustomDateTimeOffset, DateTimeOffset>(this, columnName: "ctNDateTimeOffset");
            NDateOnly = new NullableColumn<CustomDateOnly, DateOnly>(this, columnName: "ctNDateOnly");
            NTimeOnly = new NullableColumn<CustomTimeOnly, TimeOnly>(this, columnName: "ctNTimeOnly");
            NFloat = new NullableColumn<CustomFloat, float>(this, columnName: "ctNFloat");
            NDouble = new NullableColumn<CustomDouble, double>(this, columnName: "ctNDouble");
        }
    }

    public readonly struct CustomGuid : ICustomType<Guid, CustomGuid>, IEquatable<CustomGuid> {

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

    public readonly struct CustomShort : ICustomType<short, CustomShort>, IEquatable<CustomShort> {

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

    public readonly struct CustomInt : ICustomType<int, CustomInt>, IEquatable<CustomInt> {

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

    public readonly struct CustomLong : ICustomType<long, CustomLong>, IEquatable<CustomLong> {

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

    public readonly struct CustomString : ICustomType<string, CustomString>, IEquatable<CustomString> {

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

    public readonly struct CustomBool : ICustomType<bool, CustomBool>, IEquatable<CustomBool> {

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

    public readonly struct CustomDecimal : ICustomType<decimal, CustomDecimal>, IEquatable<CustomDecimal> {

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

    public readonly struct CustomDateTime : ICustomType<DateTime, CustomDateTime>, IEquatable<CustomDateTime> {

        public DateTime Value { get; }

        public CustomDateTime(DateTime value) {
            Value = value;
        }
        public static CustomDateTime ValueOf(DateTime value) {
            return new CustomDateTime(value);
        }
        public bool Equals(CustomDateTime other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomDateTime? pA, CustomDateTime? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomDateTime? pA, CustomDateTime? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomDateTime identifier) {
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

    public readonly struct CustomDateTimeOffset : ICustomType<DateTimeOffset, CustomDateTimeOffset>, IEquatable<CustomDateTimeOffset> {

        public DateTimeOffset Value { get; }

        public CustomDateTimeOffset(DateTimeOffset value) {
            Value = value;
        }
        public static CustomDateTimeOffset ValueOf(DateTimeOffset value) {
            return new CustomDateTimeOffset(value);
        }
        public bool Equals(CustomDateTimeOffset other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomDateTimeOffset? pA, CustomDateTimeOffset? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomDateTimeOffset? pA, CustomDateTimeOffset? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomDateTimeOffset identifier) {
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

    public readonly struct CustomDateOnly : ICustomType<DateOnly, CustomDateOnly>, IEquatable<CustomDateOnly> {

        public DateOnly Value { get; }

        public CustomDateOnly(DateOnly value) {
            Value = value;
        }
        public static CustomDateOnly ValueOf(DateOnly value) {
            return new CustomDateOnly(value);
        }
        public bool Equals(CustomDateOnly other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomDateOnly? pA, CustomDateOnly? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomDateOnly? pA, CustomDateOnly? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomDateOnly identifier) {
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

    public readonly struct CustomTimeOnly : ICustomType<TimeOnly, CustomTimeOnly>, IEquatable<CustomTimeOnly> {

        public TimeOnly Value { get; }

        public CustomTimeOnly(TimeOnly value) {
            Value = value;
        }
        public static CustomTimeOnly ValueOf(TimeOnly value) {
            return new CustomTimeOnly(value);
        }
        public bool Equals(CustomTimeOnly other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomTimeOnly? pA, CustomTimeOnly? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomTimeOnly? pA, CustomTimeOnly? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomTimeOnly identifier) {
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

    public readonly struct CustomFloat : ICustomType<float, CustomFloat>, IEquatable<CustomFloat> {

        public float Value { get; }

        public CustomFloat(float value) {
            Value = value;
        }
        public static CustomFloat ValueOf(float value) {
            return new CustomFloat(value);
        }
        public bool Equals(CustomFloat other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomFloat? pA, CustomFloat? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomFloat? pA, CustomFloat? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomFloat identifier) {
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

    public readonly struct CustomDouble : ICustomType<double, CustomDouble>, IEquatable<CustomDouble> {

        public double Value { get; }

        public CustomDouble(double value) {
            Value = value;
        }
        public static CustomDouble ValueOf(double value) {
            return new CustomDouble(value);
        }
        public bool Equals(CustomDouble other) {
            return Value == other.Value;
        }

        public static bool operator ==(CustomDouble? pA, CustomDouble? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(CustomDouble? pA, CustomDouble? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public override bool Equals(object? obj) {

            if(obj is CustomDouble identifier) {
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