/*
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
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryLite.Databases.PostgreSql.Collectors {

    internal sealed class PostgreSqlPreparedSetValuesCollector<PARAMETERS> : IPreparedSetValuesCollector<PARAMETERS> {

        public PreparedParameterList<PARAMETERS> Parameters { get; } = new PreparedParameterList<PARAMETERS>();

        private StringBuilder _sql;
        private StringBuilder? _paramSql;

        private IDatabase _database;
        private CollectorMode _collectorMode;

        private int _counter = 0;

        public PostgreSqlPreparedSetValuesCollector(StringBuilder sql, StringBuilder? paramSql, IDatabase database, CollectorMode mode) {
            _sql = sql;
            _paramSql = paramSql;
            _database = database;
            _collectorMode = mode;
        }

        private IPreparedSetValuesCollector<PARAMETERS> AddParameter<TYPE>(IColumn column, Func<PARAMETERS, TYPE> func, CreateParameterDelegate setParameterFunc) {

            string paramName = Parameters.GetNextParameterName();

            Parameters.Add(new PreparedParameter<PARAMETERS, TYPE>(name: paramName, func, setParameterFunc));

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    _paramSql!.Append(',');
                }

                SqlHelper.AppendEncloseColumnName(_sql, column);

                _paramSql!.Append(paramName);

            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                //SqlHelper.AppendEncloseAlias(_sql, column.Table.Alias);
                //_sql.Append('.');
                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=').Append(paramName);

            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            _counter++;
            return this;
        }

        private IPreparedSetValuesCollector<PARAMETERS> AddFunction(IColumn column, IFunction function) {

            if(_collectorMode == CollectorMode.Insert) {

                if(_counter > 0) {
                    _sql.Append(',');
                    _paramSql!.Append(',');
                }

                SqlHelper.AppendEncloseColumnName(_sql, column);

                _paramSql!.Append(function.GetSql(_database, useAlias: false, parameters: null));

            }
            else if(_collectorMode == CollectorMode.Update) {

                if(_counter > 0) {
                    _sql.Append(',');
                }

                //SqlHelper.AppendEncloseAlias(_sql, column.Table.Alias);
                //_sql.Append('.');
                SqlHelper.AppendEncloseColumnName(_sql, column);
                _sql.Append('=');
                _sql.Append(function.GetSql(_database, useAlias: false, parameters: null));
            }
            else {
                throw new InvalidOperationException($"Unknown {nameof(_collectorMode)}. Value = '{_collectorMode}'");
            }
            return this;
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<string> column, Func<PARAMETERS, string> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(string)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<string> column, Func<PARAMETERS, string?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(string)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<Guid> column, Func<PARAMETERS, Guid> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(Guid)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<Guid> column, Func<PARAMETERS, Guid?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(Guid?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<bool> column, Func<PARAMETERS, bool> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(bool)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<bool> column, Func<PARAMETERS, bool?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(bool?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<Bit> column, Func<PARAMETERS, Bit> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(Bit)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<Bit> column, Func<PARAMETERS, Bit?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(Bit?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<decimal> column, Func<PARAMETERS, decimal> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(decimal)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<decimal> column, Func<PARAMETERS, decimal?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(decimal?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<short> column, Func<PARAMETERS, short> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(short)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<short> column, Func<PARAMETERS, short?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(short?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<int> column, Func<PARAMETERS, int> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(int)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<int> column, Func<PARAMETERS, int?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(int?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<long> column, Func<PARAMETERS, long> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(long)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<long> column, Func<PARAMETERS, long?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(long?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<float> column, Func<PARAMETERS, float> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(float)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<float> column, Func<PARAMETERS, float?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(float?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<double> column, Func<PARAMETERS, double> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(double)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<double> column, Func<PARAMETERS, double?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(double?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<TimeOnly> column, Func<PARAMETERS, TimeOnly> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(TimeOnly)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<TimeOnly> column, Func<PARAMETERS, TimeOnly?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(TimeOnly?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateTime> column, Func<PARAMETERS, DateTime> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(DateTime)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateTime> column, Func<PARAMETERS, DateTime?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(DateTime?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateOnly> column, Func<PARAMETERS, DateOnly> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(DateOnly)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateOnly> column, Func<PARAMETERS, DateOnly?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(DateOnly?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<DateTimeOffset> column, Func<PARAMETERS, DateTimeOffset> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(DateTimeOffset)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<DateTimeOffset> column, Func<PARAMETERS, DateTimeOffset?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(DateTimeOffset?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<byte> column, Func<PARAMETERS, byte> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(byte)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<byte> column, Func<PARAMETERS, byte?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(byte?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(Column<byte[]> column, Func<PARAMETERS, byte[]> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(byte[])));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(NullableColumn<byte[]> column, Func<PARAMETERS, byte[]?> value) {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(byte[])));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<ENUM>(Column<ENUM> column, Func<PARAMETERS, ENUM> value) where ENUM : notnull, Enum {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(ENUM)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<ENUM>(NullableColumn<ENUM> column, Func<PARAMETERS, ENUM?> value) where ENUM : notnull, Enum {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(ENUM?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<StringKey<TYPE>> column, Func<PARAMETERS, StringKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(StringKey<TYPE>)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<StringKey<TYPE>> column, Func<PARAMETERS, StringKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(StringKey<TYPE>?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<GuidKey<TYPE>> column, Func<PARAMETERS, GuidKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(GuidKey<TYPE>)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<GuidKey<TYPE>> column, Func<PARAMETERS, GuidKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(GuidKey<TYPE>?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<ShortKey<TYPE>> column, Func<PARAMETERS, ShortKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(ShortKey<TYPE>)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<ShortKey<TYPE>> column, Func<PARAMETERS, ShortKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(ShortKey<TYPE>?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<IntKey<TYPE>> column, Func<PARAMETERS, IntKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(IntKey<TYPE>)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<IntKey<TYPE>> column, Func<PARAMETERS, IntKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(IntKey<TYPE>?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<LongKey<TYPE>> column, Func<PARAMETERS, LongKey<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(LongKey<TYPE>)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<LongKey<TYPE>> column, Func<PARAMETERS, LongKey<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(LongKey<TYPE>?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(Column<BoolValue<TYPE>> column, Func<PARAMETERS, BoolValue<TYPE>> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(BoolValue<TYPE>)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set<TYPE>(NullableColumn<BoolValue<TYPE>> column, Func<PARAMETERS, BoolValue<TYPE>?> value) where TYPE : notnull {
            return AddParameter(column, value, _database.ParameterMapper.GetCreateParameterDelegate(typeof(BoolValue<TYPE>?)));
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<string> column, AFunction<string> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<Guid> column, AFunction<Guid> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<bool> column, AFunction<bool> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<Bit> column, AFunction<Bit> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<decimal> column, AFunction<decimal> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<short> column, AFunction<short> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<int> column, AFunction<int> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<long> column, AFunction<long> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<float> column, AFunction<float> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<double> column, AFunction<double> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<TimeOnly> column, AFunction<TimeOnly> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateTime> column, AFunction<DateTime> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateOnly> column, AFunction<DateOnly> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<DateTimeOffset> column, AFunction<DateTimeOffset> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<byte> column, AFunction<byte> value) {
            return AddFunction(column, value);
        }

        public IPreparedSetValuesCollector<PARAMETERS> Set(AColumn<byte[]> column, AFunction<byte[]> value) {
            return AddFunction(column, value);
        }
    }
}