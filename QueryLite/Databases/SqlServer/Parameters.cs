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
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace QueryLite.Databases.SqlServer {

    public sealed class SqlServerParameters : IParametersBuilder {

        public IList<DbParameter> ParameterList { get; }

        public SqlServerParameters(int initParams) {
            ParameterList = new List<DbParameter>(initParams);
        }

        public void AddParameter(IDatabase database, Type type, object? value, out string paramName) {

            if(ParamNameCache.ParamNames.Length < ParameterList.Count) {
                paramName = ParamNameCache.ParamNames[ParameterList.Count];
            }
            else {
                paramName = $"@{ParameterList.Count}";
            }

            if(value != null) {

                if(value is IKeyValue keyValue) {
                    value = keyValue.GetValueAsObject();
                }
                else if(value is Bit bitValue) {
                    value = bitValue.Value;
                }
                else if(value is IValue<Guid> guidValue) {
                    value = guidValue.Value;
                }
                else if(value is IValue<short> shortValue) {
                    value = shortValue.Value;
                }
                else if(value is IValue<int> intValue) {
                    value = intValue.Value;
                }
                else if(value is IValue<long> longValue) {
                    value = longValue.Value;
                }
                else if(value is IValue<string> stringValue) {
                    value = stringValue.Value;
                }
                else if(value is IValue<bool> boolValue) {
                    value = boolValue.Value;
                }
                else if(value is IValue<decimal> decimalValue) {
                    value = decimalValue.Value;
                }
                else if(value is IValue<DateTime> dateTimeValue) {
                    value = dateTimeValue.Value;
                }
                else if(value is IValue<DateTimeOffset> dateTimeOffsetValue) {
                    value = dateTimeOffsetValue.Value;
                }
                else if(value is IValue<DateOnly> dateOnlyValue) {
                    value = dateOnlyValue.Value;
                }
                else if(value is IValue<TimeOnly> timeOnlyValue) {
                    value = timeOnlyValue.Value;
                }
                else if(value is IValue<float> floatValue) {
                    value = floatValue.Value;
                }
                else if(value is IValue<double> doubleValue) {
                    value = doubleValue.Value;
                }
                else if(value is IValue<Bit> bitIValue) {
                    value = bitIValue.Value;
                }
            }
            else {
                value = DBNull.Value;
            }
            ParameterList.Add(
                new SqlParameter(parameterName: paramName, value: SqlServerSqlTypeMappings.ConvertToRawType(value)) {
                    SqlDbType = SqlServerSqlTypeMappings.GetDbType(type)
                }
            );
        }
    }
}