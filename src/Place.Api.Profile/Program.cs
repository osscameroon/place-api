using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Place.Api.Profile;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Place.Core;
using Place.Core.Database;
using Place.Core.Identity;
using Place.Core.Logging;
using Place.Core.Swagger.Docs;
using Place.Core.Swagger.WebApi;
using Place.Core.Versioning;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseLogging((context, loggerConfiguration) => { });

builder
    .Services.AddPlace(builder.Configuration)
    .AddDatabase(optionsBuilder => optionsBuilder.AddDbContext<ProfileDbContext>())
    .AddCorrelationContextLogging()
    .AddWebApiSwaggerDocs()
    .AddSwaggerDocs()
    .AddApiVersioning();
builder.Services.AddControllers();

builder.Services.RegisterMediatr();
builder.AddServiceDefaults();

WebApplication app = builder.Build();

// Initialize the database
//await app.InitializeDatabaseAsync();

// Configure middleware
app.UsePlace().UserCorrelationContextLogging();
app.UseSwaggerDocs();
app.UseHttpsRedirection();

// Enable routing for controllers
app.UseIdentityConfiguration();
app.MapControllers();

await app.RunAsync();
