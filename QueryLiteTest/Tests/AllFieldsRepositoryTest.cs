using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class AllFieldsRepositoryTest {

        [TestInitialize]
        public void ClearTable() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            AllTypesRepository repository = new AllTypesRepository();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                repository.SelectRows.Execute(transaction);

                foreach(AllTypesRow row in repository) {
                    repository.DeleteRow(row);
                }
                repository.PersistDeletesOnly(transaction);

                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = Query
                    .Select(
                        row => row.Get(count)
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
        public void BasicWithQueriesAndNoParameters() {

            Settings.UseParameters = false;
            BasicInsertUpdateAndDeleteWithQueries();
        }

        [TestMethod]
        public async Task BasicWithQueriesAndNoParametersAsync() {

            Settings.UseParameters = false;
            await BasicInsertUpdateAndDeleteWithQueriesAsync();
        }

        [TestMethod]
        public void BasicInsertAndTruncationWithQueries() {

            Settings.UseParameters = false;
            BasicInsertAndTruncateWithQueries();
        }

        [TestMethod]
        public async Task BasicInsertAndTruncationWithQueriesAsync() {

            Settings.UseParameters = false;
            await BasicInsertAndTruncateWithQueriesAsync();
        }

        [TestMethod]
        public void BasicWithQueriesAndParameters() {

            Settings.UseParameters = true;
            BasicInsertUpdateAndDeleteWithQueries();
        }

        [TestMethod]
        public async Task BasicWithQueriesAndParametersAsync() {

            Settings.UseParameters = true;
            await BasicInsertUpdateAndDeleteWithQueriesAsync();
        }

        private static AllTypes GetAllTypes1() {

            return new AllTypes(
                id: IntKey<AllTypes>.NotSet,
                guid: Guid.NewGuid(),
                @string: "88udskj🐘a8adf😀q23",
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

        private static void AssertRowExists(AllTypes allTypes) {

            AllTypesRepository repository = new AllTypesRepository();

            repository
                .SelectRows
                .Where(repository.Table.Id == allTypes.Id)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, repository.Count);

            AssertRow(repository[0], allTypes);
        }

        private static async Task AssertRowExistsAsync(AllTypes allTypes) {

            AllTypesRepository repository = new AllTypesRepository();

            await repository.SelectRows
                .Where(repository.Table.Id == allTypes.Id)
                .ExecuteAsync(TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, repository.Count);

            AssertRow(repository[0], allTypes);
        }

        private static void AssertRowExists(AllTypes allTypes, Transaction transaction) {

            AllTypesRepository repository = new AllTypesRepository();

            repository.SelectRows
                .Where(repository.Table.Id == allTypes.Id)
                .Execute(transaction);

            Assert.AreEqual(1, repository.Count);

            AssertRow(repository[0], allTypes);
        }

        private static void AssertRowDoesNotExists(AllTypes allTypes) {

            AllTypesRepository repository = new AllTypesRepository();

            repository.SelectRows
                .Where(repository.Table.Id == allTypes.Id)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(0, repository.Count);
        }

        private static void AssertRowDoesNotExists(AllTypes allTypes, Transaction transaction) {

            AllTypesRepository repository = new AllTypesRepository();

            repository.SelectRows
                .Where(repository.Table.Id == allTypes.Id)
                .Execute(transaction);

            Assert.AreEqual(0, repository.Count);
        }

        public static void AssertRow(AllTypesRow row, AllTypes allTypes) {

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

        private static void BasicInsertUpdateAndDeleteWithQueries() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            JoinQuery(allTypes1);
            JoinQuery(allTypes2);
            JoinQuery(allTypes3);

            DeleteWithQueryAndRollback(allTypes1);
            DeleteWithQueryAndRollback(allTypes2);
            DeleteWithQueryAndRollback(allTypes3);

            UpdateWithQuery(allTypes1);
            UpdateWithQuery(allTypes2);
            UpdateWithQuery(allTypes3);

            UpdateWithQueryAndRollback(allTypes1);
            UpdateWithQueryAndRollback(allTypes2);
            UpdateWithQueryAndRollback(allTypes3);

            DeleteWithQuery(allTypes1);
            DeleteWithQuery(allTypes2);

            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
        }

        private static void BasicInsertAndTruncateWithQueries() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            Truncate();
        }

        private static async Task BasicInsertAndTruncateWithQueriesAsync() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            await TruncateAsync();
        }

        private static async Task BasicInsertUpdateAndDeleteWithQueriesAsync() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            await JoinQueryAsync(allTypes1);
            await JoinQueryAsync(allTypes2);
            await JoinQueryAsync(allTypes3);

            DeleteWithQueryAndRollback(allTypes1);
            DeleteWithQueryAndRollback(allTypes2);
            DeleteWithQueryAndRollback(allTypes3);

            await UpdateWithQueryAsync(allTypes1);
            await UpdateWithQueryAsync(allTypes2);
            await UpdateWithQueryAsync(allTypes3);

            UpdateWithQueryAndRollback(allTypes1);
            UpdateWithQueryAndRollback(allTypes2);
            UpdateWithQueryAndRollback(allTypes3);

            await DeleteWithQueryAsync(allTypes1);
            await DeleteWithQueryAsync(allTypes2);

            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
        }

        private static void InsertWithQuery(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesRepository repository = new AllTypesRepository();

                AllTypesRow row = new AllTypesRow(
                    allTypes.Id,
                    allTypes.Guid,
                    allTypes.String,
                    allTypes.SmallInt,
                    allTypes.Int,
                    allTypes.BigInt,
                    allTypes.Decimal,
                    allTypes.Float,
                    allTypes.Double,
                    allTypes.Boolean,
                    allTypes.Bytes,
                    allTypes.DateTime,
                    allTypes.DateTimeOffset,
                    allTypes.Enum,
                    allTypes.DateOnly,
                    allTypes.TimeOnly
                );

                repository.AddNewRow(row);

                int rowsEffected = repository.SaveChanges(transaction);

                transaction.Commit();

                Assert.AreEqual(1, repository.Count);
                Assert.AreEqual(1, rowsEffected);

                allTypes.Id = repository[0].Id;

                AssertRow(repository[0], allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                AssertRowExists(allTypes);
            }
        }

        private static void Truncate() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult truncateResult = Query
                    .Truncate(allTypesTable)
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                transaction.Commit();
            }

            COUNT_ALL count = COUNT_ALL.Instance;

            var result = Query
                .Select(row => new { Count = row.Get(count) })
                .From(allTypesTable)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            Assert.AreEqual(0, result.Rows.First().Count);
        }

        private static async Task TruncateAsync() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult truncateResult = await Query
                    .Truncate(allTypesTable)
                    .ExecuteAsync(transaction);

                transaction.Commit();
            }

            COUNT_ALL count = COUNT_ALL.Instance;

            var result = await Query
                .Select(row => new { Count = row.Get(count) })
                .From(allTypesTable)
                .ExecuteAsync(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            Assert.AreEqual(0, result.Rows.First().Count);
        }

        public static async Task InsertWithQueryAsync(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesRepository repository = new AllTypesRepository();

                AllTypesRow row = new AllTypesRow(
                    allTypes.Id,
                    allTypes.Guid,
                    allTypes.String,
                    allTypes.SmallInt,
                    allTypes.Int,
                    allTypes.BigInt,
                    allTypes.Decimal,
                    allTypes.Float,
                    allTypes.Double,
                    allTypes.Boolean,
                    allTypes.Bytes,
                    allTypes.DateTime,
                    allTypes.DateTimeOffset,
                    allTypes.Enum,
                    allTypes.DateOnly,
                    allTypes.TimeOnly
                );

                repository.AddNewRow(row);

                int rowsEffected = repository.SaveChanges(transaction);

                transaction.Commit();

                Assert.AreEqual(1, repository.Count);
                Assert.AreEqual(1, rowsEffected);

                allTypes.Id = repository[0].Id;

                AssertRow(repository[0], allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                await AssertRowExistsAsync(allTypes);
            }
        }

        private static void UpdateWithQuery(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            allTypes.UpdateValues(
                guid: Guid.NewGuid(),
                @string: "-4at3=🦕_)(*&_(*#(*Kj🐘s734-g*%🦗lf]|][",
                smallInt: 9794,
                @int: 7761843,
                bigInt: 5546328205,
                @decimal: 614.887298m,
                @float: 676832.13291f,
                @double: 552761.997868d,
                boolean: false,
                bytes: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                dateTime: new DateTime(year: 2023, month: 1, day: 2, hour: 3, minute: 4, second: 5),
                dateTimeOffset: new DateTimeOffset(year: 2030, month: 12, day: 11, hour: 10, minute: 9, second: 8, new TimeSpan(hours: 0, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.B,
                dateOnly: new DateOnly(year: 1990, month: 1, day: 31),
                timeOnly: new TimeOnly(hour: 12, minute: 14, second: 55, millisecond: 130, microsecond: 999)
            );

            AllTypesRepository repository = new AllTypesRepository();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                repository.SelectRows
                    .Where(repository.Table.Id == allTypes.Id)
                    .Execute(transaction);

                Assert.AreEqual(1, repository.Count);

                AllTypesRow row = repository[0];

                row.SetValues(
                    id: row.Id,
                    guid: allTypes.Guid,
                    @string: allTypes.String,
                    smallInt: allTypes.SmallInt,
                    @int: allTypes.Int,
                    bigInt: allTypes.BigInt,
                    @decimal: allTypes.Decimal,
                    @float: allTypes.Float,
                    @double: allTypes.Double,
                    boolean: allTypes.Boolean,
                    bytes: allTypes.Bytes,
                    dateTime: allTypes.DateTime,
                    dateTimeOffset: allTypes.DateTimeOffset,
                    @enum: allTypes.Enum,
                    dateOnly: allTypes.DateOnly,
                    timeOnly: allTypes.TimeOnly
                );

                int rowsEffected = repository.SaveChanges(transaction);

                Assert.AreEqual(1, rowsEffected);
                Assert.AreEqual(1, repository.Count);

                AssertRow(repository[0], allTypes);

                transaction.Commit();

                AssertRowExists(allTypes);
            }
        }

        private static async Task UpdateWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            allTypes.UpdateValues(
                guid: Guid.NewGuid(),
                @string: "🦏-4at3=_)(*&_(🌍*#(*Kjs734💻-g*%lf]|][",
                smallInt: 9794,
                @int: 7761843,
                bigInt: 5546328205,
                @decimal: 614.887298m,
                @float: 676832.13291f,
                @double: 552761.997868d,
                boolean: false,
                bytes: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                dateTime: new DateTime(year: 2023, month: 1, day: 2, hour: 3, minute: 4, second: 5),
                dateTimeOffset: new DateTimeOffset(year: 2030, month: 12, day: 11, hour: 10, minute: 9, second: 8, new TimeSpan(hours: 0, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.B,
                dateOnly: new DateOnly(year: 1854, month: 05, day: 27),
                timeOnly: new TimeOnly(hour: 1, minute: 4, second: 5, millisecond: 30, microsecond: 100)
            );

            AllTypesRepository repository = new AllTypesRepository();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                await repository.SelectRows
                    .Where(repository.Table.Id == allTypes.Id)
                    .ExecuteAsync(transaction, CancellationToken.None);

                Assert.AreEqual(1, repository.Count);

                AllTypesRow row = repository[0];

                row.SetValues(
                    id: row.Id,
                    guid: allTypes.Guid,
                    @string: allTypes.String,
                    smallInt: allTypes.SmallInt,
                    @int: allTypes.Int,
                    bigInt: allTypes.BigInt,
                    @decimal: allTypes.Decimal,
                    @float: allTypes.Float,
                    @double: allTypes.Double,
                    boolean: allTypes.Boolean,
                    bytes: allTypes.Bytes,
                    dateTime: allTypes.DateTime,
                    dateTimeOffset: allTypes.DateTimeOffset,
                    @enum: allTypes.Enum,
                    dateOnly: allTypes.DateOnly,
                    timeOnly: allTypes.TimeOnly
                );

                int rowsEffected = await repository.SaveChangesAsync(transaction, CancellationToken.None);

                Assert.AreEqual(1, rowsEffected);
                Assert.AreEqual(1, repository.Count);

                AssertRow(repository[0], allTypes);

                transaction.Commit();

                await AssertRowExistsAsync(allTypes);
            }
        }

        private static void DeleteWithQuery(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = GetNumberOfRows();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesRepository repository = new AllTypesRepository();

                repository.SelectRows
                    .Where(repository.Table.Id == allTypes.Id)
                    .Execute(transaction);

                Assert.AreEqual(1, repository.Count);

                repository.DeleteRow(repository[0]);

                int rowsEffected = repository.SaveChanges(transaction);

                Assert.AreEqual(1, rowsEffected);

                transaction.Commit();
            }

            {

                AllTypesTable allTypesTable = AllTypesTable.Instance;

                COUNT_ALL count = COUNT_ALL.Instance;

                var result = Query
                    .Select(
                        result => new {
                            Count = result.Get(count)
                        }
                    )
                    .From(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0].Count;

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows() + 1);
        }

        private static async Task DeleteWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await GetNumberOfRowsAsync();

            AllTypesRepository repository = new AllTypesRepository();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                await repository.SelectRows
                    .Where(repository.Table.Id == allTypes.Id)
                    .ExecuteAsync(transaction, CancellationToken.None);

                Assert.AreEqual(1, repository.Count);

                repository.DeleteRow(repository[0]);

                int rowsEffected = await repository.SaveChangesAsync(transaction, CancellationToken.None);

                Assert.AreEqual(1, rowsEffected);

                transaction.Commit();
            }

            {

                AllTypesTable allTypesTable = AllTypesTable.Instance;

                COUNT_ALL count = COUNT_ALL.Instance;

                var result = await Query
                    .Select(
                        result => new {
                            Count = result.Get(count)
                        }
                    )
                    .From(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0].Count;

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
            Assert.AreEqual(beginRowCount, await GetNumberOfRowsAsync() + 1);
        }

        private static int GetNumberOfRows() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

            var result = Query
                .Select(
                    result => new {
                        Count = result.Get(count)
                    }
                )
                .From(allTypesTable)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            int? countValue = result.Rows[0].Count;

            Assert.IsNotNull(countValue);
            return countValue!.Value;
        }

        private static async Task<int> GetNumberOfRowsAsync() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

            var result = await Query
                .Select(
                    result => new {
                        Count = result.Get(count)
                    }
                )
                .From(allTypesTable)
                .ExecuteAsync(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            int? countValue = result.Rows[0].Count;

            Assert.IsNotNull(countValue);
            return countValue!.Value;
        }

        private static void InsertWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            AllTypesRepository repository = new AllTypesRepository();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable table = AllTypesTable.Instance;

                AllTypesRow row = new AllTypesRow();

                row.SetValues(
                    id: IntKey<AllTypes>.NotSet,
                    guid: allTypes.Guid,
                    @string: allTypes.String,
                    smallInt: allTypes.SmallInt,
                    @int: allTypes.Int,
                    bigInt: allTypes.BigInt,
                    @decimal: allTypes.Decimal,
                    @float: allTypes.Float,
                    @double: allTypes.Double,
                    boolean: allTypes.Boolean,
                    bytes: allTypes.Bytes,
                    dateTime: allTypes.DateTime,
                    dateTimeOffset: allTypes.DateTimeOffset,
                    @enum: allTypes.Enum,
                    dateOnly: allTypes.DateOnly,
                    timeOnly: allTypes.TimeOnly
                );

                repository.AddNewRow(row);

                int rowsEffected = repository.SaveChanges(transaction);

                Assert.AreEqual(1, repository.Count);
                Assert.AreEqual(1, rowsEffected);

                allTypes.Id = repository[0].Id;

                AssertRow(repository[0], allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                AssertRowExists(allTypes, transaction);

                transaction.Rollback();

                AssertRowDoesNotExists(allTypes);

                allTypes.Id = IntKey<AllTypes>.NotSet;
            }
        }

        private static void UpdateWithQueryAndRollback(AllTypes initialAllTypes) {

            Assert.IsTrue(initialAllTypes.Id.IsValid);

            AllTypes newAllTypes = new AllTypes(
                id: initialAllTypes.Id,
                guid: Guid.NewGuid(),
                @string: "F89&sad💾^&%$Djadsa",
                smallInt: 1594,
                @int: 7742143,
                bigInt: 55461234205,
                @decimal: 6324.843298m,
                @float: 612342.142391f,
                @double: 554231.942368d,
                boolean: false,
                bytes: [5, 99, 3, 6, 5, 4, 7, 3, 1, 10],
                dateTime: new DateTime(year: 2019, month: 11, day: 12, hour: 13, minute: 14, second: 15),
                dateTimeOffset: new DateTimeOffset(year: 2025, month: 11, day: 10, hour: 1, minute: 7, second: 5, new TimeSpan(hours: 3, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.C,
                dateOnly: new DateOnly(year: 9999, month: 12, day: 31),
                timeOnly: new TimeOnly(hour: 9, minute: 59, second: 1, millisecond: 770, microsecond: 11)
            );

            AllTypesRepository repository = new AllTypesRepository();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                repository.SelectRows
                    .Where(repository.Table.Id == newAllTypes.Id)
                    .Execute(transaction);

                Assert.AreEqual(1, repository.Count);

                AllTypesRow row = repository[0];

                row.Guid = newAllTypes.Guid;
                row.String = newAllTypes.String;
                row.SmallInt = newAllTypes.SmallInt;
                row.Int = newAllTypes.Int;
                row.BigInt = newAllTypes.BigInt;
                row.Decimal = newAllTypes.Decimal;
                row.Float = newAllTypes.Float;
                row.Double = newAllTypes.Double;
                row.Boolean = newAllTypes.Boolean;
                row.Bytes = newAllTypes.Bytes;
                row.DateTime = newAllTypes.DateTime;
                row.DateTimeOffset = newAllTypes.DateTimeOffset;
                row.Enum = newAllTypes.Enum;
                row.DateOnly = newAllTypes.DateOnly;
                row.TimeOnly = newAllTypes.TimeOnly;

                int rowsEffected = repository.SaveChanges(transaction);

                Assert.AreEqual(1, rowsEffected);
                Assert.AreEqual(1, repository.Count);

                AssertRow(repository[0], newAllTypes);
                AssertRowExists(newAllTypes, transaction);

                transaction.Rollback();
                AssertRowExists(initialAllTypes);   //Assert initial values
            }
        }

        private static void DeleteWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                Assert.AreEqual(1, result.RowsEffected);

                AssertRowDoesNotExists(allTypes, transaction);

                transaction.Rollback();

                AssertRowExists(allTypes);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows());
        }

        private static void JoinQuery(AllTypes allTypes) {

            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;
            AllTypesTable allTypesTable4 = AllTypesTable.Instance4;

            AllTypesRepository repository = new AllTypesRepository();

            repository.SelectRows
                .With(SqlServerTableHint.UPDLOCK, SqlServerTableHint.SERIALIZABLE)
                .Join(allTypesTable2).On(repository.Table.Id == allTypesTable2.Id)
                .Join(allTypesTable3).On(allTypesTable2.Id == allTypesTable3.Id)
                .LeftJoin(allTypesTable4).On(allTypesTable4.Id == new IntKey<AllTypes>(928756923))
                .Where(allTypesTable3.Id == allTypes.Id)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, repository.Count);

            AllTypesRow row = repository[0];

            AssertRow(row, allTypes);
        }

        private static async Task JoinQueryAsync(AllTypes allTypes) {

            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;
            AllTypesTable allTypesTable4 = AllTypesTable.Instance4;

            AllTypesRepository repository = new AllTypesRepository();

            await repository.SelectRows
                .With(SqlServerTableHint.UPDLOCK, SqlServerTableHint.SERIALIZABLE)
                .Join(allTypesTable2).On(repository.Table.Id == allTypesTable2.Id)
                .Join(allTypesTable3).On(allTypesTable2.Id == allTypesTable3.Id)
                .LeftJoin(allTypesTable4).On(allTypesTable4.Id == new IntKey<AllTypes>(928756923))
                .Where(allTypesTable3.Id == allTypes.Id)
                .ExecuteAsync(TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, repository.Count);

            AllTypesRow row = repository[0];

            AssertRow(row, allTypes);
        }
    }
}