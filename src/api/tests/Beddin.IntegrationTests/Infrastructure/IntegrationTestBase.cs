using Beddin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;
using Microsoft.Data.Sqlite;
using System;


namespace Beddin.IntegrationTests.Infrastructure
{
    //public abstract class IntegrationTestBase : IDisposable
    //{
    //    protected readonly AppDbContext DbContext;
    //    private readonly SqliteConnection _connection;

    //    protected IntegrationTestBase()
    //    {
    //        // Create and open SQLite in-memory connection
    //        _connection = new SqliteConnection("Filename=:memory:");
    //        _connection.Open();

    //        var options = new DbContextOptionsBuilder<AppDbContext>()
    //            .UseNpgsql("Host=localhost;Port=5432;Database=beddin_test;Username=postgres;Password=Admin123") // Use PostgreSQL connection string
    //            //.UseSqlite(_connection)
    //            .EnableSensitiveDataLogging() // optional but useful for debugging
    //            .Options;

    //        DbContext = new AppDbContext(options);

    //        // Ensure database schema is created
    //        DbContext.Database.EnsureCreated();
    //    }

    //    public void Dispose()
    //    {
    //        DbContext.Dispose();
    //        _connection.Close();
    //        _connection.Dispose();
    //    }
    //}
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgis/postgis:16-3.4")
            //.WithImage("postgres:16")
            .WithDatabase("beddin_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        protected AppDbContext DbContext { get; private set; } = null!;
        private Respawner _respawner = null!;
        private NpgsqlConnection _dbConnection = null!;

        public virtual async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    _postgres.GetConnectionString(),
                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
                )
            );

            var serviceProvider = services.BuildServiceProvider();
            DbContext = serviceProvider.GetRequiredService<AppDbContext>();

            await DbContext.Database.MigrateAsync();

            // Create and open a connection for Respawner
            _dbConnection = new NpgsqlConnection(_postgres.GetConnectionString());
            await _dbConnection.OpenAsync();

            _respawner = await Respawner.CreateAsync(
                _dbConnection,
                new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    SchemasToInclude = new[] { "public" }
                }
            );
        }

        protected async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        public async Task DisposeAsync()
        {
            await DbContext.DisposeAsync();
            if (_dbConnection != null)
            {
                await _dbConnection.CloseAsync();
                await _dbConnection.DisposeAsync();
            }
            await _postgres.DisposeAsync();
        }
    }
}