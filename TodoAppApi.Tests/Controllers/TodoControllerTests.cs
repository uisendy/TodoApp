using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAppApi.Controllers.v1;
using TodoAppApi.Data;
using TodoAppApi.DTOs.Common;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;
using TodoAppApi.Infrastructure.Mappings;
using TodoAppApi.Repositories;
using TodoAppApi.Services;
using TodoAppApi.Tests.Helpers;

namespace TodoAppApi.Tests.Controllers;

public class TodoControllerTests
{
    private readonly TodoController _controller;
    private readonly AppDbContext _context;
    private readonly Guid _userId;

    public TodoControllerTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();

        var repo = new TodoRepository(_context);
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<TodoMappingProfile>()).CreateMapper();
        var service = new TodoService(repo, mapper);

        _controller = new TodoController(service);

        (_userId, var token, var user) = TestAuthHelper.GenerateTestToken();
        TestAuthHelper.SetupAuthenticatedUser(_controller, _userId);
    }



    [Fact]
    public async Task GetById_ReturnsTodo_WhenExists()
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Name = "GetById Todo",
            Description = "Desc",
            UserId = _userId,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Todos.AddAsync(todo);
        await _context.SaveChangesAsync();

        var result = await _controller.GetById(todo.Id);
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponseDto<TodoResponseDto>>(ok.Value);

        Assert.Equal(todo.Id, response.Data.Id);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsAll()
    {
        await _context.Todos.AddRangeAsync(
            new Todo { Id = Guid.NewGuid(), Name = "1", UserId = _userId, CreatedAt = DateTime.UtcNow },
            new Todo { Id = Guid.NewGuid(), Name = "2", UserId = _userId, CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var repo = new TodoRepository(_context);
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<TodoMappingProfile>()).CreateMapper();
        var service = new TodoService(repo, mapper);

        var todos = await service.GetAllTodosAsync();

        Assert.True(todos.Count() >= 2);
    }
}
