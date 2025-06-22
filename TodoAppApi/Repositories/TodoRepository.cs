using Microsoft.EntityFrameworkCore;
using TodoAppApi.Data;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Todo?> GetByIdAsync(Guid id)
        {
            return await _context.Todos
                .Include(t => t.TodoTags)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Todo>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Todos
                .Include(t => t.TodoTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Todo>> GetAllAsync()
        {
            return await _context.Todos
                .Include(t => t.TodoTags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync();
        }

        public async Task AddAsync(Todo todo)
        {
            await _context.Todos.AddAsync(todo);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetTagsByIdsAsync(List<Guid> ids)
        {
            return await _context.Tags
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        public async Task<Todo> GetByIdWithTagsAsync(Guid id)
        {
            var todo=  await _context.Todos
                .Include(t => t.TodoTags)
                    .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            return todo ?? throw new Exception("Todo not found");
        }


        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<bool> ArchiveAsync(Guid userId, Guid todoId)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null) return false;

            todo.IsArchived = true;
            todo.ArchivedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<Todo?> UpdateAsync(Guid userId, Guid todoId, Todo update)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);
            if (todo == null) return null;

            todo.Name = update.Name;
            todo.Description = update.Description;
            todo.Priority = update.Priority;
            todo.UpdatedAt = DateTime.UtcNow;




            todo.TodoTags.Clear(); 

            
            if (update.TodoTags != null && update.TodoTags.Count > 0)
            {
                foreach (var tag in update.TodoTags)
                {
                    todo.TodoTags.Add(new TodoTag
                    {
                        TodoId = todo.Id,
                        TagId = tag.TagId
                    });
                }
            }

            return todo;
        }

        public async Task<int> DeleteOldArchivedAsync(DateTime cutoff)
        {
            var oldArchived = _context.Todos.Where(t => t.IsArchived && t.ArchivedAt <= cutoff);
            _context.Todos.RemoveRange(oldArchived);
            return await _context.SaveChangesAsync();
        }
    }
}
