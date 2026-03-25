using Beddin.API.Extensions;
using Beddin.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiServices(builder.Configuration);


var app = builder.Build();

await app.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync(seedSampleData: true);
}

app.UseApiMiddleware();

// Map health check endpoint for ECS
app.MapHealthChecks("/api/health");

app.Run();

public partial class Program { }

