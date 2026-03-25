using Beddin.API.Middleware;
using Beddin.Application.Common.Helpers;
using Beddin.Domain.Aggregates.Users;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Seed;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StackExchange.Redis;

namespace Beddin.API.Extensions
{
    public static class ApplicationExtensions
    {
        // Fixed advisory lock key used to elect a single migration leader across replicas.
        // Value is derived from the ASCII encoding of "beddin-migrate" to minimise the chance
        // of colliding with advisory locks used by other tools (e.g. Hangfire, pg_partman).
        private const long MigrationAdvisoryLockKey = 7367473L;

        // How long a non-leader replica waits for the leader to finish migrations.
        private static readonly TimeSpan MigrationLockTimeout = TimeSpan.FromMinutes(2);

        // How often non-leader replicas poll to check whether the leader has finished.
        private static readonly TimeSpan MigrationLockPollInterval = TimeSpan.FromSeconds(5);

        public static async Task<IApplicationBuilder> ApplyMigrationsAsync(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();

            // Guard: only run automatic migrations when explicitly enabled via configuration.
            if (!configuration.GetValue<bool>("Database:AutoMigrate"))
            {
                logger.LogInformation("Automatic database migration is disabled (Database:AutoMigrate is not set)");
                return app;
            }

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is not configured. " +
                    "Database migrations cannot proceed.");
            var db = services.GetRequiredService<AppDbContext>();

            try
            {
                // Open a dedicated connection to hold the Postgres session-level advisory lock.
                // This ensures that in a multi-replica deployment only one instance applies
                // migrations while the others wait, preventing DDL lock contention.
                await using var lockConnection = new NpgsqlConnection(connectionString);
                await lockConnection.OpenAsync();

                bool lockAcquired;
                await using (var tryLockCmd = lockConnection.CreateCommand())
                {
                    tryLockCmd.CommandText = "SELECT pg_try_advisory_lock(@key)";
                    tryLockCmd.Parameters.AddWithValue("key", MigrationAdvisoryLockKey);
                    var result = await tryLockCmd.ExecuteScalarAsync();
                    lockAcquired = result is bool b && b;
                }

                if (lockAcquired)
                {
                    try
                    {
                        logger.LogInformation("Migration lock acquired. Applying database migrations...");
                        await db.Database.MigrateAsync();
                        logger.LogInformation("Database migrations applied successfully");
                    }
                    finally
                    {
                        await using var unlockCmd = lockConnection.CreateCommand();
                        unlockCmd.CommandText = "SELECT pg_advisory_unlock(@key)";
                        unlockCmd.Parameters.AddWithValue("key", MigrationAdvisoryLockKey);
                        await unlockCmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    // Another replica is applying migrations; poll until the lock is released.
                    logger.LogInformation("Another instance is applying database migrations, waiting for it to complete...");

                    var timeout = MigrationLockTimeout;
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var migrationComplete = false;

                    while (stopwatch.Elapsed < timeout)
                    {
                        await Task.Delay(MigrationLockPollInterval);

                        await using var checkCmd = lockConnection.CreateCommand();
                        checkCmd.CommandText = "SELECT pg_try_advisory_lock(@key)";
                        checkCmd.Parameters.AddWithValue("key", MigrationAdvisoryLockKey);
                        var checkResult = await checkCmd.ExecuteScalarAsync();
                        var acquired = checkResult is bool b && b;

                        if (acquired)
                        {
                            // Lock is free — the leader has finished; release immediately.
                            await using var relCmd = lockConnection.CreateCommand();
                            relCmd.CommandText = "SELECT pg_advisory_unlock(@key)";
                            relCmd.Parameters.AddWithValue("key", MigrationAdvisoryLockKey);
                            await relCmd.ExecuteNonQueryAsync();
                            migrationComplete = true;
                            break;
                        }
                    }

                    if (migrationComplete)
                        logger.LogInformation("Database migrations completed by peer instance");
                    else
                        logger.LogWarning("Timed out waiting for peer instance to complete database migrations");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations to the database");
                throw;
            }

            return app;
        }
        public static async Task<IApplicationBuilder> SeedDatabaseAsync(
            this IApplicationBuilder app,
            bool seedSampleData = false)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();
                var seeder = new DatabaseSeeder(context, logger, configuration);

                logger.LogInformation("Starting database seeding...");

                await seeder.SeedAsync();

                //if (seedSampleData)
                //{
                //    logger.LogInformation("Seeding sample data for development...");
                //    await seeder.SeedSampleDataAsync();
                //}

                logger.LogInformation("Database seeding completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }

            return app;
        }
        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Beddin API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

            
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (!app.Environment.IsDevelopment())
                app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions 
            {
                Authorization = app.Environment.IsDevelopment()
                ? new[] { new LocalRequestsOnlyAuthorizationFilter() } 
                : new IDashboardAuthorizationFilter[] { new HangfireAuthorizationFilter() }
            });
            app.UseMiddleware<SessionValidationMiddleware>();
            app.UseMiddleware<PasswordPolicyMiddleware>();

            app.MapControllers();

            app.UseRecurringJobs();

            return app;
        }

    }
}
