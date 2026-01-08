using QueryLite;

namespace QueryLiteTest.Tables {

    public sealed class EnumTestTableTable : ATable {

        public static readonly EnumTestTableTable Instance = new();

        public Column<ByteTestEnum> ByteEnum { get; }
        public Column<ShortTestEnum> ShortEnum { get; }
        public Column<IntegerTestEnum> IntEnum { get; }
        public Column<LongTestEnum> LongEnum { get; }
        public NullableColumn<ByteTestEnum> ByteNullEnum { get; }
        public NullableColumn<ShortTestEnum> ShortNullEnum { get; }
        public NullableColumn<IntegerTestEnum> IntNullEnum { get; }
        public NullableColumn<LongTestEnum> LongNullEnum { get; }

        private EnumTestTableTable() : base(tableName: "EnumTestTable", schemaName: "dbo") {

            ByteEnum = new Column<ByteTestEnum>(this, name: "etByteEnum");
            ShortEnum = new Column<ShortTestEnum>(this, name: "etShortEnum");
            IntEnum = new Column<IntegerTestEnum>(this, name: "etIntEnum");
            LongEnum = new Column<LongTestEnum>(this, name: "etLongEnum");
            ByteNullEnum = new NullableColumn<ByteTestEnum>(this, name: "etByteNullEnum");
            ShortNullEnum = new NullableColumn<ShortTestEnum>(this, name: "etShortNullEnum");
            IntNullEnum = new NullableColumn<IntegerTestEnum>(this, name: "etIntNullEnum");
            LongNullEnum = new NullableColumn<LongTestEnum>(this, name: "etLongNullEnum");
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