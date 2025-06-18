using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();

    }

}
