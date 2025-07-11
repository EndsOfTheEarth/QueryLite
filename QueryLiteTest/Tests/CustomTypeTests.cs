using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
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

                COUNT_ALL count = COUNT_ALL.Instance;

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

                            .SetGuid(table.Guid, customTypesA.CustomGuid)
                            .SetShort(table.Short, customTypesA.CustomShort)
                            .SetInt(table.Int, customTypesA.CustomInt)
                            .SetLong(table.Long, customTypesA.CustomLong)
                            .SetString(table.String, customTypesA.CustomString)
                            .SetBool(table.Bool, customTypesA.CustomBool)
                            .SetDecimal(table.Decimal, customTypesA.CustomDecimal)
                            .SetDateTime(table.DateTime, customTypesA.CustomDateTime)
                            .SetDateTimeOffset(table.DateTimeOffset, customTypesA.CustomDateTimeOffset)
                            .SetDateOnly(table.DateOnly, customTypesA.CustomDateOnly)
                            .SetTimeOnly(table.TimeOnly, customTypesA.CustomTimeOnly)
                            .SetFloat(table.Float, customTypesA.CustomFloat)
                            .SetDouble(table.Double, customTypesA.CustomDouble)

                            .SetGuid(table.NGuid, customTypesA.NCustomGuid)
                            .SetShort(table.NShort, customTypesA.NCustomShort)
                            .SetInt(table.NInt, customTypesA.NCustomInt)
                            .SetLong(table.NLong, customTypesA.NCustomLong)
                            .SetString(table.NString, customTypesA.NCustomString)
                            .SetBool(table.NBool, customTypesA.NCustomBool)
                            .SetDecimal(table.NDecimal, customTypesA.NCustomDecimal)
                            .SetDateTime(table.NDateTime, customTypesA.NCustomDateTime)
                            .SetDateTimeOffset(table.NDateTimeOffset, customTypesA.NCustomDateTimeOffset)
                            .SetDateOnly(table.NDateOnly, customTypesA.NCustomDateOnly)
                            .SetTimeOnly(table.NTimeOnly, customTypesA.NCustomTimeOnly)
                            .SetFloat(table.NFloat, customTypesA.NCustomFloat)
                            .SetDouble(table.NDouble, customTypesA.NCustomDouble)
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

                            .SetGuid(table.Guid, customTypesB.CustomGuid)
                            .SetShort(table.Short, customTypesB.CustomShort)
                            .SetInt(table.Int, customTypesB.CustomInt)
                            .SetLong(table.Long, customTypesB.CustomLong)
                            .SetString(table.String, customTypesB.CustomString)
                            .SetBool(table.Bool, customTypesB.CustomBool)
                            .SetDecimal(table.Decimal, customTypesB.CustomDecimal)
                            .SetDateTime(table.DateTime, customTypesB.CustomDateTime)
                            .SetDateTimeOffset(table.DateTimeOffset, customTypesB.CustomDateTimeOffset)
                            .SetDateOnly(table.DateOnly, customTypesB.CustomDateOnly)
                            .SetTimeOnly(table.TimeOnly, customTypesB.CustomTimeOnly)
                            .SetFloat(table.Float, customTypesB.CustomFloat)
                            .SetDouble(table.Double, customTypesB.CustomDouble)

                            .SetGuid(table.NGuid, customTypesB.NCustomGuid)
                            .SetShort(table.NShort, customTypesB.NCustomShort)
                            .SetInt(table.NInt, customTypesB.NCustomInt)
                            .SetLong(table.NLong, customTypesB.NCustomLong)
                            .SetString(table.NString, customTypesB.NCustomString)
                            .SetBool(table.NBool, customTypesB.NCustomBool)
                            .SetDecimal(table.NDecimal, customTypesB.NCustomDecimal)
                            .SetDateTime(table.NDateTime, customTypesB.NCustomDateTime)
                            .SetDateTimeOffset(table.NDateTimeOffset, customTypesB.NCustomDateTimeOffset)
                            .SetDateOnly(table.NDateOnly, customTypesB.NCustomDateOnly)
                            .SetTimeOnly(table.NTimeOnly, customTypesB.NCustomTimeOnly)
                            .SetFloat(table.NFloat, customTypesB.NCustomFloat)
                            .SetDouble(table.NDouble, customTypesB.NCustomDouble)
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

        private static void AssertOnlyOneRowExists() {

            CustomTypesTable table = CustomTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

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

                    customGuid: row.GetGuid(table.Guid),
                    customShort: row.GetShort(table.Short),
                    customInt: row.GetInt(table.Int),
                    customLong: row.GetLong(table.Long),
                    customString: row.GetString(table.String),
                    customBool: row.GetBool(table.Bool),
                    customDecimal: row.GetDecimal(table.Decimal),
                    customDateTime: row.GetDateTime(table.DateTime),
                    customDateTimeOffset: row.GetDateTimeOffset(table.DateTimeOffset),
                    customDateOnly: row.GetDateOnly(table.DateOnly),
                    customTimeOnly: row.GetTimeOnly(table.TimeOnly),
                    customFloat: row.GetFloat(table.Float),
                    customDouble: row.GetDouble(table.Double),

                    nCustomGuid: row.GetGuid(table.NGuid),
                    nCustomShort: row.GetShort(table.NShort),
                    nCustomInt: row.GetInt(table.NInt),
                    nCustomLong: row.GetLong(table.NLong),
                    nCustomString: row.GetString(table.NString),
                    nCustomBool: row.GetBool(table.NBool),
                    nCustomDecimal: row.GetDecimal(table.NDecimal),
                    nCustomDateTime: row.GetDateTime(table.NDateTime),
                    nCustomDateTimeOffset: row.GetDateTimeOffset(table.NDateTimeOffset),
                    nCustomDateOnly: row.GetDateOnly(table.NDateOnly),
                    nCustomTimeOnly: row.GetTimeOnly(table.NTimeOnly),
                    nCustomFloat: row.GetFloat(table.NFloat),
                    nCustomDouble: row.GetDouble(table.NDouble)
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