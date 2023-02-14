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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tvwTables = new System.Windows.Forms.TreeView();
            this.chkIncludeJsonAttributes = new System.Windows.Forms.CheckBox();
            this.chkIncludeConstraints = new System.Windows.Forms.CheckBox();
            this.chkIncludeDescriptions = new System.Windows.Forms.CheckBox();
            this.chkUseIdentifiers = new System.Windows.Forms.CheckBox();
            this.chkSingleFiles = new System.Windows.Forms.CheckBox();
            this.btnOutputAllToFile = new System.Windows.Forms.Button();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.lblNamespace = new System.Windows.Forms.Label();
            this.chkIncludeMessagePackAttributes = new System.Windows.Forms.CheckBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblDatabaseType = new System.Windows.Forms.Label();
            this.cboDatabaseType = new System.Windows.Forms.ComboBox();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblExample = new System.Windows.Forms.Label();
            this.txtExampleConnectionString = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.grpConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 153);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tvwTables);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.chkIncludeJsonAttributes);
            this.splitContainer.Panel2.Controls.Add(this.chkIncludeConstraints);
            this.splitContainer.Panel2.Controls.Add(this.chkIncludeDescriptions);
            this.splitContainer.Panel2.Controls.Add(this.chkUseIdentifiers);
            this.splitContainer.Panel2.Controls.Add(this.chkSingleFiles);
            this.splitContainer.Panel2.Controls.Add(this.btnOutputAllToFile);
            this.splitContainer.Panel2.Controls.Add(this.txtNamespace);
            this.splitContainer.Panel2.Controls.Add(this.lblNamespace);
            this.splitContainer.Panel2.Controls.Add(this.chkIncludeMessagePackAttributes);
            this.splitContainer.Panel2.Controls.Add(this.txtPrefix);
            this.splitContainer.Panel2.Controls.Add(this.lblPrefix);
            this.splitContainer.Panel2.Controls.Add(this.btnClose);
            this.splitContainer.Panel2.Controls.Add(this.txtCode);
            this.splitContainer.Size = new System.Drawing.Size(1677, 907);
            this.splitContainer.SplitterDistance = 328;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 0;
            // 
            // tvwTables
            // 
            this.tvwTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwTables.HideSelection = false;
            this.tvwTables.Location = new System.Drawing.Point(0, 0);
            this.tvwTables.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tvwTables.Name = "tvwTables";
            this.tvwTables.Size = new System.Drawing.Size(326, 857);
            this.tvwTables.TabIndex = 0;
            this.tvwTables.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvwTables_AfterSelect);
            // 
            // chkIncludeJsonAttributes
            // 
            this.chkIncludeJsonAttributes.AutoSize = true;
            this.chkIncludeJsonAttributes.Location = new System.Drawing.Point(889, 53);
            this.chkIncludeJsonAttributes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIncludeJsonAttributes.Name = "chkIncludeJsonAttributes";
            this.chkIncludeJsonAttributes.Size = new System.Drawing.Size(218, 29);
            this.chkIncludeJsonAttributes.TabIndex = 8;
            this.chkIncludeJsonAttributes.Text = "Include Json Attributes";
            this.chkIncludeJsonAttributes.UseVisualStyleBackColor = true;
            this.chkIncludeJsonAttributes.CheckedChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // chkIncludeConstraints
            // 
            this.chkIncludeConstraints.AutoSize = true;
            this.chkIncludeConstraints.Checked = true;
            this.chkIncludeConstraints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncludeConstraints.Location = new System.Drawing.Point(379, 53);
            this.chkIncludeConstraints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIncludeConstraints.Name = "chkIncludeConstraints";
            this.chkIncludeConstraints.Size = new System.Drawing.Size(189, 29);
            this.chkIncludeConstraints.TabIndex = 6;
            this.chkIncludeConstraints.Text = "Include Constraints";
            this.chkIncludeConstraints.UseVisualStyleBackColor = true;
            this.chkIncludeConstraints.CheckedChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // chkIncludeDescriptions
            // 
            this.chkIncludeDescriptions.AutoSize = true;
            this.chkIncludeDescriptions.Location = new System.Drawing.Point(174, 53);
            this.chkIncludeDescriptions.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIncludeDescriptions.Name = "chkIncludeDescriptions";
            this.chkIncludeDescriptions.Size = new System.Drawing.Size(198, 29);
            this.chkIncludeDescriptions.TabIndex = 5;
            this.chkIncludeDescriptions.Text = "Include Descriptions";
            this.chkIncludeDescriptions.UseVisualStyleBackColor = true;
            this.chkIncludeDescriptions.CheckedChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // chkUseIdentifiers
            // 
            this.chkUseIdentifiers.AutoSize = true;
            this.chkUseIdentifiers.Checked = true;
            this.chkUseIdentifiers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseIdentifiers.Location = new System.Drawing.Point(6, 53);
            this.chkUseIdentifiers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkUseIdentifiers.Name = "chkUseIdentifiers";
            this.chkUseIdentifiers.Size = new System.Drawing.Size(150, 29);
            this.chkUseIdentifiers.TabIndex = 4;
            this.chkUseIdentifiers.Text = "Use Identifiers";
            this.chkUseIdentifiers.UseVisualStyleBackColor = true;
            this.chkUseIdentifiers.CheckedChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // chkSingleFiles
            // 
            this.chkSingleFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSingleFiles.AutoSize = true;
            this.chkSingleFiles.Location = new System.Drawing.Point(190, 867);
            this.chkSingleFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkSingleFiles.Name = "chkSingleFiles";
            this.chkSingleFiles.Size = new System.Drawing.Size(125, 29);
            this.chkSingleFiles.TabIndex = 12;
            this.chkSingleFiles.Text = "Single Files";
            this.chkSingleFiles.UseVisualStyleBackColor = true;
            // 
            // btnOutputAllToFile
            // 
            this.btnOutputAllToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOutputAllToFile.Location = new System.Drawing.Point(4, 863);
            this.btnOutputAllToFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOutputAllToFile.Name = "btnOutputAllToFile";
            this.btnOutputAllToFile.Size = new System.Drawing.Size(176, 38);
            this.btnOutputAllToFile.TabIndex = 11;
            this.btnOutputAllToFile.Text = "Output All To File";
            this.btnOutputAllToFile.UseVisualStyleBackColor = true;
            // 
            // txtNamespace
            // 
            this.txtNamespace.Location = new System.Drawing.Point(111, 7);
            this.txtNamespace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(248, 31);
            this.txtNamespace.TabIndex = 1;
            this.txtNamespace.Text = "MyProject";
            this.txtNamespace.TextChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // lblNamespace
            // 
            this.lblNamespace.AutoSize = true;
            this.lblNamespace.Location = new System.Drawing.Point(4, 15);
            this.lblNamespace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNamespace.Name = "lblNamespace";
            this.lblNamespace.Size = new System.Drawing.Size(104, 25);
            this.lblNamespace.TabIndex = 0;
            this.lblNamespace.Text = "Namespace";
            // 
            // chkIncludeMessagePackAttributes
            // 
            this.chkIncludeMessagePackAttributes.AutoSize = true;
            this.chkIncludeMessagePackAttributes.Location = new System.Drawing.Point(586, 53);
            this.chkIncludeMessagePackAttributes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkIncludeMessagePackAttributes.Name = "chkIncludeMessagePackAttributes";
            this.chkIncludeMessagePackAttributes.Size = new System.Drawing.Size(293, 29);
            this.chkIncludeMessagePackAttributes.TabIndex = 7;
            this.chkIncludeMessagePackAttributes.Text = "Include Message Pack Attributes";
            this.chkIncludeMessagePackAttributes.UseVisualStyleBackColor = true;
            this.chkIncludeMessagePackAttributes.CheckedChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(431, 7);
            this.txtPrefix.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(73, 31);
            this.txtPrefix.TabIndex = 3;
            this.txtPrefix.TextChanged += new System.EventHandler(this.CheckBoxes_CheckedChanged);
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(370, 15);
            this.lblPrefix.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(55, 25);
            this.lblPrefix.TabIndex = 2;
            this.lblPrefix.Text = "Prefix";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1218, 863);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(107, 38);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCode.Location = new System.Drawing.Point(4, 93);
            this.txtCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(1319, 764);
            this.txtCode.TabIndex = 10;
            this.txtCode.WordWrap = false;
            // 
            // lblDatabaseType
            // 
            this.lblDatabaseType.AutoSize = true;
            this.lblDatabaseType.Location = new System.Drawing.Point(6, 87);
            this.lblDatabaseType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDatabaseType.Name = "lblDatabaseType";
            this.lblDatabaseType.Size = new System.Drawing.Size(128, 25);
            this.lblDatabaseType.TabIndex = 2;
            this.lblDatabaseType.Text = "Database Type";
            // 
            // cboDatabaseType
            // 
            this.cboDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDatabaseType.FormattingEnabled = true;
            this.cboDatabaseType.Location = new System.Drawing.Point(160, 80);
            this.cboDatabaseType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboDatabaseType.Name = "cboDatabaseType";
            this.cboDatabaseType.Size = new System.Drawing.Size(135, 33);
            this.cboDatabaseType.TabIndex = 3;
            this.cboDatabaseType.SelectedIndexChanged += new System.EventHandler(this.CboDatabaseType_SelectedIndexChanged);
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(6, 40);
            this.lblConnectionString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(153, 25);
            this.lblConnectionString.TabIndex = 0;
            this.lblConnectionString.Text = "Connection String";
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnectionString.Location = new System.Drawing.Point(160, 33);
            this.txtConnectionString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(1501, 31);
            this.txtConnectionString.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnConnect.Location = new System.Drawing.Point(303, 73);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(137, 47);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // grpConnection
            // 
            this.grpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnection.Controls.Add(this.lblExample);
            this.grpConnection.Controls.Add(this.txtExampleConnectionString);
            this.grpConnection.Controls.Add(this.lblConnectionString);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.lblDatabaseType);
            this.grpConnection.Controls.Add(this.txtConnectionString);
            this.grpConnection.Controls.Add(this.cboDatabaseType);
            this.grpConnection.Location = new System.Drawing.Point(6, 13);
            this.grpConnection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpConnection.Size = new System.Drawing.Size(1669, 133);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Database Connection";
            // 
            // lblExample
            // 
            this.lblExample.AutoSize = true;
            this.lblExample.Location = new System.Drawing.Point(469, 87);
            this.lblExample.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(78, 25);
            this.lblExample.TabIndex = 5;
            this.lblExample.Text = "Example";
            // 
            // txtExampleConnectionString
            // 
            this.txtExampleConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExampleConnectionString.Location = new System.Drawing.Point(549, 80);
            this.txtExampleConnectionString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtExampleConnectionString.Name = "txtExampleConnectionString";
            this.txtExampleConnectionString.ReadOnly = true;
            this.txtExampleConnectionString.Size = new System.Drawing.Size(1103, 31);
            this.txtExampleConnectionString.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1677, 1062);
            this.Controls.Add(this.grpConnection);
            this.Controls.Add(this.splitContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Query Lite Code Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblExample;
        private System.Windows.Forms.TextBox txtExampleConnectionString;
        private System.Windows.Forms.CheckBox chkIncludeConstraints;
        private System.Windows.Forms.CheckBox chkIncludeJsonAttributes;
    }
}