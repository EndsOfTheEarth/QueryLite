using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLiteTest.Tables;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class EnumTests {

        [TestInitialize]
        public void ClearTable() {

            EnumTestTableTable table = EnumTestTableTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(table)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                transaction.Commit();
            }
        }

        [TestCleanup]
        public void CleanUp() {
            Settings.UseParameters = false;
        }

        [TestMethod]
        public void TestEnumValueOneColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.One, shortValue: ShortTestEnum.One, intValue: IntegerTestEnum.One, longValue: LongTestEnum.One,
                byteNullValue: ByteTestEnum.One, shortNullValue: ShortTestEnum.One, intNullValue: IntegerTestEnum.One, longNullValue: LongTestEnum.One);
        }

        [TestMethod]
        public void TestEnumValueOneColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.One, shortValue: ShortTestEnum.One, intValue: IntegerTestEnum.One, longValue: LongTestEnum.One,
                byteNullValue: ByteTestEnum.One, shortNullValue: ShortTestEnum.One, intNullValue: IntegerTestEnum.One, longNullValue: LongTestEnum.One);
        }

        [TestMethod]
        public void TestEnumValueMinColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.Min, shortValue: ShortTestEnum.Min, intValue: IntegerTestEnum.Min, longValue: LongTestEnum.Min,
                byteNullValue: ByteTestEnum.Min, shortNullValue: ShortTestEnum.Min, intNullValue: IntegerTestEnum.Min, longNullValue: LongTestEnum.Min);
        }

        [TestMethod]
        public void TestEnumValueMinColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.Min, shortValue: ShortTestEnum.Min, intValue: IntegerTestEnum.Min, longValue: LongTestEnum.Min,
                byteNullValue: ByteTestEnum.Min, shortNullValue: ShortTestEnum.Min, intNullValue: IntegerTestEnum.Min, longNullValue: LongTestEnum.Min);
        }

        [TestMethod]
        public void TestEnumValueMaxColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max,
                byteNullValue: ByteTestEnum.Max, shortNullValue: ShortTestEnum.Max, intNullValue: IntegerTestEnum.Max, longNullValue: LongTestEnum.Max);
        }

        [TestMethod]
        public void TestEnumValueMaxColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max,
                byteNullValue: ByteTestEnum.Max, shortNullValue: ShortTestEnum.Max, intNullValue: IntegerTestEnum.Max, longNullValue: LongTestEnum.Max);
        }

        [TestMethod]
        public void TestEnumValueNullColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max,
                byteNullValue: null, shortNullValue: null, intNullValue: null, longNullValue: null);
        }

        [TestMethod]
        public void TestEnumValueNullColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max,
                byteNullValue: null, shortNullValue: null, intNullValue: null, longNullValue: null);
        }

        /// <summary>
        /// Here we are testing the saving and loading of Enums with the types byte, short, int and long.
        /// Note that unsigned types are not supported by Sql Server or PostgreSql.
        /// </summary>
        private void TestEnumValueColumns(ByteTestEnum byteValue, ShortTestEnum shortValue, IntegerTestEnum intValue, LongTestEnum longValue, ByteTestEnum? byteNullValue, ShortTestEnum? shortNullValue, IntegerTestEnum? intNullValue, LongTestEnum? longNullValue) {

            EnumTestTableTable table = EnumTestTableTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Insert(table)
                    .Values(values => values
                        .Set(table.ByteEnum, byteValue)
                        .Set(table.ShortEnum, shortValue)
                        .Set(table.IntEnum, intValue)
                        .Set(table.LongEnum, longValue)
                        .Set(table.ByteNullEnum, byteNullValue)
                        .Set(table.ShortNullEnum, shortNullValue)
                        .Set(table.IntNullEnum, intNullValue)
                        .Set(table.LongNullEnum, longNullValue)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            AssertSingleRow(
                byteValue: byteValue, shortValue: shortValue, intValue: intValue, longValue: longValue,
                byteNullValue: byteNullValue, shortNullValue: shortNullValue, intNullValue: intNullValue, longNullValue: longNullValue
            );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Update(table)
                    .Values(values => values
                        .Set(table.ByteEnum, byteValue)
                        .Set(table.ShortEnum, shortValue)
                        .Set(table.IntEnum, intValue)
                        .Set(table.LongEnum, longValue)
                        .Set(table.ByteNullEnum, byteNullValue)
                        .Set(table.ShortNullEnum, shortNullValue)
                        .Set(table.IntNullEnum, intNullValue)
                        .Set(table.LongNullEnum, longNullValue)
                    )
                    .NoWhereCondition()
                    .Execute(transaction);

                transaction.Commit();
            }

            AssertSingleRow(
                byteValue: byteValue, shortValue: shortValue, intValue: intValue, longValue: longValue,
                byteNullValue: byteNullValue, shortNullValue: shortNullValue, intNullValue: intNullValue, longNullValue: longNullValue
            );
        }

        private static void AssertSingleRow(
            ByteTestEnum byteValue, ShortTestEnum shortValue, IntegerTestEnum intValue, LongTestEnum longValue,
            ByteTestEnum? byteNullValue, ShortTestEnum? shortNullValue, IntegerTestEnum? intNullValue, LongTestEnum? longNullValue
            ) {

            EnumTestTableTable table = EnumTestTableTable.Instance;

            var result = Query
                .Select(
                    row => new {
                        ByteEnum = row.Get(table.ByteEnum),
                        ShortEnum = row.Get(table.ShortEnum),
                        IntEnum = row.Get(table.IntEnum),
                        LongEnum = row.Get(table.LongEnum),
                        ByteNullEnum = row.Get(table.ByteNullEnum),
                        ShortNullEnum = row.Get(table.ShortNullEnum),
                        IntNullEnum = row.Get(table.IntNullEnum),
                        LongNullEnum = row.Get(table.LongNullEnum)
                    }
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            var row = result.Rows[0];

            Assert.AreEqual(byteValue, row.ByteEnum);
            Assert.AreEqual(shortValue, row.ShortEnum);
            Assert.AreEqual(intValue, row.IntEnum);
            Assert.AreEqual(longValue, row.LongEnum);
            Assert.AreEqual(byteNullValue, row.ByteNullEnum);
            Assert.AreEqual(shortNullValue, row.ShortNullEnum);
            Assert.AreEqual(intNullValue, row.IntNullEnum);
            Assert.AreEqual(longNullValue, row.LongNullEnum);
        }
    }
}