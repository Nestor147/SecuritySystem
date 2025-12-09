using Microsoft.Extensions.Configuration;
using SecuritySystem.Application.Interfaces.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace SecuritySystem.Application.Services.Authentication
{
    public sealed class AesGcmPrivateKeyProtector : IPrivateKeyProtector
    {
        private readonly byte[] _masterKey; // 32 bytes (256 bits)

        public AesGcmPrivateKeyProtector(IConfiguration cfg)
        {
            // MasterKey should come from env var or KeyVault, not from plain appsettings.json
            var keyBase64 = cfg["Auth:MasterKeyBase64"];
            if (string.IsNullOrWhiteSpace(keyBase64))
                throw new InvalidOperationException("Auth:MasterKeyBase64 is not configured.");

            _masterKey = Convert.FromBase64String(keyBase64);
            if (_masterKey.Length != 32)
                throw new InvalidOperationException("Master key must be 256 bits (32 bytes).");
        }

        public byte[] EncryptPrivateKey(string privateKeyPem)
        {
            var plaintext = Encoding.UTF8.GetBytes(privateKeyPem);

            using var aes = new AesGcm(_masterKey);
            var nonce = RandomNumberGenerator.GetBytes(12); // 96-bit nonce
            var ciphertext = new byte[plaintext.Length];
            var tag = new byte[16];

            aes.Encrypt(nonce, plaintext, ciphertext, tag);

            // Layout: [nonce(12)][tag(16)][ciphertext]
            var result = new byte[12 + 16 + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, 12);
            Buffer.BlockCopy(tag, 0, result, 12, 16);
            Buffer.BlockCopy(ciphertext, 0, result, 28, ciphertext.Length);

            return result;
        }

        public string DecryptPrivateKey(byte[] encryptedPrivateKey)
        {
            if (encryptedPrivateKey.Length < 28)
                throw new ArgumentException("Invalid encrypted private key payload.");

            var nonce = new byte[12];
            var tag = new byte[16];
            var ciphertext = new byte[encryptedPrivateKey.Length - 28];

            Buffer.BlockCopy(encryptedPrivateKey, 0, nonce, 0, 12);
            Buffer.BlockCopy(encryptedPrivateKey, 12, tag, 0, 16);
            Buffer.BlockCopy(encryptedPrivateKey, 28, ciphertext, 0, ciphertext.Length);

            using var aes = new AesGcm(_masterKey);
            var plaintext = new byte[ciphertext.Length];

            aes.Decrypt(nonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
