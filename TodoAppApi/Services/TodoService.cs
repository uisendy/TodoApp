using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepo;
        private readonly IMapper _mapper;

        public TodoService(ITodoRepository todoRepo, IMapper mapper)
        {
            _todoRepo = todoRepo;
            _mapper = mapper;
        }

        public async Task<TodoResponseDto> GetTodoByIdAsync(Guid id)
        {
            var todo = await _todoRepo.GetByIdAsync(id);
            if (todo == null)
                throw new KeyNotFoundException("Todo not found");

            return _mapper.Map<TodoResponseDto>(todo);
        }

        public async Task<List<TodoResponseDto>> GetTodosByUserIdAsync(Guid userId)
        {
            var todos = await _todoRepo.GetByUserIdAsync(userId);
            return _mapper.Map<List<TodoResponseDto>>(todos);
        }

        public async Task<List<TodoResponseDto>> GetAllTodosAsync()
        {
            var todos = await _todoRepo.GetAllAsync();
            return _mapper.Map<List<TodoResponseDto>>(todos);
        }

        public async Task<TodoResponseDto> CreateTodoAsync(Guid userId, TodoRequestDto request)
        {
            var todo = _mapper.Map<Todo>(request);
            todo.Id = Guid.NewGuid();
            todo.UserId = userId;
            todo.CreatedAt = DateTime.UtcNow;

            if (request.Tags.Count != 0)
            {
                var tagIds = request.Tags.Select(Guid.Parse).ToList();
                var tags = await _todoRepo.GetTagsByIdsAsync(tagIds);

                todo.TodoTags = tags.Select(tag => new TodoTag
                {
                    TodoId = todo.Id,
                    TagId = tag.Id
                }).ToList();
            }



            await _todoRepo.AddAsync(todo);
            await _todoRepo.SaveChangesAsync();

            return _mapper.Map<TodoResponseDto>(todo);
        }

        public async Task<bool> ArchiveTodoAsync(Guid userId, Guid todoId)
        {
            var result = await _todoRepo.ArchiveAsync(userId, todoId);
            if (result)
                await _todoRepo.SaveChangesAsync();
            return result;
        }

        public async Task<TodoResponseDto> UpdateTodoAsync(Guid userId, Guid todoId, TodoRequestDto request)
        {
            var existing = await _todoRepo.GetByIdAsync(todoId);
            if (existing == null || existing.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to update this todo.");

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await _todoRepo.SaveChangesAsync();
            return _mapper.Map<TodoResponseDto>(existing);
        }

        public async Task<TodoResponseDto> ToggleCompletionAsync(Guid userId, Guid todoId)
        {
            var existing = await _todoRepo.GetByIdAsync(todoId);
            if (existing == null || existing.UserId != userId)
                throw new UnauthorizedAccessException("You do not have access to update this todo.");

            existing.IsCompleted = !existing.IsCompleted;
            existing.UpdatedAt = DateTime.UtcNow;

            await _todoRepo.SaveChangesAsync();
            return _mapper.Map<TodoResponseDto>(existing);
        }

        public IEnumerable<PriorityDto> GetTodoPriorities()
        {
            return Enum.GetValues<PriorityLevel>()
                       .Select(p => new PriorityDto
                       {
                           Name = p.ToString(),
                           Value = (int)p
                       });
        }

        public async Task<IEnumerable<TodoTagDto>> GetTodoTagsAsync()
        {
            var tags = await _todoRepo.GetAllTagsAsync();

            return tags.Select(t => new TodoTagDto
            {
                Id = t.Id,
                Name = t.Name
            });
        }

        public async Task<int> DeleteOldArchivedTodosAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);
            return await _todoRepo.DeleteOldArchivedAsync(cutoff);
        }
    }
}
