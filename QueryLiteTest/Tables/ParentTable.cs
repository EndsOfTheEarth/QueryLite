using QueryLite;
using System;

namespace QueryLiteTest.Tables {

    public sealed class ParentTable : ATable {

        public static readonly ParentTable Instance = new();
        public static readonly ParentTable Instance2 = new();

        public Column<ParentId, Guid> Id { get; }
        public Column<ParentId, Guid> Id2 { get; }

        public override PrimaryKey? PrimaryKey => new(table: this, name: "pk_Parent", Id);

        public override UniqueConstraint[] UniqueConstraints => [
            new UniqueConstraint(this, name: "unq_parent", Id2)
        ];

        private ParentTable() : base(name: "Parent", schemaName: "dbo") {
            Id = new Column<ParentId, Guid>(this, name: "Id");
            Id2 = new Column<ParentId, Guid>(this, name: "Id2");
        }
    }
}