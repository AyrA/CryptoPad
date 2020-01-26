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

        public static RSAParameters GenerateKey(int Size = DEFAULT_KEYSIZE)
        {
            using (var Alg = RSA.Create())
            {
                Alg.KeySize = Size;
                return Alg.ExportParameters(true);
            }
        }

        public static RSAParameters StripPrivate(RSAParameters Key)
        {
            return new RSAParameters()
            {
                Modulus = (byte[])Key.Modulus.Clone(),
                Exponent = (byte[])Key.Exponent.Clone()
            };
        }

        public static byte[] Encrypt(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.Encrypt(Data, RSAEncryptionPadding.OaepSHA256);
            }
        }

        public static byte[] Decrypt(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.Decrypt(Data, RSAEncryptionPadding.OaepSHA256);
            }
        }

        public static byte[] Sign(RSAParameters Key, byte[] Data)
        {
            using (var Alg = RSA.Create())
            {
                Alg.ImportParameters(Key);
                return Alg.SignData(Data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
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
