using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;
using TodoAppApi.Infrastructure.Mappings;
using TodoAppApi.Repositories;
using TodoAppApi.Services;
using TodoAppApi.Tests.Helpers;
using Xunit;

namespace TodoAppApi.Tests.Services
{
    public class TodoServiceTests
    {
        private readonly IMapper _mapper;

        public TodoServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TodoMappingProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task CreateTodoAsync_ShouldReturnCreatedTodo()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var service = new TodoService(repo, _mapper);

            var request = new TodoRequestDto
            {
                Name = "Creating My first Todo Title",
                Description = "Just consider this as a new Todo i am doing",
                Priority = PriorityLevel.Low
            };

            var userId = Guid.NewGuid();

            var created = await service.CreateTodoAsync(userId, request);

            Assert.NotNull(created);
            Assert.Equal("Creating My first Todo Title", created.Name);
            Assert.Equal("Just consider this as a new Todo i am doing", created.Description);
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldUpdateTodo()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var service = new TodoService(repo, _mapper);

            var userId = Guid.NewGuid();
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Buy Bag",
                Description = "Buy bag from Obalende for 50k",
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(todo);
            await repo.SaveChangesAsync();

            var updateRequest = new TodoRequestDto
            {
                Name = "Buy the cheaper bag",
                Description = "Lets Buy the cheaper bag, maintain steeze and still hold change for Pocket",
                Priority = PriorityLevel.High
            };

            var updated = await service.UpdateTodoAsync(userId, todo.Id, updateRequest);

            Assert.NotNull(updated);
            Assert.Equal("Buy the cheaper bag", updated.Name);
            Assert.Equal("Lets Buy the cheaper bag, maintain steeze and still hold change for Pocket", updated.Description);
            Assert.Equal(PriorityLevel.High, updated.Priority);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ShouldReturnTodo()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var service = new TodoService(repo, _mapper);

            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = "Shop Construction",
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(todo);
            await repo.SaveChangesAsync();

            var result = await service.GetTodoByIdAsync(todo.Id);

            Assert.NotNull(result);
            Assert.Equal("Shop Construction", result.Name);
        }

        [Fact]
        public async Task GetTodosByUserIdAsync_ShouldReturnOnlyUserTodos()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var service = new TodoService(repo, _mapper);

            var userId = Guid.NewGuid();
            await repo.AddAsync(new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Todo 1",
                CreatedAt = DateTime.UtcNow
            });

            await repo.AddAsync(new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), 
                Name = "Other User Todo",
                CreatedAt = DateTime.UtcNow
            });

            await repo.SaveChangesAsync();

            var result = await service.GetTodosByUserIdAsync(userId);

            Assert.Single(result);
            Assert.Equal("Todo 1", result.First().Name);
        }

        [Fact]
        public async Task GetAllTodosAsync_ShouldReturnAll()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var service = new TodoService(repo, _mapper);

            await repo.AddAsync(new Todo { Id = Guid.NewGuid(), Name = "Todo A", CreatedAt = DateTime.UtcNow });
            await repo.AddAsync(new Todo { Id = Guid.NewGuid(), Name = "Todo B", CreatedAt = DateTime.UtcNow });
            await repo.SaveChangesAsync();

            var allTodos = await service.GetAllTodosAsync();

            Assert.Equal(2, allTodos.Count());
        }

        [Fact]
        public async Task ArchiveTodoAsync_ShouldMarkTodoAsArchived()
        {
            using var db = TestDbContextFactory.CreateInMemoryContext();
            var repo = new TodoRepository(db);
            var service = new TodoService(repo, _mapper);

            var userId = Guid.NewGuid();
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "01/01/2024 - Attend Management meeting ",
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(todo);
            await repo.SaveChangesAsync();

            var result = await service.ArchiveTodoAsync(userId, todo.Id);

            var updated = await repo.GetByIdAsync(todo.Id);

            Assert.True(result);
            Assert.True(updated!.IsArchived);
        }
    }
}
