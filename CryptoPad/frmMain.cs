using System;
using System.Collections.Generic;
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
            tsStatusLabel.Text = string.IsNullOrEmpty(FileName) ? "New File" : Path.GetFileName(FileName);
            tsEncryptionLabel.Text = CurrentFile == null ? "Not Encrypted" : string.Join(", ", CurrentFile.Providers.Select(m => m.Mode));
            tsSizeLabel.Text = $"{tbEditor.Text.Length} chars";
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
                    //TODO:Show encryption dialog (next line is for testing only right now)
                    CurrentFile = Encryption.Encrypt(CryptoMode.CryptMachine | CryptoMode.CryptUser, Encoding.UTF8.GetBytes(tbEditor.Text));
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
                //Save the current file
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
                        TempFile = EncryptedData.FromXML(File.ReadAllText(dlgOpen.FileName));
                        try
                        {
                            Data = Encryption.Decrypt(TempFile);
                        }
                        catch
                        {
                            if (TempFile.HasProvider(CryptoMode.Keyfile) || TempFile.HasProvider(CryptoMode.Password))
                            {
                                using (var pwd = new frmCryptoInput((CryptoMode)TempFile.Providers.Sum(m => (int)m.Mode)))
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
                                                    Data = Encryption.Decrypt(TempFile);
                                                }
                                                catch
                                                {
                                                    Program.ErrorMsg("Unable to decrypt the file using the supplied data. Invalid key file or password?");
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
                tbEditor.Text = Encoding.UTF8.GetString(Data);
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

        #region Menu

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

        #endregion
    }
}
