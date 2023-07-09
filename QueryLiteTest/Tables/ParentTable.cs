namespace QueryLiteTest.Tables {

    using QueryLite;

    public interface IParent { }

    public sealed class ParentTable : ATable {

        public static readonly ParentTable Instance = new ParentTable();

        public Column<GuidKey<IParent>> Id { get; }
        public Column<GuidKey<IParent>> Id2 { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "pk_Parent", Id);

        public override UniqueConstraint[] UniqueConstraints => new UniqueConstraint[] {
            new UniqueConstraint(this, constraintName: "unq_parent", Id2)
        };

        private ParentTable() : base(tableName: "Parent", schemaName: "dbo") {

            Id = new Column<GuidKey<IParent>>(this, columnName: "Id");
            Id2 = new Column<GuidKey<IParent>>(this, columnName: "Id2");
        }
    }
}