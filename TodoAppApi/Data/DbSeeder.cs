using TodoAppApi.Data;
using TodoAppApi.Entities;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!context.Tags.Any())
        {
            var tags = new[]
            {
                new Tag { Id = Guid.NewGuid(), Name = "Work" },
                new Tag { Id = Guid.NewGuid(), Name = "Personal" },
                new Tag { Id = Guid.NewGuid(), Name = "Urgent" },
                new Tag { Id = Guid.NewGuid(), Name = "Shopping" },
                new Tag { Id = Guid.NewGuid(), Name = "Health" },
                new Tag { Id = Guid.NewGuid(), Name = "Finance" },
                new Tag { Id = Guid.NewGuid(), Name = "Home" },
                new Tag { Id = Guid.NewGuid(), Name = "Learning" },
                new Tag { Id = Guid.NewGuid(), Name = "Travel" },
                new Tag { Id = Guid.NewGuid(), Name = "Goals" }
            };

            await context.Tags.AddRangeAsync(tags);
        }

        await context.SaveChangesAsync();
    }
}
