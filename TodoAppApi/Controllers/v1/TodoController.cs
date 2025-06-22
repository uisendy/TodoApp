using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAppApi.DTOs.Common;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;
using TodoAppApi.Infrastructure.Extensions;

namespace TodoAppApi.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/todos")]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTodos()
        {
            var userId = User.GetUserId();
            var todos = await _todoService.GetTodosByUserIdAsync(userId);
            return Ok(ApiResponseDto<IEnumerable<TodoResponseDto>>.Success(todos));
        }

        [HttpGet("priorities")]
        public IActionResult GetPriorities()
        {
            var priorities = _todoService.GetTodoPriorities();
            return Ok(ApiResponseDto<IEnumerable<PriorityDto>>.Success(priorities));
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetTodoTags()
        {
            var tags = await _todoService.GetTodoTagsAsync();
            return Ok(ApiResponseDto<IEnumerable<TodoTagDto>>.Success(tags));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            return Ok(ApiResponseDto<TodoResponseDto>.Success(todo));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] TodoRequestDto request)
        {
            var userId = User.GetUserId();
            var todo = await _todoService.CreateTodoAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, ApiResponseDto<TodoResponseDto>.Success(todo));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoRequestDto request)
        {
            var userId = User.GetUserId();
            var updated = await _todoService.UpdateTodoAsync(userId, id, request);
            return Ok(ApiResponseDto<TodoResponseDto>.Success(updated));
        }

        [HttpPatch("{id}/toggle-completion")]
        public async Task<IActionResult> ToggleCompletion(Guid id)
        {
            var userId = User.GetUserId();
            var updatedTodo = await _todoService.ToggleCompletionAsync(userId, id);
            return Ok(ApiResponseDto<TodoResponseDto>.Success(updatedTodo));
        }


        [HttpPut("{id}/archive")]
        public async Task<IActionResult> Archive(Guid id)
        {
            var userId = User.GetUserId();
            var success = await _todoService.ArchiveTodoAsync(userId, id);
            return success
                ? Ok(ApiResponseDto<string>.Success("Todo archived"))
                : NotFound(ApiResponseDto<string>.Error("Todo not found"));
        }
    }

}
