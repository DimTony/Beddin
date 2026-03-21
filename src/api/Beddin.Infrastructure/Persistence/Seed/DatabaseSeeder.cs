using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Beddin.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IConfiguration _configuration;
        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            try
            {
                var seedingEnabled = _configuration.GetValue<bool>("DatabaseSeeding:Enabled", true);
                if (!seedingEnabled)
                {
                    _logger.LogInformation("Database seeding is disabled");
                    return;
                }

                // Ensure database is created and migrations are applied
                await _context.Database.MigrateAsync();

                // Seed admin user
                await SeedAdminUserAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = _configuration.GetValue<string>("DatabaseSeeding:AdminUser:Email")
                ?? "admin@beddin.com";

            // Check if admin user already exists
            var adminExists = await _context.Users
                .AnyAsync(u => u.Email == adminEmail);

            if (adminExists)
            {
                _logger.LogInformation("Admin user already exists, skipping seed");
                return;
            }

            _logger.LogInformation("Creating default admin user...");

            var firstName = _configuration.GetValue<string>("DatabaseSeeding:AdminUser:FirstName")
                ?? "System";
            var lastName = _configuration.GetValue<string>("DatabaseSeeding:AdminUser:LastName")
                ?? "Administrator";
            var role = _configuration.GetValue<string>("DatabaseSeeding:AdminUser:Role")
                ?? "Admin";

            var adminUser = User.Create(
                firstName: firstName,
                lastName: lastName,
                role: role,
                email: adminEmail
            );

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Admin user created successfully with ID: {UserId} and Email: {Email}",
                adminUser.Id.Value,
                adminUser.Email);
        }

        ///// <summary>
        ///// Seed sample data for development/testing purposes
        ///// </summary>
        //public async Task SeedSampleDataAsync()
        //{
        //    await SeedSampleUsersAsync();
        //    // Add more seed methods as needed
        //    // await SeedSamplePropertiesAsync();
        //    // await SeedSampleBookingsAsync();
        //}

        //private async Task SeedSampleUsersAsync()
        //{
        //    var sampleUsers = new[]
        //    {
        //    ("John", "Doe", "Customer", "john.doe@example.com"),
        //    ("Jane", "Smith", "PropertyManager", "jane.smith@example.com"),
        //    ("Bob", "Johnson", "Customer", "bob.johnson@example.com")
        //};

        //    foreach (var (firstName, lastName, role, email) in sampleUsers)
        //    {
        //        var userExists = await _context.Users.AnyAsync(u => u.Email == email);
        //        if (!userExists)
        //        {
        //            var user = User.Create(firstName, lastName, role, email);
        //            _context.Users.Add(user);
        //            _logger.LogInformation("Created sample user: {Email}", email);
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //}
    }
}
