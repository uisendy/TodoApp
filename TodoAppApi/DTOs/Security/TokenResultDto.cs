namespace TodoAppApi.DTOs.Security
{
    public class TokenResultDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string CurrentJti { get; set; } = default!;
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
