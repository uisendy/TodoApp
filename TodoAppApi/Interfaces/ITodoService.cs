using TodoAppApi.DTOs.Todos;

public interface ITodoService
{
    Task<TodoResponseDto> GetTodoByIdAsync(Guid id);
    Task<List<TodoResponseDto>> GetTodosByUserIdAsync(Guid userId);
    Task<List<TodoResponseDto>> GetAllTodosAsync(); 
    Task<TodoResponseDto> CreateTodoAsync(Guid userId, TodoRequestDto request);
    Task<bool> ArchiveTodoAsync(Guid userId, Guid todoId);
    Task<TodoResponseDto> UpdateTodoAsync(Guid userId, Guid todoId, TodoRequestDto request);
    Task<TodoResponseDto> ToggleCompletionAsync(Guid userId, Guid todoId);
    IEnumerable<PriorityDto> GetTodoPriorities();

    Task<IEnumerable<TodoTagDto>> GetTodoTagsAsync();
    Task<int> DeleteOldArchivedTodosAsync(); 
}
