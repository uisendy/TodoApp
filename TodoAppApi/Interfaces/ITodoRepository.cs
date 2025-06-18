using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface ITodoRepository
    {
        Task<Todo?> GetByIdAsync(Guid id);
        Task<IEnumerable<Todo>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Todo>> GetAllAsync();
        Task AddAsync(Todo todo);
        Task SaveChangesAsync();
        Task<bool> ArchiveAsync(Guid userId, Guid todoId);
        Task<Todo?> UpdateAsync(Guid userId, Guid todoId, Todo update);
        Task<int> DeleteOldArchivedAsync(DateTime cutoff);
    }
}
