using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace CryptoPad
{
    [Serializable]
    public class RSAKey
    {
        private const int BASE_HASHCODE = 0x6EDC1C5E;

        public string Name { get; set; }

        public RSAParameters Key { get; set; }

        [XmlIgnore]
        public int Size
        {
            get
            {
                return Key.Modulus.Length * 8;
            }
        }

        public RSAKey()
        {
            //Empty key
        }

        public RSAKey(string Name)
        {
            this.Name = Name;
        }

        public RSAKey(RSAParameters Params)
        {
            Key = Params;
        }

        public RSAKey(string Name, RSAParameters Params)
        {
            this.Name = Name;
            Key = Params;
        }

        public bool IsSamePublicKey(RSAKey K)
        {
            return IsSamePublicKey(K.Key);
        }

        public bool IsSamePublicKey(RSAParameters K)
        {
            return Key.Modulus.SequenceEqual(K.Modulus)
                && Key.Exponent.SequenceEqual(K.Exponent);
        }

        public bool IsValid()
        {
            return RSAEncryption.HasPrivateKey(Key) || RSAEncryption.HasPublicKey(Key);
        }

        public override int GetHashCode()
        {
            //Hash code of the name
            var N = string.IsNullOrEmpty(Name) ? 0 : Name.GetHashCode();
            //Base hash code plus name hash code
            var CodeSum = BASE_HASHCODE ^ N;
            int Shifter = 0;
            //Iterate over all Fields. Make sure they're always in a pretictable order
            foreach (var F in typeof(RSAParameters).GetFields().OrderBy(m => m.Name))
            {
                //Only care about byte arrays
                if (F.FieldType == typeof(byte[]))
                {
                    var o = F.GetValue(Key);
                    if (o != null)
                    {
                        var a = (byte[])o;
                        for (var i = 0; i < a.Length; i++)
                        {
                            //Because the values are bytes, they would not add a lot of entropy.
                            //We shift them so they change different parts of the hash code
                            //This also increases the chance that two swapped values will yield a different code.
                            CodeSum ^= a[i] << (Shifter++ % 24);
                        }
                    }
                }
            }
            return CodeSum;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(RSAKey))
            {
                var o = (RSAKey)obj;
                return o == this || (Name == o.Name && RSAEncryption.Compare(Key, o.Key));
            }
            return base.Equals(obj);
        }
    }
}
