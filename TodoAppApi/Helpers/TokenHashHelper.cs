using System.Security.Cryptography;
using System.Text;

namespace TodoAppApi.Helpers
{
    public static class TokenHashHelper
    {
        public static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyToken(string rawToken, string hashedToken)
        {
            return BCrypt.Net.BCrypt.Verify(rawToken, hashedToken);
        }
    }
}
