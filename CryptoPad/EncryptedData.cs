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
        /// Gets all modes as a single <see cref="CryptoMode"/> flag
        /// </summary>
        /// <remarks>This property is automatically generated</remarks>
        public CryptoMode AllModes
        {
            get
            {
                return Providers == null ? 0 : (CryptoMode)Providers.Sum(m => (int)m.Mode);
            }
        }

        /// <summary>
        /// AES key used to encrypt <see cref="Data"/>
        /// </summary>
        /// <remarks>
        /// Never serialize or otherwise store this information.
        /// This is automatically set by functions in the <see cref="Encryption"/> class.
        /// </remarks>
        [XmlIgnore, NonSerialized]
        public byte[] AesKey;
        /// <summary>
        /// HMAC key used to authenticate <see cref="Data"/>.
        /// This is automatically set by functions in the <see cref="Encryption"/> class.
        /// </summary>
        /// <remarks>Never serialize or otherwise store this information</remarks>
        [XmlIgnore, NonSerialized]
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
    }
}
