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

                            .SetGuid(table.NGuid, customTypesA.NCustomGuid)
                            .SetShort(table.NShort, customTypesA.NCustomShort)
                            .SetInt(table.NInt, customTypesA.NCustomInt)
                            .SetLong(table.NLong, customTypesA.NCustomLong)
                            .SetString(table.NString, customTypesA.NCustomString)
                            .SetBool(table.NBool, customTypesA.NCustomBool)
                            .SetDecimal(table.NDecimal, customTypesA.NCustomDecimal)
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

                            .SetGuid(table.NGuid, customTypesB.NCustomGuid)
                            .SetShort(table.NShort, customTypesB.NCustomShort)
                            .SetInt(table.NInt, customTypesB.NCustomInt)
                            .SetLong(table.NLong, customTypesB.NCustomLong)
                            .SetString(table.NString, customTypesB.NCustomString)
                            .SetBool(table.NBool, customTypesB.NCustomBool)
                            .SetDecimal(table.NDecimal, customTypesB.NCustomDecimal)
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
                            table.Decimal == customTypesB.CustomDecimal
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

                        nCustomGuid: row.GetGuid(table.NGuid),
                        nCustomShort: row.GetShort(table.NShort),
                        nCustomInt: row.GetInt(table.NInt),
                        nCustomLong: row.GetLong(table.NLong),
                        nCustomString: row.GetString(table.NString),
                        nCustomBool: row.GetBool(table.NBool),
                        nCustomDecimal: row.GetDecimal(table.NDecimal)
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

            Assert.AreEqual(customTypes.NCustomGuid, values.NCustomGuid);
            Assert.AreEqual(customTypes.NCustomShort, values.NCustomShort);
            Assert.AreEqual(customTypes.NCustomInt, values.NCustomInt);
            Assert.AreEqual(customTypes.NCustomLong, values.NCustomLong);
            Assert.AreEqual(customTypes.NCustomString, values.NCustomString);
            Assert.AreEqual(customTypes.NCustomBool, values.NCustomBool);
            Assert.AreEqual(customTypes.NCustomDecimal, values.NCustomDecimal);
        }

        private static CustomTypes GetCustomTypesA() => new CustomTypes(

            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(12),
            customInt: CustomInt.ValueOf(55),
            customLong: CustomLong.ValueOf(43213412),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(true),
            customDecimal: CustomDecimal.ValueOf(0.22345200m),

            nCustomGuid: null,
            nCustomShort: null,
            nCustomInt: null,
            nCustomLong: null,
            nCustomString: null,
            nCustomBool: null,
            nCustomDecimal: null
        );

        private static CustomTypes GetCustomTypesB() => new CustomTypes(

            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(43),
            customInt: CustomInt.ValueOf(2384234),
            customLong: CustomLong.ValueOf(6525234),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(false),
            customDecimal: CustomDecimal.ValueOf(-23452345.65474567m),

            nCustomGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            nCustomShort: CustomShort.ValueOf(11),
            nCustomInt: CustomInt.ValueOf(3232),
            nCustomLong: CustomLong.ValueOf(2414564),
            nCustomString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            nCustomBool: CustomBool.ValueOf(true),
            nCustomDecimal: CustomDecimal.ValueOf(2134.45234m)
        );

        public class CustomTypes {

            public CustomGuid CustomGuid { get; set; }
            public CustomShort CustomShort { get; set; }
            public CustomInt CustomInt { get; set; }
            public CustomLong CustomLong { get; set; }
            public CustomString CustomString { get; set; }
            public CustomBool CustomBool { get; set; }
            public CustomDecimal CustomDecimal { get; set; }

            public CustomGuid? NCustomGuid { get; set; }
            public CustomShort? NCustomShort { get; set; }
            public CustomInt? NCustomInt { get; set; }
            public CustomLong? NCustomLong { get; set; }
            public CustomString? NCustomString { get; set; }
            public CustomBool? NCustomBool { get; set; }
            public CustomDecimal? NCustomDecimal { get; set; }

            public CustomTypes(CustomGuid customGuid, CustomShort customShort, CustomInt customInt, CustomLong customLong, CustomString customString, CustomBool customBool, CustomDecimal customDecimal, CustomGuid? nCustomGuid, CustomShort? nCustomShort, CustomInt? nCustomInt, CustomLong? nCustomLong, CustomString? nCustomString, CustomBool? nCustomBool, CustomDecimal? nCustomDecimal) {
                CustomGuid = customGuid;
                CustomShort = customShort;
                CustomInt = customInt;
                CustomLong = customLong;
                CustomString = customString;
                CustomBool = customBool;
                CustomDecimal = customDecimal;
                NCustomGuid = nCustomGuid;
                NCustomShort = nCustomShort;
                NCustomInt = nCustomInt;
                NCustomLong = nCustomLong;
                NCustomString = nCustomString;
                NCustomBool = nCustomBool;
                NCustomDecimal = nCustomDecimal;
            }
        }
    }
}