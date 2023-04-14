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
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;
using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;
using QueryLite.DbSchema.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace QueryLite.CodeGeneratorUI {

    public partial class MainForm : Form {

        public MainForm() {

            InitializeComponent();

            cboDatabaseType.Items.Add(new DatabaseTypeItem(DatabaseType.PostgreSql));
            cboDatabaseType.Items.Add(new DatabaseTypeItem(DatabaseType.SqlServer));

            cboDatabaseType.SelectedItem = cboDatabaseType.Items[1];
        }

        private void Form1_Load(object sender, EventArgs e) {
            splitContainer.Enabled = false;
        }

        private void BtnLoad_Click(object sender, EventArgs e) {

            if(string.IsNullOrWhiteSpace(txtConnectionString.Text)) {
                MessageBox.Show(owner: this, text: "Please enter a connection string", caption: "Connection String Required", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }
            Tables.Clear();
            tvwTables.Nodes.Clear();
            txtCode.Clear();
            txtNamespace.Clear();
            txtPrefix.Clear();

            splitContainer.Enabled = false;

            if(cboDatabaseType.SelectedItem is DatabaseTypeItem item) {

                if(item.DatabaseType == DatabaseType.PostgreSql) {
                    _database = new PostgreSqlDatabase(name: "", connectionString: txtConnectionString.Text);
                }
                else if(item.DatabaseType == DatabaseType.SqlServer) {
                    _database = new SqlServerDatabase(name: "", connectionString: txtConnectionString.Text);
                }
                else {
                    throw new Exception($"Unknown database type. Value = '{item.DatabaseType}'");
                }
                Cursor.Current = Cursors.WaitCursor;
                LoadTables();
                splitContainer.Enabled = true;
                Cursor.Current = Cursor.Current;
            }
        }

        private IDatabase? _database = null;

        private List<DatabaseTable> Tables { get; set; } = new List<DatabaseTable>();

        private void LoadTables() {

            if(_database == null) {
                return;
            }

            txtNamespace.Text = !string.IsNullOrWhiteSpace(_database.Name) ? _database.Name : "MyProject";

            if(_database.DatabaseType == DatabaseType.PostgreSql) {
                Tables = PostgreSqlSchemaLoader.LoadTables(_database);
            }
            else if(_database.DatabaseType == DatabaseType.SqlServer) {
                Tables = SqlServerSchemaLoader.LoadTables(_database);
            }
            else {
                throw new Exception($"Unknown database type. Value = '{_database.DatabaseType}'");
            }

            tvwTables.Nodes.Clear();

            Dictionary<StringKey<ISchemaName>, SchemaNode> schemaLookup = new Dictionary<StringKey<ISchemaName>, SchemaNode>();

            bool includeSystemSchemas = chkIncludeSystemSchemas.Checked;

            foreach(DatabaseTable table in Tables) {

                if(!includeSystemSchemas && _database.DatabaseType == DatabaseType.PostgreSql) { //Skip system schemas

                    if(string.Compare(table.Schema.Value, "pg_catalog", ignoreCase: true) == 0 || string.Compare(table.Schema.Value, "information_schema", ignoreCase: true) == 0) {
                        continue;
                    }
                }

                if(!schemaLookup.TryGetValue(table.Schema, out SchemaNode? schemaNode)) {
                    schemaNode = new SchemaNode(table.Schema);
                    schemaLookup.Add(table.Schema, schemaNode);
                    tvwTables.Nodes.Add(schemaNode);
                }
                TableNode tableNode = new TableNode(table);

                foreach(DatabaseColumn column in table.Columns) {
                    tableNode.Nodes.Add(new ColumnNode(tableNode, column));
                }
                schemaNode.Nodes.Add(tableNode);
            }
            foreach(TreeNode node in tvwTables.Nodes) {
                node.Expand();
            }
        }

        private class DatabaseTypeItem {

            public DatabaseType DatabaseType { get; }

            public DatabaseTypeItem(DatabaseType databaseType) {
                DatabaseType = databaseType;
            }

            public override string ToString() {
                return DatabaseType.ToString();
            }
        }

        private class SchemaNode : TreeNode {

            public SchemaNode(StringKey<ISchemaName> schemaName) {
                Text = schemaName.Value;
            }
        }
        private class TableNode : TreeNode {

            public DatabaseTable Table { get; }

            public TableNode(DatabaseTable table) {
                Table = table;
                Text = table.TableName.Value + (table.IsView ? " (View)" : "");
            }
        }
        private class ColumnNode : TreeNode {

            public TableNode TableNode { get; }
            public DatabaseColumn Column { get; }

            public ColumnNode(TableNode tableNode, DatabaseColumn column) {
                TableNode = tableNode;
                Column = column;
                Text = $"{column.ColumnName} ({column.SqlDataTypeName} {(column.IsNullable ? "NULL" : "NOT NULL")})";
            }
        }

        private void TvwTables_AfterSelect(object sender, TreeViewEventArgs e) {
            UpdateCode(true);
        }

        private bool _ignoreChanges;

        private void UpdateCode(bool updatePrefix) {

            if(_database == null) {
                txtCode.Text = string.Empty;
                return;
            }
            _ignoreChanges = true;

            Cursor.Current = Cursors.WaitCursor;

            try {

                txtCode.Text = string.Empty;

                TableNode? tableNode = null;

                if(tvwTables.SelectedNode is TableNode node) {
                    tableNode = node;
                }
                if(tvwTables.SelectedNode is ColumnNode columnNode) {
                    tableNode = columnNode.TableNode;
                }

                if(tableNode != null) {

                    DatabaseTable table = tableNode.Table;

                    TablePrefix prefix;

                    if(updatePrefix) {
                        prefix = new TablePrefix(table);
                        txtPrefix.Text = prefix.Prefix;
                    }
                    else {
                        prefix = new TablePrefix(txtPrefix.Text);
                    }

                    string baseNamespace = txtNamespace.Text;

                    Namespaces namespaces = new Namespaces(baseNamespace: baseNamespace, tableNamespace: $"{baseNamespace}.Tables", classNamespace: $"{baseNamespace}.Classes");

                    CodeGeneratorSettings settings = new CodeGeneratorSettings() {
                        IncludeMessagePackAttributes = chkIncludeMessagePackAttributes.Checked,
                        IncludeJsonAttributes = chkIncludeJsonAttributes.Checked,
                        UseIdentifiers = chkUseIdentifiers.Checked,
                        IncludeDescriptions = chkIncludeDescriptions.Checked,
                        IncludeConstraints = chkIncludeConstraints.Checked,
                        NumberOfInstanceProperties = (int)numNumberOfInstanceProperties.Value,
                        Namespaces = namespaces
                    };

                    CodeBuilder code = TableCodeGenerator.Generate(table, prefix, settings, includeUsings: true, generateKeyInterface: true);

                    txtCode.Text = code.ToString();

                    txtCode.Text += Environment.NewLine;

                    CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(_database, table, prefix, settings);

                    txtCode.Text += Environment.NewLine + classCode.ToString();

                    if(!table.IsView) {

                        CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(_database, table, prefix, useIdentifiers: chkUseIdentifiers.Checked, namespaces);

                        txtCode.Text += Environment.NewLine + Environment.NewLine + validationCode.ToString();
                    }
                }
            }
            finally {
                _ignoreChanges = false;
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void BtnOutputAllToFile_Click(object sender, EventArgs e) {

            if(_database == null) {
                throw new NullReferenceException($"{nameof(_database)} cannot be null");
            }

            FolderBrowserDialog fd = new FolderBrowserDialog();

            if(fd.ShowDialog() == DialogResult.OK) {

                Cursor.Current = Cursors.WaitCursor;

                try {

                    string folder = fd.SelectedPath;

                    if(Directory.GetFiles(folder).Length > 0) {
                        MessageBox.Show(this, "Selected output folder must be empty", "Folder must be empty", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string baseNamespace = txtNamespace.Text;

                    Namespaces namespaces = new Namespaces(baseNamespace: baseNamespace, tableNamespace: $"{baseNamespace}.Tables", classNamespace: $"{baseNamespace}.Classes");

                    CodeGeneratorSettings settings = new CodeGeneratorSettings() {
                        IncludeMessagePackAttributes = chkIncludeMessagePackAttributes.Checked,
                        IncludeJsonAttributes = chkIncludeJsonAttributes.Checked,
                        UseIdentifiers = chkUseIdentifiers.Checked,
                        IncludeDescriptions = chkIncludeDescriptions.Checked,
                        IncludeConstraints = chkIncludeConstraints.Checked,
                        NumberOfInstanceProperties = (int)numNumberOfInstanceProperties.Value,
                        Namespaces = namespaces
                    };

                    OutputToFolder.Output(
                        tables: Tables,
                        namespaces: namespaces,
                        settings: settings,
                        folder: folder,
                        singleFiles: chkSingleFiles.Checked,
                        database: _database
                    );
                }
                finally {
                    Cursor.Current = Cursors.Arrow;
                }
            }
        }

        private void CheckBoxes_CheckedChanged(object sender, EventArgs e) {

            if(!_ignoreChanges) {
                UpdateCode(updatePrefix: false);
            }
        }

        private void CboDatabaseType_SelectedIndexChanged(object sender, EventArgs e) {

            if(cboDatabaseType.SelectedItem is DatabaseTypeItem item) {

                if(item.DatabaseType == DatabaseType.PostgreSql) {
                    txtExampleConnectionString.Text = "Server=127.0.0.1;Port=5432;Database=Northwind;User Id=postgres;Password=password;";
                }
                else if(item.DatabaseType == DatabaseType.SqlServer) {
                    txtExampleConnectionString.Text = "Server=localhost;Database=Northwind;Trusted_Connection=True;";
                }
                else {
                    txtExampleConnectionString.Text = "";
                }
            }
        }

        private void BtnExpandAll_Click(object sender, EventArgs e) {

            TreeNode? selectedNode = tvwTables.SelectedNode;

            foreach(TreeNode node in tvwTables.Nodes) {
                node.Expand();
            }
            if(selectedNode != null) {
                tvwTables.SelectedNode = selectedNode;
            }
        }

        private void BtnCollapseAll_Click(object sender, EventArgs e) {

            TreeNode? selectedNode = tvwTables.SelectedNode;

            tvwTables.CollapseAll();

            if(selectedNode != null) {
                tvwTables.SelectedNode = selectedNode;
            }
        }
    }
}