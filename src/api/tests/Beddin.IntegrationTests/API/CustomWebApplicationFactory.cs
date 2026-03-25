//using Beddin.Application.Common.Behaviours;
//using Beddin.Application.Common.Interfaces;
//using Beddin.Domain.Common;
//using Beddin.Infrastructure.Persistence;
//using MediatR;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.IdentityModel.Tokens;
//using Npgsql;
//using Respawn;
//using System.IdentityModel.Tokens.Jwt;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text;
//using Testcontainers.PostgreSql;

//namespace Beddin.IntegrationTests.API
//{
//    public abstract class ApiIntegrationTestBase : IClassFixture<BeddinApiFactory>, IAsyncLifetime
//    {
//        protected readonly HttpClient Client;
//        protected readonly BeddinApiFactory Factory;

//        protected ApiIntegrationTestBase(BeddinApiFactory factory)
//        {
//            Factory = factory;
//            Client = factory.CreateClient();
//        }

//        public Task InitializeAsync() => Task.CompletedTask;

//        public async Task DisposeAsync() => await Factory.ResetDatabaseAsync();

//        // Helper — generate a token signed with the test secret
//        protected static string GenerateToken(
//            string email = "tony@beddin.ng",
//            string role = "Seeker",
//            bool expired = false)
//        {
//            var key = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes("test-secret-key-must-be-at-least-32-chars!!"));

//            var claims = new[]
//            {
//            new Claim(ClaimTypes.Email, email),
//            new Claim("role", role)
//        };

//            var expiry = expired
//                ? DateTime.UtcNow.AddMinutes(-1)
//                : DateTime.UtcNow.AddHours(1);

//            var token = new JwtSecurityToken(
//                claims: claims,
//                expires: expiry,
//                signingCredentials: new SigningCredentials(
//                    key, SecurityAlgorithms.HmacSha256));

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        protected void AuthorizeClient(string role = "Seeker")
//        {
//            Client.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", GenerateToken(role: role));
//        }
//    }
//    //public class BeddinApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
//    //{
//    //    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgis/postgis:16-3.4")
//    //        .WithDatabase("beddin_test")
//    //        .WithUsername("test_user")
//    //        .WithPassword("test_password")
//    //        .Build();

//    //    private Respawner _respawner = null!;
//    //    private NpgsqlConnection _dbConnection = null!;

//    //    protected override void ConfigureWebHost(IWebHostBuilder builder)
//    //    {
//    //        builder.ConfigureServices(services =>
//    //        {
//    //            // Swap the real DB with the test container
//    //            services.RemoveAll<DbContextOptions<AppDbContext>>();
//    //            services.RemoveAll<AppDbContext>();

//    //            services.AddDbContext<AppDbContext>(options =>
//    //                options.UseNpgsql(
//    //                    _postgres.GetConnectionString(),
//    //                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
//    //                )
//    //            );

//    //            // Swap JWT so tests control the signing key
//    //            services.PostConfigure<JwtBearerOptions>(
//    //                JwtBearerDefaults.AuthenticationScheme, options =>
//    //                {
//    //                    options.TokenValidationParameters = new TokenValidationParameters
//    //                    {
//    //                        ValidateIssuerSigningKey = true,
//    //                        IssuerSigningKey = new SymmetricSecurityKey(
//    //                            Encoding.UTF8.GetBytes("test-secret-key-must-be-at-least-32-chars!!")),
//    //                        ValidateIssuer = false,
//    //                        ValidateAudience = false,
//    //                        ClockSkew = TimeSpan.Zero  // expiry tests must be honest
//    //                    };
//    //                });
//    //        });
//    //    }

//    //    public async Task InitializeAsync()
//    //    {
//    //        await _postgres.StartAsync();

//    //        // Run migrations once on startup
//    //        using var scope = Services.CreateScope();
//    //        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    //        await db.Database.MigrateAsync();

//    //        _dbConnection = new NpgsqlConnection(_postgres.GetConnectionString());
//    //        await _dbConnection.OpenAsync();

//    //        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
//    //        {
//    //            DbAdapter = DbAdapter.Postgres,
//    //            SchemasToInclude = new[] { "public" }
//    //        });
//    //    }

//    //    public async Task ResetDatabaseAsync() =>
//    //        await _respawner.ResetAsync(_dbConnection);

//    //    public new async Task DisposeAsync()
//    //    {
//    //        await _dbConnection.CloseAsync();
//    //        await _dbConnection.DisposeAsync();
//    //        await _postgres.DisposeAsync();
//    //    }
//    //}

//    public class BeddinApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
//    {
//        private Respawner _respawner = null!;
//        private NpgsqlConnection _dbConnection = null!;

//        private static string ConnectionString =>
//            Environment.GetEnvironmentVariable("TEST_DB_CONNECTION_STRING")
//            ?? "Host=localhost;Port=5432;Database=beddin_test;Username=postgres;Password=Admin123";

//        protected override void ConfigureWebHost(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices(services =>
//            {
//                ReplaceDatabase(services);
//                ReplacePipelineBehaviours(services);
//                ReplaceExternalServices(services);

//                // Reset schema once per factory lifetime (one factory per test class via IClassFixture)
//                var sp = services.BuildServiceProvider();
//                using var scope = sp.CreateScope();
//                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//                //db.Database.EnsureDeleted();
//                //db.Database.EnsureCreated();
//            });
//        }

//        private static void ReplaceDatabase(IServiceCollection services)
//        {
//            services.RemoveAll<DbContextOptions<AppDbContext>>();

//            services.AddDbContext<AppDbContext>(options =>
//                options.UseNpgsql(ConnectionString));
//        }

//        private static void ReplacePipelineBehaviours(IServiceCollection services)
//        {
//            services.RemoveAll(typeof(IPipelineBehavior<,>));

//            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NoOpDomainEventBehavior<,>));
//        }

//        private static void ReplaceExternalServices(IServiceCollection services)
//        {
//            services.RemoveAll<IAuditLogService>();
//            services.AddScoped<IAuditLogService, NullAuditLogService>();
//        }

//        public async Task InitializeAsync()
//        {
//            _dbConnection = new NpgsqlConnection(ConnectionString);
//            await _dbConnection.OpenAsync();

//            using var scope = Services.CreateScope();
//            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//            await db.Database.MigrateAsync();

//            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
//            {
//                DbAdapter = DbAdapter.Postgres,
//                SchemasToInclude = ["public"]
//            });
//        }

//        // Satisfies IAsyncLifetime explicitly — keeps the interface happy
//        async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();

//        // Overrides WebApplicationFactory<T>
//        public override async ValueTask DisposeAsync()
//        {
//            if (_dbConnection is not null)
//                await _dbConnection.DisposeAsync();
//            await base.DisposeAsync();
//        }

//        public async Task ResetDatabaseAsync()
//        {
//            if (_respawner is null || _dbConnection is null) return;
//            await _respawner.ResetAsync(_dbConnection);
//        }
//    }

//    public class PostgresWebApplicationFactory : WebApplicationFactory<Program>
//    {
//        private const string ConnectionString =
//            "Host=localhost;Port=5432;Database=beddin_test;Username=postgres;Password=Admin123";

//        protected override void ConfigureWebHost(IWebHostBuilder builder)
//        {
//            builder.ConfigureServices(services =>
//            {
//                ReplaceDatabase(services);
//                ReplacePipelineBehaviours(services);
//                ReplaceExternalServices(services);

//                // Reset schema once per factory lifetime (one factory per test class via IClassFixture)
//                var sp = services.BuildServiceProvider();
//                using var scope = sp.CreateScope();
//                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//                db.Database.EnsureDeleted();
//                db.Database.EnsureCreated();
//            });
//        }

//        // ------------------------------------------------------------------
//        // 1. Swap the real Npgsql+NetTopologySuite DbContext for a plain one.
//        //    NetTopologySuite isn't needed for auth tests and the retry-on-failure
//        //    option masks real errors during test runs.
//        // ------------------------------------------------------------------
//        private static void ReplaceDatabase(IServiceCollection services)
//        {
//            services.RemoveAll<DbContextOptions<AppDbContext>>();

//            services.AddDbContext<AppDbContext>(options =>
//                options.UseNpgsql(ConnectionString));
//        }

//        // ------------------------------------------------------------------
//        // 2. Replace pipeline behaviours that cause 500s in the test host.
//        //
//        //    DomainEventBehavior  — publishes UserCreatedEvent after every command.
//        //                           Any INotificationHandler<UserCreatedEvent> that
//        //                           has an unavailable dependency (push service,
//        //                           read-model projector, etc.) causes a 500.
//        //                           Replaced with a no-op passthrough.
//        //
//        //    AuditLogBehavior     — depends on IAuditLogService; even with the
//        //                           feature flag off the service is still resolved.
//        //                           Replaced with a passthrough.
//        //
//        //    FeatureFlagBehavior  — safe but depends on config keys being present
//        //                           in the test environment. Removed to keep tests
//        //                           independent of feature flag state.
//        //
//        //    ValidationBehavior   — kept. We need FluentValidation to run so the
//        //                           invalid-data [Theory] tests get their 400s.
//        // ------------------------------------------------------------------
//        private static void ReplacePipelineBehaviours(IServiceCollection services)
//        {
//            services.RemoveAll(typeof(IPipelineBehavior<,>));

//            // Only re-add validation — the one behaviour tests actually depend on.
//            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

//            // No-op domain event behaviour keeps the handler running without
//            // triggering cascading infrastructure calls from event handlers.
//            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NoOpDomainEventBehavior<,>));
//        }

//        // ------------------------------------------------------------------
//        // 3. Replace services that write to external systems or require
//        //    config that won't be present in a test environment.
//        // ------------------------------------------------------------------
//        private static void ReplaceExternalServices(IServiceCollection services)
//        {
//            services.RemoveAll<IAuditLogService>();
//            services.AddScoped<IAuditLogService, NullAuditLogService>();
//        }
//    }

//    // ---------------------------------------------------------------------------
//    // Test doubles — file-scoped so they're invisible outside this file
//    // ---------------------------------------------------------------------------

//    /// <summary>
//    /// Calls next() and returns. Domain events raised during the command
//    /// are not dispatched, so no event handler infrastructure is exercised.
//    /// </summary>
//    file sealed class NoOpDomainEventBehavior<TRequest, TResponse>
//        : IPipelineBehavior<TRequest, TResponse>
//        where TRequest : IRequest<TResponse>
//    {
//        public Task<TResponse> Handle(
//            TRequest request,
//            RequestHandlerDelegate<TResponse> next,
//            CancellationToken ct) => next();
//    }

//    /// <summary>
//    /// Satisfies IAuditLogService without writing anything to the database.
//    /// Returns a stable fake ID so UpdateOutcomeAsync calls don't throw.
//    /// </summary>
//    file sealed class NullAuditLogService : IAuditLogService
//    {
//        private static readonly AuditLogId _fakeId = AuditLogId.New();

//        public Task<AuditLogId> RecordAsync(
//            Guid? userId, string action, string resource,
//            Guid? resourceId, object? oldValue, object? newValue,
//            string? ipAddress, CancellationToken ct = default)
//            => Task.FromResult(_fakeId);

//        public Task UpdateOutcomeAsync(
//            AuditLogId auditEntryId, bool succeeded,
//            string? failureReason = null, CancellationToken ct = default)
//            => Task.CompletedTask;
//    }


//    //public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
//    //{
//    //    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16")
//    //        //.WithImage("postgres:16")
//    //        .WithDatabase("beddin_test")
//    //        .WithUsername("test_user")
//    //        .WithPassword("test_password")
//    //        .Build();

//    //    protected override void ConfigureWebHost(IWebHostBuilder builder)
//    //    {
//    //        builder.ConfigureServices(services =>
//    //        {
//    //            // Remove existing DbContext registration
//    //            services.RemoveAll<DbContextOptions<AppDbContext>>();
//    //            services.RemoveAll<AppDbContext>();

//    //            // Add test database
//    //            services.AddDbContext<AppDbContext>(options =>
//    //                options.UseNpgsql(
//    //                    _postgres.GetConnectionString(),
//    //                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite()
//    //                )
//    //            );

//    //            // Build the service provider and create the database
//    //            var sp = services.BuildServiceProvider();
//    //            using var scope = sp.CreateScope();
//    //            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    //            db.Database.Migrate();
//    //        });
//    //    }

//    //    public async Task InitializeAsync()
//    //    {
//    //        await _postgres.StartAsync();
//    //    }

//    //    async Task IAsyncLifetime.DisposeAsync()
//    //    {
//    //        await _postgres.DisposeAsync();
//    //    }
//    //}

//}