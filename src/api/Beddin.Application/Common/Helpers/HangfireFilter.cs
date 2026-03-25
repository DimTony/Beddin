using Hangfire.Dashboard;

namespace Beddin.Application.Common.Helpers
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Example: only allow authenticated users with Admin role
            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.IsInRole("Admin");
        }
    }
}
