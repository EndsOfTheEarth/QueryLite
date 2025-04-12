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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class AllFieldsPreparedTest {

        private static readonly IPreparedDeleteQuery<bool> _deleteAllQuery = Query
                .Prepare<bool>()
                .Delete(AllTypesTable.Instance)
                .NoWhereCondition()
                .Build();

        [TestInitialize]
        public void ClearTable() {

            InitQueries();

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                _deleteAllQuery.Execute(parameters: true, transaction);

                QueryResult<int> result = query1!.Execute(parameters: true, transaction);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int countValue = result.Rows[0];

                Assert.AreEqual(0, countValue);

                transaction.Commit();
            }
        }

        private IPreparedQueryExecute<bool, int>? query1;
        private IPreparedQueryExecute<AllTypes, AllTypesInfo>? _selectAllTypesQuery;
        private IPreparedQueryExecute<bool, int>? _selectAllCountQuery;
        private IPreparedQueryExecute<AllTypes, int>? _selectAllTypesCountQuery;

        private IPreparedDeleteQuery<AllTypes>? _deleteQuery1;
        private IPreparedDeleteQuery<IntKey<AllTypes>, AllTypesInfo>? _deleteQuery4;

        public void InitQueries() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

            {

                query1 = Query
                    .Prepare<bool>()
                    .Select(row => row.Get(count))
                    .From(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable.Id))
                    .Build();
            }

            {

                _selectAllTypesQuery = Query
                   .Prepare<AllTypes>()
                   .Select(
                       row => new AllTypesInfo(row, allTypesTable)
                   )
                   .From(allTypesTable)
                   .Where(where => where.EQUALS(allTypesTable.Id, (info) => info.Id))
                   .Build();
            }

            {

                _selectAllCountQuery = Query
                    .Prepare<bool>()
                    .Select(row => row.Get(count))
                    .From(allTypesTable)
                    .Build();
            }
            {

                _selectAllTypesCountQuery = Query
                    .Prepare<AllTypes>()
                    .Select(row => row.Get(count))
                    .From(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, (info) => info.Id))
                    .Build();
            }

            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

            _deleteQuery1 = Query
                .Prepare<AllTypes>()
                .Delete(allTypesTable)
                .From(allTypesTable2)
                .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable2.Id) & where.EQUALS(allTypesTable.Id, info => info.Id))
                .Build();

            _deleteQuery4 = Query
                .Prepare<IntKey<AllTypes>>()
                .Delete(allTypesTable)
                .From(allTypesTable2)
                .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable2.Id) & where.EQUALS(allTypesTable.Id, id => id))
                .Build(deleted => new AllTypesInfo(deleted, allTypesTable));
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

        [TestMethod]
        public void LoadDocumentation() {

            string doc = DocumentationGenerator.GenerateForAssembly([Assembly.GetExecutingAssembly()], applicationName: "Auto Tester", version: "v1.0");
            Assert.IsNotEmpty(doc);
        }

        [TestMethod]
        public void UpdateJoinTests() {
            UpdateJoinTest();
        }

        [TestMethod]
        public void UpdateJoinTests2() {
            UpdateJoinTest2();
        }

        [TestMethod]
        public async Task UpdateJoinTestsAsync() {
            await UpdateJoinTestAsync();
        }
        [TestMethod]
        public async Task UpdateJoinTests2Async() {
            await UpdateJoinTest2Async();
        }

        [TestMethod]
        public void RunSchemaValidator() {

            SchemaValidationSettings settings = new SchemaValidationSettings() {
                ValidatePrimaryKeys = true,
                ValidateUniqueConstraints = true,
                ValidateForeignKeys = true,
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
                await BasicInsertAndDeleteJoinQueriesPostgreSqlAsync();
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

        private static readonly IPreparedInsertQuery<AllTypes> _insertQueryWithoutReturning = Query
            .Prepare<AllTypes>()
            .Insert(AllTypesTable.Instance)
            .Values(values => values
                .Set(AllTypesTable.Instance.Guid, (info) => info.Guid)
                .Set(AllTypesTable.Instance.String, (info) => info.String)
                .Set(AllTypesTable.Instance.SmallInt, (info) => info.SmallInt)
                .Set(AllTypesTable.Instance.Int, (info) => info.Int)
                .Set(AllTypesTable.Instance.BigInt, (info) => info.BigInt)
                .Set(AllTypesTable.Instance.Decimal, (info) => info.Decimal)
                .Set(AllTypesTable.Instance.Float, (info) => info.Float)
                .Set(AllTypesTable.Instance.Double, (info) => info.Double)
                .Set(AllTypesTable.Instance.Boolean, (info) => info.Boolean)
                .Set(AllTypesTable.Instance.Bytes, (info) => info.Bytes)
                .Set(AllTypesTable.Instance.DateTime, (info) => info.DateTime)
                .Set(AllTypesTable.Instance.DateTimeOffset, (info) => info.DateTimeOffset)
                .Set(AllTypesTable.Instance.Enum, (info) => info.Enum)
                .Set(AllTypesTable.Instance.DateOnly, (info) => info.DateOnly)
                .Set(AllTypesTable.Instance.TimeOnly, (info) => info.TimeOnly)
            )
            .Build();

        [TestMethod]
        public void TestInsertWithoutReturning() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();

            using Transaction transaction = new Transaction(TestDatabase.Database);

            NonQueryResult insertResult = _insertQueryWithoutReturning.Execute(allTypes1, transaction);

            Assert.AreEqual(1, insertResult.RowsEffected);

            transaction.Commit();

            AllTypesTable table = AllTypesTable.Instance;

            QueryResult<AllTypesInfo> queryResult = Query
                .Select(
                    row => new AllTypesInfo(row, table)
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, queryResult.Rows.Count);
            Assert.AreEqual(0, queryResult.RowsEffected);

            AllTypesInfo row = queryResult.Rows[0];
            allTypes1.Id = row.Id;

            AssertRow(row, allTypes1);
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

        private void AssertRowExists(AllTypes allTypes) {

            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameters: allTypes, TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            AssertRow(result.Rows[0], allTypes);
        }

        private async Task AssertRowExistsAsync(AllTypes allTypes) {

            QueryResult<AllTypesInfo> result = await _selectAllTypesQuery!.ExecuteAsync(parameters: allTypes, TestDatabase.Database, cancellationToken: CancellationToken.None);

            Assert.AreEqual(1, result.Rows.Count);

            AssertRow(result.Rows[0], allTypes);
        }

        private void AssertRowExists(AllTypes allTypes, Transaction transaction) {


            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameters: allTypes, transaction);

            Assert.AreEqual(1, result.Rows.Count);

            AssertRow(result.Rows[0], allTypes);
        }

        private void AssertRowDoesNotExists(AllTypes allTypes) {

            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameters: allTypes, TestDatabase.Database);

            Assert.AreEqual(0, result.Rows.Count);
        }

        private void AssertRowDoesNotExists(AllTypes allTypes, Transaction transaction) {


            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameters: allTypes, transaction);

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

        private void BasicInsertUpdateAndDeleteWithQueries() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

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
            DeleteWithQueryReturning(allTypes3);

            InsertWithQueryAndRollback(AllFieldsPreparedTest.GetAllTypes1());
            InsertWithQueryAndRollback(AllFieldsPreparedTest.GetAllTypes1());
            InsertWithQueryAndRollback(AllFieldsPreparedTest.GetAllTypes1());
        }

        private void BasicInsertAndTruncateWithQueries() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            Truncate();
        }

        private async Task BasicInsertAndTruncateWithQueriesAsync() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            await TruncateAsync();
        }

        private async Task BasicInsertUpdateAndDeleteWithQueriesAsync() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

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
            await DeleteWithQueryReturningAsync(allTypes3);

            InsertWithQueryAndRollback(AllFieldsPreparedTest.GetAllTypes1());
            InsertWithQueryAndRollback(AllFieldsPreparedTest.GetAllTypes1());
            InsertWithQueryAndRollback(AllFieldsPreparedTest.GetAllTypes1());
        }

        private async Task BasicInsertAndDeleteJoinQueriesSqlServerAsync() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = await _deleteQuery1!.ExecuteAsync(parameters: allTypes1, transaction);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameters: true, TestDatabase.Database, cancellationToken: CancellationToken.None);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(2, countValue!.Value);  //There should be two records left
            }

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await _deleteQuery4!.ExecuteAsync(parameters: allTypes3.Id, transaction);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes3);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllCountQuery.ExecuteAsync(parameters: true, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(1, countValue!.Value);
            }
        }

        private async Task BasicInsertAndDeleteJoinQueriesPostgreSqlAsync() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            AllTypesTable allTypesTable = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<AllTypes> deleteQuery5 = Query
                    .Prepare<AllTypes>()
                    .Delete(allTypesTable)
                    .Using(allTypesTable2)
                    .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable2.Id) & where.EQUALS(allTypesTable.Id, info => info.Id))
                    .Build();

                NonQueryResult result = await deleteQuery5.ExecuteAsync(parameters: allTypes1, transaction);

                Assert.AreEqual(1, result.RowsEffected);

                IPreparedDeleteQuery<AllTypes> deleteQuery6 = Query
                    .Prepare<AllTypes>()
                    .Delete(allTypesTable)
                    .Using(allTypesTable2)
                    .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable2.Id) & where.EQUALS(allTypesTable.Id, info => info.Id))
                    .Build();

                result = await deleteQuery6.ExecuteAsync(parameters: allTypes2, transaction);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameters: true, TestDatabase.Database, cancellationToken: CancellationToken.None);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(1, countValue!.Value);  //There should be one record left
            }

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<IntKey<AllTypes>, AllTypesInfo> deleteQuery7 = Query
                    .Prepare<IntKey<AllTypes>>()
                    .Delete(allTypesTable)
                    .Using(allTypesTable2)
                    .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable2.Id) & where.EQUALS(allTypesTable.Id, id => id))
                    .Build(deleted => new AllTypesInfo(deleted, allTypesTable));

                QueryResult<AllTypesInfo> result = await deleteQuery7.ExecuteAsync(parameters: allTypes3.Id, transaction);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes3);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllCountQuery.ExecuteAsync(parameters: true, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
        }

        private static readonly IPreparedInsertQuery<AllTypes, AllTypesInfo> _insertQuery1 = Query
            .Prepare<AllTypes>()
            .Insert(AllTypesTable.Instance)
            .Values(values => values
                .Set(AllTypesTable.Instance.Guid, (info) => info.Guid)
                .Set(AllTypesTable.Instance.String, (info) => info.String)
                .Set(AllTypesTable.Instance.SmallInt, (info) => info.SmallInt)
                .Set(AllTypesTable.Instance.Int, (info) => info.Int)
                .Set(AllTypesTable.Instance.BigInt, (info) => info.BigInt)
                .Set(AllTypesTable.Instance.Decimal, (info) => info.Decimal)
                .Set(AllTypesTable.Instance.Float, (info) => info.Float)
                .Set(AllTypesTable.Instance.Double, (info) => info.Double)
                .Set(AllTypesTable.Instance.Boolean, (info) => info.Boolean)
                .Set(AllTypesTable.Instance.Bytes, (info) => info.Bytes)
                .Set(AllTypesTable.Instance.DateTime, (info) => info.DateTime)
                .Set(AllTypesTable.Instance.DateTimeOffset, (info) => info.DateTimeOffset)
                .Set(AllTypesTable.Instance.Enum, (info) => info.Enum)
                .Set(AllTypesTable.Instance.DateOnly, (info) => info.DateOnly)
                .Set(AllTypesTable.Instance.TimeOnly, (info) => info.TimeOnly)
            )
            .Build(
                returning => new AllTypesInfo(returning, AllTypesTable.Instance)
            );

        private void InsertWithQuery(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = _insertQuery1
                    .Execute(
                        parameters: allTypes,
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                transaction.Commit();

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(1, result.RowsEffected);

                allTypes.Id = result.Rows[0].Id;

                AssertRow(result.Rows[0], allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                AssertRowExists(allTypes);
            }
        }

        private void Truncate() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult truncateResult = Query
                    .Truncate(allTypesTable)
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                transaction.Commit();
            }

            QueryResult<int> result = _selectAllCountQuery!.Execute(parameters: true, TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            Assert.AreEqual(0, result.Rows.First());
        }

        private async Task TruncateAsync() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult truncateResult = await Query
                    .Truncate(allTypesTable)
                    .ExecuteAsync(transaction);

                transaction.Commit();
            }

            QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameters: true, TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, result.Rows.Count);

            Assert.AreEqual(0, result.Rows.First());
        }

        public async Task InsertWithQueryAsync(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedInsertQuery<AllTypes, AllTypesInfo> insertQuery = Query
                .Prepare<AllTypes>()
                .Insert(table)
                .Values(values => values
                    .Set(table.Guid, info => info.Guid)
                    .Set(table.String, info => info.String)
                    .Set(table.SmallInt, info => info.SmallInt)
                    .Set(table.Int, info => info.Int)
                    .Set(table.BigInt, info => info.BigInt)
                    .Set(table.Decimal, info => info.Decimal)
                    .Set(table.Float, info => info.Float)
                    .Set(table.Double, info => info.Double)
                    .Set(table.Boolean, info => info.Boolean)
                    .Set(table.Bytes, info => info.Bytes)
                    .Set(table.DateTime, info => info.DateTime)
                    .Set(table.DateTimeOffset, info => info.DateTimeOffset)
                    .Set(table.Enum, info => info.Enum)
                    .Set(table.DateOnly, info => info.DateOnly)
                    .Set(table.TimeOnly, info => info.TimeOnly)
                )
                .Build(
                    returning => new AllTypesInfo(returning, table)
                );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await insertQuery
                    .ExecuteAsync(
                        parameters: allTypes,
                        transaction,
                        timeout: TimeoutLevel.ShortInsert
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

        private void UpdateWithQuery(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            allTypes.UpdateValues(
                guid: Guid.NewGuid(),
                @string: "-4at3=_)(*&_(*#(*Kjs734-g*%lf]|][",
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

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedUpdateQuery<AllTypes, AllTypesInfo> updateQuery = Query
                .Prepare<AllTypes>()
                .Update(table)
                .Values(values => values
                    .Set(table.Guid, info => info.Guid)
                    .Set(table.String, info => info.String)
                    .Set(table.SmallInt, info => info.SmallInt)
                    .Set(table.Int, info => info.Int)
                    .Set(table.BigInt, info => info.BigInt)
                    .Set(table.Decimal, info => info.Decimal)
                    .Set(table.Float, info => info.Float)
                    .Set(table.Double, info => info.Double)
                    .Set(table.Boolean, info => info.Boolean)
                    .Set(table.Bytes, info => info.Bytes)
                    .Set(table.DateTime, info => info.DateTime)
                    .Set(table.DateTimeOffset, info => info.DateTimeOffset)
                    .Set(table.Enum, info => info.Enum)
                    .Set(table.DateOnly, info => info.DateOnly)
                    .Set(table.TimeOnly, info => info.TimeOnly)
                )
                .Where(where => where.EQUALS(table.Id, info => info.Id))
                .Build(
                    returning => new AllTypesInfo(returning, table)
                 );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = updateQuery.Execute(parameters: allTypes, transaction, TimeoutLevel.ShortUpdate);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes);

                transaction.Commit();

                AssertRowExists(allTypes);
            }
        }

        private async Task UpdateWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            allTypes.UpdateValues(
                guid: Guid.NewGuid(),
                @string: "-4at3=_)(*&_(*#(*Kjs734-g*%lf]|][",
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

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedUpdateQuery<AllTypes, AllTypesInfo> updateQuery = Query
                .Prepare<AllTypes>()
                .Update(table)
                .Values(values => values
                    .Set(table.Guid, info => info.Guid)
                    .Set(table.String, info => info.String)
                    .Set(table.SmallInt, info => info.SmallInt)
                    .Set(table.Int, info => info.Int)
                    .Set(table.BigInt, info => info.BigInt)
                    .Set(table.Decimal, info => info.Decimal)
                    .Set(table.Float, info => info.Float)
                    .Set(table.Double, info => info.Double)
                    .Set(table.Boolean, info => info.Boolean)
                    .Set(table.Bytes, info => info.Bytes)
                    .Set(table.DateTime, info => info.DateTime)
                    .Set(table.DateTimeOffset, info => info.DateTimeOffset)
                    .Set(table.Enum, info => info.Enum)
                    .Set(table.DateOnly, info => info.DateOnly)
                    .Set(table.TimeOnly, info => info.TimeOnly)
                )
                .Where(where => where.EQUALS(table.Id, info => info.Id))
                .Build(
                    returning => new AllTypesInfo(returning, table)
                 );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await updateQuery.ExecuteAsync(parameters: allTypes, transaction, cancellationToken: null, timeout: TimeoutLevel.ShortUpdate);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], allTypes);

                transaction.Commit();

                await AssertRowExistsAsync(allTypes);
            }
        }

        private void DeleteWithQuery(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<AllTypes> deleteQuery8 = Query
                    .Prepare<AllTypes>()
                    .Delete(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, info => info.Id))
                    .Build();

                NonQueryResult result = deleteQuery8.Execute(parameters: allTypes, transaction, timeout: TimeoutLevel.ShortDelete);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {

                QueryResult<int> result = _selectAllTypesCountQuery!.Execute(parameters: allTypes, TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows() + 1);
        }

        private void DeleteWithQueryReturning(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<IntKey<AllTypes>, AllTypesInfo> deleteQuery9 = Query
                    .Prepare<IntKey<AllTypes>>()
                    .Delete(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, id => id))
                    .Build(deleted => new AllTypesInfo(deleted, allTypesTable));

                QueryResult<AllTypesInfo> result = deleteQuery9.Execute(parameters: allTypes.Id, transaction, TimeoutLevel.ShortDelete);

                Assert.AreEqual(1, result.RowsEffected);

                AllTypesInfo row = result.Rows.First();

                AssertRow(row, allTypes);

                transaction.Commit();
            }

            {

                QueryResult<int> result = _selectAllTypesCountQuery!.Execute(parameters: allTypes, TestDatabase.Database);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows() + 1);
        }

        private async Task DeleteWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await GetNumberOfRowsAsync();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<IntKey<AllTypes>> deleteQuery10 = Query
                    .Prepare<IntKey<AllTypes>>()
                    .Delete(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, id => id))
                    .Build();

                NonQueryResult result = await deleteQuery10.ExecuteAsync(parameters: allTypes.Id, transaction);

                Assert.AreEqual(1, result.RowsEffected);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllTypesCountQuery!.ExecuteAsync(parameters: allTypes, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
            Assert.AreEqual(beginRowCount, await GetNumberOfRowsAsync() + 1);
        }

        private async Task DeleteWithQueryReturningAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await GetNumberOfRowsAsync();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<IntKey<AllTypes>, AllTypesInfo> deleteQuery10 = Query
                    .Prepare<IntKey<AllTypes>>()
                    .Delete(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, id => id))
                    .Build(deleted => new AllTypesInfo(deleted, allTypesTable));

                QueryResult<AllTypesInfo> result = await deleteQuery10.ExecuteAsync(parameters: allTypes.Id, transaction);

                Assert.AreEqual(1, result.RowsEffected);

                AllTypesInfo row = result.Rows.First();

                AssertRow(row, allTypes);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllTypesCountQuery!.ExecuteAsync(parameters: allTypes, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(0, countValue!.Value);
            }
            Assert.AreEqual(beginRowCount, await GetNumberOfRowsAsync() + 1);
        }

        private int GetNumberOfRows() {

            QueryResult<int> result = _selectAllCountQuery!.Execute(parameters: true, TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            int? countValue = result.Rows[0];

            Assert.IsNotNull(countValue);
            return countValue!.Value;
        }

        private async Task<int> GetNumberOfRowsAsync() {

            QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameters: true, TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            int? countValue = result.Rows[0];

            Assert.IsNotNull(countValue);
            return countValue!.Value;
        }

        private class AllTypesInfoResult4 {

            public AllTypesInfoResult4(AllTypesInfo allTypesRow1, AllTypesInfo allTypesRow2, AllTypesInfo allTypesRow3, AllTypesInfo allTypesRow4) {
                AllTypesRow1 = allTypesRow1;
                AllTypesRow2 = allTypesRow2;
                AllTypesRow3 = allTypesRow3;
                AllTypesRow4 = allTypesRow4;
            }
            public AllTypesInfo AllTypesRow1 { get; set; }
            public AllTypesInfo AllTypesRow2 { get; set; }
            public AllTypesInfo AllTypesRow3 { get; set; }
            public AllTypesInfo AllTypesRow4 { get; set; }
        }

        private class JoinQueryParams {

            public JoinQueryParams(AllTypes allTypes, IntKey<AllTypes> id) {
                AllTypes = allTypes;
                Id = id;
            }
            public AllTypes AllTypes { get; }
            public IntKey<AllTypes> Id { get; }
        }

        private static void JoinQuery(AllTypes allTypes) {

            IntKey<AllTypes> id = new IntKey<AllTypes>(928756923);

            JoinQueryParams joinQueryParams = new JoinQueryParams(allTypes, id);

            AllTypesTable allTypesTable1 = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;
            AllTypesTable allTypesTable4 = AllTypesTable.Instance4;

            IPreparedQueryExecute<JoinQueryParams, AllTypesInfoResult4> joinQuery1 = Query
                .Prepare<JoinQueryParams>()
                .Select(
                    row => new AllTypesInfoResult4(
                        allTypesRow1: new AllTypesInfo(row, allTypesTable1),
                        allTypesRow2: new AllTypesInfo(row, allTypesTable2),
                        allTypesRow3: new AllTypesInfo(row, allTypesTable3),
                        allTypesRow4: new AllTypesInfo(row, allTypesTable4)
                    )
                )
                .From(allTypesTable1)
                .With(SqlServerTableHint.UPDLOCK, SqlServerTableHint.SERIALIZABLE)
                .Join(allTypesTable2).On(on => on.EQUALS(allTypesTable1.Id, allTypesTable2.Id) & on.EQUALS(allTypesTable1.Id, allTypesTable2.Id))   //Duplicate conditions to test C# type checker
                .Join(allTypesTable3).On(on => on.EQUALS(allTypesTable2.Id, allTypesTable3.Id) & on.EQUALS(allTypesTable2.Id, allTypesTable3.Id))
                .LeftJoin(allTypesTable4).On(on => on.EQUALS(allTypesTable4.Id, (info) => info.Id))
                .Where(where => where.EQUALS(allTypesTable1.Id, (info) => info.AllTypes.Id))
                .Option(labelName: "Label 1", SqlServerQueryOption.FORCE_ORDER)
                .Build();

            QueryResult<AllTypesInfoResult4> result = joinQuery1.Execute(parameters: joinQueryParams, TestDatabase.Database);

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

        private class AllTypesInfoResult3 {

            public AllTypesInfoResult3(AllTypesInfo allTypesRow1, AllTypesInfo allTypesRow2, AllTypesInfo allTypesRow3) {
                AllTypesRow1 = allTypesRow1;
                AllTypesRow2 = allTypesRow2;
                AllTypesRow3 = allTypesRow3;
            }
            public AllTypesInfo AllTypesRow1 { get; set; }
            public AllTypesInfo AllTypesRow2 { get; set; }
            public AllTypesInfo AllTypesRow3 { get; set; }
        }
        private static async Task JoinQueryAsync(AllTypes allTypes) {

            AllTypesTable allTypesTable1 = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;

            IPreparedQueryExecute<AllTypes, AllTypesInfoResult3> joinQuery = Query
                .Prepare<AllTypes>()
                .Select(
                    row => new AllTypesInfoResult3(
                        allTypesRow1: new AllTypesInfo(row, allTypesTable1),
                        allTypesRow2: new AllTypesInfo(row, allTypesTable2),
                        allTypesRow3: new AllTypesInfo(row, allTypesTable3)
                    )
                )
                .From(allTypesTable1)
                .Join(allTypesTable2).On(on => on.EQUALS(allTypesTable1.Id, allTypesTable2.Id))
                .Join(allTypesTable3).On(on => on.EQUALS(allTypesTable2.Id, allTypesTable3.Id))
                .Where(where => where.EQUALS(allTypesTable1.Id, info => info.Id))
                .Build();

            QueryResult<AllTypesInfoResult3> result = await joinQuery.ExecuteAsync(parameters: allTypes, TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(0, result.RowsEffected);

            AllTypesInfo row1 = result.Rows[0].AllTypesRow1;
            AllTypesInfo row2 = result.Rows[0].AllTypesRow2;
            AllTypesInfo row3 = result.Rows[0].AllTypesRow3;

            AssertRow(row1, allTypes);
            AssertRow(row2, allTypes);
            AssertRow(row3, allTypes);
        }

        private void InsertWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsFalse(allTypes.Id.IsValid);

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedInsertQuery<AllTypes, AllTypesInfo> insertQuery = Query
                .Prepare<AllTypes>()
                .Insert(table)
                .Values(values => values
                    .Set(table.Guid, info => info.Guid)
                        .Set(table.String, info => info.String)
                        .Set(table.SmallInt, info => info.SmallInt)
                        .Set(table.Int, info => info.Int)
                        .Set(table.BigInt, info => info.BigInt)
                        .Set(table.Decimal, info => info.Decimal)
                        .Set(table.Float, info => info.Float)
                        .Set(table.Double, info => info.Double)
                        .Set(table.Boolean, info => info.Boolean)
                        .Set(table.Bytes, info => info.Bytes)
                        .Set(table.DateTime, info => info.DateTime)
                        .Set(table.DateTimeOffset, info => info.DateTimeOffset)
                        .Set(table.Enum, info => info.Enum)
                        .Set(table.DateOnly, info => info.DateOnly)
                        .Set(table.TimeOnly, info => info.TimeOnly)
                    )
                .Build(
                    returning => new AllTypesInfo(returning, table)
                );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = insertQuery
                    .Execute(
                        parameters: allTypes,
                        transaction
                    );

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(1, result.RowsEffected);

                allTypes.Id = result.Rows[0].Id;

                AssertRow(result.Rows[0], allTypes);

                Assert.IsTrue(allTypes.Id.IsValid);

                AssertRowExists(allTypes, transaction);

                transaction.Rollback();

                AssertRowDoesNotExists(allTypes);

                allTypes.Id = IntKey<AllTypes>.NotSet;
            }
        }

        private void UpdateWithQueryAndRollback(AllTypes initialAllTypes) {

            Assert.IsTrue(initialAllTypes.Id.IsValid);

            AllTypes newAllTypes = new AllTypes(
                id: initialAllTypes.Id,
                guid: Guid.NewGuid(),
                @string: "F89&sad^&%$Djadsa",
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

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedUpdateQuery<AllTypes, AllTypesInfo> updateQuery = Query
                .Prepare<AllTypes>()
                .Update(table)
                .Values(values => values
                    .Set(table.Guid, info => info.Guid)
                    .Set(table.String, info => info.String)
                    .Set(table.SmallInt, info => info.SmallInt)
                    .Set(table.Int, info => info.Int)
                    .Set(table.BigInt, info => info.BigInt)
                    .Set(table.Decimal, info => info.Decimal)
                    .Set(table.Float, info => info.Float)
                    .Set(table.Double, info => info.Double)
                    .Set(table.Boolean, info => info.Boolean)
                    .Set(table.Bytes, info => info.Bytes)
                    .Set(table.DateTime, info => info.DateTime)
                    .Set(table.DateTimeOffset, info => info.DateTimeOffset)
                    .Set(table.Enum, info => info.Enum)
                    .Set(table.DateOnly, info => info.DateOnly)
                    .Set(table.TimeOnly, info => info.TimeOnly)
                )
                .Where(where => where.EQUALS(table.Id, info => info.Id))
                .Build(
                    returning => new AllTypesInfo(returning, table)
                );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = updateQuery.Execute(parameters: newAllTypes, transaction);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                AssertRow(result.Rows[0], newAllTypes);
                AssertRowExists(newAllTypes, transaction);

                transaction.Rollback();
                AssertRowExists(initialAllTypes);   //Assert initial values
            }
        }

        private void DeleteWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = GetNumberOfRows();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                IPreparedDeleteQuery<IntKey<AllTypes>> deleteQuery11 = Query
                    .Prepare<IntKey<AllTypes>>()
                    .Delete(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, id => id))
                    .Build();

                NonQueryResult result = deleteQuery11.Execute(parameters: allTypes.Id, transaction);

                Assert.AreEqual(1, result.RowsEffected);

                AssertRowDoesNotExists(allTypes, transaction);

                transaction.Rollback();

                AssertRowExists(allTypes);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows());
        }

        /*
         * Test the update join syntax
         */
        private void UpdateJoinTest() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                IPreparedUpdateQuery<AllTypes, AllTypesInfo> updateQuery = Query.Prepare<AllTypes>()
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, info => info.Guid)
                        .Set(tableA.String, info => info.String)
                        .Set(tableA.SmallInt, info => info.SmallInt)
                        .Set(tableA.Int, info => info.Int)
                        .Set(tableA.BigInt, info => info.BigInt)
                        .Set(tableA.Decimal, info => info.Decimal)
                        .Set(tableA.Float, info => info.Float)
                        .Set(tableA.Double, info => info.Double)
                        .Set(tableA.Boolean, info => info.Boolean)
                        .Set(tableA.Bytes, info => info.Bytes)
                        .Set(tableA.DateTime, info => info.DateTime)
                        .Set(tableA.DateTimeOffset, info => info.DateTimeOffset)
                        .Set(tableA.Enum, info => info.Enum)
                        .Set(tableA.DateOnly, info => info.DateOnly)
                        .Set(tableA.TimeOnly, info => info.TimeOnly)
                    )
                    .From(tableB)
                    .Where(
                        where => where.EQUALS(tableA.BigInt, tableB.BigInt) &   //Join condition
                        where.NOT_EQUALS(tableA.Id, info => info.Id) &
                        where.EQUALS(tableB.Id, info => info.Id)
                    )
                    .Build(updated => new AllTypesInfo(updated, tableA));

                QueryResult<AllTypesInfo> result = updateQuery.Execute(parameters: allTypes1, transaction);

                Assert.AreEqual(2, result.RowsEffected);
                Assert.AreEqual(2, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AssertRowExists(allTypes1, transaction);

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

                AssertRowExists(allTypes2, transaction);

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

                AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }

        /*
         * Test the update join syntax
         */
        private async Task UpdateJoinTestAsync() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                IPreparedUpdateQuery<AllTypes, AllTypesInfo> updateQuery = Query.Prepare<AllTypes>()
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, info => info.Guid)
                        .Set(tableA.String, info => info.String)
                        .Set(tableA.SmallInt, info => info.SmallInt)
                        .Set(tableA.Int, info => info.Int)
                        .Set(tableA.BigInt, info => info.BigInt)
                        .Set(tableA.Decimal, info => info.Decimal)
                        .Set(tableA.Float, info => info.Float)
                        .Set(tableA.Double, info => info.Double)
                        .Set(tableA.Boolean, info => info.Boolean)
                        .Set(tableA.Bytes, info => info.Bytes)
                        .Set(tableA.DateTime, info => info.DateTime)
                        .Set(tableA.DateTimeOffset, info => info.DateTimeOffset)
                        .Set(tableA.Enum, info => info.Enum)
                        .Set(tableA.DateOnly, info => info.DateOnly)
                        .Set(tableA.TimeOnly, info => info.TimeOnly)
                    )
                    .From(tableB)
                    .Where(
                        where => where.EQUALS(tableA.BigInt, tableB.BigInt) &   //Join condition
                        where.NOT_EQUALS(tableA.Id, info => info.Id) &
                        where.EQUALS(tableB.Id, info => info.Id)
                    )
                    .Build(updated => new AllTypesInfo(updated, tableA));

                QueryResult<AllTypesInfo> result = await updateQuery.ExecuteAsync(parameters: allTypes1, transaction);

                Assert.AreEqual(2, result.RowsEffected);
                Assert.AreEqual(2, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AssertRowExists(allTypes1, transaction);

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

                AssertRowExists(allTypes2, transaction);

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

                AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }

        /*
         * Test the update join syntax
         */
        private void UpdateJoinTest2() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                (AllTypes Info, IntKey<AllTypes> Id1, IntKey<AllTypes> Id3) parameters = new(allTypes1, allTypes1.Id, allTypes3.Id);

                IPreparedUpdateQuery<(AllTypes Info, IntKey<AllTypes> Id1, IntKey<AllTypes> Id3), AllTypesInfo> updateQuery = Query
                    .Prepare<(AllTypes Info, IntKey<AllTypes> Id1, IntKey<AllTypes> Id3)>()
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, x => x.Info.Guid)
                        .Set(tableA.String, x => x.Info.String)
                        .Set(tableA.SmallInt, x => x.Info.SmallInt)
                        .Set(tableA.Int, x => x.Info.Int)
                        .Set(tableA.BigInt, x => x.Info.BigInt)
                        .Set(tableA.Decimal, x => x.Info.Decimal)
                        .Set(tableA.Float, x => x.Info.Float)
                        .Set(tableA.Double, x => x.Info.Double)
                        .Set(tableA.Boolean, x => x.Info.Boolean)
                        .Set(tableA.Bytes, x => x.Info.Bytes)
                        .Set(tableA.DateTime, x => x.Info.DateTime)
                        .Set(tableA.DateTimeOffset, x => x.Info.DateTimeOffset)
                        .Set(tableA.Enum, x => x.Info.Enum)
                        .Set(tableA.DateOnly, x => x.Info.DateOnly)
                        .Set(tableA.TimeOnly, x => x.Info.TimeOnly)
                    )
                    .From(tableB)
                    .Where( //Only update allTypes2
                        where => where.EQUALS(tableA.BigInt, tableB.BigInt) &   //Join condition
                        where.NOT_EQUALS(tableA.Id, x => x.Id1) &
                        where.EQUALS(tableB.Id, x => x.Id1) &
                        where.NOT_EQUALS(tableA.Id, x => x.Id3)
                    )
                    .Build(updated => new AllTypesInfo(updated, tableA));

                QueryResult<AllTypesInfo> result = updateQuery.Execute(parameters, transaction);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AssertRowExists(allTypes1, transaction);

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

                AssertRowExists(allTypes2, transaction);
                AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }

        /*
         * Test the update join syntax
         */
        private async Task UpdateJoinTest2Async() {

            AllTypes allTypes1 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes2 = AllFieldsPreparedTest.GetAllTypes1();
            AllTypes allTypes3 = AllFieldsPreparedTest.GetAllTypes1();

            //Set all BigInt to the same value so we can update join on it and set all records to the same values
            allTypes1.BigInt = 1;
            allTypes2.BigInt = 1;
            allTypes3.BigInt = 1;

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable tableA = AllTypesTable.Instance;
                AllTypesTable tableB = AllTypesTable.Instance2;

                (AllTypes Info, IntKey<AllTypes> Id1, IntKey<AllTypes> Id3) parameters = new(allTypes1, allTypes1.Id, allTypes3.Id);

                IPreparedUpdateQuery<(AllTypes Info, IntKey<AllTypes> Id1, IntKey<AllTypes> Id3), AllTypesInfo> updateQuery = Query
                    .Prepare<(AllTypes Info, IntKey<AllTypes> Id1, IntKey<AllTypes> Id3)>()
                    .Update(tableA)
                    .Values(values => values
                        .Set(tableA.Guid, x => x.Info.Guid)
                        .Set(tableA.String, x => x.Info.String)
                        .Set(tableA.SmallInt, x => x.Info.SmallInt)
                        .Set(tableA.Int, x => x.Info.Int)
                        .Set(tableA.BigInt, x => x.Info.BigInt)
                        .Set(tableA.Decimal, x => x.Info.Decimal)
                        .Set(tableA.Float, x => x.Info.Float)
                        .Set(tableA.Double, x => x.Info.Double)
                        .Set(tableA.Boolean, x => x.Info.Boolean)
                        .Set(tableA.Bytes, x => x.Info.Bytes)
                        .Set(tableA.DateTime, x => x.Info.DateTime)
                        .Set(tableA.DateTimeOffset, x => x.Info.DateTimeOffset)
                        .Set(tableA.Enum, x => x.Info.Enum)
                        .Set(tableA.DateOnly, x => x.Info.DateOnly)
                        .Set(tableA.TimeOnly, x => x.Info.TimeOnly)
                    )
                    .From(tableB)
                    .Where( //Only update allTypes2
                        where => where.EQUALS(tableA.BigInt, tableB.BigInt) &   //Join condition
                        where.NOT_EQUALS(tableA.Id, x => x.Id1) &
                        where.EQUALS(tableB.Id, x => x.Id1) &
                        where.NOT_EQUALS(tableA.Id, x => x.Id3)
                    )
                    .Build(updated => new AllTypesInfo(updated, tableA));

                QueryResult<AllTypesInfo> result = await updateQuery.ExecuteAsync(parameters, transaction);

                Assert.AreEqual(1, result.RowsEffected);
                Assert.AreEqual(1, result.Rows.Count);

                /*
                 *  Not that all rows have been set to the same values (except the Id field), we need to check those fields have the correct values
                 */
                AssertRowExists(allTypes1, transaction);

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

                AssertRowExists(allTypes2, transaction);
                AssertRowExists(allTypes3, transaction);

                transaction.Commit();
            }
        }
    }
}