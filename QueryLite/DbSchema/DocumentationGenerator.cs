﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace QueryLite.DbSchema {

    public static class DocumentationGenerator {

        public static string GenerateForAssembly(Assembly[] assemblies) {

            List<Type> types = new List<Type>();

            foreach(Assembly assembly in assemblies) {

                List<Type> listOfTypes = assembly.GetTypes()
                    .Select(type => type)
                    .Where(type => typeof(ITable).IsAssignableFrom(type) && type != typeof(ITable) && type != typeof(ATable)).ToList();

                types.AddRange(listOfTypes);
            }
            return GenerateForTableTypes(types);
        }

        public static string GenerateForTables(List<ITable> tables) {

            List<Type> types = new List<Type>();

            foreach(ITable table in tables) {
                types.Add(table.GetType());
            }
            return GenerateForTableTypes(types);
        }

        public static string GenerateForTableTypes(List<Type> types) {

            List<TableValidation> validation = new List<TableValidation>();

            List<Table> tables = new List<Table>();

            Dictionary<Type, Table> tableLookup = new Dictionary<Type, Table>();

            foreach(Type type in types) {

                Table table = new Table((ITable)Activator.CreateInstance(type, nonPublic: true)!);
                tables.Add(table);

                DescriptionAttribute[] tableDescriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);

                if(tableDescriptions.Length > 0) {
                    table.Description = tableDescriptions[0].Description;
                }

                TableValidation tableValidation = new TableValidation(table.GetType());

                List<ColumnAndDescription> columns = LoadTableColumns(table.TableClass, tableValidation, out PrimaryKey? primaryKey, out List<ForeignKey> foreignKeys);

                table.Columns.AddRange(columns);
                table.PrimaryKey = primaryKey;
                table.ForeignKeys = foreignKeys;

                tableLookup.Add(type, table);
            }

            foreach(Table table in tables) {

                foreach(ForeignKey foreignKey in table.ForeignKeys) {

                    if(tableLookup.TryGetValue(foreignKey.PrimaryKeyTableType, out Table? primaryKeyTable)) {
                        foreignKey.PrimaryKeyTable = primaryKeyTable;
                    }
                }
            }
            tables.Sort((a, b) => a.TableClass.TableName.CompareTo(b.TableClass.TableName));
            return GenerateForTables(tables);
        }

        private static string GenerateForTables(List<Table> tables) {

            StringBuilder html = new StringBuilder();

            html.Append("<html><header>");

            html.Append(@"
            <style>
                body {
                    font-family: Arial, Helvetica, sans-serif;
                }
                table, th, td {
                  border: 1px solid black;
                  border-collapse: collapse;
                  padding: 5px;
                  text-align: center;
                }
                th {
                    background-color: #abb2b9;
                }
                tr:nth-child(even) {
                    background-color: #eaecee;
                }
                h2 {
                    color: #b03a2e;
                }
                h3 {
                    color: #2874a6;
                }
                a, a:visited {
                    color: blue;
                }
            </style>"
            );
            html.Append(" </header>");

            html.Append("<body>");

            html.Append("<h1>Schema Documentation</h1>");

            html.Append("<h1>Index</h1>");

            html.Append("<h2>Tables & Views:</h2>");

            html.Append("<ul>");

            for(int index = 0; index < tables.Count; index++) {
                Table table = tables[index];
                html.Append("<li><a href=#").Append(index).Append('>').Append(WebUtility.HtmlEncode(table.TableClass.TableName)).Append("</a></li>");
            }
            html.Append("</ul>");

            for(int index = 0; index < tables.Count; index++) {
                Table table = tables[index];
                html.AppendLine(GenerateForTable(table, id: index));
                html.Append("<br/>");
            }
            html.Append("</body></html>");
            return html.ToString();
        }

        private static string GenerateForTable(Table table, int id) {

            StringBuilder html = new StringBuilder();

            html.Append("<hr/>");
            html.Append($"<h2 id={id}>Table: ").Append(WebUtility.HtmlEncode(table.TableClass.TableName)).Append("</h2>");
            html.Append("<p>").Append(WebUtility.HtmlEncode(table.Description)).Append("</p>");

            if(table.PrimaryKey != null || table.ForeignKeys.Count > 0) {

                int counter = 0;

                if(table.PrimaryKey != null) {

                    html.Append("<h3>Primary Key</h3><p>");
                    html.Append("<table><tr><th style=\"text-align: center;\">Name</th><th style=\"text-align: center;\">Columns</th>");

                    html.Append("<tr><td>").Append(WebUtility.HtmlEncode(table.PrimaryKey.Name)).Append("</td>");
                    html.Append("<td>");

                    foreach(IColumn pkColumn in table.PrimaryKey.Columns) {

                        if(counter > 0) {
                            html.Append(",&nbsp;");
                        }
                        html.Append(WebUtility.HtmlEncode(pkColumn.ColumnName));
                        counter++;
                    }
                    html.Append("</td></table>");
                }

                html.Append($"<h3>Foreign Keys({table.ForeignKeys.Count})</h3><p>");
                html.Append("<table><tr><th style=\"text-align: center;\">Name</th><th style=\"text-align: center;\">Columns</th><th style=\"text-align: center;\">References</th>");

                foreach(ForeignKey foreignKey in table.ForeignKeys) {

                    counter = 0;

                    html.Append("<tr><td>").Append(WebUtility.HtmlEncode(foreignKey.Name)).Append("</td>");
                    html.Append("<td>");

                    foreach(IColumn pkColumn in foreignKey.Columns) {

                        if(counter > 0) {
                            html.Append(",&nbsp;");
                        }
                        html.Append(WebUtility.HtmlEncode(pkColumn.ColumnName));
                        counter++;
                    }
                    html.Append("</td>");

                    html.Append("<td>");

                    if(foreignKey.PrimaryKeyTable != null) {

                        html.Append(WebUtility.HtmlEncode(foreignKey.PrimaryKeyTable.TableClass.TableName));

                        html.Append('(');

                        int pkCounter = 0;

                        if(foreignKey.PrimaryKeyTable.PrimaryKey != null) {

                            foreach(IColumn pkColumn in foreignKey.PrimaryKeyTable.PrimaryKey.Columns) {

                                if(pkCounter > 0) {
                                    html.Append(", ");
                                }
                                html.Append(pkColumn.ColumnName);
                                pkCounter++;
                            }
                        }
                        html.Append(')');
                    }
                    html.Append("</td>");
                }
                html.Append("</table>");
                html.Append("</p>");
            }

            html.Append($"<h3>Columns({table.Columns.Count})</h3><p>");

            html.Append("<table style=\"width:100%\">");

            html.Append("<tr><th style=\"width:20%;text-align:left;\"> Column Name</th><th style=\"width:10%\">.net Type</th><th>Nullable</th><th>PK</th><th>Auto</th><th style=\"width:50%;text-align:left;\">Description</th><tr>");

            foreach(ColumnAndDescription colAndDesc in table.Columns) {

                IColumn column = colAndDesc.Column;

                html.Append($"<tr>");
                html.Append($"<td style=\"text-align:left;\">").Append(WebUtility.HtmlEncode(column.ColumnName)).Append("</td>");
                html.Append($"<td>").Append(WebUtility.HtmlEncode(column.Type.Name));

                if(column.Length != null) {
                    html.Append('(').Append(column.Length.Value).Append(')');
                }
                html.Append("</td>");
                html.Append($"<td>").Append(WebUtility.HtmlEncode(column.IsNullable ? "True" : "")).Append("</td>");

                html.Append($"<td>").Append(WebUtility.HtmlEncode((column.IsPrimaryKey ? column.IsPrimaryKey.ToString() : string.Empty))).Append("</td>");
                html.Append($"<td>").Append(WebUtility.HtmlEncode((column.IsAutoGenerated ? column.IsAutoGenerated.ToString() : string.Empty))).Append("</td>");
                html.Append($"<td style=\"text-align:left;\">").Append(WebUtility.HtmlEncode(colAndDesc.Description)).Append("</td>");

                html.Append("</tr>");
            }
            html.Append("</table>");
            return html.ToString();
        }

        private static List<ColumnAndDescription> LoadTableColumns(ITable table, TableValidation tableValidation, out PrimaryKey? primaryKey, out List<ForeignKey> foreignKeys) {

            Type tableType = table.GetType();

            PropertyInfo[] properties = tableType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            List<ColumnAndDescription> columns = new List<ColumnAndDescription>();

            primaryKey = null;
            foreignKeys = new List<ForeignKey>();

            foreach(PropertyInfo property in properties) {

                Type underlyingPropertyType = property.PropertyType.IsGenericType ? property.PropertyType.GetGenericTypeDefinition() : property.PropertyType;

                if(underlyingPropertyType == typeof(Column<>) || underlyingPropertyType == typeof(NullableColumn<>)) {

                    IColumn? column = (IColumn?)property.GetValue(table);

                    if(column == null) {
                        tableValidation.Add($"Table: {table.TableName}, Column property '{property.Name}' is returning null. This property should have an IColumn assigned");
                    }
                    else {

                        DescriptionAttribute[] descriptions = (DescriptionAttribute[])property.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);

                        string description = string.Empty;

                        if(descriptions.Length > 0) {
                            description = descriptions[0].Description;
                        }
                        columns.Add(new ColumnAndDescription(column, description));

                        PrimaryKeyAttribute[] primaryKeyAttributes = (PrimaryKeyAttribute[])property.GetCustomAttributes(typeof(PrimaryKeyAttribute), inherit: false);

                        if(primaryKeyAttributes.Length > 0) {

                            if(primaryKey == null) {
                                primaryKey = new PrimaryKey(primaryKeyAttributes[0].Name);
                            }
                            primaryKey.Columns.Add(column);
                        }

                        object[] foreignKeyAttributes = property.GetCustomAttributes(typeof(ForeignKeyAttribute<>), inherit: false);

                        if(foreignKeyAttributes.Length > 0) {

                            ForeignKey? foreignKey = null;

                            IForeignKeyAttribute foreignKeyAttribute = (IForeignKeyAttribute)foreignKeyAttributes[0];

                            foreach(ForeignKey findKey in foreignKeys) {

                                if(string.Compare(findKey.Name, foreignKeyAttribute.Name, ignoreCase: true) == 0) {
                                    foreignKey = findKey;
                                    break;
                                }
                            }
                            if(foreignKey == null) {
                                foreignKey = new ForeignKey(foreignKeyAttribute.Name, foreignKeyAttribute.PrimaryKeyTable);
                                foreignKeys.Add(foreignKey);
                            }
                            foreignKey.Columns.Add(column);
                        }
                    }
                }
            }
            return columns;
        }

        private sealed class Table {

            public ITable TableClass { get; }
            public string Description { get; set; } = string.Empty;
            public List<ColumnAndDescription> Columns { get; } = new List<ColumnAndDescription>();
            public PrimaryKey? PrimaryKey { get; set; }
            public List<ForeignKey> ForeignKeys { get; set; } = new List<ForeignKey>();

            public Table(ITable table) {
                TableClass = table;
            }
        }

        private sealed class ColumnAndDescription {

            public IColumn Column { get; }
            public string Description { get; }

            public ColumnAndDescription(IColumn column, string description) {
                Column = column;
                Description = description;
            }
        }

        private sealed class PrimaryKey {

            public string Name { get; }
            public List<IColumn> Columns { get; } = new List<IColumn>();

            public PrimaryKey(string name) {
                Name = name;
            }
        }

        private sealed class ForeignKey {

            public string Name { get; }
            public Type PrimaryKeyTableType { get; }
            public List<IColumn> Columns { get; } = new List<IColumn>();

            public Table? PrimaryKeyTable { get; set; }

            public ForeignKey(string name, Type primaryKeyTableType) {
                Name = name;
                PrimaryKeyTableType = primaryKeyTableType;
            }
        }
    }
}