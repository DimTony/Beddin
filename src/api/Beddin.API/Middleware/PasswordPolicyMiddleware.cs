// <copyright file="PasswordPolicyMiddleware.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Beddin.API.Middleware
{
    /// <summary>
    /// Middleware that enforces password change policy for authenticated users.
    /// If the user is required to change their password, access to all endpoints except the allowed password change URL is forbidden.
    /// </summary>
    public class PasswordPolicyMiddleware
    {
        /// <summary>
        /// The next middleware in the pipeline.
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The application configuration.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordPolicyMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="configuration">The application configuration.</param>
        public PasswordPolicyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        /// <summary>
        /// Invokes the middleware logic to enforce password change policy.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            var mustChange = context.User.FindFirst("must_change_password")?.Value == "true";
            var path = context.Request.Path.Value ?? string.Empty;

            // Read allowed URL from configuration
            var allowedPath = this.configuration["AllowedUrl:PasswordChange"] ?? string.Empty;
            var isAllowedPath = path.Contains(allowedPath, StringComparison.OrdinalIgnoreCase);

            if (isAuthenticated && mustChange && !isAllowedPath)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Password change required.",
                    mustChangePassword = true,
                });
                return;
            }

            await this.next(context);
        }
    }
}
