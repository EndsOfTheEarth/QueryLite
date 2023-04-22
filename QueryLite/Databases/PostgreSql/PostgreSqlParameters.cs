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
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace QueryLite.Databases.PostgreSql {

    public sealed class PostgreSqlParameters : IParameters {

        private List<PostgreSqlParam> ParameterList { get; }

        public PostgreSqlParameters(int initParams) {
            ParameterList = new List<PostgreSqlParam>(initParams);
        }
        private int count;

        public void Add(IDatabase database, Type type, object? value, out string paramName) {

            paramName = "@" + count.ToString();

            if(value != null) {

                if(value is IKeyValue keyValue) {
                    value = keyValue.GetValueAsObject();
                }
                else if(value is Bit bitValue) {
                    value = bitValue.Value;
                }
            }

            ParameterList.Add(
                new PostgreSqlParam(
                    name: paramName,
                    type: type,
                    dbType: PostgreSqlTypeMappings.GetNpgsqlDbType(type),
                    value: value
                )
            );
            count++;
        }
        public void SetParameters(IDatabase database, DbCommand command) {

            foreach(PostgreSqlParam param in ParameterList) {

                NpgsqlParameter parameter = ((NpgsqlCommand)command).CreateParameter();

                parameter.ParameterName = param.Name;
                parameter.NpgsqlDbType = param.DbType;

                if(param.Value != null) {
                    parameter.Value = PostgreSqlTypeMappings.ConvertToRawType(param.Value);
                }
                else {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }
        private sealed class PostgreSqlParam {

            public string Name { get; set; }
            public Type Type { get; set; }
            public NpgsqlDbType DbType { get; set; }
            public object? Value { get; set; }

            public PostgreSqlParam(string name, Type type, NpgsqlDbType dbType, object? value) {
                Name = name;
                Type = type;
                DbType = dbType;
                Value = value;
            }
        }
    }    
}