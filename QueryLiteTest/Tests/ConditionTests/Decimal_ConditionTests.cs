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
    public sealed class Decimal_ConditionTests {

        [TestInitialize]
        public void ClearTable() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transation = new Transaction(TestDatabase.Database)) {

                Query.Delete(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transation);

                COUNT_ALL count = new COUNT_ALL();

                QueryResult<int> result = Query
                    .Select(
                        result => result.Get(count)
                    )
                    .From(allTypesTable)
                    .Execute(transation);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue, 0);

                transation.Commit();
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

            types1.Decimal = 1;
            types2.Decimal = 2;
            types3.Decimal = 3;

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
                    .Where(table.Decimal.In(new List<decimal>() { types1.Decimal, types2.Decimal, types3.Decimal }))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.In(new List<decimal>() { types1.Decimal, types2.Decimal }))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.In(new List<decimal>() { types2.Decimal }))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.NotIn(new List<decimal>() { types1.Decimal, types2.Decimal, types3.Decimal }))
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal.NotIn(new List<decimal>() { types1.Decimal, types2.Decimal }))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.NotIn(new List<decimal>() { types1.Decimal }))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal == short.MaxValue)
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal == types1.Decimal)
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal != types1.Decimal)
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal < short.MinValue)
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal <= short.MinValue)
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal < types2.Decimal)
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal <= types2.Decimal)
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal > short.MaxValue)
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal >= short.MaxValue)
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal == types1.Decimal)
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal >= types2.Decimal)
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.SqlEquals_NonTypeSafe(int.MaxValue))
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal.SqlEquals_NonTypeSafe(types1.Decimal))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.SqlNotEquals_NonTypeSafe(types1.Decimal))
                    .OrderBy(table.Decimal.ASC)
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
                    .Where(table.Decimal.IsNull)
                    .OrderBy(table.Decimal.ASC)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
            }

            {
                QueryResult<AllTypesInfo> result = await Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .Where(table.Decimal.IsNotNull)
                    .OrderBy(table.Decimal.ASC)
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
                        table.Decimal.In(
                            Query.NestedSelect(table2.Decimal)
                                .From(table2)
                                .Where(table2.Decimal == types1.Decimal)
                        )
                    )
                    .OrderBy(table.Decimal.ASC)
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
                        table.Decimal.In(
                            Query.NestedSelect(table2.Decimal)
                                .From(table2)
                                .Where(table2.Decimal.In(new List<decimal>() { types2.Decimal, types3.Decimal }))
                        )
                    )
                    .OrderBy(table.Decimal.ASC)
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
                        table.Decimal.NotIn(
                            Query.NestedSelect(table2.Decimal)
                                .From(table2)
                        )
                    )
                    .OrderBy(table.Decimal.ASC)
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
                        table.Decimal.NotIn(
                            Query.NestedSelect(table2.Decimal)
                                .From(table2)
                                .Where(table2.Decimal == types1.Decimal)
                        )
                    )
                    .OrderBy(table.Decimal.ASC)
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