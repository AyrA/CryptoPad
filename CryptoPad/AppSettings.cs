using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CryptoPad
{
    [Serializable]
    public class AppSettings
    {
        [XmlIgnore]
        public string KeyStorage { get; private set; }

        public Size WindowSize { get; set; }

        public FormWindowState WindowStartupState { get; set; }

        public ColorCode EditorForegroundColor { get; set; }

        public ColorCode EditorBackgroundColor { get; set; }

        public string FontName { get; set; }
        public float FontSize { get; set; }
        public FontStyle FontStyle { get; set; }

        public static string PortableSettingsFile
        {
            get
            {
                var Module = Assembly.GetExecutingAssembly().Modules.First().FullyQualifiedName;
                return Path.Combine(Path.GetDirectoryName(Module), "settings.xml");
            }
        }

        public static string GlobalSettingsFile
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%ProgramData%\CryptoPad\settings.xml");
            }
        }

        public static string UserSettingsFile
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%APPDATA%\CryptoPad\settings.xml");
            }
        }

        public AppSettings()
        {
            WindowStartupState = FormWindowState.Normal;
            WindowSize = new Size(600, 600);

            EditorForegroundColor = new ColorCode(Color.FromKnownColor(KnownColor.WindowText).Name);
            EditorBackgroundColor = new ColorCode(Color.FromKnownColor(KnownColor.Window).Name);

            SetFont(SystemFonts.DefaultFont);
        }

        public RSAParameters[] LoadRSAKeys()
        {
            var Params = new List<RSAParameters>();
            if (Directory.Exists(KeyStorage))
            {
                foreach (var F in Directory.GetFiles(KeyStorage, "*.xml"))
                {
                    try
                    {
                        Params.Add(Tools.FromXML<RSAParameters>(File.ReadAllText(F)));
                    }
                    catch
                    {
                        //Invalid key maybe?
                        try
                        {
                            File.Move(F, Path.ChangeExtension(F, ".invalid"));
                        }
                        catch
                        {
                            //Can't rename it either. Just skip
                        }
                    }
                }
            }
            return Params.ToArray();
        }

        public void SaveRSAKeys(IEnumerable<RSAParameters> Keys, bool Purge = false)
        {
            if (!Directory.Exists(KeyStorage))
            {
                Directory.CreateDirectory(KeyStorage);
            }
            if (Purge)
            {
                foreach (var F in Directory.GetFiles(KeyStorage, "*.xml"))
                {
                    try
                    {
                        File.Delete(F);
                    }
                    catch
                    {
                        //Don't care
                        //We can't abort because it would leave a partially deleted directory
                    }
                }
                SaveRSAKeys(Keys, false);
            }
            else
            {
                var ExistingKeys = new List<RSAParameters>(LoadRSAKeys());
                foreach (var Key in Keys)
                {
                    if (!ExistingKeys.Any(m => RSAEncryption.Compare(m, Key)))
                    {
                        ExistingKeys.Add(Key);
                    }
                }
                foreach (var K in ExistingKeys)
                {
                    var Data = K.ToXML().Trim();
                    File.WriteAllText(Path.Combine(KeyStorage, Encryption.HashSHA256(Data) + ".xml"), Data);
                }
            }
        }

        public Font GetFont()
        {
            return new Font(FontName, FontSize, FontStyle);
        }

        public Font SetFont(Font F)
        {
            FontName = F.Name;
            FontSize = F.Size;
            FontStyle = F.Style;
            return F;
        }

        public static AppSettings GetSettings()
        {
            AppSettings ASGlobal = null;
            AppSettings ASLocal = null;

            try
            {
                //Portable settings win over all others
                var Settings = Tools.FromXML<AppSettings>(File.ReadAllText(PortableSettingsFile));
                Settings.KeyStorage = Path.Combine(Path.GetDirectoryName(PortableSettingsFile), "Keys");
                return Settings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize portable file: {ex.Message}");
            }

            //Read global and local settings
            try
            {
                ASGlobal = Tools.FromXML<AppSettings>(File.ReadAllText(GlobalSettingsFile));
                ASGlobal.KeyStorage = Path.Combine(Path.GetDirectoryName(GlobalSettingsFile), "Keys");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize global settings file: {ex.Message}");
            }
            try
            {
                ASLocal = Tools.FromXML<AppSettings>(File.ReadAllText(UserSettingsFile));
                ASLocal.KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize user settings file: {ex.Message}");
            }
            //Return local settings if present
            if (ASGlobal == null)
            {
                if (ASLocal == null)
                {
                    return new AppSettings()
                    {
                        KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys")
                    };
                }
                return ASLocal;
            }
            return ASGlobal;
        }

        public void SaveSettings()
        {
            var Data = Tools.ToXML(this);
            if (File.Exists(PortableSettingsFile))
            {
                File.WriteAllText(PortableSettingsFile, Data);
                KeyStorage = Path.Combine(Path.GetDirectoryName(PortableSettingsFile), "Keys");
            }
            var DirName = Path.GetDirectoryName(UserSettingsFile);
            try
            {
                Directory.CreateDirectory(DirName);
            }
            catch
            {
                //Don't care
            }
            File.WriteAllText(UserSettingsFile, Data);
            KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys");
        }
    }

    [Serializable]
    public class ColorCode
    {
        private string _name;
        private int _value;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _value = Color.FromName(value).ToArgb();
                }
                _name = value;
            }
        }

        [XmlAttribute]
        public int Value
        {
            get { return _value; }
            set
            {
                var v = value | (0xFF << 24);
                if (v != _value)
                {
                    _name = FindColorName(v);
                    _value = v;
                }
            }
        }

        public ColorCode() : this(Color.Transparent)
        {
            //NOOP
        }

        public ColorCode(string Name)
        {
            this.Name = Name;
            Value = Color.FromName(Name).ToArgb();
        }

        public ColorCode(int Argb)
        {
            Name = null;
            Value = Argb;
        }

        public ColorCode(Color ExistingColor)
        {
            Name = ExistingColor.IsNamedColor ? ExistingColor.Name : null;
            Value = ExistingColor.ToArgb();
        }

        public Color GetColor()
        {
            return string.IsNullOrEmpty(Name) ? Color.FromArgb(Value) : Color.FromName(Name);
        }

        public static string FindColorName(int Value)
        {
            foreach (var name in Enum.GetValues(typeof(KnownColor)))
            {
                var c = Color.FromKnownColor((KnownColor)name);
                if (c.ToArgb() == Value)
                {
                    return c.Name;
                }
            }
            return null;
        }

        public override string ToString()
        {
            var c = GetColor();
            var n = c.IsNamedColor ? c.Name : $"{c.A},{c.R},{c.G},{c.B}";
            return $"Color: " + n;
        }
    }
}
