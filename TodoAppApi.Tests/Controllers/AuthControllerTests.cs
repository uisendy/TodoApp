using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using TodoAppApi.Controllers.v1;
using TodoAppApi.DTOs.Auth;
using TodoAppApi.DTOs.Common;
using TodoAppApi.DTOs.Security;
using TodoAppApi.Entities;
using TodoAppApi.Infrastructure.Filters;
using TodoAppApi.Infrastructure.Security;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<ITokenManager> _tokenManagerMock = new();
        private readonly Mock<IAuthRepository> _repoMock = new();

        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _controller = new AuthController(
                _authServiceMock.Object,
                _tokenManagerMock.Object,
                _repoMock.Object
            );
        }

        [Fact]
        public async Task Register_ReturnsCreated_WhenRegistrationSuccessful()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "securePassword"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = "hashed"
            };

            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(user);

            // Act
            var result = await _controller.Register(request) as CreatedResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                Email = "john@example.com",
                Password = "securePassword"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = request.Password,
                IsVerified = true
            };

            var tokenResult = new TokenResultDto
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
                            .ReturnsAsync(user);

            _tokenManagerMock.Setup(t => t.GenerateTokens(user))
                             .Returns(tokenResult);

            _repoMock.Setup(r => r.SaveChangesAsync())
                     .Returns(Task.CompletedTask);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var actionResult = await _controller.Login(request);

            if (actionResult is BadRequestObjectResult badResult)
            {
                Console.WriteLine("Login failed: " + badResult.Value);
            }
            else if (actionResult is OkObjectResult okResult)
            {
                Console.WriteLine("Login succeeded");
            }


            // Assert
            var result = Assert.IsType<OkObjectResult>(actionResult);

            Assert.Equal(200, result.StatusCode);

            var response = Assert.IsType<ApiResponseDto<AuthResponseDto>>(result.Value);
            Assert.NotNull(response);
            Assert.NotNull(response.Data);
            Assert.Equal(user.Email, response.Data.Email);
            Assert.Equal(user.FirstName, response.Data.FirstName);
            Assert.Equal(user.LastName, response.Data.LastName);
        }


        [Fact]
        public async Task SendOtp_ReturnsOk_WhenUserIsUnverified()
        {
            var email = "test@example.com";

            _authServiceMock.Setup(s => s.SendOtpAsync(email)).Returns(Task.CompletedTask);

            var result = await _controller.SendOtp(email) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task VerifyOtp_ReturnsOk_WhenOtpIsValid()
        {
            var request = new OtpVerificationRequestDto
            {
                UserId = Guid.NewGuid(),
                Otp = "123456"
            };

            _authServiceMock.Setup(s => s.VerifyOtpAsync(request)).ReturnsAsync(true);

            var result = await _controller.VerifyOtp(request) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOk_WhenRefreshTokenIsValid()
        {
            // Arrange
            string oldToken = "old-token";
            var tokenResult = new TokenResultDto
            {
                AccessToken = "new-access",
                RefreshToken = "new-refresh",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            var httpContext = new DefaultHttpContext();

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            var context = new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                _controller
            );

            httpContext.Items["TokenContext"] = new TokenContext
            {
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _authServiceMock.Setup(s => s.RefreshTokenAsync(oldToken))
                            .ReturnsAsync((tokenResult.AccessToken, tokenResult.RefreshToken));

            // Act
            var result = await _controller.RefreshToken(oldToken) as OkObjectResult;

            // Manually invoke the filter to simulate action finishing
            var filter = new TokenHeaderFilter();
            filter.OnActionExecuted(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("new-access", httpContext.Response.Headers["X-Access-Token"]);
            Assert.Equal("new-refresh", httpContext.Response.Headers["X-Refresh-Token"]);
        }

        [Fact]
        public void TokenHeaderFilter_AddsTokensToResponseHeaders_WhenTokenContextExists()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["TokenContext"] = new TokenContext
            {
                AccessToken = "test-access",
                RefreshToken = "test-refresh"
            };

            var context = new ActionExecutedContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                controller: null
            );

            var filter = new TokenHeaderFilter();
            filter.OnActionExecuted(context);

            Assert.Equal("test-access", httpContext.Response.Headers["X-Access-Token"]);
            Assert.Equal("test-refresh", httpContext.Response.Headers["X-Refresh-Token"]);
        }

    }
}
