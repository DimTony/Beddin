using Azure.Core;
using Beddin.Application.Common.Interfaces;
using Beddin.Domain.Aggregates.Users;
using Beddin.Domain.Common;
using Beddin.Infrastructure.Persistence.Repositories;
using MediatR;
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

    
    public class MigrationHandler
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MigrationHandler> _logger;
        private readonly IConfiguration _configuration;

        public MigrationHandler(AppDbContext context, ILogger<MigrationHandler> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task ApplyMigrations()
        {
            try
            {
                var migrationsEnabled = _configuration.GetValue<bool>("DatabaseMigrations:Enabled", true);
                if (!migrationsEnabled)
                {
                    _logger.LogInformation("Database migrations are disabled");
                    return;
                }

                // Ensure database is created and migrations are applied
                await _context.Database.MigrateAsync();

                _logger.LogInformation("Database migrations applied successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying migrations to the database");
                throw;
            }
        }
    }
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IConfiguration _configuration;
        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger, IConfiguration configuration)
        {
            //_userManager = userManager;
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
        //private async Task SeedUsersAsync()
        //{
        //    var usersToSeed = new[]
        //    {
        //        new
        //        {
        //            Email = _configuration.GetValue<string>("MockUsers:Admin:Email") ?? "",
        //            FirstName = "System",
        //            LastName = "Administrator",
        //            Role = "Admin",
        //            Password = _configuration.GetValue<string>("MockUsers:Admin:Password") ?? ""
        //        },
        //        new
        //        {
        //            Email = _configuration.GetValue<string>("MockUsers:Owner:Email") ?? "",
        //            FirstName = "Store",
        //            LastName = "Owner",
        //            Role = "Owner",
        //            Password = _configuration.GetValue<string>("MockUsers:Owner:Password") ?? ""
        //        },
        //        new
        //        {
        //            Email = _configuration.GetValue<string>("MockUsers:Buyer:Email") ?? "",
        //            FirstName = "Test",
        //            LastName = "Buyer",
        //            Role = "Buyer",
        //            Password = _configuration.GetValue<string>("MockUsers:Buyer:Password") ?? ""
        //        }
        //    };

        //    foreach (var userData in usersToSeed)
        //    {
        //        if (string.IsNullOrWhiteSpace(userData.Email))
        //            continue;

        //        var exists = await _context.Users
        //            .AnyAsync(u => u.Email == userData.Email);

        //        if (exists)
        //        {
        //            _logger.LogInformation("User with email {Email} already exists, skipping", userData.Email);
        //            continue;
        //        }

        //        _logger.LogInformation("Seeding user {Email} with role {Role}", userData.Email, userData.Role);

        //        var user = User.Create(
        //            userData.FirstName,
        //            userData.LastName,
        //            userData.Role,
        //            userData.Password,
        //            userData.Email
        //        );

        //        _context.Users.Add(user);
        //    }

        //    await _context.SaveChangesAsync();
        //}

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = _configuration.GetValue<string>("MockUsers:Admin:Email")
                ?? "";
            var ownerEmail = _configuration.GetValue<string>("MockUsers:Owner:Email")
                ?? "";
            var buyerEmail = _configuration.GetValue<string>("MockUsers:Buyer:Email")
                ?? "";



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

            return;

            //var identityUser = new ApplicationUser
            //{
            //    UserName = adminEmail,
            //    Email = adminEmail,
            //    EmailConfirmed = false,
            //    FailedLoginAttempts = 0
            //};

            //var adminUser = User.Create(
            //    identityUser.Id,
            //    firstName: firstName,
            //    lastName: lastName,
            //    role: UserRole.Admin,
            //    email: adminEmail
            //);

            //var identityResult = await _userManager.CreateAsync(identityUser, password);
            //if (!identityResult.Succeeded)
            //{
            //    var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            //    _logger.LogError("Failed to create admin user: {Errors}", errors);
            //    return;
            //}

            //try
            //{
            //    await _userManager.AddClaimAsync(identityUser,
            //        new System.Security.Claims.Claim("role", UserRole.Admin.ToString()));

            //    _context.AppUsers.Add(adminUser);
            //    await _context.SaveChangesAsync();

            //    //await _userRepository.AddAsync(user, cancellationToken);
            //    //await _unitOfWork.SaveChangesAsync(cancellationToken);
            //}
            //catch (Exception)
            //{
            //    // Compensate — delete the identity user so we don't leave orphans
            //    await _userManager.DeleteAsync(identityUser);
            //    throw;  // let the exception bubble — caller gets a 500, not a silent split
            //}

            //_logger.LogInformation(
            //    "Admin user created successfully with ID: {UserId} and Email: {Email}",
            //    adminUser.Id.Value,
            //    adminUser.Email);
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
