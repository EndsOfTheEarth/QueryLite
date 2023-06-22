
namespace QueryLiteTest.Tables {

    using QueryLite;

    public interface IGeoTest { }

    public sealed class GeoTestTable : ATable {

        public static readonly GeoTestTable Instance = new GeoTestTable();

        public Column<GuidKey<IGeoTest>> Guid { get; }
        public Column<IGeography> Geography { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "pk_GeoTest", Guid);

        private GeoTestTable() : base(tableName: "GeoTest", schemaName: "dbo") {

            Guid = new Column<GuidKey<IGeoTest>>(this, columnName: "gtGuid");
            Geography = new Column<IGeography>(this, columnName: "gtGeography", length: int.MaxValue);
        }
    }
}