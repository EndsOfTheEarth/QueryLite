using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
        public async Task Test01() {

            CustomTypesRow rowA = GetCustomTypesA();
            
            CustomTypesRepository repository = new CustomTypesRepository();

            { //Test insert
                repository.AddNewRow(rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.UpdateAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                AssertOnlyOneRowExists();
                await AssertCustomTypesAsync(rowA);
            }

            CustomTypesRow rowB = GetCustomTypesB();

            {   //Test update

                CopyValuesTo(from: rowB, to: rowA);

                using(Transaction transaction = new Transaction(TestDatabase.Database)) {

                    await repository.UpdateAsync(transaction, TimeoutLevel.ShortInsert, CancellationToken.None);
                    await transaction.CommitAsync();
                }
                AssertOnlyOneRowExists();
                await AssertCustomTypesAsync(rowA);
            }
        }

        private static void AssertOnlyOneRowExists() {

            CustomTypesTable table = CustomTypesTable.Instance;

            COUNT_ALL count = COUNT_ALL.Instance;

            QueryResult<int> result = Query.Select(
                    row => row.Get(count)
                )
                .From(table)
                .Execute(TestDatabase.Database);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(1, result.Rows[0]);
        }

        private static void CopyValuesTo(CustomTypesRow from, CustomTypesRow to) {

            to.CustomGuid = from.CustomGuid;
            to.CustomShort = from.CustomShort;
            to.CustomInt = from.CustomInt;
            to.CustomLong = from.CustomLong;
            to.CustomString = from.CustomString;
            to.CustomBool = from.CustomBool;
            to.CustomDecimal = from.CustomDecimal;
            to.CustomDateTime = from.CustomDateTime;
            to.CustomDateTimeOffset = from.CustomDateTimeOffset;
            to.CustomDateOnly = from.CustomDateOnly;
            to.CustomTimeOnly = from.CustomTimeOnly;
            to.CustomFloat = from.CustomFloat;
            to.CustomDouble = from.CustomDouble;

            to.NCustomGuid = from.NCustomGuid;
            to.NCustomShort = from.NCustomShort;
            to.NCustomInt = from.NCustomInt;
            to.NCustomLong = from.NCustomLong;
            to.NCustomString = from.NCustomString;
            to.NCustomBool = from.NCustomBool;
            to.NCustomDecimal = from.NCustomDecimal;
            to.NCustomDateTime = from.NCustomDateTime;
            to.NCustomDateTimeOffset = from.NCustomDateTimeOffset;
            to.NCustomDateOnly = from.NCustomDateOnly;
            to.NCustomTimeOnly = from.NCustomTimeOnly;
            to.NCustomFloat = from.NCustomFloat;
            to.NCustomDouble = from.NCustomDouble;
        }

        private static async Task AssertCustomTypesAsync(CustomTypesRow row) {

            CustomTypesRepository repository = new CustomTypesRepository();

            await repository
                .SelectAll
                .Where(repository.Table.Guid == row.CustomGuid)
                .ExecuteAsync(TestDatabase.Database, CancellationToken.None);

            Assert.AreEqual(1, repository.Count);

            CustomTypesRow values = repository[0];

            Assert.AreEqual(row.CustomGuid, values.CustomGuid);
            Assert.AreEqual(row.CustomShort, values.CustomShort);
            Assert.AreEqual(row.CustomInt, values.CustomInt);
            Assert.AreEqual(row.CustomLong, values.CustomLong);
            Assert.AreEqual(row.CustomString, values.CustomString);
            Assert.AreEqual(row.CustomBool, values.CustomBool);
            Assert.AreEqual(row.CustomDecimal, values.CustomDecimal);
            Assert.AreEqual(row.CustomDateTime, values.CustomDateTime);
            Assert.AreEqual(row.CustomDateTimeOffset, values.CustomDateTimeOffset);
            Assert.AreEqual(row.CustomDateOnly, values.CustomDateOnly);
            Assert.AreEqual(row.CustomTimeOnly, values.CustomTimeOnly);
            Assert.AreEqual(row.CustomFloat, values.CustomFloat);
            Assert.AreEqual(row.CustomDouble, values.CustomDouble);

            Assert.AreEqual(row.NCustomGuid, values.NCustomGuid);
            Assert.AreEqual(row.NCustomShort, values.NCustomShort);
            Assert.AreEqual(row.NCustomInt, values.NCustomInt);
            Assert.AreEqual(row.NCustomLong, values.NCustomLong);
            Assert.AreEqual(row.NCustomString, values.NCustomString);
            Assert.AreEqual(row.NCustomBool, values.NCustomBool);
            Assert.AreEqual(row.NCustomDecimal, values.NCustomDecimal);
            Assert.AreEqual(row.NCustomDateTime, values.NCustomDateTime);
            Assert.AreEqual(row.NCustomDateTimeOffset, values.NCustomDateTimeOffset);
            Assert.AreEqual(row.NCustomDateOnly, values.NCustomDateOnly);
            Assert.AreEqual(row.NCustomTimeOnly, values.NCustomTimeOnly);
            Assert.AreEqual(row.NCustomFloat, values.NCustomFloat);
            Assert.AreEqual(row.NCustomDouble, values.NCustomDouble);
        }

        public sealed class CustomTypesRepository : ARepository<CustomTypesTable, CustomTypesRow>, IRepository<CustomTypesTable, CustomTypesRow> {

            public CustomTypesRepository() : base(CustomTypesTable.Instance, concurrencyCheck: false) { }
        }

        public record class CustomTypesRow : IRepositoryRow<CustomTypesTable, CustomTypesRow> {

            public CustomGuid PreviousCustomGuid { get; set; }

            public CustomGuid CustomGuid { get; set; }
            public CustomShort CustomShort { get; set; }
            public CustomInt CustomInt { get; set; }
            public CustomLong CustomLong { get; set; }
            public CustomString CustomString { get; set; }
            public CustomBool CustomBool { get; set; }
            public CustomDecimal CustomDecimal { get; set; }
            public CustomDateTime CustomDateTime { get; set; }
            public CustomDateTimeOffset CustomDateTimeOffset { get; set; }
            public CustomDateOnly CustomDateOnly { get; set; }
            public CustomTimeOnly CustomTimeOnly { get; set; }
            public CustomFloat CustomFloat { get; set; }
            public CustomDouble CustomDouble { get; set; }

            public CustomGuid? NCustomGuid { get; set; }
            public CustomShort? NCustomShort { get; set; }
            public CustomInt? NCustomInt { get; set; }
            public CustomLong? NCustomLong { get; set; }
            public CustomString? NCustomString { get; set; }
            public CustomBool? NCustomBool { get; set; }
            public CustomDecimal? NCustomDecimal { get; set; }
            public CustomDateTime? NCustomDateTime { get; set; }
            public CustomDateTimeOffset? NCustomDateTimeOffset { get; set; }
            public CustomDateOnly? NCustomDateOnly { get; set; }
            public CustomTimeOnly? NCustomTimeOnly { get; set; }
            public CustomFloat? NCustomFloat { get; set; }
            public CustomDouble? NCustomDouble { get; set; }

            public CustomTypesRow(CustomGuid customGuid, CustomShort customShort, CustomInt customInt,
                               CustomLong customLong, CustomString customString, CustomBool customBool,
                               CustomDecimal customDecimal, CustomDateTime customDateTime,
                               CustomDateTimeOffset customDateTimeOffset, CustomDateOnly customDateOnly,
                               CustomTimeOnly customTimeOnly, CustomFloat customFloat, CustomDouble customDouble,
                               CustomGuid? nCustomGuid, CustomShort? nCustomShort, CustomInt? nCustomInt,
                               CustomLong? nCustomLong, CustomString? nCustomString, CustomBool? nCustomBool,
                               CustomDecimal? nCustomDecimal, CustomDateTime? nCustomDateTime,
                               CustomDateTimeOffset? nCustomDateTimeOffset, CustomDateOnly? nCustomDateOnly,
                               CustomTimeOnly? nCustomTimeOnly, CustomFloat? nCustomFloat, CustomDouble? nCustomDouble) {
                CustomGuid = customGuid;
                CustomShort = customShort;
                CustomInt = customInt;
                CustomLong = customLong;
                CustomString = customString;
                CustomBool = customBool;
                CustomDecimal = customDecimal;
                CustomDateTime = customDateTime;
                CustomDateTimeOffset = customDateTimeOffset;
                CustomDateOnly = customDateOnly;
                CustomTimeOnly = customTimeOnly;
                CustomFloat = customFloat;
                CustomDouble = customDouble;
                NCustomGuid = nCustomGuid;
                NCustomShort = nCustomShort;
                NCustomInt = nCustomInt;
                NCustomLong = nCustomLong;
                NCustomString = nCustomString;
                NCustomBool = nCustomBool;
                NCustomDecimal = nCustomDecimal;
                NCustomDateTime = nCustomDateTime;
                NCustomDateTimeOffset = nCustomDateTimeOffset;
                NCustomDateOnly = nCustomDateOnly;
                NCustomTimeOnly = nCustomTimeOnly;
                NCustomFloat = nCustomFloat;
                NCustomDouble = nCustomDouble;
            }

            public static CustomTypesRow CloneRow(CustomTypesRow row) => row with { };

            public static List<(IColumn, Func<CustomTypesRow, object?>)> GetColumnMap(CustomTypesTable table) =>
            [
                (table.Guid, (row) => row.CustomGuid),
                (table.Short, (row) => row.CustomShort),
                (table.Int, (row) => row.CustomInt),
                (table.Long, (row) => row.CustomLong),
                (table.String, (row) => row.CustomString),
                (table.Bool, (row) => row.CustomBool),
                (table.Decimal, (row) => row.CustomDecimal),
                (table.DateTime, (row) => row.CustomDateTime),
                (table.DateTimeOffset, (row) => row.CustomDateTimeOffset),
                (table.DateOnly, (row) => row.CustomDateOnly),
                (table.TimeOnly, (row) => row.CustomTimeOnly),
                (table.Float, (row) => row.CustomFloat),
                (table.Double, (row) => row.CustomDouble),

                (table.NGuid, (row) => row.NCustomGuid),
                (table.NShort, (row) => row.NCustomShort),
                (table.NInt, (row) => row.NCustomInt),
                (table.NLong, (row) => row.NCustomLong),
                (table.NString, (row) => row.NCustomString),
                (table.NBool, (row) => row.NCustomBool),
                (table.NDecimal, (row) => row.NCustomDecimal),
                (table.NDateTime, (row) => row.NCustomDateTime),
                (table.NDateTimeOffset, (row) => row.NCustomDateTimeOffset),
                (table.NDateOnly, (row) => row.NCustomDateOnly),
                (table.NTimeOnly, (row) => row.NCustomTimeOnly),
                (table.NFloat, (row) => row.NCustomFloat),
                (table.NDouble, (row) => row.NCustomDouble)
            ];

            public static CustomTypesRow LoadRow(CustomTypesTable table, IResultRow resultRow) {

                CustomTypesRow row = new CustomTypesRow(

                    customGuid: resultRow.Get(table.Guid),
                    customShort: resultRow.Get(table.Short),
                    customInt: resultRow.Get(table.Int),
                    customLong: resultRow.Get(table.Long),
                    customString: resultRow.Get(table.String),
                    customBool: resultRow.Get(table.Bool),
                    customDecimal: resultRow.Get(table.Decimal),
                    customDateTime: resultRow.Get(table.DateTime),
                    customDateTimeOffset: resultRow.Get(table.DateTimeOffset),
                    customDateOnly: resultRow.Get(table.DateOnly),
                    customTimeOnly: resultRow.Get(table.TimeOnly),
                    customFloat: resultRow.Get(table.Float),
                    customDouble: resultRow.Get(table.Double),

                    nCustomGuid: resultRow.Get(table.NGuid),
                    nCustomShort: resultRow.Get(table.NShort),
                    nCustomInt: resultRow.Get(table.NInt),
                    nCustomLong: resultRow.Get(table.NLong),
                    nCustomString: resultRow.Get(table.NString),
                    nCustomBool: resultRow.Get(table.NBool),
                    nCustomDecimal: resultRow.Get(table.NDecimal),
                    nCustomDateTime: resultRow.Get(table.NDateTime),
                    nCustomDateTimeOffset: resultRow.Get(table.NDateTimeOffset),
                    nCustomDateOnly: resultRow.Get(table.NDateOnly),
                    nCustomTimeOnly: resultRow.Get(table.NTimeOnly),
                    nCustomFloat: resultRow.Get(table.NFloat),
                    nCustomDouble: resultRow.Get(table.NDouble)
                );
                return row;
            }
        }

        private static CustomTypesRow GetCustomTypesA() => new CustomTypesRow(

            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(12),
            customInt: CustomInt.ValueOf(55),
            customLong: CustomLong.ValueOf(43213412),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(true),
            customDecimal: CustomDecimal.ValueOf(0.22345200m),
            customDateTime: CustomDateTime.ValueOf(new DateTime(year: 2025, month: 03, day: 10, hour: 15, minute: 01, second: 7)),
            customDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2024, month: 02, day: 11, hour: 22, minute: 52, second: 12, new TimeSpan(hours: 5, minutes: 0, seconds: 0))),
            customDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 2025, month: 05, day: 12)),
            customTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 01, minute: 00, second: 25)),
            customFloat: CustomFloat.ValueOf(342.1234423f),
            customDouble: CustomDouble.ValueOf(45152345234.234523452345d),

            nCustomGuid: null,
            nCustomShort: null,
            nCustomInt: null,
            nCustomLong: null,
            nCustomString: null,
            nCustomBool: null,
            nCustomDecimal: null,
            nCustomDateTime: null,
            nCustomDateTimeOffset: null,
            nCustomDateOnly: null,
            nCustomTimeOnly: null,
            nCustomFloat: null,
            nCustomDouble: null
        );

        private static CustomTypesRow GetCustomTypesB() => new CustomTypesRow(

            customGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            customShort: CustomShort.ValueOf(43),
            customInt: CustomInt.ValueOf(2384234),
            customLong: CustomLong.ValueOf(6525234),
            customString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            customBool: CustomBool.ValueOf(false),
            customDecimal: CustomDecimal.ValueOf(-23452345.65474567m),
            customDateTime: CustomDateTime.ValueOf(new DateTime(year: 2010, month: 01, day: 30, hour: 17, minute: 02, second: 7)),
            customDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 1990, month: 10, day: 11, hour: 21, minute: 05, second: 01, new TimeSpan(hours: 12, minutes: 0, seconds: 0))),
            customDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 2024, month: 06, day: 13)),
            customTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 04, minute: 01, second: 27)),
            customFloat: CustomFloat.ValueOf(3.14535f),
            customDouble: CustomDouble.ValueOf(92345234523.98726452345d),

            nCustomGuid: CustomGuid.ValueOf(Guid.NewGuid()),
            nCustomShort: CustomShort.ValueOf(11),
            nCustomInt: CustomInt.ValueOf(3232),
            nCustomLong: CustomLong.ValueOf(2414564),
            nCustomString: CustomString.ValueOf(Guid.NewGuid().ToString()),
            nCustomBool: CustomBool.ValueOf(true),
            nCustomDecimal: CustomDecimal.ValueOf(2134.45234m),
            nCustomDateTime: CustomDateTime.ValueOf(new DateTime(year: 2023, month: 9, day: 11, hour: 15, minute: 01, second: 7)),
            nCustomDateTimeOffset: CustomDateTimeOffset.ValueOf(new DateTimeOffset(year: 2023, month: 05, day: 12, hour: 10, minute: 52, second: 15, new TimeSpan(hours: 7, minutes: 0, seconds: 0))),
            nCustomDateOnly: CustomDateOnly.ValueOf(new DateOnly(year: 1990, month: 12, day: 31)),
            nCustomTimeOnly: CustomTimeOnly.ValueOf(new TimeOnly(hour: 23, minute: 59, second: 59)),
            nCustomFloat: CustomFloat.ValueOf(123423.2345234f),
            nCustomDouble: CustomDouble.ValueOf(73458.22347589234d)
        );
    }
}