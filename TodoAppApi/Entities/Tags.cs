namespace TodoAppApi.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public ICollection<TodoTag> TodoTags { get; set; } = new List<TodoTag>();
    }

}
