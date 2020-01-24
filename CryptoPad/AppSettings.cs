using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CryptoPad
{
    [Serializable]
    public class AppSettings
    {
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
                return Tools.FromXML<AppSettings>(File.ReadAllText(PortableSettingsFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize portable file: {ex.Message}");
            }

            //Read global and local settings
            try
            {
                ASGlobal = Tools.FromXML<AppSettings>(File.ReadAllText(GlobalSettingsFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize global settings file: {ex.Message}");
            }
            try
            {
                ASLocal = Tools.FromXML<AppSettings>(File.ReadAllText(UserSettingsFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize user settings file: {ex.Message}");
            }
            //Return local settings if present
            if (ASGlobal == null)
            {
                return ASLocal == null ? new AppSettings() : ASLocal;
            }
            return ASGlobal;
        }

        public void SaveSettings()
        {
            var Data = Tools.ToXML(this);
            if (File.Exists(PortableSettingsFile))
            {
                File.WriteAllText(PortableSettingsFile, Data);
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
