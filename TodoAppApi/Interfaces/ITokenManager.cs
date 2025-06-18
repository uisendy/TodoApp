using TodoAppApi.DTOs.Security;
using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface ITokenManager
    {
        TokenResultDto GenerateAndHashTokens(User user);
    }
}
