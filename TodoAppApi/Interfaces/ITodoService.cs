using TodoAppApi.DTOs.Todos;

public interface ITodoService
{
    Task<TodoResponseDto> GetTodoByIdAsync(Guid id);
    Task<IEnumerable<TodoResponseDto>> GetTodosByUserIdAsync(Guid userId);
    Task<IEnumerable<TodoResponseDto>> GetAllTodosAsync(); 
    Task<TodoResponseDto> CreateTodoAsync(Guid userId, TodoRequestDto request);
    Task<bool> ArchiveTodoAsync(Guid userId, Guid todoId);
    Task<TodoResponseDto> UpdateTodoAsync(Guid userId, Guid todoId, TodoRequestDto request);
    Task<int> DeleteOldArchivedTodosAsync(); 
}
