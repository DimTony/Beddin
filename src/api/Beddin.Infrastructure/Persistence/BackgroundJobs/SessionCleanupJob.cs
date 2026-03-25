using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Beddin.Infrastructure.Persistence.BackgroundJobs
{
    public class SessionCleanupJob
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SessionCleanupJob> _logger;

        public SessionCleanupJob(AppDbContext db, ILogger<SessionCleanupJob> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var now = DateTime.UtcNow;

            var deleted = await _db.UserSessions
                .Where(s => s.InvalidatedAt != null || s.ExpiresAt <= now)
                .ExecuteDeleteAsync();

            _logger.LogInformation("Deleted {Count} invalidated sessions at {Time}", deleted, now);
        }
    }
}
