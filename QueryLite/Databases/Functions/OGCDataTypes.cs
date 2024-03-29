﻿/*
 * MIT License
 *
 * Copyright (c) 2023 EndsOfTheEarth
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **/
using QueryLite.Utility;

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
    public sealed class GeographyPoint : Function<IGeography>, IGeographySqlType {

        public double Latitude { get; }
        public double Longitude { get; }
        public int SRID { get; }

        public GeographyPoint(double latitude, double longitude, int srid = 4326) : base(name: "geography::Point") {
            Latitude = latitude;
            Longitude = longitude;
            SRID = srid;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {

                parameters.AddParameter(database, typeof(double), Latitude, out string latitudeParam);
                parameters.AddParameter(database, typeof(double), Longitude, out string longitudeParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);

                return $"geography::Point({latitudeParam},{longitudeParam},{sridParam})";
            }
            else {
                return $"geography::Point({Latitude},{Longitude},{SRID})";
            }
        }
    }

    public sealed class STArea : NullableFunction<double> {

        private AColumn<IGeography>? Column { get; }
        private IGeographySqlType? OCGType { get; }

        public STArea(AColumn<IGeography> column) : base(name: "STArea") {
            Column = column;
        }
        public STArea(IGeographySqlType? oCGType) : base(name: "STArea") {
            OCGType = oCGType;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(Column is not null) {
                return useAlias ? $"{Column.Table.Alias}.{SqlHelper.EncloseColumnName(Column)}.STArea()" : $"{SqlHelper.EncloseColumnName(Column)}.STArea()";
            }
            else {
                return $"{OCGType!.GetSql(database, useAlias: useAlias, parameters)}.STArea()";
            }
        }
    }

    public sealed class STEquals : NullableFunction<int> {

        private AColumn<IGeography> ColumnA { get; }
        private AColumn<IGeography>? ColumnB { get; }
        private IGeographySqlType? ToGeography { get; }

        public STEquals(AColumn<IGeography> columnA, AColumn<IGeography> columnB) : base(name: "STEquals") {
            ColumnA = columnA;
            ColumnB = columnB;
        }
        public STEquals(AColumn<IGeography> columnA, IGeographySqlType? geographyB) : base(name: "STEquals") {
            ColumnA = columnA;
            ToGeography = geographyB;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            string sql;

            if(ColumnB is not null) {

                if(useAlias) {
                    sql = $"{ColumnA.Table.Alias}.{SqlHelper.EncloseColumnName(ColumnA)}.STEquals({ColumnB.Table.Alias}.{SqlHelper.EncloseColumnName(ColumnB)})";
                }
                else {
                    sql = $"{SqlHelper.EncloseColumnName(ColumnA)}.STEquals({SqlHelper.EncloseColumnName(ColumnB)})";
                }
            }
            else {

                string toPointSql = ToGeography!.GetSql(database, useAlias: useAlias, parameters);

                if(useAlias) {
                    sql = $"{ColumnA.Table.Alias}.{SqlHelper.EncloseColumnName(ColumnA)}.STEquals({toPointSql})";
                }
                else {
                    sql = $"{SqlHelper.EncloseColumnName(ColumnA)}.STEquals({toPointSql})";
                }
            }
            return sql;
        }
    }

    public sealed class STAsBinary : NullableFunction<byte[]> {

        private AColumn<IGeography>? Column { get; }
        private IGeographySqlType? OCGType { get; }

        public STAsBinary(AColumn<IGeography> column) : base(name: "STAsBinary") {
            Column = column;
        }
        public STAsBinary(IGeographySqlType? oCGType) : base(name: "STAsBinary") {
            OCGType = oCGType;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(Column is not null) {
                return useAlias ? $"{Column.Table.Alias}.{SqlHelper.EncloseColumnName(Column)}.STAsBinary()" : $"{SqlHelper.EncloseColumnName(Column)}.STAsBinary()";
            }
            else {
                return $"{OCGType!.GetSql(database, useAlias: useAlias, parameters)}.STAsBinary()";
            }
        }
    }

    public sealed class STAsText : NullableFunction<string> {

        private AColumn<IGeography>? Column { get; }
        private IGeographySqlType? OCGType { get; }

        public STAsText(AColumn<IGeography> column) : base(name: "STAsText") {
            Column = column;
        }
        public STAsText(IGeographySqlType? oCGType) : base(name: "STAsText") {
            OCGType = oCGType;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(Column is not null) {
                return useAlias ? $"{Column.Table.Alias}.{SqlHelper.EncloseColumnName(Column)}.STAsText()" : $"{SqlHelper.EncloseColumnName(Column)}.STAsText()";
            }
            else {
                return $"{OCGType!.GetSql(database, useAlias: useAlias, parameters)}.STAsText()";
            }
        }
    }

    public sealed class STContains : NullableFunction<Bit> {

        private AColumn<IGeography> ColumnA { get; }
        private AColumn<IGeography>? ColumnB { get; }
        private IGeographySqlType? ToGeography { get; }

        public STContains(AColumn<IGeography> columnA, AColumn<IGeography> columnB) : base(name: "STContains") {
            ColumnA = columnA;
            ColumnB = columnB;
        }
        public STContains(AColumn<IGeography> columnA, IGeographySqlType? geographyB) : base(name: "STContains") {
            ColumnA = columnA;
            ToGeography = geographyB;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            string sql;

            if(ColumnB is not null) {

                if(useAlias) {
                    sql = $"{ColumnA.Table.Alias}.{SqlHelper.EncloseColumnName(ColumnA)}.STContains({ColumnB.Table.Alias}.{SqlHelper.EncloseColumnName(ColumnB)})";
                }
                else {
                    sql = $"{SqlHelper.EncloseColumnName(ColumnA)}.STContains({SqlHelper.EncloseColumnName(ColumnB)})";
                }
            }
            else {

                string toPointSql = ToGeography!.GetSql(database, useAlias: useAlias, parameters);

                if(useAlias) {
                    sql = $"{ColumnA.Table.Alias}.{SqlHelper.EncloseColumnName(ColumnA)}.STContains({toPointSql})";
                }
                else {
                    sql = $"{SqlHelper.EncloseColumnName(ColumnA)}.STContains({toPointSql})";
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

        private AColumn<IGeography> FromColumn { get; }
        private AColumn<IGeography>? ToColumn { get; }
        private IGeographySqlType? ToGeography { get; }

        public STDistance(AColumn<IGeography> fromColumn, AColumn<IGeography> toColumn) : base(name: "STDistance") {
            FromColumn = fromColumn;
            ToColumn = toColumn;
        }
        public STDistance(AColumn<IGeography> fromColumn, IGeographySqlType? toGeography) : base(name: "STDistance") {
            FromColumn = fromColumn;
            ToGeography = toGeography;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            string sql;

            if(ToColumn is not null) {

                if(useAlias) {
                    sql = $"{FromColumn.Table.Alias}.{SqlHelper.EncloseColumnName(FromColumn)}.STDistance({ToColumn.Table.Alias}.{SqlHelper.EncloseColumnName(ToColumn)})";
                }
                else {
                    sql = $"{SqlHelper.EncloseColumnName(FromColumn)}.STDistance({SqlHelper.EncloseColumnName(ToColumn)})";
                }
            }
            else {

                string toPointSql = ToGeography!.GetSql(database, useAlias: useAlias, parameters);

                if(useAlias) {
                    sql = $"{FromColumn.Table.Alias}.{SqlHelper.EncloseColumnName(FromColumn)}.STDistance({toPointSql})";
                }
                else {
                    sql = $"{SqlHelper.EncloseColumnName(FromColumn)}.STDistance({toPointSql})";
                }
            }
            return sql;
        }
    }
}