using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface ITokenService
    {
        (string AccessToken, string Jti) GenerateAccessToken(User user);
        string GenerateRefreshToken();

    }

}
