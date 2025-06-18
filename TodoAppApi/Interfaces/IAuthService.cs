using TodoAppApi.DTOs.Auth;
using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterRequestDto request);
        Task<User> LoginAsync(LoginRequestDto request);
        Task SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(OtpVerificationRequestDto request);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    }
}
