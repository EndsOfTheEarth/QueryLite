using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public readonly partial struct AllTypesId : ICustomType<int, AllTypesId>, IEquatable<AllTypesId>, IComparable<AllTypesId> {

        public static AllTypesId NotSet { get; } = new AllTypesId(0);

        public int Value { get; }

        public AllTypesId(int value) {
            Value = value;
        }
        public static AllTypesId ValueOf(int value) {
            return new AllTypesId(value);
        }
        public bool Equals(AllTypesId other) {
            return Value == other.Value;
        }

        public bool IsValid => Value != 0;

        public static bool operator ==(AllTypesId? pA, AllTypesId? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(AllTypesId? pA, AllTypesId? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(AllTypesId other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is AllTypesId identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(AllTypesId a, AllTypesId b, StringComparison comparison) {
            return a.Value == b.Value;
        }
        public static int Compare(AllTypesId a, AllTypesId b, bool ignoreCase) {
            return a.Value.CompareTo(b.Value);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }

    public readonly partial struct ChildId : ICustomType<Guid, ChildId>, IEquatable<ChildId>, IComparable<ChildId> {

        public static ChildId NotSet { get; } = new ChildId(Guid.Empty);

        public Guid Value { get; }

        public ChildId(Guid value) {
            Value = value;
        }
        public static ChildId ValueOf(Guid value) {
            return new ChildId(value);
        }
        public bool Equals(ChildId other) {
            return Value == other.Value;
        }

        public bool IsValid => Value != Guid.Empty;

        public static bool operator ==(ChildId? pA, ChildId? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(ChildId? pA, ChildId? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(ChildId other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is ChildId identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(ChildId a, ChildId b, StringComparison comparison) {
            return a.Value == b.Value;
        }
        public static int Compare(ChildId a, ChildId b, bool ignoreCase) {
            return a.Value.CompareTo(b.Value);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }

    public readonly partial struct ParentId : ICustomType<Guid, ParentId>, IEquatable<ParentId>, IComparable<ParentId> {

        public static ParentId NotSet { get; } = new ParentId(Guid.Empty);

        public Guid Value { get; }

        public ParentId(Guid value) {
            Value = value;
        }
        public static ParentId ValueOf(Guid value) {
            return new ParentId(value);
        }
        public bool Equals(ParentId other) {
            return Value == other.Value;
        }

        public bool IsValid => Value != Guid.Empty;

        public static bool operator ==(ParentId? pA, ParentId? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(ParentId? pA, ParentId? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(ParentId other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is ParentId identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(ParentId a, ParentId b, StringComparison comparison) {
            return a.Value == b.Value;
        }
        public static int Compare(ParentId a, ParentId b, bool ignoreCase) {
            return a.Value.CompareTo(b.Value);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }

    public readonly partial struct GeoTestId : ICustomType<Guid, GeoTestId>, IEquatable<GeoTestId>, IComparable<GeoTestId> {

        public static GeoTestId NotSet { get; } = new GeoTestId(Guid.Empty);

        public Guid Value { get; }

        public GeoTestId(Guid value) {
            Value = value;
        }
        public static GeoTestId ValueOf(Guid value) {
            return new GeoTestId(value);
        }
        public bool Equals(GeoTestId other) {
            return Value == other.Value;
        }

        public bool IsValid => Value != Guid.Empty;

        public static bool operator ==(GeoTestId? pA, GeoTestId? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(GeoTestId? pA, GeoTestId? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(GeoTestId other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is GeoTestId identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(GeoTestId a, GeoTestId b, StringComparison comparison) {
            return a.Value == b.Value;
        }
        public static int Compare(GeoTestId a, GeoTestId b, bool ignoreCase) {
            return a.Value.CompareTo(b.Value);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }

    public readonly partial struct RowVersionId : ICustomType<int, RowVersionId>, IEquatable<RowVersionId>, IComparable<RowVersionId> {

        public static RowVersionId NotSet { get; } = new RowVersionId(0);

        public int Value { get; }

        public RowVersionId(int value) {
            Value = value;
        }
        public static RowVersionId ValueOf(int value) {
            return new RowVersionId(value);
        }
        public bool Equals(RowVersionId other) {
            return Value == other.Value;
        }

        public bool IsValid => Value != 0;

        public static bool operator ==(RowVersionId? pA, RowVersionId? pB) {

            if(pA is null && pB is null) {
                return true;
            }
            if(pA is not null) {
                return pA.Equals(pB);
            }
            return false;
        }
        public static bool operator !=(RowVersionId? pA, RowVersionId? pB) {

            if(pA is null && pB is null) {
                return false;
            }
            if(pA is not null) {
                return !pA.Equals(pB);
            }
            return true;
        }

        public int CompareTo(RowVersionId other) {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object? obj) {

            if(obj is RowVersionId identifier) {
                return Value.CompareTo(identifier.Value) == 0;
            }
            return false;
        }

        public static bool Equals(RowVersionId a, RowVersionId b, StringComparison comparison) {
            return a.Value == b.Value;
        }
        public static int Compare(RowVersionId a, RowVersionId b, bool ignoreCase) {
            return a.Value.CompareTo(b.Value);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override string ToString() {
            return Value.ToString() ?? "";
        }
    }
}