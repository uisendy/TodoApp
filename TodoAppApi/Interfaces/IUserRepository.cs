using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}