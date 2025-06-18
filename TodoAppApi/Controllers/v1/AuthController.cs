using Microsoft.AspNetCore.Mvc;
using TodoAppApi.DTOs.Auth;
using TodoAppApi.DTOs.Common;
using TodoAppApi.Helpers;
using TodoAppApi.Infrastructure.Security;
using TodoAppApi.Interfaces;
using Asp.Versioning;

namespace TodoAppApi.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenManager _tokenManager;
        private readonly IAuthRepository _repo;

        public AuthController(IAuthService authService, ITokenManager tokenManager, IAuthRepository repo)
        {
            _authService = authService;
            _tokenManager = tokenManager;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);

                var response = new AuthResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                return Created(string.Empty, ApiResponseDto<AuthResponseDto>.Success(
                    response,
                    "Registration successful. Please verify the OTP sent to your email."
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseDto<string>.Error(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            try
            {
                var user = await _authService.LoginAsync(request);

                var tokens = _tokenManager.GenerateAndHashTokens(user);

                user.RefreshToken = TokenHashHelper.HashToken(tokens.RefreshToken);
                user.RefreshTokenExpiry = tokens.RefreshTokenExpiry;
                await _repo.SaveChangesAsync();

                HttpContext.Items["TokenContext"] = new TokenContext
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                };


                var response = new AuthResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                return Ok(ApiResponseDto<AuthResponseDto>.Success(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseDto<string>.Error(ex.Message));
            }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            try
            {
                await _authService.SendOtpAsync(email);
                return Ok(ApiResponseDto<string>.Success("OTP sent successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseDto<string>.Error(ex.Message));
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(OtpVerificationRequestDto request)
        {
            var success = await _authService.VerifyOtpAsync(request);
            if (!success)
                return BadRequest(ApiResponseDto<string>.Error("Invalid or expired OTP"));

            return Ok(ApiResponseDto<string>.Success("Verification successful"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromHeader(Name = "X-Refresh-Token")] string refreshToken)
        {
            try
            {
                var (accessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);

                HttpContext.Items["TokenContext"] = new TokenContext
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
                };

                return Ok(ApiResponseDto<string>.Success());
            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponseDto<string>.Error(ex.Message));
            }
        }
    }
}
