﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests.ConditionTests {

    [TestClass]
    public sealed class IntegerKey_ConditionTests {

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

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int countValue = result.Rows[0];

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
        public async Task TestConditions_Parameters_Async() {

            Settings.UseParameters = true;

            await TestConditions_Async();
        }

        [TestMethod]
        public async Task InCondition_NoParameters_Async() {

            Settings.UseParameters = false;

            await TestConditions_Async();
        }

        public async Task TestConditions_Async() {

            AllTypes types1 = GetAllType();
            AllTypes types2 = GetAllType();
            AllTypes types3 = GetAllType();

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
                    .Where(table.Id.In(new List<IntKey<AllTypes>>() { types1.Id, types2.Id, types3.Id }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 3);

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
                    .Where(table.Id.In(new List<IntKey<AllTypes>>() { types1.Id, types2.Id }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
                AllFieldsTest.AssertRow(result.Rows[1], types2);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.In(new List<IntKey<AllTypes>>() { types2.Id }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.NotIn(new List<IntKey<AllTypes>>() { types1.Id, types2.Id, types3.Id }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.NotIn(new List<IntKey<AllTypes>>() { types1.Id, types2.Id }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types3);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.NotIn(new List<IntKey<AllTypes>>() { types1.Id }))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

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
                    .Where(table.Id == IntKey<AllTypes>.ValueOf(int.MaxValue))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id == types1.Id)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id != types1.Id)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

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
                    .Where(table.Id < IntKey<AllTypes>.ValueOf(int.MinValue))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id <= IntKey<AllTypes>.ValueOf(int.MinValue))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id < types2.Id)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id <= types2.Id)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

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
                    .Where(table.Id > IntKey<AllTypes>.ValueOf(int.MaxValue))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id >= IntKey<AllTypes>.ValueOf(int.MaxValue))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id == types1.Id)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id >= types2.Id)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

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
                    .Where(table.Id.SqlEquals_NonTypeSafe(IntKey<AllTypes>.ValueOf(int.MaxValue)))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.SqlEquals_NonTypeSafe(types1.Id))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.SqlNotEquals_NonTypeSafe(types1.Id))
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

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
                    .Where(table.Id.IsNull)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Id.IsNotNull)
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 3);

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
                        table.Id.In(
                            Query.NestedSelect(table2.Id)
                                .From(table2)
                                .Where(table2.Id == types1.Id)
                        )
                    )
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);

                AllFieldsTest.AssertRow(result.Rows[0], types1);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(
                        table.Id.In(
                            Query.NestedSelect(table2.Id)
                                .From(table2)
                                .Where(table2.Id.In(new List<IntKey<AllTypes>>() { types2.Id, types3.Id }))
                        )
                    )
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

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

                Assert.AreEqual(result.Rows.Count, 0);
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
                                .Where(table2.Id == types1.Id)
                        )
                    )
                    .OrderBy(table.Id.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 2);

                AllFieldsTest.AssertRow(result.Rows[0], types2);
                AllFieldsTest.AssertRow(result.Rows[1], types3);
            }
        }

        private static AllTypes GetAllType() {

            return new AllTypes(
                id: IntKey<AllTypes>.NotSet,
                guid: Guid.NewGuid(),
                @string: "88udskja8adfq23",
                smallInt: 7261,
                @int: 846218432,
                bigInt: 94377682378523423,
                @decimal: 743.534234m,
                @float: 7324.2521342f,
                @double: 93234.487213123d,
                boolean: true,
                bytes: new byte[] { 5, 43, 23, 7, 8 },
                dateTime: new DateTime(year: 2021, month: 12, day: 01, hour: 23, minute: 59, second: 59),
                dateTimeOffset: new DateTimeOffset(year: 2022, month: 11, day: 02, hour: 20, minute: 55, second: 57, new TimeSpan(hours: 5, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.A,
                dateOnly: new DateOnly(year: 2005, month: 11, day: 1),
                timeOnly: new TimeOnly(hour: 9, minute: 59, second: 1, millisecond: 770, microsecond: 1)
            );
        }
    }
}