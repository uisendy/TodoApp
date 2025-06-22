using TodoAppApi.DTOs.Security;
using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface ITokenManager
    {
        TokenResultDto GenerateTokens(User user);
    }
}
