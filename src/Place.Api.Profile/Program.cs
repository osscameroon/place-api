using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Place.Api.Common;
using Place.Api.Common.Logging;
using Place.Api.Common.Swagger.Docs;
using Place.Api.Common.Swagger.WebApi;
using Place.Api.Common.Versioning;
using Place.Api.Profile;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Place.Core.Identity;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseLogging((context, loggerConfiguration) => { });

builder
    .Services.AddPlace(builder.Configuration)
    .AddCorrelationContextLogging()
    .AddWebApiSwaggerDocs()
    .AddSwaggerDocs()
    .AddApiVersioning();
builder.Services.AddControllers();
builder.Services.RegisterMediatr();

WebApplication app = builder.Build();

// Initialize the database
await app.InitializeDatabaseAsync();

// Configure middleware
app.UsePlace().UserCorrelationContextLogging();
app.UseSwaggerDocs();
app.UseHttpsRedirection();

// Enable routing for controllers
app.UseIdentityConfiguration();
app.MapControllers();

await app.RunAsync();
