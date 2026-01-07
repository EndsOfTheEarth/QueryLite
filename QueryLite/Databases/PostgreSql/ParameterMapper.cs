/*
 * MIT License
 *
 * Copyright (c) 2026 EndsOfTheEarth
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
using Npgsql;
using NpgsqlTypes;
using QueryLite.Databases.PostgreSql;
using QueryLite.Utility;

namespace QueryLite.Databases.SqlServer {

    /// <summary>
    /// Creates PostgreSql parameters for the supported csharp types.
    /// </summary>
    public sealed class PostgreSqlParameterMap : AParameterMap<NpgsqlParameter, NpgsqlDbType>, IPreparedParameterMapper {

        public PostgreSqlParameterMap() : base(new PostgreSqlTypeMap()) { }

        protected override NpgsqlParameter CreateParameter(string name, Bit? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(Bit))) {
                Value = (value != null ? value.Value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, bool? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(bool))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, byte[]? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(byte[]))) {
                Value = (value != null ? value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, DateOnly? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(DateOnly))) {
                Value = (value != null ? value.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, DateTime? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(DateTime))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, DateTimeOffset? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(DateTimeOffset))) {
                Value = (value != null ? new DateTimeOffset(value.Value.UtcDateTime) : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, decimal? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(decimal))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, double? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(double))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, float? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(float))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, Guid? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(Guid))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, int? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(int))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, long? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(long))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, short? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(short))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, string? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(string))) {
                Value = (value != null ? value : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, TimeOnly? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(TimeOnly))) {
                Value = (value != null ? value.Value.ToTimeSpan() : DBNull.Value)
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, byte? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(byte))) {
                Value = value != null ? value.Value : DBNull.Value
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, sbyte? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(sbyte))) {
                Value = value != null ? value.Value : DBNull.Value
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, Json? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(Json))) {
                Value = value != null ? value.Value.Value : DBNull.Value
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, Jsonb? value) {

            return new NpgsqlParameter(parameterName: name, parameterType: TypeMap.GetDbType(typeof(Jsonb))) {
                Value = value != null ? value.Value.Value : DBNull.Value
            };
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<Guid>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<string>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<short>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<int>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<long>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<bool>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<Bit>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<decimal>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<float>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<double>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<byte[]>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<DateTime>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<DateTimeOffset>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<DateOnly>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<TimeOnly>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<Json>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override NpgsqlParameter CreateParameter(string name, IValue<Jsonb>? value) {
            return CreateParameter(name, value?.Value);
        }
    }
}