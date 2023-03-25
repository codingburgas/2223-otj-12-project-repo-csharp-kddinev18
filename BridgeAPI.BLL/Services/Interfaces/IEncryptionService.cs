using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services.Interfaces
{
    public interface IEncryptionService
    {
        public Tuple<byte[], byte[]> GetKeyAndIVFromPublicKey(string publicKey);
        public Task<string> Encrypt(string text, Tuple<byte[], byte[]> KeyIv);
        public Task<string> Decrypt(string encriptedText, Tuple<byte[], byte[]> KeyIv);
    }
}
