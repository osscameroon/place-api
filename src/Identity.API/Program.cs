using Identity.API;
using Identity.API.Authenticate.Composition;
using Identity.API.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


{
    builder.Services.RegisterPlace(builder);
    builder.Services.AddOpenApiFeature(builder.Configuration);
    builder.AddServiceDefaults();
}

WebApplication app = builder.Build();


{
    app.UseHttpSecurity();
    app.WithAuthenticationEndpoints();
    app.UsePlaceServices();
    if (app.Environment.IsDevelopment())
    {
        app.WithSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    await app.RunAsync();
}

public partial class Program { }
