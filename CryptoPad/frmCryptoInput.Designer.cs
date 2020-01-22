namespace CryptoPad
{
    partial class frmCryptoInput
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
            this.lblDesc = new System.Windows.Forms.Label();
            this.btnKeyfile = new System.Windows.Forms.Button();
            this.cbKeyfile = new System.Windows.Forms.CheckBox();
            this.cbPassword = new System.Windows.Forms.CheckBox();
            this.tbKeyfile = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDesc
            // 
            this.lblDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDesc.Location = new System.Drawing.Point(12, 9);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(468, 61);
            this.lblDesc.TabIndex = 0;
            this.lblDesc.Text = "Decrypting the specified file requires your input.\r\nDepending on how it was encry" +
    "pted you can supply the password or select the key file.\r\nYou only need to suppl" +
    "y one of the allowed methods.";
            // 
            // btnKeyfile
            // 
            this.btnKeyfile.Location = new System.Drawing.Point(405, 71);
            this.btnKeyfile.Name = "btnKeyfile";
            this.btnKeyfile.Size = new System.Drawing.Size(75, 23);
            this.btnKeyfile.TabIndex = 3;
            this.btnKeyfile.Text = "&Browse...";
            this.btnKeyfile.UseVisualStyleBackColor = true;
            this.btnKeyfile.Click += new System.EventHandler(this.btnKeyfile_Click);
            // 
            // cbKeyfile
            // 
            this.cbKeyfile.AutoSize = true;
            this.cbKeyfile.Location = new System.Drawing.Point(26, 75);
            this.cbKeyfile.Name = "cbKeyfile";
            this.cbKeyfile.Size = new System.Drawing.Size(79, 17);
            this.cbKeyfile.TabIndex = 1;
            this.cbKeyfile.Text = "Use Keyfile";
            this.cbKeyfile.UseVisualStyleBackColor = true;
            // 
            // cbPassword
            // 
            this.cbPassword.AutoSize = true;
            this.cbPassword.Location = new System.Drawing.Point(26, 101);
            this.cbPassword.Name = "cbPassword";
            this.cbPassword.Size = new System.Drawing.Size(94, 17);
            this.cbPassword.TabIndex = 4;
            this.cbPassword.Text = "Use Password";
            this.cbPassword.UseVisualStyleBackColor = true;
            // 
            // tbKeyfile
            // 
            this.tbKeyfile.Location = new System.Drawing.Point(126, 73);
            this.tbKeyfile.Name = "tbKeyfile";
            this.tbKeyfile.Size = new System.Drawing.Size(273, 20);
            this.tbKeyfile.TabIndex = 2;
            this.tbKeyfile.TextChanged += new System.EventHandler(this.tbKeyfile_TextChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(126, 99);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(273, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(324, 137);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(405, 137);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmCryptoInput
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(492, 172);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbKeyfile);
            this.Controls.Add(this.cbPassword);
            this.Controls.Add(this.cbKeyfile);
            this.Controls.Add(this.btnKeyfile);
            this.Controls.Add(this.lblDesc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCryptoInput";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Decryption requires user information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Button btnKeyfile;
        private System.Windows.Forms.CheckBox cbKeyfile;
        private System.Windows.Forms.CheckBox cbPassword;
        private System.Windows.Forms.TextBox tbKeyfile;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}