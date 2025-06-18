using TodoAppApi.DTOs.Security;
using TodoAppApi.Entities;
using TodoAppApi.Helpers;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Services.Security
{
    public class TokenManager(ITokenService tokenService) : ITokenManager
    {
        private readonly ITokenService _tokenService = tokenService;

        public TokenResultDto GenerateAndHashTokens(User user)
        {
            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var hashedRefreshToken = TokenHashHelper.HashToken(refreshToken);

            return new TokenResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
            };
        }
    }

}
