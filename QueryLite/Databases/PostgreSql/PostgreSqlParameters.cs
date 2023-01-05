using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace QueryLite.Databases.PostgreSql {

    public sealed class PostgreSqlParameters : IParameters {

        private List<PostgreSqlParam> ParameterList { get; } = new List<PostgreSqlParam>();

        private int count;

        public void Add(IDatabase database, Type type, object? value, out string paramName) {

            paramName = "@" + count.ToString();

            if(value != null) {

                if(value is IKeyValue keyValue) {
                    value = keyValue.GetValueAsObject();
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