using Beddin.API.Middleware;
using Beddin.Domain.Aggregates.Users;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;

namespace Beddin.API.Extensions
{
    public static class ApplicationExtensions
    {
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
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var seeder = new DatabaseSeeder(userManager, context, logger, configuration);

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

            app.UseMiddleware<SessionValidationMiddleware>();
            app.UseMiddleware<PasswordPolicyMiddleware>();

            app.MapControllers();

            return app;
        }

    }
}
