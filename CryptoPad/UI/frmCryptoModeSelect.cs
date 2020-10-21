using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmCryptoModeSelect : Form
    {
        public string Keyfile { get; private set; }
        public string Password { get; private set; }
        public RSAKey RsaKey { get; private set; }
        public CryptoMode Modes { get; private set; }

        public bool ValidInput
        {
            get
            {
                return
                    !string.IsNullOrEmpty(Keyfile) ||
                    !string.IsNullOrEmpty(Password) ||
                    RsaKey != null;
            }
        }

        private AppSettings Settings;

        public frmCryptoModeSelect(AppSettings Settings, CryptoMode AllowedModes = CryptoMode._ALL, CryptoMode PreselectedModes = 0)
        {
            InitializeComponent();

            this.Settings = Settings;

            //Set enabled controls
            cbUserAccount.Enabled = AllowedModes.HasFlag(CryptoMode.CryptUser);
            cbComputerAccount.Enabled = AllowedModes.HasFlag(CryptoMode.CryptMachine);
            cbKeyfile.Enabled = AllowedModes.HasFlag(CryptoMode.Keyfile);
            cbPassword.Enabled = AllowedModes.HasFlag(CryptoMode.Password);
            cbRSA.Enabled = AllowedModes.HasFlag(CryptoMode.RSA);

            //Set checked controls
            cbUserAccount.Checked = cbUserAccount.Enabled && PreselectedModes.HasFlag(CryptoMode.CryptUser);
            cbComputerAccount.Checked = cbComputerAccount.Enabled && PreselectedModes.HasFlag(CryptoMode.CryptMachine);
            cbKeyfile.Checked = cbKeyfile.Enabled && PreselectedModes.HasFlag(CryptoMode.Keyfile);
            cbPassword.Checked = cbPassword.Enabled && PreselectedModes.HasFlag(CryptoMode.Password);
            cbRSA.Checked = cbRSA.Enabled && PreselectedModes.HasFlag(CryptoMode.RSA);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                tbKeyfile.Text = OFD.FileName;
                cbKeyfile.Checked = true;
            }
        }

        private void tbKeyfile_TextChanged(object sender, EventArgs e)
        {
            //cbKeyfile.Checked = !string.IsNullOrEmpty(tbKeyfile.Text);
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            //cbPassword.Checked = !string.IsNullOrEmpty(tbPassword.Text);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Modes = 0;
            if (!string.IsNullOrEmpty(tbPassword.Text) && cbPassword.Checked)
            {
                Password = tbPassword.Text;
                Modes |= CryptoMode.Password;
            }
            if (!string.IsNullOrEmpty(tbKeyfile.Text) && cbKeyfile.Checked)
            {
                Keyfile = tbKeyfile.Text;
                Modes |= CryptoMode.Keyfile;
            }
            if (RsaKey != null && cbRSA.Checked)
            {
                Modes |= CryptoMode.RSA;
            }
            else
            {
                RsaKey = null;
            }
            if (cbUserAccount.Checked)
            {
                Modes |= CryptoMode.CryptUser;
            }
            if (cbComputerAccount.Checked)
            {
                Modes |= CryptoMode.CryptMachine;
            }
        }

        private void btnRsaSelect_Click(object sender, EventArgs e)
        {
            using (var F = new frmRSASelect(Settings.LoadRSAKeys(), true, RsaKey))
            {
                if (F.ShowDialog() == DialogResult.OK)
                {
                    cbRSA.Checked = true;
                    RsaKey = F.SelectedKey;
                    if (RsaKey == null)
                    {
                        lblRsaName.Text = "<No key selected>";
                        cbRSA.Checked = false;
                    }
                    else
                    {
                        lblRsaName.Text = RsaKey.Name;
                        if (!RSAEncryption.HasPrivateKey(RsaKey.Key))
                        {
                            Program.AlertMsg(
                                "You picked a key that can only encrypt, not decrypt. " +
                                "You will not be able to open the file again once you close it.\r\n" +
                                "You should only do this if you're encrypting the file for someone else.");
                        }
                    }
                }
                Settings.SaveRSAKeys(F.AllKeys, true);
            }
        }

        private void cbKeyfile_CheckedChanged(object sender, EventArgs e)
        {
            tbKeyfile.Enabled = btnBrowse.Enabled = cbKeyfile.Checked;
        }

        private void cbPassword_CheckedChanged(object sender, EventArgs e)
        {
            tbPassword.Enabled = cbPassword.Checked;
        }

        private void cbRSA_CheckedChanged(object sender, EventArgs e)
        {
            lblRsaName.Enabled = btnRsaSelect.Enabled = cbRSA.Checked;
        }
    }
}
