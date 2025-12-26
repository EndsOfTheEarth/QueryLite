using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.DbSchema;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using QueryLiteTestLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class AllFieldsTest {

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
            AllFieldsTest.BasicInsertUpdateAndDeleteWithQueries();
        }

        [TestMethod]
        public async Task BasicWithQueriesAndNoParametersAsync() {

            Settings.UseParameters = false;
            await AllFieldsTest.BasicInsertUpdateAndDeleteWithQueriesAsync();
        }

        [TestMethod]
        public void BasicInsertAndTruncationWithQueries() {

            Settings.UseParameters = false;
            AllFieldsTest.BasicInsertAndTruncateWithQueries();
        }

        [TestMethod]
        public async Task BasicInsertAndTruncationWithQueriesAsync() {

            Settings.UseParameters = false;
            await AllFieldsTest.BasicInsertAndTruncateWithQueriesAsync();
        }

        [TestMethod]
        public void BasicWithQueriesAndParameters() {

            Settings.UseParameters = true;
            AllFieldsTest.BasicInsertUpdateAndDeleteWithQueries();
        }

        [TestMethod]
        public async Task BasicWithQueriesAndParametersAsync() {

            Settings.UseParameters = true;
            await AllFieldsTest.BasicInsertUpdateAndDeleteWithQueriesAsync();
        }

        [TestMethod]
        public void LoadDocumentation() {

            string doc = DocumentationGenerator.GenerateForAssembly([Assembly.GetExecutingAssembly()], applicationName: "Auto Tester", version: "v1.0");
            Assert.IsNotEmpty(doc);
        }

        [TestMethod]
        public void UpdateJoinTestWithoutParams() {

            Settings.UseParameters = false;

            AllFieldsTest.UpdateJoinTest();
        }
        [TestMethod]
        public void UpdateJoinTestWithoutParams2() {

            Settings.UseParameters = false;

            AllFieldsTest.UpdateJoinTest2();
        }

        [TestMethod]
        public void UpdateJoinTestWithParams() {

            Settings.UseParameters = true;

            AllFieldsTest.UpdateJoinTest();
        }
        [TestMethod]
        public void UpdateJoinTestWithParams2() {

            Settings.UseParameters = true;

            AllFieldsTest.UpdateJoinTest2();
        }

        [TestMethod]
        public async Task UpdateJoinTestWithoutParamsAsync() {

            Settings.UseParameters = false;

            await AllFieldsTest.UpdateJoinTestAsync();
        }
        [TestMethod]
        public async Task UpdateJoinTestWithoutParamsAsync2() {

            Settings.UseParameters = false;

            await AllFieldsTest.UpdateJoinTestAsync2();
        }

        [TestMethod]
        public async Task UpdateJoinTestWithParamsAsync() {

            Settings.UseParameters = true;

            await AllFieldsTest.UpdateJoinTestAsync();
        }
        [TestMethod]
        public async Task UpdateJoinTestWithParamsAsync2() {

            Settings.UseParameters = true;

            await AllFieldsTest.UpdateJoinTestAsync2();
        }

        [TestMethod]
        public void RunSchemaValidator() {

            SchemaValidationSettings settings = new SchemaValidationSettings() {
                ValidatePrimaryKeys = true,
                ValidateUniqueConstraints = true,
                ValidateForeignKeys = true,
                ValidateCheckConstraintNames = true,
                ValidateMissingCodeTables = true
            };

            List<ITable> tables = new List<ITable>() {
                AllTypesTable.Instance,
                ParentTable.Instance,
                ChildTable.Instance,
                EnumTestTableTable.Instance,
                CustomTypesTable.Instance
            };

            if(TestDatabase.Database.DatabaseType == DatabaseType.SqlServer) {
                tables.Add(GeoTestTable.Instance);
                tables.Add(RowVersionTestTable.Instance);
            }

            ValidationResult result = SchemaValidator.ValidateTables(TestDatabase.Database, tables, settings);

            StringBuilder messages = new StringBuilder();

            foreach(ValidationItem item in result.Items) {

                foreach(string message in item.ValidationMessages) {
                    messages.AppendLine(message);
                }
            }

            string text = messages.ToString();

            if(TestDatabase.Database.DatabaseType == DatabaseType.SqlServer) {
                Assert.AreEqual(7, result.Items.Count);
            }
            else {
                Assert.AreEqual(5, result.Items.Count);
            }

            foreach(ValidationItem val in result.Items) {

                Assert.AreEqual(0, val.ValidationMessages.Count);
            }
        }

        [TestMethod]
        public async Task DeleteJoinQueriesAsync() {

            if(TestDatabase.Database.DatabaseType == DatabaseType.SqlServer) {
                Settings.UseParameters = true;
                await BasicInsertAndDeleteJoinQueriesSqlServerAsync();
            }
            else if(TestDatabase.Database.DatabaseType == DatabaseType.PostgreSql) {
                Settings.UseParameters = true;
                await AllFieldsTest.BasicInsertAndDeleteJoinQueriesPostgreSqlAsync();
            }
        }

        [TestMethod]
        public void KeyTest() {

            //Test that the key types equals operator does not cause a stack overflow

            {
                GuidKey<AllTypes>? guidKey = GuidKey<AllTypes>.ValueOf(Guid.NewGuid());

                Assert.IsFalse(guidKey == null);
                Assert.IsTrue(guidKey != null);

                guidKey = null;

                Assert.IsTrue(guidKey == null);
                Assert.IsFalse(guidKey != null);
            }

            {
                StringKey<AllTypes>? stringKey = StringKey<AllTypes>.ValueOf("abc");

                if(stringKey == null) { }
                if(stringKey != null) { }

                Assert.IsFalse(stringKey == null);
                Assert.IsTrue(stringKey != null);

                stringKey = null;

                Assert.IsTrue(stringKey == null);
                Assert.IsFalse(stringKey != null);
            }

            {
                ShortKey<AllTypes>? shortKey = ShortKey<AllTypes>.ValueOf(1);

                if(shortKey == null) { }
                if(shortKey != null) { }

                Assert.IsFalse(shortKey == null);
                Assert.IsTrue(shortKey != null);

                shortKey = null;

                Assert.IsTrue(shortKey == null);
                Assert.IsFalse(shortKey != null);
            }

            {
                IntKey<AllTypes>? intKey = IntKey<AllTypes>.ValueOf(1);

                Assert.IsFalse(intKey == null);
                Assert.IsTrue(intKey != null);

                intKey = null;

                Assert.IsTrue(intKey == null);
                Assert.IsFalse(intKey != null);
            }

            {
                LongKey<AllTypes>? longKey = LongKey<AllTypes>.ValueOf(1);

                Assert.IsFalse(longKey == null);
                Assert.IsTrue(longKey != null);

                longKey = null;

                Assert.IsTrue(longKey == null);
                Assert.IsFalse(longKey != null);
            }

            {
                BoolValue<AllTypes>? boolValue = BoolValue<AllTypes>.ValueOf(true);

                Assert.IsFalse(boolValue == null);
                Assert.IsTrue(boolValue != null);

                boolValue = null;

                Assert.IsTrue(boolValue == null);
                Assert.IsFalse(boolValue != null);
            }
        }

        [TestMethod]
        public void JsonSerializationTest() {

            {
                Guid guid = new Guid("0a0d021f-4385-4a33-b7cf-cb5dd14bedbe");

                GuidKey<AllTypes> key1 = GuidKey<AllTypes>.ValueOf(guid);

                Assert.AreEqual(key1.Value, guid);

                string json = JsonSerializer.Serialize(key1);

                GuidKey<AllTypes> key2 = JsonSerializer.Deserialize<GuidKey<AllTypes>>(json);

                Assert.AreEqual(key1, key2);
            }

            {
                string str = "jkadkjfahdf";

                StringKey<AllTypes> key1 = StringKey<AllTypes>.ValueOf(str);

                Assert.AreEqual(key1.Value, str);

                string json = JsonSerializer.Serialize(key1);

                StringKey<AllTypes> key2 = JsonSerializer.Deserialize<StringKey<AllTypes>>(json);

                Assert.AreEqual(key1, key2);
            }

            {
                short shortValue = 123;

                ShortKey<AllTypes> key1 = ShortKey<AllTypes>.ValueOf(shortValue);

                Assert.AreEqual(key1.Value, shortValue);

                string json = JsonSerializer.Serialize(key1);

                ShortKey<AllTypes> key2 = JsonSerializer.Deserialize<ShortKey<AllTypes>>(json);

                Assert.AreEqual(key1, key2);
            }

            {
                int intValue = 123456789;

                IntKey<AllTypes> key1 = IntKey<AllTypes>.ValueOf(intValue);

                Assert.AreEqual(key1.Value, intValue);

                string json = JsonSerializer.Serialize(key1);

                IntKey<AllTypes> key2 = JsonSerializer.Deserialize<IntKey<AllTypes>>(json);

                Assert.AreEqual(key1, key2);
            }

            {
                long longValue = 1234567892123122;

                LongKey<AllTypes> key1 = LongKey<AllTypes>.ValueOf(longValue);

                Assert.AreEqual(key1.Value, longValue);

                string json = JsonSerializer.Serialize(key1);

                LongKey<AllTypes> key2 = JsonSerializer.Deserialize<LongKey<AllTypes>>(json);

                Assert.AreEqual(key1, key2);
            }

            {
                bool boolValue = true;

                BoolValue<AllTypes> key1 = BoolValue<AllTypes>.ValueOf(boolValue);

                Assert.AreEqual(key1.Value, boolValue);

                string json = JsonSerializer.Serialize(key1);

                BoolValue<AllTypes> key2 = JsonSerializer.Deserialize<BoolValue<AllTypes>>(json);

                Assert.AreEqual(key1, key2);
            }
            return;
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

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            var result = Query
                .Select(
                    result => new {
                        AllTypesRow = new AllTypesInfo(result, allTypesTable)
                    }
                )
                .From(allTypesTable)
                .Where(allTypesTable.Id == allTypes.Id)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            AssertRow(result.Rows[0].AllTypesRow, allTypes);
        }

        private static async Task AssertRowExistsAsync(AllTypes allTypes) {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            var result = await Query
                .Select(
                    result => new {
                        AllTypesRow = new AllTypesInfo(result, allTypesTable)
                    }
                )
                .From(allTypesTable)
                .Where(allTypesTable.Id == allTypes.Id)
                .ExecuteAsync(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            AssertRow(result.Rows[0].AllTypesRow, allTypes);
        }

        private static void AssertRowExists(AllTypes allTypes, Transaction transaction) {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            var result = Query
                .Select(
                    result => new {
                        AllTypesRow = new AllTypesInfo(result, allTypesTable)
                    }
                )
                .From(allTypesTable)
                .Where(allTypesTable.Id == allTypes.Id)
                .Execute(transaction);

            Assert.AreEqual(1, result.Rows.Count);

            AssertRow(result.Rows[0].AllTypesRow, allTypes);
        }

        private static void AssertRowDoesNotExists(AllTypes allTypes) {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            var result = Query
                .Select(
                    result => new {
                        AllTypesRow = new AllTypesInfo(result, allTypesTable)
                    }
                )
                .From(allTypesTable)
                .Where(allTypesTable.Id == allTypes.Id)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(0, result.Rows.Count);
        }

        private static void AssertRowDoesNotExists(AllTypes allTypes, Transaction transaction) {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            var result = Query
                .Select(
                    result => new {
                        AllTypesRow = new AllTypesInfo(result, allTypesTable)
                    }
                )
                .From(allTypesTable)
                .Where(allTypesTable.Id == allTypes.Id)
                .Execute(transaction);

            Assert.AreEqual(0, result.Rows.Count);
        }

        public static void AssertRow(AllTypesInfo row, AllTypes allTypes) {

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

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            AllFieldsTest.JoinQuery(allTypes1);
            AllFieldsTest.JoinQuery(allTypes2);
            AllFieldsTest.JoinQuery(allTypes3);

            AllFieldsTest.DeleteWithQueryAndRollback(allTypes1);
            AllFieldsTest.DeleteWithQueryAndRollback(allTypes2);
            AllFieldsTest.DeleteWithQueryAndRollback(allTypes3);

            AllFieldsTest.UpdateWithQuery(allTypes1);
            AllFieldsTest.UpdateWithQuery(allTypes2);
            AllFieldsTest.UpdateWithQuery(allTypes3);

            AllFieldsTest.UpdateWithQueryAndRollback(allTypes1);
            AllFieldsTest.UpdateWithQueryAndRollback(allTypes2);
            AllFieldsTest.UpdateWithQueryAndRollback(allTypes3);

            AllFieldsTest.DeleteWithQuery(allTypes1);
            AllFieldsTest.DeleteWithQuery(allTypes2);
            AllFieldsTest.DeleteWithQueryReturning(allTypes3);

            AllFieldsTest.InsertWithQueryAndRollback(AllFieldsTest.GetAllTypes1());
            AllFieldsTest.InsertWithQueryAndRollback(AllFieldsTest.GetAllTypes1());
            AllFieldsTest.InsertWithQueryAndRollback(AllFieldsTest.GetAllTypes1());
        }

        private static void BasicInsertAndTruncateWithQueries() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            AllFieldsTest.Truncate();
        }

        private static async Task BasicInsertAndTruncateWithQueriesAsync() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            await AllFieldsTest.TruncateAsync();
        }

        private static async Task BasicInsertUpdateAndDeleteWithQueriesAsync() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            await AllFieldsTest.JoinQueryAsync(allTypes1);
            await AllFieldsTest.JoinQueryAsync(allTypes2);
            await AllFieldsTest.JoinQueryAsync(allTypes3);

            AllFieldsTest.DeleteWithQueryAndRollback(allTypes1);
            AllFieldsTest.DeleteWithQueryAndRollback(allTypes2);
            AllFieldsTest.DeleteWithQueryAndRollback(allTypes3);

            await AllFieldsTest.UpdateWithQueryAsync(allTypes1);
            await AllFieldsTest.UpdateWithQueryAsync(allTypes2);
            await AllFieldsTest.UpdateWithQueryAsync(allTypes3);

            AllFieldsTest.UpdateWithQueryAndRollback(allTypes1);
            AllFieldsTest.UpdateWithQueryAndRollback(allTypes2);
            AllFieldsTest.UpdateWithQueryAndRollback(allTypes3);

            await AllFieldsTest.DeleteWithQueryAsync(allTypes1);
            await AllFieldsTest.DeleteWithQueryAsync(allTypes2);
            await AllFieldsTest.DeleteWithQueryReturningAsync(allTypes3);

            AllFieldsTest.InsertWithQueryAndRollback(AllFieldsTest.GetAllTypes1());
            AllFieldsTest.InsertWithQueryAndRollback(AllFieldsTest.GetAllTypes1());
            AllFieldsTest.InsertWithQueryAndRollback(AllFieldsTest.GetAllTypes1());
        }

        private static async Task BasicInsertAndDeleteJoinQueriesSqlServerAsync() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            AllTypesTable allTypesTable = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = await Query
                    .Delete(allTypesTable)
                    .From(allTypesTable2)
                    .Where(allTypesTable.Id == allTypesTable2.Id & allTypesTable.Id == allTypes1.Id)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {
                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = await Query
                    .Select(result => result.Get(count))
                    .From(allTypesTable)
                    .Where(allTypesTable.Id == allTypesTable.Id)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(2, countValue!.Value);  //There should be one record left
            }

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await Query
                    .Delete(allTypesTable)
                    .From(allTypesTable2)
                    .Where(allTypesTable.Id == allTypesTable2.Id & allTypesTable.Id == allTypes3.Id)
                    .ExecuteAsync(
                        deleted => new AllTypesInfo(deleted, allTypesTable),
                        transaction
                   );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes3);

                transaction.Commit();
            }

            {
                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = await Query
                    .Select(result => result.Get(count))
                    .From(allTypesTable)
                    .Where(allTypesTable.Id == allTypesTable.Id)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(1, countValue!.Value);
            }
        }

        private static async Task BasicInsertAndDeleteJoinQueriesPostgreSqlAsync() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            AllTypesTable allTypesTable = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = await Query
                    .Delete(allTypesTable)
                    .Using(allTypesTable2)
                    .Where(allTypesTable.Id == allTypesTable2.Id & allTypesTable.Id == allTypes1.Id)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(1, result.RowsEffected);

                result = await Query
                    .Delete(allTypesTable)
                    .Using(allTypesTable2)
                    .Where(allTypesTable.Id == allTypesTable2.Id & allTypesTable.Id == allTypes2.Id)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {
                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = await Query
                    .Select(result => result.Get(count))
                    .From(allTypesTable)
                    .Where(allTypesTable.Id == allTypesTable.Id)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(1, countValue!.Value);  //There should be one record left
            }

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await Query
                    .Delete(allTypesTable)
                    .Using(allTypesTable2)
                    .Where(allTypesTable.Id == allTypesTable2.Id & allTypesTable.Id == allTypes3.Id)
                    .ExecuteAsync(
                        deleted => new AllTypesInfo(deleted, allTypesTable),
                        transaction
                   );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes3);

                transaction.Commit();
            }

            {
                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = await Query
                    .Select(result => result.Get(count))
                    .From(allTypesTable)
                    .Where(allTypesTable.Id == allTypesTable.Id)
                    .ExecuteAsync(TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
        }

        private static void InsertWithQuery(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable table = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = Query.Insert(table)
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
                        result => new AllTypesInfo(result, table),
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                transaction.Commit();

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(1, result.RowsEffected);

                allTypes.Id = result.Rows[0].Id;

                AssertRow(result.Rows[0], allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                AllFieldsTest.AssertRowExists(allTypes);
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

                AllTypesTable table = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = await Query.Insert(table)
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
                    .ExecuteAsync(
                        result => new AllTypesInfo(result, table),
                        transaction,
                        cancellationToken: null,
                        TimeoutLevel.ShortInsert
                    );

                transaction.Commit();

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(1, result.RowsEffected);

                allTypes.Id = result.Rows[0].Id;

                AssertRow(result.Rows[0], allTypes);

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

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable allTypesTable = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = Query
                    .Update(allTypesTable)
                    .Values(values => values
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
                        .Set(allTypesTable.TimeOnly, allTypes.TimeOnly)
                    )
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(
                        result => new AllTypesInfo(result, allTypesTable),
                        transaction,
                        TimeoutLevel.ShortUpdate
                    );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes);

                transaction.Commit();

                AllFieldsTest.AssertRowExists(allTypes);
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

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable allTypesTable = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = await Query
                    .Update(allTypesTable)
                    .Values(values => values
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
                        .Set(allTypesTable.TimeOnly, allTypes.TimeOnly)
                    )
                    .Where(allTypesTable.Id == allTypes.Id)
                    .ExecuteAsync(
                        result => new AllTypesInfo(result, allTypesTable),
                        transaction,
                        cancellationToken: null,
                        TimeoutLevel.ShortUpdate
                    );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes);

                transaction.Commit();

                await AssertRowExistsAsync(allTypes);
            }
        }

        private static void DeleteWithQuery(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = AllFieldsTest.GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(transaction, timeout: TimeoutLevel.ShortDelete);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {
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
            Assert.AreEqual(beginRowCount, AllFieldsTest.GetNumberOfRows() + 1);
        }

        private static void DeleteWithQueryReturning(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = AllFieldsTest.GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(
                        result => new AllTypesInfo(result, allTypesTable),
                        transaction,
                        TimeoutLevel.ShortDelete
                    );

                Assert.AreEqual(1, result.RowsEffected);

                AllTypesInfo row = result.Rows.First();

                AssertRow(row, allTypes);

                transaction.Commit();
            }

            {
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
            Assert.AreEqual(beginRowCount, AllFieldsTest.GetNumberOfRows() + 1);
        }

        private static async Task DeleteWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await AllFieldsTest.GetNumberOfRowsAsync();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = await Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {
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
            Assert.AreEqual(beginRowCount, await AllFieldsTest.GetNumberOfRowsAsync() + 1);
        }

        private static async Task DeleteWithQueryReturningAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await AllFieldsTest.GetNumberOfRowsAsync();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .ExecuteAsync(
                        result => new AllTypesInfo(result, allTypesTable),
                        transaction
                    );

                Assert.AreEqual(1, result.RowsEffected);

                AllTypesInfo row = result.Rows.First();

                AssertRow(row, allTypes);

                transaction.Commit();
            }

            {
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
            Assert.AreEqual(beginRowCount, await AllFieldsTest.GetNumberOfRowsAsync() + 1);
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

        private static void JoinQuery(AllTypes allTypes) {

            AllTypesTable allTypesTable1 = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;
            AllTypesTable allTypesTable4 = AllTypesTable.Instance4;

            var result = Query
                .Select(
                    result => new {
                        AllTypesRow1 = new AllTypesInfo(result, allTypesTable1),
                        AllTypesRow2 = new AllTypesInfo(result, allTypesTable2),
                        AllTypesRow3 = new AllTypesInfo(result, allTypesTable3),
                        AllTypesRow4 = new AllTypesInfo(result, allTypesTable4),
                    }
                )
                .From(allTypesTable1)
                .With(SqlServerTableHint.UPDLOCK, SqlServerTableHint.SERIALIZABLE)
                .Join(allTypesTable2).On(allTypesTable1.Id == allTypesTable2.Id)
                .Join(allTypesTable3).On(allTypesTable2.Id == allTypesTable3.Id)
                .LeftJoin(allTypesTable4).On(allTypesTable4.Id == new IntKey<AllTypes>(928756923))
                .Where(allTypesTable1.Id == allTypes.Id)
                .Option(labelName: "Label 1", SqlServerQueryOption.FORCE_ORDER)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            AllTypesInfo row1 = result.Rows[0].AllTypesRow1;
            AllTypesInfo row2 = result.Rows[0].AllTypesRow2;
            AllTypesInfo row3 = result.Rows[0].AllTypesRow3;
            AllTypesInfo row4 = result.Rows[0].AllTypesRow4;

            AssertRow(row1, allTypes);
            AssertRow(row2, allTypes);
            AssertRow(row3, allTypes);
            Assert.IsFalse(row4.Id.IsValid);
        }

        private static async Task JoinQueryAsync(AllTypes allTypes) {

            AllTypesTable allTypesTable1 = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;

            var result = await Query
                .Select(
                    result => new {
                        AllTypesRow1 = new AllTypesInfo(result, allTypesTable1),
                        AllTypesRow2 = new AllTypesInfo(result, allTypesTable2),
                        AllTypesRow3 = new AllTypesInfo(result, allTypesTable3)
                    }
                )
                .From(allTypesTable1)
                .Join(allTypesTable2).On(allTypesTable1.Id == allTypesTable2.Id)
                .Join(allTypesTable3).On(allTypesTable2.Id == allTypesTable3.Id)
                .Where(allTypesTable1.Id == allTypes.Id)
                .ExecuteAsync(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            AllTypesInfo row1 = result.Rows[0].AllTypesRow1;
            AllTypesInfo row2 = result.Rows[0].AllTypesRow2;
            AllTypesInfo row3 = result.Rows[0].AllTypesRow3;

            AssertRow(row1, allTypes);
            AssertRow(row2, allTypes);
            AssertRow(row3, allTypes);
        }

        private static void InsertWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable table = AllTypesTable.Instance;

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
                            AllTypesRow = new AllTypesInfo(result, table)
                        },
                        transaction
                    );

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(1, result.RowsEffected);

                allTypes.Id = result.Rows[0].AllTypesRow.Id;

                AssertRow(result.Rows[0].AllTypesRow, allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                AllFieldsTest.AssertRowExists(allTypes, transaction);

                transaction.Rollback();

                AllFieldsTest.AssertRowDoesNotExists(allTypes);

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

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable allTypesTable = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = Query
                    .Update(allTypesTable)
                    .Values(values => values
                        .Set(allTypesTable.Guid, newAllTypes.Guid)
                        .Set(allTypesTable.String, newAllTypes.String)
                        .Set(allTypesTable.SmallInt, newAllTypes.SmallInt)
                        .Set(allTypesTable.Int, newAllTypes.Int)
                        .Set(allTypesTable.BigInt, newAllTypes.BigInt)
                        .Set(allTypesTable.Decimal, newAllTypes.Decimal)
                        .Set(allTypesTable.Float, newAllTypes.Float)
                        .Set(allTypesTable.Double, newAllTypes.Double)
                        .Set(allTypesTable.Boolean, newAllTypes.Boolean)
                        .Set(allTypesTable.Bytes, newAllTypes.Bytes)
                        .Set(allTypesTable.DateTime, newAllTypes.DateTime)
                        .Set(allTypesTable.DateTimeOffset, newAllTypes.DateTimeOffset)
                        .Set(allTypesTable.Enum, newAllTypes.Enum)
                        .Set(allTypesTable.DateOnly, newAllTypes.DateOnly)
                        .Set(allTypesTable.TimeOnly, newAllTypes.TimeOnly)
                    )
                    .Where(allTypesTable.Id == newAllTypes.Id)
                    .Execute(
                        updated => new AllTypesInfo(updated, allTypesTable),
                        transaction
                    );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], newAllTypes);
                AllFieldsTest.AssertRowExists(newAllTypes, transaction);

                transaction.Rollback();
                AllFieldsTest.AssertRowExists(initialAllTypes);   //Assert initial values
            }
        }

        private static void DeleteWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = AllFieldsTest.GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                Assert.AreEqual(1, result.RowsEffected);

                AllFieldsTest.AssertRowDoesNotExists(allTypes, transaction);

                transaction.Rollback();

                AllFieldsTest.AssertRowExists(allTypes);
            }
            Assert.AreEqual(beginRowCount, AllFieldsTest.GetNumberOfRows());
        }


        /*
         * Test the update join syntax
         */
        private static void UpdateJoinTest() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                QueryResult<AllTypesInfo> result = Query
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, allTypes1.Guid)
                        .Set(tableA.String, allTypes1.String)
                        .Set(tableA.SmallInt, allTypes1.SmallInt)
                        .Set(tableA.Int, allTypes1.Int)
                        .Set(tableA.BigInt, allTypes1.BigInt)
                        .Set(tableA.Decimal, allTypes1.Decimal)
                        .Set(tableA.Float, allTypes1.Float)
                        .Set(tableA.Double, allTypes1.Double)
                        .Set(tableA.Boolean, allTypes1.Boolean)
                        .Set(tableA.Bytes, allTypes1.Bytes)
                        .Set(tableA.DateTime, allTypes1.DateTime)
                        .Set(tableA.DateTimeOffset, allTypes1.DateTimeOffset)
                        .Set(tableA.Enum, allTypes1.Enum)
                        .Set(tableA.DateOnly, allTypes1.DateOnly)
                        .Set(tableA.TimeOnly, allTypes1.TimeOnly)
                    )
                    .From(tableB)
                    .Where(tableA.BigInt == tableB.BigInt & tableA.Id != allTypes1.Id & tableB.Id == allTypes1.Id)   //Update allTypes2 and AllTypes3
                    .Execute(
                        updated => new AllTypesInfo(updated, tableA),
                        transaction
                    );

                Assert.AreEqual(2, result.RowsEffected);
                Assert.AreEqual(2, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AllFieldsTest.AssertRowExists(allTypes1, transaction);

                allTypes2.Guid = allTypes1.Guid;
                allTypes2.String = allTypes1.String;
                allTypes2.SmallInt = allTypes1.SmallInt;
                allTypes2.Int = allTypes1.Int;
                allTypes2.BigInt = allTypes1.BigInt;
                allTypes2.Decimal = allTypes1.Decimal;
                allTypes2.Float = allTypes1.Float;
                allTypes2.Double = allTypes1.Double;
                allTypes2.Boolean = allTypes1.Boolean;
                allTypes2.Bytes = allTypes1.Bytes;
                allTypes2.DateTime = allTypes1.DateTime;
                allTypes2.DateTimeOffset = allTypes1.DateTimeOffset;
                allTypes2.Enum = allTypes1.Enum;
                allTypes2.DateOnly = allTypes1.DateOnly;
                allTypes2.TimeOnly = allTypes1.TimeOnly;

                AllFieldsTest.AssertRowExists(allTypes2, transaction);

                allTypes3.Guid = allTypes1.Guid;
                allTypes3.String = allTypes1.String;
                allTypes3.SmallInt = allTypes1.SmallInt;
                allTypes3.Int = allTypes1.Int;
                allTypes3.BigInt = allTypes1.BigInt;
                allTypes3.Decimal = allTypes1.Decimal;
                allTypes3.Float = allTypes1.Float;
                allTypes3.Double = allTypes1.Double;
                allTypes3.Boolean = allTypes1.Boolean;
                allTypes3.Bytes = allTypes1.Bytes;
                allTypes3.DateTime = allTypes1.DateTime;
                allTypes3.DateTimeOffset = allTypes1.DateTimeOffset;
                allTypes3.Enum = allTypes1.Enum;
                allTypes3.DateOnly = allTypes1.DateOnly;
                allTypes3.TimeOnly = allTypes1.TimeOnly;

                AllFieldsTest.AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }

        /*
         * Test the update join syntax
         */
        private static async Task UpdateJoinTestAsync() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                QueryResult<AllTypesInfo> result = await Query
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, allTypes1.Guid)
                        .Set(tableA.String, allTypes1.String)
                        .Set(tableA.SmallInt, allTypes1.SmallInt)
                        .Set(tableA.Int, allTypes1.Int)
                        .Set(tableA.BigInt, allTypes1.BigInt)
                        .Set(tableA.Decimal, allTypes1.Decimal)
                        .Set(tableA.Float, allTypes1.Float)
                        .Set(tableA.Double, allTypes1.Double)
                        .Set(tableA.Boolean, allTypes1.Boolean)
                        .Set(tableA.Bytes, allTypes1.Bytes)
                        .Set(tableA.DateTime, allTypes1.DateTime)
                        .Set(tableA.DateTimeOffset, allTypes1.DateTimeOffset)
                        .Set(tableA.Enum, allTypes1.Enum)
                        .Set(tableA.DateOnly, allTypes1.DateOnly)
                        .Set(tableA.TimeOnly, allTypes1.TimeOnly)
                    )
                    .From(tableB)
                    .Where(tableA.BigInt == tableB.BigInt & tableA.Id != allTypes1.Id & tableB.Id == allTypes1.Id)   //Update allTypes2 and AllTypes3
                    .ExecuteAsync(
                        updated => new AllTypesInfo(updated, tableA),
                        transaction
                    );

                Assert.AreEqual(2, result.RowsEffected);
                Assert.AreEqual(2, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AllFieldsTest.AssertRowExists(allTypes1, transaction);

                allTypes2.Guid = allTypes1.Guid;
                allTypes2.String = allTypes1.String;
                allTypes2.SmallInt = allTypes1.SmallInt;
                allTypes2.Int = allTypes1.Int;
                allTypes2.BigInt = allTypes1.BigInt;
                allTypes2.Decimal = allTypes1.Decimal;
                allTypes2.Float = allTypes1.Float;
                allTypes2.Double = allTypes1.Double;
                allTypes2.Boolean = allTypes1.Boolean;
                allTypes2.Bytes = allTypes1.Bytes;
                allTypes2.DateTime = allTypes1.DateTime;
                allTypes2.DateTimeOffset = allTypes1.DateTimeOffset;
                allTypes2.Enum = allTypes1.Enum;
                allTypes2.DateOnly = allTypes1.DateOnly;
                allTypes2.TimeOnly = allTypes1.TimeOnly;

                AllFieldsTest.AssertRowExists(allTypes2, transaction);

                allTypes3.Guid = allTypes1.Guid;
                allTypes3.String = allTypes1.String;
                allTypes3.SmallInt = allTypes1.SmallInt;
                allTypes3.Int = allTypes1.Int;
                allTypes3.BigInt = allTypes1.BigInt;
                allTypes3.Decimal = allTypes1.Decimal;
                allTypes3.Float = allTypes1.Float;
                allTypes3.Double = allTypes1.Double;
                allTypes3.Boolean = allTypes1.Boolean;
                allTypes3.Bytes = allTypes1.Bytes;
                allTypes3.DateTime = allTypes1.DateTime;
                allTypes3.DateTimeOffset = allTypes1.DateTimeOffset;
                allTypes3.Enum = allTypes1.Enum;
                allTypes3.DateOnly = allTypes1.DateOnly;
                allTypes3.TimeOnly = allTypes1.TimeOnly;

                AllFieldsTest.AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }

        /*
         * Test the update join syntax
         */
        private static void UpdateJoinTest2() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                QueryResult<AllTypesInfo> result = Query
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, allTypes1.Guid)
                        .Set(tableA.String, allTypes1.String)
                        .Set(tableA.SmallInt, allTypes1.SmallInt)
                        .Set(tableA.Int, allTypes1.Int)
                        .Set(tableA.BigInt, allTypes1.BigInt)
                        .Set(tableA.Decimal, allTypes1.Decimal)
                        .Set(tableA.Float, allTypes1.Float)
                        .Set(tableA.Double, allTypes1.Double)
                        .Set(tableA.Boolean, allTypes1.Boolean)
                        .Set(tableA.Bytes, allTypes1.Bytes)
                        .Set(tableA.DateTime, allTypes1.DateTime)
                        .Set(tableA.DateTimeOffset, allTypes1.DateTimeOffset)
                        .Set(tableA.Enum, allTypes1.Enum)
                        .Set(tableA.DateOnly, allTypes1.DateOnly)
                        .Set(tableA.TimeOnly, allTypes1.TimeOnly)
                    )
                    .From(tableB)
                    .Where(tableA.BigInt == tableB.BigInt & tableA.Id != allTypes1.Id & tableB.Id == allTypes1.Id & tableA.Id != allTypes3.Id)   //Only update allTypes2
                    .Execute(
                        updated => new AllTypesInfo(updated, tableA),
                        transaction
                    );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AllFieldsTest.AssertRowExists(allTypes1, transaction);

                allTypes2.Guid = allTypes1.Guid;
                allTypes2.String = allTypes1.String;
                allTypes2.SmallInt = allTypes1.SmallInt;
                allTypes2.Int = allTypes1.Int;
                allTypes2.BigInt = allTypes1.BigInt;
                allTypes2.Decimal = allTypes1.Decimal;
                allTypes2.Float = allTypes1.Float;
                allTypes2.Double = allTypes1.Double;
                allTypes2.Boolean = allTypes1.Boolean;
                allTypes2.Bytes = allTypes1.Bytes;
                allTypes2.DateTime = allTypes1.DateTime;
                allTypes2.DateTimeOffset = allTypes1.DateTimeOffset;
                allTypes2.Enum = allTypes1.Enum;
                allTypes2.DateOnly = allTypes1.DateOnly;
                allTypes2.TimeOnly = allTypes1.TimeOnly;

                AllFieldsTest.AssertRowExists(allTypes2, transaction);
                AllFieldsTest.AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }

        /*
         * Test the update join syntax
         */
        private static async Task UpdateJoinTestAsync2() {

            AllTypes allTypes1 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            AllFieldsTest.InsertWithQuery(allTypes1);
            AllFieldsTest.InsertWithQuery(allTypes2);
            AllFieldsTest.InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                QueryResult<AllTypesInfo> result = await Query
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, allTypes1.Guid)
                        .Set(tableA.String, allTypes1.String)
                        .Set(tableA.SmallInt, allTypes1.SmallInt)
                        .Set(tableA.Int, allTypes1.Int)
                        .Set(tableA.BigInt, allTypes1.BigInt)
                        .Set(tableA.Decimal, allTypes1.Decimal)
                        .Set(tableA.Float, allTypes1.Float)
                        .Set(tableA.Double, allTypes1.Double)
                        .Set(tableA.Boolean, allTypes1.Boolean)
                        .Set(tableA.Bytes, allTypes1.Bytes)
                        .Set(tableA.DateTime, allTypes1.DateTime)
                        .Set(tableA.DateTimeOffset, allTypes1.DateTimeOffset)
                        .Set(tableA.Enum, allTypes1.Enum)
                        .Set(tableA.DateOnly, allTypes1.DateOnly)
                        .Set(tableA.TimeOnly, allTypes1.TimeOnly)
                    )
                    .From(tableB)
                    .Where(tableA.BigInt == tableB.BigInt & tableA.Id != allTypes1.Id & tableB.Id == allTypes1.Id & tableA.Id != allTypes3.Id)   //Only update allTypes2
                    .ExecuteAsync(
                        updated => new AllTypesInfo(updated, tableA),
                        transaction
                    );

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AllFieldsTest.AssertRowExists(allTypes1, transaction);

                allTypes2.Guid = allTypes1.Guid;
                allTypes2.String = allTypes1.String;
                allTypes2.SmallInt = allTypes1.SmallInt;
                allTypes2.Int = allTypes1.Int;
                allTypes2.BigInt = allTypes1.BigInt;
                allTypes2.Decimal = allTypes1.Decimal;
                allTypes2.Float = allTypes1.Float;
                allTypes2.Double = allTypes1.Double;
                allTypes2.Boolean = allTypes1.Boolean;
                allTypes2.Bytes = allTypes1.Bytes;
                allTypes2.DateTime = allTypes1.DateTime;
                allTypes2.DateTimeOffset = allTypes1.DateTimeOffset;
                allTypes2.Enum = allTypes1.Enum;
                allTypes2.DateOnly = allTypes1.DateOnly;
                allTypes2.TimeOnly = allTypes1.TimeOnly;

                AllFieldsTest.AssertRowExists(allTypes2, transaction);
                AllFieldsTest.AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }
    }
}