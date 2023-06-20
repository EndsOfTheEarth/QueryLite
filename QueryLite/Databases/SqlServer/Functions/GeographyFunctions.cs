namespace QueryLite.Databases.SqlServer.Functions {

    public sealed class GeographyPoint : Function<IGeographyType> {

        public double Latitude { get; }
        public double Longitude { get; }
        public int SRID { get; }

        public GeographyPoint(double latitude, double longitude, int srid = 4326) : base(name: "geometry::Point") {
            Latitude = latitude;
            Longitude = longitude;
            SRID = srid;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::Point({Latitude},{Longitude},{SRID})";
        }
    }

    public sealed class STDistance : NullableFunction<double> {

        private AColumn<IGeographyType> FromColumn { get; }
        private AColumn<IGeographyType>? ToColumn { get; }
        private GeographyPoint? ToPoint { get; }

        public STDistance(AColumn<IGeographyType> fromColumn, AColumn<IGeographyType> toColumn) : base(name: "STDistance") {
            FromColumn = fromColumn;
            ToColumn = toColumn;
        }
        public STDistance(AColumn<IGeographyType> fromColumn, GeographyPoint? toPoint) : base(name: "STDistance") {
            FromColumn = fromColumn;
            ToPoint = toPoint;
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

                string toPointSql = ToPoint!.GetSql(database, useAlias: useAlias, parameters);

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

    //.ShortestLineTo ( geography_other ) 

    public sealed class Longitude : NullableFunction<double> {

        public AColumn<IGeographyType> Column { get; }

        public Longitude(AColumn<IGeographyType> column) : base(name: "Longitude") {
            Column = column;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.Long" : $"{Column.ColumnName}.Long";
        }
    }
    
    public sealed class Latitude : NullableFunction<double> {

        public AColumn<IGeographyType> Column { get; }

        public Latitude(AColumn<IGeographyType> column) : base(name: "Latitude") {
            Column = column;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.Lat" : $"{Column.ColumnName}.Lat";
        }
    }
}