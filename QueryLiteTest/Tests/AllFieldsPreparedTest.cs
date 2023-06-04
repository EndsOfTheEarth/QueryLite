using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.DbSchema;
using QueryLite.PreparedQuery;
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

        [TestInitialize]
        public void ClearTable() {

            InitQueries();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(allTypesTable)
                    .NoWhereCondition()
                    .Execute(transaction);

                QueryResult<int> result = query1!.Execute(parameterValues: true, transaction);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue, 0);

                transaction.Commit();
            }
        }

        private IPreparedQueryExecute<bool, int>? query1;
        private IPreparedQueryExecute<AllTypes, AllTypesInfo>? _selectAllTypesQuery;
        private IPreparedQueryExecute<bool, int>? _selectAllCountQuery;
        private IPreparedQueryExecute<AllTypes, int>? _selectAllTypesCountQuery;

        public void InitQueries() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

            {

                query1 = Query
                    .PrepareWithParameters<bool>()
                    .Select(row => row.Get(count))
                    .From(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, allTypesTable.Id))
                    .Build();
            }

            {

                _selectAllTypesQuery = Query
                   .PrepareWithParameters<AllTypes>()
                   .Select(
                       row => new AllTypesInfo(row, allTypesTable)
                   )
                   .From(allTypesTable)
                   .Where(where => where.EQUALS(allTypesTable.Id, (info) => info.Id))
                   .Build();
            }

            {

                _selectAllCountQuery = Query
                    .PrepareWithParameters<bool>()
                    .Select(row => row.Get(count))
                    .From(allTypesTable)
                    .Build();
            }
            {

                _selectAllTypesCountQuery = Query
                    .PrepareWithParameters<AllTypes>()
                    .Select(row => row.Get(count))
                    .From(allTypesTable)
                    .Where(where => where.EQUALS(allTypesTable.Id, (info) => info.Id))
                    .Build();
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

        [TestMethod]
        public void LoadDocumentation() {

            string doc = DocumentationGenerator.GenerateForAssembly(new Assembly[] { Assembly.GetExecutingAssembly() });
            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void RunSchemaValidator() {

            SchemaValidationSettings settings = new SchemaValidationSettings() {
                ValidatePrimaryKeys = true,
                ValidateForeignKeys = true,   //TODO set to true
                ValidateMissingCodeTables = true
            };

            List<ITable> tables = new List<ITable>() {
                AllTypesTable.Instance,
                ParentTable.Instance,
                ChildTable.Instance
            };

            ValidationResult result = SchemaValidator.ValidateTables(TestDatabase.Database, tables, settings);

            Assert.AreEqual(result.TableValidation.Count, 3);

            foreach(TableValidation val in result.TableValidation) {

                Assert.AreEqual(val.ValidationMessages.Count, 0);
            }
        }

        [TestMethod]
        public async Task DeleteJoinQueriesAsync() {

            if(TestDatabase.Database.DatabaseType == DatabaseType.SqlServer) {
                Settings.UseParameters = true;
                await BasicInsertAndDeleteJoinQueriesAsync();
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

        private static IPreparedInsertQuery<AllTypes> _insertQueryWithoutReturning = Query
            .PreparedInsert<AllTypes>(AllTypesTable.Instance)
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
            .Build(TestDatabase.Database);

        [TestMethod]
        public void TestInsertWithoutReturning() {

            AllTypes allTypes1 = GetAllTypes1();

            using Transaction transaction = new Transaction(TestDatabase.Database);

            NonQueryResult insertResult = _insertQueryWithoutReturning.Execute(allTypes1, transaction);

            Assert.AreEqual(insertResult.RowsEffected, 1);

            transaction.Commit();

            AllTypesTable table = AllTypesTable.Instance;

            QueryResult<AllTypesInfo> queryResult = Query
                .Select(
                    row => new AllTypesInfo(row, table)
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(queryResult.Rows.Count, 1);
            Assert.AreEqual(queryResult.RowsEffected, 0);

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

        private AllTypes GetAllTypes1() {

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

        private void AssertRowExists(AllTypes allTypes) {

            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameterValues: allTypes, TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);

            AssertRow(result.Rows[0], allTypes);
        }

        private async Task AssertRowExistsAsync(AllTypes allTypes) {

            QueryResult<AllTypesInfo> result = await _selectAllTypesQuery!.ExecuteAsync(parameterValues: allTypes, TestDatabase.Database, cancellationToken: CancellationToken.None);

            Assert.AreEqual(result.Rows.Count, 1);

            AssertRow(result.Rows[0], allTypes);
        }

        private void AssertRowExists(AllTypes allTypes, Transaction transaction) {


            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameterValues: allTypes, transaction);

            Assert.AreEqual(result.Rows.Count, 1);

            AssertRow(result.Rows[0], allTypes);
        }

        private void AssertRowDoesNotExists(AllTypes allTypes) {

            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameterValues: allTypes, TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 0);
        }

        private void AssertRowDoesNotExists(AllTypes allTypes, Transaction transaction) {


            QueryResult<AllTypesInfo> result = _selectAllTypesQuery!.Execute(parameterValues: allTypes, transaction);

            Assert.AreEqual(result.Rows.Count, 0);
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
            DeleteWithQueryReturning(allTypes3);

            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
        }

        private void BasicInsertAndTruncateWithQueries() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            Truncate();
        }

        private async Task BasicInsertAndTruncateWithQueriesAsync() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            InsertWithQuery(allTypes1);
            InsertWithQuery(allTypes2);
            InsertWithQuery(allTypes3);

            await TruncateAsync();
        }

        private async Task BasicInsertUpdateAndDeleteWithQueriesAsync() {

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
            await DeleteWithQueryReturningAsync(allTypes3);

            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
            InsertWithQueryAndRollback(GetAllTypes1());
        }

        private async Task BasicInsertAndDeleteJoinQueriesAsync() {

            AllTypes allTypes1 = GetAllTypes1();
            AllTypes allTypes2 = GetAllTypes1();
            AllTypes allTypes3 = GetAllTypes1();

            await InsertWithQueryAsync(allTypes1);
            await InsertWithQueryAsync(allTypes2);
            await InsertWithQueryAsync(allTypes3);

            AllTypesTable allTypesTable = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = await Query
                    .Delete(allTypesTable)
                    .Join(allTypesTable2).On(allTypesTable.Id == allTypesTable2.Id)
                    .Where(allTypesTable.Id == allTypes1.Id)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(result.RowsEffected, 1);

                result = await Query
                    .Delete(allTypesTable)
                    .LeftJoin(allTypesTable2).On(allTypesTable.Id == IntKey<AllTypes>.ValueOf(int.MaxValue))    //Left join with an id that does not exist
                    .Where(allTypesTable.Id == allTypes2.Id & allTypesTable2.Id.IsNotNull)  //Is not null should return zero rows
                    .ExecuteAsync(transaction);

                Assert.AreEqual(result.RowsEffected, 0);

                result = await Query
                    .Delete(allTypesTable)
                    .LeftJoin(allTypesTable2).On(allTypesTable.Id == IntKey<AllTypes>.ValueOf(int.MaxValue))    //Left join with an id that does not exist
                    .Where(allTypesTable.Id == allTypes2.Id & allTypesTable2.Id.IsNull)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(result.RowsEffected, 1);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameterValues: true, TestDatabase.Database, cancellationToken: CancellationToken.None);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue!.Value, 1);  //There should be one record left
            }

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await Query
                    .Delete(allTypesTable)
                    .Join(allTypesTable2).On(allTypesTable.Id == allTypesTable2.Id)
                    .Where(allTypesTable.Id == allTypes3.Id)
                    .ExecuteAsync(
                        deleted => new AllTypesInfo(deleted, allTypesTable),
                        transaction
                    );

                Assert.AreEqual(result.RowsEffected, 1);
                Assert.AreEqual(result.Rows.Count, 1);

                AssertRow(result.Rows[0], allTypes3);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllCountQuery.ExecuteAsync(parameterValues: true, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue!.Value, 0);
            }
        }

        private static IPreparedInsertQuery<AllTypes, AllTypesInfo> _insertQuery1 = Query
            .PreparedInsert<AllTypes>(AllTypesTable.Instance)
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
                returning => new AllTypesInfo(returning, AllTypesTable.Instance),
                TestDatabase.Database
            );

        private void InsertWithQuery(AllTypes allTypes) {

            Assert.IsTrue(!allTypes.Id.IsValid);

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = _insertQuery1
                    .Execute(
                        parameters: allTypes,
                        transaction,
                        TimeoutLevel.ShortInsert
                    );

                transaction.Commit();

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 1);

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

            QueryResult<int> result = _selectAllCountQuery!.Execute(parameterValues: true, TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);

            Assert.AreEqual(result.Rows.First(), 0);
        }

        private async Task TruncateAsync() {

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult truncateResult = await Query
                    .Truncate(allTypesTable)
                    .ExecuteAsync(transaction);

                transaction.Commit();
            }

            QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameterValues: true, TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(result.Rows.Count, 1);

            Assert.AreEqual(result.Rows.First(), 0);
        }

        public async Task InsertWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(!allTypes.Id.IsValid);

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedInsertQuery<AllTypes, AllTypesInfo> insertQuery = Query
                .PreparedInsert<AllTypes>(table)
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
                    returning => new AllTypesInfo(returning, table),
                    TestDatabase.Database
                );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await insertQuery
                    .ExecuteAsync(
                        parameters: allTypes,
                        transaction,
                        timeout: TimeoutLevel.ShortInsert
                    );

                transaction.Commit();

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 1);

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
                bytes: new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                dateTime: new DateTime(year: 2023, month: 1, day: 2, hour: 3, minute: 4, second: 5),
                dateTimeOffset: new DateTimeOffset(year: 2030, month: 12, day: 11, hour: 10, minute: 9, second: 8, new TimeSpan(hours: 0, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.B,
                dateOnly: new DateOnly(year: 1990, month: 1, day: 31),
                timeOnly: new TimeOnly(hour: 12, minute: 14, second: 55, millisecond: 130, microsecond: 999)
            );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable table = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = Query
                    .Update(table)
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
                    .Where(table.Id == allTypes.Id)
                    .Execute(
                        result => new AllTypesInfo(result, table),
                        transaction,
                        TimeoutLevel.ShortUpdate
                    );

                Assert.AreEqual(result.RowsEffected, 1);
                Assert.AreEqual(result.Rows.Count, 1);

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
                bytes: new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                dateTime: new DateTime(year: 2023, month: 1, day: 2, hour: 3, minute: 4, second: 5),
                dateTimeOffset: new DateTimeOffset(year: 2030, month: 12, day: 11, hour: 10, minute: 9, second: 8, new TimeSpan(hours: 0, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.B,
                dateOnly: new DateOnly(year: 1854, month: 05, day: 27),
                timeOnly: new TimeOnly(hour: 1, minute: 4, second: 5, millisecond: 30, microsecond: 100)
            );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable table = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = await Query
                    .Update(table)
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
                    .Where(table.Id == allTypes.Id)
                    .ExecuteAsync(
                        result => new AllTypesInfo(result, table),
                        transaction,
                        cancellationToken: null,
                        TimeoutLevel.ShortUpdate
                    );

                Assert.AreEqual(result.RowsEffected, 1);
                Assert.AreEqual(result.Rows.Count, 1);

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

                NonQueryResult result = Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(transaction, timeout: TimeoutLevel.ShortDelete);

                Assert.AreEqual(result.RowsEffected, 1);

                transaction.Commit();
            }

            {

                QueryResult<int> result = _selectAllTypesCountQuery!.Execute(parameterValues: allTypes, TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue!.Value, 0);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows() + 1);
        }

        private void DeleteWithQueryReturning(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = GetNumberOfRows();

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

                Assert.AreEqual(result.RowsEffected, 1);

                AllTypesInfo row = result.Rows.First();

                AssertRow(row, allTypes);

                transaction.Commit();
            }

            {

                QueryResult<int> result = _selectAllTypesCountQuery!.Execute(parameterValues: allTypes, TestDatabase.Database);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue!.Value, 0);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows() + 1);
        }

        private async Task DeleteWithQueryAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await GetNumberOfRowsAsync();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult result = await Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .ExecuteAsync(transaction);

                Assert.AreEqual(result.RowsEffected, 1);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllTypesCountQuery!.ExecuteAsync(parameterValues: allTypes, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue!.Value, 0);
            }
            Assert.AreEqual(beginRowCount, await GetNumberOfRowsAsync() + 1);
        }

        private async Task DeleteWithQueryReturningAsync(AllTypes allTypes) {

            Assert.IsTrue(allTypes.Id.IsValid);

            int beginRowCount = await GetNumberOfRowsAsync();

            AllTypesTable allTypesTable = AllTypesTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = await Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .ExecuteAsync(
                        result => new AllTypesInfo(result, allTypesTable),
                        transaction
                    );

                Assert.AreEqual(result.RowsEffected, 1);

                AllTypesInfo row = result.Rows.First();

                AssertRow(row, allTypes);

                transaction.Commit();
            }

            {

                QueryResult<int> result = await _selectAllTypesCountQuery!.ExecuteAsync(parameterValues: allTypes, TestDatabase.Database, CancellationToken.None);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

                Assert.IsNotNull(countValue);
                Assert.AreEqual(countValue!.Value, 0);
            }
            Assert.AreEqual(beginRowCount, await GetNumberOfRowsAsync() + 1);
        }

        private int GetNumberOfRows() {

            QueryResult<int> result = _selectAllCountQuery!.Execute(parameterValues: true, TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);
            Assert.AreEqual(result.RowsEffected, 0);

            int? countValue = result.Rows[0];

            Assert.IsNotNull(countValue);
            return countValue!.Value;
        }

        private async Task<int> GetNumberOfRowsAsync() {

            QueryResult<int> result = await _selectAllCountQuery!.ExecuteAsync(parameterValues: true, TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(result.Rows.Count, 1);
            Assert.AreEqual(result.RowsEffected, 0);

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

        private void JoinQuery(AllTypes allTypes) {

            IntKey<AllTypes> id = new IntKey<AllTypes>(928756923);

            JoinQueryParams joinQueryParams = new JoinQueryParams(allTypes, id);

            AllTypesTable allTypesTable1 = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;
            AllTypesTable allTypesTable4 = AllTypesTable.Instance4;

            IPreparedQueryExecute<JoinQueryParams, AllTypesInfoResult4> joinQuery1 = Query
                .PrepareWithParameters<JoinQueryParams>()
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

            QueryResult<AllTypesInfoResult4> result = joinQuery1.Execute(parameterValues: joinQueryParams, TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);
            Assert.AreEqual(result.RowsEffected, 0);

            AllTypesInfo row1 = result.Rows[0].AllTypesRow1;
            AllTypesInfo row2 = result.Rows[0].AllTypesRow2;
            AllTypesInfo row3 = result.Rows[0].AllTypesRow3;
            AllTypesInfo row4 = result.Rows[0].AllTypesRow4;

            AssertRow(row1, allTypes);
            AssertRow(row2, allTypes);
            AssertRow(row3, allTypes);
            Assert.IsTrue(!row4.Id.IsValid);
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
        private async Task JoinQueryAsync(AllTypes allTypes) {

            AllTypesTable allTypesTable1 = AllTypesTable.Instance;
            AllTypesTable allTypesTable2 = AllTypesTable.Instance2;
            AllTypesTable allTypesTable3 = AllTypesTable.Instance3;

            IPreparedQueryExecute<AllTypes, AllTypesInfoResult3> joinQuery = Query
                .PrepareWithParameters<AllTypes>()
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

            QueryResult<AllTypesInfoResult3> result = await joinQuery.ExecuteAsync(parameterValues: allTypes, TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(result.Rows.Count, 1);
            Assert.AreEqual(result.RowsEffected, 0);

            AllTypesInfo row1 = result.Rows[0].AllTypesRow1;
            AllTypesInfo row2 = result.Rows[0].AllTypesRow2;
            AllTypesInfo row3 = result.Rows[0].AllTypesRow3;

            AssertRow(row1, allTypes);
            AssertRow(row2, allTypes);
            AssertRow(row3, allTypes);
        }

        private void InsertWithQueryAndRollback(AllTypes allTypes) {

            Assert.IsTrue(!allTypes.Id.IsValid);

            AllTypesTable table = AllTypesTable.Instance;

            IPreparedInsertQuery<AllTypes, AllTypesInfo> insertQuery = Query
                .PreparedInsert<AllTypes>(table)
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
                    returning => new AllTypesInfo(returning, table),
                    TestDatabase.Database
                );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                QueryResult<AllTypesInfo> result = insertQuery
                    .Execute(
                        parameters: allTypes,
                        transaction
                    );

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 1);

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
                bytes: new byte[] { 5, 99, 3, 6, 5, 4, 7, 3, 1, 10 },
                dateTime: new DateTime(year: 2019, month: 11, day: 12, hour: 13, minute: 14, second: 15),
                dateTimeOffset: new DateTimeOffset(year: 2025, month: 11, day: 10, hour: 1, minute: 7, second: 5, new TimeSpan(hours: 3, minutes: 0, seconds: 0)),
                @enum: AllTypesEnum.C,
                dateOnly: new DateOnly(year: 9999, month: 12, day: 31),
                timeOnly: new TimeOnly(hour: 9, minute: 59, second: 1, millisecond: 770, microsecond: 11)
            );

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                AllTypesTable table = AllTypesTable.Instance;

                QueryResult<AllTypesInfo> result = Query
                    .Update(table)
                    .Values(values => values
                        .Set(table.Guid, newAllTypes.Guid)
                        .Set(table.String, newAllTypes.String)
                        .Set(table.SmallInt, newAllTypes.SmallInt)
                        .Set(table.Int, newAllTypes.Int)
                        .Set(table.BigInt, newAllTypes.BigInt)
                        .Set(table.Decimal, newAllTypes.Decimal)
                        .Set(table.Float, newAllTypes.Float)
                        .Set(table.Double, newAllTypes.Double)
                        .Set(table.Boolean, newAllTypes.Boolean)
                        .Set(table.Bytes, newAllTypes.Bytes)
                        .Set(table.DateTime, newAllTypes.DateTime)
                        .Set(table.DateTimeOffset, newAllTypes.DateTimeOffset)
                        .Set(table.Enum, newAllTypes.Enum)
                        .Set(table.DateOnly, newAllTypes.DateOnly)
                        .Set(table.TimeOnly, newAllTypes.TimeOnly)
                    )
                    .Where(table.Id == newAllTypes.Id)
                    .Execute(
                        updated => new AllTypesInfo(updated, table),
                        transaction
                    );

                Assert.AreEqual(result.RowsEffected, 1);
                Assert.AreEqual(result.Rows.Count, 1);

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

                NonQueryResult result = Query
                    .Delete(allTypesTable)
                    .Where(allTypesTable.Id == allTypes.Id)
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                Assert.AreEqual(result.RowsEffected, 1);

                AssertRowDoesNotExists(allTypes, transaction);

                transaction.Rollback();

                AssertRowExists(allTypes);
            }
            Assert.AreEqual(beginRowCount, GetNumberOfRows());
        }
    }
}