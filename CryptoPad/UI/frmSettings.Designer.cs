﻿namespace CryptoPad
{
    partial class frmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.flowPanelMode = new System.Windows.Forms.FlowLayoutPanel();
            this.lblModeDesc = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.LinkLabel();
            this.tabRSA = new System.Windows.Forms.TabPage();
            this.pbGenerator = new System.Windows.Forms.ProgressBar();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.lvRSA = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEncrypt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDecrypt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClose = new System.Windows.Forms.Button();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.SFD = new System.Windows.Forms.SaveFileDialog();
            this.FBD = new System.Windows.Forms.FolderBrowserDialog();
            this.tabs.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.flowPanelMode.SuspendLayout();
            this.tabRSA.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabSettings);
            this.tabs.Controls.Add(this.tabRSA);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(570, 522);
            this.tabs.TabIndex = 0;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.flowPanelMode);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(562, 496);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "General";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // flowPanelMode
            // 
            this.flowPanelMode.Controls.Add(this.lblModeDesc);
            this.flowPanelMode.Controls.Add(this.lblMode);
            this.flowPanelMode.Location = new System.Drawing.Point(6, 12);
            this.flowPanelMode.Name = "flowPanelMode";
            this.flowPanelMode.Size = new System.Drawing.Size(550, 28);
            this.flowPanelMode.TabIndex = 0;
            // 
            // lblModeDesc
            // 
            this.lblModeDesc.AutoSize = true;
            this.lblModeDesc.Location = new System.Drawing.Point(3, 0);
            this.lblModeDesc.Name = "lblModeDesc";
            this.lblModeDesc.Size = new System.Drawing.Size(74, 13);
            this.lblModeDesc.TabIndex = 0;
            this.lblModeDesc.Text = "Current Mode:";
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(83, 0);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(46, 13);
            this.lblMode.TabIndex = 1;
            this.lblMode.TabStop = true;
            this.lblMode.Text = "<Mode>";
            this.lblMode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblMode_LinkClicked);
            // 
            // tabRSA
            // 
            this.tabRSA.Controls.Add(this.pbGenerator);
            this.tabRSA.Controls.Add(this.btnNew);
            this.tabRSA.Controls.Add(this.btnDelete);
            this.tabRSA.Controls.Add(this.btnImport);
            this.tabRSA.Controls.Add(this.btnExport);
            this.tabRSA.Controls.Add(this.btnBackup);
            this.tabRSA.Controls.Add(this.lvRSA);
            this.tabRSA.Location = new System.Drawing.Point(4, 22);
            this.tabRSA.Name = "tabRSA";
            this.tabRSA.Padding = new System.Windows.Forms.Padding(3);
            this.tabRSA.Size = new System.Drawing.Size(562, 496);
            this.tabRSA.TabIndex = 1;
            this.tabRSA.Text = "RSA Keys";
            this.tabRSA.UseVisualStyleBackColor = true;
            // 
            // pbGenerator
            // 
            this.pbGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbGenerator.Location = new System.Drawing.Point(6, 467);
            this.pbGenerator.Name = "pbGenerator";
            this.pbGenerator.Size = new System.Drawing.Size(145, 23);
            this.pbGenerator.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbGenerator.TabIndex = 6;
            this.pbGenerator.Visible = false;
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Location = new System.Drawing.Point(157, 467);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 1;
            this.btnNew.Text = "&New...";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(481, 467);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "&Delete...";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(400, 467);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "&Import...";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(319, 467);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "&Export...";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackup.Location = new System.Drawing.Point(238, 467);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(75, 23);
            this.btnBackup.TabIndex = 2;
            this.btnBackup.Text = "&Backup...";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // lvRSA
            // 
            this.lvRSA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRSA.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSize,
            this.chEncrypt,
            this.chDecrypt});
            this.lvRSA.FullRowSelect = true;
            this.lvRSA.HideSelection = false;
            this.lvRSA.Location = new System.Drawing.Point(6, 6);
            this.lvRSA.Name = "lvRSA";
            this.lvRSA.Size = new System.Drawing.Size(550, 455);
            this.lvRSA.TabIndex = 0;
            this.lvRSA.UseCompatibleStateImageBehavior = false;
            this.lvRSA.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 100;
            // 
            // chSize
            // 
            this.chSize.Text = "Size";
            this.chSize.Width = 100;
            // 
            // chEncrypt
            // 
            this.chEncrypt.Text = "Can Encrypt";
            this.chEncrypt.Width = 100;
            // 
            // chDecrypt
            // 
            this.chDecrypt.Text = "Can Decrypt";
            this.chDecrypt.Width = 100;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(503, 540);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // OFD
            // 
            this.OFD.DefaultExt = "rsa";
            this.OFD.Filter = "RSA keys|*.rsa|All files|*.*";
            this.OFD.Multiselect = true;
            this.OFD.Title = "Select RSA key(s)";
            // 
            // SFD
            // 
            this.SFD.DefaultExt = "rsa";
            this.SFD.Filter = "RSA keys|*.rsa";
            this.SFD.Title = "Save key";
            // 
            // FBD
            // 
            this.FBD.Description = "Save multiple keys";
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(594, 575);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.tabs.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.flowPanelMode.ResumeLayout(false);
            this.flowPanelMode.PerformLayout();
            this.tabRSA.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabRSA;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.ListView lvRSA;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chEncrypt;
        private System.Windows.Forms.ColumnHeader chDecrypt;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.SaveFileDialog SFD;
        private System.Windows.Forms.FolderBrowserDialog FBD;
        private System.Windows.Forms.FlowLayoutPanel flowPanelMode;
        private System.Windows.Forms.Label lblModeDesc;
        private System.Windows.Forms.LinkLabel lblMode;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.ProgressBar pbGenerator;
    }
}