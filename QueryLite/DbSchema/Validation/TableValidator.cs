/*
 * MIT License
 *
 * Copyright (c) 2024 EndsOfTheEarth
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
using QueryLite.DbSchema;
using QueryLite.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace QueryLite {

    public sealed class ValidationItem {

        public Type? TableType { get; }

        public ITable? Table { get; private set; }

        public void SetTable(ITable table) {
            Table = table;
            Schema = table.SchemaName;
            TableName = table.TableName;
        }
        public string Schema { get; private set; } = string.Empty;
        public string TableName { get; private set; } = string.Empty;

        public bool HasErrors => ValidationMessages.Count > 0;
        public List<string> ValidationMessages { get; } = new List<string>();

        public void Add(string message) {
            ValidationMessages.Add(message);
        }

        public ValidationItem(Type? tableType, string schema = "", string tableName = "") {
            TableType = tableType;
            Schema = schema;
            TableName = tableName;
        }
        public override string? ToString() {
            return TableType?.FullName ?? base.ToString();
        }
    }

    public class SchemaValidationSettings {

        public required bool ValidatePrimaryKeys { get; init; }
        public required bool ValidateUniqueConstraints { get; init; }
        public required bool ValidateForeignKeys { get; init; }
        public required bool ValidateMissingCodeTables { get; init; }
    }

    public class ValidationResult {

        public List<ValidationItem> Items { get; } = new List<ValidationItem>();

        public DatabaseSchema Schema { get; set; }

        public ValidationResult(DatabaseSchema schema) {
            Schema = schema;
        }
    }
    public static class SchemaValidator {

        public static ValidationResult ValidateTablesInCurrentDomain(IDatabase database, SchemaValidationSettings validationSettings) {

            using DbConnection dbConnection = database.GetNewConnection();

            DatabaseSchema dbSchema = LoadSchema(database);

            dbConnection.Close();

            ValidationResult result = new ValidationResult(dbSchema);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> types = new List<Type>();

            foreach(Assembly assembly in assemblies) {

                try {

                    Type[] assemblyTypes = assembly.GetTypes();

                    foreach(Type p in assemblyTypes) {

                        if(typeof(ITable).IsAssignableFrom(p) && p != typeof(ITable) && p != typeof(ATable)) {
                            types.Add(p);
                        }
                    }
                }
                catch(ReflectionTypeLoadException ex) {
                    //Ignore as some types cannot be loaded
                    ValidationItem tableValidation = new ValidationItem(tableType: null);
                    tableValidation.Add($"Warning: Unable to load the a type from the assembly '{assembly.FullName}'.{Environment.NewLine}{ex}");
                    result.Items.Add(tableValidation);
                }
            }

            List<ITable> codeTables = new List<ITable>();

            foreach(Type type in types) {

                result.Items.Add(ValidateFromType(type, database, dbSchema, validationSettings, out ITable? table));

                if(table != null) {
                    codeTables.Add(table);
                }
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, database, codeTables, result);
            }
            return result;
        }

        public static ValidationResult ValidateTablesInAssemblies(Assembly[] assemblies, IDatabase database, SchemaValidationSettings validationSettings) {

            using DbConnection dbConnection = database.GetNewConnection();

            DatabaseSchema dbSchema = LoadSchema(database);

            dbConnection.Close();

            List<Type> types = new List<Type>();

            foreach(Assembly assembly in assemblies) {

                try {

                    Type[] assemblyTypes = assembly.GetTypes();

                    foreach(Type p in assemblyTypes) {

                        if(typeof(ITable).IsAssignableFrom(p) && p != typeof(ITable) && p != typeof(ATable)) {
                            types.Add(p);
                        }
                    }
                }
                catch(ReflectionTypeLoadException ex) {
                    //Ignore as some types cannot be loaded
                    ex = true ? ex : ex;
                }
            }

            ValidationResult result = new ValidationResult(dbSchema);

            List<ITable> codeTables = new List<ITable>();

            foreach(Type type in types) {

                result.Items.Add(ValidateFromType(type, database, dbSchema, validationSettings, out ITable? table));

                if(table != null) {
                    codeTables.Add(table);
                }
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, database, codeTables, result);
            }
            return result;
        }

        public static ValidationResult ValidateTablesInAssembly(IDatabase database, Assembly assembly, SchemaValidationSettings validationSettings) {

            using DbConnection dbConnection = database.GetNewConnection();

            DatabaseSchema dbSchema = LoadSchema(database);

            dbConnection.Close();

            List<Type> types = assembly.GetTypes()
                .Select(type => type)
                .Where(type => typeof(ITable).IsAssignableFrom(type) && type != typeof(ITable) && type != typeof(ATable)).ToList();

            ValidationResult result = new ValidationResult(dbSchema);

            List<ITable> codeTables = new List<ITable>();

            foreach(Type type in types) {

                result.Items.Add(ValidateFromType(type, database, dbSchema, validationSettings, out ITable? table));

                if(table != null) {
                    codeTables.Add(table);
                }
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, database, codeTables, result);
            }
            return result;
        }

        public static ValidationResult ValidateTables(IDatabase database, List<ITable> tables, SchemaValidationSettings validationSettings) {

            using DbConnection dbConnection = database.GetNewConnection();

            DatabaseSchema dbSchema = LoadSchema(database);

            dbConnection.Close();

            ValidationResult result = new ValidationResult(dbSchema);

            foreach(ITable table in tables) {
                result.Items.Add(ValidateFromType(table.GetType(), database, dbSchema, validationSettings, table: out _));
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, database, tables, result);
            }
            return result;
        }

        public static ValidationItem ValidateTable(IDatabase database, ITable table, SchemaValidationSettings validationSettings) {

            using DbConnection dbConnection = database.GetNewConnection();

            DatabaseSchema dbSchema = LoadSchema(database);

            dbConnection.Close();

            return ValidateFromType(table.GetType(), database, dbSchema, validationSettings, table: out _);
        }

        private static DatabaseSchema LoadSchema(IDatabase database) {

            List<DatabaseTable> dbTables;

            if(database.DatabaseType == DatabaseType.PostgreSql) {
                dbTables = PostgreSqlSchemaLoader.LoadTables(database);
            }
            else if(database.DatabaseType == DatabaseType.SqlServer) {
                dbTables = SqlServerSchemaLoader.LoadTables(database);
            }
            else {
                throw new Exception($"Unknown {nameof(DatabaseType)} == {database.DatabaseType}");
            }
            return new DatabaseSchema(dbTables);
        }

        private static ValidationItem ValidateFromType(Type type, IDatabase database, DatabaseSchema dbSchema, SchemaValidationSettings validationSettings, out ITable? table) {

            FieldInfo? instanceField1 = type.GetField("Instance1");

            if(instanceField1 == null) {
                instanceField1 = type.GetField("Instance");
            }

            PropertyInfo? instance1Property = type.GetProperty("Instance1");

            if(instance1Property == null) {
                type.GetProperty("Instance");
            }

            ValidationItem tableValidation = new ValidationItem(type);

            if(instanceField1 == null && instance1Property == null) {
                tableValidation.Add($"Table does not have a static field or property called 'Instance' or 'Instance1' which returns an instance of the code table");
            }

            table = null;

            if(instanceField1 != null) {
                table = (ITable?)instanceField1.GetValue(null);
            }
            else if(instance1Property != null) {
                table = (ITable?)instance1Property.GetValue(null);
            }

            if(table == null) {
                tableValidation.Add($"Unable to load an instance of class table in order to validate table");
            }
            else {
                
                tableValidation.SetTable(table);

                Validate(database, table, dbSchema, tableValidation, validationSettings);
            }
            return tableValidation;
        }

        private static List<DatabaseTable> FindMatchingSchemaTables(IDatabase database, ITable table, DatabaseSchema dbSchema) {

            List<DatabaseTable> foundTables = new List<DatabaseTable>();

            string mappedSchemaName = !string.IsNullOrEmpty(table.SchemaName) ? database.SchemaMap(table.SchemaName) : table.SchemaName;

            foreach(DatabaseTable schemaTable in dbSchema.Tables) {

                if(string.Compare(schemaTable.TableName.Value, table.TableName, ignoreCase: true) == 0) {

                    if(string.Compare(schemaTable.Schema.Value, mappedSchemaName, ignoreCase: true) == 0) {
                        foundTables.Add(schemaTable);
                    }
                }
            }
            return foundTables;
        }

        public static void Validate(IDatabase database, ITable table, DatabaseSchema dbSchema, ValidationItem tableValidation, SchemaValidationSettings validationSettings) {

            if(string.IsNullOrWhiteSpace(table.SchemaName)) {
                tableValidation.Add($"Table schema name cannot be null or empty in code");
            }

            List<DatabaseTable> dbTables = FindMatchingSchemaTables(database, table, dbSchema);

            if(dbTables == null || dbTables.Count == 0) {
                tableValidation.Add($"Code table definition exists but the database table does not. This can happen if the table does not exist in the database or the schema names do not match or the table names do not match.");
                return;
            }

            if(dbTables.Count > 1) {
                string mappedSchemaName = !string.IsNullOrEmpty(table.SchemaName) ? database.SchemaMap(table.SchemaName) : table.SchemaName;
                tableValidation.Add($"Unable to match table name to database table. {dbTables.Count} tables exist with the name '{mappedSchemaName}.{table.TableName}' in the database.");
                return;
            }

            DatabaseTable dbTable = dbTables[0];

            List<CodeColumnProperty> tableColumnProperties = LoadTableColumnsProperties(table, tableValidation);

            foreach(DatabaseColumn dbColumn in dbTable.Columns) {

                string columnDetail = $"Table Column: '{dbColumn.ColumnName.Value}'";

                CodeColumnProperty? codeColumnProperty = tableColumnProperties.Find(c => string.Compare(c.Column.ColumnName, dbColumn.ColumnName.Value, ignoreCase: true) == 0);

                if(codeColumnProperty == null) {
                    tableValidation.Add($"{columnDetail}: Column exists in database table but not in code table");
                }
                else {

                    IColumn codeColumn = codeColumnProperty.Column;

                    if(!codeColumn.Enclose && SqlKeyWordLookup.IsKeyWord(codeColumn.ColumnName)) {
                        tableValidation.Add($"{columnDetail}, column name '{codeColumn.ColumnName}' is an SQL keyword so the {nameof(codeColumn.Enclose)} property should be set to true on the column");
                    }

                    Type dbNetType = dbColumn.DataType.DotNetType;
                    Type codeAdoType = ConvertToAdoType(codeColumn.Type);    //This is for the case with types like IntKey<> where the ado type is int

                    if(!codeColumnProperty.SuppressColumnTypeValidation) {

                        if(codeColumn.Type.IsEnum) {

                            if(dbNetType != typeof(short) && dbNetType != typeof(int) && dbNetType != typeof(long)) {
                                throw new Exception("An enum type must be mapped as a short, integer or long. This might be a bug");
                            }
                            if(codeAdoType != typeof(int) && codeAdoType != typeof(short) && codeAdoType != typeof(byte) && codeAdoType != typeof(long)) {
                                tableValidation.Add($"{columnDetail}, column types are different ({codeAdoType.Name} != {dbNetType.Name}) between database and code column. Tip: The attribute [SuppressColumnTypeValidation] can be used on the table column property to suppress this error.");
                            }
                        }
                        else if(codeAdoType == typeof(bool) && dbNetType == typeof(short)) {
                            //For sql servers TINYINT data type we can map to bool or short
                        }
                        else if(codeAdoType != dbNetType) {
                            tableValidation.Add($"{columnDetail}, column types are different ({codeAdoType.Name} != {dbNetType.Name}) between database and code column. Tip: The attribute [SuppressColumnTypeValidation] can be used on the table column property to suppress this error.");
                        }
                    }

                    if(dbColumn.IsNullable && !codeColumn.IsNullable) {
                        tableValidation.Add($"{columnDetail}, database column is nullable but code column is non-nullable");
                    }
                    else if(!dbColumn.IsNullable && codeColumn.IsNullable) {
                        tableValidation.Add($"{columnDetail}, database column is non-nullable but code column is nullable");
                    }

                    if(dbColumn.IsAutoGenerated && !codeColumn.IsAutoGenerated) {
                        tableValidation.Add($"{columnDetail}, database column is auto generated but code column is not");
                    }
                    else if(!dbColumn.IsAutoGenerated && codeColumn.IsAutoGenerated) {
                        tableValidation.Add($"{columnDetail}, database column is not auto generated but code column is");
                    }

                    if(dbColumn.Length != null) {

                        if(codeColumn.Length == null) {
                            tableValidation.Add($"{columnDetail}, database column length is not null but code column length is null");
                        }
                        else if(dbColumn.Length.Value == -1) {  //-1 is unlimited length

                            if(codeColumn.Length != int.MaxValue) {
                                tableValidation.Add($"{columnDetail}, database column has an unlimited length but code column is not set to int.MaxValue. Database Length = {dbColumn.Length}, Code Column Length = {codeColumn.Length}");
                            }
                        }
                        else if(codeColumn.Length != null && dbColumn.Length.Value != codeColumn.Length.Value) {
                            tableValidation.Add($"{columnDetail}, database column length does not match code column length. Database Length = {dbColumn.Length}, Code Column Length = {codeColumn.Length}");
                        }
                    }
                    else {

                        if(codeColumn.Length != null && codeColumn.Length != int.MaxValue) {
                            tableValidation.Add($"{columnDetail}, database column length is null but code column length is not null");
                        }
                    }
                }
            }

            Dictionary<string, int> duplicateColumnNameLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach(CodeColumnProperty columnProperty in tableColumnProperties) {

                IColumn codeColumn = columnProperty.Column;

                string columnDetail = $"Column: '{codeColumn.ColumnName}'";

                DatabaseColumn? dbColumn = dbTable.Columns.Find(c => string.Compare(c.ColumnName.Value, codeColumn.ColumnName, ignoreCase: true) == 0);

                if(dbColumn == null) {
                    tableValidation.Add($"{columnDetail}, column exists in code but not in the database");
                }
                else {

                    if(!duplicateColumnNameLookup.ContainsKey(codeColumn.ColumnName)) {
                        duplicateColumnNameLookup.Add(codeColumn.ColumnName, 1);
                    }
                    else {
                        duplicateColumnNameLookup[codeColumn.ColumnName]++;
                    }
                }
            }

            foreach(KeyValuePair<string, int> duplicate in duplicateColumnNameLookup) {

                if(duplicate.Value > 1) {

                    string columnDetail = $"Column: '{duplicate.Key}'";

                    tableValidation.Add($"{columnDetail}, column name is defined {duplicate.Value} times in code table");
                }
            }

            Type tableType = table.GetType();

            if(tableType.BaseType == null || tableType.BaseType != typeof(ATable)) {
                tableValidation.Add($"code table base class must be of type {typeof(ATable)}");
            }

            PropertyInfo[] tableProperties = tableType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            List<PropertyInfo> columnProperties = new List<PropertyInfo>();

            foreach(PropertyInfo tableProperty in tableProperties) {

                if(tableProperty.Name == nameof(table.PrimaryKey) || tableProperty.Name == nameof(table.UniqueConstraints) || tableProperty.Name == nameof(table.ForeignKeys)) {
                    continue;
                }

                Type underlyingPropertyType = tableProperty.PropertyType.IsGenericType ? tableProperty.PropertyType.GetGenericTypeDefinition() : tableProperty.PropertyType;

                string tablePropertyDetail = $"Table Property '{tableProperty.Name}'";

                if(underlyingPropertyType != typeof(Column<>) && underlyingPropertyType != typeof(NullableColumn<>)) {
                    tableValidation.Add($"{tablePropertyDetail} (Type: '{underlyingPropertyType}') is not a column type. All table instance properties must inherit from either {typeof(Column<>).Name}<> or {typeof(NullableColumn<>).Name}<>");
                }
                else {

                    columnProperties.Add(tableProperty);

                    //TODO: Checking reference nullability is not straight forward at the moment so I have chosen to skip that for now

                    object? propertyValue = tableProperty.GetValue(table);

                    if(propertyValue == null) {
                        tableValidation.Add($"{tablePropertyDetail} (Type: '{underlyingPropertyType}') is not populated. Value is null");
                    }
                    else {

                        bool match = false;

                        foreach(CodeColumnProperty columnProperty in tableColumnProperties) {

                            if(columnProperty.Column == ((IColumn)propertyValue)) {
                                match = true;
                                break;
                            }
                        }
                        if(!match) {
                            tableValidation.Add($"{tablePropertyDetail}  (Type: '{underlyingPropertyType}') column has not been assigned to table");
                        }
                    }

                    MethodInfo? getMethod = tableProperty.GetGetMethod();

                    if(getMethod == null) {
                        tableValidation.Add($"{tablePropertyDetail}  (Type: '{underlyingPropertyType}') Get method does not exist");
                    }
                    else if(!getMethod.IsPublic) {
                        tableValidation.Add($"{tablePropertyDetail}  (Type: '{underlyingPropertyType}') Get method must be public");
                    }

                    MethodInfo? setMethod = tableProperty.GetSetMethod();

                    if(setMethod != null && !setMethod.IsPrivate) {
                        tableValidation.Add($"{tablePropertyDetail}  (Type: '{underlyingPropertyType}') Set method must either not exist or be private");
                    }
                }
            }
            if(validationSettings.ValidatePrimaryKeys) {
                ValidatePrimaryKeyForTable(table, dbTable, tableValidation);
            }
            if(validationSettings.ValidateUniqueConstraints) {
                ValidateUniqueConstraintsForTable(table, dbTable, tableValidation);
            }

            if(validationSettings.ValidateForeignKeys) {
                ValidateForeignKeys(table, dbTable, tableValidation);
            }
        }

        private static void ValidatePrimaryKeyForTable(ITable table, DatabaseTable dbTable, ValidationItem tableValidation) {

            DatabasePrimaryKey? dbPrimaryKey = dbTable.PrimaryKey;
            PrimaryKey? codePrimaryKey = table.PrimaryKey;

            if(dbPrimaryKey != null && codePrimaryKey == null) {
                tableValidation.Add($"Code table is missing primary key definition");
            }
            else if(dbPrimaryKey == null && codePrimaryKey != null) {
                tableValidation.Add($"Code table has a primary key definition but it does not exist in the database");
            }
            else if(dbPrimaryKey != null && codePrimaryKey != null) {

                if(string.Compare(dbPrimaryKey!.ConstraintName, codePrimaryKey!.ConstraintName, ignoreCase: true) != 0) {
                    tableValidation.Add($"Code and database primary key constraint names are different. '{dbPrimaryKey.ConstraintName}' != '{codePrimaryKey.ConstraintName}'");
                }

                if(dbPrimaryKey.ColumnNames.Count != codePrimaryKey.Columns.Length) {
                    tableValidation.Add($"Code and database primary key column counts do not match. {dbPrimaryKey.ColumnNames.Count} != {codePrimaryKey.Columns.Length}");
                }
                else {

                    for(int index = 0; index < dbPrimaryKey.ColumnNames.Count; index++) {

                        string dbColumn = dbPrimaryKey.ColumnNames[index];
                        string codeColumn = codePrimaryKey.Columns[index].ColumnName;

                        if(string.Compare(dbColumn, codeColumn, ignoreCase: true) != 0) {
                            tableValidation.Add($"Code and database primary key columns do not match. '{dbColumn}' != '{codeColumn}'");
                        }
                    }
                }
            }
        }

        private static void ValidateUniqueConstraintsForTable(ITable table, DatabaseTable dbTable, ValidationItem tableValidation) {

            if(table.UniqueConstraints.Length != dbTable.UniqueConstraints.Count) {
                tableValidation.Add($"The number of unique constraints between the code and database do not match. '{table.UniqueConstraints.Length}' != '{dbTable.UniqueConstraints.Count}'");
            }

            foreach(DatabaseUniqueConstraint dbUniqueConstraint in dbTable.UniqueConstraints) {

                UniqueConstraint? tableUniqueConstraint = null;

                foreach(UniqueConstraint tableUC in table.UniqueConstraints) {

                    if(string.Equals(dbUniqueConstraint.ConstraintName, tableUC.ConstraintName, StringComparison.OrdinalIgnoreCase)) {

                        if(tableUniqueConstraint != null) {
                            tableValidation.Add($"The unique constraint '{dbUniqueConstraint.ConstraintName}'is defined more than once in code");
                            break;
                        }
                        tableUniqueConstraint = tableUC;
                    }
                }

                if(tableUniqueConstraint == null) {
                    tableValidation.Add($"The unique constraint '{dbUniqueConstraint.ConstraintName}' is not defined in code");
                }
                else {
                    //Check columns exist

                    if(dbUniqueConstraint.ColumnNames.Count != tableUniqueConstraint.Columns.Length) {
                        tableValidation.Add($"Code and database unique constraint '{dbUniqueConstraint.ConstraintName}' column counts do not match. {dbUniqueConstraint.ColumnNames.Count} != {tableUniqueConstraint.Columns.Length}");
                    }
                    else {

                        for(int index = 0; index < dbUniqueConstraint.ColumnNames.Count; index++) {

                            string dbColumn = dbUniqueConstraint.ColumnNames[index].Value;
                            string codeColumn = tableUniqueConstraint.Columns[index].ColumnName;

                            if(!string.Equals(dbColumn, codeColumn, StringComparison.OrdinalIgnoreCase)) {
                                tableValidation.Add($"Code and database unique constraint columns do not match. '{dbColumn}' != '{codeColumn}'. Names must match and be in the same order as defined in the database");
                            }
                        }
                    }
                }
            }

            foreach(UniqueConstraint tableUniqueConstraint in table.UniqueConstraints) {

                bool found = false;

                foreach(DatabaseUniqueConstraint dbUniqueConstraint in dbTable.UniqueConstraints) {

                    if(string.Equals(dbUniqueConstraint.ConstraintName, tableUniqueConstraint.ConstraintName, StringComparison.OrdinalIgnoreCase)) {
                        found = true;
                        break;
                    }
                }
                if(!found) {
                    tableValidation.Add($"The unique code constraint name '{tableUniqueConstraint.ConstraintName}' does not exist in the database.");
                }
            }
        }

        private static void ValidateForeignKeys(ITable table, DatabaseTable dbTable, ValidationItem tableValidation) {

            if(table.ForeignKeys.Length != dbTable.ForeignKeys.Count) {
                tableValidation.Add($"The number of foreign keys defined in code is different from database. {table.ForeignKeys.Length} foreign keys are defined in code and {dbTable.ForeignKeys.Count} {(dbTable.ForeignKeys.Count == 1 ? "is" : "are")} defined in the database");
            }

            foreach(DatabaseForeignKey dbForeignKey in dbTable.ForeignKeys) {

                ForeignKey? matchingForeignKey = null;

                foreach(ForeignKey codeForeignKey in table.ForeignKeys) {

                    if(string.Compare(dbForeignKey.ConstraintName, codeForeignKey.ConstraintName, ignoreCase: true) == 0) {

                        if(matchingForeignKey != null) {
                            tableValidation.Add($"The foreign key '{matchingForeignKey.ConstraintName}'is defined more than once in code");
                            break;
                        }
                        matchingForeignKey = codeForeignKey;
                    }
                }
                if(matchingForeignKey == null) {
                    tableValidation.Add($"The foreign key '{dbForeignKey.ConstraintName}' is not defined in code");
                }
                else {  //Compare referenced table and columns

                    if(dbForeignKey.References.Count != matchingForeignKey.ColumnReferences.Count) {
                        tableValidation.Add($"The foreign key constraint '{matchingForeignKey.ConstraintName}' has a different number of column references between the code and database");
                    }
                    else {

                        for(int refIndex = 0; refIndex < dbForeignKey.References.Count; refIndex++) {

                            DatabaseForeignKeyReference dbReference = dbForeignKey.References[refIndex];

                            ForeignKeyReference codeReference = matchingForeignKey.ColumnReferences[refIndex];

                            string dbFkColumnName = dbReference.ForeignKeyColumn.ColumnName.Value;
                            string codeFkColumnName = codeReference.ForeignKeyColumn.ColumnName;

                            if(string.Compare(dbFkColumnName, codeFkColumnName, ignoreCase: true) != 0) {
                                tableValidation.Add($"The foreign key constraint '{matchingForeignKey.ConstraintName}' column references do not match. '{dbFkColumnName} != '{codeFkColumnName}'. (Note: Columns must be in the same order between the code and database)");
                            }
                            else {

                                string dbPkTableName = dbReference.PrimaryKeyColumn.Table.TableName.Value;
                                string codePkTableName = codeReference.PrimaryKeyColumn.Table.TableName;

                                if(string.Compare(dbPkTableName, codePkTableName, ignoreCase: true) != 0) {
                                    tableValidation.Add($"The foreign key constraint '{matchingForeignKey.ConstraintName}' column '{dbFkColumnName}' reference table is different between code and database. Table name: '{dbPkTableName}' != '{codePkTableName}'");
                                }
                            }
                        }
                    }
                }
            }

            foreach(ForeignKey codeForeignKey in table.ForeignKeys) {

                bool matchFound = false;

                foreach(DatabaseForeignKey dbForeignKey in dbTable.ForeignKeys) {

                    if(string.Compare(dbForeignKey.ConstraintName, codeForeignKey.ConstraintName, ignoreCase: true) == 0) {

                        if(matchFound) {
                            tableValidation.Add($"The foreign key '{codeForeignKey.ConstraintName}' is defined more than once in table code definition");
                        }
                        matchFound = true;
                    }
                }

                if(!matchFound) {
                    tableValidation.Add($"The foreign key '{codeForeignKey.ConstraintName}' is defined in code but does not exist in the database");
                }
            }
        }

        private static void ValidateMissingCodeTables(DatabaseSchema dbSchema, IDatabase database, List<ITable> tables, ValidationResult validationResult) {

            Dictionary<string, ITable> codeTableLookup = new Dictionary<string, ITable>(StringComparer.OrdinalIgnoreCase);

            foreach(ITable codeTable in tables) {

                string key = $"{database.SchemaMap(codeTable.SchemaName)}~~{codeTable.TableName}";

                if(!codeTableLookup.TryAdd(key, codeTable)) {
                    throw new Exception($"Table schema name and table name are not unique. Table: {codeTable.SchemaName}.{codeTable.TableName}");
                }
            }

            foreach(DatabaseTable dbTable in dbSchema.Tables) {

                if(string.Compare(dbTable.Schema.Value, "pg_catalog", ignoreCase: true) == 0 || string.Compare(dbTable.Schema.Value, "information_schema", ignoreCase: true) == 0) {
                    continue;
                }

                string key = $"{dbTable.Schema}~~{dbTable.TableName}";

                if(!codeTableLookup.ContainsKey(key)) {

                    ValidationItem validation = new ValidationItem(tableType: null, schema: dbTable.Schema.Value, tableName: dbTable.TableName.Value);

                    validation.Add($"Code table definition is missing for the database table: '{dbTable.Schema}.{dbTable.TableName}'. This can happen if the table code definition does not exist or the schema names do not match or the table names do not match.");
                    validationResult.Items.Add(validation);
                }
            }
        }

        private static List<CodeColumnProperty> LoadTableColumnsProperties(ITable table, ValidationItem tableValidation) {

            Type tableType = table.GetType();

            PropertyInfo[] properties = tableType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            List<CodeColumnProperty> columns = new List<CodeColumnProperty>();

            foreach(PropertyInfo property in properties) {

                Type underlyingPropertyType = property.PropertyType.IsGenericType ? property.PropertyType.GetGenericTypeDefinition() : property.PropertyType;

                if(underlyingPropertyType == typeof(Column<>) || underlyingPropertyType == typeof(NullableColumn<>)) {

                    object? column = property.GetValue(table);

                    if(column == null) {
                        tableValidation.Add($"Table: {table.TableName}, Column property '{property.Name}' is returning null. This property should have an IColumn assigned");
                    }
                    else {
                        SuppressColumnTypeValidationAttribute? suppressAttribute = property.GetCustomAttribute<SuppressColumnTypeValidationAttribute>();
                        columns.Add(new CodeColumnProperty(property.Name, (IColumn)column, suppressColumnTypeValidation: suppressAttribute != null));
                    }
                }
            }
            return columns;
        }

        private class CodeColumnProperty {

            public CodeColumnProperty(string propertyName, IColumn column, bool suppressColumnTypeValidation) {
                PropertyName = propertyName;
                Column = column;
                SuppressColumnTypeValidation = suppressColumnTypeValidation;
            }
            public string PropertyName { get; }
            public IColumn Column { get; }
            public bool SuppressColumnTypeValidation { get; }
        }

        private static Type ConvertToAdoType(Type type) {

            if(type.IsGenericType) {

                if(type.GetGenericTypeDefinition() == typeof(ShortKey<>)) {
                    type = typeof(short);
                }
                else if(type.GetGenericTypeDefinition() == typeof(IntKey<>)) {
                    type = typeof(int);
                }
                else if(type.GetGenericTypeDefinition() == typeof(LongKey<>)) {
                    type = typeof(long);
                }
                else if(type.GetGenericTypeDefinition() == typeof(GuidKey<>)) {
                    type = typeof(Guid);
                }
                else if(type.GetGenericTypeDefinition() == typeof(StringKey<>)) {
                    type = typeof(string);
                }
                else if(type.GetGenericTypeDefinition() == typeof(BoolValue<>)) {
                    type = typeof(bool);
                }                
            }
            else if(type.IsAssignableTo(typeof(IValue<Guid>))) {
                type = typeof(Guid);
            }
            else if(type.IsAssignableTo(typeof(IValue<short>))) {
                type = typeof(short);
            }
            else if(type.IsAssignableTo(typeof(IValue<int>))) {
                type = typeof(int);
            }
            else if(type.IsAssignableTo(typeof(IValue<long>))) {
                type = typeof(long);
            }
            else if(type.IsAssignableTo(typeof(IValue<string>))) {
                type = typeof(string);
            }
            else if(type.IsAssignableTo(typeof(IValue<bool>))) {
                type = typeof(bool);
            }
            else if(type.IsEnum) {
                type = typeof(int);
            }
            return type;
        }
    }
}