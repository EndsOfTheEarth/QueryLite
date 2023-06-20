namespace QueryLite.Databases.Functions {

    /*
        Standard Type Methods yet to be implemented

        STBuffer
        STConvexHull
        STCurveN (geography Data Type)
        STCurveToLine (geography Data Type)
        STDifference
        STDimension
        STDisjoint
        STEndpoint
        STEquals
        STGeometryN
        STGeometryType
        STIntersection
        STIntersects
        STIsClosed
        STIsEmpty
        STIsValid
        STLength
        STNumCurves (geography Data Type)
        STNumGeometries
        STNumPoints
        STOverlaps
        STPointN
        STSrid
        STStartPoint
        STSymDifference
        STUnion
        STWithin
    */

    /// <summary>
    /// Geography function for defining a Point
    /// e.g. geography::Point(Latitude},Longitude,SRID)
    /// </summary>
    public sealed class GeographyPoint : Function<IGeographyType>, IGeographySqlType {

        public double Latitude { get; }
        public double Longitude { get; }
        public int SRID { get; }

        public GeographyPoint(double latitude, double longitude, int srid = 4326) : base(name: "geography::Point") {
            Latitude = latitude;
            Longitude = longitude;
            SRID = srid;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::Point({Latitude},{Longitude},{SRID})";
        }
    }

    public sealed class STArea : NullableFunction<double> {

        private AColumn<IGeographyType>? Column { get; }
        private IGeographySqlType? OCGType { get; }

        public STArea(AColumn<IGeographyType> column) : base(name: "STArea") {
            Column = column;
        }
        public STArea(IGeographySqlType? oCGType) : base(name: "STArea") {
            OCGType = oCGType;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(Column is not null) {
                return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.STArea()" : $"{Column.ColumnName}.STArea()";
            }
            else {                
                return $"{OCGType!.GetSql(database, useAlias: useAlias, parameters)}.STArea()";
            }
        }
    }

    public sealed class STAsBinary : NullableFunction<byte[]> {

        private AColumn<IGeographyType>? Column { get; }
        private IGeographySqlType? OCGType { get; }

        public STAsBinary(AColumn<IGeographyType> column) : base(name: "STAsBinary") {
            Column = column;
        }
        public STAsBinary(IGeographySqlType? oCGType) : base(name: "STAsBinary") {
            OCGType = oCGType;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(Column is not null) {
                return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.STAsBinary()" : $"{Column.ColumnName}.STAsBinary()";
            }
            else {
                return $"{OCGType!.GetSql(database, useAlias: useAlias, parameters)}.STAsBinary()";
            }
        }
    }

    public sealed class STAsText : NullableFunction<string> {

        private AColumn<IGeographyType>? Column { get; }
        private IGeographySqlType? OCGType { get; }

        public STAsText(AColumn<IGeographyType> column) : base(name: "STAsText") {
            Column = column;
        }
        public STAsText(IGeographySqlType? oCGType) : base(name: "STAsText") {
            OCGType = oCGType;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(Column is not null) {
                return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.STAsText()" : $"{Column.ColumnName}.STAsText()";
            }
            else {
                return $"{OCGType!.GetSql(database, useAlias: useAlias, parameters)}.STAsText()";
            }
        }
    }

    public sealed class STContains : NullableFunction<double> {

        private AColumn<IGeographyType> ColumnA { get; }
        private AColumn<IGeographyType>? ColumnB { get; }
        private IGeographySqlType? ToGeography { get; }

        public STContains(AColumn<IGeographyType> columnA, AColumn<IGeographyType> columnB) : base(name: "STContains") {
            ColumnA = columnA;
            ColumnB = columnB;
        }
        public STContains(AColumn<IGeographyType> columnA, IGeographySqlType? geographyB) : base(name: "STContains") {
            ColumnA = columnA;
            ToGeography = geographyB;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            string sql;

            if(ColumnB is not null) {

                if(useAlias) {
                    sql = $"{ColumnA.Table.Alias}.{ColumnA.ColumnName}.STContains({ColumnB.Table.Alias}.{ColumnB.ColumnName})";
                }
                else {
                    sql = $"{ColumnA.ColumnName}.STContains({ColumnB.ColumnName})";
                }
            }
            else {

                string toPointSql = ToGeography!.GetSql(database, useAlias: useAlias, parameters);

                if(useAlias) {
                    sql = $"{ColumnA.Table.Alias}.{ColumnA.ColumnName}.STContains({toPointSql})";
                }
                else {
                    sql = $"{ColumnA.ColumnName}.STContains({toPointSql})";
                }
            }
            return sql;
        }
    }

    /// <summary>
    /// Geography function for measuring the shortest distance between two geographies
    /// e.g. select columnA.STDistance(columnB) from table
    /// </summary>
    public sealed class STDistance : NullableFunction<double> {

        private AColumn<IGeographyType> FromColumn { get; }
        private AColumn<IGeographyType>? ToColumn { get; }
        private IGeographySqlType? ToGeography { get; }

        public STDistance(AColumn<IGeographyType> fromColumn, AColumn<IGeographyType> toColumn) : base(name: "STDistance") {
            FromColumn = fromColumn;
            ToColumn = toColumn;
        }
        public STDistance(AColumn<IGeographyType> fromColumn, IGeographySqlType? toGeography) : base(name: "STDistance") {
            FromColumn = fromColumn;
            ToGeography = toGeography;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            string sql;

            if(ToColumn is not null) {

                if(useAlias) {
                    sql = $"{FromColumn.Table.Alias}.{FromColumn.ColumnName}.STDistance({ToColumn.Table.Alias}.{ToColumn.ColumnName})";
                }
                else {
                    sql = $"{FromColumn.ColumnName}.STDistance({ToColumn.ColumnName})";
                }
            }
            else {

                string toPointSql = ToGeography!.GetSql(database, useAlias: useAlias, parameters);

                if(useAlias) {
                    sql = $"{FromColumn.Table.Alias}.{FromColumn.ColumnName}.STDistance({toPointSql})";
                }
                else {
                    sql = $"{FromColumn.ColumnName}.STDistance({toPointSql})";
                }
            }
            return sql;
        }
    }
}