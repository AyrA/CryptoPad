using System;
using System.Collections.Generic;
using System.Linq;
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
            var ObjList = AllKeys
                .Select(m => new KeyLabel() { Key = m })
                .OrderBy(m => m.Key.Name)
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
                    if (AllKeys[i] == PreSelected)
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
                    var Key = RSAEncryption.GenerateKey(F.KeyName, F.KeySize);
                    AllKeys.Add(Key);
                    InitList(Key);
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

        private struct KeyLabel
        {
            public RSAKey Key;

            public override string ToString()
            {
                return Key.Name;
            }
        }
    }
}
