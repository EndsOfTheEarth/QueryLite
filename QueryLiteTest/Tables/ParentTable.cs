namespace QueryLiteTest.Tables {

    using QueryLite;
    using System;

    public interface IParent { }

    public sealed class ParentTable : ATable {

        public static readonly ParentTable Instance = new ParentTable();

        public Column<GuidKey<IParent>> Id { get; }
        public Column<GuidKey<IParent>> Id2 { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "pk_Parent", Id);

        private ParentTable() : base(tableName: "Parent", schemaName: "dbo") {

            Id = new Column<GuidKey<IParent>>(this, columnName: "Id");
            Id2 = new Column<GuidKey<IParent>>(this, columnName: "Id2");
        }
    }
}