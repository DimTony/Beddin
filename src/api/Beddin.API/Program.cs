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

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var context = services.GetRequiredService<AppDbContext>();
//    var passwordHasher = services.GetRequiredService<IPasswordHasher>();
//    var passwordgenerator = services.GetRequiredService<IPasswordGenerator>();
//    var seedSettings = services.GetRequiredService<IOptions<DatabaseSeedOptions>>();
//    var mediator = services.GetRequiredService<IMediator>();

//    await context.Database.MigrateAsync();

//    await DbSeeder.SeedAsync(context, passwordHasher, passwordgenerator, seedSettings, mediator);
//}

app.UseApiMiddleware();
app.Run();

// Make Program accessible to integration tests
public partial class Program { }

