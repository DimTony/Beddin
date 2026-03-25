using Beddin.Infrastructure.Persistence.BackgroundJobs;
using Hangfire;

namespace Beddin.API.Extensions
{
    public static class HangfireExtensions
    {
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
