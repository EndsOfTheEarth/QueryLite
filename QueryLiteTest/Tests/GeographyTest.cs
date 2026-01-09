using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using QueryLite.Databases.Functions;
using QueryLite.Databases.SqlServer.Functions;
using QueryLite.Functions;
using QueryLite.Utility;
using QueryLiteTest.Tables;
using System;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class GeographyTest {

        [TestInitialize]
        public void ClearTable() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            GeoTestTable table = GeoTestTable.Instance;

            using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                Query.Delete(table)
                    .NoWhereCondition()
                    .Execute(transaction, TimeoutLevel.ShortDelete);

                Count count = new();

                QueryResult<int> result = Query
                    .Select(
                        result => result.Get(count)
                    )
                    .From(table)
                    .Execute(transaction);

                Assert.AreEqual(1, result.Rows.Count);
                Assert.AreEqual(0, result.RowsEffected);

                int? countValue = result.Rows[0];

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
        public void TestGeographyFunctionsWithoutParams() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            Settings.UseParameters = false;
            GeographyTest.TestGeographyFunctions();
        }

        [TestMethod]
        public void TestGeographyFunctionsWithParams() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            try {

                Settings.UseParameters = true;

                GeographyTest.TestGeographyFunctions();
            }
            finally {
                Settings.UseParameters = false;
            }
        }

        private static void TestGeographyFunctions() {

            GeoTestTable table = GeoTestTable.Instance;

            STPointFromText stPointFromText = new STPointFromText(kwText: "POINT(-122.34900 47.65100)");

            GeoTestId guid = GeoTestId.ValueOf(Guid.NewGuid());

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
            STAsBinary stAsBinary = new STAsBinary(table.Geography);
            STAsText stAsText = new STAsText(table.Geography);

            Longitude longitude = new Longitude(table.Geography);
            Latitude latitude = new Latitude(table.Geography);

            STAsText stGeomFromTextAsText = new STAsText(new STGeomFromText(kwText: "LINESTRING(-121.360 47.646,-122.343 47.601)"));
            STAsText stLineFromTextAsText = new STAsText(new STLineFromText(kwText: "LINESTRING(-123.360 47.496,-121.323 47.256)"));
            STAsText stPolyFromTextAsText = new STAsText(new STPolyFromText(kwText: "POLYGON((-121.358 47.643, -121.348 47.629, -121.348 47.678, -121.358 47.668, -121.358 47.643))"));
            STAsText stMPointFromText = new STAsText(new STMPointFromText(kwText: "MULTIPOINT((-121.361 46.156), (-121.343 47.444))"));
            STAsText stMLineFromText = new STAsText(new STMLineFromText(kwText: "MULTILINESTRING ((-121.358 47.653, -121.348 47.649, -121.348 47.658, -121.358 47.658, -121.358 47.653), (-121.357 47.654, -121.357 47.657, -121.349 47.657, -121.349 47.651, -121.357 47.654))"));
            STAsText stMPolyFromText = new STAsText(new STMPolyFromText(kwText: "MULTIPOLYGON(((-111.228 47.313, -122.348 47.649, -122.358 47.658, -111.228 47.313)), ((-222.341 27.111, -122.341 46.661, -122.351 46.661, -222.341 27.111)))"));

            var result = Query
                .Select(
                    row => new {
                        Guid = row.Get(table.Guid),
                        Distance = row.Get(distance),
                        Binary = row.Get(stAsBinary),
                        Text = row.Get(stAsText),
                        Longitude = row.Get(longitude),
                        Latitude = row.Get(latitude),
                        STGeomFromTextAsText = row.Get(stGeomFromTextAsText),
                        STLineFromTextAsText = row.Get(stLineFromTextAsText),
                        STPolyFromTextAsText = row.Get(stPolyFromTextAsText),
                        STMPointFromText = row.Get(stMPointFromText),
                        STMLineFromText = row.Get(stMLineFromText),
                        STMPolyFromText = row.Get(stMPolyFromText)
                    }
                )
                .From(table)
                .Where(new STEquals(table.Geography, geographyPoint) == 1)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            var row = result.Rows[0];

            Assert.AreEqual(row.Guid, guid);
            Assert.AreEqual(0, row.Distance);
            Assert.AreEqual("01010000007593180456965EC017D9CEF753D34740", Convert.ToHexString(row.Binary!).Replace("-", ""));
            Assert.AreEqual("POINT (-122.349 47.651)", row.Text);
            Assert.AreEqual(-122.349, row.Longitude);
            Assert.AreEqual(47.651, row.Latitude);
            Assert.AreEqual("LINESTRING (-121.36 47.646, -122.343 47.601)", row.STGeomFromTextAsText);
            Assert.AreEqual("LINESTRING (-123.36 47.496, -121.323 47.256)", row.STLineFromTextAsText);
            Assert.AreEqual("POLYGON ((-121.358 47.643, -121.348 47.629, -121.348 47.678, -121.358 47.668, -121.358 47.643))", row.STPolyFromTextAsText);
            Assert.AreEqual("MULTIPOINT ((-121.361 46.156), (-121.343 47.444))", row.STMPointFromText);
            Assert.AreEqual("MULTILINESTRING ((-121.358 47.653, -121.348 47.649, -121.348 47.658, -121.358 47.658, -121.358 47.653), (-121.357 47.654, -121.357 47.657, -121.349 47.657, -121.349 47.651, -121.357 47.654))", row.STMLineFromText);
            Assert.AreEqual("MULTIPOLYGON (((-111.228 47.313, -122.348 47.649, -122.358 47.658, -111.228 47.313)), ((-222.341 27.111, -122.341 46.661, -122.351 46.661, -222.341 27.111)))", row.STMPolyFromText);
        }

        [TestMethod]
        public void TestGeographyDistanceWithoutParams() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            Settings.UseParameters = false;
            GeographyTest.TestGeographyDistance();
        }

        [TestMethod]
        public void TestGeographyDistanceWithParams() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            try {

                Settings.UseParameters = true;
                GeographyTest.TestGeographyDistance();
            }
            finally {
                Settings.UseParameters = false;
            }
        }

        private static void TestGeographyDistance() {

            GeoTestTable table = GeoTestTable.Instance;

            STLineFromText stLineFromText = new STLineFromText(kwText: "LINESTRING(-122.360 47.656, -122.343 47.656)");

            GeoTestId guid = GeoTestId.ValueOf(Guid.NewGuid());

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

            Assert.AreEqual(1, result.Rows.Count);

            var row = result.Rows[0];

            Assert.AreEqual(row.Guid, guid);

            Assert.AreEqual(555.94977427172694, row.Distance);
        }

        [TestMethod]
        public void TestSTFunctionsWithoutParams() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            Settings.UseParameters = false;
            GeographyTest.TestSTFunctions();
        }

        [TestMethod]
        public void TestSTFunctionsWithParams() {

            if(TestDatabase.Database.DatabaseType != DatabaseType.SqlServer) {
                return;
            }

            try {

                Settings.UseParameters = true;
                GeographyTest.TestSTFunctions();
            }
            finally {
                Settings.UseParameters = false;
            }
        }

        private static void TestSTFunctions() {

            GeoTestTable table = GeoTestTable.Instance;

            //Sql server only
            GeographyParse geography_Parse = new GeographyParse(kwtText: "CURVEPOLYGON (COMPOUNDCURVE (CIRCULARSTRING (-122.200928 47.454094, -122.810669 47.00648, -122.942505 46.687131, -121.14624 45.786679, -119.119263 46.183634), (-119.119263 46.183634, -119.273071 47.107523, -120.640869 47.569114, -122.200928 47.454094)))");

            GeoTestId guid = GeoTestId.ValueOf(Guid.NewGuid());

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

            STArea stArea = new STArea(table.Geography);

            var result = Query
                .Select(
                    row => new {
                        Guid = row.Get(table.Guid),
                        ContainsA = row.Get(stContainsA),
                        ContainsB = row.Get(stContainsB),
                        Area = row.Get(stArea)
                    }
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);

            var row = result.Rows[0];

            Assert.AreEqual(row.Guid, guid);

            Assert.AreEqual(row.ContainsA, Bit.TRUE);
            Assert.AreEqual(row.ContainsB, Bit.FALSE);
            Assert.AreEqual(45023599772.742432, row.Area);
        }
    }
}