using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmMain : Form
    {
        private string FileName;
        private string BaseContent;
        private Dictionary<CryptoMode, object> FileParams;
        private EncryptedData CurrentFile;
        private string StringToPrint;
        private AppSettings Settings;

        private bool HasChange
        {
            get
            {
                if (string.IsNullOrEmpty(BaseContent))
                {
                    return !string.IsNullOrEmpty(tbEditor.Text);
                }
                return tbEditor.Text != BaseContent;
            }
        }

        public frmMain()
        {
            InitializeComponent();

            Settings = AppSettings.GetSettings();

            WindowState = Settings.WindowStartupState;
            if (WindowState == FormWindowState.Normal)
            {
                Size = Settings.WindowSize;
            }

            tbEditor.Font = Settings.GetFont();
            try
            {
                tbEditor.ForeColor = Settings.EditorForegroundColor.GetColor();
            }
            catch
            {
                Settings.EditorForegroundColor = new ColorCode(tbEditor.ForeColor);
            }
            try
            {
                tbEditor.BackColor = Settings.EditorBackgroundColor.GetColor();
            }
            catch
            {
                Settings.EditorBackgroundColor = new ColorCode(tbEditor.BackColor);
            }

            FileParams = new Dictionary<CryptoMode, object>();

            //Copy icons over into the context menu
            cutToolStripMenuItem.Image = cutToolStripButton.Image;
            copyToolStripMenuItem.Image = copyToolStripButton.Image;
            pasteToolStripMenuItem.Image = pasteToolStripButton.Image;

            UpdateStatus();

            dlgFont.Apply += delegate
            {
                tbEditor.Font = dlgFont.Font;
            };
        }

        public void UpdateStatus()
        {
            var name = string.IsNullOrEmpty(FileName) ? "New File" : Path.GetFileName(FileName);
            var encStatus = "Not encrypted";
            if (HasChange)
            {
                name += "*";
            }
            if (CurrentFile != null)
            {
                encStatus = string.Join(", ", CurrentFile.Providers.Select(m => m.Mode));
            }
            Text = $"CryptoPad: [{name}]";
            tsStatusLabel.Text = name;
            tsEncryptionLabel.Text = encStatus;
            tsSizeLabel.Text = $"{tbEditor.Text.Length} UTF-8 characters";
            tsSizeLabel.ToolTipText = $"{Encoding.UTF8.GetByteCount(tbEditor.Text)} bytes";
        }

        private void NewText()
        {
            if (!HasChange || SaveText(false))
            {
                tbEditor.Text = FileName = BaseContent = string.Empty;
                FileParams.Clear();
                CurrentFile = null;
                UpdateStatus();
            }
        }

        private bool SaveText(bool SaveAs)
        {
            if ((!string.IsNullOrEmpty(FileName) && !SaveAs) || dlgSave.ShowDialog() == DialogResult.OK)
            {
                if (CurrentFile == null)
                {
                    using (var dlgCrypt = new frmCryptoModeSelect())
                    {
                        if (dlgCrypt.ShowDialog() == DialogResult.OK)
                        {
                            if (dlgCrypt.Modes == 0)
                            {
                                Program.ErrorMsg("Please select at least one mode of encryption");
                                return false;
                            }
                            var Params = new Dictionary<CryptoMode, object>();
                            if (dlgCrypt.Modes.HasFlag(CryptoMode.Password))
                            {
                                Params[CryptoMode.Password] = dlgCrypt.Password;
                            }
                            if (dlgCrypt.Modes.HasFlag(CryptoMode.Keyfile))
                            {
                                Params[CryptoMode.Keyfile] = dlgCrypt.Keyfile;
                            }

                            try
                            {
                                CurrentFile = Encryption.Encrypt(dlgCrypt.Modes, Encoding.UTF8.GetBytes(tbEditor.Text), Params);
                                FileParams = Params;
                            }
                            catch (Exception ex)
                            {
                                Program.ErrorMsg($"Unable to encrypt your file.\r\n{ex.Message}");
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Encryption.ReEncrypt(CurrentFile, Encoding.UTF8.GetBytes(tbEditor.Text));
                }
                if (SaveAs || string.IsNullOrEmpty(FileName))
                {
                    FileName = dlgSave.FileName;
                }
                try
                {
                    File.WriteAllText(FileName, CurrentFile.ToXML());
                }
                catch (Exception ex)
                {
                    Program.ErrorMsg($"Unable to save your file.\r\n{ex.Message}");
                    return false;
                }
                //Saved the current file
                BaseContent = tbEditor.Text;
                UpdateStatus();
                return true;
            }
            return false;
        }

        private void OpenText()
        {
            byte[] Data = null;
            EncryptedData TempFile = null;
            if (!HasChange || SaveText(false))
            {
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TempFile = Tools.FromXML<EncryptedData>(File.ReadAllText(dlgOpen.FileName));
                        try
                        {
                            Data = Encryption.Decrypt(TempFile);
                        }
                        catch
                        {
                            if (TempFile.HasProvider(CryptoMode.RSA))
                            {
                                //Try all RSA keys until one succeeds
                                foreach (var K in Settings.LoadRSAKeys().Where(m => RSAEncryption.HasPrivateKey(m.Key)))
                                {
                                    FileParams[CryptoMode.RSA] = K;
                                    try
                                    {
                                        Data = Encryption.Decrypt(TempFile, FileParams);
                                    }
                                    catch
                                    {
                                        //Try next key
                                    }
                                }
                                if (Data == null)
                                {
                                    FileParams.Remove(CryptoMode.RSA);
                                }
                            }
                            if (Data == null)
                            {
                                if (TempFile.HasProvider(CryptoMode.Keyfile) || TempFile.HasProvider(CryptoMode.Password))
                                {
                                    using (var pwd = new frmCryptoInput((CryptoMode)TempFile.Providers.Sum(m => (int)m.Mode), null))
                                    {
                                        if (pwd.ShowDialog() == DialogResult.OK)
                                        {
                                            if (pwd.ValidInput)
                                            {
                                                if (!string.IsNullOrEmpty(pwd.Password))
                                                {
                                                    FileParams[CryptoMode.Password] = pwd.Password;
                                                }
                                                if (!string.IsNullOrEmpty(pwd.Keyfile))
                                                {
                                                    if (File.Exists(pwd.Keyfile))
                                                    {
                                                        FileParams[CryptoMode.Password] = pwd.Keyfile;
                                                    }
                                                    else
                                                    {
                                                        Program.ErrorMsg("Invalid key file selected");
                                                    }
                                                }
                                                if (FileParams.Count > 0)
                                                {
                                                    try
                                                    {
                                                        Data = Encryption.Decrypt(TempFile, FileParams);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Program.ErrorMsg($"Unable to decrypt the file using the supplied data. Invalid key file or password?\r\n{ex.Message}");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Program.ErrorMsg("You need to provide at least one of the offered options to decrypt the file.");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Program.ErrorMsg("Failed to decrypt the data.");
                                }
                            }
                        }
                    }
                    catch
                    {
                        Program.ErrorMsg("Unable to open the specified file. It's not a valid encrypted text document");
                    }
                }
            }
            //Open the selected file, provided it could be decrypted
            if (Data != null)
            {
                FileName = dlgOpen.FileName;
                CurrentFile = TempFile;
                BaseContent = tbEditor.Text = Encoding.UTF8.GetString(Data);
                UpdateStatus();
            }
        }

        private void tbEditor_TextChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        tbEditor.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.C:
                        tbEditor.Copy();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.V:
                        tbEditor.Paste();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.X:
                        tbEditor.Cut();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.S:
                        SaveText(false);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.N:
                        NewText();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.O:
                        OpenText();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                }
            }
        }

        private void PrintTextFileHandler(object sender, PrintPageEventArgs e)
        {
            int charactersOnPage = 0;
            int linesPerPage = 0;

            if (string.IsNullOrEmpty(StringToPrint))
            {
                StringToPrint = tbEditor.Text.TrimEnd();
            }

            //Sets the value of charactersOnPage to the number of characters 
            //of StringToPrint that will fit within the bounds of the page.
            e.Graphics.MeasureString(StringToPrint, tbEditor.Font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            //Draws the string within the bounds of the page
            e.Graphics.DrawString(StringToPrint, tbEditor.Font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);

            //Remove the portion of the string that has been printed.
            StringToPrint = StringToPrint.Substring(charactersOnPage);

            //Check to see if more pages are to be printed.
            e.HasMorePages = (StringToPrint.Length > 0);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var canCancel =
                //e.CloseReason != CloseReason.TaskManagerClosing &&
                e.CloseReason != CloseReason.WindowsShutDown;
            var msg = "You have unsaved changed. Save before exit?";
            if (!canCancel)
            {
                msg += "\r\nCAUTION! Application exit was requested by the system. You can't cancel it.";
            }
            if (HasChange)
            {
                switch (Program.AlertMsg(msg, true, canCancel))
                {
                    case DialogResult.Yes:
                        if (!SaveText(false))
                        {
                            frmMain_FormClosing(sender, e);
                        }
                        break;
                    case DialogResult.No:
                        //No action
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void tsEncryptionLabel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                if (Program.InfoMsg("To set the encryption mode for a new file you have to save it first. Save now?", true) == DialogResult.Yes)
                {
                    SaveText(false);
                }
            }
            else
            {
                using (var dlgModes = new frmCryptoModeSelect())
                {
                    if (dlgModes.ShowDialog() == DialogResult.OK)
                    {
                        if (dlgModes.Modes == 0)
                        {
                            Program.ErrorMsg("Please select at least one mode of encryption");
                        }
                        else
                        {
                            var Params = new Dictionary<CryptoMode, object>();
                            if (dlgModes.Modes.HasFlag(CryptoMode.Password))
                            {
                                Params[CryptoMode.Password] = dlgModes.Password;
                            }
                            if (dlgModes.Modes.HasFlag(CryptoMode.Keyfile))
                            {
                                Params[CryptoMode.Keyfile] = dlgModes.Keyfile;
                            }
                            try
                            {
                                CurrentFile = Encryption.Encrypt(dlgModes.Modes, Encoding.UTF8.GetBytes(tbEditor.Text), Params);
                                if (Program.InfoMsg("The cryptographic modes were changed. Save the file now?", true) == DialogResult.Yes)
                                {
                                    SaveText(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Program.ErrorMsg($"Unable to encrypt your file.\r\n{ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        #region Menu

        private void ExitAction_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NewAction_Click(object sender, EventArgs e)
        {
            NewText();
        }

        private void OpenAction_Click(object sender, EventArgs e)
        {
            OpenText();
        }

        private void SaveAction_Click(object sender, EventArgs e)
        {
            SaveText(false);
        }

        private void SaveAsAction_Click(object sender, EventArgs e)
        {
            SaveText(true);
        }

        private void CopyAction_Click(object sender, EventArgs e)
        {
            tbEditor.Copy();
        }

        private void CutAction_Click(object sender, EventArgs e)
        {
            tbEditor.Cut();
        }

        private void PasteAction_Click(object sender, EventArgs e)
        {
            tbEditor.Paste();
        }

        private void SelectAllAction_Click(object sender, EventArgs e)
        {
            tbEditor.SelectAll();
        }

        private void CustomizeAction_Click(object sender, EventArgs e)
        {
            dlgFont.Font = tbEditor.Font;
            if (dlgFont.ShowDialog() == DialogResult.OK)
            {
                tbEditor.Font = dlgFont.Font;
            }
        }

        private void PrintAction_Click(object sender, EventArgs e)
        {
            using (var pd = new PrintDocument())
            {
                pd.PrintPage += PrintTextFileHandler;
                pd.DocumentName = Path.GetFileName(FileName);
                printDialog.Document = pd;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    pd.Print();
                }
            }
        }

        private void PrintPreviewAction_Click(object sender, EventArgs e)
        {
            using (var pd = new PrintDocument())
            {
                pd.PrintPage += PrintTextFileHandler;
                pd.DocumentName = Path.GetFileName(FileName);
                printPreview.Document = pd;
                printPreview.ShowDialog();
            }
        }

        private void HelpAction_Click(object sender, EventArgs e)
        {
            Program.InfoMsg("The help is not available yet. You can mostly use this application like a regular text editor.");
        }

        #endregion

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.WindowStartupState = WindowState;
            if (WindowState == FormWindowState.Normal)
            {
                Settings.WindowSize = Size;
            }

            Settings.SetFont(tbEditor.Font);
            Settings.EditorForegroundColor = new ColorCode(tbEditor.ForeColor);
            Settings.EditorBackgroundColor = new ColorCode(tbEditor.BackColor);

            try
            {
                Settings.SaveSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to save settings on exit: {ex.Message}");
            }
        }
    }
}
