using System.Linq;
using System.Security.Cryptography;

namespace CryptoPad
{
    public static class RSAEncryption
    {
        public const int DEFAULT_KEYSIZE = 4096;

        private static bool NotEmpty(params byte[][] Data)
        {
            return Data != null && !Data.Any(m => m == null || m.Length == 0);
        }

        /*
        
        private static int GetIntegerSize(BinaryReader BR)
        {
            int count = 0;
            if (BR.ReadByte() != 0x02)
            {
                throw new FormatException("Expected integer field");
            }
            byte b = BR.ReadByte();
            switch (b)
            {
                case 0x81:
                    count = BR.ReadByte();
                    break;
                case 0x82:
                    count = BR.ReadByte() << 8;
                    count += BR.ReadByte();
                    break;
                default:
                    count = b;
                    break;
            }
            while (BR.ReadByte() == 0x00)
            {
                //remove high order zeros in data
                count -= 1;
            }
            //last ReadByte wasn't a removed zero, so back up a byte
            BR.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        public static RSAParameters ImportPEMKey(string Key)
        {
            Key = Key.Trim();
            if (Key.StartsWith("-----"))
            {
                Key = Key.Substring(Key.IndexOf('\n') + 1);
            }
            if(Key.EndsWith("-----"))
            {
                Key = Key.Substring(0, Key.LastIndexOf('\n')).Trim();
            }
            return ImportPEMKey(Convert.FromBase64String(Key));
        }

        public static RSAParameters ImportPEMKey(byte[] Key)
        {
            RSAParameters Params = new RSAParameters();

            using (MemoryStream MS = new MemoryStream(Key))
            {
                using (BinaryReader BR = new BinaryReader(MS))
                {
                    //wrap Memory Stream with BinaryReader for easy reading
                    int elems = 0;
                    try
                    {
                        //data read as little endian order (actual data order for Sequence is 30 81)
                        switch (BR.ReadUInt16())
                        {
                            case 0x8130:
                                BR.ReadByte();
                                break;
                            case 0x8230:
                                BR.ReadInt16();
                                break;
                            default:
                                throw new FormatException("Key is in an invalid format");
                        }

                        //version number
                        if (BR.ReadUInt16() != 0x0102)
                        {
                            throw new FormatException("Invalid version number");
                        }
                        //Null byte must follow the version number
                        if (BR.ReadByte() != 0x00)
                        {
                            throw new FormatException("Invalid key format");
                        }


                        //------  all private key components are Integer sequences ----
                        elems = GetIntegerSize(BR);
                        Params.Modulus = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.Exponent = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.D = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.P = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.Q = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.DP = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.DQ = BR.ReadBytes(elems);

                        elems = GetIntegerSize(BR);
                        Params.InverseQ = BR.ReadBytes(elems);

                        return Params;
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException("Unable to decode the key. See inner exception for details", ex);
                    }
                }
            }
        }

        //*/

        public static bool HasPrivateKey(RSAParameters Param)
        {
            return NotEmpty(Param.D, Param.DP, Param.DQ, Param.InverseQ, Param.P, Param.Q);
        }

        public static bool HasPublicKey(RSAParameters Param)
        {
            return NotEmpty(Param.Exponent, Param.Modulus);
        }

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
                        return false;
                    }
                }
                else
                {
                    //Private key missing from one of them
                    return false;
                }
            }
            //Keys are identical
            return true;
        }

        public static RSAKey GenerateKey(string Name, int Size = DEFAULT_KEYSIZE)
        {
            using (var Alg = new RSACryptoServiceProvider(Size))
            {
                Alg.PersistKeyInCsp = false;
                return new RSAKey(Name, Alg.ExportParameters(true));
            }
        }

        public static RSAKey StripPrivate(RSAKey Key)
        {
            return new RSAKey(Key.Name, new RSAParameters()
            {
                Modulus = (byte[])Key.Key.Modulus.Clone(),
                Exponent = (byte[])Key.Key.Exponent.Clone()
            });
        }

        public static byte[] Encrypt(RSAKey Key, byte[] Data)
        {
            return Encrypt(Key.Key, Data);
        }

        public static byte[] Encrypt(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.Encrypt(Data, RSAEncryptionPadding.Pkcs1);
            }
        }

        public static byte[] Decrypt(RSAKey Key, byte[] Data)
        {
            return Decrypt(Key.Key, Data);
        }

        public static byte[] Decrypt(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.Decrypt(Data, RSAEncryptionPadding.Pkcs1);
            }
        }

        public static byte[] Sign(RSAKey Key, byte[] Data)
        {
            return Sign(Key.Key, Data);
        }

        public static byte[] Sign(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.SignData(Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static bool Verify(RSAKey Key, byte[] Data, byte[] Signature)
        {
            return Verify(Key.Key, Data, Signature);
        }

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
