namespace QueryLiteTest.Tables {

    using QueryLite;
    using QueryLite.Utility;

    public interface IChild { }

    public sealed class ChildTable : ATable {

        public static readonly ChildTable Instance = new ChildTable();
        public static readonly ChildTable Instance2 = new ChildTable();
        public static readonly ChildTable Instance3 = new ChildTable();

        public Column<GuidKey<IChild>> Id { get; }

        public Column<GuidKey<IParent>> ParentId { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "pk_Child", Id);

        public override ForeignKey[] ForeignKeys => new ForeignKey[] {
            new ForeignKey(this, constraintName: "fk_Child_Parent").References(ParentId, ParentTable.Instance.Id),
            new ForeignKey(this, constraintName: "fk_Child_Parent_Id2").References(ParentId, ParentTable.Instance.Id2)
        };

        private ChildTable() : base(tableName: "Child", schemaName: "dbo") {

            Id = new Column<GuidKey<IChild>>(this, columnName: "Id");
            ParentId = new Column<GuidKey<IParent>>(this, columnName: "ParentId");
        }
    }
}