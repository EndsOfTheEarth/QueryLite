﻿/*
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
using QueryLite.DbSchema;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace QueryLite {

    public sealed class TableValidation {

        public Type? TableType { get; }
        public ITable? Table { get; set; }
        public bool HasErrors => ValidationMessages.Count > 0;
        public List<string> ValidationMessages { get; } = new List<string>();

        public void Add(string message) {
            ValidationMessages.Add(message);
        }

        public TableValidation(Type? tableType) {
            TableType = tableType;
        }
        public override string? ToString() {
            return TableType?.FullName ?? base.ToString();
        }
    }

    public class SchemaValidationSettings {

        public required bool ValidatePrimaryKeyAttributes { get; init; }
        public required bool ValidateForeignKeyAttributes { get; init; }
        public required bool ValidateMissingCodeTables { get; init; }
    }


    public class ValidationResult {

        public List<TableValidation> TableValidation { get; } = new List<TableValidation>();
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
                    TableValidation tableValidation = new TableValidation(tableType: null);
                    tableValidation.Add($"Warning: Unable to load the a type from the assembly '{assembly.FullName}'.{Environment.NewLine}{ex}");
                    result.TableValidation.Add(tableValidation);
                }
            }

            List<ITable> codeTables = new List<ITable>();

            foreach(Type type in types) {

                result.TableValidation.Add(ValidateFromType(type, database, dbSchema, validationSettings, out ITable? table));

                if(table != null) {
                    codeTables.Add(table);
                }
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, codeTables, result);
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

                result.TableValidation.Add(ValidateFromType(type, database, dbSchema, validationSettings, out ITable? table));

                if(table != null) {
                    codeTables.Add(table);
                }
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, codeTables, result);
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

                result.TableValidation.Add(ValidateFromType(type, database, dbSchema, validationSettings, out ITable? table));

                if(table != null) {
                    codeTables.Add(table);
                }
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, codeTables, result);
            }
            return result;
        }

        public static ValidationResult ValidateTables(IDatabase database, List<ITable> tables, SchemaValidationSettings validationSettings) {

            using DbConnection dbConnection = database.GetNewConnection();

            DatabaseSchema dbSchema = LoadSchema(database);

            dbConnection.Close();

            ValidationResult result = new ValidationResult(dbSchema);

            foreach(ITable table in tables) {
                result.TableValidation.Add(ValidateFromType(table.GetType(), database, dbSchema, validationSettings, table: out _));
            }

            if(validationSettings.ValidateMissingCodeTables) {
                ValidateMissingCodeTables(dbSchema, tables, result);
            }
            return result;
        }

        public static TableValidation ValidateTable(IDatabase database, ITable table, SchemaValidationSettings validationSettings) {

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

        private static TableValidation ValidateFromType(Type type, IDatabase database, DatabaseSchema dbSchema, SchemaValidationSettings validationSettings, out ITable? table) {

            FieldInfo? instanceField1 = type.GetField("Instance1");

            if(instanceField1 == null) {
                instanceField1 = type.GetField("Instance");
            }

            PropertyInfo? instance1Property = type.GetProperty("Instance1");

            if(instance1Property == null) {
                type.GetProperty("Instance");
            }

            TableValidation tableValidation = new TableValidation(type);

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
                tableValidation.Table = table;
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

        public static void Validate(IDatabase database, ITable table, DatabaseSchema dbSchema, TableValidation tableValidation, SchemaValidationSettings validationSettings) {

            if(string.IsNullOrWhiteSpace(table.SchemaName)) {
                tableValidation.Add($"Table schema name cannot be null or empty in code");
            }

            List<DatabaseTable> dbTables = FindMatchingSchemaTables(database, table, dbSchema);

            if(dbTables == null) {
                tableValidation.Add($"Code table exists but not database table");
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

                    Type dbNetType = dbColumn.DataType.DotNetType;
                    Type codeAdoType = ConvertToAdoType(codeColumn.Type);    //This is for the case with types like IntKey<> where the ado type is int

                    if(codeColumn.Type.IsEnum) {

                        if(dbNetType != typeof(short) && dbNetType != typeof(int) && dbNetType != typeof(long)) {
                            throw new Exception("An enum type must be mapped as a short, integer or long. This might be a bug");
                        }
                        if(codeAdoType != typeof(int) && codeAdoType != typeof(short) && codeAdoType != typeof(byte)) {
                            tableValidation.Add($"{columnDetail}, column types are different ({codeAdoType.Name} != {dbNetType.Name}) between database and code column");
                        }
                    }
                    else if(codeAdoType != dbNetType) {
                        tableValidation.Add($"{columnDetail}, column types are different ({codeAdoType.Name} != {dbNetType.Name}) between database and code column");
                    }

                    if(dbColumn.IsNullable && !codeColumn.IsNullable) {
                        tableValidation.Add($"{columnDetail}, database column is nullable but code column is non-nullable");
                    }
                    else if(!dbColumn.IsNullable && codeColumn.IsNullable) {
                        tableValidation.Add($"{columnDetail}, database column is non-nullable but code column is nullable");
                    }

                    if(dbColumn.IsPrimaryKey && !codeColumn.IsPrimaryKey) {
                        tableValidation.Add($"{columnDetail}, database column is a primary key field but code column is not");
                    }
                    else if(!dbColumn.IsPrimaryKey && codeColumn.IsPrimaryKey) {
                        tableValidation.Add($"{columnDetail}, database column is not a primary key field but code column is");
                    }

                    if(dbColumn.IsAutoGenerated && !codeColumn.IsAutoGenerated) {
                        tableValidation.Add($"{columnDetail}, database column is an auto number but code column is not");
                    }
                    else if(!dbColumn.IsAutoGenerated && codeColumn.IsAutoGenerated) {
                        tableValidation.Add($"{columnDetail}, database column is not an auto number but code column is");
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
            if(validationSettings.ValidatePrimaryKeyAttributes) {
                ValidatePrimaryKeyForTable(database, table, tableColumnProperties, dbTable, tableValidation);
            }

            if(validationSettings.ValidateForeignKeyAttributes) {
                ValidateForeignKeys(database, table, tableColumnProperties, dbTable, tableValidation);
            }
        }

        private static void ValidatePrimaryKeyForTable(IDatabase database, ITable table, List<CodeColumnProperty> tableColumnProperties, DatabaseTable dbTable, TableValidation tableValidation) {

            HashSet<CodeColumnProperty> hasValidationError = new HashSet<CodeColumnProperty>();

            //Check primary key in database exists in the code table
            foreach(DatabaseColumn dbColumn in dbTable.Columns) {

                if(!string.IsNullOrEmpty(dbColumn.PrimaryKeyConstraintName)) {

                    foreach(CodeColumnProperty columnProperty in tableColumnProperties) {

                        if(string.Compare(columnProperty.Column.ColumnName, dbColumn.ColumnName.Value, ignoreCase: true) == 0) {

                            if(columnProperty.PrimaryKeyAttributes.Length == 0) {
                                tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', should have a '{nameof(PrimaryKeyAttribute)}' for the primary key constraint '{dbColumn.PrimaryKeyConstraintName}'");
                                hasValidationError.Add(columnProperty);
                            }
                            else {

                                foreach(PrimaryKeyAttribute attr in columnProperty.PrimaryKeyAttributes) { //Validate all primary key attributes even though only one is allowed. The next stage of validaiton will check that only one attribute exists

                                    if(string.Compare(attr.Name, dbColumn.PrimaryKeyConstraintName, ignoreCase: true) != 0) {
                                        tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', the attribute {nameof(PrimaryKeyAttribute)} name '{attr.Name}' does not match the primary key constraint name '{dbColumn.PrimaryKeyConstraintName}' in the database'");
                                        hasValidationError.Add(columnProperty);
                                    }
                                    else if(!columnProperty.Column.IsPrimaryKey) {
                                        tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', {nameof(columnProperty.Column.IsPrimaryKey)} property should be set to true");
                                        hasValidationError.Add(columnProperty);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            //Check primary key in code exists in the database table
            foreach(CodeColumnProperty columnProperty in tableColumnProperties) {

                if(hasValidationError.Contains(columnProperty)) {
                    continue;
                }

                if(columnProperty.PrimaryKeyAttributes.Length == 0) {

                    if(columnProperty.Column.IsPrimaryKey) {
                        tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', {nameof(columnProperty.Column.IsPrimaryKey)} property is set to true but no '{nameof(PrimaryKeyAttribute)}' exists");
                    }
                    continue;
                }

                if(columnProperty.PrimaryKeyAttributes.Length > 1) {
                    tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', should not have more than one '{nameof(PrimaryKeyAttribute)}' attibute");
                    continue;
                }

                PrimaryKeyAttribute attr = columnProperty.PrimaryKeyAttributes[0];

                foreach(DatabaseColumn dbColumn in dbTable.Columns) {

                    if(string.Compare(columnProperty.Column.ColumnName, dbColumn.ColumnName.Value, ignoreCase: true) == 0) {

                        if(string.IsNullOrEmpty(dbColumn.PrimaryKeyConstraintName)) {
                            tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', the attribute {nameof(PrimaryKeyAttribute)} name '{attr.Name}' does not exist for column in the database table'");
                        }
                        else if(string.Compare(attr.Name, dbColumn.PrimaryKeyConstraintName, ignoreCase: true) != 0) {
                            tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', the attribute {nameof(PrimaryKeyAttribute)} name '{attr.Name}' does not match the primary key constraint name '{dbColumn.PrimaryKeyConstraintName}' in the database'");
                        }
                        else if(!columnProperty.Column.IsPrimaryKey) {
                            tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', {nameof(columnProperty.Column.IsPrimaryKey)} property should be set to true");
                        }
                        break;
                    }
                }
            }
        }

        private static void ValidateForeignKeys(IDatabase database, ITable table, List<CodeColumnProperty> tableColumnProperties, DatabaseTable dbTable, TableValidation tableValidation) {

            HashSet<CodeColumnProperty> hasValidationError = new HashSet<CodeColumnProperty>();

            //Check that database foreign keys exists in the code table
            foreach(DatabaseColumn dbColumn in dbTable.Columns) {

                foreach(ForeignKey foreignKey in dbColumn.ForeignKeys) {

                    foreach(CodeColumnProperty columnProperty in tableColumnProperties) {

                        if(string.Compare(columnProperty.Column.ColumnName, dbColumn.ColumnName.Value, ignoreCase: true) == 0) {

                            if(columnProperty.ForeignKeyAttributes.Length == 0) {
                                tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', should have a '{nameof(ForeignKeyAttribute<ITable>)}' for the foreign key constraint '{foreignKey.ConstraintName}'");
                                hasValidationError.Add(columnProperty);
                            }
                            else {

                                bool foreignKeyExistsInCode = false;

                                foreach(IForeignKeyAttribute attr in columnProperty.ForeignKeyAttributes) { //Validate all foreign key attributes in the code definition

                                    if(string.Compare(attr.Name, foreignKey.ConstraintName, ignoreCase: true) == 0) {
                                        foreignKeyExistsInCode = true;
                                        break;
                                    }
                                }
                                if(!foreignKeyExistsInCode) {
                                    tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', the database foreign key '{foreignKey.ConstraintName}' is not defined against the code column property");
                                    hasValidationError.Add(columnProperty);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            //Check foreign key in code exists in the database table
            foreach(CodeColumnProperty columnProperty in tableColumnProperties) {

                if(hasValidationError.Contains(columnProperty) || columnProperty.ForeignKeyAttributes.Length == 0) {
                    continue;
                }

                foreach(DatabaseColumn dbColumn in dbTable.Columns) {

                    if(string.Compare(columnProperty.Column.ColumnName, dbColumn.ColumnName.Value, ignoreCase: true) == 0) {

                        foreach(IForeignKeyAttribute attr in columnProperty.ForeignKeyAttributes) {

                            bool foundForeignKeyInDatabase = false;

                            foreach(ForeignKey dbForeignKey in dbColumn.ForeignKeys) {

                                if(string.Compare(dbForeignKey.ConstraintName, attr.Name, ignoreCase: true) == 0) {
                                    foundForeignKeyInDatabase = true;
                                    break;
                                }
                            }
                            if(!foundForeignKeyInDatabase) {
                                tableValidation.Add($"Table Column Property: '{columnProperty.PropertyName}', the foreign key attribute {nameof(ForeignKeyAttribute<ITable>)} name '{attr.Name}' does not exist in the database");
                            }
                        }
                    }
                }
            }
        }

        private static void ValidateMissingCodeTables(DatabaseSchema dbSchema, List<ITable> tables, ValidationResult validationResult) {

            Dictionary<string, ITable> codeTableLookup = new Dictionary<string, ITable>(StringComparer.OrdinalIgnoreCase);

            foreach(ITable codeTable in tables) {

                string key = $"{codeTable.SchemaName}~~{codeTable.TableName}";

                if(!codeTableLookup.TryAdd(key, codeTable)) {
                    throw new Exception($"Table schema name and table name are not unique. Table: {codeTable.SchemaName}.{codeTable.TableName}");
                }
            }
            
            foreach(DatabaseTable dbTable in dbSchema.Tables) {

                string key = $"{dbTable.Schema}~~{dbTable.TableName}";

                if(!codeTableLookup.ContainsKey(key)) {

                    TableValidation validation = new TableValidation(tableType: null);

                    validation.Add($"Code table definition is missing for the database table: '{dbTable.Schema}.{dbTable.TableName}'");
                    validationResult.TableValidation.Add(validation);
                }
            }
        }

        private static List<CodeColumnProperty> LoadTableColumnsProperties(ITable table, TableValidation tableValidation) {

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
                        PrimaryKeyAttribute[] primaryKeyAttributes = (PrimaryKeyAttribute[])property.GetCustomAttributes(typeof(PrimaryKeyAttribute), inherit: false);

                        IForeignKeyAttribute[] obj = (IForeignKeyAttribute[])property.GetCustomAttributes(typeof(IForeignKeyAttribute), inherit: false);

                        List<IForeignKeyAttribute> foreignKeyAttributes = new List<IForeignKeyAttribute>();

                        foreach(Attribute propAttr in property.GetCustomAttributes()) {

                            if(propAttr is IForeignKeyAttribute foreignKeyAttr) {
                                foreignKeyAttributes.Add(foreignKeyAttr);
                            }
                        }
                        columns.Add(new CodeColumnProperty(property.Name, (IColumn)column, primaryKeyAttributes, foreignKeyAttributes.ToArray()));
                    }
                }
            }
            return columns;
        }

        private class CodeColumnProperty {

            public CodeColumnProperty(string propertyName, IColumn column, PrimaryKeyAttribute[] primaryKeyAttributes, IForeignKeyAttribute[] foreignKeyAttributes) {
                PropertyName = propertyName;
                Column = column;
                PrimaryKeyAttributes = primaryKeyAttributes;
                ForeignKeyAttributes = foreignKeyAttributes;
            }
            public string PropertyName { get; }
            public IColumn Column { get; }
            public PrimaryKeyAttribute[] PrimaryKeyAttributes { get; }
            public IForeignKeyAttribute[] ForeignKeyAttributes { get; }
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

            if(type.IsEnum) {
                type = typeof(int);
            }
            return type;
        }
    }
}