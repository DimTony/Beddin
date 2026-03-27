using Beddin.API.Middleware;
using Beddin.Application.Common.Helpers;
using Beddin.Domain.Aggregates.Users;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Seed;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StackExchange.Redis;

namespace Beddin.API.Extensions
{
    public static class ApplicationExtensions
    {
        
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
