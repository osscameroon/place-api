using Identity.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PlaceAPi.Identity.Authenticate.Composition;
using PlaceAPi.Identity.OpenApi;

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
