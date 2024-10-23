/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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
namespace QueryLite.Databases.Functions {

    public interface IGeographySqlType {

        internal string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters);
    }

    public sealed class STGeomFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STGeomFromText(string kwText, int srid = 4326) : base(name: "STGeomFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STGeomFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STGeomFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STPointFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STPointFromText(string kwText, int srid = 4326) : base(name: "STPointFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STPointFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STPointFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STLineFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STLineFromText(string kwText, int srid = 4326) : base(name: "STLineFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STLineFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STLineFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STPolyFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STPolyFromText(string kwText, int srid = 4326) : base(name: "STPolyFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STPolyFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STPolyFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STMPointFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STMPointFromText(string kwText, int srid = 4326) : base(name: "STMPointFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STMPointFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STMPointFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STMLineFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STMLineFromText(string kwText, int srid = 4326) : base(name: "STMLineFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STMLineFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STMLineFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STMPolyFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STMPolyFromText(string kwText, int srid = 4326) : base(name: "STMPolyFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STMPolyFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STMPolyFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STGeomCollFromText : NullableFunction<IGeography>, IGeographySqlType {

        private string KwText { get; }
        public int SRID { get; }

        public STGeomCollFromText(string kwText, int srid = 4326) : base(name: "STGeomCollFromText") {
            KwText = kwText;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(string), KwText, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STGeomCollFromText({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STGeomCollFromText('{Helpers.EscapeForSql(KwText)}', {SRID})";
            }
        }
    }

    public sealed class STGeomCollFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STGeomCollFromWKB(string kwBinary, int srid = 4326) : base(name: "STGeomCollFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STGeomCollFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STGeomCollFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STGeomFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STGeomFromWKB(string kwBinary, int srid = 4326) : base(name: "STGeomFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STGeomFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STGeomFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STPointFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STPointFromWKB(string kwBinary, int srid = 4326) : base(name: "STPointFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STPointFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STPointFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STLineFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STLineFromWKB(string kwBinary, int srid = 4326) : base(name: "STLineFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STLineFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STLineFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STPolyFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STPolyFromWKB(string kwBinary, int srid = 4326) : base(name: "STPolyFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STPolyFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STPolyFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STMPointFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STMPointFromWKB(string kwBinary, int srid = 4326) : base(name: "STMPointFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STMPointFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STMPointFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STMLineFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STMLineFromWKB(string kwBinary, int srid = 4326) : base(name: "STMLineFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STMLineFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STMLineFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }

    public sealed class STMPolyFromWKB : NullableFunction<IGeography>, IGeographySqlType {

        private string KwBinary { get; }
        public int SRID { get; }

        public STMPolyFromWKB(string kwBinary, int srid = 4326) : base(name: "STMPolyFromWKB") {
            KwBinary = kwBinary;
            SRID = srid;
        }

        public override string GetSql(IDatabase database, bool useAlias, IParametersBuilder? parameters) {

            if(parameters != null) {
                parameters.AddParameter(database, typeof(byte[]), KwBinary, out string kwtParam);
                parameters.AddParameter(database, typeof(int), SRID, out string sridParam);
                return $"geography::STMPolyFromWKB({kwtParam},{sridParam})";
            }
            else {
                return $"geography::STMPolyFromWKB({Helpers.EscapeForSql(KwBinary)}, {SRID})";
            }
        }
    }
}