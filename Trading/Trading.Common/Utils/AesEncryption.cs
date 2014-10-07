using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Trading.Common.Utils
{
    public class AesEncryption
    {
        private readonly byte[] key = { 251, 85, 101, 176, 139, 205, 176, 228, 177, 135, 171, 197, 60, 65, 69, 8, 70, 50, 211, 125, 80, 121, 124, 224, 3, 69, 182, 222, 227, 219, 150, 37 };
        private readonly byte[] vector = { 86, 225, 6, 60, 27, 231, 153, 85, 131, 147, 66, 121, 140, 202, 92, 76 };
        private readonly ICryptoTransform encryptorTransform;
        private readonly ICryptoTransform decryptorTransform;
        private readonly UTF8Encoding utfEncoder;

        public AesEncryption()
        {
            var rm = new RijndaelManaged();

            encryptorTransform = rm.CreateEncryptor(key, vector);
            decryptorTransform = rm.CreateDecryptor(key, vector);

            //Used to translate bytes to text and vice versa
            utfEncoder = new UTF8Encoding();
        }

        /// <summary>
        /// Generates an encryption key.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateEncryptionKey()
        {
            var rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        /// <summary>
        /// Generates a unique encryption vector
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateEncryptionVector()
        {
            var rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }

        public string EncryptToString(string text, string salt)
        {
            return EncryptToString(text + salt);
        }

        public string EncryptToString(string text)
        {
            return Convert.ToBase64String(Encrypt(text));
        }

        /// Encrypt some text and return an encrypted byte array.
        public byte[] Encrypt(string text)
        {
            //Translates our text value into a byte array.
            Byte[] bytes = utfEncoder.GetBytes(text);

            //Used to stream the data in and out of the CryptoStream.
            var memoryStream = new MemoryStream();

            /*
             * We will have to write the unencrypted bytes to the stream,
             * then read the encrypted result back from the stream.
             */
            #region Write the decrypted value to the encryption stream
            var cs = new CryptoStream(memoryStream, encryptorTransform, CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            #endregion

            #region Read encrypted value back out of the stream
            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            #endregion

            //Clean up.
            cs.Close();
            memoryStream.Close();

            return encrypted;
        }

        public string DecryptString(string encrypted)
        {
            return Decrypt(Convert.FromBase64String(encrypted));
        }

        public string DecryptString(string encrypted, string salt)
        {
            return DecryptString(encrypted).Replace(salt, "");
        }

        /// Decryption when working with byte arrays.    
        public string Decrypt(byte[] encryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, decryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(encryptedValue, 0, encryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            byte[] decryptedBytes = new byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return utfEncoder.GetString(decryptedBytes);
        }
    }
}