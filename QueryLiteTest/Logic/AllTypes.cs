
namespace QueryLiteTestLogic {

    using System;
    using QueryLite;

    public enum AllTypesEnum {
        A = 1,
        B = 2,
        C = 3
    }
    public sealed class AllTypes {

        public AllTypes(IntKey<AllTypes> id, Guid guid, string @string, short smallInt, int @int, long bigInt, decimal @decimal, float @float, double @double, bool boolean, byte[] bytes, DateTime dateTime, DateTimeOffset dateTimeOffset, AllTypesEnum @enum, DateOnly dateOnly) {
            Id = id;
            Guid = guid;
            String = @string;
            SmallInt = smallInt;
            Int = @int;
            BigInt = bigInt;
            Decimal = @decimal;
            Float = @float;
            Double = @double;
            Boolean = boolean;
            Bytes = bytes;
            DateTime = dateTime;
            DateTimeOffset = dateTimeOffset;
            Enum = @enum;
            DateOnly = dateOnly;
        }

        public void UpdateValues(Guid guid, string @string, short smallInt, int @int, long bigInt, decimal @decimal, float @float, double @double, bool boolean, byte[] bytes, DateTime dateTime, DateTimeOffset dateTimeOffset, AllTypesEnum @enum, DateOnly dateOnly) {
            Guid = guid;
            String = @string;
            SmallInt = smallInt;
            Int = @int;
            BigInt = bigInt;
            Decimal = @decimal;
            Float = @float;
            Double = @double;
            Boolean = boolean;
            Bytes = bytes;
            DateTime = dateTime;
            DateTimeOffset = dateTimeOffset;
            Enum = @enum;
            DateOnly = dateOnly;
        }

        public IntKey<AllTypes> Id { get; set; }
        public Guid Guid { get; set; }
        public string String { get; set; } = string.Empty;
        public short SmallInt { get; set; }
        public int Int { get; set; }
        public long BigInt { get; set; }
        public decimal Decimal { get; set; }
        public float Float { get; set; }
        public double Double { get; set; }
        public bool Boolean { get; set; }
        public byte[] Bytes { get; set; } = new byte[] { };
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public AllTypesEnum Enum { get; set; }
        public DateOnly DateOnly { get; set; }
    }
}