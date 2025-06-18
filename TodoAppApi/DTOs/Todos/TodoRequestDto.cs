using TodoAppApi.Entities;

namespace TodoAppApi.DTOs.Todos
{
    public class TodoRequestDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public List<string> Tags { get; set; } = new();
    }
}




