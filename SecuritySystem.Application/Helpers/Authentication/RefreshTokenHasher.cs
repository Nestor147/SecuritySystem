using System.Security.Cryptography;
using System.Text;

namespace SecuritySystem.Application.Helpers.Authentication
{
    public static class RefreshTokenHasher
    {
        public static string GenerateOpaque()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }

        public static string Hash(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
