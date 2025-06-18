using Microsoft.EntityFrameworkCore;
using TodoAppApi.Entities;

namespace TodoAppApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Todo> Todos => Set<Todo>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<TodoTag> TodoTags => Set<TodoTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoTag>()
                .HasKey(tt => new { tt.TodoId, tt.TagId });

            modelBuilder.Entity<TodoTag>()
                .HasOne(tt => tt.Todo)
                .WithMany(t => t.TodoTags)
                .HasForeignKey(tt => tt.TodoId);

            modelBuilder.Entity<TodoTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TodoTags)
                .HasForeignKey(tt => tt.TagId);
        }
    }

}
