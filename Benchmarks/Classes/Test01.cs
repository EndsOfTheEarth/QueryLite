namespace Benchmarks.Classes {

    using System;
    using QueryLite;
    using Benchmarks.Tables;

    public sealed class Test01 {

        public Test01(IntKey<ITest01> id, Guid row_guid, string message, DateTime date) {
            Id = id;
            Row_guid = row_guid;
            Message = message;
            Date = date;
        }

        public Test01(Test01Table table, IResultRow row) {
            Id = row.Get(table.Id);
            Row_guid = row.Get(table.Row_guid);
            Message = row.Get(table.Message);
            Date = row.Get(table.Date);
        }
        public IntKey<ITest01> Id { get; set; }
        public Guid Row_guid { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}