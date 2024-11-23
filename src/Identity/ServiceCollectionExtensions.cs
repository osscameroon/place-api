using Core.Framework;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity;

public static class ServiceCollectionExtensions
{
    public static void RegisterPlace(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        builder.AddNpgsqlDbContext<IdentityApplicationDbContext>("identityDb");
        builder
            .AddIdentity()
            .Services.AddPasswordRules(builder.Configuration)
            .AddEmailSender()
            .AddEndpoints();
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

    public static WebApplication UsePlaceServices(this WebApplication app)
    {
        app.UseCoreFramework();
        app.UseHttpsRedirection();
        app.UseIdentityConfiguration();

        return app;
    }
}
