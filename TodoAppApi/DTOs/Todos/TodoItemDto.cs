using TodoAppApi.Entities;

namespace TodoAppApi.DTOs.Todos
{
    public class TodoItemDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsArchived { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
