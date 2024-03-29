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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace QueryLite.DbSchema {

    public static class DocumentationGenerator {

        public static string GenerateForAssembly(Assembly[] assemblies, string applicationName, string version) {

            List<Type> types = new List<Type>();

            foreach(Assembly assembly in assemblies) {

                List<Type> listOfTypes = assembly.GetTypes()
                    .Select(type => type)
                    .Where(type => typeof(ITable).IsAssignableFrom(type) && type != typeof(ITable) && type != typeof(ATable)).ToList();

                types.AddRange(listOfTypes);
            }
            return GenerateForTableTypes(types, applicationName, version);
        }

        public static string GenerateForTables(List<ITable> tables, string applicationName, string version) {

            List<Type> types = new List<Type>();

            foreach(ITable table in tables) {
                types.Add(table.GetType());
            }
            return GenerateForTableTypes(types, applicationName, version);
        }

        public static string GenerateForTableTypes(List<Type> types, string applicationName, string version) {

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

                List<ColumnAndDescription> columns = LoadTableColumns(table.TableClass, tableValidation, out List<ForeignKey> foreignKeys);

                table.Columns.AddRange(columns);

                tableLookup.Add(type, table);
            }
            tables.Sort((a, b) => {

                int compare = a.TableClass.SchemaName.CompareTo(b.TableClass.SchemaName);

                if(compare == 0) {
                    compare = a.TableClass.TableName.CompareTo(b.TableClass.TableName);
                }
                return compare;
            });
            return GenerateForTables(tables, applicationName, version);
        }

        private static string GenerateForTables(List<Table> tables, string applicationName, string version) {

            StringBuilder html = new StringBuilder();

            html.Append("<html><header>");

            html.Append(
@"
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

            if(!string.IsNullOrWhiteSpace(applicationName) || !string.IsNullOrWhiteSpace(version)) {

                html.Append("<h1>");

                if(!string.IsNullOrWhiteSpace(applicationName)) {
                    html.Append(WebUtility.HtmlEncode(applicationName)).Append(' ');
                }
                if(!string.IsNullOrWhiteSpace(version)) {
                    html.Append('(').Append(WebUtility.HtmlEncode(version)).Append(')').Append(' ');
                }
                html.Append("</h1>");
            }
            html.Append("<h1>Schema Documentation</h1>");

            html.Append("<h1>Index</h1>");

            html.Append("<h2>Tables & Views</h2>");

            html.Append("<ul>");

            for(int index = 0; index < tables.Count; index++) {
                Table table = tables[index];
                html.Append("<li><a href=#").Append(index).Append('>').Append(WebUtility.HtmlEncode($"[{table.TableClass.SchemaName}].{table.TableClass.TableName}")).Append($"</a>{(table.TableClass.IsView ? "&nbsp;(View)" : string.Empty)}</li>");
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
            html.Append($"<h2 id={id}>{(table.TableClass.IsView ? "View" : "Table")}: ").Append(WebUtility.HtmlEncode($"[{table.TableClass.SchemaName}].{table.TableClass.TableName}")).Append("</h2>");
            html.Append("<p>").Append(WebUtility.HtmlEncode(table.Description)).Append("</p>");

            {
                html.Append($"<h3>Columns({table.Columns.Count})</h3><p>");

                html.Append("<table style=\"width:100%\">");

                html.Append("<tr><th style=\"width:20%;text-align:left;\"> Column Name</th><th style=\"width:10%\">.net Type</th><th>Nullable</th><th>Auto</th><th style=\"width:50%;text-align:left;\">Description</th><tr>");

                foreach(ColumnAndDescription colAndDesc in table.Columns) {

                    IColumn column = colAndDesc.Column;

                    html.Append($"<tr>");
                    html.Append($"<td style=\"text-align:left;\">").Append(WebUtility.HtmlEncode(column.ColumnName)).Append("</td>");
                    html.Append($"<td>").Append(WebUtility.HtmlEncode(column.Type.Name));

                    if(column.Length != null && column.Type != typeof(IGeography)) {
                        html.Append('(').Append(column.Length.Value).Append(')');
                    }
                    html.Append("</td>");
                    html.Append($"<td>").Append(WebUtility.HtmlEncode(column.IsNullable ? "NULL" : "NOT NULL")).Append("</td>");

                    //html.Append($"<td>").Append(WebUtility.HtmlEncode((column.IsPrimaryKey ? column.IsPrimaryKey.ToString() : string.Empty))).Append("</td>");
                    html.Append($"<td>").Append(WebUtility.HtmlEncode((column.IsAutoGenerated ? column.IsAutoGenerated.ToString() : string.Empty))).Append("</td>");
                    html.Append($"<td style=\"text-align:left;\">").Append(WebUtility.HtmlEncode(colAndDesc.Description)).Append("</td>");

                    html.Append("</tr>");
                }
                html.Append("</table>");
            }

            if(table.TableClass.PrimaryKey != null || table.TableClass.ForeignKeys.Length > 0) {

                int counter = 0;

                PrimaryKey? primaryKey = table.TableClass.PrimaryKey;

                if(primaryKey != null) {

                    html.Append("<h3>Primary Key</h3><p>");
                    html.Append("<table><tr><th style=\"text-align: center;\">Name</th><th style=\"text-align: center;\">Columns</th>");

                    html.Append("<tr><td>").Append(WebUtility.HtmlEncode(primaryKey.ConstraintName)).Append("</td>");
                    html.Append("<td>");

                    foreach(IColumn pkColumn in primaryKey.Columns) {

                        if(counter > 0) {
                            html.Append(",&nbsp;");
                        }
                        html.Append(WebUtility.HtmlEncode(pkColumn.ColumnName));
                        counter++;
                    }
                    html.Append("</td></table>");
                }

                html.Append($"<h3>Unique Constraints({table.TableClass.UniqueConstraints.Length})</h3><p>");
                html.Append("<table><tr><th style=\"text-align: center;\">Name</th><th style=\"text-align: center;\">Columns</th>");

                counter = 0;

                foreach(UniqueConstraint uniqueConstraint in table.TableClass.UniqueConstraints) {

                    html.Append("<tr><td>").Append(WebUtility.HtmlEncode(uniqueConstraint.ConstraintName)).Append("</td>");
                    html.Append("<td>");

                    foreach(IColumn columnName in uniqueConstraint.Columns) {

                        if(counter > 0) {
                            html.Append(",&nbsp;");
                        }
                        html.Append(WebUtility.HtmlEncode(columnName.ColumnName));
                        counter++;
                    }
                    html.Append("</td>");

                }
                html.Append("</table>");

                html.Append($"<h3>Foreign Keys({table.TableClass.ForeignKeys.Length})</h3><p>");
                html.Append("<table><tr><th style=\"text-align: center;\">Name</th><th style=\"text-align: center;\">Columns</th><th style=\"text-align: center;\">References</th>");

                foreach(ForeignKey foreignKey in table.TableClass.ForeignKeys) {

                    counter = 0;

                    html.Append("<tr><td>").Append(WebUtility.HtmlEncode(foreignKey.ConstraintName)).Append("</td>");
                    html.Append("<td>");

                    foreach(ForeignKeyReference reference in foreignKey.ColumnReferences) {

                        if(counter > 0) {
                            html.Append(",&nbsp;");
                        }
                        html.Append(WebUtility.HtmlEncode(reference.ForeignKeyColumn.ColumnName));
                        counter++;
                    }
                    html.Append("</td>");
                    html.Append("<td>");

                    counter = 0;

                    foreach(ForeignKeyReference reference in foreignKey.ColumnReferences) {

                        if(counter == 0) {
                            html.Append(WebUtility.HtmlEncode(reference.PrimaryKeyColumn.Table.TableName)).Append('(');
                        }
                        if(counter > 0) {
                            html.Append(",&nbsp;");
                        }
                        html.Append(WebUtility.HtmlEncode(reference.PrimaryKeyColumn.ColumnName));
                        counter++;
                    }
                    if(foreignKey.ColumnReferences.Count > 0) {
                        html.Append(')');
                    }
                    html.Append("</td>");
                }
                html.Append("</table>");
                html.Append("</p>");
            }
            return html.ToString();
        }

        private static List<ColumnAndDescription> LoadTableColumns(ITable table, TableValidation tableValidation, out List<ForeignKey> foreignKeys) {

            Type tableType = table.GetType();

            PropertyInfo[] properties = tableType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            List<ColumnAndDescription> columns = new List<ColumnAndDescription>();

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
                    }
                }
            }
            return columns;
        }

        private sealed class Table {

            public ITable TableClass { get; }
            public string Description { get; set; } = string.Empty;
            public List<ColumnAndDescription> Columns { get; } = new List<ColumnAndDescription>();

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

        //private sealed class ForeignKey {

        //    public string Name { get; }
        //    public Type PrimaryKeyTableType { get; }
        //    public List<IColumn> Columns { get; } = new List<IColumn>();

        //    public Table? PrimaryKeyTable { get; set; }

        //    public ForeignKey(string name, Type primaryKeyTableType) {
        //        Name = name;
        //        PrimaryKeyTableType = primaryKeyTableType;
        //    }
        //}
    }
}