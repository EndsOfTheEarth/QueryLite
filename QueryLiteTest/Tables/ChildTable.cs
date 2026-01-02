using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class ChildTable : ATable {

        public static readonly ChildTable Instance = new();
        public static readonly ChildTable Instance2 = new();
        public static readonly ChildTable Instance3 = new();

        public Column<ChildId, Guid> Id { get; }

        public Column<ParentId, Guid> ParentId { get; }

        public override PrimaryKey? PrimaryKey => new(table: this, name: "pk_Child", Id);

        public override ForeignKey[] ForeignKeys => [
            new ForeignKey(this, name: "fk_Child_Parent").References(ParentId, ParentTable.Instance.Id),
            new ForeignKey(this, name: "fk_Child_Parent_Id2").References(ParentId, ParentTable.Instance.Id2)
        ];

        private ChildTable() : base(tableName: "Child", schemaName: "dbo") {
            Id = new Column<ChildId, Guid>(this, columnName: "Id");
            ParentId = new Column<ParentId, Guid>(this, columnName: "ParentId");
        }
    }
}