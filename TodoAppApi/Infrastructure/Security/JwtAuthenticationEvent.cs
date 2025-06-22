using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Infrastructure.Security
{
    public class JwtAuthenticationEvents : JwtBearerEvents
    {
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(@"
                    {
                        ""status"": ""error"",
                        ""message"": ""Token has expired"",
                        ""data"": null
                    }");
            }

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(@"
                {
                    ""status"": ""error"",
                    ""message"": ""Authentication failed"",
                    ""data"": null
                }");
        }

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            if (!context.Response.HasStarted)
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(@"
                    {
                        ""status"": ""error"",
                        ""message"": ""Unauthorized"",
                        ""data"": null
                    }");
            }

            return Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var userIdStr = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var tokenJti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            
            if (string.IsNullOrWhiteSpace(userIdStr) || string.IsNullOrWhiteSpace(tokenJti))
            {
                context.Fail("Token is missing required claims.");
                return;
            }

           
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                context.Fail("Invalid user ID format.");
                return;
            }

            var repo = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
            var user = await repo.GetUserByIdAsync(userId);

            if (user == null || user.CurrentJti != tokenJti)
            {
                context.Fail("Token has been revoked or is invalid.");
            }
        }
    }
}
