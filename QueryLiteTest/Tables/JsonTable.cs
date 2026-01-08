using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class JsonTable : ATable {

        public static readonly JsonTable Instance = new();

        public Column<Guid> Id { get; }
        public Column<Jsonb> Detail { get; }

        public override PrimaryKey? PrimaryKey => new(this, name: "pk_JsonTable", Id);

        private JsonTable() : base(name: "jsontable", schemaName: "dbo") {
            Id = new Column<Guid>(this, name: "id");
            Detail = new Column<Jsonb>(this, name: "detail", length: ColumnLength.MAX);
        }
    }

    [Repository<JsonTable>(MatchOn.AllColumns, "JsonRepository")]
    public partial record JsonRow {

    }
}