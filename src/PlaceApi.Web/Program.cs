using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.SharedKernel;
using PlaceApi.Web;
using Serilog;

ILogger logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddWebApplication(builder.Configuration);

// Add Module Services
List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray()));
builder.Services.AddMediatRLoggingBehavior();
builder.Services.AddMediatRFluentValidationBehavior();

// Add MediatR Domain Event Dispatcher
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

WebApplication app = builder.Build();

app.UseWebApplication();

app.MapGet("/", () => "It works");

//app.MapIdentityApi<>()>()

app.Run();

public partial class Program { } // needed for tests
