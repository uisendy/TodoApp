using System.IdentityModel.Tokens.Jwt;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAppApi.DTOs.Common;
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
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            
            try
            {
                var userId = HttpContext.User.GetUserId();
                var userProfile = await _userService.GetUserProfileAsync(userId);
                return Ok(ApiResponseDto<UserResponseDto>.Success(userProfile));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<string>.Error(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: ex.Message);
                return StatusCode(500, ApiResponseDto<string>.Error("An unexpected error occurred."));
            }
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequestDto dto)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var updatedUser = await _userService.UpdateUserProfileAsync(userId, dto);
                return Ok(ApiResponseDto<UserResponseDto>.Success(updatedUser, "User Profile Updated Successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: ex.Message);
                return StatusCode(500, ApiResponseDto<string>.Error("Failed to update profile"));
            }
        }

    }
}
