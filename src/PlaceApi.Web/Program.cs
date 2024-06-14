using System.Collections.Generic;
using System.Reflection;
using FastEndpoints;
using FastEndpoints.Swagger;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Authentication;
using PlaceApi.EmailSending;
using PlaceApi.SharedKernel;
using Serilog;

ILogger logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

services.AddAuthentication();
services.AddAuthorizationBuilder();

services.AddFastEndpoints();
services.SwaggerDocument(o =>
{
    o.MaxEndpointVersion = 1;
    o.DocumentSettings = s =>
    {
        s.DocumentName = "Release 1.0";
        s.Title = "Swagger Place API";
        s.Version = "v1.0";
    };
});

// Add Module Services


List<Assembly> mediatRAssemblies = [typeof(PlaceApi.Web.Program).Assembly];

builder.Services.AddAuthModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddEmailSendingModule(builder.Configuration, logger, mediatRAssemblies);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray()));
builder.Services.AddMediatRLoggingBehavior();
builder.Services.AddMediatRFluentValidationBehavior();

// Add MediatR Domain Event Dispatcher
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

WebApplication app = builder.Build();

app.MapGet("/", () => "It works");

//app.MapIdentityApi<>()>()

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
});

app.UseHangfireDashboard();
app.MapHangfireDashboard();
app.UseSwaggerGen();

app.Run();

namespace PlaceApi.Web
{
    public partial class Program { }
} // needed for tests
