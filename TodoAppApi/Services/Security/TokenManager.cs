using TodoAppApi.DTOs.Security;
using TodoAppApi.Entities;
using TodoAppApi.Helpers;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Services.Security
{
    public class TokenManager(ITokenService tokenService) : ITokenManager
    {
        private readonly ITokenService _tokenService = tokenService;

        public TokenResultDto GenerateTokens(User user)
        {
            var (accessToken, jti) = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new TokenResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                CurrentJti = jti,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
            };
        }
    }

}
