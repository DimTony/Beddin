// <copyright file="DatabaseSeeder.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Beddin.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Provides methods for seeding the database with initial data such as default users, roles, and other necessary entities.
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly AppDbContext context;
        private readonly ILogger<DatabaseSeeder> logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSeeder"/> class.
        /// </summary>
        /// <param name="context">The database context used for seeding data.</param>
        /// <param name="logger">The logger used for logging seeding progress and errors.</param>
        /// <param name="configuration">The configuration used for retrieving seeding settings.</param>
        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger, IConfiguration configuration)
        {
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Seeds the database with initial data, such as default users, roles, and other necessary entities. This method checks if seeding is enabled in the configuration before proceeding. It ensures that the database is created and migrations are applied before seeding data. The method logs the progress and any errors that occur during the seeding process for monitoring and troubleshooting purposes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SeedAsync()
        {
            try
            {
                var seedingEnabled = this.configuration.GetValue<bool>("DatabaseSeeding:Enabled", true);
                if (!seedingEnabled)
                {
                    this.logger.LogInformation("Database seeding is disabled");
                    return;
                }

                // Ensure database is created and migrations are applied
                await this.context.Database.MigrateAsync();

                // Seed admin user
                await this.SeedAdminUserAsync();

                this.logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = this.configuration.GetValue<string>("MockUsers:Admin:Email")
                ?? string.Empty;
            var ownerEmail = this.configuration.GetValue<string>("MockUsers:Owner:Email")
                ?? string.Empty;
            var buyerEmail = this.configuration.GetValue<string>("MockUsers:Buyer:Email")
                ?? string.Empty;

            // Check if admin user already exists
            var adminExists = await this.context.Users
                .AnyAsync(u => u.Email == adminEmail);

            if (adminExists)
            {
                this.logger.LogInformation("Admin user already exists, skipping seed");
                return;
            }

            this.logger.LogInformation("Creating default admin user...");
            var firstName = this.configuration.GetValue<string>("DatabaseSeeding:AdminUser:FirstName")
                ?? "System";
            var lastName = this.configuration.GetValue<string>("DatabaseSeeding:AdminUser:LastName")
                ?? "Administrator";
            var role = this.configuration.GetValue<string>("DatabaseSeeding:AdminUser:Role")
                ?? "Admin";
            var password = this.configuration.GetValue<string>("DatabaseSeeding:AdminUser:Password")
                ?? "Admin123!";

            return;
        }
    }
}
