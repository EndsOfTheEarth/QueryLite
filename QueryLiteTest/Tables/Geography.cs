using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class GeoTestTable : ATable {

        public static readonly GeoTestTable Instance = new();

        public Column<GeoTestId, Guid> Guid { get; }
        public Column<IGeography> Geography { get; }

        public override PrimaryKey? PrimaryKey => new(table: this, name: "pk_GeoTest", Guid);

        private GeoTestTable() : base(tableName: "GeoTest", schemaName: "dbo") {

            Guid = new Column<GeoTestId, Guid>(this, name: "gtGuid");
            Geography = new Column<IGeography>(this, name: "gtGeography", length: ColumnLength.MAX);
        }
    }
}