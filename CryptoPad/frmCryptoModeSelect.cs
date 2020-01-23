using System;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmCryptoModeSelect : Form
    {
        public string Keyfile { get; private set; }
        public string Password { get; private set; }

        public CryptoMode Modes { get; private set; }

        public bool ValidInput
        {
            get
            {
                return !string.IsNullOrEmpty(Keyfile) || !string.IsNullOrEmpty(Password);
            }
        }

        public frmCryptoModeSelect(CryptoMode AllowedModes = (CryptoMode)~0, CryptoMode PreselectedModes = 0)
        {
            InitializeComponent();
            //Set enabled controls
            cbUserAccount.Enabled = AllowedModes.HasFlag(CryptoMode.CryptUser);
            cbComputerAccount.Enabled = AllowedModes.HasFlag(CryptoMode.CryptMachine);
            btnBrowse.Enabled = tbKeyfile.Enabled = cbKeyfile.Enabled = AllowedModes.HasFlag(CryptoMode.Keyfile);
            tbPassword.Enabled = cbPassword.Enabled = AllowedModes.HasFlag(CryptoMode.Password);
            //Set checked controls
            cbUserAccount.Checked = cbUserAccount.Enabled && PreselectedModes.HasFlag(CryptoMode.CryptUser);
            cbComputerAccount.Checked = cbUserAccount.Enabled && PreselectedModes.HasFlag(CryptoMode.CryptMachine);
            cbKeyfile.Checked = cbUserAccount.Enabled && PreselectedModes.HasFlag(CryptoMode.Keyfile);
            cbPassword.Checked = cbUserAccount.Enabled && PreselectedModes.HasFlag(CryptoMode.Password);
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
            cbKeyfile.Checked = !string.IsNullOrEmpty(tbKeyfile.Text);
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            cbPassword.Checked = !string.IsNullOrEmpty(tbPassword.Text);
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
            if (cbUserAccount.Checked)
            {
                Modes |= CryptoMode.CryptUser;
            }
            if (cbComputerAccount.Checked)
            {
                Modes |= CryptoMode.CryptMachine;
            }
        }
    }
}
