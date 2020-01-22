using System;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace CryptoPad
{
    [Serializable]
    public class AesCryptoData
    {
        private const int SALT_SIZE = 32;
        private const int AES_BLOCKSIZE = 128;

        public byte[] IV { get; set; }
        public byte[] Mac { get; set; }
        public byte[] Salt { get; set; }
        public byte[] Data { get; set; }
        [XmlAttribute]
        public CipherMode Mode { get; set; }
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
