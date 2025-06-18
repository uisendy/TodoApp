namespace TodoAppApi.Infrastructure.Security
{
    public class TokenContext
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
