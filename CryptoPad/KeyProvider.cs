using System;
using System.Xml.Serialization;

namespace CryptoPad
{
    /// <summary>
    /// Represents a key provider to encrypt/decrypt the real AES key
    /// </summary>
    [Serializable]
    public class KeyProvider
    {
        /// <summary>
        /// Type this provider is for
        /// </summary>
        [XmlAttribute]
        public CryptoMode Mode { get; set; }
        /// <summary>
        /// Provider specific data
        /// </summary>
        public AesCryptoData KeyData { get; set; }
    }
}
