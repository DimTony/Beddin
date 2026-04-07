// <copyright file="SessionCleanupJob.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Beddin.Infrastructure.Persistence.BackgroundJobs
{
    /// <summary>
    /// Represents a background job responsible for cleaning up invalidated or expired user sessions from the database. This job periodically runs to ensure that stale session records are removed, improving database performance and security by preventing unauthorized access through old sessions. The job uses Entity Framework Core to query and delete sessions that have either been explicitly invalidated or have passed their expiration time. Logging is included to track the number of sessions deleted and the time of cleanup operations.
    /// </summary>
    public class SessionCleanupJob
    {
        private readonly AppDbContext db;
        private readonly ILogger<SessionCleanupJob> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionCleanupJob"/> class.
        /// </summary>
        /// <param name="db">The database context used for accessing user sessions.</param>
        /// <param name="logger">The logger instance used for logging information and errors.</param>
        public SessionCleanupJob(AppDbContext db, ILogger<SessionCleanupJob> logger)
        {
            this.db = db;
            this.logger = logger;
        }

        /// <summary>
        /// Executes the session cleanup operation by deleting all user sessions that have either been invalidated or have expired. The method queries the database for sessions that meet these criteria and performs a bulk delete operation. After the cleanup, it logs the number of sessions deleted and the time of the operation for monitoring purposes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            var now = DateTime.UtcNow;

            var deleted = await this.db.UserSessions
                .Where(s => s.InvalidatedAt != null || s.ExpiresAt <= now)
                .ExecuteDeleteAsync();

            this.logger.LogInformation("Deleted {Count} invalidated sessions at {Time}", deleted, now);
        }
    }
}
