using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Functions;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Collections.Generic;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class MultiRowTest {

        [TestInitialize]
        public void ClearTable() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                Count count = new();

                var result = Query
                    .Select(
                        result => new {
                            Count = result.Get(count)
                        }
                    )
                    .From(allTypesTable)
                    .Execute(transaction);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0].Count;

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue);

                transaction.Commit();
            }
        }

        [TestCleanup]
        public void CleanUp() {
            Settings.UseParameters = false;
        }

        [TestMethod]
        public void InsertManyRows() {

            AllTypesTable table = AllTypesTable.Instance;

            const short records = 1000;

            List<AllTypes> list = new List<AllTypes>();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                for(short index = 0; index < records; index++) {

                    AllTypes allTypes = GetAllTypes1(index);

                    list.Add(allTypes);

                    var result = Query.Insert(table)
                        .Values(values => values
                            .Set(table.Guid, allTypes.Guid)
                            .Set(table.String, allTypes.String)
                            .Set(table.SmallInt, allTypes.SmallInt)
                            .Set(table.Int, allTypes.Int)
                            .Set(table.BigInt, allTypes.BigInt)
                            .Set(table.Decimal, allTypes.Decimal)
                            .Set(table.Float, allTypes.Float)
                            .Set(table.Double, allTypes.Double)
                            .Set(table.Boolean, allTypes.Boolean)
                            .Set(table.Bytes, allTypes.Bytes)
                            .Set(table.DateTime, allTypes.DateTime)
                            .Set(table.DateTimeOffset, allTypes.DateTimeOffset)
                            .Set(table.Enum, allTypes.Enum)
                            .Set(table.DateOnly, allTypes.DateOnly)
                            .Set(table.TimeOnly, allTypes.TimeOnly)
                        )
                        .Execute(
                            result => new {
                                Id = result.Get(table.Id)
                            },
                            transaction,
                            TimeoutLevel.ShortInsert
                        );

                    Assert.AreEqual(1, result.Rows.Count);
                    Assert.AreEqual(1, result.RowsEffected);

                    allTypes.Id = result.Rows[0].Id;
                }
                transaction.Commit();
            }

            {
                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, table)
                        }
                    )
                    .From(table)
                    .OrderBy(table.Id.ASC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(records, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                for(short index = 0; index < result.Rows.Count; index++) {

                    AllTypesInfo row = result.Rows[index].AllTypesRow;

                    AssertRow(row, list[index]);
                }
            }

            {   //Test order by
                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, table)
                        }
                    )
                    .From(table)
                    .OrderBy(table.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(records, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);


                for(short index = 0, allTypesIndex = records - 1; index < result.Rows.Count; index++, allTypesIndex--) {
                    AssertRow(result.Rows[index].AllTypesRow, list[allTypesIndex]);
                }
            }

            {

                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, table)
                        }
                    )
                    .From(table)
                    .UnionSelect(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, table)
                        }
                    )
                    .From(table)
                    .OrderBy(table.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(records, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                for(short index = 0, allTypesIndex = records - 1; index < result.Rows.Count; index++, allTypesIndex--) {
                    AssertRow(result.Rows[index].AllTypesRow, list[allTypesIndex]);
                }
            }

            {

                AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, table)
                        }
                    )
                    .From(table)
                    .Where(
                        table.Id.In(
                            Query.NestedSelect(allTypesTable2.Id)
                            .From(allTypesTable2)
                        )
                    )
                    .OrderBy(table.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(records, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                for(short index = 0, allTypesIndex = records - 1; index < result.Rows.Count; index++, allTypesIndex--) {
                    AssertRow(result.Rows[index].AllTypesRow, list[allTypesIndex]);
                }
            }

            {

                AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, table)
                        }
                    )
                    .From(table)
                    .Where(
                        table.Id.NotIn(
                            Query.NestedSelect(allTypesTable2.Id)
                            .From(allTypesTable2)
                        )
                    )
                    .OrderBy(table.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(0, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);
            }
        }

        private static AllTypes GetAllTypes1(short index) {
            return new AllTypes(
                id: AllTypesId.NotSet,
                guid: Guid.NewGuid(),
                @string: Guid.NewGuid().ToString(),
                smallInt: (short)(7261 + index),
                @int: 846218432 + index,
                bigInt: 94377682378523423 + index,
                @decimal: 743.534234m + index,
                @float: 7324.2521342f + index,
                @double: 93234.487213123d + index,
                boolean: index % 2 == 0,
                bytes: BitConverter.GetBytes(index),
                dateTime: new DateTime(year: 1800 + index, month: 12, day: 01, hour: 23, minute: 59, second: 59),
                dateTimeOffset: new DateTimeOffset(year: 1800 + index, month: 11, day: 02, hour: 20, minute: 55, second: 57, new TimeSpan(hours: 5, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.A,
                dateOnly: new DateOnly(year: 1925, month: 12, day: 21),
                timeOnly: new TimeOnly(hour: 23, minute: 57, second: 0, millisecond: 1, microsecond: 777)
            );
        }
        private static void AssertRow(AllTypesInfo row, AllTypes allTypes) {

            Assert.AreEqual(row.Id, allTypes.Id);
            Assert.AreEqual(row.Guid, allTypes.Guid);
            Assert.AreEqual(row.String, allTypes.String);
            Assert.AreEqual(row.SmallInt, allTypes.SmallInt);
            Assert.AreEqual(row.Int, allTypes.Int);
            Assert.AreEqual(row.BigInt, allTypes.BigInt);
            Assert.AreEqual(row.Decimal, allTypes.Decimal);
            Assert.AreEqual(row.Float, allTypes.Float);
            Assert.AreEqual(row.Double, allTypes.Double);
            Assert.AreEqual(row.Boolean, allTypes.Boolean);
            Assert.AreEqual(row.Bytes.Length, allTypes.Bytes.Length);

            for(int index = 0; index < row.Bytes.Length; index++) {
                Assert.AreEqual(row.Bytes[index], allTypes.Bytes[index]);
            }
            Assert.AreEqual(row.DateTime, allTypes.DateTime);
            Assert.AreEqual(row.DateTimeOffset, allTypes.DateTimeOffset);
            Assert.AreEqual(row.Enum, allTypes.Enum);
            Assert.AreEqual(row.DateOnly, allTypes.DateOnly);
            Assert.AreEqual(row.TimeOnly, allTypes.TimeOnly);
        }
    }
}