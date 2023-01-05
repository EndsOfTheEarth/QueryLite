using System;

namespace QueryLite.Databases.SqlServer.Functions {

    public sealed class CountAll : Function<int> {

        public CountAll() : base("COUNT(*)") { }

        public override string GetSql(IDatabase database, bool useAlias, IParameters? parameters) {
            return "COUNT(*)";
        }
    }

    public sealed class GetDate : Function<DateTime> {

        public GetDate() : base("GETDATE()") { }

        public override string GetSql(IDatabase database, bool useAlias, IParameters? parameters) {
            return "GETDATE()";
        }
    }

    public sealed class GetDateTimeOffset : Function<DateTimeOffset> {

        public GetDateTimeOffset() : base("SYSDATETIMEOFFSET()") { }

        public override string GetSql(IDatabase database, bool useAlias, IParameters? parameters) {
            return "SYSDATETIMEOFFSET()";
        }
    }
}