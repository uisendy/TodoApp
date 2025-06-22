namespace TodoAppApi.DTOs.Users
{
    public class UpdateUserRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
    }
}
