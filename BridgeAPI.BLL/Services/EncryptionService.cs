using BridgeAPI.BLL.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public async Task<string> Decrypt(string encriptedText, Tuple<byte[], byte[]> KeyIv)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encriptedText);
            string plaintext = string.Empty;

            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = KeyIv.Item1;
                aesAlg.IV = KeyIv.Item2;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherTextBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = await srDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }

            return plaintext;
        }

        public async Task<string> Encrypt(string text, Tuple<byte[], byte[]> KeyIv)
        {
            byte[] encrypted;
            using (var aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = KeyIv.Item1;
                aesAlg.IV = KeyIv.Item2;
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
                        await csEncrypt.WriteAsync(plainTextBytes, 0, plainTextBytes.Length);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

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
