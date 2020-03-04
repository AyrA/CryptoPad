using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CryptoPad
{
    /// <summary>
    /// Utility functions
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Converts an enum made of multiple values into individual values
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="Values">Enum values</param>
        /// <returns>Seperated values</returns>
        public static T[] FlagsToArray<T>(T Values) where T : struct
        {
            var VarType = typeof(T);
            var Val = IntVal(Values);
            if (VarType.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
            {
                throw new Exception($"{VarType.Name} is not an enumeration with the Flags attribute");
            }
            return Enum.GetValues(VarType).OfType<T>().Where(m => (IntVal(m) & Val) == IntVal(m)).ToArray();
        }

        /// <summary>
        /// Removes invalid characters from a file name
        /// </summary>
        /// <param name="Filename">File name</param>
        /// <param name="Replacement">Replacement character</param>
        /// <returns>Sanitized file name</returns>
        public static string SanitizeName(string Filename, char Replacement = '_')
        {
            //Handle missing/invalid names
            if (string.IsNullOrEmpty(Filename) || Filename == "." || Filename == "..")
            {
                return Replacement.ToString();
            }
            var Invalids =
                //Disallow known bad characters
                Path.GetInvalidFileNameChars()
                //Disallow DEL char
                .Concat("\x7F".ToCharArray())
                //Disallow control characters
                .Concat(Enumerable.Range(0, 0x1F).Select(n => (char)n))
                .ToArray();
            //Run through the file name string only once
            var Name = Filename
                .Select(m => Invalids.Contains(m) ? Replacement : m)
                .ToArray();
            return new string(Name);
        }

        /// <summary>
        /// Combines a file name and path information into a unique name
        /// </summary>
        /// <param name="Directory">Base directory</param>
        /// <param name="Filename">Base file name</param>
        /// <returns>Unique name</returns>
        /// <remarks>
        /// - Tries to use Directory+Filename first as-is.
        /// - Tries to make a name unique by adding numbers.
        /// - Does not guarantee that the name will stay unique.
        /// </remarks>
        public static string UniqueName(string Directory, string Filename)
        {
            string Basename = Path.Combine(Directory, Filename);
            int ctr = 0;
            string ext = Path.GetExtension(Filename);
            string nameonly = Path.GetFileNameWithoutExtension(Filename);
            while (File.Exists(Basename))
            {
                ++ctr;
                Basename = Path.Combine(Directory, $"{nameonly}_{ctr}{ext}");
            }
            return Basename;
        }

        /// <summary>
        /// Tries to cast an enum of unknown type to an integer
        /// </summary>
        /// <param name="someEnum">Enum value</param>
        /// <returns>Integer value</returns>
        /// <remarks>
        /// Will throw if the supplied value is not an enumeration or otherwise can be casted to an integer
        /// </remarks>
        private static int IntVal(object someEnum)
        {
            if (someEnum == null)
            {
                throw new ArgumentNullException(nameof(someEnum));
            }
            try
            {
                return (int)(long)someEnum;
            }
            catch { }
            try
            {
                return (int)someEnum;
            }
            catch { }
            try
            {
                return (short)someEnum;
            }
            catch { }
            try
            {
                return (int)(ulong)someEnum;
            }
            catch { }
            try
            {
                return (int)(uint)someEnum;
            }
            catch { }
            try
            {
                return (ushort)someEnum;
            }
            catch { }
            try
            {
                return (byte)someEnum;
            }
            catch { }
            throw new ArgumentException("Value can't be converted to an integer");
        }

        /// <summary>
        /// Serializes this instance into an XML string
        /// </summary>
        /// <returns>XML string</returns>
        public static string ToXML<T>(this T ObjectToSerialize)
        {
            using (var MS = new MemoryStream())
            {
                XmlSerializer S = new XmlSerializer(typeof(T));
                S.Serialize(MS, ObjectToSerialize);
                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }

        /// <summary>
        /// Deserializes an object from an XML string
        /// </summary>
        /// <param name="Data">XML string</param>
        /// <returns>Deserialized object</returns>
        public static T FromXML<T>(string Data)
        {
            using (var MS = new MemoryStream(Encoding.UTF8.GetBytes(Data), false))
            {
                XmlSerializer S = new XmlSerializer(typeof(T));
                return (T)S.Deserialize(MS);
            }
        }
    }
}
