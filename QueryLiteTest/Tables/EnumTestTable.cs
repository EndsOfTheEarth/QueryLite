using QueryLite;

namespace QueryLiteTest.Tables {

    public sealed class EnumTestTableTable : ATable {

        public static readonly EnumTestTableTable Instance = new();

        public Column<ByteTestEnum> ByteEnum { get; }
        public Column<ShortTestEnum> ShortEnum { get; }
        public Column<IntegerTestEnum> IntEnum { get; }
        public Column<LongTestEnum> LongEnum { get; }
        public NColumn<ByteTestEnum> ByteNullEnum { get; }
        public NColumn<ShortTestEnum> ShortNullEnum { get; }
        public NColumn<IntegerTestEnum> IntNullEnum { get; }
        public NColumn<LongTestEnum> LongNullEnum { get; }

        private EnumTestTableTable() : base(tableName: "EnumTestTable", schemaName: "dbo") {

            ByteEnum = new Column<ByteTestEnum>(this, name: "etByteEnum");
            ShortEnum = new Column<ShortTestEnum>(this, name: "etShortEnum");
            IntEnum = new Column<IntegerTestEnum>(this, name: "etIntEnum");
            LongEnum = new Column<LongTestEnum>(this, name: "etLongEnum");
            ByteNullEnum = new NColumn<ByteTestEnum>(this, name: "etByteNullEnum");
            ShortNullEnum = new NColumn<ShortTestEnum>(this, name: "etShortNullEnum");
            IntNullEnum = new NColumn<IntegerTestEnum>(this, name: "etIntNullEnum");
            LongNullEnum = new NColumn<LongTestEnum>(this, name: "etLongNullEnum");
        }
    }

    [Repository<EnumTestTableTable>(MatchOn.AllColumns, "EnumRepository")]
    public partial record EnumRow {

    }

    public enum ByteTestEnum : byte {
        One = 1,
        Min = byte.MinValue,
        Max = byte.MaxValue
    }
    public enum ShortTestEnum : short {
        One = 1,
        Min = short.MinValue,
        Max = short.MaxValue
    }
    public enum IntegerTestEnum : int {
        One = 1,
        Min = int.MinValue,
        Max = int.MaxValue
    }
    public enum LongTestEnum : long {
        One = 1,
        Min = long.MinValue,
        Max = long.MaxValue
    }
}