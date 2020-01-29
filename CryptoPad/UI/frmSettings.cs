using System;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmSettings : Form
    {
        private AppSettings Settings;

        public frmSettings(AppSettings CurrentSettings)
        {
            InitializeComponent();
            Settings = CurrentSettings;

            InitRSA();
        }

        private void InitRSA()
        {
            lvRSA.Items.Clear();
            foreach (var Key in Settings.LoadRSAKeys())
            {
                var Item = lvRSA.Items.Add(Key.Name);
                Item.SubItems.Add((Key.Key.Modulus.Length * 8).ToString());
                Item.SubItems.Add(RSAEncryption.HasPublicKey(Key.Key) ? "Yes" : "No");
                Item.SubItems.Add(RSAEncryption.HasPrivateKey(Key.Key) ? "Yes" : "No");
            }
        }
    }
}
