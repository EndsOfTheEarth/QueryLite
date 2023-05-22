using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.PreparedQuery;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class CompiledSelectQueryTests {

        //[TestInitialize]
        //public void ClearTable() {

        //    AllTypesTable allTypesTable = AllTypesTable.Instance;

        //    using(Transaction transation = new Transaction(TestDatabase.Database)) {

        //        Query.Delete(allTypesTable)
        //            .NoWhereCondition()
        //            .Execute(transation, TimeoutLevel.ShortDelete);

        //        COUNT_ALL count = new COUNT_ALL();

        //        var result = Query
        //            .Select(
        //                result => new {
        //                    Count = result.Get(count)
        //                }
        //            )
        //            .From(allTypesTable)
        //            .Execute(transation);

        //        Assert.AreEqual(result.Rows.Count, 1);
        //        Assert.AreEqual(result.RowsEffected, 0);

        //        int? countValue = result.Rows[0].Count;

        //        Assert.IsNotNull(countValue);
        //        Assert.AreEqual(countValue, 0);

        //        transation.Commit();
        //    }
        //}

        //[TestCleanup]
        //public void CleanUp() {
        //    Settings.UseParameters = false;
        //}

        public enum SByteEnum : sbyte {
            A = -1,
            B = 0,
            C = 3
        }

        [TestMethod]
        public void TestSByteEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<SByteEnum>();

            Assert.IsTrue(type == NumericType.SByte);

            Assert.AreEqual(EnumHelper.UnsafeConvertToSByte(SByteEnum.A), (sbyte)SByteEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToSByte(SByteEnum.B), (sbyte)SByteEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToSByte(SByteEnum.C), (sbyte)SByteEnum.C);
        }

        public enum ByteEnum : byte {
            A = 0,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestByteEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<ByteEnum>();

            Assert.IsTrue(type == NumericType.Byte);

            Assert.AreEqual(EnumHelper.UnsafeConvertToByte(ByteEnum.A), (byte)ByteEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToByte(ByteEnum.B), (byte)ByteEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToByte(ByteEnum.C), (byte)ByteEnum.C);
        }

        public enum UShortEnum : ushort {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestUShortEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<UShortEnum>();

            Assert.IsTrue(type == NumericType.UShort);

            Assert.AreEqual(EnumHelper.UnsafeConvertToUShort(UShortEnum.A), (ushort)UShortEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToUShort(UShortEnum.B), (ushort)UShortEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToUShort(UShortEnum.C), (ushort)UShortEnum.C);
        }

        public enum ShortEnum : short {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestShortEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<ShortEnum>();

            Assert.IsTrue(type == NumericType.Short);

            Assert.AreEqual(EnumHelper.UnsafeConvertToShort(ShortEnum.A), (short)ShortEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToShort(ShortEnum.B), (short)ShortEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToShort(ShortEnum.C), (short)ShortEnum.C);
        }

        public enum UIntEnum : uint {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestUIntEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<UIntEnum>();

            Assert.IsTrue(type == NumericType.UInt);

            Assert.AreEqual(EnumHelper.UnsafeConvertToUInt(UIntEnum.A), (uint)UIntEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToUInt(UIntEnum.B), (uint)UIntEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToUInt(UIntEnum.C), (uint)UIntEnum.C);
        }

        public enum IntEnum : int {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestIntEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<IntEnum>();

            Assert.IsTrue(type == NumericType.Int);

            Assert.AreEqual(EnumHelper.UnsafeConvertToInt(IntEnum.A), (int)IntEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToInt(IntEnum.B), (int)IntEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToInt(IntEnum.C), (int)IntEnum.C);
        }

        public enum ULongEnum : ulong {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestULongEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<ULongEnum>();

            Assert.IsTrue(type == NumericType.ULong);

            Assert.AreEqual(EnumHelper.UnsafeConvertToULong(ULongEnum.A), (ulong)ULongEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToULong(ULongEnum.B), (ulong)ULongEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToULong(ULongEnum.C), (ulong)ULongEnum.C);
        }

        public enum LongEnum : long {
            A = 123,
            B = 1,
            C = 2
        }

        [TestMethod]
        public void TestLongEnumConversions() {

            NumericType type = EnumHelper.GetNumericType<LongEnum>();

            Assert.IsTrue(type == NumericType.Long);

            Assert.AreEqual(EnumHelper.UnsafeConvertToLong(LongEnum.A), (long)LongEnum.A);
            Assert.AreEqual(EnumHelper.UnsafeConvertToLong(LongEnum.B), (long)LongEnum.B);
            Assert.AreEqual(EnumHelper.UnsafeConvertToLong(LongEnum.C), (long)LongEnum.C);
        }

        public struct ParameterValues {

            public Guid Guid { get; set; } = Guid.NewGuid();
            public Guid Guid2 { get; set; } = Guid.NewGuid();
            public AllTypesEnum Enum { get; set; } = AllTypesEnum.A;

            public string StringValue = "asdasfs";

            public ParameterValues() {
            }
        }
        [TestMethod]
        public void Test01() {

            ParameterValues parameterValues = new ParameterValues();

            IParameter<Guid, ParameterValues> guidParameter = ParameterFactory.CreateParameter<ParameterValues>(item => item.Guid);

            IParameter<AllTypesEnum, ParameterValues> enumParameter = ParameterFactory.CreateParameter<ParameterValues, AllTypesEnum>(item => item.Enum);

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedQueryExecute<ParameterValues, IntKey<AllTypes>> selectQuery =
                Query
                .Prepare<ParameterValues>()
                .Select(row => row.Get(table.Id))
                .From(table)
                .Where(
                    table.Guid.NOT_EQUALS(guidParameter)
                    .AND(table.Guid.IS_NOT_NULL<ParameterValues, Guid>())
                    .AND(table.Enum.EQUALS(enumParameter))
                )
                .Build();

            QueryResult<IntKey<AllTypes>> result = selectQuery.Execute(parameterValues, TestDatabase.Database);
        }
    }
}