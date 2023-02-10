namespace QueryLiteTest.Tables {

    using QueryLite;
    using System;

    public interface IParent { }

    public sealed class ParentTable : ATable {

        public static readonly ParentTable Instance = new ParentTable();
        public static readonly ParentTable Instance2 = new ParentTable();
        public static readonly ParentTable Instance3 = new ParentTable();

        [PrimaryKey("pk_Parent")]
        public Column<GuidKey<IParent>> Id { get; }

        public Column<Guid> Id2 { get; }

        private ParentTable() : base(tableName: "Parent", schemaName: "dbo") {

            Id = new Column<GuidKey<IParent>>(this, columnName: "Id", isPrimaryKey: true);
            Id2 = new Column<Guid>(this, columnName: "Id2");
        }
    }
}