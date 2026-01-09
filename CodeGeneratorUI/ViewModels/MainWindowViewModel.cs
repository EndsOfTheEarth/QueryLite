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
using CommunityToolkit.Mvvm.ComponentModel;
using QueryLite.DbSchema;
using System.Collections.ObjectModel;

namespace QueryLite.CodeGeneratorUI.ViewModels {

    public partial class MainWindowViewModel : ViewModelBase {

        internal ObservableCollection<TreeNodeViewModel> Nodes { get; set; }
        public ObservableCollection<TreeNodeViewModel> SelectedNodes { get; }

        public MainWindowViewModel() {
            Nodes = [];
            SelectedNodes = [];
        }
    }

    public partial class TreeNodeViewModel : ViewModelBase {

        public string Text { get; set; } = "";

        [ObservableProperty]
        private bool isEnabled = true;

        public ObservableCollection<TreeNodeViewModel> Nodes { get; set; } = [];
    }
    internal class SchemaNode : TreeNodeViewModel {

        public SchemaName SchemaName { get; }

        public SchemaNode(SchemaName schemaName) {
            SchemaName = schemaName;
            Text = !string.IsNullOrWhiteSpace(SchemaName.Value) ? SchemaName.Value : "[Unknown]";
        }
    }
    internal class TableNode : TreeNodeViewModel {

        public DatabaseTable Table { get; }

        public TableNode(DatabaseTable table) {
            Table = table;
            Text = $"{table.TableName.Value}{(table.IsView ? " (View)" : "")}";
        }
    }
    internal partial class ColumnNode : TreeNodeViewModel {

        public TableNode TableNode { get; }
        public DatabaseColumn Column { get; }

        public ColumnNode(TableNode tableNode, DatabaseColumn column) {
            TableNode = tableNode;
            Column = column;
            Text = $"{column.ColumnName} ({column.SqlDataTypeName} {(column.IsNullable ? "NULL" : "NOT NULL")} {(column.IsPrimaryKey ? "PK" : "")})";
        }
    }
}