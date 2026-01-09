using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Functions;
using QueryLiteTest.Tables;
using System;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class CustomTypeTests {

        [TestInitialize]
        public void ClearTable() {

            CustomTypesTable table = CustomTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(table)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                Count count = new();

                QueryResult<int> result = Query
                    .Select(result => result.Get(count))
                    .From(table)
                    .Execute(transaction);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int countValue = result.Rows[0];

                Assert.AreEqual(0, countValue);

                transaction.Commit();
            }
        }

        [TestCleanup]
        public void CleanUp() {
            Settings.UseParameters = false;
        }

        [TestMethod]
        public void TestRowVersions_NoParameters() {

            Settings.UseParameters = false;
            TestInsert();
        }

        [TestMethod]
        public void TestRowVersions_Parameters() {

            Settings.UseParameters = true;
            TestInsert();
        }

        private static void TestInsert() {

            CustomTypesTable table = CustomTypesTable.Instance;

            CustomTypes customTypesA = GetCustomTypesA();

            {
                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    NonQueryResult insertResult = Query
                        .Insert(table)
                        .Values(values => values

                            .Set(table.Guid, customTypesA.CustomGuid)
                            .Set(table.Short, customTypesA.CustomShort)
                            .Set(table.Int, customTypesA.CustomInt)
                            .Set(table.Long, customTypesA.CustomLong)
                            .Set(table.String, customTypesA.CustomString)
                            .Set(table.Bool, customTypesA.CustomBool)
                            .Set(table.Decimal, customTypesA.CustomDecimal)
                            .Set(table.DateTime, customTypesA.CustomDateTime)
                            .Set(table.DateTimeOffset, customTypesA.CustomDateTimeOffset)
                            .Set(table.DateOnly, customTypesA.CustomDateOnly)
                            .Set(table.TimeOnly, customTypesA.CustomTimeOnly)
                            .Set(table.Float, customTypesA.CustomFloat)
                            .Set(table.Double, customTypesA.CustomDouble)

                            .Set(table.NGuid, customTypesA.NCustomGuid)
                            .Set(table.NShort, customTypesA.NCustomShort)
                            .Set(table.NInt, customTypesA.NCustomInt)
                            .Set(table.NLong, customTypesA.NCustomLong)
                            .Set(table.NString, customTypesA.NCustomString)
                            .Set(table.NBool, customTypesA.NCustomBool)
                            .Set(table.NDecimal, customTypesA.NCustomDecimal)
                            .Set(table.NDateTime, customTypesA.NCustomDateTime)
                            .Set(table.NDateTimeOffset, customTypesA.NCustomDateTimeOffset)
                            .Set(table.NDateOnly, customTypesA.NCustomDateOnly)
                            .Set(table.NTimeOnly, customTypesA.NCustomTimeOnly)
                            .Set(table.NFloat, customTypesA.NCustomFloat)
                            .Set(table.NDouble, customTypesA.NCustomDouble)
                        )
                        .Execute(transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }

            AssertOnlyOneRowExists();

            AssertCustomTypes(customTypesA);

            CustomTypes customTypesB = GetCustomTypesB();

            {
                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    NonQueryResult insertResult = Query
                        .Update(table)
                        .Values(values => values

                            .Set(table.Guid, customTypesB.CustomGuid)
                            .Set(table.Short, customTypesB.CustomShort)
                            .Set(table.Int, customTypesB.CustomInt)
                            .Set(table.Long, customTypesB.CustomLong)
                            .Set(table.String, customTypesB.CustomString)
                            .Set(table.Bool, customTypesB.CustomBool)
                            .Set(table.Decimal, customTypesB.CustomDecimal)
                            .Set(table.DateTime, customTypesB.CustomDateTime)
                            .Set(table.DateTimeOffset, customTypesB.CustomDateTimeOffset)
                            .Set(table.DateOnly, customTypesB.CustomDateOnly)
                            .Set(table.TimeOnly, customTypesB.CustomTimeOnly)
                            .Set(table.Float, customTypesB.CustomFloat)
                            .Set(table.Double, customTypesB.CustomDouble)

                            .Set(table.NGuid, customTypesB.NCustomGuid)
                            .Set(table.NShort, customTypesB.NCustomShort)
                            .Set(table.NInt, customTypesB.NCustomInt)
                            .Set(table.NLong, customTypesB.NCustomLong)
                            .Set(table.NString, customTypesB.NCustomString)
                            .Set(table.NBool, customTypesB.NCustomBool)
                            .Set(table.NDecimal, customTypesB.NCustomDecimal)
                            .Set(table.NDateTime, customTypesB.NCustomDateTime)
                            .Set(table.NDateTimeOffset, customTypesB.NCustomDateTimeOffset)
                            .Set(table.NDateOnly, customTypesB.NCustomDateOnly)
                            .Set(table.NTimeOnly, customTypesB.NCustomTimeOnly)
                            .Set(table.NFloat, customTypesB.NCustomFloat)
                            .Set(table.NDouble, customTypesB.NCustomDouble)
                        )
                        .Where(table.Guid == customTypesA.CustomGuid)
                        .Execute(transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }
            AssertCustomTypes(customTypesB);

            {
                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    NonQueryResult insertResult = Query
                        .Delete(table)
                        .Where(
                            table.Guid == customTypesB.CustomGuid &
                            table.Short == customTypesB.CustomShort &
                            table.Int == customTypesB.CustomInt &
                            table.Long == customTypesB.CustomLong &
                            table.String == customTypesB.CustomString &
                            table.Bool == customTypesB.CustomBool &
                            table.Decimal == customTypesB.CustomDecimal &
                            table.DateTime == customTypesB.CustomDateTime &
                            table.DateTimeOffset == customTypesB.CustomDateTimeOffset &
                            table.DateOnly == customTypesB.CustomDateOnly &
                            table.TimeOnly == customTypesB.CustomTimeOnly
                        //Note: Comparing floats and doubles is problematic so this is excluded from the tests.
                        //table.Float == customTypesB.CustomFloat & 
                        //table.Double == customTypesB.CustomDouble
                        )
                        .Execute(transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }
        }

        [TestMethod]
        public void TestPreparedInsert() {

            CustomTypesTable table = CustomTypesTable.Instance;

            CustomTypes customTypesA = GetCustomTypesA();

            {

                IPreparedInsertQuery<CustomTypes> preparedInsertQuery = Query
                    .Prepare<CustomTypes>()
                    .Insert(table)
                    .Values(values => values

                        .Set(table.Guid, t => t.CustomGuid)
                        .Set(table.Short, t => t.CustomShort)
                        .Set(table.Int, t => t.CustomInt)
                        .Set(table.Long, t => t.CustomLong)
                        .Set(table.String, t => t.CustomString)
                        .Set(table.Bool, t => t.CustomBool)
                        .Set(table.Decimal, t => t.CustomDecimal)
                        .Set(table.DateTime, t => t.CustomDateTime)
                        .Set(table.DateTimeOffset, t => t.CustomDateTimeOffset)
                        .Set(table.DateOnly, t => t.CustomDateOnly)
                        .Set(table.TimeOnly, t => t.CustomTimeOnly)
                        .Set(table.Float, t => t.CustomFloat)
                        .Set(table.Double, t => t.CustomDouble)

                        .Set(table.NGuid, t => t.NCustomGuid)
                        .Set(table.NShort, t => t.NCustomShort)
                        .Set(table.NInt, t => t.NCustomInt)
                        .Set(table.NLong, t => t.NCustomLong)
                        .Set(table.NString, t => t.NCustomString)
                        .Set(table.NBool, t => t.NCustomBool)
                        .Set(table.NDecimal, t => t.NCustomDecimal)
                        .Set(table.NDateTime, t => t.NCustomDateTime)
                        .Set(table.NDateTimeOffset, t => t.NCustomDateTimeOffset)
                        .Set(table.NDateOnly, t => t.NCustomDateOnly)
                        .Set(table.NTimeOnly, t => t.NCustomTimeOnly)
                        .Set(table.NFloat, t => t.NCustomFloat)
                        .Set(table.NDouble, t => t.NCustomDouble)
                    )
                    .Build();

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    NonQueryResult insertResult = preparedInsertQuery.Execute(customTypesA, transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }

            AssertOnlyOneRowExists();

            AssertCustomTypes(customTypesA);

            CustomTypes customTypesB = GetCustomTypesB();

            {

                IPreparedUpdateQuery<CustomTypes> preparedUpdateQuery = Query
                    .Prepare<CustomTypes>()
                    .Update(table)
                    .Values(values => values

                        .Set(table.Guid, t => t.CustomGuid)
                        .Set(table.Short, t => t.CustomShort)
                        .Set(table.Int, t => t.CustomInt)
                        .Set(table.Long, t => t.CustomLong)
                        .Set(table.String, t => t.CustomString)
                        .Set(table.Bool, t => t.CustomBool)
                        .Set(table.Decimal, t => t.CustomDecimal)
                        .Set(table.DateTime, t => t.CustomDateTime)
                        .Set(table.DateTimeOffset, t => t.CustomDateTimeOffset)
                        .Set(table.DateOnly, t => t.CustomDateOnly)
                        .Set(table.TimeOnly, t => t.CustomTimeOnly)
                        .Set(table.Float, t => t.CustomFloat)
                        .Set(table.Double, t => t.CustomDouble)

                        .Set(table.NGuid, t => t.NCustomGuid)
                        .Set(table.NShort, t => t.NCustomShort)
                        .Set(table.NInt, t => t.NCustomInt)
                        .Set(table.NLong, t => t.NCustomLong)
                        .Set(table.NString, t => t.NCustomString)
                        .Set(table.NBool, t => t.NCustomBool)
                        .Set(table.NDecimal, t => t.NCustomDecimal)
                        .Set(table.NDateTime, t => t.NCustomDateTime)
                        .Set(table.NDateTimeOffset, t => t.NCustomDateTimeOffset)
                        .Set(table.NDateOnly, t => t.NCustomDateOnly)
                        .Set(table.NTimeOnly, t => t.NCustomTimeOnly)
                        .Set(table.NFloat, t => t.NCustomFloat)
                        .Set(table.NDouble, t => t.NCustomDouble)
                    )
                    .Where(where => where.EQUALS(table.Guid, t => t.PreviousCustomGuid))
                    .Build();

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    customTypesB.PreviousCustomGuid = customTypesA.CustomGuid;

                    NonQueryResult insertResult = preparedUpdateQuery.Execute(customTypesB, transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }
            AssertCustomTypes(customTypesB);

            {

                IPreparedDeleteQuery<CustomTypes> preparedDeleteQuery = Query.Prepare<CustomTypes>()
                    .Delete(table)
                    .Where(where =>
                        where.EQUALS(table.Guid, t => t.CustomGuid) &
                        where.EQUALS(table.Short, t => t.CustomShort) &
                        where.EQUALS(table.Int, t => t.CustomInt) &
                        where.EQUALS(table.Long, t => t.CustomLong) &
                        where.EQUALS(table.String, t => t.CustomString) &
                        where.EQUALS(table.Bool, t => t.CustomBool) &
                        where.EQUALS(table.Decimal, t => t.CustomDecimal) &
                        where.EQUALS(table.DateTime, t => t.CustomDateTime) &
                        where.EQUALS(table.DateTimeOffset, t => t.CustomDateTimeOffset) &
                        where.EQUALS(table.DateOnly, t => t.CustomDateOnly) &
                        where.EQUALS(table.TimeOnly, t => t.CustomTimeOnly)
                    //Note: Comparing floats and doubles is problematic so this is excluded from the tests.
                    //where.EQUALS(table.Float, t => t.CustomFloat) &
                    //where.EQUALS(table.Double, t => t.CustomDouble)
                    )
                    .Build();

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    NonQueryResult insertResult = preparedDeleteQuery.Execute(customTypesB, transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }
        }

        private static void AssertOnlyOneRowExists() {

            CustomTypesTable table = CustomTypesTable.Instance;

            Count count = new();

            QueryResult<int> result = Query.Select(
                    row => row.Get(count)
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(1, result.Rows[0]);
        }

        private static void AssertCustomTypes(CustomTypes customTypes) {

            CustomTypesTable table = CustomTypesTable.Instance;

            QueryResult<CustomTypes> result = Query.Select(
                row => new CustomTypes(

                    customGuid: row.Get(table.Guid),
                    customShort: row.Get(table.Short),
                    customInt: row.Get(table.Int),
                    customLong: row.Get(table.Long),
                    customString: row.Get(table.String),
                    customBool: row.Get(table.Bool),
                    customDecimal: row.Get(table.Decimal),
                    customDateTime: row.Get(table.DateTime),
                    customDateTimeOffset: row.Get(table.DateTimeOffset),
                    customDateOnly: row.Get(table.DateOnly),
                    customTimeOnly: row.Get(table.TimeOnly),
                    customFloat: row.Get(table.Float),
                    customDouble: row.Get(table.Double),

                    nCustomGuid: row.Get(table.NGuid),
                    nCustomShort: row.Get(table.NShort),
                    nCustomInt: row.Get(table.NInt),
                    nCustomLong: row.Get(table.NLong),
                    nCustomString: row.Get(table.NString),
                    nCustomBool: row.Get(table.NBool),
                    nCustomDecimal: row.Get(table.NDecimal),
                    nCustomDateTime: row.Get(table.NDateTime),
                    nCustomDateTimeOffset: row.Get(table.NDateTimeOffset),
                    nCustomDateOnly: row.Get(table.NDateOnly),
                    nCustomTimeOnly: row.Get(table.NTimeOnly),
                    nCustomFloat: row.Get(table.NFloat),
                    nCustomDouble: row.Get(table.NDouble)
                )
            )
            .From(table)
            .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            CustomTypes values = result.Rows[0];

            Assert.AreEqual(customTypes.CustomGuid, values.CustomGuid);
            Assert.AreEqual(customTypes.CustomShort, values.CustomShort);
            Assert.AreEqual(customTypes.CustomInt, values.CustomInt);
            Assert.AreEqual(customTypes.CustomLong, values.CustomLong);
            Assert.AreEqual(customTypes.CustomString, values.CustomString);
            Assert.AreEqual(customTypes.CustomBool, values.CustomBool);
            Assert.AreEqual(customTypes.CustomDecimal, values.CustomDecimal);
            Assert.AreEqual(customTypes.CustomDateTime, values.CustomDateTime);
            Assert.AreEqual(customTypes.CustomDateTimeOffset, values.CustomDateTimeOffset);
            Assert.AreEqual(customTypes.CustomDateOnly, values.CustomDateOnly);
            Assert.AreEqual(customTypes.CustomTimeOnly, values.CustomTimeOnly);
            Assert.AreEqual(customTypes.CustomFloat, values.CustomFloat);
            Assert.AreEqual(customTypes.CustomDouble, values.CustomDouble);

            Assert.AreEqual(customTypes.NCustomGuid, values.NCustomGuid);
            Assert.AreEqual(customTypes.NCustomShort, values.NCustomShort);
            Assert.AreEqual(customTypes.NCustomInt, values.NCustomInt);
            Assert.AreEqual(customTypes.NCustomLong, values.NCustomLong);
            Assert.AreEqual(customTypes.NCustomString, values.NCustomString);
            Assert.AreEqual(customTypes.NCustomBool, values.NCustomBool);
            Assert.AreEqual(customTypes.NCustomDecimal, values.NCustomDecimal);
            Assert.AreEqual(customTypes.NCustomDateTime, values.NCustomDateTime);
            Assert.AreEqual(customTypes.NCustomDateTimeOffset, values.NCustomDateTimeOffset);
            Assert.AreEqual(customTypes.NCustomDateOnly, values.NCustomDateOnly);
            Assert.AreEqual(customTypes.NCustomTimeOnly, values.NCustomTimeOnly);
            Assert.AreEqual(customTypes.NCustomFloat, values.NCustomFloat);
            Assert.AreEqual(customTypes.NCustomDouble, values.NCustomDouble);
        }

        private static CustomTypes GetCustomTypesA() => new CustomTypes(

            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(12),
            customInt: CustomInt.ValueOf(55),
            customLong: CustomLong.ValueOf(43213412),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(true),
            customDecimal: CustomDecimal.ValueOf(0.22345200m),
            customDateTime: CustomDateTime.ValueOf(new DateTime(year: 2025, month: 03, day: 10, hour: 15, minute: 01, second: 7)),
            customDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2024, month: 02, day: 11, hour: 22, minute: 52, second: 12, new TimeSpan(hours: 5, minutes: 0, seconds: 0))),
            customDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 2025, month: 05, day: 12)),
            customTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 01, minute: 00, second: 25)),
            customFloat: CustomFloat.ValueOf(342.1234423f),
            customDouble: CustomDouble.ValueOf(45152345234.234523452345d),

            nCustomGuid: null,
            nCustomShort: null,
            nCustomInt: null,
            nCustomLong: null,
            nCustomString: null,
            nCustomBool: null,
            nCustomDecimal: null,
            nCustomDateTime: null,
            nCustomDateTimeOffset: null,
            nCustomDateOnly: null,
            nCustomTimeOnly: null,
            nCustomFloat: null,
            nCustomDouble: null
        );

        private static CustomTypes GetCustomTypesB() => new CustomTypes(

            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(43),
            customInt: CustomInt.ValueOf(2384234),
            customLong: CustomLong.ValueOf(6525234),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(false),
            customDecimal: CustomDecimal.ValueOf(-23452345.65474567m),
            customDateTime: CustomDateTime.ValueOf(new DateTime(year: 2010, month: 01, day: 30, hour: 17, minute: 02, second: 7)),
            customDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1990, month: 10, day: 11, hour: 21, minute: 05, second: 01, new TimeSpan(hours: 12, minutes: 0, seconds: 0))),
            customDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 2024, month: 06, day: 13)),
            customTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 04, minute: 01, second: 27)),
            customFloat: CustomFloat.ValueOf(3.14535f),
            customDouble: CustomDouble.ValueOf(92345234523.98726452345d),

            nCustomGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            nCustomShort: CustomShort.ValueOf(11),
            nCustomInt: CustomInt.ValueOf(3232),
            nCustomLong: CustomLong.ValueOf(2414564),
            nCustomString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            nCustomBool: CustomBool.ValueOf(true),
            nCustomDecimal: CustomDecimal.ValueOf(2134.45234m),
            nCustomDateTime: CustomDateTime.ValueOf(new DateTime(year: 2023, month: 9, day: 11, hour: 15, minute: 01, second: 7)),
            nCustomDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2023, month: 05, day: 12, hour: 10, minute: 52, second: 15, new TimeSpan(hours: 7, minutes: 0, seconds: 0))),
            nCustomDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 1990, month: 12, day: 31)),
            nCustomTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 23, minute: 59, second: 59)),
            nCustomFloat: CustomFloat.ValueOf(123423.2345234f),
            nCustomDouble: CustomDouble.ValueOf(73458.22347589234d)
        );

        public class CustomTypes {

            public CustomGuid PreviousCustomGuid { get; set; }

            public CustomGuid CustomGuid { get; set; }
            public CustomShort CustomShort { get; set; }
            public CustomInt CustomInt { get; set; }
            public CustomLong CustomLong { get; set; }
            public CustomString CustomString { get; set; }
            public CustomBool CustomBool { get; set; }
            public CustomDecimal CustomDecimal { get; set; }
            public CustomDateTime CustomDateTime { get; set; }
            public CustomDateTimeOffset CustomDateTimeOffset { get; set; }
            public CustomDateOnly CustomDateOnly { get; set; }
            public CustomTimeOnly CustomTimeOnly { get; set; }
            public CustomFloat CustomFloat { get; set; }
            public CustomDouble CustomDouble { get; set; }

            public CustomGuid? NCustomGuid { get; set; }
            public CustomShort? NCustomShort { get; set; }
            public CustomInt? NCustomInt { get; set; }
            public CustomLong? NCustomLong { get; set; }
            public CustomString? NCustomString { get; set; }
            public CustomBool? NCustomBool { get; set; }
            public CustomDecimal? NCustomDecimal { get; set; }
            public CustomDateTime? NCustomDateTime { get; set; }
            public CustomDateTimeOffset? NCustomDateTimeOffset { get; set; }
            public CustomDateOnly? NCustomDateOnly { get; set; }
            public CustomTimeOnly? NCustomTimeOnly { get; set; }
            public CustomFloat? NCustomFloat { get; set; }
            public CustomDouble? NCustomDouble { get; set; }

            public CustomTypes(CustomGuid customGuid, CustomShort customShort, CustomInt customInt,
                               CustomLong customLong, CustomString customString, CustomBool customBool,
                               CustomDecimal customDecimal, CustomDateTime customDateTime,
                               CustomDateTimeOffset customDateTimeOffset, CustomDateOnly customDateOnly,
                               CustomTimeOnly customTimeOnly, CustomFloat customFloat, CustomDouble customDouble,
                               CustomGuid? nCustomGuid, CustomShort? nCustomShort, CustomInt? nCustomInt,
                               CustomLong? nCustomLong, CustomString? nCustomString, CustomBool? nCustomBool,
                               CustomDecimal? nCustomDecimal, CustomDateTime? nCustomDateTime,
                               CustomDateTimeOffset? nCustomDateTimeOffset, CustomDateOnly? nCustomDateOnly,
                               CustomTimeOnly? nCustomTimeOnly, CustomFloat? nCustomFloat, CustomDouble? nCustomDouble) {
                CustomGuid = customGuid;
                CustomShort = customShort;
                CustomInt = customInt;
                CustomLong = customLong;
                CustomString = customString;
                CustomBool = customBool;
                CustomDecimal = customDecimal;
                CustomDateTime = customDateTime;
                CustomDateTimeOffset = customDateTimeOffset;
                CustomDateOnly = customDateOnly;
                CustomTimeOnly = customTimeOnly;
                CustomFloat = customFloat;
                CustomDouble = customDouble;
                NCustomGuid = nCustomGuid;
                NCustomShort = nCustomShort;
                NCustomInt = nCustomInt;
                NCustomLong = nCustomLong;
                NCustomString = nCustomString;
                NCustomBool = nCustomBool;
                NCustomDecimal = nCustomDecimal;
                NCustomDateTime = nCustomDateTime;
                NCustomDateTimeOffset = nCustomDateTimeOffset;
                NCustomDateOnly = nCustomDateOnly;
                NCustomTimeOnly = nCustomTimeOnly;
                NCustomFloat = nCustomFloat;
                NCustomDouble = nCustomDouble;
            }
        }
    }
}