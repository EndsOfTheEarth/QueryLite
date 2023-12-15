using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.Utility;
using QueryLiteTest.Tables;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class RowVersionTests {

        [TestInitialize]
        public void ClearTable() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            RowVersionTestTable table = RowVersionTestTable.Instance;

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
            TestRowVersions();
        }

        [TestMethod]
        public void TestRowVersions_Parameters() {

            Settings.UseParameters = true;
            TestRowVersions();
        }

        private void TestRowVersions() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            RowVersionTestTable table = RowVersionTestTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                for(int index = 0; index < 10; index++) {

                    NonQueryResult insertResult = Query
                        .Insert(table)
                        .Values(values => values
                            .Set(table.Id, IntKey<IRowVersionTest>.ValueOf(index))
                            .Set(table.TextValue, index.ToString())
                        )
                        .Execute(transaction);

                    Assert.AreEqual(insertResult.RowsEffected, 1);
                }
                transaction.Commit();
            }

            QueryResult<byte[]> result = Query
                .Select(
                    row => row.Get(table.RowVersion)
                )
                .From(table)
                .OrderBy(table.Id.ASC)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 10);

            byte[] previousBytes = new byte[8];

            for(int index = 0; index < result.Rows.Count; index++) {

                byte[] rowVersion = result.Rows[index];

                Assert.AreEqual(rowVersion.Length, 8);

                bool isEqual = true;

                //Check the row version is different from the previous value
                for(int byteIndex = 0; byteIndex < rowVersion.Length; byteIndex++) {

                    if(rowVersion[byteIndex] != previousBytes[byteIndex]) {
                        isEqual = false;
                        break;
                    }
                }
                Assert.IsFalse(isEqual);
            }
        }
    }
}