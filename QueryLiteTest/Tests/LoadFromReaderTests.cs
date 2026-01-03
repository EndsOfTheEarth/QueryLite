using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLiteTest.Tables;
using System;

namespace QueryLiteTest.Tests {

    [TestClass]
    public class LoadFromReaderTests {

        [TestInitialize]
        public void ClearTable() {

            ParentTable table = ParentTable.Instance;

            using Transaction transaction = new(TestDatabase.Database);

            Query.Delete(table).NoWhereCondition().Execute(transaction);

            transaction.Commit();
        }

        [TestMethod]
        public void TestLoadFromReaderUsingGuidKey() {

            ParentTable table = ParentTable.Instance;

            using Transaction transaction = new(TestDatabase.Database);

            const int rows = 10;

            for(int index = 0; index < rows; index++) {

                Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, ParentId.ValueOf(Guid.NewGuid()))
                        .Set(table.Id2, ParentId.ValueOf(Guid.NewGuid()))
                    )
                    .Execute(transaction);
            }
            transaction.Commit();

            var result = Query.Select(
                row => new {
                    Id = row.LoadFromReader(table.Id, (reader, ordinal) => ParentId.ValueOf(reader.GetGuid(ordinal)), ParentId.NotSet),
                    Id2 = row.LoadFromReader(table.Id2, (reader, ordinal) => ParentId.ValueOf(reader.GetGuid(ordinal)), ParentId.NotSet),
                }
            )
            .From(table)
            .Execute(TestDatabase.Database);

            Assert.AreEqual(rows, result.Rows.Count);

            foreach(var row in result.Rows) {
                Assert.IsTrue(row.Id.Value != Guid.Empty);
            }
        }

        [TestMethod]
        public void TestLoadFromReaderCustomSqlFunction() {

            RawSqlFunction<string> concat = new(sql: "CONCAT('abc', 'efg')");

            QueryResult<string> result = Query.Select(
                row => row.LoadFromReader(concat, (reader, ordinal) => reader.GetString(ordinal), @default: "")
            )
            .NoFromClause()
            .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            foreach(string text in result.Rows) {
                Assert.AreEqual("abcefg", text);
            }
        }
    }
}