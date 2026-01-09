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
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace QueryLite.Databases.Sqlite {

    public class SqliteDatabase : IDatabase {

        public string Name { get; }
        string IDatabase.Name => Name;
        public Func<string, string> SchemaMap { get; }
        private string ConnectionString { get; }

        IQueryGenerator IInternalConnection.QueryGenerator { get; } = new SqliteSelectQueryGenerator();
        IInsertQueryGenerator IInternalConnection.InsertGenerator { get; } = new SqliteInsertQueryGenerator();
        IUpdateQueryGenerator IInternalConnection.UpdateGenerator { get; } = new SqliteUpdateQueryGenerator();

        IDeleteQueryGenerator IInternalConnection.DeleteGenerator { get; } = new SqliteDeleteQueryGenerator();
        ITruncateQueryGenerator IInternalConnection.TruncateGenerator { get; } = new SqliteTruncateQueryGenerator();

        IPreparedParameterMapper IInternalConnection.ParameterMapper { get; } = new SqliteParameterMap();

        IPreparedQueryGenerator IInternalConnection.PreparedQueryGenerator { get; } = new SqlitePreparedSelectQueryGenerator();
        IPreparedUpdateQueryGenerator IInternalConnection.PreparedUpdateGenerator { get; } = new SqlitePreparedUpdateQueryGenerator();
        IPreparedInsertQueryGenerator IInternalConnection.PreparedInsertGenerator { get; } = new SqlitePreparedInsertQueryGenerator();
        IPreparedDeleteQueryGenerator IInternalConnection.PreparedDeleteQueryGenerator { get; } = new SqlitePreparedDeleteQueryGenerator();

        ILikeSqlConditionGenerator IInternalConnection.LikeSqlConditionGenerator { get; } = new SqliteLikeSqlConditionGenerator();

        public DatabaseType DatabaseType => DatabaseType.Sqlite;

        public EncloseWith EncloseWith => EncloseWith.SquareBracket;

        public UtfType DefaultUtfType { get; } = UtfType.UTF8;

        public SqliteDatabase(string name, string connectionString, Func<string, string>? schemaMap = null) {

            ArgumentException.ThrowIfNullOrEmpty(connectionString);

            Name = name ?? "";
            SchemaMap = schemaMap ?? ((schema) => schema);

            ConnectionString = connectionString;
        }

        public DbConnection GetNewConnection()  => new SqliteConnection(ConnectionString);

        public string ConvertToSql(object value) {
            return SqliteTypeMappings.ToSqlStringFunctions.ConvertToSql(value);
        }
        public IParametersBuilder CreateParameters(int initParams) {
            return new SqliteParameters(initParams);
        }

        public string? GetCSharpCodeSet(Type dotNetType) {
            return SqliteTypeMappings.ToSqlStringFunctions.GetCSharpCodeSet(dotNetType);
        }
    }
}