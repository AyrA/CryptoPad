using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmRSASelect : Form
    {
        public RSAKey SelectedKey { get; private set; }
        public List<RSAKey> AllKeys { get; private set; }

        public frmRSASelect(IEnumerable<RSAKey> Keys, bool CanCreate, RSAKey PreSelected = null)
        {
            if (Keys == null)
            {
                AllKeys = new List<RSAKey>();
            }
            AllKeys = Keys.ToList();

            InitializeComponent();

            btnCreate.Enabled = CanCreate;
            InitList(PreSelected);

        }

        private void InitList(RSAKey PreSelected)
        {
            AllKeys.Sort((a, b) => string.Compare(a.Name, b.Name));
            var ObjList = AllKeys
                .Select(m => new KeyLabel() { Key = m })
                .Cast<object>()
                .ToArray();
            cbKey.Items.Clear();
            cbKey.Items.AddRange(ObjList);
            if (ObjList.Length > 0)
            {
                cbKey.SelectedIndex = 0;
            }
            if (PreSelected != null)
            {
                for (var i = 0; i < AllKeys.Count; i++)
                {
                    if (AllKeys[i].Equals(PreSelected))
                    {
                        cbKey.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var F = new frmRsaGen())
            {
                if (F.ShowDialog() == DialogResult.OK)
                {
                    var Props = new { F.KeyName, F.KeySize };
                    foreach (var C in Controls) { ((Control)C).Enabled = false; }
                    pbKeygen.Visible = true;
                    Thread T = new Thread(delegate ()
                    {
                        var Key = RSAEncryption.GenerateKey(Props.KeyName, Props.KeySize);
                        AllKeys.Add(Key);
                        Invoke((MethodInvoker)delegate
                        {
                            foreach (var C in Controls) { ((Control)C).Enabled = true; }
                            pbKeygen.Visible = false;
                            InitList(Key);
                        });
                    });
                    T.IsBackground = true;
                    T.Start();
                }
            }
        }

        private void cbKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbKey.SelectedIndex >= 0)
            {
                SelectedKey = ((KeyLabel)cbKey.SelectedItem).Key;
            }
            else
            {
                SelectedKey = null;
            }
        }

        private void frmRSASelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Don't allow the user to close the form while the key generator is running
            if (e.Cancel = e.CloseReason == CloseReason.UserClosing && pbKeygen.Visible)
            {
                Program.AlertMsg("Please wait for the key generator to finish. This usually takes a few seconds");
            }
        }

        private struct KeyLabel
        {
            public RSAKey Key;

            public override string ToString()
            {
                return $"{Key.Name} ({Key.Size} bits)";
            }
        }
    }
}
