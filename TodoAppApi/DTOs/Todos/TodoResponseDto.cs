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
        public PriorityLevel Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
    }

}
