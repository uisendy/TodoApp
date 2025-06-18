using TodoAppApi.Data;
using TodoAppApi.Entities;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Make sure DB is created
        await context.Database.EnsureCreatedAsync();

        // Seed Tags
        if (!context.Tags.Any())
        {
            var tags = new[]
            {
                new Tag { Id = Guid.NewGuid(), Name = "Work" },
                new Tag { Id = Guid.NewGuid(), Name = "Personal" },
                new Tag { Id = Guid.NewGuid(), Name = "Health" }
            };

            await context.Tags.AddRangeAsync(tags);
        }

        // Seed a User
        if (!context.Users.Any())
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@todo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"), // You can use BCrypt
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(user);
        }

        await context.SaveChangesAsync();

        // Optional: Add sample Todo for the user if no Todos exist
        if (!context.Todos.Any())
        {
            var user = context.Users.First();
            var tags = context.Tags.Take(2).ToList(); // Use 2 tags

            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                Name = "Complete coding challenge",
                Description = "Implement seeding logic for initial setup",
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                Priority = PriorityLevel.High,
                TodoTags = tags.Select(tag => new TodoTag
                {
                    TagId = tag.Id
                }).ToList()
            };

            await context.Todos.AddAsync(todo);
            await context.SaveChangesAsync();
        }
    }
}
