using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using TodoAppApi.Entities;
using TodoAppApi.Repositories;
using TodoAppApi.Tests.Helpers;
using Xunit;

namespace TodoAppApi.Tests.Repositories
{
    public class TodoRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddTodoToDatabase()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);

            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Test Todo",
                Description = "Unit test",
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(todo);
            await repo.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(todo.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Test Todo", fetched.Name);
        }

        [Fact]
        public async Task ArchiveAsync_ShouldMarkTodoAsArchived()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var userId = Guid.NewGuid();

            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Archivable Todo",
                CreatedAt = DateTime.UtcNow
            };

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            var result = await repo.ArchiveAsync(userId, todo.Id);
            await repo.SaveChangesAsync();

            var updated = await repo.GetByIdAsync(todo.Id);

            Assert.True(result);
            Assert.True(updated!.IsArchived);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectTodo()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var todoId = Guid.NewGuid();

            var todo = new Todo
            {
                Id = todoId,
                UserId = Guid.NewGuid(),
                Name = "Fetch Me",
                CreatedAt = DateTime.UtcNow
            };

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(todoId);

            Assert.NotNull(fetched);
            Assert.Equal(todoId, fetched.Id);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnOnlyNonArchivedTodos()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var userId = Guid.NewGuid();

            var todo1 = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Active",
                CreatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            var todo2 = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Archived",
                IsArchived = true,
                CreatedAt = DateTime.UtcNow
            };

            await db.Todos.AddRangeAsync(todo1, todo2);
            await db.SaveChangesAsync();

            var userTodos = await repo.GetByUserIdAsync(userId);

            Assert.Single(userTodos);
            Assert.Equal("Active", userTodos.First().Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTodos()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);

            await db.Todos.AddRangeAsync(
                new Todo { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Todo 1", CreatedAt = DateTime.UtcNow },
                new Todo { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Todo 2", CreatedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();

            var todos = await repo.GetAllAsync();

            Assert.Equal(2, todos.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTodoFields()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var userId = Guid.NewGuid();

            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Old Name",
                Description = "Old Desc",
                CreatedAt = DateTime.UtcNow
            };

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            var update = new Todo
            {
                Name = "New Name",
                Description = "New Description",
                Priority = PriorityLevel.Low
            };

            var updated = await repo.UpdateAsync(userId, todo.Id, update);
            await repo.SaveChangesAsync();

            Assert.NotNull(updated);
            Assert.Equal("New Name", updated!.Name);
            Assert.Equal("New Description", updated.Description);
            Assert.Equal(PriorityLevel.Low, updated.Priority);
        }

        [Fact]
        public async Task DeleteOldArchivedAsync_ShouldRemoveOldArchivedTodos()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);

            var oldArchived = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Old Archived",
                IsArchived = true,
                ArchivedAt = DateTime.UtcNow.AddDays(-40),
                CreatedAt = DateTime.UtcNow.AddDays(-50)
            };

            var recentArchived = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Recent Archived",
                IsArchived = true,
                ArchivedAt = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            };

            await db.Todos.AddRangeAsync(oldArchived, recentArchived);
            await db.SaveChangesAsync();

            var deletedCount = await repo.DeleteOldArchivedAsync(DateTime.UtcNow.AddDays(-30));

            Assert.Equal(1, deletedCount);
            Assert.Null(await db.Todos.FindAsync(oldArchived.Id));
            Assert.NotNull(await db.Todos.FindAsync(recentArchived.Id));
        }
    }
}
