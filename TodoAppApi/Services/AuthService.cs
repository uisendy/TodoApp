using TodoAppApi.DTOs.Auth;
using TodoAppApi.Entities;
using TodoAppApi.Helpers;
using TodoAppApi.Interfaces;
using TodoAppApi.Services.Security;

namespace TodoAppApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly ITokenManager _tokenManager;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository repo, ITokenManager tokenManager, IEmailService emailService)
        {
            _repo = repo;
            _tokenManager = tokenManager;
            _emailService = emailService;
        }

        public async Task<User> RegisterAsync(RegisterRequestDto request)
        {
            var existing = await _repo.GetUserByEmailAsync(request.Email);
            if (existing != null)
                throw new Exception("User already exists");

            var otp = OTPHelper.GenerateOtp();
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Otp = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddUserAsync(user);
            await _repo.SaveChangesAsync();
            await _emailService.SendEmailOtpAsync(user.Email, otp);

            return user;
        }

        public async Task<User> LoginAsync(LoginRequestDto request)
        {
            var user = await _repo.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            if (!user.IsVerified)
                throw new Exception("Email not verified");

            return user;
        }

        public async Task SendOtpAsync(string email)
        {
            var user = await _repo.GetUserByEmailAsync(email);
            if (user == null) throw new Exception("User not found");
            if (user.IsVerified) throw new Exception("User already verified");

            var otp = OTPHelper.GenerateOtp();
            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

            await _repo.SaveChangesAsync();
            await _emailService.SendEmailOtpAsync(user.Email, otp);
        }

        public async Task<bool> VerifyOtpAsync(OtpVerificationRequestDto request)
        {
            var user = await _repo.GetUserByIdAsync(request.UserId);
            if (user == null || user.IsVerified) return false;

            if (user.Otp != request.Otp || user.OtpExpiry < DateTime.UtcNow)
                return false;

            user.IsVerified = true;
            user.Otp = null;
            user.OtpExpiry = null;

            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var user = await _repo.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            var tokens = _tokenManager.GenerateAndHashTokens(user);

            user.RefreshToken = TokenHashHelper.HashToken(tokens.RefreshToken);
            user.RefreshTokenExpiry = tokens.RefreshTokenExpiry;
            await _repo.SaveChangesAsync();

            return (tokens.AccessToken, tokens.RefreshToken);
        }
    }
}
