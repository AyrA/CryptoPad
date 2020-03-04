using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace CryptoPad
{
    /// <summary>
    /// Represents a serializable RSA key
    /// </summary>
    [Serializable]
    public class RSAKey
    {
        /// <summary>
        /// Base hash code of this inscance.
        /// </summary>
        /// <remarks>
        /// Can be any value but it's recommended to not be zero or close to it.
        /// If set to zero, an empty instance will compare equal to (int)0
        /// </remarks>
        private const int BASE_HASHCODE = 0x6EDC1C5E;

        /// <summary>
        /// Key name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// RSA key
        /// </summary>
        public RSAParameters Key { get; set; }

        /// <summary>
        /// RSA key size
        /// </summary>
        [XmlIgnore]
        public int Size
        {
            get
            {
                return Key.Modulus.Length * 8;
            }
        }

        /// <summary>
        /// Creates an empty instance
        /// </summary>
        public RSAKey()
        {
            //Empty key
        }

        /// <summary>
        /// Creates a named instance
        /// </summary>
        /// <param name="Name">Key name</param>
        public RSAKey(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// Creates an instance with an RSA key
        /// </summary>
        /// <param name="Params">RSA key</param>
        public RSAKey(RSAParameters Params)
        {
            Key = Params;
        }

        /// <summary>
        /// Creates an instance with name and key
        /// </summary>
        /// <param name="Name">Key name</param>
        /// <param name="Params">RSA key</param>
        public RSAKey(string Name, RSAParameters Params)
        {
            this.Name = Name;
            Key = Params;
        }

        /// <summary>
        /// Checks if two given public keys are identical
        /// </summary>
        /// <param name="K">Key to compare with</param>
        /// <returns>true, if identical public keys</returns>
        public bool IsSamePublicKey(RSAKey K)
        {
            return IsSamePublicKey(K.Key);
        }

        /// <summary>
        /// Checks if two given public keys are identical
        /// </summary>
        /// <param name="K">Key to compare with</param>
        /// <returns>true, if identical public keys</returns>
        public bool IsSamePublicKey(RSAParameters K)
        {
            return Key.Modulus.SequenceEqual(K.Modulus)
                && Key.Exponent.SequenceEqual(K.Exponent);
        }

        /// <summary>
        /// Checks if the instance has a valid key
        /// </summary>
        /// <returns>true, if key is valid</returns>
        public bool IsValid()
        {
            //We currently don't allow keys that only have the private key parts set.
            //This means a key must either have the public part only, or both parts.
            //"x || (x && y)" can be simplified as just "x" due to the 4 possibilities collapsing:
            //true || (true && false)   --> true
            //true || (true && true)    --> true
            //false || (false && false) --> false
            //false || (false && true)  --> false
            return RSAEncryption.HasPublicKey(Key);
        }

        /// <summary>
        /// Calculates the hash code of the given instance
        /// </summary>
        /// <returns>Hash code</returns>
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

        /// <summary>
        /// Checks if two given instances are identical
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true, if identical</returns>
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
