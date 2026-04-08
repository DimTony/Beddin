// <copyright file="HangfireExtensions.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Infrastructure.Persistence.BackgroundJobs;
using Hangfire;

namespace Beddin.API.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring Hangfire background jobs in the application. This class contains methods that can be called during the application's startup configuration to set up recurring background jobs using Hangfire. By using these extension methods, you can easily register and manage background tasks that need to run at specific intervals or schedules, such as cleaning up expired sessions or performing other maintenance tasks. This helps ensure that the application remains performant and efficient by offloading time-consuming operations to background processing.
    /// </summary>
    public static class HangfireExtensions
    {
        /// <summary>
        /// Configures recurring background jobs using Hangfire. This method sets up a recurring job for cleaning up expired sessions by scheduling the SessionCleanupJob to run at regular intervals (e.g., hourly). By calling this extension method in the application's startup configuration, you can ensure that the necessary background tasks are registered and executed as needed to maintain the application's performance and resource management.
        /// </summary>
        /// <param name="app">The web application builder used to configure the application.</param>
        /// <returns>The configured web application builder.</returns>
        public static WebApplication UseRecurringJobs(this WebApplication app)
        {
            RecurringJob.AddOrUpdate<SessionCleanupJob>(
                recurringJobId: "session-cleanup",
                methodCall: job => job.RunAsync(),
                cronExpression: Cron.Hourly);   // or "0 */2 * * *" for every 2 hours

            return app;
        }
    }
}
