using System;
using System.Windows.Forms;

namespace CryptoPad
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        private static MessageBoxButtons GetButton(bool Ask, bool Cancel)
        {
            var btn = MessageBoxButtons.OK;
            if (Ask)
            {
                btn = MessageBoxButtons.YesNo;
                if (Cancel)
                {
                    btn = MessageBoxButtons.YesNoCancel;
                }
            }
            else if (Cancel)
            {
                btn = MessageBoxButtons.OKCancel;
            }
            return btn;
        }

        public static DialogResult ErrorMsg(string Msg, bool Ask = false, bool Cancel = false)
        {
            return MessageBox.Show(Msg, "Error", GetButton(Ask, Cancel), MessageBoxIcon.Error);
        }

        public static DialogResult AlertMsg(string Msg, bool Ask = false, bool Cancel = false)
        {
            return MessageBox.Show(Msg, "Caution", GetButton(Ask, Cancel), MessageBoxIcon.Exclamation);
        }

        public static DialogResult InfoMsg(string Msg, bool Ask = false, bool Cancel = false)
        {
            return MessageBox.Show(Msg, "Information", GetButton(Ask, Cancel), MessageBoxIcon.Information);
        }
    }
}
