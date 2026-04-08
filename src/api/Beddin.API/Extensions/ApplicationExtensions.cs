// <copyright file="ApplicationExtensions.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.API.Middleware;
using Beddin.Application.Common.Helpers;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Seed;
using Hangfire;
using Hangfire.Dashboard;

namespace Beddin.API.Extensions
{
    /// <summary>
    /// Provides extension methods for application configuration and middleware setup.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Applies pending database migrations using the application's service provider.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance to use for applying migrations.</param>
        /// <returns>
        /// A <see cref="Task{IApplicationBuilder}"/> representing the asynchronous operation, with the original <see cref="IApplicationBuilder"/> instance.
        /// </returns>
        public static async Task<IApplicationBuilder> ApplyMigrationsAsync(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<MigrationHandler>>();

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();
                var orchestrator = new MigrationHandler(context, logger, configuration);

                var migrationsEnabled = configuration.GetValue<bool>("DatabaseMigrations:Enabled", true);
                if (!migrationsEnabled)
                {
                    logger.LogInformation("Database migrations are disabled");
                    return app;
                }

                logger.LogInformation("Starting database migrations...");

                await orchestrator.ApplyMigrations();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations to the database");
                throw;
            }

            return app;
        }

        /// <summary>
        /// Seeds the database with required data and optionally with sample data for development.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance to use for seeding the database.</param>
        /// <param name="seedSampleData">A value indicating whether to seed sample data for development purposes.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
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

                if (seedSampleData)
                {
                    logger.LogInformation("Seeding sample data for development...");

                    // await seeder.SeedSampleDataAsync();
                }

                logger.LogInformation("Database seeding completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }

            return app;
        }

        /// <summary>
        /// Configures and applies API middleware components to the specified <see cref="WebApplication"/> instance.
        /// </summary>
        /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
        /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
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
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = app.Environment.IsDevelopment()
                ? new[] { new LocalRequestsOnlyAuthorizationFilter() }
                : new IDashboardAuthorizationFilter[] { new HangfireAuthorizationFilter() },
            });
            app.UseMiddleware<SessionValidationMiddleware>();
            app.UseMiddleware<PasswordPolicyMiddleware>();

            app.MapControllers();

            app.UseRecurringJobs();

            return app;
        }
    }
}
