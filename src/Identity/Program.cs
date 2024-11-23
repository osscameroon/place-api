using Core.Framework;
using Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddCoreFramework();
IWebHostEnvironment environment = builder.Environment;

builder.Services.AddIdentityModule(builder);

WebApplication app = builder.Build();


{
    app.UseIdentityModule(environment);
    app.WithAuthenticationEndpoints();
    await app.RunAsync();
}

app?.MapGet("/", () => "Hello World!");

public partial class Program { }
