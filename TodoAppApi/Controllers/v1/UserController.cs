using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAppApi.DTOs.Common;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.DTOs.Users;
using TodoAppApi.Infrastructure.Extensions;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.GetUserId();
                var userProfile = await _userService.GetUserProfileAsync(userId);
                return Ok(ApiResponseDto<UserResponseDto>.Success(userProfile));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<string>.Error(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseDto<string>.Error("An unexpected error occurred."));
            }
        }

    }
}
