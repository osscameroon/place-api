using Core.Framework;
using Identity;
using Identity.Authenticate.Composition;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddCoreFramework();


{
    builder.Services.RegisterPlace(builder);
}

WebApplication app = builder.Build();


{
    app.UseHttpSecurity();
    app.WithAuthenticationEndpoints();
    app.UsePlaceServices();
    await app.RunAsync();
}

app?.MapGet("/", () => "Hello World!");

public partial class Program { }
