using Microsoft.AspNetCore.Mvc.Filters;
using TodoAppApi.Infrastructure.Security;

namespace TodoAppApi.Infrastructure.Filters
{
    public class TokenHeaderFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var httpContext = context.HttpContext;

            if (httpContext.Items.TryGetValue("TokenContext", out var value) && value is TokenContext token)
            {
                if (!string.IsNullOrWhiteSpace(token.AccessToken))
                    httpContext.Response.Headers["X-Access-Token"] = token.AccessToken;

                if (!string.IsNullOrWhiteSpace(token.RefreshToken))
                    httpContext.Response.Headers["X-Refresh-Token"] = token.RefreshToken;
            }
        }
    }
}
