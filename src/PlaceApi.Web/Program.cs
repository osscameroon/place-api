using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlaceApi.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddWebApplication(builder.Configuration);

WebApplication app = builder.Build();

app.UseWebApplication();

app.MapGet("/", () => "It works");

//app.MapIdentityApi<>()>()

app.Run();
