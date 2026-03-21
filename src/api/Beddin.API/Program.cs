using Beddin.API.Extensions;
using Beddin.Infrastructure;
using Beddin.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateSlimBuilder(args);

//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
//});


//builder.Services.AddHealthChecks().AddCheck<HealthCheck>("Infrastructure");

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.SeedDatabaseAsync(seedSampleData: true);
}

app.UseApiMiddleware();
app.Run();

// Make Program accessible to integration tests
public partial class Program { }

