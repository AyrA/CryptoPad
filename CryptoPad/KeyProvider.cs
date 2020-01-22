using System;
using System.Xml.Serialization;

namespace CryptoPad
{
    [Serializable]
    public class KeyProvider
    {
        [XmlAttribute]
        public CryptoMode Mode { get; set; }
        public AesCryptoData KeyData { get; set; }
    }
}
