// <copyright file="HangfireAuthorizationFilter.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Hangfire.Dashboard;

namespace Beddin.Application.Common.Helpers
{
    /// <summary>
    /// Provides an authorization filter for Hangfire dashboard access.
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Determines whether the current user is authorized to access the Hangfire dashboard.
        /// </summary>
        /// <param name="context">The dashboard context.</param>
        /// <returns>
        /// <c>true</c> if the user is authenticated and has the Admin role; otherwise, <c>false</c>.
        /// </returns>
        /// <inheritdoc/>
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Example: only allow authenticated users with Admin role
            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.IsInRole("Admin");
        }
    }
}
