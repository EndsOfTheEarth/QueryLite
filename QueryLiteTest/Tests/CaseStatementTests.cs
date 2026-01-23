using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.Functions;
using QueryLite.Functions;
using QueryLiteTest.Tables;
using System;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class CaseStatementTests {

        [TestInitialize]
        public void ClearTable() {

            ParentTable table = ParentTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(table)
                    .NoWhereCondition()
                    .Execute(transaction);

                Count count = new();

                QueryResult<int> result = Query
                    .Select(
                        row => row.Get(count)
                    )
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
        public void ExistsTest01_WithoutParameters() {
            Settings.UseParameters = false;
            ExistsTest01();
        }
        [TestMethod]
        public void ExistsTest01_WithParameters() {
            Settings.UseParameters = true;
            ExistsTest01();
        }

        private static void ExistsTest01() {

            ParentTable table = ParentTable.Instance;

            {   //Test EXISTS() when no records exist
                Case<string> caseStatement = Case<string>
                    .When(new Exists(Query.Select(row => row.Get(table.Id)).From(table))).Then("abc")
                    .Else("xyz")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("xyz", result.Rows[0]);
            }

            {   //Test NOT EXISTS() when no records exist
                Case<string> caseStatement = Case<string>
                    .When(new NotExists(Query.Select(row => row.Get(table.Id)).From(table))).Then("123")
                    .Else("456")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("123", result.Rows[0]);
            }
        }

        [TestMethod]
        public void ExistsTest02_WithoutParameters() {
            Settings.UseParameters = false;
            ExistsTest02();
        }
        [TestMethod]
        public void ExistsTest02_WithParameters() {
            Settings.UseParameters = true;
            ExistsTest02();
        }
        
        private static void ExistsTest02() {

            ParentTable table = ParentTable.Instance;

            {   //Insert record

                using Transaction transaction = new(TestDatabase.Database);

                Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, ParentId.ValueOf(Guid.NewGuid()))
                        .Set(table.Id2, ParentId.ValueOf(Guid.NewGuid()))
                    )
                    .Execute(transaction);

                transaction.Commit();
            }
            {   //Test EXISTS() when no records exist
                Case<string> caseStatement = Case<string>
                    .When(new Exists(Query.Select(row => row.Get(table.Id)).From(table))).Then("abc")
                    .Else("xyz")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("abc", result.Rows[0]);
            }

            {   //Test NOT EXISTS() when no records exist
                Case<string> caseStatement = Case<string>
                    .When(new NotExists(Query.Select(row => row.Get(table.Id)).From(table))).Then("123")
                    .Else("456")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("456", result.Rows[0]);
            }
        }

        [TestMethod]
        public void InsertTest01_WithParameters() {
            Settings.UseParameters = true;
            InsertTest01();
        }
        [TestMethod]
        public void InsertTest01_WithoutParameters() {
            Settings.UseParameters = false;
            InsertTest01();
        }
            
        private static void InsertTest01() {

            ParentTable table = ParentTable.Instance;

            ParentId id1 = ParentId.ValueOf(Guid.NewGuid());

            {   //Insert record

                using Transaction transaction = new(TestDatabase.Database);

                Case<ParentId> caseStatement = Case<ParentId>
                          .When(new NotExists(Query.Select(row => row.Get(table.Id)).From(table))).Then(id1)    //If there are no rows in the table use id1
                          .Else(ParentId.ValueOf(Guid.NewGuid()))
                          .End();

                NonQueryResult insertResult = Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, caseStatement)
                        .Set(table.Id2, ParentId.ValueOf(Guid.NewGuid()))
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            {

                QueryResult<ParentId> result = Query
                    .Select(row => row.Get(table.Id))
                    .From(table)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(id1, result.Rows[0]);
            }
        }

        [TestMethod]
        public void InsertTest02_WithParameters() {
            Settings.UseParameters = true;
            InsertTest02();
        }
        [TestMethod]
        public void InsertTest02_WithoutParameters() {
            Settings.UseParameters = false;
            InsertTest02();
        }
        
        private static void InsertTest02() {

            ParentTable table = ParentTable.Instance;

            ParentId id1 = ParentId.ValueOf(Guid.NewGuid());
            ParentId id2 = ParentId.ValueOf(Guid.NewGuid());

            {   //Insert record

                using Transaction transaction = new(TestDatabase.Database);

                Case<ParentId> caseStatement = Case<ParentId>
                          .When(new Exists(Query.Select(row => row.Get(table.Id)).From(table))).Then(id1)    //If there are not rows in the table use id2
                          .Else(id2)
                          .End();

                NonQueryResult insertResult = Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, caseStatement)
                        .Set(table.Id2, ParentId.ValueOf(Guid.NewGuid()))
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            {

                QueryResult<ParentId> result = Query
                    .Select(row => row.Get(table.Id))
                    .From(table)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(id2, result.Rows[0]);
            }
        }

        [TestMethod]
        public void SimpleCaseStatementTest01_WithoutParameters() {
            Settings.UseParameters = false;
            SimpleCaseStatementTest01();
        }
        [TestMethod]
        public void SimpleCaseStatementTest01_WithParameters() {
            Settings.UseParameters = true;
            SimpleCaseStatementTest01();
        }

        private static void SimpleCaseStatementTest01() {

            ParentTable table = ParentTable.Instance;

            ParentId id1 = ParentId.ValueOf(Guid.NewGuid());
            ParentId id2 = ParentId.ValueOf(Guid.NewGuid());

            {   //Insert record

                using Transaction transaction = new(TestDatabase.Database);

                Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, id1)
                        .Set(table.Id2, id2)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }
            {   //Test nested query in a simple case statement
                Case<string> caseStatement = Case<string>
                    .Simple(Query.Select(row => row.Get(table.Id)).From(table).Where(table.Id == id1))
                    .WhenValue(id1).Then("A")
                    .WhenValue(id2).Then("B")
                    .Else("C")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("A", result.Rows[0]);
            }

            {   //Test nested query in a simple case statement
                Case<string> caseStatement = Case<string>
                    .Simple(Query.Select(row => row.Get(table.Id2)).From(table).Where(table.Id2 == id2))
                    .WhenValue(id1).Then("A")
                    .WhenValue(id2).Then("B")
                    .Else("C")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("B", result.Rows[0]);
            }
            {   //Test nested query in a simple case statement
                Case<string> caseStatement = Case<string>
                    .Simple(Query.Select(row => row.Get(table.Id)).From(table).Where(table.Id == ParentId.ValueOf(Guid.NewGuid())))
                    .WhenValue(id1).Then("A")
                    .WhenValue(id2).Then("B")
                    .Else("C")
                    .End();

                QueryResult<string> result = Query
                    .Select(row => row.Get(caseStatement))
                    .NoFromClause()
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("C", result.Rows[0]);
            }
        }

        [TestMethod]
        public void SimpleCaseStatementTest02_WithoutParameters() {
            Settings.UseParameters = false;
            SimpleCaseStatementTest02();
        }
        [TestMethod]
        public void SimpleCaseStatementTest02_WithParameters() {
            Settings.UseParameters = true;
            SimpleCaseStatementTest02();
        }
        
        private static void SimpleCaseStatementTest02() {

            /*
             * Test table aliasing
             */
            ParentTable table = ParentTable.Instance;
            ParentTable table2 = ParentTable.Instance2;

            ParentId id1 = ParentId.ValueOf(Guid.NewGuid());
            ParentId id2 = ParentId.ValueOf(Guid.NewGuid());

            {   //Insert record

                using Transaction transaction = new(TestDatabase.Database);

                Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, id1)
                        .Set(table.Id2, id2)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }
            {   //Test nested query in a simple case statement
                Case<string> caseStatement = Case<string>
                    .Simple(Query.Select(row => row.Get(table.Id)).From(table).Where(table.Id == id1))
                    .WhenValue(id1).Then("A")
                    .WhenValue(id2).Then("B")
                    .Else("C")
                    .End();

                var result = Query
                    .Select(
                        row => new {
                            Case = row.Get(caseStatement),
                            Id1 = row.Get(table.Id),
                            Id2 = row.Get(table.Id2)
                        }
                    )
                    .From(table)
                    .Join(table2).On(table.Id == table2.Id) //Trigger an alias with two tables
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("A", result.Rows[0].Case);
            }

            {   //Test nested query in a simple case statement
                Case<string> caseStatement = Case<string>
                    .Simple(Query.Select(row => row.Get(table.Id2)).From(table).Where(table.Id2 == id2))
                    .WhenValue(id1).Then("A")
                    .WhenValue(id2).Then("B")
                    .Else("C")
                    .End();

                var result = Query
                    .Select(
                        row => new {
                            Case = row.Get(caseStatement),
                            Id1 = row.Get(table.Id),
                            Id2 = row.Get(table.Id2)
                        }
                    )
                    .From(table)
                    .Join(table2).On(table.Id == table2.Id) //Trigger an alias with two tables
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("B", result.Rows[0].Case);
            }
            {   //Test nested query in a simple case statement
                Case<string> caseStatement = Case<string>
                    .Simple(Query.Select(row => row.Get(table.Id)).From(table).Where(table.Id == ParentId.ValueOf(Guid.NewGuid())))
                    .WhenValue(id1).Then("A")
                    .WhenValue(id2).Then("B")
                    .Else("C")
                    .End();

                var result = Query
                    .Select(
                        row => new {
                            Case = row.Get(caseStatement),
                            Id1 = row.Get(table.Id),
                            Id2 = row.Get(table.Id2)
                        }
                    )
                    .From(table)
                    .Join(table2).On(table.Id == table2.Id) //Trigger an alias with two tables
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual("C", result.Rows[0].Case);
            }
        }
    }
}