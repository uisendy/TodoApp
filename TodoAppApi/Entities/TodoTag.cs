namespace TodoAppApi.Entities
{
    public class TodoTag
    {
        public Guid TodoId { get; set; }
        public Todo Todo { get; set; } = default!;

        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = default!;
    }

}
