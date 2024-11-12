using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Place.Core;
using Place.Core.Identity;
using Place.Core.Logging;
using PlaceAPi.Identity;
using PlaceAPi.Identity.Authenticate.Composition;
using PlaceAPi.Identity.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging((context, loggerConfiguration) => { });


{
    builder
        .Services.AddPlace(builder.Configuration)
        .AddIdentity()
        .AddPasswordRules()
        .AddEmailSender()
        .AddEndpoints()
        .AddCorrelationContextLogging();
    builder.Services.AddOpenApiFeature(builder.Configuration);

    builder.AddServiceDefaults();
}
WebApplication app = builder.Build();

app.WithAuthenticationEndpoints();

app.UsePlace();
app.UserCorrelationContextLogging();


{
    if (app.Environment.IsDevelopment())
    {
        app.WithSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    app.UseIdentityConfiguration();

    app.UseHttpSecurity();

    await app.RunAsync();
}

public partial class Program { }
