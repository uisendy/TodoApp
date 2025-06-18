using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
    }
}