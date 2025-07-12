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
using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace QueryLite.Databases.SqlServer {

    public class SqlServerDatabase : IDatabase {

        public string Name { get; }
        string IDatabase.Name => Name;
        public Func<string, string> SchemaMap { get; }
        private string ConnectionString { get; }

        IQueryGenerator IInternalConnection.QueryGenerator { get; } = new SqlServerSelectQueryGenerator();
        IInsertQueryGenerator IInternalConnection.InsertGenerator { get; } = new SqlServerInsertQueryGenerator();
        IUpdateQueryGenerator IInternalConnection.UpdateGenerator { get; } = new SqlServerUpdateQueryGenerator();
        IDeleteQueryGenerator IInternalConnection.DeleteGenerator { get; } = new SqlServerDeleteQueryGenerator();
        ITruncateQueryGenerator IInternalConnection.TruncateGenerator { get; } = new SqlServerTruncateQueryGenerator();
        IPreparedParameterMapper IInternalConnection.ParameterMapper { get; } = new SqlServerParameterMapper();

        IPreparedQueryGenerator IInternalConnection.PreparedQueryGenerator { get; } = new SqlServerPreparedSelectQueryGenerator();
        IPreparedInsertQueryGenerator IInternalConnection.PreparedInsertGenerator { get; } = new SqlServerPreparedInsertQueryGenerator();
        IPreparedUpdateQueryGenerator IInternalConnection.PreparedUpdateGenerator => new SqlServerPreparedUpdateQueryGenerator();
        IPreparedDeleteQueryGenerator IInternalConnection.PreparedDeleteQueryGenerator { get; } = new PreparedDeleteQueryGenerator();

        public DatabaseType DatabaseType => DatabaseType.SqlServer;

        public SqlServerDatabase(string name, string connectionString, Func<string, string>? schemaMap = null) {

            ArgumentException.ThrowIfNullOrEmpty(connectionString);

            Name = name ?? string.Empty;
            SchemaMap = schemaMap ?? ((schema) => schema);

            ConnectionString = connectionString;
        }

        public DbConnection GetNewConnection() => new SqlConnection(ConnectionString);

        DbConnection IDatabase.GetNewConnection() {
            return GetNewConnection();
        }

        public string ConvertToSql(object value) {
            return SqlServerSqlTypeMappings.ConvertToSql(value);
        }
        public IParametersBuilder CreateParameters(int initParams) {
            return new SqlServerParameters(initParams);
        }
        public string? GetCSharpCodeSet(Type dotNetType) {
            return SqlServerSqlTypeMappings.ToSqlStringFunctions.GetCSharpCodeSet(dotNetType);
        }
    }
}