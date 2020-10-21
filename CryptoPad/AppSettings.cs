using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CryptoPad
{
    /// <summary>
    /// Provides portable, local and global application settings
    /// </summary>
    [Serializable]
    public class AppSettings
    {
        /// <summary>
        /// Path that holds the keys
        /// </summary>
        [XmlIgnore]
        public string KeyStorage { get; private set; }

        /// <summary>
        /// Type of these settings
        /// </summary>
        [XmlIgnore]
        public SettingsType Type { get; private set; }

        /// <summary>
        /// Startup window size
        /// </summary>
        public Size WindowSize { get; set; }

        /// <summary>
        /// Startup window state
        /// </summary>
        public FormWindowState WindowStartupState { get; set; }

        /// <summary>
        /// Editor text color
        /// </summary>
        public ColorCode EditorForegroundColor { get; set; }

        /// <summary>
        /// Editor background color
        /// </summary>
        public ColorCode EditorBackgroundColor { get; set; }

        /// <summary>
        /// Editor font name
        /// </summary>
        public string FontName { get; set; }
        /// <summary>
        /// Editor font size
        /// </summary>
        public float FontSize { get; set; }
        /// <summary>
        /// Editor font properties
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Administrative restrictions
        /// </summary>
        /// <remarks>
        /// These are only ever saved in the global settings.
        /// When saving or loading, this field is stripped for all other modes
        /// </remarks>
        public Restrictions Restrictions { get; set; }

        /// <summary>
        /// Full file name of the portable settings file
        /// </summary>
        public static string PortableSettingsFile
        {
            get
            {
                var Module = Assembly.GetExecutingAssembly().Modules.First().FullyQualifiedName;
                return Path.Combine(Path.GetDirectoryName(Module), "settings.xml");
            }
        }

        /// <summary>
        /// Full file name of the global settings file
        /// </summary>
        public static string GlobalSettingsFile
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%ProgramData%\CryptoPad\settings.xml");
            }
        }

        /// <summary>
        /// Full file name of the user specific settings file
        /// </summary>
        public static string UserSettingsFile
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(@"%APPDATA%\CryptoPad\settings.xml");
            }
        }

        /// <summary>
        /// Initializes defaults
        /// </summary>
        public AppSettings()
        {
            WindowStartupState = FormWindowState.Normal;
            WindowSize = new Size(600, 600);

            EditorForegroundColor = new ColorCode(Color.FromKnownColor(KnownColor.WindowText).Name);
            EditorBackgroundColor = new ColorCode(Color.FromKnownColor(KnownColor.Window).Name);

            SetFont(SystemFonts.DefaultFont);

            Type = SettingsType.Local;
        }

        /// <summary>
        /// Loads RSA keys from the disk
        /// </summary>
        /// <returns>List of RSA keys</returns>
        public RSAKey[] LoadRSAKeys()
        {
            var Params = new List<RSAKey>();
            if (Directory.Exists(KeyStorage))
            {
                foreach (var F in Directory.GetFiles(KeyStorage, "*.xml"))
                {
                    try
                    {
                        var K = Tools.FromXML<RSAKey>(File.ReadAllText(F));
                        if (K.IsValid())
                        {
                            Params.Add(K);
                        }
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

        /// <summary>
        /// Saves RSA keys to disk
        /// </summary>
        /// <param name="Keys">RSA keys</param>
        /// <param name="Purge">Deletes all existing keys from the directory if set to <see cref="true"/></param>
        public void SaveRSAKeys(IEnumerable<RSAKey> Keys, bool Purge = false)
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
                        //We shouldn't abort because it would leave a partially deleted directory
                    }
                }
                SaveRSAKeys(Keys, false);
            }
            else
            {
                var ExistingKeys = new List<RSAKey>(LoadRSAKeys());
                foreach (var Key in Keys.Where(m => m.IsValid()))
                {
                    if (!ExistingKeys.Any(m => m.Equals(Key)))
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

        /// <summary>
        /// Gets a font object from the current configured values
        /// </summary>
        /// <returns>Font</returns>
        public Font GetFont()
        {
            return new Font(FontName, FontSize, FontStyle);
        }

        /// <summary>
        /// Sets font specific settings value to match the given font
        /// </summary>
        /// <param name="F">Font</param>
        /// <returns><paramref name="F"/></returns>
        public Font SetFont(Font F)
        {
            FontName = F.Name;
            FontSize = F.Size;
            FontStyle = F.Style;
            return F;
        }

        /// <summary>
        /// Gets the keys defined by the administrator
        /// </summary>
        /// <returns>Administrative Keys</returns>
        public static RSAKey[] GetAdministrativeKeys()
        {
            //Ignore administrative keys if we're running on portable settings.
            if (GetSettings().Type != SettingsType.Portable)
            {
                var GS = GlobalSettings();
                if (GS != null)
                {
                    return GS.LoadRSAKeys();
                }
            }
            return new RSAKey[0];
        }

        /// <summary>
        /// Loads settings from Disk
        /// </summary>
        /// <returns></returns>
        /// <remarks>Load order (first match wins): Portable, Local, Global, Defaults</remarks>
        public static AppSettings GetSettings()
        {
            AppSettings ASGlobal = null;
            AppSettings ASLocal = null;

            try
            {
                //Portable settings win over all others
                var Settings = Tools.FromXML<AppSettings>(File.ReadAllText(PortableSettingsFile));
                Settings.KeyStorage = Path.Combine(Path.GetDirectoryName(PortableSettingsFile), "Keys");
                Settings.Type = SettingsType.Portable;
                //Don't allow restrictions in portable mode
                Settings.Restrictions = null;
                return Settings;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize portable file at {PortableSettingsFile}: {ex.Message}");
            }

            //Read global and local settings
            try
            {
                ASGlobal = Tools.FromXML<AppSettings>(File.ReadAllText(GlobalSettingsFile));
                ASGlobal.KeyStorage = Path.Combine(Path.GetDirectoryName(GlobalSettingsFile), "Keys");
                ASGlobal.Type = SettingsType.Global;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize global settings file at {GlobalSettingsFile}: {ex.Message}");
            }
            try
            {
                ASLocal = Tools.FromXML<AppSettings>(File.ReadAllText(UserSettingsFile));
                ASLocal.KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys");
                ASLocal.Type = SettingsType.Local;
                //Ignore restrictions in local file and replace with global file
                ASLocal.Restrictions = ASGlobal?.Restrictions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to deserialize user settings file at {UserSettingsFile}: {ex.Message}");
            }
            //Return local settings if present
            if (ASLocal == null)
            {
                if (ASGlobal == null)
                {
                    Debug.WriteLine($"No settings present. Probably first run");
                    //Invent new local settings
                    return new AppSettings()
                    {
                        KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys"),
                        Type = SettingsType.Local
                    };
                }
                else if (ASGlobal.Restrictions == null)
                {
                    ASGlobal.Restrictions = new Restrictions();
                }
            }
            else
            {
                return ASLocal;
            }
            return ASGlobal;
        }

        /// <summary>
        /// Gets global settings (including the restriction object)
        /// </summary>
        /// <returns>Global settings, null if none found</returns>
        public static AppSettings GlobalSettings()
        {
            try
            {
                var ASGlobal = Tools.FromXML<AppSettings>(File.ReadAllText(GlobalSettingsFile));
                ASGlobal.KeyStorage = Path.Combine(Path.GetDirectoryName(GlobalSettingsFile), "Keys");
                ASGlobal.Type = SettingsType.Global;
                return ASGlobal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading global configuration. {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="Mode">mode to save settings as. Use zero to not change the mode</param>
        /// <returns>this instance</returns>
        public AppSettings SaveSettings(SettingsType Mode = 0)
        {
            var Data = Tools.ToXML(this);
            //Auto detect mode
            if (Mode == 0)
            {
                Restrictions = null;
                if (File.Exists(PortableSettingsFile))
                {
                    File.WriteAllText(PortableSettingsFile, Data);
                    Type = SettingsType.Portable;
                    KeyStorage = Path.Combine(Path.GetDirectoryName(PortableSettingsFile), "Keys");
                }
                else
                {
                    //Create settings directory
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
                    Type = SettingsType.Local;
                    KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys");
                }
            }
            else
            {
                switch (Mode)
                {
                    case SettingsType.Local:
                        Restrictions = null;
                        KeyStorage = Path.Combine(Path.GetDirectoryName(UserSettingsFile), "Keys");
                        File.WriteAllText(UserSettingsFile, Data);
                        break;
                    case SettingsType.Global:
                        KeyStorage = Path.Combine(Path.GetDirectoryName(GlobalSettingsFile), "Keys");
                        File.WriteAllText(GlobalSettingsFile, Data);
                        break;
                    case SettingsType.Portable:
                        Restrictions = null;
                        KeyStorage = Path.Combine(Path.GetDirectoryName(PortableSettingsFile), "Keys");
                        File.WriteAllText(PortableSettingsFile, Data);
                        break;
                    default:
                        throw new NotImplementedException($"The given {nameof(SettingsType)} value is invalid");
                }
                Type = Mode;
            }
            return this;
        }
    }

    /// <summary>
    /// Type of supported settings
    /// </summary>
    public enum SettingsType : int
    {
        /// <summary>
        /// Global settings file.
        /// This is the only file that preserves the <see cref="Restrictions"/> type
        /// </summary>
        Global = 1,
        /// <summary>
        /// Local settings in the user application data folder (supports roaming profiles)
        /// </summary>
        Local = 2,
        /// <summary>
        /// Portable settings in the directory that also holds the executable
        /// </summary>
        Portable = 3
    }

    /// <summary>
    /// Supplies restrictions
    /// </summary>
    /// <remarks>Only really usable for enterprise environments</remarks>
    [Serializable]
    public class Restrictions
    {
        /// <summary>
        /// Minimum (inclusive) RSA key size in bits
        /// </summary>
        /// <remarks>Keys below the size will be hidden from view but continue to work on existing files</remarks>
        public int MinimumRsaSize { get; set; }

        /// <summary>
        /// Disallowed modes for encrypting files
        /// </summary>
        /// <remarks>
        /// Methods listed here will not be available for user selection but continue to work on existing files
        /// </remarks>
        public CryptoMode[] BlockedModes { get; set; }

        /// <summary>
        /// Gets the combined allowed methods
        /// </summary>
        [XmlIgnore]
        public CryptoMode AllowedModes
        {
            get
            {
                var BaseMode = CryptoMode._ALL;
                var Blocked = BlockedModes;
                if (Blocked == null || Blocked.Length == 0)
                {
                    return CryptoMode._ALL;
                }
                foreach (var cm in Blocked)
                {
                    BaseMode ^= cm;
                }
                return BaseMode;
            }
        }

        /// <summary>
        /// Disallow conversion into portable version
        /// </summary>
        /// <remarks>
        /// This merely disables the option in the settings.
        /// Since this is open source software,
        /// anyone can rewrite their own version to ignore this.
        /// </remarks>
        public bool BlockPortable { get; set; }
        /// <summary>
        /// If set, will add these keys to newly encrypted files.
        /// </summary>
        /// <remarks>Has no effect on existing files</remarks>
        public RSAKey[] AutoRsaKeys { get; set; }

        public Restrictions()
        {
            AutoRsaKeys = new RSAKey[0];
            BlockedModes = new CryptoMode[0];
        }
    }

    /// <summary>
    /// Provides serializable color information that preserves the name (if any)
    /// </summary>
    [Serializable]
    public class ColorCode
    {
        /// <summary>
        /// Color name
        /// </summary>
        private string _name;
        /// <summary>
        /// Color value (Format: 0xAARRGGBB)
        /// </summary>
        /// <remarks>
        /// Because the alpha component is set to 0xFF for almost all colors,
        /// it's normal for this value to almost always be negative
        /// </remarks>
        private int _value;

        /// <summary>
        /// Gets or sets the name of the color
        /// </summary>
        /// <remarks>Setting the name will also change the value of <see cref="Value"/></remarks>
        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                //Try to set the value before setting the name in case it throws an exception
                if (!string.IsNullOrEmpty(value))
                {
                    _value = Color.FromName(value).ToArgb();
                }
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the color value
        /// </summary>
        /// <remarks>
        /// Tries to also set the <see cref="Name"/> appropriately.
        /// If not possible, will set it to <see cref="null"/>
        /// </remarks>
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

        /// <summary>
        /// Initializes this instance with full transparency
        /// </summary>
        public ColorCode() : this(Color.Transparent)
        {
            //NOOP
        }

        /// <summary>
        /// Initializes a color code from the given name
        /// </summary>
        /// <param name="Name"></param>
        public ColorCode(string Name)
        {
            this.Name = Name;
            Value = Color.FromName(Name).ToArgb();
        }

        /// <summary>
        /// Initializes a color code from the given ARGB value
        /// </summary>
        /// <param name="Argb">Color value. See <see cref="RGBToInt(int, int, int, int)"/> for conversion</param>
        public ColorCode(int Argb)
        {
            Name = null;
            Value = Argb;
        }

        /// <summary>
        /// Initializes a color from the given parameter
        /// </summary>
        /// <param name="ExistingColor">Color to use</param>
        public ColorCode(Color ExistingColor)
        {
            Name = ExistingColor.IsNamedColor ? ExistingColor.Name : null;
            Value = ExistingColor.ToArgb();
        }

        /// <summary>
        /// Converts this instance into a color
        /// </summary>
        /// <returns>Color</returns>
        public Color GetColor()
        {
            return string.IsNullOrEmpty(Name) ? Color.FromArgb(Value) : Color.FromName(Name);
        }

        /// <summary>
        /// Tries to find the name of the given ARGB value.
        /// </summary>
        /// <param name="Value">ARGB value. See <see cref="RGBToInt(int, int, int, int)"/> for conversion</param>
        /// <returns>Color name, <see cref="null"/> if none found</returns>
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

        /// <summary>
        /// Converts individual channel values into a single ARGB value used by Windows
        /// </summary>
        /// <param name="R">Red channel</param>
        /// <param name="G">Green channel</param>
        /// <param name="B">Blue channel</param>
        /// <param name="A">Alpha channel</param>
        /// <returns>Color value</returns>
        /// <remarks>All arguments must be in the range 0-255</remarks>
        public static int RGBToInt(int R, int G, int B, int A = 0xFF)
        {
            if (R > 0xFF || R < 0x00)
            {
                throw new ArgumentException($"Red value must be in the range of {0x00}-{0xFF}", nameof(R));
            }
            if (G > 0xFF || G < 0x00)
            {
                throw new ArgumentException($"Green value must be in the range of {0x00}-{0xFF}", nameof(G));
            }
            if (B > 0xFF || B < 0x00)
            {
                throw new ArgumentException($"Blue value must be in the range of {0x00}-{0xFF}", nameof(B));
            }
            if (A > 0xFF || A < 0x00)
            {
                throw new ArgumentException($"Alpha value must be in the range of {0x00}-{0xFF}", nameof(A));
            }
            return A << 24 | R << 16 | G << 8 | B;
        }

        /// <summary>
        /// Returns a string suitable for displaying
        /// </summary>
        /// <returns>Display string</returns>
        /// <remarks>Technically also suitable to compare</remarks>
        public override string ToString()
        {
            var c = GetColor();
            var n = c.IsNamedColor ? c.Name : $"{c.A},{c.R},{c.G},{c.B}";
            return $"Color: " + n;
        }

        /// <summary>
        /// Gets a value that can be used to compare instances
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            var code = string.IsNullOrEmpty(_name) ? 0 : _name.GetHashCode();
            code ^= _value;
            return code;
        }

        /// <summary>
        /// Checks if the given argument equals this instance
        /// </summary>
        /// <param name="obj">Object argument</param>
        /// <returns>true, if instance of the same type with the same values</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is ColorCode)
            {
                var o = (ColorCode)obj;
                return _value == o._value && _name == o._name;
            }
            return base.Equals(obj);
        }
    }
}
