using Core;
using Core.Identity;
using Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API;

public static class ServiceCollectionExtensions
{
    public static void RegisterPlace(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        builder.Host.UseLogging((context, loggerConfiguration) => { });
        builder
            .Services.AddPlace(builder.Configuration)
            .AddIdentity()
            .AddPasswordRules()
            .AddEmailSender()
            .AddEndpoints()
            .AddCorrelationContextLogging();
    }

    public static void UseHttpSecurity(this IApplicationBuilder app)
    {
        app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(opts => opts.NoReferrer());
        app.UseXXssProtection(options => options.Disabled());
        app.UseXfo(options => options.Deny());

        app.UseCsp(opts =>
            opts.BlockAllMixedContent()
                .StyleSources(s => s.Self())
                .StyleSources(s => s.UnsafeInline())
                .FontSources(s => s.Self())
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self())
                .ScriptSources(s => s.Self())
        );
    }

    public static IApplicationBuilder UsePlaceServices(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection()
            .UsePlace()
            .UserCorrelationContextLogging()
            .UseIdentityConfiguration();

        return app;
    }
}
