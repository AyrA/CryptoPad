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
#if DEBUG
            var Settings = AppSettings.GetSettings();
            Settings.Restrictions = new Restrictions()
            {
                AutoRsaKeys = AppSettings.GetAdministrativeKeys(),
                BlockedModes = new CryptoMode[] { CryptoMode.Keyfile, CryptoMode.CryptMachine },
                BlockPortable = true,
                MinimumRsaSize = 2048
            };
            Settings.SaveSettings(SettingsType.Global);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        /// <summary>
        /// Gets the appropriate buttons for a message box
        /// </summary>
        /// <param name="Ask">true=Yes+No,false=OK</param>
        /// <param name="Cancel">true=+Cancel</param>
        /// <returns>Button enumeration value</returns>
        private static MessageBoxButtons GetButton(bool Ask, bool Cancel)
        {
            //Default is OK only
            var btn = MessageBoxButtons.OK;
            if (Ask)
            {
                //Use Yes+No option
                btn = MessageBoxButtons.YesNo;
                if (Cancel)
                {
                    //Add cancel button
                    btn = MessageBoxButtons.YesNoCancel;
                }
            }
            else if (Cancel)
            {
                //OK+Cancel
                btn = MessageBoxButtons.OKCancel;
            }
            return btn;
        }

        /// <summary>
        /// Shows an error dialog (red cross)
        /// </summary>
        /// <param name="Msg">Text</param>
        /// <param name="Ask">true=Yes+No,false=OK</param>
        /// <param name="Cancel">true=+Cancel</param>
        /// <returns>Message box result</returns>
        public static DialogResult ErrorMsg(string Msg, bool Ask = false, bool Cancel = false)
        {
            return MessageBox.Show(Msg, "Error", GetButton(Ask, Cancel), MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a warning dialog (Yellow exclamation mark)
        /// </summary>
        /// <param name="Msg">Text</param>
        /// <param name="Ask">true=Yes+No,false=OK</param>
        /// <param name="Cancel">true=+Cancel</param>
        /// <returns>Message box result</returns>
        public static DialogResult AlertMsg(string Msg, bool Ask = false, bool Cancel = false)
        {
            return MessageBox.Show(Msg, "Caution", GetButton(Ask, Cancel), MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Shows an info dialog (blue info)
        /// </summary>
        /// <param name="Msg">Text</param>
        /// <param name="Ask">true=Yes+No,false=OK</param>
        /// <param name="Cancel">true=+Cancel</param>
        /// <returns>Message box result</returns>
        public static DialogResult InfoMsg(string Msg, bool Ask = false, bool Cancel = false)
        {
            return MessageBox.Show(Msg, "Information", GetButton(Ask, Cancel), MessageBoxIcon.Information);
        }
    }
}
