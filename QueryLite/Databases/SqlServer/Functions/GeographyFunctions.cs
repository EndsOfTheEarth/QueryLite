namespace QueryLite.Databases.SqlServer.Functions {

    

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