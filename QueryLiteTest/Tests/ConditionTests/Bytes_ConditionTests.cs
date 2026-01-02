using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests.ConditionTests {

    [TestClass]
    public sealed class Bytes_ConditionTests {

        [TestInitialize]
        public void ClearTable() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transaction);

                COUNT_ALL count = COUNT_ALL.Instance;

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

            await Bytes_ConditionTests.TestConditions_Async();
        }

        [TestMethod]
        public async Task InCondition_NoParameters_Async() {

            Settings.UseParameters = false;

            await Bytes_ConditionTests.TestConditions_Async();
        }

        public static async Task TestConditions_Async() {

            AllTypes types1 = GetAllType();
            AllTypes types2 = GetAllType();
            AllTypes types3 = GetAllType();

            types1.Bytes = [1, 2, 3, 4, 5];
            types2.Bytes = [2, 7, 9, 0, 7];
            types3.Bytes = [3, 4, 9, 1, 1];

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
                    .Where(table.Bytes.In(types1.Bytes, types2.Bytes, types3.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.In(types1.Bytes, types2.Bytes, types3.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.In(types1.Bytes, types2.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.In(types2.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.NotIn(types1.Bytes, types2.Bytes, types3.Bytes))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes.NotIn(types1.Bytes, types2.Bytes, types3.Bytes))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                if(!Sequence<byte[]>.TryCreateFrom([types1.Bytes, types2.Bytes], out Sequence<byte[]>? bytes)) {
                    throw new Exception();
                }

                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes.NotIn(bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.NotIn(types1.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes == new byte[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 })
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes == types1.Bytes)
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes != types1.Bytes)
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes < new byte[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 })
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes <= new byte[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 })
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes < types2.Bytes)
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes <= types2.Bytes)
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes > new byte[] { 9, 1, 2, 2, 3, 3, 4, 4, 5, 5 })
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes >= new byte[] { 9, 1, 2, 2, 3, 3, 4, 4, 5, 5 })
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes == types1.Bytes)
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes >= types2.Bytes)
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.SqlEquals_NonTypeSafe(new byte[] { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5 }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes.SqlEquals_NonTypeSafe(types1.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.SqlNotEquals_NonTypeSafe(types1.Bytes))
                    .OrderBy(table.Id.ASC)
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
                    .Where(table.Bytes.IsNull)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Bytes.IsNotNull)
                    .OrderBy(table.Id.ASC)
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
                        table.Bytes.In(
                            Query.NestedSelect(table2.Bytes)
                                .From(table2)
                                .Where(table2.Bytes == types1.Bytes)
                        )
                    )
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {

                if(!Sequence<byte[]>.TryCreateFrom([types2.Bytes, types3.Bytes], out Sequence<byte[]>? bytes)) {
                    throw new Exception();
                }

                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(
                        table.Bytes.In(
                            Query.NestedSelect(table2.Bytes)
                                .From(table2)
                                .Where(table2.Bytes.In(bytes))
                        )
                    )
                    .OrderBy(table.Id.ASC)
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
                        table.Id.NotIn(
                            Query.NestedSelect(table2.Id)
                                .From(table2)
                        )
                    )
                    .OrderBy(table.Id.ASC)
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
                        table.Bytes.NotIn(
                            Query.NestedSelect(table2.Bytes)
                                .From(table2)
                                .Where(table2.Bytes == types1.Bytes)
                        )
                    )
                    .OrderBy(table.Id.ASC)
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