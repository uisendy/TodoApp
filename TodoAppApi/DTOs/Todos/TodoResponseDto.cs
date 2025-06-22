using TodoAppApi.Entities;

namespace TodoAppApi.DTOs.Todos
{
    public class TodoResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsArchived { get; set; }
        public DateTime ArchivedAt { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TodoTagDto> Tags { get; set; } = new();
    }

}
