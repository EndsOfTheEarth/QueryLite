using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

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

                DbParameter parameter = command.CreateParameter();

                parameter.ParameterName = param.Name;
                parameter.DbType = param.DbType;

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
            public DbType DbType { get; set; }
            public object? Value { get; set; }

            public Param(string name, Type type, DbType dbType, object? value) {
                Name = name;
                Type = type;
                DbType = dbType;
                Value = value;
            }
        }
    }
}