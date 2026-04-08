// <copyright file="SessionValidationMiddleware.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Application.Common.Interfaces;

namespace Beddin.API.Middleware
{
    /// <summary>
    /// Middleware that validates the user's session for authenticated requests, ensuring that the session is still active in the database. If the session is invalid, it returns a 401 Unauthorized response. Otherwise, it allows the request to proceed to the next middleware or endpoint.
    /// </summary>
    public class SessionValidationMiddleware
    {
        // These paths skip session validation
        private static readonly string[] ExcludedPaths =
        [
            "/Authentication/Login",
            "/Authentication/Logout",
            "/swagger",
            "/health",
            "/hangfire"
        ];

        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionValidationMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public SessionValidationMiddleware(RequestDelegate next) => this.next = next;

        /// <summary>
        /// Invokes the middleware to validate the user's session for authenticated requests, ensuring that the session is still active in the database. If the session is invalid, it returns a 401 Unauthorized response. Otherwise, it allows the request to proceed to the next middleware or endpoint.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="sessionRepo">The user session repository for validating sessions.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, IUserSessionRepository sessionRepo)
        {
            // Skip unauthenticated requests and excluded paths
            var path = context.Request.Path.Value ?? string.Empty;
            var isExcluded = ExcludedPaths.Any(p => path.StartsWith(
                p,
                StringComparison.OrdinalIgnoreCase));

            if (isExcluded || !context.User.Identity?.IsAuthenticated == true)
            {
                await this.next(context);
                return;
            }

            // Extract session ID from token claims
            var sessionIdClaim = context.User.FindFirst("session_id")?.Value;
            if (!Guid.TryParse(sessionIdClaim, out var sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errors = new[] { "Invalid session." },
                });
                return;
            }

            // Validate session is still active in DB
            var session = await sessionRepo.GetById(sessionId);
            if (session is null || !session.IsActive)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errors = new[] { "Session has been invalidated. Please log in again." },
                });
                return;
            }

            await this.next(context);
        }
    }
}
