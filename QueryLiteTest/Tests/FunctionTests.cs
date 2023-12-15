using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.Functions;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Threading;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class FunctionTests {

        [TestInitialize]
        public void ClearTable() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                COUNT_ALL count = COUNT_ALL.Instance;

                var result = Query
                    .Select(
                        result => new {
                            Count = result.Get(count)
                        }
                    )
                    .From(allTypesTable)
                    .Execute(transaction);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0].Count;

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
        public void InsertWithSetFunctions_NoParameters() {

            Settings.UseParameters = false;
            InsertWithSetFunctions();
        }

        [TestMethod]
        public void InsertWithSetFunctions_Parameters() {

            Settings.UseParameters = true;
            InsertWithSetFunctions();
        }

        private void InsertWithSetFunctions() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            AllTypesTable table = AllTypesTable.Instance;

            AllTypes info = GetAllTypes1();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<IntKey<AllTypes>> result = Query.Insert(table)
                    .Values(values => values
                        .Set(table.Guid, NEWID.Instance)
                        .Set(table.String, info.String)
                        .Set(table.SmallInt, info.SmallInt)
                        .Set(table.Int, info.Int)
                        .Set(table.BigInt, info.BigInt)
                        .Set(table.Decimal, info.Decimal)
                        .Set(table.Float, info.Float)
                        .Set(table.Double, info.Double)
                        .Set(table.Boolean, info.Boolean)
                        .Set(table.Bytes, info.Bytes)
                        .Set(table.DateTime, GETDATE.Instance)
                        .Set(table.DateTimeOffset, SYSDATETIMEOFFSET.Instance)
                        .Set(table.Enum, info.Enum)
                        .Set(table.DateOnly, info.DateOnly)
                        .Set(table.TimeOnly, info.TimeOnly)
                    )
                    .Execute(
                        result => result.Get(table.Id),
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 1);

                info.Id = result.Rows[0];
                transaction.Commit();
            }

            Guid guid;
            DateTime dateTime;
            DateTimeOffset dateTimeOffset;

            {
                QueryResult<AllTypesInfo> result = Query
                    .Select(
                        row => new AllTypesInfo(row, table)
                    )
                    .From(table)
                    .OrderBy(table.Id.ASC)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                AllTypesInfo row = result.Rows[0];

                guid = row.Guid;
                dateTime = row.DateTime;
                dateTimeOffset = row.DateTimeOffset;

                Assert.AreEqual(row.Id, info.Id);
                //Assert.AreEqual(row.Guid, allTypes.Guid);
                Assert.AreEqual(row.String, info.String);
                Assert.AreEqual(row.SmallInt, info.SmallInt);
                Assert.AreEqual(row.Int, info.Int);
                Assert.AreEqual(row.BigInt, info.BigInt);
                Assert.AreEqual(row.Decimal, info.Decimal);
                Assert.AreEqual(row.Float, info.Float);
                Assert.AreEqual(row.Double, info.Double);
                Assert.AreEqual(row.Boolean, info.Boolean);
                Assert.AreEqual(row.Bytes.Length, info.Bytes.Length);

                for(int index = 0; index < row.Bytes.Length; index++) {
                    Assert.AreEqual(row.Bytes[index], info.Bytes[index]);
                }
                //Assert.AreEqual(row.DateTime, allTypes.DateTime);
                //Assert.AreEqual(row.DateTimeOffset, allTypes.DateTimeOffset);
                Assert.AreEqual(row.Enum, info.Enum);
                Assert.AreEqual(row.DateOnly, info.DateOnly);
                Assert.AreEqual(row.TimeOnly, info.TimeOnly);
            }

            Thread.Sleep(10);  //Add a delay so the date fields are populated with a different value

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                var result = Query.Update(table)
                    .Values(values => values
                        .Set(table.Guid, NEWID.Instance)
                        .Set(table.DateTime, GETDATE.Instance)
                        .Set(table.DateTimeOffset, SYSDATETIMEOFFSET.Instance)
                        .Set(table.Int, SqlMath.Add(table.Int, 1))
                        .Set(table.BigInt, SqlMath.Subtract(table.BigInt, 1))
                    )
                    .Where(table.Id == info.Id)
                    .Execute(
                        result => new {
                            Guid = result.Get(table.Guid),
                            DateTime = result.Get(table.DateTime),
                            DateTimeOffset = result.Get(table.DateTimeOffset),
                            Int = result.Get(table.Int),
                            BigInt = result.Get(table.BigInt)
                        },
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 1);

                var row = result.Rows[0];

                Assert.AreNotEqual(row.Guid, guid);
                Assert.AreNotEqual(row.DateTime, dateTime);
                Assert.AreNotEqual(row.DateTimeOffset, dateTimeOffset);
                Assert.AreEqual(row.Int, info.Int + 1);
                Assert.AreEqual(row.BigInt, info.BigInt - 1);

                guid = row.Guid;
                dateTime = row.DateTime;
                dateTimeOffset = row.DateTimeOffset;

                transaction.Commit();
            }

            Thread.Sleep(10);  //Add a delay so the date fields are populated with a different value

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                var result = Query
                    .Delete(table)
                    .Where(table.Guid == NEWID.Instance | table.DateTime == GETDATE.Instance | table.DateTimeOffset == SYSDATETIMEOFFSET.Instance)
                    .Execute(
                        result => new {
                            Guid = result.Get(table.Guid),
                            DateTime = result.Get(table.DateTime),
                            DateTimeOffset = result.Get(table.DateTimeOffset)
                        },
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                Assert.AreEqual(result.Rows.Count, 0);
                Assert.AreEqual(result.RowsEffected, 0);

                transaction.Commit();
            }

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                var result = Query
                    .Delete(table)
                    .Where(table.Guid != NEWID.Instance)
                    .Execute(
                        result => new {
                            Guid = result.Get(table.Guid),
                            DateTime = result.Get(table.DateTime),
                            DateTimeOffset = result.Get(table.DateTimeOffset)
                        },
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 1);

                var row = result.Rows[0];

                Assert.AreEqual(row.Guid, guid);
                Assert.AreEqual(row.DateTime, dateTime);
                Assert.AreEqual(row.DateTimeOffset, dateTimeOffset);

                transaction.Commit();
            }
        }

        private AllTypes GetAllTypes1() {
            return new AllTypes(
                id: IntKey<AllTypes>.NotSet,
                guid: Guid.NewGuid(),
                @string: Guid.NewGuid().ToString(),
                smallInt: 7261,
                @int: 846218432,
                bigInt: 94377682378523423,
                @decimal: 743.534234m,
                @float: 7324.2521342f,
                @double: 93234.487213123d,
                boolean: true,
                bytes: new byte[] { 1, 2, 3 },
                dateTime: new DateTime(year: 1800, month: 12, day: 01, hour: 23, minute: 59, second: 59),
                dateTimeOffset: new DateTimeOffset(year: 1800, month: 11, day: 02, hour: 20, minute: 55, second: 57, new TimeSpan(hours: 5, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.A,
                dateOnly: new DateOnly(year: 1925, month: 12, day: 21),
                timeOnly: new TimeOnly(hour: 23, minute: 57, second: 0, millisecond: 1, microsecond: 777)
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
            Assert.AreEqual(row.TimeOnly, allTypes.TimeOnly);
        }
    }
}