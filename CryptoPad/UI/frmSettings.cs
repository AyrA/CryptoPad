using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmSettings : Form
    {
        private AppSettings Settings;

        public frmSettings(AppSettings CurrentSettings)
        {
            bool RSA = true;
            InitializeComponent();
            Settings = CurrentSettings;

            lblMode.Text = Settings.Type.ToString();
            if (Settings.Type != SettingsType.Portable)
            {
                var GS = AppSettings.GlobalSettings();
                if (GS != null)
                {
                    lblMode.Enabled = !GS.Restrictions.BlockPortable;
                    if (GS.Restrictions.BlockedModes.Contains(CryptoMode.RSA))
                    {
                        RSA = false;
                        foreach (var ctrl in tabRSA.Controls.OfType<Control>())
                        {
                            ctrl.Enabled = false;
                        }
                    }
                }
            }
            if (RSA)
            {
                InitRSA();
            }
        }

        private void InitRSA()
        {
            lvRSA.Items.Clear();
            foreach (var Key in Settings.LoadRSAKeys())
            {
                var Item = lvRSA.Items.Add(Key.Name);
                Item.Tag = Key;
                Item.SubItems.Add(Key.Size.ToString());
                Item.SubItems.Add(RSAEncryption.HasPublicKey(Key.Key) ? "Yes" : "No");
                Item.SubItems.Add(RSAEncryption.HasPrivateKey(Key.Key) ? "Yes" : "No");
            }
            foreach (var Key in AppSettings.GetAdministrativeKeys())
            {
                var Item = lvRSA.Items.Add(Key.Name);
                Item.Tag = null;
                Item.BackColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00);
                Item.SubItems.Add(Key.Size.ToString());
                Item.SubItems.Add(RSAEncryption.HasPublicKey(Key.Key) ? "Yes" : "No");
                Item.SubItems.Add(RSAEncryption.HasPrivateKey(Key.Key) ? "Yes" : "No");
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            const string ExportAlert =
                "You're about to export at least one RSA key that has private key information.\r\n" +
                "Under no circumstances should you share those keys with anybody, regardless of what they tell you.\r\nContinue?";
            if (lvRSA.SelectedItems.Count > 0)
            {
                var Keys = lvRSA.SelectedItems
                    .OfType<ListViewItem>()
                    .Where(m => m.Tag != null)
                    .Select(m => (RSAKey)m.Tag)
                    .ToArray();
                if (Keys.Length == 0)
                {
                    Program.AlertMsg("You can't export administratively added keys");
                    return;
                }
                if (!Keys.Any(m => RSAEncryption.HasPrivateKey(m.Key)) || Program.AlertMsg(ExportAlert, true) == DialogResult.Yes)
                {
                    if (lvRSA.SelectedItems.Count == 1)
                    {
                        SFD.FileName = Tools.SanitizeName(Keys[0].Name + ".rsa");
                        if (SFD.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                System.IO.File.WriteAllText(SFD.FileName, Keys[0].ToXML());
                            }
                            catch (Exception ex)
                            {
                                Program.ErrorMsg("Unable to back up your key. Error:\r\n" + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        if (FBD.ShowDialog() == DialogResult.OK)
                        {
                            foreach (var K in Keys)
                            {
                                try
                                {
                                    System.IO.File.WriteAllText(Tools.UniqueName(FBD.SelectedPath, Tools.SanitizeName(K.Name + ".rsa")), K.ToXML());
                                }
                                catch (Exception ex)
                                {
                                    Program.ErrorMsg($"Unable to back up your key named {K.Name}. Error:\r\n{ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Program.AlertMsg("Please select at least one key");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            const string ExportAlert =
                "You're about to export only the public RSA key parts.\r\n" +
                "This key will allow encryption only and is meant for sharing with other people and not creating backups.\r\n" +
                "Continue?";
            if (lvRSA.SelectedItems.Count > 0)
            {
                var Keys = lvRSA.SelectedItems
                    .OfType<ListViewItem>()
                    .Where(m => m.Tag != null)
                    .Select(m => RSAEncryption.StripPrivate((RSAKey)m.Tag))
                    .ToArray();
                if (Keys.Length > 0 && Program.AlertMsg(ExportAlert, true) == DialogResult.Yes)
                {
                    if (lvRSA.SelectedItems.Count == 1)
                    {
                        SFD.FileName = Tools.SanitizeName(Keys[0].Name + ".rsa");
                        if (SFD.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                System.IO.File.WriteAllText(SFD.FileName, Keys[0].ToXML());
                            }
                            catch (Exception ex)
                            {
                                Program.ErrorMsg("Unable to export your key. Error:\r\n" + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        if (FBD.ShowDialog() == DialogResult.OK)
                        {
                            foreach (var K in Keys)
                            {
                                try
                                {
                                    System.IO.File.WriteAllText(Tools.UniqueName(FBD.SelectedPath, Tools.SanitizeName(K.Name + ".rsa")), K.ToXML());
                                }
                                catch (Exception ex)
                                {
                                    Program.ErrorMsg($"Unable to export your key named {K.Name}. Error:\r\n{ex.Message}");
                                }
                            }
                        }
                    }
                }
                else if (Keys.Length == 0)
                {
                    Program.AlertMsg("You can't export administratively added keys");
                }
            }
            else
            {
                Program.AlertMsg("Please select at least one key");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var Keys = Settings.LoadRSAKeys();
                var AdminKeys = AppSettings.GetAdministrativeKeys();
                var NewKeys = new List<RSAKey>();
                foreach (var Name in OFD.FileNames)
                {
                    try
                    {
                        var Key = Tools.FromXML<RSAKey>(System.IO.File.ReadAllText(Name));
                        if (!Key.IsValid())
                        {
                            throw new Exception("The loaded RSA key is not valid");
                        }
                        //Check if the key exists as-is
                        if (!Keys.Concat(AdminKeys).Any(m => m.Equals(Key)))
                        {
                            //Check if the key has a private key
                            if (RSAEncryption.HasPrivateKey(Key.Key))
                            {
                                //Check if any existing keys have the same public key
                                for (var i = 0; i < Keys.Length; i++)
                                {
                                    //Replace existing key with imported key if the imported key has a private key
                                    if (Keys[i].IsSamePublicKey(Key))
                                    {
                                        Keys[i] = Key;
                                    }
                                }
                            }
                            else
                            {
                                //Key does not exists and has no private key. Just add it.
                                NewKeys.Add(Key);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.ErrorMsg("Unable to import your key. Error:\r\n" + ex.Message);
                    }
                }
                Settings.SaveRSAKeys(Keys.Concat(NewKeys), true);
                //Render new RSA key list
                InitRSA();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvRSA.SelectedItems.Count > 0)
            {
                if (Program.AlertMsg("Really delete all selected keys? This can't be undone and you will lose access to any file encrypted with them", true) == DialogResult.Yes)
                {
                    var SelectedKeys = lvRSA.SelectedItems
                        .OfType<ListViewItem>()
                        .Select(m => (RSAKey)m.Tag)
                        .ToArray();
                    var adminCount = SelectedKeys.Count(m => m == null);
                    var removed = false;
                    var AllKeys = Settings.LoadRSAKeys().ToList();
                    foreach (var K in SelectedKeys)
                    {
                        removed |= AllKeys.Remove(K);
                    }
                    if (!removed)
                    {
                        Program.AlertMsg("No keys were removed. Note that you can't delete administratively added keys");
                    }
                    Settings.SaveRSAKeys(AllKeys, true);
                    InitRSA();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (var Keygen = new frmRsaGen())
            {
                if (Keygen.ShowDialog() == DialogResult.OK)
                {
                    var Props = new { Keygen.KeyName, Keygen.KeySize };
                    foreach (var C in Controls) { ((Control)C).Enabled = false; }
                    pbGenerator.Visible = true;
                    Thread T = new Thread(delegate ()
                    {
                        var Key = RSAEncryption.GenerateKey(Props.KeyName, Props.KeySize);
                        Invoke((MethodInvoker)delegate
                        {
                            var Keys = Settings.LoadRSAKeys().ToList();
                            Keys.Add(Key);
                            foreach (var C in Controls) { ((Control)C).Enabled = true; }
                            pbGenerator.Visible = false;
                            Settings.SaveRSAKeys(Keys, true);
                            InitRSA();
                        });
                    });
                    T.IsBackground = true;
                    T.Start();
                }
            }
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Don't allow the user to close the form while the key generator is running
            if (e.Cancel = e.CloseReason == CloseReason.UserClosing && pbGenerator.Visible)
            {
                Program.AlertMsg("Please wait for the key generator to finish. This usually takes only a few seconds");
            }
        }

        private void lblMode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var Keys = Settings.LoadRSAKeys();
            var Global = AppSettings.GlobalSettings();
            if (Settings.Type == SettingsType.Local || Settings.Type == SettingsType.Global)
            {
                if (Global == null || Global.Restrictions == null || !Global.Restrictions.BlockPortable)
                {
                    if (Program.AlertMsg("Turn on portable mode? This allows you to take your settings and keys with you on a flash drive, but it's easier to accidentally delete your keys.", true) == DialogResult.Yes)
                    {
                        var KeyStore = Settings.KeyStorage;
                        var isLocal = Settings.Type == SettingsType.Local;
                        Settings.SaveSettings(SettingsType.Portable);
                        Settings.SaveRSAKeys(Keys);
                        if (isLocal && Program.AlertMsg("Portable mode enabled. Delete old settings and keys from user profile?", true) == DialogResult.Yes)
                        {
                            System.IO.File.Delete(AppSettings.UserSettingsFile);
                            System.IO.Directory.Delete(KeyStore, true);
                        }
                    }
                }
                else
                {
                    Program.ErrorMsg("The administrator disabled switching to portable mode");
                }
            }
            else if (Settings.Type == SettingsType.Portable)
            {
                if (Program.AlertMsg("Really change into regular mode? This will copy all RSA keys to the local user profile (keeps existing keys) and delete the portable settings and keys.", true) == DialogResult.Yes)
                {
                    var KeyStore = Settings.KeyStorage;
                    Settings = Settings.SaveSettings(SettingsType.Local);
                    Settings.SaveRSAKeys(Keys);
                    try
                    {
                        System.IO.File.Delete(AppSettings.PortableSettingsFile);
                        System.IO.Directory.Delete(KeyStore, true);
                    }
                    catch
                    {
                        Program.ErrorMsg("Unable to delete the key store");
                    }
                }
            }
            else
            {
                throw new NotImplementedException($"Invalid {nameof(SettingsType)} value");
            }
            lblMode.Text = Settings.Type.ToString();
            InitRSA();
        }
    }
}
