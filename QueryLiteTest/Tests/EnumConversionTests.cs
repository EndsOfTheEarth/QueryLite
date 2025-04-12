using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite.Databases;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class EnumConversionTests {

        public enum SByteEnum : sbyte {
            A = -1,
            B = 0,
            C = 3
        }

        [TestMethod]
        public void TestSByteEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<SByteEnum>();

            Assert.AreEqual(NumericType.SByte, type);

            Assert.AreEqual((sbyte)SByteEnum.A, EnumHelper.UnsafeConvertToSByte(SByteEnum.A));
            Assert.AreEqual((sbyte)SByteEnum.B, EnumHelper.UnsafeConvertToSByte(SByteEnum.B));
            Assert.AreEqual((sbyte)SByteEnum.C, EnumHelper.UnsafeConvertToSByte(SByteEnum.C));
        }

        public enum ByteEnum : byte {
            A = 0,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestByteEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<ByteEnum>();

            Assert.AreEqual(NumericType.Byte, type);

            Assert.AreEqual((byte)ByteEnum.A, EnumHelper.UnsafeConvertToByte(ByteEnum.A));
            Assert.AreEqual((byte)ByteEnum.B, EnumHelper.UnsafeConvertToByte(ByteEnum.B));
            Assert.AreEqual((byte)ByteEnum.C, EnumHelper.UnsafeConvertToByte(ByteEnum.C));
        }

        public enum UShortEnum : ushort {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestUShortEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<UShortEnum>();

            Assert.AreEqual(NumericType.UShort, type);

            Assert.AreEqual((ushort)UShortEnum.A, EnumHelper.UnsafeConvertToUShort(UShortEnum.A));
            Assert.AreEqual((ushort)UShortEnum.B, EnumHelper.UnsafeConvertToUShort(UShortEnum.B));
            Assert.AreEqual((ushort)UShortEnum.C, EnumHelper.UnsafeConvertToUShort(UShortEnum.C));
        }

        public enum ShortEnum : short {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestShortEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<ShortEnum>();

            Assert.AreEqual(NumericType.Short, type);

            Assert.AreEqual((short)ShortEnum.A, EnumHelper.UnsafeConvertToShort(ShortEnum.A));
            Assert.AreEqual((short)ShortEnum.B, EnumHelper.UnsafeConvertToShort(ShortEnum.B));
            Assert.AreEqual((short)ShortEnum.C, EnumHelper.UnsafeConvertToShort(ShortEnum.C));
        }

        public enum UIntEnum : uint {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestUIntEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<UIntEnum>();

            Assert.AreEqual(NumericType.UInt, type);

            Assert.AreEqual((uint)UIntEnum.A, EnumHelper.UnsafeConvertToUInt(UIntEnum.A));
            Assert.AreEqual((uint)UIntEnum.B, EnumHelper.UnsafeConvertToUInt(UIntEnum.B));
            Assert.AreEqual((uint)UIntEnum.C, EnumHelper.UnsafeConvertToUInt(UIntEnum.C));
        }

        public enum IntEnum : int {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestIntEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<IntEnum>();

            Assert.AreEqual(NumericType.Int, type);

            Assert.AreEqual((int)IntEnum.A, EnumHelper.UnsafeConvertToInt(IntEnum.A));
            Assert.AreEqual((int)IntEnum.B, EnumHelper.UnsafeConvertToInt(IntEnum.B));
            Assert.AreEqual((int)IntEnum.C, EnumHelper.UnsafeConvertToInt(IntEnum.C));
        }

        public enum ULongEnum : ulong {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestULongEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<ULongEnum>();

            Assert.AreEqual(NumericType.ULong, type);

            Assert.AreEqual((ulong)ULongEnum.A, EnumHelper.UnsafeConvertToULong(ULongEnum.A));
            Assert.AreEqual((ulong)ULongEnum.B, EnumHelper.UnsafeConvertToULong(ULongEnum.B));
            Assert.AreEqual((ulong)ULongEnum.C, EnumHelper.UnsafeConvertToULong(ULongEnum.C));
        }

        public enum LongEnum : long {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestLongEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<LongEnum>();

            Assert.AreEqual(NumericType.Long, type);

            Assert.AreEqual((long)LongEnum.A, EnumHelper.UnsafeConvertToLong(LongEnum.A));
            Assert.AreEqual((long)LongEnum.B, EnumHelper.UnsafeConvertToLong(LongEnum.B));
            Assert.AreEqual((long)LongEnum.C, EnumHelper.UnsafeConvertToLong(LongEnum.C));
        }
    }
}