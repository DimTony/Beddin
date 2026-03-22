using Beddin.Application.Common.Interfaces;
using Beddin.Application.Common.Options;
using Beddin.Domain.Aggregates.Users;
using Beddin.Infrastructure.Persistence;
using Beddin.Infrastructure.Persistence.Repositories;
using Beddin.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Beddin.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddDatabase(configuration)
                .AddRepositories()
                .AddServices()
                .AddObservability(configuration);

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.UseNetTopologySuite();
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null
                        );
                    }
                )
            );

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // AppDbContext now implements IUnitOfWork
            services.AddScoped<IUnitOfWork>(provider =>
                provider.GetRequiredService<AppDbContext>());

            services.AddScoped<IDomainEventCollector, EfDomainEventCollector>();

            services.Configure<EmailOptions>(
                configuration.GetSection(EmailOptions.SectionName));

            //services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

            //services.Configure<ApprovalPolicyOptions>(
            //    configuration.GetSection(ApprovalPolicyOptions.SectionName));

            //services.Configure<DatabaseSeedOptions>(
                //configuration.GetSection(DatabaseSeedOptions.SectionName));

            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            // Add concrete repositories (e.g. IPropertyRepository -> PropertyRepository) as you implement them:
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            return services;
        }

        private static IServiceCollection AddServices(
            this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuditLogService, AuditLogService>();


            // Auth Services
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
        public static IServiceCollection AddObservability(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var otlpEndpoint = configuration["OpenTelemetry:Endpoint"]
                ?? "http://localhost:4317";
            var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "Beddin";

            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                        opt.Filter = ctx =>
                            !ctx.Request.Path.StartsWithSegments("/health");
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(serviceName)          // custom ActivitySource
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint))
                    .AddConsoleExporter()            // remove in prod
                )
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint))
                );

            return services;
        }
    }
}
