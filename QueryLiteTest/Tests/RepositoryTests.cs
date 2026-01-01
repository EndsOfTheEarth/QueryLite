using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLiteTest.Tables;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class RepositoryTests {

        [TestInitialize]
        public void ClearTable() {

            CustomTypesTable table = CustomTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(table)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = Query
                    .Select(result => result.Get(count))
                    .From(table)
                    .Execute(transaction);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int countValue = result.Rows[0];

                Assert.AreEqual(0, countValue);

                transaction.Commit();
            }
        }

        [TestMethod]
        public void TestRefCompare() {

            /*
             * Test record equality is by instance rather that value when using RefCompare.
             */
            CustomTypesRow rowA = GetCustomTypesA();

            RefCompare<CustomTypesRow> refA = new RefCompare<CustomTypesRow>(rowA);
            int hashA = refA.GetHashCode();

            Dictionary<RefCompare<CustomTypesRow>, CustomTypesRow> lookup = [];

            lookup.Add(refA, rowA);

            // Change property on record - This will change the Record.Equals(...) return value
            rowA.Guid = CustomGuid.ValueOf(Guid.NewGuid());

            RefCompare<CustomTypesRow> refAA = new RefCompare<CustomTypesRow>(rowA);
            int hashAA = refAA.GetHashCode();

            Assert.AreEqual(hashA, hashAA);
            Assert.AreEqual(refA, refAA);
        }

        [TestMethod]
        public void TestAddUpdateAndDelete() {

            CustomTypesRow rowA = GetCustomTypesA();

            CustomTypesRowRepository repository = new CustomTypesRowRepository();

            { //Test insert
                repository.AddNewRow(rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }
                
                Assert.IsTrue(rowA.Identifier.Value > 0); //Check auto generated column value is loaded after insert

                AssertNumberOfRowsExists(rows: 1);
                AssertCustomTypes(rowA);

                Assert.IsFalse(repository.RequiresUpdate(rowA));
            }

            CustomTypesRow rowB = GetCustomTypesB();

            {   //Test update

                CopyValuesTo(from: rowB, to: rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }
                AssertNumberOfRowsExists(rows: 1);
                AssertCustomTypes(rowA);
                Assert.IsFalse(repository.RequiresUpdate(rowA));
            }

            {   //Test delete

                repository.DeleteRow(rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }
                AssertNumberOfRowsExists(rows: 0);
            }
        }

        [TestMethod]
        public async Task TestAddUpdateAndDeleteAsync() {

            CustomTypesRow rowA = GetCustomTypesA();

            CustomTypesRowRepository repository = new CustomTypesRowRepository();

            { //Test insert
                repository.AddNewRow(rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }

                Assert.IsTrue(rowA.Identifier.Value > 0); //Check auto generated column value is loaded after insert

                AssertNumberOfRowsExists(rows: 1);
                await AssertCustomTypesAsync(rowA);

                Assert.IsFalse(repository.RequiresUpdate(rowA));
            }

            CustomTypesRow rowB = GetCustomTypesB();

            {   //Test update

                CopyValuesTo(from: rowB, to: rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                AssertNumberOfRowsExists(rows: 1);
                await AssertCustomTypesAsync(rowA);
                Assert.IsFalse(repository.RequiresUpdate(rowA));
            }

            {   //Test delete

                repository.DeleteRow(rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                AssertNumberOfRowsExists(rows: 0);
            }
        }

        [TestMethod]
        public void TestAddSelectUpdateAndDelete() {

            CustomTypesRow rowA = GetCustomTypesA();
            CustomTypesRow rowB = GetCustomTypesB();

            Assert.IsTrue(rowA.Int.Value < rowB.Int.Value); //rowA CustomInt value is used for sorting below. So it needs to be less than rowB for test to work.

            CustomTypesRowRepository repository = new CustomTypesRowRepository();

            {   //Test select on empty table
                repository.SelectRows.Execute(TestDatabase.Database);

                Assert.AreEqual(0, repository.Count);
            }

            { //Test insert
                repository.AddNewRow(rowA);
                repository.AddNewRow(rowB);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }

                AssertNumberOfRowsExists(rows: 2);

                AssertCustomTypes(rowA);
                AssertCustomTypes(rowB);

                Assert.IsFalse(repository.RequiresUpdate(rowA));
                Assert.IsFalse(repository.RequiresUpdate(rowB));
            }

            {   //Test select on table with two rows

                repository
                    .SelectRows
                    .OrderBy(repository.Table.Int)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                AssertAreEqual(rowA, loadedRowA);
                AssertAreEqual(rowB, loadedRowB);
            }

            {   //Test update

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                loadedRowA.Guid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowA.Short = CustomShort.ValueOf(44);
                loadedRowA.Int = CustomInt.ValueOf(3384214);
                loadedRowA.Long = CustomLong.ValueOf(435234);
                loadedRowA.String = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowA.Bool = CustomBool.ValueOf(true);
                loadedRowA.Decimal = CustomDecimal.ValueOf(27652345.67344567m);
                loadedRowA.DateTime = CustomDateTime.ValueOf(new DateTime(year: 2011, month: 02, day: 20, hour: 07, minute: 03, second: 08));
                loadedRowA.DateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1991, month: 11, day: 10, hour: 20, minute: 06, second: 02, new TimeSpan(hours: 13, minutes: 00, seconds: 00)));
                loadedRowA.DateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 2025, month: 07, day: 14));
                loadedRowA.TimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 02, minute: 05, second: 29));
                loadedRowA.Float = CustomFloat.ValueOf(4323.14535f);
                loadedRowA.Double = CustomDouble.ValueOf(976547.94356345345d);

                loadedRowA.NGuid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowA.NShort = CustomShort.ValueOf(12);
                loadedRowA.NInt = CustomInt.ValueOf(1132434);
                loadedRowA.NLong = CustomLong.ValueOf(92783452345234);
                loadedRowA.NString = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowA.NBool = CustomBool.ValueOf(false);
                loadedRowA.NDecimal = CustomDecimal.ValueOf(23455432.2345234m);
                loadedRowA.NDateTime = CustomDateTime.ValueOf(new DateTime(year: 2024, month: 8, day: 12, hour: 14, minute: 02, second: 25));
                loadedRowA.NDateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2024, month: 02, day: 17, hour: 12, minute: 53, second: 14, new TimeSpan(hours: 05, minutes: 0, seconds: 0)));
                loadedRowA.NDateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 1991, month: 11, day: 29));
                loadedRowA.NTimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 23, minute: 59, second: 59));
                loadedRowA.NFloat = CustomFloat.ValueOf(6234563.54325f);
                loadedRowA.NDouble = CustomDouble.ValueOf(295234523.23452345d);

                Assert.IsTrue(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }
                AssertNumberOfRowsExists(rows: 2);
                AssertCustomTypes(loadedRowA);
                AssertCustomTypes(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test Update when no changes are required

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }

                Assert.AreEqual(2, repository.Count);

                AssertNumberOfRowsExists(rows: 2);

                AssertCustomTypes(loadedRowA);
                AssertCustomTypes(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test update rowB

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                loadedRowB.Guid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowB.Short = CustomShort.ValueOf(4324);
                loadedRowB.Int = CustomInt.ValueOf(33842141);
                loadedRowB.Long = CustomLong.ValueOf(4315234);
                loadedRowB.String = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowB.Bool = CustomBool.ValueOf(true);
                loadedRowB.Decimal = CustomDecimal.ValueOf(1231252345.61123144m);
                loadedRowB.DateTime = CustomDateTime.ValueOf(new DateTime(year: 2012, month: 02, day: 20, hour: 07, minute: 03, second: 08));
                loadedRowB.DateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1992, month: 11, day: 10, hour: 20, minute: 06, second: 02, new TimeSpan(hours: 13, minutes: 00, seconds: 00)));
                loadedRowB.DateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 2026, month: 07, day: 14));
                loadedRowB.TimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 01, minute: 05, second: 29));
                loadedRowB.Float = CustomFloat.ValueOf(8923.53454535f);
                loadedRowB.Double = CustomDouble.ValueOf(5345634.35634564d);

                loadedRowB.NGuid = null;
                loadedRowB.NShort = null;
                loadedRowB.NInt = null;
                loadedRowB.NLong = null;
                loadedRowB.NString = null;
                loadedRowB.NBool = null;
                loadedRowB.NDecimal = null;
                loadedRowB.NDateTime = null;
                loadedRowB.NDateTimeOffset = null;
                loadedRowB.NDateOnly = null;
                loadedRowB.NTimeOnly = null;
                loadedRowB.NFloat = null;
                loadedRowB.NDouble = null;

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsTrue(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }
                Assert.AreEqual(2, repository.Count);

                AssertNumberOfRowsExists(rows: 2);
                AssertCustomTypes(loadedRowA);
                AssertCustomTypes(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test update rowB when nullable values are null

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                loadedRowB.Guid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowB.Short = CustomShort.ValueOf(6324);
                loadedRowB.Int = CustomInt.ValueOf(3534141);
                loadedRowB.Long = CustomLong.ValueOf(5324523415234);
                loadedRowB.String = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowB.Bool = CustomBool.ValueOf(false);
                loadedRowB.Decimal = CustomDecimal.ValueOf(22345.52345234m);
                loadedRowB.DateTime = CustomDateTime.ValueOf(new DateTime(year: 2014, month: 02, day: 20, hour: 07, minute: 03, second: 08));
                loadedRowB.DateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1993, month: 11, day: 10, hour: 20, minute: 06, second: 02, new TimeSpan(hours: 13, minutes: 00, seconds: 00)));
                loadedRowB.DateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 2027, month: 07, day: 14));
                loadedRowB.TimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 02, minute: 05, second: 29));
                loadedRowB.Float = CustomFloat.ValueOf(18923.253454535f);
                loadedRowB.Double = CustomDouble.ValueOf(15345634.3563456445d);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsTrue(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }
                AssertNumberOfRowsExists(rows: 2);
                AssertCustomTypes(loadedRowA);
                AssertCustomTypes(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test delete rowA

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                repository.DeleteRow(loadedRowA);

                Assert.IsTrue(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }

                Assert.AreEqual(1, repository.Count);

                AssertNumberOfRowsExists(rows: 1);

                AssertCustomTypes(loadedRowB);

                Assert.IsFalse(repository.TryGetRowState(loadedRowA, out RowUpdateState? state));
                Assert.IsNull(state);
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test delete rowB

                Assert.AreEqual(1, repository.Count);

                CustomTypesRow loadedRowB = repository[0];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                repository.DeleteRow(loadedRowB);

                Assert.IsTrue(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    repository.SaveChanges(transaction, TimeoutLevel.ShortInsert);
                    transaction.Commit();
                }

                Assert.AreEqual(0, repository.Count);

                AssertNumberOfRowsExists(rows: 0);

                Assert.IsFalse(repository.TryGetRowState(loadedRowB, out RowUpdateState? state));
                Assert.IsNull(state);
            }
        }

        [TestMethod]
        public async Task TestAddSelectUpdateAndDeleteAsync() {

            CustomTypesRow rowA = GetCustomTypesA();
            CustomTypesRow rowB = GetCustomTypesB();

            Assert.IsTrue(rowA.Int.Value < rowB.Int.Value); //rowA CustomInt value is used for sorting below. So it needs to be less than rowB for test to work.

            CustomTypesRowRepository repository = new CustomTypesRowRepository();

            {   //Test select on empty table
                await repository.SelectRows.ExecuteAsync(TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(0, repository.Count);
            }

            { //Test insert
                repository.AddNewRow(rowA);
                repository.AddNewRow(rowB);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }

                AssertNumberOfRowsExists(rows: 2);

                await AssertCustomTypesAsync(rowA);
                await AssertCustomTypesAsync(rowB);

                Assert.IsFalse(repository.RequiresUpdate(rowA));
                Assert.IsFalse(repository.RequiresUpdate(rowB));
            }

            {   //Test select on table with two rows

                await repository
                    .SelectRows
                    .OrderBy(repository.Table.Int)
                    .ExecuteAsync(TestDatabase.Database, CancellationToken.None);

                foreach(CustomTypesRow x in repository) {

                }

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                AssertAreEqual(rowA, loadedRowA);
                AssertAreEqual(rowB, loadedRowB);
            }

            {   //Test update

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                loadedRowA.Guid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowA.Short = CustomShort.ValueOf(44);
                loadedRowA.Int = CustomInt.ValueOf(3384214);
                loadedRowA.Long = CustomLong.ValueOf(435234);
                loadedRowA.String = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowA.Bool = CustomBool.ValueOf(true);
                loadedRowA.Decimal = CustomDecimal.ValueOf(27652345.67344567m);
                loadedRowA.DateTime = CustomDateTime.ValueOf(new DateTime(year: 2011, month: 02, day: 20, hour: 07, minute: 03, second: 08));
                loadedRowA.DateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1991, month: 11, day: 10, hour: 20, minute: 06, second: 02, new TimeSpan(hours: 13, minutes: 00, seconds: 00)));
                loadedRowA.DateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 2025, month: 07, day: 14));
                loadedRowA.TimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 02, minute: 05, second: 29));
                loadedRowA.Float = CustomFloat.ValueOf(4323.14535f);
                loadedRowA.Double = CustomDouble.ValueOf(976547.94356345345d);

                loadedRowA.NGuid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowA.NShort = CustomShort.ValueOf(12);
                loadedRowA.NInt = CustomInt.ValueOf(1132434);
                loadedRowA.NLong = CustomLong.ValueOf(92783452345234);
                loadedRowA.NString = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowA.NBool = CustomBool.ValueOf(false);
                loadedRowA.NDecimal = CustomDecimal.ValueOf(23455432.2345234m);
                loadedRowA.NDateTime = CustomDateTime.ValueOf(new DateTime(year: 2024, month: 8, day: 12, hour: 14, minute: 02, second: 25));
                loadedRowA.NDateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2024, month: 02, day: 17, hour: 12, minute: 53, second: 14, new TimeSpan(hours: 05, minutes: 0, seconds: 0)));
                loadedRowA.NDateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 1991, month: 11, day: 29));
                loadedRowA.NTimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 23, minute: 59, second: 59));
                loadedRowA.NFloat = CustomFloat.ValueOf(6234563.54325f);
                loadedRowA.NDouble = CustomDouble.ValueOf(295234523.23452345d);

                Assert.IsTrue(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                AssertNumberOfRowsExists(rows: 2);
                await AssertCustomTypesAsync(loadedRowA);
                await AssertCustomTypesAsync(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test Update when no changes are required

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }

                Assert.AreEqual(2, repository.Count);

                AssertNumberOfRowsExists(rows: 2);

                await AssertCustomTypesAsync(loadedRowA);
                await AssertCustomTypesAsync(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }


            {   //Test update rowB

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                loadedRowB.Guid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowB.Short = CustomShort.ValueOf(4324);
                loadedRowB.Int = CustomInt.ValueOf(33842141);
                loadedRowB.Long = CustomLong.ValueOf(4315234);
                loadedRowB.String = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowB.Bool = CustomBool.ValueOf(true);
                loadedRowB.Decimal = CustomDecimal.ValueOf(1231252345.61123144m);
                loadedRowB.DateTime = CustomDateTime.ValueOf(new DateTime(year: 2012, month: 02, day: 20, hour: 07, minute: 03, second: 08));
                loadedRowB.DateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1992, month: 11, day: 10, hour: 20, minute: 06, second: 02, new TimeSpan(hours: 13, minutes: 00, seconds: 00)));
                loadedRowB.DateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 2026, month: 07, day: 14));
                loadedRowB.TimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 01, minute: 05, second: 29));
                loadedRowB.Float = CustomFloat.ValueOf(8923.53454535f);
                loadedRowB.Double = CustomDouble.ValueOf(5345634.35634564d);

                loadedRowB.NGuid = null;
                loadedRowB.NShort = null;
                loadedRowB.NInt = null;
                loadedRowB.NLong = null;
                loadedRowB.NString = null;
                loadedRowB.NBool = null;
                loadedRowB.NDecimal = null;
                loadedRowB.NDateTime = null;
                loadedRowB.NDateTimeOffset = null;
                loadedRowB.NDateOnly = null;
                loadedRowB.NTimeOnly = null;
                loadedRowB.NFloat = null;
                loadedRowB.NDouble = null;

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsTrue(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                Assert.AreEqual(2, repository.Count);

                AssertNumberOfRowsExists(rows: 2);
                await AssertCustomTypesAsync(loadedRowA);
                await AssertCustomTypesAsync(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test update rowB when nullable values are null

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                loadedRowB.Guid = CustomGuid.ValueOf(Guid.NewGuid());
                loadedRowB.Short = CustomShort.ValueOf(6324);
                loadedRowB.Int = CustomInt.ValueOf(3534141);
                loadedRowB.Long = CustomLong.ValueOf(5324523415234);
                loadedRowB.String = CustomString.ValueOf(Guid.NewGuid().ToString());
                loadedRowB.Bool = CustomBool.ValueOf(false);
                loadedRowB.Decimal = CustomDecimal.ValueOf(22345.52345234m);
                loadedRowB.DateTime = CustomDateTime.ValueOf(new DateTime(year: 2014, month: 02, day: 20, hour: 07, minute: 03, second: 08));
                loadedRowB.DateTimeOffset = CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1993, month: 11, day: 10, hour: 20, minute: 06, second: 02, new TimeSpan(hours: 13, minutes: 00, seconds: 00)));
                loadedRowB.DateOnly = CustomDateOnly.ValueOf(new DateOnly(year: 2027, month: 07, day: 14));
                loadedRowB.TimeOnly = CustomTimeOnly.ValueOf(new TimeOnly(hour: 02, minute: 05, second: 29));
                loadedRowB.Float = CustomFloat.ValueOf(18923.253454535f);
                loadedRowB.Double = CustomDouble.ValueOf(15345634.3563456445d);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsTrue(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                AssertNumberOfRowsExists(rows: 2);
                await AssertCustomTypesAsync(loadedRowA);
                await AssertCustomTypesAsync(loadedRowB);

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test delete rowA

                Assert.AreEqual(2, repository.Count);

                CustomTypesRow loadedRowA = repository[0];
                CustomTypesRow loadedRowB = repository[1];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                repository.DeleteRow(loadedRowA);

                Assert.IsTrue(repository.RequiresUpdate(loadedRowA));
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }

                Assert.AreEqual(1, repository.Count);

                AssertNumberOfRowsExists(rows: 1);

                await AssertCustomTypesAsync(loadedRowB);

                Assert.IsFalse(repository.TryGetRowState(loadedRowA, out RowUpdateState? state));
                Assert.IsNull(state);
                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));
            }

            {   //Test delete rowB

                Assert.AreEqual(1, repository.Count);

                CustomTypesRow loadedRowB = repository[0];

                Assert.IsFalse(repository.RequiresUpdate(loadedRowB));

                repository.DeleteRow(loadedRowB);

                Assert.IsTrue(repository.RequiresUpdate(loadedRowB));

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.SaveChangesAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }

                Assert.AreEqual(0, repository.Count);

                AssertNumberOfRowsExists(rows: 0);

                Assert.IsFalse(repository.TryGetRowState(loadedRowB, out RowUpdateState? state));
                Assert.IsNull(state);
            }
        }

        private static void AssertNumberOfRowsExists(int rows) {

            CustomTypesTable table = CustomTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

            QueryResult<int> result = Query.Select(
                    row => row.Get(count)
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(rows, result.Rows[0]);
        }

        private static void CopyValuesTo(CustomTypesRow from, CustomTypesRow to) {

            to.Guid = from.Guid;
            to.Short = from.Short;
            to.Int = from.Int;
            to.Long = from.Long;
            to.String = from.String;
            to.Bool = from.Bool;
            to.Decimal = from.Decimal;
            to.DateTime = from.DateTime;
            to.DateTimeOffset = from.DateTimeOffset;
            to.DateOnly = from.DateOnly;
            to.TimeOnly = from.TimeOnly;
            to.Float = from.Float;
            to.Double = from.Double;

            to.NGuid = from.NGuid;
            to.NShort = from.NShort;
            to.NInt = from.NInt;
            to.NLong = from.NLong;
            to.NString = from.NString;
            to.NBool = from.NBool;
            to.NDecimal = from.NDecimal;
            to.NDateTime = from.NDateTime;
            to.NDateTimeOffset = from.NDateTimeOffset;
            to.NDateOnly = from.NDateOnly;
            to.NTimeOnly = from.NTimeOnly;
            to.NFloat = from.NFloat;
            to.NDouble = from.NDouble;
        }

        private static void AssertCustomTypes(CustomTypesRow row) {

            CustomTypesRowRepository repository = new CustomTypesRowRepository();

            repository
                .SelectRows
                .Where(repository.Table.Guid == row.Guid)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, repository.Count);

            CustomTypesRow values = repository[0];

            Assert.AreEqual(row.Guid, values.Guid);
            Assert.AreEqual(row.Identifier, values.Identifier);
            Assert.AreEqual(row.Short, values.Short);
            Assert.AreEqual(row.Int, values.Int);
            Assert.AreEqual(row.Long, values.Long);
            Assert.AreEqual(row.String, values.String);
            Assert.AreEqual(row.Bool, values.Bool);
            Assert.AreEqual(row.Decimal, values.Decimal);
            Assert.AreEqual(row.DateTime, values.DateTime);
            Assert.AreEqual(row.DateTimeOffset, values.DateTimeOffset);
            Assert.AreEqual(row.DateOnly, values.DateOnly);
            Assert.AreEqual(row.TimeOnly, values.TimeOnly);
            Assert.AreEqual(row.Float, values.Float);
            Assert.AreEqual(row.Double, values.Double);

            Assert.AreEqual(row.NGuid, values.NGuid);
            Assert.AreEqual(row.NShort, values.NShort);
            Assert.AreEqual(row.NInt, values.NInt);
            Assert.AreEqual(row.NLong, values.NLong);
            Assert.AreEqual(row.NString, values.NString);
            Assert.AreEqual(row.NBool, values.NBool);
            Assert.AreEqual(row.NDecimal, values.NDecimal);
            Assert.AreEqual(row.NDateTime, values.NDateTime);
            Assert.AreEqual(row.NDateTimeOffset, values.NDateTimeOffset);
            Assert.AreEqual(row.NDateOnly, values.NDateOnly);
            Assert.AreEqual(row.NTimeOnly, values.NTimeOnly);
            Assert.AreEqual(row.NFloat, values.NFloat);
            Assert.AreEqual(row.NDouble, values.NDouble);
        }

        private static async Task AssertCustomTypesAsync(CustomTypesRow row) {

            CustomTypesRowRepository repository = new CustomTypesRowRepository();

            await repository
                .SelectRows
                .Where(repository.Table.Guid == row.Guid)
                .ExecuteAsync(TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, repository.Count);

            CustomTypesRow values = repository[0];

            Assert.AreEqual(row.Guid, values.Guid);
            Assert.AreEqual(row.Identifier, values.Identifier);
            Assert.AreEqual(row.Short, values.Short);
            Assert.AreEqual(row.Int, values.Int);
            Assert.AreEqual(row.Long, values.Long);
            Assert.AreEqual(row.String, values.String);
            Assert.AreEqual(row.Bool, values.Bool);
            Assert.AreEqual(row.Decimal, values.Decimal);
            Assert.AreEqual(row.DateTime, values.DateTime);
            Assert.AreEqual(row.DateTimeOffset, values.DateTimeOffset);
            Assert.AreEqual(row.DateOnly, values.DateOnly);
            Assert.AreEqual(row.TimeOnly, values.TimeOnly);
            Assert.AreEqual(row.Float, values.Float);
            Assert.AreEqual(row.Double, values.Double);

            Assert.AreEqual(row.NGuid, values.NGuid);
            Assert.AreEqual(row.NShort, values.NShort);
            Assert.AreEqual(row.NInt, values.NInt);
            Assert.AreEqual(row.NLong, values.NLong);
            Assert.AreEqual(row.NString, values.NString);
            Assert.AreEqual(row.NBool, values.NBool);
            Assert.AreEqual(row.NDecimal, values.NDecimal);
            Assert.AreEqual(row.NDateTime, values.NDateTime);
            Assert.AreEqual(row.NDateTimeOffset, values.NDateTimeOffset);
            Assert.AreEqual(row.NDateOnly, values.NDateOnly);
            Assert.AreEqual(row.NTimeOnly, values.NTimeOnly);
            Assert.AreEqual(row.NFloat, values.NFloat);
            Assert.AreEqual(row.NDouble, values.NDouble);
        }

        private static void AssertAreEqual(CustomTypesRow rowA, CustomTypesRow rowB) {

            Assert.AreEqual(rowA.Guid, rowB.Guid);
            Assert.AreEqual(rowA.Identifier, rowB.Identifier);
            Assert.AreEqual(rowA.Short, rowB.Short);
            Assert.AreEqual(rowA.Int, rowB.Int);
            Assert.AreEqual(rowA.Long, rowB.Long);
            Assert.AreEqual(rowA.String, rowB.String);
            Assert.AreEqual(rowA.Bool, rowB.Bool);
            Assert.AreEqual(rowA.Decimal, rowB.Decimal);
            Assert.AreEqual(rowA.DateTime, rowB.DateTime);
            Assert.AreEqual(rowA.DateTimeOffset, rowB.DateTimeOffset);
            Assert.AreEqual(rowA.DateOnly, rowB.DateOnly);
            Assert.AreEqual(rowA.TimeOnly, rowB.TimeOnly);
            Assert.AreEqual(rowA.Float, rowB.Float);
            Assert.AreEqual(rowA.Double, rowB.Double);

            Assert.AreEqual(rowA.NGuid, rowB.NGuid);
            Assert.AreEqual(rowA.NShort, rowB.NShort);
            Assert.AreEqual(rowA.NInt, rowB.NInt);
            Assert.AreEqual(rowA.NLong, rowB.NLong);
            Assert.AreEqual(rowA.NString, rowB.NString);
            Assert.AreEqual(rowA.NBool, rowB.NBool);
            Assert.AreEqual(rowA.NDecimal, rowB.NDecimal);
            Assert.AreEqual(rowA.NDateTime, rowB.NDateTime);
            Assert.AreEqual(rowA.NDateTimeOffset, rowB.NDateTimeOffset);
            Assert.AreEqual(rowA.NDateOnly, rowB.NDateOnly);
            Assert.AreEqual(rowA.NTimeOnly, rowB.NTimeOnly);
            Assert.AreEqual(rowA.NFloat, rowB.NFloat);
            Assert.AreEqual(rowA.NDouble, rowB.NDouble);
        }

        private static CustomTypesRow GetCustomTypesA() => new CustomTypesRow(

            guid: CustomGuid.ValueOf(Guid.NewGuid()),
            identifier: CustomLong.ValueOf(-1),
            @short: CustomShort.ValueOf(12),
            @int: CustomInt.ValueOf(55),
            @long: CustomLong.ValueOf(43213412),
            @string: CustomString.ValueOf(Guid.NewGuid().ToString()),
            @bool: CustomBool.ValueOf(true),
            @decimal: CustomDecimal.ValueOf(0.22345200m),
            dateTime: CustomDateTime.ValueOf(new DateTime(year: 2025, month: 03, day: 10, hour: 15, minute: 01, second: 7)),
            dateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2024, month: 02, day: 11, hour: 22, minute: 52, second: 12, new TimeSpan(hours: 5, minutes: 0, seconds: 0))),
            dateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 2025, month: 05, day: 12)),
            timeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 01, minute: 00, second: 25)),
            @float: CustomFloat.ValueOf(342.1234423f),
            @double: CustomDouble.ValueOf(45152345234.234523452345d),

            nGuid: null,
            nShort: null,
            nInt: null,
            nLong: null,
            nString: null,
            nBool: null,
            nDecimal: null,
            nDateTime: null,
            nDateTimeOffset: null,
            nDateOnly: null,
            nTimeOnly: null,
            nFloat: null,
            nDouble: null
        );

        private static CustomTypesRow GetCustomTypesB() => new CustomTypesRow(

            @guid: CustomGuid.ValueOf(Guid.NewGuid()),
            identifier: CustomLong.ValueOf(-1),
            @short: CustomShort.ValueOf(43),
            @int: CustomInt.ValueOf(2384234),
            @long: CustomLong.ValueOf(6525234),
            @string: CustomString.ValueOf(Guid.NewGuid().ToString()),
            @bool: CustomBool.ValueOf(false),
            @decimal: CustomDecimal.ValueOf(-23452345.65474567m),
            dateTime: CustomDateTime.ValueOf(new DateTime(year: 2010, month: 01, day: 30, hour: 17, minute: 02, second: 7)),
            dateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1990, month: 10, day: 11, hour: 21, minute: 05, second: 01, new TimeSpan(hours: 12, minutes: 0, seconds: 0))),
            dateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 2024, month: 06, day: 13)),
            timeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 04, minute: 01, second: 27)),
            @float: CustomFloat.ValueOf(3.14535f),
            @double: CustomDouble.ValueOf(92345234523.98726452345d),

            nGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            nShort: CustomShort.ValueOf(11),
            nInt: CustomInt.ValueOf(3232),
            nLong: CustomLong.ValueOf(2414564),
            nString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            nBool: CustomBool.ValueOf(true),
            nDecimal: CustomDecimal.ValueOf(2134.45234m),
            nDateTime: CustomDateTime.ValueOf(new DateTime(year: 2023, month: 9, day: 11, hour: 15, minute: 01, second: 7)),
            nDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2023, month: 05, day: 12, hour: 10, minute: 52, second: 15, new TimeSpan(hours: 7, minutes: 0, seconds: 0))),
            nDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 1990, month: 12, day: 31)),
            nTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 23, minute: 59, second: 59)),
            nFloat: CustomFloat.ValueOf(123423.2345234f),
            nDouble: CustomDouble.ValueOf(73458.22347589234d)
        );
    }
}