using Asp.Versioning;
using Asp.Versioning.Builder;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using PlaceAPi.Identity.Authenticate.Endpoints;

namespace PlaceAPi.Identity.Authenticate.Composition;

public static class AuthenticateExtensions
{
    internal static WebApplication WithAuthenticationEndpoints(this WebApplication app)
    {
        string? apiTitle = app.Configuration.GetSection("ApiVersioning:Title").Get<string>();

        ApiVersionSet apiVersionSet = app.NewApiVersionSet($"{apiTitle}")
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder groupBuilder = app.MapGroup("api/v{apiVersion:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        groupBuilder.MapAuthenticationEndpoints<ApplicationUser>();

        return app;
    }
}
