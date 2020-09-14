using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmRsaGen : Form
    {
        private static readonly KeyInfo[] KeySizes = new KeyInfo[]
        {
            new KeyInfo(1024 * 1, "Legacy"),
            new KeyInfo(1024 * 2, "Low Security"),
            new KeyInfo(1024 * 3, "Medium Security"),
            new KeyInfo(1024 * 4, "High Security")
        }
        .OrderByDescending(m => m.Size)
            .ToArray();

        public int KeySize { get; private set; }

        public string KeyName { get; private set; }

        public frmRsaGen()
        {
            InitializeComponent();
            var config = AppSettings.GlobalSettings();
            if (config != null && config.Restrictions != null)
            {
                //Add only values at or above the minimum value
                cbSize.Items.AddRange(KeySizes.Where(m => m.Size >= config.Restrictions.MinimumRsaSize).Cast<object>().ToArray());
            }
            else
            {
                //Add all keys
                cbSize.Items.AddRange(KeySizes.Cast<object>().ToArray());
            }
            cbSize.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            KeySize = ((KeyInfo)cbSize.SelectedItem).Size;
            KeyName = tbName.Text;
        }

        private struct KeyInfo
        {
            public int Size;
            public string Label;

            public KeyInfo(int Size, string Label)
            {
                this.Size = Size;
                this.Label = Label;
            }

            public override string ToString()
            {
                return $"{Label} ({Size} bits)";
            }
        }
    }
}
