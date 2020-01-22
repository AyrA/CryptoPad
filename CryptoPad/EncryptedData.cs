using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CryptoPad
{
    /// <summary>
    /// Contains encrypted data and providers to decrypt
    /// </summary>
    [Serializable]
    public class EncryptedData
    {
        /// <summary>
        /// Registered providers and the data necessary to decrypt
        /// </summary>
        /// <remarks>
        /// Providers can be deleted at any time.
        /// Providers can manually be added without changing existing providers.
        /// Providers can be changed without changing other providers.
        /// </remarks>
        public KeyProvider[] Providers { get; set; }
        /// <summary>
        /// Encrypted data
        /// </summary>
        public AesCryptoData Data { get; set; }

        /// <summary>
        /// AES key used to encrypt <see cref="Data"/>
        /// </summary>
        /// <remarks>
        /// Never serialize or otherwise store this information.
        /// This is automatically set by functions in the <see cref="Encryption"/> class.
        /// </remarks>
        [NonSerialized]
        public byte[] AesKey;
        /// <summary>
        /// HMAC key used to authenticate <see cref="Data"/>.
        /// This is automatically set by functions in the <see cref="Encryption"/> class.
        /// </summary>
        /// <remarks>Never serialize or otherwise store this information</remarks>
        [NonSerialized]
        public byte[] MacKey;

        /// <summary>
        /// Checks if the given provider is registered
        /// </summary>
        /// <param name="M">Single provider</param>
        /// <returns>true, if the provider exists</returns>
        public bool HasProvider(CryptoMode M)
        {
            return Providers.Any(m => m.Mode == M);
        }

        /// <summary>
        /// Serializes this instance into an XML string
        /// </summary>
        /// <returns>XML string</returns>
        public string ToXML()
        {
            using (var MS = new MemoryStream())
            {
                XmlSerializer S = new XmlSerializer(typeof(EncryptedData));
                S.Serialize(MS, this);
                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }

        /// <summary>
        /// Deserializes an object from an XML string
        /// </summary>
        /// <param name="Data">XML string</param>
        /// <returns>Deserialized object</returns>
        public static EncryptedData FromXML(string Data)
        {
            using (var MS = new MemoryStream(Encoding.UTF8.GetBytes(Data), false))
            {
                XmlSerializer S = new XmlSerializer(typeof(EncryptedData));
                return (EncryptedData)S.Deserialize(MS);
            }
        }
    }
}
