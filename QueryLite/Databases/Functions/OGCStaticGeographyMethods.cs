
namespace QueryLite.Databases.Functions {

    public interface IGeographySqlType {

        internal string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters);
    }

    public sealed class STGeomFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STGeomFromText(string kwText, int srid = 4326) : base(name: "STGeomFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STGeomFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STPointFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STPointFromText(string kwText, int srid = 4326) : base(name: "STPointFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STPointFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STLineFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STLineFromText(string kwText, int srid = 4326) : base(name: "STLineFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STLineFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STPolyFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STPolyFromText(string kwText, int srid = 4326) : base(name: "STPolyFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STPolyFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STMPointFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STMPointFromText(string kwText, int srid = 4326) : base(name: "STMPointFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STMPointFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STMLineFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STMLineFromText(string kwText, int srid = 4326) : base(name: "STMLineFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STMLineFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STMPolyFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STMPolyFromText(string kwText, int srid = 4326) : base(name: "STMPolyFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STMPolyFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STGeomCollFromText : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STGeomCollFromText(string kwText, int srid = 4326) : base(name: "STGeomCollFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STGeomCollFromText('{KwText}', {SRID})";
        }
    }

    public sealed class STGeomCollFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STGeomCollFromWKB(string kwBinary, int srid = 4326) : base(name: "STGeomCollFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STGeomCollFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STGeomFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STGeomFromWKB(string kwBinary, int srid = 4326) : base(name: "STGeomFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STGeomFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STPointFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STPointFromWKB(string kwBinary, int srid = 4326) : base(name: "STPointFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STPointFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STLineFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STLineFromWKB(string kwBinary, int srid = 4326) : base(name: "STLineFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STLineFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STPolyFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STPolyFromWKB(string kwBinary, int srid = 4326) : base(name: "STPolyFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STPolyFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STMPointFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STMPointFromWKB(string kwBinary, int srid = 4326) : base(name: "STMPointFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STMPointFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STMLineFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STMLineFromWKB(string kwBinary, int srid = 4326) : base(name: "STMLineFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STMLineFromWKB({KwBinary}, {SRID})";
        }
    }

    public sealed class STMPolyFromWKB : NullableFunction<IGeographyType>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STMPolyFromWKB(string kwBinary, int srid = 4326) : base(name: "STMPolyFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STMPolyFromWKB({KwBinary}, {SRID})";
        }
    }
}