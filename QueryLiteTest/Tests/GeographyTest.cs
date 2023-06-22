using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.Functions;
using QueryLite.Databases.SqlServer.Functions;
using QueryLiteTest.Tables;
using System;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class GeographyTest {

        [TestInitialize]
        public void ClearTable() {

            GeoTestTable table = GeoTestTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(table)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                COUNT_ALL count = COUNT_ALL.Instance;

                QueryResult<int> result = Query
                    .Select(
                        result => result.Get(count)
                    )
                    .From(table)
                    .Execute(transaction);

                Assert.AreEqual(result.Rows.Count, 1);
                Assert.AreEqual(result.RowsEffected, 0);

                int? countValue = result.Rows[0];

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
        public void TestGeographyFunctionsWithoutParams() {
            Settings.UseParameters = false;
            TestGeographyFunctions();
        }

        [TestMethod]
        public void TestGeographyFunctionsWithParams() {

            try {

                Settings.UseParameters = true;

                TestGeographyFunctions();
            }
            finally {
                Settings.UseParameters = false;
            }
        }

        private void TestGeographyFunctions() {

            GeoTestTable table = GeoTestTable.Instance;

            STPointFromText stPointFromText = new STPointFromText(kwText: "POINT(-122.34900 47.65100)");

            GuidKey<IGeoTest> guid = GuidKey<IGeoTest>.ValueOf(Guid.NewGuid());

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult insertResult = Query
                    .Insert(table)
                    .Values(values => values
                        .Set(table.Guid, guid)
                        .Set(table.Geography, stPointFromText)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            GeographyPoint geographyPoint = new GeographyPoint(latitude: 47.65100, longitude: -122.34900);
            STDistance distance = new STDistance(table.Geography, geographyPoint);

            var result = Query
                .Select(
                    row => new {
                        Guid = row.Get(table.Guid),
                        Distance = row.Get(distance)
                    }
                )
                .From(table)
                .Where(new STEquals(table.Geography, geographyPoint) == 1)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);

            var row = result.Rows[0];

            Assert.AreEqual(row.Guid, guid);
            Assert.AreEqual(row.Distance, 0);
        }

        [TestMethod]
        public void TestGeographyDistanceWithoutParams() {
            Settings.UseParameters = false;
            TestGeographyDistance();
        }

        [TestMethod]
        public void TestGeographyDistanceWithParams() {

            try {

                Settings.UseParameters = true;
                TestGeographyDistance();
            }
            finally {
                Settings.UseParameters = false;
            }
        }

        private void TestGeographyDistance() {

            GeoTestTable table = GeoTestTable.Instance;

            STLineFromText stLineFromText = new STLineFromText(kwText: "LINESTRING(-122.360 47.656, -122.343 47.656)");

            GuidKey<IGeoTest> guid = GuidKey<IGeoTest>.ValueOf(Guid.NewGuid());

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult insertResult = Query
                    .Insert(table)
                    .Values(values => values
                        .Set(table.Guid, guid)
                        .Set(table.Geography, stLineFromText)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            GeographyPoint geographyPoint = new GeographyPoint(latitude: 47.65100, longitude: -122.34900);
            STDistance distance = new STDistance(table.Geography, geographyPoint);

            var result = Query
                .Select(
                    row => new {
                        Guid = row.Get(table.Guid),
                        Distance = row.Get(distance)
                    }
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);

            var row = result.Rows[0];

            Assert.AreEqual(row.Guid, guid);

            Assert.AreEqual(row.Distance, 555.94977427172694);
        }

        [TestMethod]
        public void TestSTContainsWithoutParams() {
            Settings.UseParameters = false;
            TestSTContains();
        }

        [TestMethod]
        public void TestSTContainsWithParams() {

            try {

                Settings.UseParameters = true;
                TestSTContains();
            }
            finally {
                Settings.UseParameters = false;
            }
        }

        private void TestSTContains() {

            GeoTestTable table = GeoTestTable.Instance;

            //Sql server only
            Geography_Parse geography_Parse = new Geography_Parse(kwtText: "CURVEPOLYGON (COMPOUNDCURVE (CIRCULARSTRING (-122.200928 47.454094, -122.810669 47.00648, -122.942505 46.687131, -121.14624 45.786679, -119.119263 46.183634), (-119.119263 46.183634, -119.273071 47.107523, -120.640869 47.569114, -122.200928 47.454094)))");

            GuidKey<IGeoTest> guid = GuidKey<IGeoTest>.ValueOf(Guid.NewGuid());

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                NonQueryResult insertResult = Query
                    .Insert(table)
                    .Values(values => values
                        .Set(table.Guid, guid)
                        .Set(table.Geography, geography_Parse)
                    )
                    .Execute(transaction);

                transaction.Commit();
            }

            STContains stContainsA = new STContains(table.Geography, new GeographyPoint(latitude: 46.893985, longitude: -121.703796));
            STContains stContainsB = new STContains(table.Geography, new GeographyPoint(latitude: 47.65100, longitude: -122.34900));

            var result = Query
                .Select(
                    row => new {
                        Guid = row.Get(table.Guid),
                        ContainsA = row.Get(stContainsA),
                        ContainsB = row.Get(stContainsB)
                    }
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(result.Rows.Count, 1);

            var row = result.Rows[0];

            Assert.AreEqual(row.Guid, guid);

            Assert.AreEqual(row.ContainsA, Bit.TRUE);
            Assert.AreEqual(row.ContainsB, Bit.FALSE);
        }
    }
}