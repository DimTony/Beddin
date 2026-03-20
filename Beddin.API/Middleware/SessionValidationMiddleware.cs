using Beddin.Application.Common.Interfaces;

namespace Beddin.API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        // These paths skip session validation
        private static readonly string[] ExcludedPaths =
        [
            "/api/auth/login",
        "/swagger",
        "/health"
        ];

        public SessionValidationMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IUserSessionRepository sessionRepo)
        {
            // Skip unauthenticated requests and excluded paths
            var path = context.Request.Path.Value ?? string.Empty;
            var isExcluded = ExcludedPaths.Any(p => path.StartsWith(p,
                StringComparison.OrdinalIgnoreCase));

            if (isExcluded || !context.User.Identity?.IsAuthenticated == true)
            {
                await _next(context);
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
                    errors = new[] { "Invalid session." }
                });
                return;
            }

            // Validate session is still active in DB
            var session = await sessionRepo.GetByIdAsync(sessionId);
            if (session is null || !session.IsActive)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errors = new[] { "Session has been invalidated. Please log in again." }
                });
                return;
            }

            await _next(context);
        }
    }
}
