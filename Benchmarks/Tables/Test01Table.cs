﻿namespace Benchmarks.Tables {

    using System;
    using QueryLite;

    public interface ITest01 { }

    public sealed class Test01Table : ATable {

        public static readonly Test01Table Instance = new Test01Table();

        public Column<IntKey<ITest01>> Id { get; }
        public Column<Guid> Row_guid { get; }
        public Column<string> Message { get; }
        public Column<DateTime> Date { get; }

        public override PrimaryKey? PrimaryKey => new PrimaryKey(table: this, constraintName: "test01_pkey", Id);

        private Test01Table() : base(tableName: "test01", schemaName: "public") {

            Id = new Column<IntKey<ITest01>>(this, columnName: "id", isAutoGenerated: true);
            Row_guid = new Column<Guid>(this, columnName: "row_guid");
            Message = new Column<string>(this, columnName: "message", length: 100);
            Date = new Column<DateTime>(this, columnName: "date");
        }
    }
}