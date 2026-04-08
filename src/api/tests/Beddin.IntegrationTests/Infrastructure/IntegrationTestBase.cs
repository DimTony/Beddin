// <copyright file="IntegrationTestBase.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using Beddin.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Beddin.IntegrationTests.Infrastructure
{
    /// <summary>
    /// Base class for integration tests, providing a configured <see cref="AppDbContext"/> and database connection.
    /// </summary>
    public abstract class IntegrationTestBase : IDisposable
    {
        /// <summary>
        /// Gets the application database context for integration tests.
        /// </summary>
        private readonly AppDbContext dbContext;
        private readonly SqliteConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestBase"/> class.
        /// Sets up the in-memory database and ensures the schema is created.
        /// </summary>
        protected IntegrationTestBase()
        {
            // Create and open SQLite in-memory connection
            this.connection = new SqliteConnection("Filename=:memory:");
            this.connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("Host=localhost;Port=5432;Database=beddin_test;Username=postgres;Password=Admin123") // Use PostgreSQL connection string
                .EnableSensitiveDataLogging() // optional but useful for debugging
                .Options;

            this.dbContext = new AppDbContext(options);

            // Ensure database schema is created
            this.dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Disposes the database context and closes the database connection.
        /// </summary>
        public void Dispose()
        {
            this.dbContext.Dispose();
            this.connection.Close();
            this.connection.Dispose();
        }
    }
}
