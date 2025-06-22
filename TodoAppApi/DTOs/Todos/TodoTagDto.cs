namespace TodoAppApi.DTOs.Todos
{
    public class TodoTagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
