using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{

    public class Encripta
    {
        private readonly Encoding encoding;
        private readonly string salt = "DeusExMachina​";


        private SicBlockCipher mode;

        //AES
        public Encripta(Encoding encoding)
        {
            this.encoding = encoding;
            this.mode = new SicBlockCipher(new AesEngine());
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }


        public string Encrypt(string plain)
        {

            var aesAlg = NewRijndaelManaged();

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


            byte[] input = this.encoding.GetBytes(plain);

            byte[] bytes = this.BouncyCastleCrypto(true, input, aesAlg.Key, aesAlg.IV);

            string result = ByteArrayToString(bytes);

            return result;
        }


        public string Decrypt(string cipher)
        {


            var aesAlg = NewRijndaelManaged();

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


            byte[] bytes = this.BouncyCastleCrypto(false, StringToByteArray(cipher), aesAlg.Key, aesAlg.IV);

            string result = this.encoding.GetString(bytes);

            return result;
        }


        private byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                this.mode.Init(forEncrypt, new ParametersWithIV(new KeyParameter(key), iv));

                BufferedBlockCipher cipher = new BufferedBlockCipher(this.mode);

                return cipher.DoFinal(input);
            }
            catch (CryptoException)
            {
                throw;
            }
        }


        private RijndaelManaged NewRijndaelManaged()
        {

            if (salt == null) throw new ArgumentNullException("salt");
            var saltBytes = Encoding.ASCII.GetBytes(salt);

            string Inputkey = "popFqAw_xq2dbp-xqL19FbQNQ7ajGfj4ftMJq4G-lqc";
            var key = new Rfc2898DeriveBytes(Inputkey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }
    }
}
