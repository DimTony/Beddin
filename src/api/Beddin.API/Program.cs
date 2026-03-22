using Beddin.API.Extensions;
using Beddin.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync(seedSampleData: true);
}

app.UseApiMiddleware();
app.Run();

public partial class Program { }

