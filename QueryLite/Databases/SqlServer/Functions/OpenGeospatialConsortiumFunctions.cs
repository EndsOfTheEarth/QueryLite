namespace QueryLite.Databases.SqlServer.Functions {

    /// <summary>
    /// Geography function for defining a Point
    /// e.g. geography::Point(Latitude},Longitude,SRID)
    /// </summary>
    public sealed class GeographyPoint : Function<IGeographyType> {

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

    /// <summary>
    /// Geography function for loading a line object from a line string text value
    /// e.g. geography::STLineFromText('LINESTRING', SRID)
    /// </summary>
    public sealed class STLineFromText : NullableFunction<double> {

        private string LineString { get; }
        public int SRID { get; }

        public STLineFromText(string lineString, int srid = 4326) : base(name: "STLineFromText") {
            LineString = lineString;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::STLineFromText('{LineString}', {SRID})";
        }
    }


    /// <summary>
    /// Geography function for measuring the total surface area of a geography
    /// e.g. select columnA.STArea() from table
    /// </summary>
    public sealed class STArea : NullableFunction<double> {

        private AColumn<IGeographyType> Column { get; }

        public STArea(AColumn<IGeographyType> column) : base(name: "STArea") {
            Column = column;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.STArea()" : $"{Column.ColumnName}.STArea()";
        }
    }



    /// <summary>
    /// Geography function for measuring the shortest distance between two geographies
    /// e.g. select columnA.STDistance(columnB) from table
    /// </summary>
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
}