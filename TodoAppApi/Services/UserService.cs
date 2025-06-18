using Microsoft.EntityFrameworkCore;
using TodoAppApi.Data;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.DTOs.Users;
using TodoAppApi.Entities;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto> GetUserProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Todos)
                    .ThenInclude(t => t.TodoTags)
                        .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Todos = user.Todos.Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    IsArchived = t.IsArchived,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    ArchivedAt = t.ArchivedAt,
                    Tags = t.TodoTags.Select(tt => tt.Tag.Name).ToList()
                }).ToList()
            };
        }

    }
}
