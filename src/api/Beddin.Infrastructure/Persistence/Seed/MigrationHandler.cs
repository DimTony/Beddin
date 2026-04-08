// <copyright file="MigrationHandler.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Beddin.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Handles the application of database migrations to ensure that the database schema is up-to-date with the latest changes defined in the application's migration files. This class is responsible for applying any pending migrations to the database and logging the progress and any errors that occur during the migration process. It ensures that the database is properly initialized and ready for use when the application starts.
    /// </summary>
    public class MigrationHandler
    {
        private readonly AppDbContext context;
        private readonly ILogger<MigrationHandler> logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationHandler"/> class.
        /// </summary>
        /// <param name="context">The database context used for applying migrations.</param>
        /// <param name="logger">The logger used for logging migration progress and errors.</param>
        /// <param name="configuration">The configuration used for accessing application settings.</param>
        public MigrationHandler(AppDbContext context, ILogger<MigrationHandler> logger, IConfiguration configuration)
        {
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Applies any pending migrations to the database. This method ensures that the database schema is up-to-date with the latest changes defined in the application's migration files. It logs the success or failure of the migration process for monitoring and troubleshooting purposes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ApplyMigrations()
        {
            try
            {
                // Ensure database is created and migrations are applied
                await this.context.Database.MigrateAsync();

                this.logger.LogInformation("Database migrations applied successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while applying migrations to the database");
                throw;
            }
        }
    }
}
