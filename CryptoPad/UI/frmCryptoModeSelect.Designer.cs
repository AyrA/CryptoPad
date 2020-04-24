namespace CryptoPad
{
    partial class frmCryptoModeSelect
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCryptoModeSelect));
            this.cbUserAccount = new System.Windows.Forms.CheckBox();
            this.cbComputerAccount = new System.Windows.Forms.CheckBox();
            this.cbKeyfile = new System.Windows.Forms.CheckBox();
            this.cbPassword = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbKeyfile = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.cbRSA = new System.Windows.Forms.CheckBox();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.lblRsaName = new System.Windows.Forms.Label();
            this.btnRsaSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbUserAccount
            // 
            this.cbUserAccount.AutoSize = true;
            this.cbUserAccount.Location = new System.Drawing.Point(12, 63);
            this.cbUserAccount.Name = "cbUserAccount";
            this.cbUserAccount.Size = new System.Drawing.Size(91, 17);
            this.cbUserAccount.TabIndex = 1;
            this.cbUserAccount.Text = "User Account";
            this.ttMain.SetToolTip(this.cbUserAccount, resources.GetString("cbUserAccount.ToolTip"));
            this.cbUserAccount.UseVisualStyleBackColor = true;
            // 
            // cbComputerAccount
            // 
            this.cbComputerAccount.AutoSize = true;
            this.cbComputerAccount.Location = new System.Drawing.Point(12, 90);
            this.cbComputerAccount.Name = "cbComputerAccount";
            this.cbComputerAccount.Size = new System.Drawing.Size(103, 17);
            this.cbComputerAccount.TabIndex = 2;
            this.cbComputerAccount.Text = "System Account";
            this.ttMain.SetToolTip(this.cbComputerAccount, "This uses keys assigned to this computer.\r\nThis allows you to decrypt the file ag" +
        "ain without any inputs with any account on this computer.\r\nif you reinstall Wind" +
        "ows, this method will fail.\r\n\r\n");
            this.cbComputerAccount.UseVisualStyleBackColor = true;
            // 
            // cbKeyfile
            // 
            this.cbKeyfile.AutoSize = true;
            this.cbKeyfile.Location = new System.Drawing.Point(12, 117);
            this.cbKeyfile.Name = "cbKeyfile";
            this.cbKeyfile.Size = new System.Drawing.Size(57, 17);
            this.cbKeyfile.TabIndex = 3;
            this.cbKeyfile.Text = "Keyfile";
            this.ttMain.SetToolTip(this.cbKeyfile, resources.GetString("cbKeyfile.ToolTip"));
            this.cbKeyfile.UseVisualStyleBackColor = true;
            // 
            // cbPassword
            // 
            this.cbPassword.AutoSize = true;
            this.cbPassword.Location = new System.Drawing.Point(12, 144);
            this.cbPassword.Name = "cbPassword";
            this.cbPassword.Size = new System.Drawing.Size(72, 17);
            this.cbPassword.TabIndex = 6;
            this.cbPassword.Text = "Password";
            this.ttMain.SetToolTip(this.cbPassword, "Allows decryption using an ordinary password.");
            this.cbPassword.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBrowse.Location = new System.Drawing.Point(478, 113);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tbKeyfile
            // 
            this.tbKeyfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbKeyfile.Location = new System.Drawing.Point(122, 115);
            this.tbKeyfile.Name = "tbKeyfile";
            this.tbKeyfile.Size = new System.Drawing.Size(350, 20);
            this.tbKeyfile.TabIndex = 4;
            this.tbKeyfile.TextChanged += new System.EventHandler(this.tbKeyfile_TextChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPassword.Location = new System.Drawing.Point(122, 145);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(350, 20);
            this.tbPassword.TabIndex = 7;
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(259, 39);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Select all methods available for decryption.\r\nAny single one of these will be abl" +
    "e to decrypt the file.\r\nHover over the check boxes to get further information";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(397, 213);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(478, 213);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 32767;
            this.ttMain.InitialDelay = 1;
            this.ttMain.ReshowDelay = 1;
            this.ttMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ttMain.ToolTipTitle = "Crypto mode information";
            this.ttMain.UseAnimation = false;
            this.ttMain.UseFading = false;
            // 
            // cbRSA
            // 
            this.cbRSA.AutoSize = true;
            this.cbRSA.Location = new System.Drawing.Point(12, 171);
            this.cbRSA.Name = "cbRSA";
            this.cbRSA.Size = new System.Drawing.Size(69, 17);
            this.cbRSA.TabIndex = 8;
            this.cbRSA.Text = "RSA Key";
            this.ttMain.SetToolTip(this.cbRSA, "Encrypts using the RSA algorithm\r\nMost useful to encrypt a file for someone else\r" +
        "\n");
            this.cbRSA.UseVisualStyleBackColor = true;
            // 
            // lblRsaName
            // 
            this.lblRsaName.AutoSize = true;
            this.lblRsaName.Location = new System.Drawing.Point(119, 173);
            this.lblRsaName.Name = "lblRsaName";
            this.lblRsaName.Size = new System.Drawing.Size(96, 13);
            this.lblRsaName.TabIndex = 9;
            this.lblRsaName.Text = "<No key selected>";
            // 
            // btnRsaSelect
            // 
            this.btnRsaSelect.Location = new System.Drawing.Point(478, 173);
            this.btnRsaSelect.Name = "btnRsaSelect";
            this.btnRsaSelect.Size = new System.Drawing.Size(75, 23);
            this.btnRsaSelect.TabIndex = 10;
            this.btnRsaSelect.Text = "&Select...";
            this.btnRsaSelect.UseVisualStyleBackColor = true;
            this.btnRsaSelect.Click += new System.EventHandler(this.btnRsaSelect_Click);
            // 
            // frmCryptoModeSelect
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(565, 248);
            this.Controls.Add(this.btnRsaSelect);
            this.Controls.Add(this.lblRsaName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbKeyfile);
            this.Controls.Add(this.cbRSA);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.cbPassword);
            this.Controls.Add(this.cbKeyfile);
            this.Controls.Add(this.cbComputerAccount);
            this.Controls.Add(this.cbUserAccount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCryptoModeSelect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Encryption mode selection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUserAccount;
        private System.Windows.Forms.CheckBox cbComputerAccount;
        private System.Windows.Forms.CheckBox cbKeyfile;
        private System.Windows.Forms.CheckBox cbPassword;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbKeyfile;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.CheckBox cbRSA;
        private System.Windows.Forms.Label lblRsaName;
        private System.Windows.Forms.Button btnRsaSelect;
    }
}