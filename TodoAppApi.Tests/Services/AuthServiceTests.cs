using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TodoAppApi.DTOs.Auth;
using TodoAppApi.DTOs.Security;
using TodoAppApi.Entities;
using TodoAppApi.Interfaces;
using TodoAppApi.Services;

namespace TodoAppApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _repoMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Mock<ITokenManager> _tokenManagerMock = new();

        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _service = new AuthService(
                _repoMock.Object,
                _tokenManagerMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenEmailNotTaken()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "securePassword"
            };

            _repoMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                     .ReturnsAsync((User)null);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.Equal(request.Email, result.Email);
            _emailServiceMock.Verify(e => e.SendEmailOtpAsync(result.Email, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenInvalidCredentials()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "wrong@example.com",
                Password = "wrong"
            };

            _repoMock.Setup(r => r.GetUserByEmailAsync(request.Email))
                     .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.LoginAsync(request));
        }

        [Fact]
        public async Task SendOtpAsync_ShouldSendOtp_WhenUserExistsAndNotVerified()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedPassword",
                IsVerified = false
            };

            _repoMock.Setup(r => r.GetUserByEmailAsync(email))
                     .ReturnsAsync(user);

            // Act
            await _service.SendOtpAsync(email);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailOtpAsync(email, It.IsAny<string>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SendOtpAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var email = "missing@example.com";
            _repoMock.Setup(r => r.GetUserByEmailAsync(email))
                     .ReturnsAsync((User)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.SendOtpAsync(email));
            Assert.Equal("User not found", ex.Message);
        }

        [Fact]
        public async Task VerifyOtpAsync_ShouldReturnFalse_WhenOtpIsWrong()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                FirstName = "TestFN",
                LastName =  "TestLN",
                Email = "TestEmail@mail.com",
                PasswordHash = "PasswordHash12=",
                Id = userId,
                Otp = "123456",
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                IsVerified = false
            };

            var request = new OtpVerificationRequestDto
            {
                UserId = userId,
                Otp = "000000"
            };

            _repoMock.Setup(r => r.GetUserByIdAsync(userId))
                     .ReturnsAsync(user);

            // Act
            var result = await _service.VerifyOtpAsync(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task VerifyOtpAsync_ShouldReturnTrue_WhenOtpIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otp = "123456";
            var user = new User
            {
                FirstName = "TestFN",
                LastName = "TestLN",
                Email = "TestEmail@mail.com",
                PasswordHash = "PasswordHash12=",
                Id = userId,
                Otp = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                IsVerified = false
            };

            var request = new OtpVerificationRequestDto
            {
                UserId = userId,
                Otp = otp
            };

            _repoMock.Setup(r => r.GetUserByIdAsync(userId))
                     .ReturnsAsync(user);

            // Act
            var result = await _service.VerifyOtpAsync(request);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenValid()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var user = new User
            {
                FirstName = "TestFN",
                LastName = "TestLN",
                Email = "TestEmail@mail.com",
                PasswordHash = "PasswordHash12=",
                Id = Guid.NewGuid(),
                RefreshToken = "hashed-token",
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30)
            };

            _repoMock.Setup(r => r.GetUserByRefreshTokenAsync(refreshToken))
                     .ReturnsAsync(user);

            _tokenManagerMock.Setup(tm => tm.GenerateAndHashTokens(user))
                             .Returns(new TokenResultDto
                             {
                                 AccessToken = "new-access-token",
                                 RefreshToken = "new-refresh-token",
                                 RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
                             });

            // Act
            var (accessToken, newRefreshToken) = await _service.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.Equal("new-access-token", accessToken);
            Assert.Equal("new-refresh-token", newRefreshToken);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

    }
}
