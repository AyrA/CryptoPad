using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmRSASelect : Form
    {
        public RSAKey SelectedKey { get; private set; }

        public frmRSASelect(IEnumerable<RSAKey> Keys, bool CanCreate, RSAKey PreSelected = null)
        {
            if (Keys == null)
            {
                Keys = new RSAKey[0];
            }
            var KeyList = Keys.ToArray();
            var ObjList = KeyList
                .Select(m => new KeyLabel() { Key = m })
                .OrderBy(m => m.Key.Name)
                .Cast<object>()
                .ToArray();
            InitializeComponent();
            btnCreate.Enabled = CanCreate;
            cbKey.Items.AddRange(ObjList);
            if (ObjList.Length > 0)
            {
                cbKey.SelectedIndex = 0;
            }
            if (PreSelected != null)
            {
                for (var i = 0; i < KeyList.Length; i++)
                {
                    if (KeyList[i] == PreSelected)
                    {
                        cbKey.SelectedIndex = i;
                        break;
                    }
                }
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
