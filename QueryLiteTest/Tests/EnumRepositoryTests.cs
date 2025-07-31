using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLiteTest.Tables;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class EnumRepositoryTests {

        [TestInitialize]
        public void ClearTable() {

            EnumTestTableTable table = EnumTestTableTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                EnumRepository repository = new EnumRepository();

                repository.SelectRows.Execute(transaction);

                foreach(EnumRow row in repository) {
                    repository.DeleteRow(row);
                }
                repository.Update(transaction);
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
        private static void TestEnumValueColumns(ByteTestEnum byteValue, ShortTestEnum shortValue, IntegerTestEnum intValue, LongTestEnum longValue, ByteTestEnum? byteNullValue, ShortTestEnum? shortNullValue, IntegerTestEnum? intNullValue, LongTestEnum? longNullValue) {

            EnumRepository repository = new EnumRepository();

            EnumRow row = new EnumRow(
                byteEnum: byteValue,
                shortEnum: shortValue,
                intEnum: intValue,
                longEnum: longValue,
                byteNullEnum: byteNullValue,
                shortNullEnum: shortNullValue,
                intNullEnum: intNullValue,
                longNullEnum: longNullValue
            );

            repository.AddNewRow(row);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                repository.PersistInsertsOnly(transaction);
                transaction.Commit();
            }

            AssertSingleRow(
                byteValue: byteValue, shortValue: shortValue, intValue: intValue, longValue: longValue,
                byteNullValue: byteNullValue, shortNullValue: shortNullValue, intNullValue: intNullValue, longNullValue: longNullValue
            );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                row.ByteEnum = byteValue;
                row.ShortEnum = shortValue;
                row.IntEnum = intValue;
                row.LongEnum = longValue;
                row.ByteNullEnum = byteNullValue;
                row.ShortNullEnum = shortNullValue;
                row.IntNullEnum = intNullValue;
                row.LongNullEnum = longNullValue;

                repository.Update(transaction);

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

            {
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

            {
                EnumRepository repository = new EnumRepository();

                repository.SelectRows
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, repository.Count);

                EnumRow row = repository[0];

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
}