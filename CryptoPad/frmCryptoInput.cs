using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmCryptoInput : Form
    {
        public string Keyfile { get; private set; }

        public string Password { get; private set; }

        public RSAKey RsaKey { get; private set; }

        public bool ValidInput
        {
            get
            {
                return
                    !string.IsNullOrEmpty(Keyfile) ||
                    !string.IsNullOrEmpty(Password)
                    || RsaKey != null;
            }
        }

        private IEnumerable<RSAKey> RsaKeys;

        public frmCryptoInput(CryptoMode Modes, IEnumerable<RSAKey> Keys)
        {
            InitializeComponent();

            RsaKeys = Keys == null ? null : Keys.ToArray();

            if (!Modes.HasFlag(CryptoMode.Keyfile))
            {
                cbKeyfile.Enabled = tbKeyfile.Enabled = btnKeyfile.Enabled = false;
            }
            if (!Modes.HasFlag(CryptoMode.Password))
            {
                cbPassword.Enabled = tbPassword.Enabled = false;
            }
            if (!Modes.HasFlag(CryptoMode.RSA) || Keys == null)
            {
                cbRSA.Enabled = lblRSAName.Enabled = btnRSA.Enabled = false;
            }
        }

        private void btnKeyfile_Click(object sender, EventArgs e)
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
            if (!cbKeyfile.Checked)
            {
                RsaKey = null;
            }
            if (!string.IsNullOrEmpty(tbPassword.Text) && cbPassword.Checked)
            {
                Password = tbPassword.Text;
            }
            if (!string.IsNullOrEmpty(tbKeyfile.Text) && cbKeyfile.Checked)
            {
                Keyfile = tbKeyfile.Text;
            }
        }

        private void btnRSA_Click(object sender, EventArgs e)
        {
            using (var F = new frmRSASelect(RsaKeys, false, RsaKey))
            {
                if (F.ShowDialog() == DialogResult.OK)
                {
                    cbRSA.Checked = true;
                    RsaKey = F.SelectedKey;
                    lblRSAName.Text = RsaKey.Name;
                }
            }
        }
    }
}
