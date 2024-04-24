using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLiteTest.Tables;
using System;
using static QueryLiteTest.Tests.CustomTypeTests;

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

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue, 0);

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

        private void TestInsert() {

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
                        )
                        .Where(table.Guid == customTypesA.CustomGuid)
                        .Execute(transaction);

                    transaction.Commit();

                    Assert.AreEqual(1, insertResult.RowsEffected);
                }
            }
            AssertCustomTypes(customTypesB);
        }

        private void AssertOnlyOneRowExists() {

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
        private void AssertCustomTypes(CustomTypes customTypes) {

            CustomTypesTable table = CustomTypesTable.Instance;

            QueryResult<CustomTypes> result = Query.Select(
                    row => new CustomTypes(
                        customGuid: row.GetGuid(table.Guid),
                        customShort: row.GetShort(table.Short),
                        customInt: row.GetInt(table.Int),
                        customLong: row.GetLong(table.Long),
                        customString: row.GetString(table.String),
                        customBool: row.GetBool(table.Bool)
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
        }

        private CustomTypes GetCustomTypesA() => new CustomTypes(
            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(12),
            customInt: CustomInt.ValueOf(55),
            customLong: CustomLong.ValueOf(43213412),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(true)
        );

        private CustomTypes GetCustomTypesB() => new CustomTypes(
            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(43),
            customInt: CustomInt.ValueOf(2384234),
            customLong: CustomLong.ValueOf(6525234),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(false)
        );

        public class CustomTypes {

            public CustomGuid CustomGuid { get; set; }
            public CustomShort CustomShort { get; set; }
            public CustomInt CustomInt { get; set; }
            public CustomLong CustomLong { get; set; }
            public CustomString CustomString { get; set; }
            public CustomBool CustomBool { get; set; }

            public CustomTypes(CustomGuid customGuid, CustomShort customShort, CustomInt customInt, CustomLong customLong, CustomString customString, CustomBool customBool) {
                CustomGuid = customGuid;
                CustomShort = customShort;
                CustomInt = customInt;
                CustomLong = customLong;
                CustomString = customString;
                CustomBool = customBool;
            }
        }
    }
}