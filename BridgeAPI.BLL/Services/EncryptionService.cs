using BridgeAPI.BLL.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services
{
    public class EncryptionService : IEncryptionService
    {
        public Task<string> Decrypt(string encriptedText)
        {
            throw new NotImplementedException();
        }

        public Task<string> Encrypt(string text, Tuple<byte[], byte[]> KeyIv)
        {
            byte[] encrypted;
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = KeyIv.Item1;
                aesAlg.IV = KeyIv.Item2;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create a memory stream to write the encrypted data to
                using (var msEncrypt = new MemoryStream())
                {
                    // Create a crypto stream that transforms data as it is written to and read from the memory stream
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Convert the plain text string to a byte array
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                        // Write the plain text byte array to the crypto stream
                        csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                    }

                    // Get the encrypted data from the memory stream
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Convert the encrypted byte array to a base64-encoded string and return it
            return Convert.ToBase64String(encrypted);
        }

        public Tuple<byte[], byte[]> GetKeyAndIVFromPublicKey(string publicKey)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(publicKey, 16, 1000))
            {
                return new Tuple<byte[], byte[]> (deriveBytes.GetBytes(32), deriveBytes.GetBytes(16));
            }
        }
    }
}
