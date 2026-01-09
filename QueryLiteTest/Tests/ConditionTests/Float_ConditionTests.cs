using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Functions;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests.ConditionTests {

    [TestClass]
    public sealed class Float_ConditionTests {

        [TestInitialize]
        public void ClearTable() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transaction);

                Count count = new();

                QueryResult<int> result = Query
                    .Select(
                        result => result.Get(count)
                    )
                    .From(allTypesTable)
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
        public async Task TestConditions_Parameters_Async() {

            Settings.UseParameters = true;

            await Float_ConditionTests.TestConditions_Async();
        }

        [TestMethod]
        public async Task InCondition_NoParameters_Async() {

            Settings.UseParameters = false;

            await Float_ConditionTests.TestConditions_Async();
        }

        public static async Task TestConditions_Async() {

            AllTypes types1 = GetAllType();
            AllTypes types2 = GetAllType();
            AllTypes types3 = GetAllType();

            types1.Float = 1;
            types2.Float = 2;
            types3.Float = 3;

            await AllFieldsTest.InsertWithQueryAsync(types1);
            await AllFieldsTest.InsertWithQueryAsync(types2);
            await AllFieldsTest.InsertWithQueryAsync(types3);

            AllTypesTable table = AllTypesTable.Instance;

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.In(types1.Float, types2.Float, types3.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(3, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
                AllFieldsTest.AssertRow(result.Rows[1], types2);
                AllFieldsTest.AssertRow(result.Rows[2], types3);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.In(types1.Float, types2.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
                AllFieldsTest.AssertRow(result.Rows[1], types2);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.In(types2.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.NotIn(types1.Float, types2.Float, types3.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.NotIn(types1.Float, types2.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types3);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.NotIn(types1.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }

            //
            //  Equals and not equals operator tests
            //
            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float == short.MaxValue)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float == types1.Float)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float != types1.Float)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }

            //
            //  Less than operator tests
            //
            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float < short.MinValue)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float <= short.MinValue)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float < types2.Float)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float <= types2.Float)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
                AllFieldsTest.AssertRow(result.Rows[1], types2);
            }

            //
            //  Greater than operator tests
            //
            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float > short.MaxValue)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float >= short.MaxValue)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float == types1.Float)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float >= types2.Float)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }

            //
            //  Non types safe equals and not equals operator tests
            //
            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.SqlEquals_NonTypeSafe(int.MaxValue))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.SqlEquals_NonTypeSafe(types1.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.SqlNotEquals_NonTypeSafe(types1.Float))
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }

            //
            // IS NULL and IS NOT NULL operator tests
            //
            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.IsNull)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Float.IsNotNull)
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(3, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
                AllFieldsTest.AssertRow(result.Rows[1], types2);
                AllFieldsTest.AssertRow(result.Rows[2], types3);
            }




            AllTypesTable table2 = AllTypesTable.Instance2;

            /*
             * Test nested query conditions
             **/
            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(
                        table.Float.In(
                            Query.NestedSelect(table2.Float)
                                .From(table2)
                                .Where(table2.Float == types1.Float)
                        )
                    )
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(
                        table.Float.In(
                            Query.NestedSelect(table2.Float)
                                .From(table2)
                                .Where(table2.Float.In(types2.Float, types3.Float))
                        )
                    )
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(
                        table.Float.NotIn(
                            Query.NestedSelect(table2.Float)
                                .From(table2)
                        )
                    )
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(
                        table.Float.NotIn(
                            Query.NestedSelect(table2.Float)
                                .From(table2)
                                .Where(table2.Float == types1.Float)
                        )
                    )
                    .OrderBy(table.Float.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(2, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }
        }

        private static AllTypes GetAllType() {

            return new AllTypes(
                id: AllTypesId.NotSet,
                guid: Guid.NewGuid(),
                @string: "88udskja8adfq23",
                smallInt: 7261,
                @int: 846218432,
                bigInt: 94377682378523423,
                @decimal: 743.534234m,
                @float: 7324.2521342f,
                @double: 93234.487213123d,
                boolean: true,
                bytes: [5, 43, 23, 7, 8],
                dateTime: new DateTime(year: 2021, month: 12, day: 01, hour: 23, minute: 59, second: 59),
                dateTimeOffset: new DateTimeOffset(year: 2022, month: 11, day: 02, hour: 20, minute: 55, second: 57, new TimeSpan(hours: 5, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.A,
                dateOnly: new DateOnly(year: 2005, month: 11, day: 1),
                timeOnly: new TimeOnly(hour: 9, minute: 59, second: 1, millisecond: 770, microsecond: 1)
            );
        }
    }
}