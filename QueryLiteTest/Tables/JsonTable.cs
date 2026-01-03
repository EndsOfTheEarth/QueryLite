using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class JsonTable : ATable {

        public static readonly JsonTable Instance = new();

        public Column<Guid> Id { get; }
        public Column<Jsonb> Detail { get; }

        public override PrimaryKey? PrimaryKey => new(this, name: "pk_JsonTable", Id);

        private JsonTable() : base(tableName: "jsontable", schemaName: "public") {
            Id = new Column<Guid>(this, columnName: "id");
            Detail = new Column<Jsonb>(this, columnName: "detail");
        }
    }

    [Repository<JsonTable>(MatchOn.AllColumns, "JsonRepository")]
    public partial record JsonRow {

    }
}