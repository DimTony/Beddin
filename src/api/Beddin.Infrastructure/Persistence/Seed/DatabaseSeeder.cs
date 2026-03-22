using Azure.Core;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Beddin.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IConfiguration _configuration;
        public DatabaseSeeder(UserManager<ApplicationUser> userManager, AppDbContext context, ILogger<DatabaseSeeder> logger, IConfiguration configuration)
        {
            _userManager = userManager;
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
            var password = _configuration.GetValue<string>("DatabaseSeeding:AdminUser:Password")
                ?? "Admin123!";

            var adminUser = User.Create(
                firstName: firstName,
                lastName: lastName,
                role: UserRole.Admin,
                email: adminEmail
            );

            var identityUser = new ApplicationUser
            {
                Id = adminUser.Id.Value.ToString(),
                UserName = adminUser.Email,
                Email = adminUser.Email,
                EmailConfirmed = false,
                FailedLoginAttempts = 0
            };

            var identityResult = await _userManager.CreateAsync(identityUser, password);
            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create admin user: {Errors}", errors);
                return;
            }

            // Add role claim
            await _userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim("role", UserRole.Admin.ToString()));

            // Save domain user
            _context.AppUsers.Add(adminUser);
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
