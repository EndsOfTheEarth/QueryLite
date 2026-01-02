/*
 * MIT License
 *
 * Copyright (c) 2025 EndsOfTheEarth
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
using Microsoft.Data.SqlClient;
using QueryLite.Utility;
using System.Data;

namespace QueryLite.Databases.SqlServer {

    /// <summary>
    /// Creates Sql Server parameters for the supported csharp types.
    /// </summary>
    public sealed class SqlServerParameterMap : AParameterMap<SqlParameter, SqlDbType>, IPreparedParameterMapper {

        public SqlServerParameterMap() : base(new SqlServerTypeMap()) { }

        protected override SqlParameter CreateParameter(string name, Bit? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(Bit))) {
                Value = (value != null ? value.Value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, bool? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(bool))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, byte[]? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(byte[]))) {
                Value = (value != null ? value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, DateOnly? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(DateOnly))) {
                Value = (value != null ? value.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, DateTime? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(DateTime))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, DateTimeOffset? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(DateTimeOffset))) {
                Value = (value != null ? value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, decimal? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(decimal))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, double? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(double))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, float? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(float))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, Guid? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(Guid))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, int? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(int))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, long? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(long))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, short? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(short))) {
                Value = (value != null ? value.Value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, string? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(string))) {
                Value = (value != null ? value : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, TimeOnly? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(TimeOnly))) {
                Value = (value != null ? value.Value.ToTimeSpan() : DBNull.Value)
            };
        }

        protected override SqlParameter CreateParameter(string name, byte? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(byte))) {
                Value = value != null ? value.Value : DBNull.Value
            };
        }

        protected override SqlParameter CreateParameter(string name, sbyte? value) {

            return new SqlParameter(parameterName: name, dbType: TypeMap.GetDbType(typeof(sbyte))) {
                Value = value != null ? value.Value : DBNull.Value
            };
        }

        protected override SqlParameter CreateParameter(string name, IValue<Guid>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<string>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<short>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<int>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<long>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<bool>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<Bit>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<decimal>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<float>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<double>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<byte[]>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<DateTime>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<DateTimeOffset>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<DateOnly>? value) {
            return CreateParameter(name, value?.Value);
        }

        protected override SqlParameter CreateParameter(string name, IValue<TimeOnly>? value) {
            return CreateParameter(name, value?.Value);
        }
    }
}