using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLiteTest.Tables;
using System;
using System.Text.Json;

namespace QueryLiteTest.Tests {

    [TestClass]
    public class JsonTests {

        [TestInitialize]
        public void ClearTable() {

            JsonTable table = JsonTable.Instance;

            using Transaction transaction = new(TestDatabase.Database);

            Query.Delete(table).NoWhereCondition().Execute(transaction);

            transaction.Commit();
        }

        [TestMethod]
        public void InsertJson_Parameters_Test() {
            Settings.UseParameters = true;
            InsertJsonTest();
        }

        [TestMethod]
        public void InsertJson_NoParameters_Test() {
            Settings.UseParameters = false;
            InsertJsonTest();
        }

        private class Product {

            public string ProductId { get; set; } = "";
            public string Name { get; set; } = "";
        }

        private static void InsertJsonTest() {

            JsonTable table = JsonTable.Instance;

            Product product = new() { ProductId = "#150323", Name = "Keyboard" };

            Guid id = new Guid("{3F9EDA4F-5D07-4DF3-9C4E-1E8DF711081D}");

            using(Transaction transaction = new(TestDatabase.Database)) {

                Jsonb json = new(JsonSerializer.Serialize(product));

                Query.Insert(table)
                    .Values(values => values
                        .Set(table.Id, id)
                        .Set(table.Detail, json)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            ValidateRecord(id, product);
            TestExpressions(id, product);

            using(Transaction transaction = new(TestDatabase.Database)) {

                product.ProductId += "1";
                product.Name += "2";

                Jsonb json = new(JsonSerializer.Serialize(product));

                Query.Update(table)
                    .Values(values => values
                        .Set(table.Detail, json)
                    )
                    .Where(table.Id == id)
                    .Execute(transaction);

                transaction.Commit();
            }
            ValidateRecord(id, product);
            TestExpressions(id, product);
        }

        [TestMethod]
        public void RepositoryTest() {

            Guid id = Guid.NewGuid();

            Product product = new() { ProductId = "#23452", Name = "Monitor" };

            JsonRow row = new(id: id, detail: Jsonb.AsJson(product));

            JsonRepository repository = new();

            repository.AddNewRow(row);

            repository.SaveChanges(TestDatabase.Database);

            ValidateRecord(id, product);

            product.ProductId += "_";
            product.Name += "_";

            row.Detail = Jsonb.AsJson(product);

            repository.SaveChanges(TestDatabase.Database);

            ValidateRecord(id, product);
        }

        private static void ValidateRecord(Guid id, Product product) {

            JsonTable table = JsonTable.Instance;

            QueryResult<Jsonb> result = Query
                .Select(
                    row => row.Get(table.Detail)
                )
                .From(table)
                .Where(table.Id == id)
                .Execute(TestDatabase.Database);

            Assert.HasCount(1, result.Rows);

            foreach(Jsonb detail in result.Rows) {

                Product? rtnProduct = JsonSerializer.Deserialize<Product>(detail.Value);

                Assert.IsNotNull(rtnProduct);
                Assert.AreEqual(product.ProductId, rtnProduct.ProductId);
                Assert.AreEqual(product.Name, rtnProduct.Name);
            }
        }

        private static void TestExpressions(Guid id, Product product) {

            JsonTable table = JsonTable.Instance;

            Expression<Json> jsonProduct = new Expression<Json>(table.Detail) + "->>" + SqlText.Quoted("ProductId");
            Expression<Json> jsonName = new Expression<Json>(table.Detail) + "->>" + SqlText.Quoted("Name");

            var result = Query
                .Select(
                    row => new {
                        ProductId = row.Get(jsonProduct),
                        Name = row.Get(jsonName),
                    }
                )
                .From(table)
                .Where(table.Id == id)
                .Execute(TestDatabase.Database);

            Assert.HasCount(1, result.Rows);

            foreach(var row in result.Rows) {

                Assert.AreEqual(product.ProductId, row.ProductId.Value);
                Assert.AreEqual(product.Name, row.Name.Value);
            }
        }

        [TestMethod]
        public void Expression_01_Parameters_Test() {
            Settings.UseParameters = true;
            Expression_01_Test();
        }
        [TestMethod]
        public void Expression_01_No_Parameters_Test() {
            Settings.UseParameters = false;
            Expression_01_Test();
        }
        private static void Expression_01_Test() {

            Expression<bool> expression = new(SqlText.QuotedAsJson(new { A = 1, B = 2 }), "::jsonb @>", SqlText.QuotedAsJson(new { B = 2 }), "::jsonb");

            QueryResult<bool> result = Query.Select(
                    row => row.Get(expression)
                )
                .NoFromClause()
                .Execute(TestDatabase.Database);

            Assert.HasCount(1, result.Rows);
            Assert.IsTrue(result.Rows[0]);
        }

        [TestMethod]
        public void Expression_02_Parameters_Test() {
            Settings.UseParameters = true;
            Expression_02_Test();
        }
        [TestMethod]
        public void Expression_02_No_Parameters_Test() {
            Settings.UseParameters = false;
            Expression_02_Test();
        }        
        private static void Expression_02_Test() {

            Settings.UseParameters = true;

            Expression<bool> expression = new(SqlText.QuotedAsJson(new { a = 1, b = 2 }), "::jsonb ?", SqlText.Quoted("b"));

            QueryResult<bool> result = Query.Select(
                    row => row.Get(expression)
                )
                .NoFromClause()
                .Execute(TestDatabase.Database);

            Assert.HasCount(1, result.Rows);
            Assert.IsTrue(result.Rows[0]);
        }

        private class AB {
            public int A { get; set; }
            public int B { get; set; }
        }

        [TestMethod]
        public void Expression_03_Parameters_Test() {
            Settings.UseParameters = true;
            Expression_03_Test();
        }
        [TestMethod]
        public void Expression_03_No_Parameters_Test() {
            Settings.UseParameters = false;
            Expression_03_Test();
        }
        private static void Expression_03_Test() {

            Settings.UseParameters = true;

            AB json1 = new AB { A = 1, B = 2 };

            Expression<Jsonb> cast = new(SqlText.QuotedAsJson(json1), "::jsonb");

            QueryResult<Jsonb> result = Query.Select(
                    row => row.Get(cast)
                )
                .NoFromClause()
                .Execute(TestDatabase.Database);

            Assert.HasCount(1, result.Rows);

            AB? json2 = JsonSerializer.Deserialize<AB>(result.Rows[0].Value);

            Assert.IsNotNull(json2);

            Assert.AreEqual(json1.A, json2.A);
            Assert.AreEqual(json1.B, json2.B);
        }
    }
}