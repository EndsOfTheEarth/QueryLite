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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace QueryLite.Databases.SqlServer {

    public sealed class SqlServerParameters : IParameters {

        private List<Param> ParameterList { get; } = new List<Param>();

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
                new Param(
                    name: paramName,
                    type: type,
                    dbType: SqlServerSqlTypeMappings.GetDbType(type),
                    value: value
                )
            );
            count++;
        }
        public void SetParameters(IDatabase database, DbCommand command) {

            foreach(Param param in ParameterList) {

                SqlParameter parameter = ((SqlCommand)command).CreateParameter();

                parameter.ParameterName = param.Name;
                parameter.SqlDbType = param.SqlDbType;

                if(param.Value != null) {
                    parameter.Value = SqlServerSqlTypeMappings.ConvertToRawType(param.Value);
                }
                else {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }
        private sealed class Param {

            public string Name { get; set; }
            public Type Type { get; set; }
            public SqlDbType SqlDbType { get; set; }
            public object? Value { get; set; }

            public Param(string name, Type type, SqlDbType dbType, object? value) {
                Name = name;
                Type = type;
                SqlDbType = dbType;
                Value = value;
            }
        }
    }
}