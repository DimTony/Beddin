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
                //.AddDatabase(configuration)
                //.AddRepositories()
                //.AddServices()
                .AddObservability(configuration);

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
