using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Beddin.API.Middleware
{
    public class PasswordPolicyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public PasswordPolicyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            var mustChange = context.User.FindFirst("must_change_password")?.Value == "true";
            var path = context.Request.Path.Value ?? "";

            // Read allowed URL from configuration
            var allowedPath = _configuration["AllowedUrl:PasswordChange"] ?? "";
            var isAllowedPath = path.Contains(allowedPath, StringComparison.OrdinalIgnoreCase);

            if (isAuthenticated && mustChange && !isAllowedPath)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Password change required.",
                    mustChangePassword = true
                });
                return;
            }

            await _next(context);
        }
    }
}