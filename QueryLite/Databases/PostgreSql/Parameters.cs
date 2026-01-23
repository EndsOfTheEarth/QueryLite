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
using System.Data.Common;

namespace QueryLite.Databases.PostgreSql {

    public sealed class PostgreSqlParameters : IParametersBuilder {

        public IList<DbParameter> ParameterList_ { get; }

        public PostgreSqlParameters(int initParams) {
            ParameterList_ = new List<DbParameter>(initParams);
        }

        public string GetNextParameterName() {

            if(ParamNameCache.ParamNames.Length < ParameterList_.Count) {
                return ParamNameCache.ParamNames[ParameterList_.Count];
            }
            return $"@{ParameterList_.Count}";
        }

        public void AddParameter(IDatabase database, Type type, object? value, out string paramName) {

            paramName = GetNextParameterName();

            if(value != null) {
                CreateParameterDelegate createParameter = database.ParameterMapper.GetCreateParameterDelegate(type);
                ParameterList_.Add(createParameter(name: paramName, value));
            }
            else {
                ParameterList_.Add(
                    new NpgsqlParameter(parameterName: paramName, value: DBNull.Value) {
                        NpgsqlDbType = PostgreSqlTypeMappings.TypeMapper.GetDbType(type)
                    }
                );
            }
        }
    }
}