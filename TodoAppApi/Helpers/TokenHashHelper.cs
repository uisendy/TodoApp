namespace TodoAppApi.Helpers
{
    public static class TokenHashHelper
    {
        public static string HashToken(string token)
        {
            return BCrypt.Net.BCrypt.HashPassword(token);
        }

        public static bool VerifyToken(string rawToken, string hashedToken)
        {
            return BCrypt.Net.BCrypt.Verify(rawToken, hashedToken);
        }
    }
}
