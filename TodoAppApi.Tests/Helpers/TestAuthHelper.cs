using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoAppApi.Controllers.v1;
using TodoAppApi.Entities;
using TodoAppApi.Infrastructure.Security;

namespace TodoAppApi.Tests.Helpers;

public static class TestAuthHelper
{
    public static (Guid userId, string token, User user) GenerateTestToken()
    {
        var config = GetFakeJwtConfig();
        var tokenService = new TokenService(config);

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "testuser@example.com", 
            FirstName = "firstName",
            LastName="lastName",
            PasswordHash="hasPassword"
            
        };

        var token = tokenService.GenerateToken(user);
        return (userId, token, user);
    }

    public static void SetupAuthenticatedUser(ControllerBase controller, Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };
    }

    private static IConfiguration GetFakeJwtConfig()
    {
        var configValues = new Dictionary<string, string>
        {
            { "Jwt:Secret", "THIS_IS_A_UNIT_TEST_SECRET_KEY_1234567890" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }
}
