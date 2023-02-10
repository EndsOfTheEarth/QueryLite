namespace QueryLiteTest.Tables {

    using QueryLite;

    public interface IChild { }

    public sealed class ChildTable : ATable {

        public static readonly ChildTable Instance = new ChildTable();
        public static readonly ChildTable Instance2 = new ChildTable();
        public static readonly ChildTable Instance3 = new ChildTable();

        [PrimaryKey("pk_Child")]
        public Column<GuidKey<IChild>> Id { get; }

        [ForeignKey<ParentTable>("fk_Child_Parent")]
        [ForeignKey<ParentTable>("fk_Child_Parent_Id2")]
        public Column<GuidKey<IParent>> ParentId { get; }

        private ChildTable() : base(tableName: "Child", schemaName: "dbo") {

            Id = new Column<GuidKey<IChild>>(this, columnName: "Id", isPrimaryKey: true);
            ParentId = new Column<GuidKey<IParent>>(this, columnName: "ParentId");
        }
    }
}