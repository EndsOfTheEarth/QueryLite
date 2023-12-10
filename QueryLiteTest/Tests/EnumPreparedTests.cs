using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLiteTest.Tables;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class EnumPreparedTests {

        private IPreparedInsertQuery<ByteValues> _InsertQuery;
        IPreparedUpdateQuery<ByteValues> _UpdateQuery;

        public EnumPreparedTests() {

            EnumTestTableTable table = EnumTestTableTable.Instance;

            _InsertQuery = Query.Prepare<ByteValues>()
                .Insert(table)
                .Values(values => values
                    .Set(table.ByteEnum, v => v.ByteValue)
                    .Set(table.ShortEnum, v => v.ShortValue)
                    .Set(table.IntEnum, v => v.IntValue)
                    .Set(table.LongEnum, v => v.LongValue)
                    .Set(table.ByteNullEnum, v => v.ByteNullValue)
                    .Set(table.ShortNullEnum, v => v.ShortNullValue)
                    .Set(table.IntNullEnum, v => v.IntNullValue)
                    .Set(table.LongNullEnum, v => v.LongNullValue)
                )
                .Build();

            _UpdateQuery = Query.Prepare<ByteValues>()
                .Update(table)
                    .Values(values => values
                        .Set(table.ByteEnum, v => v.ByteValue)
                        .Set(table.ShortEnum, v => v.ShortValue)
                        .Set(table.IntEnum, v => v.IntValue)
                        .Set(table.LongEnum, v => v.LongValue)
                        .Set(table.ByteNullEnum, v => v.ByteNullValue)
                        .Set(table.ShortNullEnum, v => v.ShortNullValue)
                        .Set(table.IntNullEnum, v => v.IntNullValue)
                        .Set(table.LongNullEnum, v => v.LongNullValue)
                    )
                    .NoWhereCondition()
                    .Build();
        }
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
                new ByteValues(byteValue: ByteTestEnum.One, shortValue: ShortTestEnum.One, intValue: IntegerTestEnum.One, longValue: LongTestEnum.One, byteNullValue: ByteTestEnum.One, shortNullValue: ShortTestEnum.One, intNullValue: IntegerTestEnum.One, longNullValue: LongTestEnum.One)
            );
        }

        [TestMethod]
        public void TestEnumValueOneColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.One, shortValue: ShortTestEnum.One, intValue: IntegerTestEnum.One, longValue: LongTestEnum.One, byteNullValue: ByteTestEnum.One, shortNullValue: ShortTestEnum.One, intNullValue: IntegerTestEnum.One, longNullValue: LongTestEnum.One)
            );
        }

        [TestMethod]
        public void TestEnumValueMinColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.Min, shortValue: ShortTestEnum.Min, intValue: IntegerTestEnum.Min, longValue: LongTestEnum.Min, byteNullValue: ByteTestEnum.Min, shortNullValue: ShortTestEnum.Min, intNullValue: IntegerTestEnum.Min, longNullValue: LongTestEnum.Min)
            );
        }

        [TestMethod]
        public void TestEnumValueMinColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.Min, shortValue: ShortTestEnum.Min, intValue: IntegerTestEnum.Min, longValue: LongTestEnum.Min, byteNullValue: ByteTestEnum.Min, shortNullValue: ShortTestEnum.Min, intNullValue: IntegerTestEnum.Min, longNullValue: LongTestEnum.Min)
            );
        }

        [TestMethod]
        public void TestEnumValueMaxColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max, byteNullValue: ByteTestEnum.Max, shortNullValue: ShortTestEnum.Max, intNullValue: IntegerTestEnum.Max, longNullValue: LongTestEnum.Max)
            );
        }

        [TestMethod]
        public void TestEnumValueMaxColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max, byteNullValue: ByteTestEnum.Max, shortNullValue: ShortTestEnum.Max, intNullValue: IntegerTestEnum.Max, longNullValue: LongTestEnum.Max)
            );
        }

        [TestMethod]
        public void TestEnumValueNullColumns_NoParameters() {

            Settings.UseParameters = false;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max, byteNullValue: null, shortNullValue: null, intNullValue: null, longNullValue: null)
            );
        }

        [TestMethod]
        public void TestEnumValueNullColumns_Parameters() {

            Settings.UseParameters = true;
            TestEnumValueColumns(
                new ByteValues(byteValue: ByteTestEnum.Max, shortValue: ShortTestEnum.Max, intValue: IntegerTestEnum.Max, longValue: LongTestEnum.Max, byteNullValue: null, shortNullValue: null, intNullValue: null, longNullValue: null)
            );
        }

        public class ByteValues {

            public ByteValues(ByteTestEnum byteValue, ShortTestEnum shortValue, IntegerTestEnum intValue, LongTestEnum longValue, ByteTestEnum? byteNullValue, ShortTestEnum? shortNullValue, IntegerTestEnum? intNullValue, LongTestEnum? longNullValue) {
                ByteValue = byteValue;
                ShortValue = shortValue;
                IntValue = intValue;
                LongValue = longValue;
                ByteNullValue = byteNullValue;
                ShortNullValue = shortNullValue;
                IntNullValue = intNullValue;
                LongNullValue = longNullValue;
            }
            public ByteTestEnum ByteValue { get; }
            public ShortTestEnum ShortValue { get; }
            public IntegerTestEnum IntValue { get; }
            public LongTestEnum LongValue { get; }
            public ByteTestEnum? ByteNullValue { get; }
            public ShortTestEnum? ShortNullValue { get; }
            public IntegerTestEnum? IntNullValue { get; }
            public LongTestEnum? LongNullValue { get; }
        }

        /// <summary>
        /// Here we are testing the saving and loading of Enums with the types byte, short, int and long.
        /// Note that unsigned types are not supported by Sql Server or PostgreSql.
        /// </summary>
        private void TestEnumValueColumns(ByteValues byteValues) {

            EnumTestTableTable table = EnumTestTableTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                _InsertQuery.Execute(byteValues, transaction);

                transaction.Commit();
            }

            AssertSingleRow(byteValues);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                _UpdateQuery.Execute(byteValues, transaction);

                transaction.Commit();
            }

            AssertSingleRow(byteValues);
        }

        private static void AssertSingleRow(ByteValues byteValues) {

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

            Assert.AreEqual(byteValues.ByteValue, row.ByteEnum);
            Assert.AreEqual(byteValues.ShortValue, row.ShortEnum);
            Assert.AreEqual(byteValues.IntValue, row.IntEnum);
            Assert.AreEqual(byteValues.LongValue, row.LongEnum);
            Assert.AreEqual(byteValues.ByteNullValue, row.ByteNullEnum);
            Assert.AreEqual(byteValues.ShortNullValue, row.ShortNullEnum);
            Assert.AreEqual(byteValues.IntNullValue, row.IntNullEnum);
            Assert.AreEqual(byteValues.LongNullValue, row.LongNullEnum);
        }
    }
}