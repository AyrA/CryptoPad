using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CryptoPad
{
    public static class Tools
    {
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
