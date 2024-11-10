using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Place.Api.Common;
using Place.Api.Common.Logging;
using Place.Api.Common.Swagger.Docs;
using Place.Api.Common.Swagger.WebApi;
using Place.Api.Common.Versioning;
using Place.Api.Profile;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Host.UseLogging((context, loggerConfiguration) => { });

// Add services
builder
    .Services.AddPlace(builder.Configuration)
    .AddCorrelationContextLogging()
    .AddWebApiSwaggerDocs()
    .AddSwaggerDocs()
    .AddApiVersioning();

// Enable MVC and controllers
builder.Services.AddControllers();

// Register MediatR and database services
builder.Services.AddProfileDatabase(builder.Configuration);
builder.Services.RegisterMediatr();

WebApplication app = builder.Build();

// Initialize the database
await app.InitializeDatabaseAsync();

// Configure middleware
app.UsePlace().UserCorrelationContextLogging();
app.UseSwaggerDocs();
app.UseHttpsRedirection();

// Enable routing for controllers
app.MapControllers();

await app.RunAsync();
