using System.Linq;
using System.Security.Cryptography;

namespace CryptoPad
{
    /// <summary>
    /// Provides cryptographic oprtations for RSA
    /// </summary>
    public static class RSAEncryption
    {
        /// <summary>
        /// Default key size for new keys
        /// </summary>
        public const int DEFAULT_KEYSIZE = 4096;

        /// <summary>
        /// Checks if no item in the given argument list of byte arrays contains null values or empty arrays
        /// </summary>
        /// <param name="Data">Any number of byte arrays</param>
        /// <returns>true, if no argument is null or empty</returns>
        private static bool NotEmpty(params byte[][] Data)
        {
            return Data != null && !Data.Any(m => m == null || m.Length == 0);
        }

        /// <summary>
        /// Checks if the supplied RSA object has private key parts (can sign and decrypt)
        /// </summary>
        /// <param name="Param">RSA parameters</param>
        /// <returns>true, if private key present</returns>
        public static bool HasPrivateKey(RSAParameters Param)
        {
            return NotEmpty(Param.D, Param.DP, Param.DQ, Param.InverseQ, Param.P, Param.Q);
        }

        /// <summary>
        /// Checks if the supplied RSA object has public key parts (can verify and encrypt)
        /// </summary>
        /// <param name="Param">RSA parameters</param>
        /// <returns>true, if public key present</returns>
        public static bool HasPublicKey(RSAParameters Param)
        {
            return NotEmpty(Param.Exponent, Param.Modulus);
        }

        /// <summary>
        /// Compares two RSA keys
        /// </summary>
        /// <param name="Key1">RSA key</param>
        /// <param name="Key2">RSA key</param>
        /// <returns>true, if considered identical</returns>
        public static bool Compare(RSAParameters Key1, RSAParameters Key2)
        {
            //Check if public keys are available
            if (HasPublicKey(Key1) || HasPublicKey(Key2))
            {
                //Check if both keys have a public key
                if (HasPublicKey(Key1) && HasPublicKey(Key2))
                {
                    if (
                        Key1.Exponent.Length != Key2.Exponent.Length ||
                        Key1.Modulus.Length != Key2.Modulus.Length ||
                        !Key1.Exponent.SequenceEqual(Key2.Exponent) ||
                        !Key1.Modulus.SequenceEqual(Key2.Modulus))
                    {
                        //Values differ
                        return false;
                    }
                }
                else
                {
                    //Public key missing from one of them
                    return false;
                }
            }
            //Check if private keys are available
            if (HasPrivateKey(Key1) || HasPrivateKey(Key2))
            {
                //Check if both keys have a private key
                if (HasPrivateKey(Key1) && HasPrivateKey(Key2))
                {
                    if (
                        Key1.D.Length != Key2.D.Length ||
                        Key1.DP.Length != Key2.DP.Length ||
                        Key1.DQ.Length != Key2.DQ.Length ||
                        Key1.InverseQ.Length != Key2.InverseQ.Length ||
                        Key1.P.Length != Key2.P.Length ||
                        Key1.Q.Length != Key2.Q.Length ||
                        !Key1.D.SequenceEqual(Key2.D) ||
                        !Key1.DP.SequenceEqual(Key2.DP) ||
                        !Key1.DQ.SequenceEqual(Key2.DQ) ||
                        !Key1.InverseQ.SequenceEqual(Key2.InverseQ) ||
                        !Key1.P.SequenceEqual(Key2.P) ||
                        !Key1.Q.SequenceEqual(Key2.Q))
                    {
                        //Values differ
                        return false;
                    }
                }
                else
                {
                    //Private key missing from one of them
                    return false;
                }
            }
            //Keys are identical (both empty or same values)
            return true;
        }

        /// <summary>
        /// Generates a new RSA key
        /// </summary>
        /// <param name="Name">Key name</param>
        /// <param name="Size">Key size, defaults to <see cref="DEFAULT_KEYSIZE"/></param>
        /// <returns></returns>
        public static RSAKey GenerateKey(string Name, int Size = DEFAULT_KEYSIZE)
        {
            using (var Alg = new RSACryptoServiceProvider(Size))
            {
                Alg.PersistKeyInCsp = false;
                return new RSAKey(Name, Alg.ExportParameters(true));
            }
        }

        /// <summary>
        /// Strips the private key information from a given key for exportingor publishing
        /// </summary>
        /// <param name="Key">Key to strip information of</param>
        /// <returns>New copy with public key parts only</returns>
        /// <remarks>The supplied key itself is not modified</remarks>
        public static RSAKey StripPrivate(RSAKey Key)
        {
            return new RSAKey(Key.Name, new RSAParameters()
            {
                Modulus = (byte[])Key.Key.Modulus.Clone(),
                Exponent = (byte[])Key.Key.Exponent.Clone()
            });
        }

        /// <summary>
        /// Encrypts the given data using the given key
        /// </summary>
        /// <param name="Key">RSA key with public key parts</param>
        /// <param name="Data">Data to encrypt</param>
        /// <returns>Encrypted data</returns>
        public static byte[] Encrypt(RSAKey Key, byte[] Data)
        {
            return Encrypt(Key.Key, Data);
        }

        /// <summary>
        /// Encrypts the given data using the given key
        /// </summary>
        /// <param name="Key">RSA key with public key parts</param>
        /// <param name="Data">Data to encrypt</param>
        /// <returns>Encrypted data</returns>
        public static byte[] Encrypt(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.Encrypt(Data, RSAEncryptionPadding.Pkcs1);
            }
        }

        /// <summary>
        /// Decrypts the given data using the given key
        /// </summary>
        /// <param name="Key">RSA key with private key parts</param>
        /// <param name="Data">Data to decrypt</param>
        /// <returns>Decrypted data</returns>
        public static byte[] Decrypt(RSAKey Key, byte[] Data)
        {
            return Decrypt(Key.Key, Data);
        }

        /// <summary>
        /// Decrypts the given data using the given key
        /// </summary>
        /// <param name="Key">RSA key with private key parts</param>
        /// <param name="Data">Data to decrypt</param>
        /// <returns>Decrypted data</returns>
        public static byte[] Decrypt(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.Decrypt(Data, RSAEncryptionPadding.Pkcs1);
            }
        }

        /// <summary>
        /// Signs data
        /// </summary>
        /// <param name="Key">RSA key with private key parts</param>
        /// <param name="Data">Data to sign</param>
        /// <returns>Signature</returns>
        public static byte[] Sign(RSAKey Key, byte[] Data)
        {
            return Sign(Key.Key, Data);
        }

        /// <summary>
        /// Signs data
        /// </summary>
        /// <param name="Key">RSA key with private key parts</param>
        /// <param name="Data">Data to sign</param>
        /// <returns>Signature</returns>
        public static byte[] Sign(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.SignData(Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        /// <summary>
        /// Verifies a signature
        /// </summary>
        /// <param name="Key">RSA key with public key parts</param>
        /// <param name="Data">Data to check the signature of</param>
        /// <param name="Signature">Expected signature</param>
        /// <returns>true, if signature matches</returns>
        public static bool Verify(RSAKey Key, byte[] Data, byte[] Signature)
        {
            return Verify(Key.Key, Data, Signature);
        }

        /// <summary>
        /// Verifies a signature
        /// </summary>
        /// <param name="Key">RSA key with public key parts</param>
        /// <param name="Data">Data to check the signature of</param>
        /// <param name="Signature">Expected signature</param>
        /// <returns>true, if signature matches</returns>
        public static bool Verify(RSAParameters Key, byte[] Data, byte[] Signature)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.VerifyData(Data, Signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
