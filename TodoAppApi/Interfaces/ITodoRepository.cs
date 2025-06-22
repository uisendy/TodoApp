using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface ITodoRepository
    {
        Task<Todo?> GetByIdAsync(Guid id);
        Task<List<Todo>> GetByUserIdAsync(Guid userId);
        Task<List<Todo>> GetAllAsync();
        Task<List<Tag>> GetTagsByIdsAsync(List<Guid> ids);
        Task<Todo> GetByIdWithTagsAsync(Guid id);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task AddAsync(Todo todo);
        Task SaveChangesAsync();
        Task<bool> ArchiveAsync(Guid userId, Guid todoId);
        Task<Todo?> UpdateAsync(Guid userId, Guid todoId, Todo update);
        Task<int> DeleteOldArchivedAsync(DateTime cutoff);
    }
}
