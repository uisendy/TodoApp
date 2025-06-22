using TodoAppApi.DTOs.Todos;

namespace TodoAppApi.DTOs.Users
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Bio { get; set; } = default!;
       
    }
}