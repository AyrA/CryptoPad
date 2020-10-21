using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoPad
{
    /// <summary>
    /// Provides encryption of data with multiple providers
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// Size in bytes for the salt value
        /// </summary>
        private const int SALT_SIZE = 32;
#if DEBUG
        /// <summary>
        /// Low iteration count for debug mode
        /// </summary>
        private const int PBKDF_ITERATIONS = 10000;
#else
        /// <summary>
        /// High iteration count for release mode
        /// </summary>
        private const int PBKDF_ITERATIONS = 50000;
#endif
        /// <summary>
        /// Size in bits of an AES key
        /// </summary>
        private const int AES_KEYSIZE = 256;
        /// <summary>
        /// Size in bits of an AES IV (and essentially the block size)
        /// </summary>
        private const int AES_IVSIZE = 128;

        #region Random Data Generators

        /// <summary>
        /// Generates cryptographically safe random bytes
        /// </summary>
        /// <param name="Count">Number of bytes</param>
        /// <returns>Random bytes</returns>
        public static byte[] GetRandomData(int Count)
        {
            using (var R = RandomNumberGenerator.Create())
            {
                var Data = new byte[Count];
                R.GetBytes(Data);
                return Data;
            }
        }

        /// <summary>
        /// Generates random bytes of the required length to be used as AES IV
        /// </summary>
        /// <returns>Random AES IV</returns>
        public static byte[] GetRandomIV()
        {
            return GetRandomData(AES_IVSIZE / 8);
        }

        /// <summary>
        /// Generates random bytes of the required length to be used as AES key
        /// </summary>
        /// <returns>Random AES key</returns>
        public static byte[] GetRandomKey()
        {
            return GetRandomData(AES_KEYSIZE / 8);
        }

        #endregion

        #region Crypto Internals

        /// <summary>
        /// Encrypts data using the user or machine account
        /// </summary>
        /// <param name="LocalMachine">Use machine account</param>
        /// <param name="Data">Data</param>
        /// <returns>Encrypted data</returns>
        private static byte[] ProtectData(bool LocalMachine, byte[] Data)
        {
            return ProtectedData.Protect(Data, null, LocalMachine ? DataProtectionScope.LocalMachine : DataProtectionScope.CurrentUser);
        }

        /// <summary>
        /// Decrypts data using the user or machine account
        /// </summary>
        /// <param name="LocalMachine">Use machine account</param>
        /// <param name="Data">Data</param>
        /// <returns>Encrypted data</returns>
        private static byte[] UnprotectData(bool LocalMachine, byte[] Data)
        {
            return ProtectedData.Unprotect(Data, null, LocalMachine ? DataProtectionScope.LocalMachine : DataProtectionScope.CurrentUser);
        }

        /// <summary>
        /// Computes a MAC
        /// </summary>
        /// <param name="Data">Data to hash</param>
        /// <param name="Salt">Secret key</param>
        /// <returns>MAC</returns>
        private static byte[] MAC(byte[] Data, byte[] Salt)
        {
            using (var Hasher = new HMACSHA256(Salt))
            {
                return Hasher.ComputeHash(Data);
            }
        }

        /// <summary>
        /// Encrypts data with a provided AES key and MAC key
        /// </summary>
        /// <param name="Data">Data to encrypt</param>
        /// <param name="AesKey">AES key (must match <see cref="Aes.KeySize"/>)</param>
        /// <param name="MacKey">MAC key (should not be the same as the AES key)</param>
        /// <returns>Encrypted and hashed data</returns>
        private static AesCryptoData EncryptWithKey(byte[] Data, byte[] AesKey, byte[] MacKey)
        {
            var CD = new AesCryptoData();
            using (var AesEncryptor = new AesManaged())
            {
                AesEncryptor.BlockSize = CD.IV.Length * 8;
                AesEncryptor.KeySize = AesKey.Length * 8;
                AesEncryptor.IV = CD.IV;
                AesEncryptor.Key = AesKey;
                AesEncryptor.Mode = CD.Mode;
                AesEncryptor.Padding = CD.Padding;
                using (var Enc = AesEncryptor.CreateEncryptor())
                {
                    CD.Data = Enc.TransformFinalBlock(Data, 0, Data.Length);
                    CD.Mac = MAC(CD.Data, MacKey);
                }
            }
            return CD;
        }

        /// <summary>
        /// Decrypts data with a provided AES key and MAC key
        /// </summary>
        /// <param name="Data">Data to decrypt</param>
        /// <param name="AesKey">AES key</param>
        /// <param name="MacKey">MAC key</param>
        /// <returns>Decrypted and authenticated data</returns>
        /// <remarks>Will throw a <see cref="CryptographicException"/> if the MAC does not validates first</remarks>
        private static byte[] DecryptWithKey(AesCryptoData Data, byte[] AesKey, byte[] MacKey)
        {
            using (var AesEncryptor = new AesManaged())
            {
                AesEncryptor.BlockSize = Data.IV.Length * 8;
                AesEncryptor.KeySize = AesKey.Length * 8;
                AesEncryptor.IV = Data.IV;
                AesEncryptor.Key = AesKey;
                AesEncryptor.Mode = Data.Mode;
                AesEncryptor.Padding = Data.Padding;

                if (!MAC(Data.Data, MacKey).SequenceEqual(Data.Mac))
                {
                    throw new CryptographicException("Invalid password or the encrypted data has been tampered with");
                }

                using (var Dec = AesEncryptor.CreateDecryptor())
                {
                    return Dec.TransformFinalBlock(Data.Data, 0, Data.Data.Length);
                }
            }
        }

        /// <summary>
        /// Encrypts data using a given key file
        /// </summary>
        /// <param name="Data">Data to encrypt</param>
        /// <param name="Keyfile">Key file to use as password</param>
        /// <returns>Encrypted data</returns>
        /// <remarks>The keyfile is read and converted to Base64, then it follows <see cref="EncryptWithPassword"/></remarks>
        private static AesCryptoData EncryptWithKeyfile(byte[] Data, string Keyfile)
        {
            return EncryptWithPassword(Data, Convert.ToBase64String(File.ReadAllBytes(Keyfile)));
        }

        /// <summary>
        /// Encrypts data using the given password
        /// </summary>
        /// <param name="Data">Data to encrypt</param>
        /// <param name="Password">Password to encrypt the data with</param>
        /// <returns>Encrypted data</returns>
        private static AesCryptoData EncryptWithPassword(byte[] Data, string Password)
        {
            var Salt = new AesCryptoData().Salt;
            using (var pbkdf = new Rfc2898DeriveBytes(Password, Salt, PBKDF_ITERATIONS))
            {
                var MacKey = pbkdf.GetBytes(AES_KEYSIZE / 8);
                var AesKey = pbkdf.GetBytes(AES_KEYSIZE / 8);
                var CD = EncryptWithKey(Data, AesKey, MacKey);
                CD.Salt = Salt;
                return CD;
            }
        }

        /// <summary>
        /// Encrypts data using the given RSA public key
        /// </summary>
        /// <param name="Data">Data to encrypt</param>
        /// <param name="Password">Key to encrypt the data with</param>
        /// <returns>Encrypted data</returns>
        private static AesCryptoData EncryptWithRSAKey(byte[] Data, RSAParameters Params)
        {
            if (RSAEncryption.HasPublicKey(Params))
            {
                return new AesCryptoData()
                {
                    IV = null,
                    Salt = null,
                    Data = RSAEncryption.Encrypt(Params, Data)
                };
            }
            throw new CryptographicException("The supplied RSA key lacks the public key parts");
        }

        /// <summary>
        /// Decrypts data using a given key file
        /// </summary>
        /// <param name="Data">Data to decrypt</param>
        /// <param name="Keyfile">Key file to use as password</param>
        /// <returns>Decrypted and authenticated data</returns>
        private static byte[] DecryptWithKeyfile(AesCryptoData Data, string Keyfile)
        {
            return DecryptWithPassword(Data, Convert.ToBase64String(File.ReadAllBytes(Keyfile)));
        }

        /// <summary>
        /// Decrypts data using a given password
        /// </summary>
        /// <param name="Data">Data to decrypt</param>
        /// <param name="Password">Password that was used to encrypt</param>
        /// <returns>Decrypted and authenticated data</returns>
        private static byte[] DecryptWithPassword(AesCryptoData Data, string Password)
        {
            using (var pbkdf = new Rfc2898DeriveBytes(Password, Data.Salt, PBKDF_ITERATIONS))
            {
                var MacKey = pbkdf.GetBytes(AES_KEYSIZE / 8);
                var AesKey = pbkdf.GetBytes(AES_KEYSIZE / 8);
                return DecryptWithKey(Data, AesKey, MacKey);
            }
        }

        /// <summary>
        /// Decrypts data using the given RSA key
        /// </summary>
        /// <param name="Data">Data to decrypt</param>
        /// <param name="Params">RSA key</param>
        /// <returns>Decrypted data</returns>
        private static byte[] DecryptWithRSAKey(AesCryptoData Data, RSAParameters Params)
        {
            if (RSAEncryption.HasPrivateKey(Params))
            {
                return RSAEncryption.Decrypt(Params, Data.Data);
            }
            throw new CryptographicException("The supplied RSA key lacks the private key parts");
        }

        #endregion

        #region Hashing

        /// <summary>
        /// Computes the SHA256 of a given string
        /// </summary>
        /// <param name="Data">String</param>
        /// <returns>Hash</returns>
        /// <remarks>Will UTF-8 encode the string</remarks>
        public static string HashSHA256(string Data)
        {
            return HashSHA256(Encoding.UTF8.GetBytes(Data));
        }

        /// <summary>
        /// Computes the SHA256 of the given bytes
        /// </summary>
        /// <param name="Data">byte data</param>
        /// <returns>Hash</returns>
        public static string HashSHA256(byte[] Data)
        {
            using (var Hasher = SHA256.Create())
            {
                var Result = Hasher.ComputeHash(Data);
                return string.Concat(Result.Select(m => m.ToString("X2")));
            }
        }
        #endregion

        #region Public Functions

        /// <summary>
        /// Encrypts the given data using the given methods
        /// </summary>
        /// <param name="Modes">Encryption modes</param>
        /// <param name="Content">Data to encrypt</param>
        /// <param name="ModeParams">Parameter for the supplied modes (for those that require parameters)</param>
        /// <returns>Encrypted and serializable data</returns>
        /// <remarks>See the <see cref="CryptoMode"/> enumeration for required arguments</remarks>
        public static EncryptedData Encrypt(CryptoMode Modes, byte[] Content, IDictionary<CryptoMode, object> ModeParams = null)
        {
            var ED = new EncryptedData();
            var AesKey = ED.AesKey = GetRandomKey();
            var MacKey = ED.MacKey = GetRandomKey();

            var KeyBlob = Encoding.ASCII.GetBytes(Convert.ToBase64String(AesKey) + ":" + Convert.ToBase64String(MacKey));

            //var EncModes = Tools.FlagsToArray(Modes);
            ED.Data = EncryptWithKey(Content, AesKey, MacKey);

            var Providers = new List<KeyProvider>();
            foreach (var ModeParam in ModeParams.Where(m => Modes.HasFlag(m.Key)))
            {
                switch (ModeParam.Key)
                {
                    case CryptoMode.CryptUser:
                        Providers.Add(new KeyProvider()
                        {
                            Mode = ModeParam.Key,
                            KeyData = new AesCryptoData()
                            {
                                Salt = null,
                                IV = null,
                                Data = ProtectData(false, KeyBlob)
                            }
                        });
                        break;
                    case CryptoMode.CryptMachine:
                        Providers.Add(new KeyProvider()
                        {
                            Mode = ModeParam.Key,
                            KeyData = new AesCryptoData()
                            {
                                Salt = null,
                                IV = null,
                                Data = ProtectData(true, KeyBlob)
                            }
                        });
                        break;
                    case CryptoMode.RSA:
                        if (ModeParam.Value == null || !(ModeParam.Value is IEnumerable<RSAParameters>))
                        {
                            throw new ArgumentException("RSA mode requires an RSAParameters structure as argument");
                        }
                        foreach (var key in (IEnumerable<RSAParameters>)ModeParam.Value)
                        {
                            Providers.Add(new KeyProvider()
                            {
                                Mode = ModeParam.Key,
                                KeyData = EncryptWithRSAKey(KeyBlob, key)
                            });
                        }
                        break;
                    case CryptoMode.Keyfile:
                        if (ModeParam.Value == null || ModeParam.Value.GetType() != typeof(string))
                        {
                            throw new ArgumentException("Keyfile mode requires a file name argument");
                        }
                        Providers.Add(new KeyProvider()
                        {
                            Mode = ModeParam.Key,
                            KeyData = EncryptWithKeyfile(KeyBlob, ModeParam.Value.ToString())
                        });
                        break;
                    case CryptoMode.Password:
                        if (ModeParam.Value == null || ModeParam.Value.GetType() != typeof(string))
                        {
                            throw new ArgumentException("Password mode requires a password argument");
                        }
                        Providers.Add(new KeyProvider()
                        {
                            Mode = ModeParam.Key,
                            KeyData = EncryptWithPassword(KeyBlob, ModeParam.Value.ToString())
                        });
                        break;
                    default:
                        throw new NotImplementedException($"Algorithm {ModeParam.Key} is not implemented");
                }
            }
            ED.Providers = Providers.ToArray();
            return ED;
        }

        /// <summary>
        /// Re-Encrypts data without modifying the allowed encryption methods
        /// </summary>
        /// <param name="Data">
        /// Existing encrypted data object produced by <see cref="Encrypt"/>
        /// or supplied to <see cref="Decrypt"/>
        /// </param>
        /// <param name="Content">New content to encrypt</param>
        /// <returns><paramref name="Data"/></returns>
        /// <remarks>
        /// This function can only be used if the <paramref name="Data"/> object was generated by the
        /// <see cref="Encrypt"/> function or was successfully passed to the <see cref="Decrypt"/> function.
        /// You can't use this on data that has only been serialized.
        /// If this is the case, ReEncrypt allows you to encrypt new data without invalidating any of the
        /// decryption providers in the object.
        /// If you want to change the allowed providers, use <see cref="Encrypt"/> instead
        /// </remarks>
        public static EncryptedData ReEncrypt(EncryptedData Data, byte[] Content)
        {
            if (Data.AesKey == null || Data.MacKey == null)
            {
                throw new ArgumentException("ReEncrypt can only be used if the data has been encrypted or decrypted while this instance was running.");
            }
            Data.Data = EncryptWithKey(Content, Data.AesKey, Data.MacKey);
            return Data;
        }

        /// <summary>
        /// Decrypts data
        /// </summary>
        /// <param name="Data">Encrypted data</param>
        /// <param name="ModeParams">Parameter for the supplied modes (for those that require parameters)</param>
        /// <returns>Decrypted data</returns>
        public static byte[] Decrypt(EncryptedData Data, IDictionary<CryptoMode, object> ModeParams = null)
        {
            var skipped = false;
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }
            for (var i = 0; i < Data.Providers.Length; i++)
            {
                byte[] KeyBlob = null;
                var P = Data.Providers[i];
                var Param = ModeParams == null ? null : (ModeParams.ContainsKey(P.Mode) ? ModeParams[P.Mode] : null);
                switch (P.Mode)
                {
                    case CryptoMode.CryptUser:
                        try
                        {
                            KeyBlob = UnprotectData(false, P.KeyData.Data);
                        }
                        catch { continue; }
                        break;
                    case CryptoMode.CryptMachine:
                        try
                        {
                            KeyBlob = UnprotectData(true, P.KeyData.Data);
                        }
                        catch { continue; }
                        break;
                    case CryptoMode.RSA:
                        try
                        {
                            skipped |= Param == null || Param.GetType() != typeof(RSAParameters);
                            KeyBlob = DecryptWithRSAKey(P.KeyData, (RSAParameters)Param);
                        }
                        catch
                        { continue; }
                        break;
                    case CryptoMode.Keyfile:
                        try
                        {
                            skipped |= Param == null || Param.GetType() != typeof(string);
                            KeyBlob = DecryptWithKeyfile(P.KeyData, Param.ToString());
                        }
                        catch
                        { continue; }
                        break;
                    case CryptoMode.Password:
                        try
                        {
                            skipped |= Param == null || Param.GetType() != typeof(string);
                            KeyBlob = DecryptWithPassword(P.KeyData, Param.ToString());
                        }
                        catch
                        { continue; }
                        break;
                    default:
                        throw new NotImplementedException($"Algorithm {P.Mode} is not implemented");
                }
                if (KeyBlob != null)
                {
                    //Try to decrypt
                    try
                    {
                        var KeyParts = Encoding.ASCII.GetString(KeyBlob).Split(':');
                        if (KeyParts.Length == 2)
                        {
                            Data.AesKey = Convert.FromBase64String(KeyParts[0]);
                            Data.MacKey = Convert.FromBase64String(KeyParts[1]);
                            return DecryptWithKey(Data.Data, Data.AesKey, Data.MacKey);
                        }
                    }
                    catch
                    {
                        Data.AesKey = Data.MacKey = null;
                        //Something doesn't works right.
                        //Try next key provider if possible
                    }
                }
            }
            if (skipped)
            {
                throw new CryptographicException("None of the used providers could decrypt the content. Some were skipped because of missing parameters. Try supplying parameters for the skipped modes.");
            }
            throw new CryptographicException("None of the available providers could decrypt the content.");
        }

        #endregion
    }

    /// <summary>
    /// Implemented cryptographic modes
    /// </summary>
    /// <remarks>Negative numbers are reserved for user defined modes</remarks>
    [Flags]
    public enum CryptoMode : int
    {
        /// <summary>
        /// Represents all possible methods. Not to be used directly by the user
        /// </summary>
        _ALL = CryptUser | CryptMachine | Password | Keyfile | RSA,
        /// <summary>
        /// Encryption bound to the current user account.
        /// To decrypt, the same account is needed.
        /// </summary>
        /// <remarks>This mode has no arguments</remarks>
        CryptUser = 1,
        /// <summary>
        /// Encryption bound to the local machine.
        /// Any user on the same machine can decrypt the data.
        /// </summary>
        /// <remarks>This mode has no arguments</remarks>
        CryptMachine = CryptUser << 1,
        /// <summary>
        /// Encryption based on a password
        /// </summary>
        /// <remarks>This mode requires a string as argument that serves as the password</remarks>
        Password = CryptMachine << 1,
        /// <summary>
        /// Encryption based on a keyfile
        /// </summary>
        /// <remarks>This mode requires a string as argument that serves as the key file name</remarks>
        Keyfile = Password << 1,
        /// <summary>
        /// Encryption using an RSA key
        /// </summary>
        /// <remarks>This mode requires an <see cref="RSAParameters"/> structure as argument</remarks>
        RSA = Keyfile << 1
    }
}
