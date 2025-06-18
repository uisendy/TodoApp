namespace TodoAppApi.Entities
{
    public enum PriorityLevel
    {
        Low,
        Medium,
        High
    }

    public class Todo
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
    }

}
