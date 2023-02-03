using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
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

            using(Transaction transation = new Transaction(TestDatabase.Database)) {

                Query.DeleteFrom(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transation, TimeoutLevel.ShortDelete);

                CountAll count = new CountAll();

                var result = Query
                    .Select(
                        result => new {
                            Count = result.Get(count)
                        }
                    )
                    .From(allTypesTable)
                    .Execute(transation);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0].Count;

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
        public void InsertManyRows() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            const short records = 1000;

            List<AllTypes> list = new List<AllTypes>();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                for(short index = 0; index < records; index++) {

                    AllTypes allTypes = GetAllTypes1(index);

                    list.Add(allTypes);

                    var result = Query.InsertInto(allTypesTable)
                        .Set(allTypesTable.Guid, allTypes.Guid)
                        .Set(allTypesTable.String, allTypes.String)
                        .Set(allTypesTable.SmallInt, allTypes.SmallInt)
                        .Set(allTypesTable.Int, allTypes.Int)
                        .Set(allTypesTable.BigInt, allTypes.BigInt)
                        .Set(allTypesTable.Decimal, allTypes.Decimal)
                        .Set(allTypesTable.Float, allTypes.Float)
                        .Set(allTypesTable.Double, allTypes.Double)
                        .Set(allTypesTable.Boolean, allTypes.Boolean)
                        .Set(allTypesTable.Bytes, allTypes.Bytes)
                        .Set(allTypesTable.DateTime, allTypes.DateTime)
                        .Set(allTypesTable.DateTimeOffset, allTypes.DateTimeOffset)
                        .Set(allTypesTable.Enum, allTypes.Enum)
                        .Set(allTypesTable.DateOnly, allTypes.DateOnly)
                        .Execute(
                            result => new {
                                Id = result.Get(allTypesTable.Id)
                            },
                            transaction,
                            TimeoutLevel.ShortInsert
                        );

                    Assert.AreEqual(result.Rows.Count, 1);
                    Assert.AreEqual(result.RowsEffected, 1);

                    allTypes.Id = result.Rows[0].Id;
                }
                transaction.Commit();
            }

            {
                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, allTypesTable)
                        }
                    )
                    .From(allTypesTable)
                    .OrderBy(allTypesTable.Id.ASC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, records);
                Assert.AreEqual(result.RowsEffected, 0);

                for(short index = 0; index < result.Rows.Count; index++) {

                    AllTypesInfo row = result.Rows[index].AllTypesRow;

                    AssertRow(row, list[index]);
                }
            }

            {   //Test order by
                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, allTypesTable)
                        }
                    )
                    .From(allTypesTable)
                    .OrderBy(allTypesTable.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, records);
                Assert.AreEqual(result.RowsEffected, 0);


                for(short index = 0, allTypesIndex = records - 1; index < result.Rows.Count; index++, allTypesIndex--) {
                    AssertRow(result.Rows[index].AllTypesRow, list[allTypesIndex]);
                }
            }

            {

                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, allTypesTable)
                        }
                    )
                    .From(allTypesTable)
                    .UnionSelect(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, allTypesTable)
                        }
                    )
                    .From(allTypesTable)
                    .OrderBy(allTypesTable.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, records);
                Assert.AreEqual(result.RowsEffected, 0);

                for(short index = 0, allTypesIndex = records - 1; index < result.Rows.Count; index++, allTypesIndex--) {
                    AssertRow(result.Rows[index].AllTypesRow, list[allTypesIndex]);
                }
            }

            {

                AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, allTypesTable)
                        }
                    )
                    .From(allTypesTable)
                    .Where(
                        allTypesTable.Id.In(
                            Query.NestedSelect(allTypesTable2.Id)
                            .From(allTypesTable2)
                        )
                    )
                    .OrderBy(allTypesTable.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, records);
                Assert.AreEqual(result.RowsEffected, 0);

                for(short index = 0, allTypesIndex = records - 1; index < result.Rows.Count; index++, allTypesIndex--) {
                    AssertRow(result.Rows[index].AllTypesRow, list[allTypesIndex]);
                }
            }

            {

                AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

                var result = Query
                    .Select(
                        result => new {
                            AllTypesRow = new AllTypesInfo(result, allTypesTable)
                        }
                    )
                    .From(allTypesTable)
                    .Where(
                        allTypesTable.Id.NotIn(
                            Query.NestedSelect(allTypesTable2.Id)
                            .From(allTypesTable2)
                        )
                    )
                    .OrderBy(allTypesTable.Id.DESC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 0);
                Assert.AreEqual(result.RowsEffected, 0);
            }
        }

        private AllTypes GetAllTypes1(short index) {
            return new AllTypes(
                id: IntKey<AllTypes>.NotSet,
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
                dateOnly: new DateOnly(year: 1925, month: 12, day: 21)
            );
        }
        private void AssertRow(AllTypesInfo row, AllTypes allTypes) {

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
        }
    }
}