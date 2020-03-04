using System;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace CryptoPad
{
    /// <summary>
    /// Represents encrypted data
    /// </summary>
    [Serializable]
    public class AesCryptoData
    {
        /// <summary>
        /// Salt length in bytes
        /// </summary>
        private const int SALT_SIZE = 32;
        /// <summary>
        /// Block size of the cipher
        /// </summary>
        private const int AES_BLOCKSIZE = 128;

        /// <summary>
        /// Initialization vector
        /// </summary>
        public byte[] IV { get; set; }
        /// <summary>
        /// HMAC to authenticate encrypted data
        /// </summary>
        public byte[] Mac { get; set; }
        /// <summary>
        /// Salt (key) for HMAC
        /// </summary>
        public byte[] Salt { get; set; }
        /// <summary>
        /// Encrypted data
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// Mode of encryption
        /// </summary>
        [XmlAttribute]
        public CipherMode Mode { get; set; }
        /// <summary>
        /// Encryption padding type
        /// </summary>
        [XmlAttribute]
        public PaddingMode Padding { get; set; }

        /// <summary>
        /// Creates a CryptoData with defaults
        /// </summary>
        public AesCryptoData()
        {
            IV = Encryption.GetRandomData(AES_BLOCKSIZE / 8);
            Salt = Encryption.GetRandomData(SALT_SIZE);
            Mode = CipherMode.CBC;
            Padding = PaddingMode.PKCS7;
        }
    }
}
