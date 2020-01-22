using System;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmCryptoInput : Form
    {
        public string Keyfile { get; set; }
        public string Password { get; set; }
        public bool ValidInput
        {
            get
            {
                return !string.IsNullOrEmpty(Keyfile) || !string.IsNullOrEmpty(Password);
            }
        }

        public frmCryptoInput(CryptoMode Modes)
        {
            InitializeComponent();
            if (!Modes.HasFlag(CryptoMode.Keyfile))
            {
                cbKeyfile.Enabled = tbKeyfile.Enabled = btnKeyfile.Enabled = false;
            }
            if (!Modes.HasFlag(CryptoMode.Password))
            {
                cbPassword.Enabled = tbPassword.Enabled = false;
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
            if (!string.IsNullOrEmpty(tbPassword.Text) && cbPassword.Checked)
            {
                Password = tbPassword.Text;
            }
            if (!string.IsNullOrEmpty(tbKeyfile.Text) && cbKeyfile.Checked)
            {
                Keyfile = tbKeyfile.Text;
            }
        }
    }
}
