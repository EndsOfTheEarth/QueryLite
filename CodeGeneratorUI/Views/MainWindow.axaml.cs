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
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using DbSchema.CodeGeneration;
using Npgsql;
using QueryLite.CodeGeneratorUI.ViewModels;
using QueryLite.Databases.PostgreSql;
using QueryLite.Databases.SqlServer;
using QueryLite.DbSchema;
using QueryLite.DbSchema.CodeGeneration;
using QueryLite.DbSchema.Tables;
using QueryLite.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QueryLite.CodeGeneratorUI.Views {

    public partial class MainWindow : Window {

        private readonly static string SQL_SERVER = "Sql Server";
        private readonly static string POSTGRESQL = "PostgreSql";

        private IDatabase? _database = null;

        private List<DatabaseTable> Tables { get; set; } = new List<DatabaseTable>();

        public MainWindow() {

            InitializeComponent();
            Initialize();
        }

        private void Initialize() {

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            MinHeight = 500;
            MinWidth = 800;

            Height = 900;
            Width = 1400;

            Opened += MainWindow_Opened;
            cboDatabaseType.SelectionChanged += CboDatabaseType_SelectionChanged;

            cboDatabaseType.Items.Add(SQL_SERVER);
            cboDatabaseType.Items.Add(POSTGRESQL);

            cboDatabaseType.SelectedIndex = 1;

            btnLoad.Click += BtnLoad_Click;

            txtNamespace.TextChanged += (sender, e) => UpdateCode(updatePrefix: false);
            txtPrefix.TextChanged += (sender, e) => UpdateCode(updatePrefix: false);

            numNumberOfInstanceProperties.ValueChanged += (sender, e) => UpdateCode(updatePrefix: false);

            radioNone.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);
            radioIdentifierKeyType.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);
            radioIdentifierCustom.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);

            chkIncludeDescriptions.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);
            chkIncludeConstraints.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);
            chkIncludeMessagePackAttributes.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);
            chkIncludeJsonAttributes.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);
            chkUsePreparedQueries.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);

            btnExpandAll.Click += BtnExpandAll_Click;
            btnCollapseAll.Click += BtnCollapseAll_Click;
            btnOutputAllToFile.Click += BtnOutputAllToFile_Click;

            btnClose.Click += (sender, e) => Close();
        }

        private async void BtnLoad_Click(object? sender, RoutedEventArgs e) {

            Cursor = new Cursor(StandardCursorType.Wait);

            btnLoad.IsEnabled = false;

            try {
                await LoadAsync();
            }
            catch(Exception ex) {
                MessageDialog dialog = new MessageDialog();
                await dialog.ShowMessage(this, title: "Exception Occurred", message: ex.ToString());
            }
            finally {
                btnLoad.IsEnabled = true;
                Cursor = new Cursor(StandardCursorType.Arrow);
            }
        }

        private async Task LoadAsync() {

            if(string.IsNullOrWhiteSpace(txtConnectionString.Text)) {
                return;
            }

            MainWindowViewModel? viewModel = GetViewModel();

            viewModel!.Nodes.Clear();

            txtCode.Text = string.Empty;

            string item = cboDatabaseType.SelectedItem as string ?? string.Empty;

            if(string.Equals(item, POSTGRESQL, StringComparison.OrdinalIgnoreCase)) {

                NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(txtConnectionString.Text);

                _database = new PostgreSqlDatabase(name: builder.Database ?? string.Empty, connectionString: txtConnectionString.Text);
            }
            else if(string.Equals(item, SQL_SERVER, StringComparison.OrdinalIgnoreCase)) {

                Microsoft.Data.SqlClient.SqlConnectionStringBuilder builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(txtConnectionString.Text);

                _database = new SqlServerDatabase(name: builder.InitialCatalog, connectionString: txtConnectionString.Text);
            }
            else {
                throw new Exception($"Unknown database type. Value = '{item}'");
            }

            await LoadTablesAsync();

            if(Tables.Count == 0) {
                MessageDialog dialog = new MessageDialog();
                await dialog.ShowMessage(this, title: "No tables found", message: "No tables found");
                return;
            }
            LoadTablesNodes(_database.DatabaseType);
        }

        private async Task LoadTablesAsync() {

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

            if(Tables.Count == 0) {
                MessageDialog dialog = new MessageDialog();
                await dialog.ShowMessage(this, title: "No tables found", message: "No tables found");
                return;
            }
            LoadTablesNodes(_database.DatabaseType);
        }

        private void LoadTablesNodes(DatabaseType databaseType) {

            Dictionary<StringKey<ISchemaName>, SchemaNode> schemaLookup = new Dictionary<StringKey<ISchemaName>, SchemaNode>();

            bool includeSystemSchemas = chkIncludeSystemSchemas.IsChecked ?? false;

            Tables.Sort((t1, t2) => t1.TableName.Value.CompareTo(t2.TableName.Value));

            MainWindowViewModel? viewModel = GetViewModel();

            viewModel!.Nodes.Clear();

            foreach(DatabaseTable table in Tables) {

                if(!includeSystemSchemas && databaseType == DatabaseType.PostgreSql) { //Skip system schemas

                    if(string.Compare(table.Schema.Value, "pg_catalog", ignoreCase: true) == 0 || string.Compare(table.Schema.Value, "information_schema", ignoreCase: true) == 0) {
                        continue;
                    }
                }

                if(!schemaLookup.TryGetValue(table.Schema, out SchemaNode? schemaNode)) {
                    schemaNode = new SchemaNode(table.Schema);
                    schemaLookup.Add(table.Schema, schemaNode);
                    viewModel.Nodes.Add(schemaNode);
                }
                TableNode tableNode = new TableNode(table);

                foreach(DatabaseColumn column in table.Columns) {
                    tableNode.Nodes.Add(new ColumnNode(tableNode, column));
                }
                schemaNode.Nodes.Add(tableNode);
            }

            foreach(SchemaNode node in viewModel.Nodes) {

                TreeViewItem? treeViewItem = (TreeViewItem?)tvwSchema.TreeContainerFromItem(node);

                if(treeViewItem != null) {
                    treeViewItem.IsExpanded = true;
                }
            }
        }

        private void CboDatabaseType_SelectionChanged(object? sender, SelectionChangedEventArgs e) {

            string item = cboDatabaseType.SelectedItem?.ToString() ?? string.Empty;

            if(string.Equals(item, SQL_SERVER, StringComparison.OrdinalIgnoreCase)) {
                txtConnectionStringExample.Text = "Server=localhost;Database=Northwind;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
            }
            else {
                txtConnectionStringExample.Text = "Server=127.0.0.1;Port=5432;Database=Northwind;User Id=postgres;Password=password;";
            }
        }

        private void MainWindow_Opened(object? sender, EventArgs e) {

        }

        private MainWindowViewModel? GetViewModel() {
            MainWindowViewModel? viewModel = (MainWindowViewModel?)DataContext;
            return viewModel;
        }
        public void TreeNodeSelectionChanged(object sender, SelectionChangedEventArgs args) {

            Cursor = new Cursor(StandardCursorType.Wait);

            try {
                UpdateCode(updatePrefix: true);
            }
            finally {
                Cursor = new Cursor(StandardCursorType.Arrow);
            }
        }

        private bool _ignoreChanges;

        private void UpdateCode(bool updatePrefix) {

            if(_ignoreChanges) {
                return;
            }
            if(_database == null) {
                txtCode.Text = string.Empty;
                return;
            }
            _ignoreChanges = true;

            try {

                MainWindowViewModel? viewModel = GetViewModel();

                txtCode.Text = string.Empty;

                TableNode? tableNode = null;

                object? selectedNode = viewModel!.SelectedNodes.Count > 0 ? viewModel!.SelectedNodes[0] : null;

                if(selectedNode is TableNode node) {
                    tableNode = node;
                }
                if(selectedNode is ColumnNode columnNode) {
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
                        prefix = new TablePrefix(txtPrefix.Text ?? string.Empty);
                    }
                    string baseNamespace = txtNamespace.Text ?? string.Empty;

                    Namespaces namespaces = new Namespaces(baseNamespace: baseNamespace, tableNamespace: $"{baseNamespace}", classNamespace: $"{baseNamespace}", requestNamespace: $"{baseNamespace}", handlerNamespace: $"{baseNamespace}");

                    CodeGeneratorSettings settings = new CodeGeneratorSettings() {
                        IncludeMessagePackAttributes = chkIncludeMessagePackAttributes.IsChecked ?? false,
                        IncludeJsonAttributes = chkIncludeJsonAttributes.IsChecked ?? false,
                        UseIdentifiers = GetIdentifierType(),
                        IncludeDescriptions = chkIncludeDescriptions.IsChecked ?? false,
                        IncludeConstraints = chkIncludeConstraints.IsChecked ?? false,
                        NumberOfInstanceProperties = (int)(numNumberOfInstanceProperties.Value ?? 1),
                        UsePreparedQueries = chkUsePreparedQueries.IsChecked ?? false,
                        Namespaces = namespaces
                    };

                    CodeBuilder code = TableCodeGenerator.Generate(table, prefix, settings, includeUsings: true, generateKeyInterface: true);

                    txtCode.Text = code.ToString();

                    txtCode.Text += Environment.NewLine;

                    CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(_database, table, prefix, settings, includeUsings: true);

                    txtCode.Text += Environment.NewLine + classCode.ToString();

                    if(!table.IsView) {

                        CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(table, prefix, settings, includeUsings: true);

                        txtCode.Text += Environment.NewLine + Environment.NewLine + validationCode.ToString();
                    }

                    txtCode.Text += Environment.NewLine + Environment.NewLine + MediatorLoadSingleRecordRequestGenerator.GetLoadRequest(table, prefix, settings);
                    txtCode.Text += MediatorLoadSingleRecordRequestGenerator.GetLoadListHandlerCode(table, prefix, settings);

                    txtCode.Text += Environment.NewLine + Environment.NewLine + MediatorLoadListRequestGenerator.GetLoadListRequest(table);
                    txtCode.Text += MediatorLoadListRequestGenerator.GetLoadListHandlerCode(table, settings);

                    txtCode.Text += Environment.NewLine + Environment.NewLine + MediatorCreateRequestGenerator.GetCreateRequest(table);
                    txtCode.Text += MediatorCreateRequestGenerator.GetCreateHandlerCode(table, prefix, settings);

                    txtCode.Text += Environment.NewLine + Environment.NewLine + MediatorUpdateSingleRecordRequestGenerator.GetUpdateRequest(table, settings);
                    txtCode.Text += MediatorUpdateSingleRecordRequestGenerator.GetUpdateHandlerCode(table, prefix, settings);

                    txtCode.Text += Environment.NewLine + Environment.NewLine + MediatorDeleteSingleRecordRequestGenerator.GetDeleteRequest(table, prefix, settings);
                    txtCode.Text += MediatorDeleteSingleRecordRequestGenerator.GetDeleteHandlerCode(table, prefix, settings);
                }
            }
            finally {
                _ignoreChanges = false;
            }
        }
        public IdentifierType GetIdentifierType() {

            IdentifierType type = IdentifierType.None;

            if(radioIdentifierKeyType.IsChecked ?? false) {
                type = IdentifierType.Key;
            }
            else if(radioIdentifierCustom.IsChecked ?? false) {
                type = IdentifierType.Custom;
            }
            return type;
        }

        private void BtnExpandAll_Click(object? sender, RoutedEventArgs e) {

            MainWindowViewModel? viewModel = GetViewModel();

            foreach(Node node in viewModel!.Nodes) {
                SetExpandAll(node, expand: true);
            }
        }

        private void BtnCollapseAll_Click(object? sender, RoutedEventArgs e) {

            MainWindowViewModel? viewModel = GetViewModel();

            foreach(Node node in viewModel!.Nodes) {
                SetExpandAll(node, expand: false);
            }
        }

        private void SetExpandAll(Node parentNode, bool expand) {

            TreeViewItem? treeViewItem = (TreeViewItem?)tvwSchema.TreeContainerFromItem(parentNode);

            if(treeViewItem != null) {
                treeViewItem.IsExpanded = expand;
            }

            foreach(Node node in parentNode.Nodes) {

                TreeViewItem? subTreeViewItem = (TreeViewItem?)tvwSchema.TreeContainerFromItem(node);

                if(subTreeViewItem != null) {
                    subTreeViewItem.IsExpanded = expand;
                }
                SetExpandAll(node, expand: expand);
            }
        }

        private async void BtnOutputAllToFile_Click(object? sender, RoutedEventArgs e) {

            if(_database == null) {
                throw new NullReferenceException($"{nameof(_database)} cannot be null");
            }

            TopLevel? topLevel = GetTopLevel(this);

            IReadOnlyList<IStorageFolder> folders = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { AllowMultiple = false });

            if(folders.Count == 0) {
                return;
            }

            Cursor = new Cursor(StandardCursorType.Wait);

            try {

                string folder = folders[0].Path.AbsolutePath;

                if(Directory.GetFiles(folder).Length > 0) {
                    MessageDialog dialog = new MessageDialog();
                    await dialog.ShowMessage(this, title: "Folder must be empty", message: "Selected output folder must be empty");
                    return;
                }

                string baseNamespace = txtNamespace.Text ?? string.Empty;

                Namespaces namespaces = new Namespaces(baseNamespace: baseNamespace, tableNamespace: $"{baseNamespace}", classNamespace: $"{baseNamespace}", requestNamespace: $"{baseNamespace}", handlerNamespace: $"{baseNamespace}");

                CodeGeneratorSettings settings = new CodeGeneratorSettings() {
                    IncludeMessagePackAttributes = chkIncludeMessagePackAttributes.IsChecked ?? false,
                    IncludeJsonAttributes = chkIncludeJsonAttributes.IsChecked ?? false,
                    UseIdentifiers = GetIdentifierType(),
                    IncludeDescriptions = chkIncludeDescriptions.IsChecked ?? false,
                    IncludeConstraints = chkIncludeConstraints.IsChecked ?? false,
                    NumberOfInstanceProperties = (int)(numNumberOfInstanceProperties.Value ?? 1),
                    UsePreparedQueries = chkUsePreparedQueries.IsChecked ?? false,
                    Namespaces = namespaces
                };

                //
                //  Filter out system tables when needed
                //
                List<DatabaseTable> tables = new List<DatabaseTable>();

                bool includeSystemSchemas = chkIncludeSystemSchemas.IsChecked ?? false;

                foreach(DatabaseTable table in Tables) {

                    if(!includeSystemSchemas && _database.DatabaseType == DatabaseType.PostgreSql) { //Skip system schemas

                        if(string.Compare(table.Schema.Value, "pg_catalog", ignoreCase: true) == 0 || string.Compare(table.Schema.Value, "information_schema", ignoreCase: true) == 0) {
                            continue;
                        }
                    }
                    tables.Add(table);
                }
                OutputToFolder.Output(
                    tables: tables,
                    namespaces: namespaces,
                    settings: settings,
                    folder: folder,
                    singleFiles: chkSingleFiles.IsChecked ?? false,
                    database: _database
                );
            }
            finally {
                Cursor = new Cursor(StandardCursorType.Arrow);
            }
        }
    }
}