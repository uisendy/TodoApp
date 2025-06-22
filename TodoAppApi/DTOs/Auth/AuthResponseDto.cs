namespace TodoAppApi.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string? Phone { get; set; }
        public string? Bio { get; set; }

    }
}
