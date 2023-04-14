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
namespace QueryLite.CodeGeneratorUI {

    partial class MainForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            splitContainer = new System.Windows.Forms.SplitContainer();
            tvwTables = new System.Windows.Forms.TreeView();
            numNumberOfInstanceProperties = new System.Windows.Forms.NumericUpDown();
            lblNumberOfInstanceProperties = new System.Windows.Forms.Label();
            chkIncludeJsonAttributes = new System.Windows.Forms.CheckBox();
            chkIncludeConstraints = new System.Windows.Forms.CheckBox();
            chkIncludeDescriptions = new System.Windows.Forms.CheckBox();
            chkUseIdentifiers = new System.Windows.Forms.CheckBox();
            chkSingleFiles = new System.Windows.Forms.CheckBox();
            btnOutputAllToFile = new System.Windows.Forms.Button();
            txtNamespace = new System.Windows.Forms.TextBox();
            lblNamespace = new System.Windows.Forms.Label();
            chkIncludeMessagePackAttributes = new System.Windows.Forms.CheckBox();
            txtPrefix = new System.Windows.Forms.TextBox();
            lblPrefix = new System.Windows.Forms.Label();
            btnClose = new System.Windows.Forms.Button();
            txtCode = new System.Windows.Forms.TextBox();
            chkIncludeSystemSchemas = new System.Windows.Forms.CheckBox();
            lblDatabaseType = new System.Windows.Forms.Label();
            cboDatabaseType = new System.Windows.Forms.ComboBox();
            lblConnectionString = new System.Windows.Forms.Label();
            txtConnectionString = new System.Windows.Forms.TextBox();
            btnLoad = new System.Windows.Forms.Button();
            grpConnection = new System.Windows.Forms.GroupBox();
            lblExample = new System.Windows.Forms.Label();
            txtExampleConnectionString = new System.Windows.Forms.TextBox();
            BtnExpandAll = new System.Windows.Forms.Button();
            BtnCollapseAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numNumberOfInstanceProperties).BeginInit();
            grpConnection.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer.Location = new System.Drawing.Point(0, 153);
            splitContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(BtnCollapseAll);
            splitContainer.Panel1.Controls.Add(BtnExpandAll);
            splitContainer.Panel1.Controls.Add(tvwTables);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(numNumberOfInstanceProperties);
            splitContainer.Panel2.Controls.Add(lblNumberOfInstanceProperties);
            splitContainer.Panel2.Controls.Add(chkIncludeJsonAttributes);
            splitContainer.Panel2.Controls.Add(chkIncludeConstraints);
            splitContainer.Panel2.Controls.Add(chkIncludeDescriptions);
            splitContainer.Panel2.Controls.Add(chkUseIdentifiers);
            splitContainer.Panel2.Controls.Add(chkSingleFiles);
            splitContainer.Panel2.Controls.Add(btnOutputAllToFile);
            splitContainer.Panel2.Controls.Add(txtNamespace);
            splitContainer.Panel2.Controls.Add(lblNamespace);
            splitContainer.Panel2.Controls.Add(chkIncludeMessagePackAttributes);
            splitContainer.Panel2.Controls.Add(txtPrefix);
            splitContainer.Panel2.Controls.Add(lblPrefix);
            splitContainer.Panel2.Controls.Add(btnClose);
            splitContainer.Panel2.Controls.Add(txtCode);
            splitContainer.Size = new System.Drawing.Size(1623, 912);
            splitContainer.SplitterDistance = 317;
            splitContainer.SplitterWidth = 6;
            splitContainer.TabIndex = 1;
            // 
            // tvwTables
            // 
            tvwTables.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tvwTables.HideSelection = false;
            tvwTables.Location = new System.Drawing.Point(0, 0);
            tvwTables.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tvwTables.Name = "tvwTables";
            tvwTables.Size = new System.Drawing.Size(314, 862);
            tvwTables.TabIndex = 0;
            tvwTables.AfterSelect += TvwTables_AfterSelect;
            // 
            // numNumberOfInstanceProperties
            // 
            numNumberOfInstanceProperties.Location = new System.Drawing.Point(737, 8);
            numNumberOfInstanceProperties.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numNumberOfInstanceProperties.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numNumberOfInstanceProperties.Name = "numNumberOfInstanceProperties";
            numNumberOfInstanceProperties.Size = new System.Drawing.Size(54, 31);
            numNumberOfInstanceProperties.TabIndex = 5;
            numNumberOfInstanceProperties.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numNumberOfInstanceProperties.ValueChanged += CheckBoxes_CheckedChanged;
            // 
            // lblNumberOfInstanceProperties
            // 
            lblNumberOfInstanceProperties.AutoSize = true;
            lblNumberOfInstanceProperties.Location = new System.Drawing.Point(527, 13);
            lblNumberOfInstanceProperties.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNumberOfInstanceProperties.Name = "lblNumberOfInstanceProperties";
            lblNumberOfInstanceProperties.Size = new System.Drawing.Size(203, 25);
            lblNumberOfInstanceProperties.TabIndex = 4;
            lblNumberOfInstanceProperties.Text = "# Of Instance Properties";
            // 
            // chkIncludeJsonAttributes
            // 
            chkIncludeJsonAttributes.AutoSize = true;
            chkIncludeJsonAttributes.Location = new System.Drawing.Point(889, 53);
            chkIncludeJsonAttributes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkIncludeJsonAttributes.Name = "chkIncludeJsonAttributes";
            chkIncludeJsonAttributes.Size = new System.Drawing.Size(218, 29);
            chkIncludeJsonAttributes.TabIndex = 10;
            chkIncludeJsonAttributes.Text = "Include Json Attributes";
            chkIncludeJsonAttributes.UseVisualStyleBackColor = true;
            chkIncludeJsonAttributes.CheckedChanged += CheckBoxes_CheckedChanged;
            // 
            // chkIncludeConstraints
            // 
            chkIncludeConstraints.AutoSize = true;
            chkIncludeConstraints.Checked = true;
            chkIncludeConstraints.CheckState = System.Windows.Forms.CheckState.Checked;
            chkIncludeConstraints.Location = new System.Drawing.Point(379, 53);
            chkIncludeConstraints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkIncludeConstraints.Name = "chkIncludeConstraints";
            chkIncludeConstraints.Size = new System.Drawing.Size(189, 29);
            chkIncludeConstraints.TabIndex = 8;
            chkIncludeConstraints.Text = "Include Constraints";
            chkIncludeConstraints.UseVisualStyleBackColor = true;
            chkIncludeConstraints.CheckedChanged += CheckBoxes_CheckedChanged;
            // 
            // chkIncludeDescriptions
            // 
            chkIncludeDescriptions.AutoSize = true;
            chkIncludeDescriptions.Location = new System.Drawing.Point(174, 53);
            chkIncludeDescriptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkIncludeDescriptions.Name = "chkIncludeDescriptions";
            chkIncludeDescriptions.Size = new System.Drawing.Size(198, 29);
            chkIncludeDescriptions.TabIndex = 7;
            chkIncludeDescriptions.Text = "Include Descriptions";
            chkIncludeDescriptions.UseVisualStyleBackColor = true;
            chkIncludeDescriptions.CheckedChanged += CheckBoxes_CheckedChanged;
            // 
            // chkUseIdentifiers
            // 
            chkUseIdentifiers.AutoSize = true;
            chkUseIdentifiers.Checked = true;
            chkUseIdentifiers.CheckState = System.Windows.Forms.CheckState.Checked;
            chkUseIdentifiers.Location = new System.Drawing.Point(6, 53);
            chkUseIdentifiers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkUseIdentifiers.Name = "chkUseIdentifiers";
            chkUseIdentifiers.Size = new System.Drawing.Size(150, 29);
            chkUseIdentifiers.TabIndex = 6;
            chkUseIdentifiers.Text = "Use Identifiers";
            chkUseIdentifiers.UseVisualStyleBackColor = true;
            chkUseIdentifiers.CheckedChanged += CheckBoxes_CheckedChanged;
            // 
            // chkSingleFiles
            // 
            chkSingleFiles.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            chkSingleFiles.AutoSize = true;
            chkSingleFiles.Location = new System.Drawing.Point(190, 871);
            chkSingleFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkSingleFiles.Name = "chkSingleFiles";
            chkSingleFiles.Size = new System.Drawing.Size(125, 29);
            chkSingleFiles.TabIndex = 13;
            chkSingleFiles.Text = "Single Files";
            chkSingleFiles.UseVisualStyleBackColor = true;
            // 
            // btnOutputAllToFile
            // 
            btnOutputAllToFile.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnOutputAllToFile.Location = new System.Drawing.Point(4, 868);
            btnOutputAllToFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnOutputAllToFile.Name = "btnOutputAllToFile";
            btnOutputAllToFile.Size = new System.Drawing.Size(176, 38);
            btnOutputAllToFile.TabIndex = 12;
            btnOutputAllToFile.Text = "Output All To File";
            btnOutputAllToFile.UseVisualStyleBackColor = true;
            btnOutputAllToFile.Click += BtnOutputAllToFile_Click;
            // 
            // txtNamespace
            // 
            txtNamespace.Location = new System.Drawing.Point(111, 7);
            txtNamespace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new System.Drawing.Size(248, 31);
            txtNamespace.TabIndex = 1;
            txtNamespace.Text = "MyProject";
            txtNamespace.TextChanged += CheckBoxes_CheckedChanged;
            // 
            // lblNamespace
            // 
            lblNamespace.AutoSize = true;
            lblNamespace.Location = new System.Drawing.Point(4, 15);
            lblNamespace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNamespace.Name = "lblNamespace";
            lblNamespace.Size = new System.Drawing.Size(104, 25);
            lblNamespace.TabIndex = 0;
            lblNamespace.Text = "Namespace";
            // 
            // chkIncludeMessagePackAttributes
            // 
            chkIncludeMessagePackAttributes.AutoSize = true;
            chkIncludeMessagePackAttributes.Location = new System.Drawing.Point(586, 53);
            chkIncludeMessagePackAttributes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkIncludeMessagePackAttributes.Name = "chkIncludeMessagePackAttributes";
            chkIncludeMessagePackAttributes.Size = new System.Drawing.Size(293, 29);
            chkIncludeMessagePackAttributes.TabIndex = 9;
            chkIncludeMessagePackAttributes.Text = "Include Message Pack Attributes";
            chkIncludeMessagePackAttributes.UseVisualStyleBackColor = true;
            chkIncludeMessagePackAttributes.CheckedChanged += CheckBoxes_CheckedChanged;
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new System.Drawing.Point(431, 7);
            txtPrefix.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new System.Drawing.Size(73, 31);
            txtPrefix.TabIndex = 3;
            txtPrefix.TextChanged += CheckBoxes_CheckedChanged;
            // 
            // lblPrefix
            // 
            lblPrefix.AutoSize = true;
            lblPrefix.Location = new System.Drawing.Point(370, 15);
            lblPrefix.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPrefix.Name = "lblPrefix";
            lblPrefix.Size = new System.Drawing.Size(55, 25);
            lblPrefix.TabIndex = 2;
            lblPrefix.Text = "Prefix";
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(1181, 867);
            btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(107, 38);
            btnClose.TabIndex = 14;
            btnClose.Text = "&Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;
            // 
            // txtCode
            // 
            txtCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtCode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtCode.Location = new System.Drawing.Point(4, 93);
            txtCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtCode.Multiline = true;
            txtCode.Name = "txtCode";
            txtCode.ReadOnly = true;
            txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtCode.Size = new System.Drawing.Size(1283, 769);
            txtCode.TabIndex = 11;
            txtCode.WordWrap = false;
            // 
            // chkIncludeSystemSchemas
            // 
            chkIncludeSystemSchemas.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            chkIncludeSystemSchemas.AutoSize = true;
            chkIncludeSystemSchemas.Location = new System.Drawing.Point(1373, 87);
            chkIncludeSystemSchemas.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkIncludeSystemSchemas.Name = "chkIncludeSystemSchemas";
            chkIncludeSystemSchemas.Size = new System.Drawing.Size(232, 29);
            chkIncludeSystemSchemas.TabIndex = 7;
            chkIncludeSystemSchemas.Text = "Include System Schemas";
            chkIncludeSystemSchemas.UseVisualStyleBackColor = true;
            // 
            // lblDatabaseType
            // 
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Location = new System.Drawing.Point(6, 87);
            lblDatabaseType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.Size = new System.Drawing.Size(128, 25);
            lblDatabaseType.TabIndex = 2;
            lblDatabaseType.Text = "Database Type";
            // 
            // cboDatabaseType
            // 
            cboDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDatabaseType.FormattingEnabled = true;
            cboDatabaseType.Location = new System.Drawing.Point(160, 80);
            cboDatabaseType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            cboDatabaseType.Name = "cboDatabaseType";
            cboDatabaseType.Size = new System.Drawing.Size(135, 33);
            cboDatabaseType.TabIndex = 3;
            cboDatabaseType.SelectedIndexChanged += CboDatabaseType_SelectedIndexChanged;
            // 
            // lblConnectionString
            // 
            lblConnectionString.AutoSize = true;
            lblConnectionString.Location = new System.Drawing.Point(6, 40);
            lblConnectionString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblConnectionString.Name = "lblConnectionString";
            lblConnectionString.Size = new System.Drawing.Size(153, 25);
            lblConnectionString.TabIndex = 0;
            lblConnectionString.Text = "Connection String";
            // 
            // txtConnectionString
            // 
            txtConnectionString.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtConnectionString.Location = new System.Drawing.Point(160, 33);
            txtConnectionString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtConnectionString.Name = "txtConnectionString";
            txtConnectionString.Size = new System.Drawing.Size(1447, 31);
            txtConnectionString.TabIndex = 1;
            // 
            // btnLoad
            // 
            btnLoad.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnLoad.Location = new System.Drawing.Point(303, 73);
            btnLoad.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new System.Drawing.Size(137, 47);
            btnLoad.TabIndex = 4;
            btnLoad.Text = "&Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += BtnLoad_Click;
            // 
            // grpConnection
            // 
            grpConnection.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpConnection.Controls.Add(chkIncludeSystemSchemas);
            grpConnection.Controls.Add(lblExample);
            grpConnection.Controls.Add(txtExampleConnectionString);
            grpConnection.Controls.Add(lblConnectionString);
            grpConnection.Controls.Add(btnLoad);
            grpConnection.Controls.Add(lblDatabaseType);
            grpConnection.Controls.Add(txtConnectionString);
            grpConnection.Controls.Add(cboDatabaseType);
            grpConnection.Location = new System.Drawing.Point(6, 13);
            grpConnection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpConnection.Name = "grpConnection";
            grpConnection.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpConnection.Size = new System.Drawing.Size(1614, 133);
            grpConnection.TabIndex = 0;
            grpConnection.TabStop = false;
            grpConnection.Text = "Database Connection";
            // 
            // lblExample
            // 
            lblExample.AutoSize = true;
            lblExample.Location = new System.Drawing.Point(469, 87);
            lblExample.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblExample.Name = "lblExample";
            lblExample.Size = new System.Drawing.Size(78, 25);
            lblExample.TabIndex = 5;
            lblExample.Text = "Example";
            // 
            // txtExampleConnectionString
            // 
            txtExampleConnectionString.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtExampleConnectionString.Location = new System.Drawing.Point(549, 80);
            txtExampleConnectionString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtExampleConnectionString.Name = "txtExampleConnectionString";
            txtExampleConnectionString.ReadOnly = true;
            txtExampleConnectionString.Size = new System.Drawing.Size(821, 31);
            txtExampleConnectionString.TabIndex = 6;
            // 
            // BtnExpandAll
            // 
            BtnExpandAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnExpandAll.Location = new System.Drawing.Point(6, 869);
            BtnExpandAll.Name = "BtnExpandAll";
            BtnExpandAll.Size = new System.Drawing.Size(112, 38);
            BtnExpandAll.TabIndex = 1;
            BtnExpandAll.Text = "Expand All";
            BtnExpandAll.UseVisualStyleBackColor = true;
            BtnExpandAll.Click += BtnExpandAll_Click;
            // 
            // BtnCollapseAll
            // 
            BtnCollapseAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnCollapseAll.Location = new System.Drawing.Point(122, 870);
            BtnCollapseAll.Name = "BtnCollapseAll";
            BtnCollapseAll.Size = new System.Drawing.Size(112, 38);
            BtnCollapseAll.TabIndex = 2;
            BtnCollapseAll.Text = "Collapse All";
            BtnCollapseAll.UseVisualStyleBackColor = true;
            BtnCollapseAll.Click += BtnCollapseAll_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1623, 1067);
            Controls.Add(grpConnection);
            Controls.Add(splitContainer);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "MainForm";
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            Text = "Query Lite Code Generator";
            Load += Form1_Load;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numNumberOfInstanceProperties).EndInit();
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView tvwTables;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.Button btnOutputAllToFile;
        private System.Windows.Forms.CheckBox chkIncludeMessagePackAttributes;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.CheckBox chkSingleFiles;
        private System.Windows.Forms.CheckBox chkUseIdentifiers;
        private System.Windows.Forms.CheckBox chkIncludeDescriptions;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cboDatabaseType;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblExample;
        private System.Windows.Forms.TextBox txtExampleConnectionString;
        private System.Windows.Forms.CheckBox chkIncludeConstraints;
        private System.Windows.Forms.CheckBox chkIncludeJsonAttributes;
        private System.Windows.Forms.NumericUpDown numNumberOfInstanceProperties;
        private System.Windows.Forms.Label lblNumberOfInstanceProperties;
        private System.Windows.Forms.CheckBox chkIncludeSystemSchemas;
        private System.Windows.Forms.Button BtnCollapseAll;
        private System.Windows.Forms.Button BtnExpandAll;
    }
}