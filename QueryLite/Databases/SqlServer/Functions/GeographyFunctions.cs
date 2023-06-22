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
namespace QueryLite.Databases.SqlServer.Functions {

    //.ShortestLineTo ( geography_other ) 

    public sealed class Longitude : NullableFunction<double> {

        public AColumn<IGeography> Column { get; }

        public Longitude(AColumn<IGeography> column) : base(name: "Longitude") {
            Column = column;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.Long" : $"{Column.ColumnName}.Long";
        }
    }

    public sealed class Latitude : NullableFunction<double> {

        public AColumn<IGeography> Column { get; }

        public Latitude(AColumn<IGeography> column) : base(name: "Latitude") {
            Column = column;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return useAlias ? $"{Column.Table.Alias}.{Column.ColumnName}.Lat" : $"{Column.ColumnName}.Lat";
        }
    }

    public sealed class Geography_Parse : NullableFunction<IGeography> {

        public string KwText { get; }

        public Geography_Parse(string kwtText) : base(name: "geography::Parse") {
            KwText = kwtText;
        }
        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {
            return $"geography::Parse('{Helpers.EscapeForSql(KwText)}')";
        }
    }
}