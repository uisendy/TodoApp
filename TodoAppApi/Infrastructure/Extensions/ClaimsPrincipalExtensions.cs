using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Serilog;

namespace TodoAppApi.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return sub != null && Guid.TryParse(sub, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Unauthorized");
        }
    }
}