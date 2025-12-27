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

        private List<DatabaseTable> Tables { get; set; } = [];

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
            chkUseRepositoryPattern.IsCheckedChanged += (sender, e) => UpdateCode(updatePrefix: false);

            btnExpandAll.Click += BtnExpandAll_Click;
            btnCollapseAll.Click += BtnCollapseAll_Click;
            btnOutputAllToFile.Click += BtnOutputAllToFile_Click;

            btnClose.Click += (sender, e) => Close();
        }

        private async void BtnLoad_Click(object? sender, RoutedEventArgs e) {

            Cursor = new Cursor(StandardCursorType.Wait);

            btnLoad.IsEnabled = false;
            btnOutputAllToFile.IsEnabled = false;

            try {
                bool success = await LoadAsync();
                btnOutputAllToFile.IsEnabled = success;
            }
            catch(Exception ex) {

                MessageDialog dialog = new();

                await dialog.ShowMessage(
                    parent: this,
                    title: "Exception Occurred",
                    message: ex.ToString()
                );
            }
            finally {
                btnLoad.IsEnabled = true;
                Cursor = new Cursor(StandardCursorType.Arrow);
            }
        }

        private async Task<bool> LoadAsync() {

            if(string.IsNullOrWhiteSpace(txtConnectionString.Text)) {
                return false;
            }

            MainWindowViewModel? viewModel = GetViewModel();

            viewModel!.Nodes.Clear();

            txtCode.Text = "";

            string item = cboDatabaseType.SelectedItem as string ?? "";

            if(string.Equals(item, POSTGRESQL, StringComparison.OrdinalIgnoreCase)) {

                NpgsqlConnectionStringBuilder builder = new(txtConnectionString.Text);

                _database = new PostgreSqlDatabase(
                    name: builder.Database ?? "",
                    connectionString: txtConnectionString.Text
                );
            }
            else if(string.Equals(item, SQL_SERVER, StringComparison.OrdinalIgnoreCase)) {

                Microsoft.Data.SqlClient.SqlConnectionStringBuilder builder = new(txtConnectionString.Text);

                _database = new SqlServerDatabase(
                    name: builder.InitialCatalog,
                    connectionString: txtConnectionString.Text
                );
            }
            else {
                throw new Exception($"Unknown database type. Value = '{item}'");
            }

            await LoadTablesAsync();

            if(Tables.Count == 0) {

                MessageDialog dialog = new();

                await dialog.ShowMessage(
                    parent: this,
                    title: "No tables found",
                    message: "No tables found"
                );
                return false;
            }
            LoadTablesNodes(_database.DatabaseType);
            return true;
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

                MessageDialog dialog = new();

                await dialog.ShowMessage(
                    parent: this,
                    title: "No tables found",
                    message: "No tables found"
                );
                return;
            }
            LoadTablesNodes(_database.DatabaseType);
        }

        private void LoadTablesNodes(DatabaseType databaseType) {

            Dictionary<StringKey<ISchemaName>, SchemaNode> schemaLookup = [];

            bool includeSystemSchemas = chkIncludeSystemSchemas.IsChecked ?? false;

            Tables.Sort((t1, t2) => t1.TableName.Value.CompareTo(t2.TableName.Value));

            MainWindowViewModel? viewModel = GetViewModel();

            viewModel!.Nodes.Clear();

            foreach(DatabaseTable table in Tables) {

                if(!includeSystemSchemas && databaseType == DatabaseType.PostgreSql) { //Skip system schemas

                    bool skip = string.Equals(table.Schema.Value, "pg_catalog", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(table.Schema.Value, "information_schema", StringComparison.OrdinalIgnoreCase);

                    if(skip) {
                        continue;
                    }
                }

                if(!schemaLookup.TryGetValue(table.Schema, out SchemaNode? schemaNode)) {
                    schemaNode = new SchemaNode(table.Schema);
                    schemaLookup.Add(table.Schema, schemaNode);
                    viewModel.Nodes.Add(schemaNode);
                }
                TableNode tableNode = new(table);

                foreach(DatabaseColumn column in table.Columns) {
                    tableNode.Nodes.Add(new ColumnNode(tableNode, column));
                }
                schemaNode.Nodes.Add(tableNode);
            }

            foreach(TreeNodeViewModel node in viewModel.Nodes) {

                TreeViewItem? treeViewItem = (TreeViewItem?)tvwSchema.TreeContainerFromItem(node);

                treeViewItem?.IsExpanded = true;
            }
        }

        private void CboDatabaseType_SelectionChanged(object? sender, SelectionChangedEventArgs e) {

            string item = cboDatabaseType.SelectedItem?.ToString() ?? "";

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
                txtCode.Text = "";
                return;
            }
            _ignoreChanges = true;

            try {

                MainWindowViewModel? viewModel = GetViewModel();

                txtCode.Text = "";

                TableNode? tableNode = null;

                object? selectedNode = viewModel!.SelectedNodes.Count > 0 ? viewModel!.SelectedNodes[0] : null;

                if(selectedNode is TableNode node) {
                    tableNode = node;
                }
                if(selectedNode is ColumnNode columnNode) {
                    tableNode = columnNode.TableNode;
                }

                if(tableNode == null) {
                    return;
                }

                DatabaseTable table = tableNode.Table;

                TablePrefix prefix;

                if(updatePrefix) {
                    prefix = new TablePrefix(table);
                    txtPrefix.Text = prefix.Prefix;
                }
                else {
                    prefix = new TablePrefix(txtPrefix.Text ?? "");
                }
                string baseNamespace = txtNamespace.Text ?? "";

                Namespaces namespaces = new(
                    baseNamespace: baseNamespace, tableNamespace: $"{baseNamespace}",
                    classNamespace: $"{baseNamespace}", requestNamespace: $"{baseNamespace}",
                    handlerNamespace: $"{baseNamespace}"
                );

                CodeGeneratorSettings settings = new() {
                    IncludeMessagePackAttributes = chkIncludeMessagePackAttributes.IsChecked ?? false,
                    IncludeJsonAttributes = chkIncludeJsonAttributes.IsChecked ?? false,
                    UseIdentifiers = GetIdentifierType(),
                    IncludeDescriptions = chkIncludeDescriptions.IsChecked ?? false,
                    IncludeConstraints = chkIncludeConstraints.IsChecked ?? false,
                    NumberOfInstanceProperties = (int)(numNumberOfInstanceProperties.Value ?? 1),
                    UsePreparedQueries = chkUsePreparedQueries.IsChecked ?? false,
                    UseRepositoryPattern = chkUseRepositoryPattern.IsChecked ?? false,
                    Namespaces = namespaces
                };

                CodeBuilder code = TableCodeGenerator.Generate(
                    table: table,
                    prefix: prefix,
                    settings: settings,
                    includeUsings: true,
                    generateKeyInterface: true
                );

                string nl = Environment.NewLine;

                txtCode.Text = code.ToString();

                txtCode.Text += nl;

                CodeBuilder classCode = ClassCodeGenerator.GenerateClassCode(
                    database: _database,
                    table: table,
                    prefix: prefix,
                    settings: settings,
                    includeUsings: true
                );

                txtCode.Text += nl + classCode.ToString();

                if(!table.IsView) {

                    CodeBuilder validationCode = FluentValidationGenerator.GenerateFluentValidationCode(
                        table: table,
                        prefix: prefix,
                        settings: settings,
                        includeUsings: true
                    );
                    txtCode.Text += nl + nl + validationCode.ToString();
                }

                txtCode.Text += nl + nl + MediatorLoadSingleRecordRequestGenerator.GetLoadRequest(table, prefix, settings);
                txtCode.Text += MediatorLoadSingleRecordRequestGenerator.GetLoadListHandlerCode(table, prefix, settings);

                txtCode.Text += nl + nl + MediatorLoadListRequestGenerator.GetLoadListRequest(table);
                txtCode.Text += MediatorLoadListRequestGenerator.GetLoadListHandlerCode(table, settings);

                txtCode.Text += nl + nl + MediatorCreateRequestGenerator.GetCreateRequest(table);
                txtCode.Text += MediatorCreateRequestGenerator.GetCreateHandlerCode(table, prefix, settings);

                txtCode.Text += nl + nl + MediatorUpdateSingleRecordRequestGenerator.GetUpdateRequest(table, settings);
                txtCode.Text += MediatorUpdateSingleRecordRequestGenerator.GetUpdateHandlerCode(table, prefix, settings);

                txtCode.Text += nl + nl + MediatorDeleteSingleRecordRequestGenerator.GetDeleteRequest(table, prefix, settings);
                txtCode.Text += MediatorDeleteSingleRecordRequestGenerator.GetDeleteHandlerCode(table, prefix, settings);

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

            foreach(TreeNodeViewModel node in viewModel!.Nodes) {
                SetExpandAll(node, expand: true);
            }
        }

        private void BtnCollapseAll_Click(object? sender, RoutedEventArgs e) {

            MainWindowViewModel? viewModel = GetViewModel();

            foreach(TreeNodeViewModel node in viewModel!.Nodes) {
                SetExpandAll(node, expand: false);
            }
        }

        private void SetExpandAll(TreeNodeViewModel parentNode, bool expand) {

            TreeViewItem? treeViewItem = (TreeViewItem?)tvwSchema.TreeContainerFromItem(parentNode);

            treeViewItem?.IsExpanded = expand;

            foreach(TreeNodeViewModel node in parentNode.Nodes) {

                TreeViewItem? subTreeViewItem = (TreeViewItem?)tvwSchema.TreeContainerFromItem(node);

                subTreeViewItem?.IsExpanded = expand;

                SetExpandAll(node, expand: expand);
            }
        }

        private async void BtnOutputAllToFile_Click(object? sender, RoutedEventArgs e) {

            if(_database == null) {

                MessageDialog dialog = new MessageDialog();

                await dialog.ShowMessage(
                    parent: this,
                    title: "Database not connected",
                    message: $"{nameof(_database)} cannot be null"
                );
                return;
            }

            TopLevel? topLevel = GetTopLevel(this);

            IReadOnlyList<IStorageFolder> folders = await topLevel!.StorageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions { AllowMultiple = false }
            );

            if(folders.Count == 0) {
                return;
            }

            Cursor = new Cursor(StandardCursorType.Wait);

            try {

                string folder = folders[0].Path.AbsolutePath;

                if(Directory.GetFiles(folder).Length > 0) {

                    MessageDialog dialog = new MessageDialog();

                    await dialog.ShowMessage(
                        parent: this,
                        title: "Folder must be empty",
                        message: "Selected output folder must be empty"
                    );
                    return;
                }

                string baseNamespace = txtNamespace.Text ?? "";

                Namespaces namespaces = new(
                    baseNamespace: baseNamespace,
                    tableNamespace: $"{baseNamespace}",
                    classNamespace: $"{baseNamespace}",
                    requestNamespace: $"{baseNamespace}",
                    handlerNamespace: $"{baseNamespace}"
                );

                CodeGeneratorSettings settings = new() {
                    IncludeMessagePackAttributes = chkIncludeMessagePackAttributes.IsChecked ?? false,
                    IncludeJsonAttributes = chkIncludeJsonAttributes.IsChecked ?? false,
                    UseIdentifiers = GetIdentifierType(),
                    IncludeDescriptions = chkIncludeDescriptions.IsChecked ?? false,
                    IncludeConstraints = chkIncludeConstraints.IsChecked ?? false,
                    NumberOfInstanceProperties = (int)(numNumberOfInstanceProperties.Value ?? 1),
                    UsePreparedQueries = chkUsePreparedQueries.IsChecked ?? false,
                    UseRepositoryPattern = chkUseRepositoryPattern.IsChecked ?? false,
                    Namespaces = namespaces
                };

                //
                //  Filter out system tables when needed
                //
                List<DatabaseTable> tables = [];

                bool includeSystemSchemas = chkIncludeSystemSchemas.IsChecked ?? false;

                foreach(DatabaseTable table in Tables) {

                    if(!includeSystemSchemas && _database.DatabaseType == DatabaseType.PostgreSql) { //Skip system schemas

                        bool skip = string.Equals(table.Schema.Value, "pg_catalog", StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(table.Schema.Value, "information_schema", StringComparison.OrdinalIgnoreCase);

                        if(skip) {
                            continue;
                        }
                    }
                    tables.Add(table);
                }
                OutputToFolder.Output(
                    tables: tables,
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