using AutoMapper;
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

        public async Task<IEnumerable<TodoResponseDto>> GetTodosByUserIdAsync(Guid userId)
        {
            var todos = await _todoRepo.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<TodoResponseDto>>(todos);
        }

        public async Task<IEnumerable<TodoResponseDto>> GetAllTodosAsync()
        {
            var todos = await _todoRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<TodoResponseDto>>(todos);
        }

        public async Task<TodoResponseDto> CreateTodoAsync(Guid userId, TodoRequestDto request)
        {
            var todo = _mapper.Map<Todo>(request);
            todo.Id = Guid.NewGuid();
            todo.UserId = userId;
            todo.CreatedAt = DateTime.UtcNow;

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

        public async Task<int> DeleteOldArchivedTodosAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);
            return await _todoRepo.DeleteOldArchivedAsync(cutoff);
        }
    }
}
