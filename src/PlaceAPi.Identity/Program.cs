using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Place.Core;
using Place.Core.Identity;
using Place.Core.Logging;
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

app.UsePlace().UserCorrelationContextLogging();


{
    if (app.Environment.IsDevelopment())
    {
        app.WithSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    //Log all errors in the application
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            IExceptionHandlerFeature? errorFeature =
                context.Features.Get<IExceptionHandlerFeature>();
            Exception? exception = errorFeature?.Error;

            ILogger<Program> logger = context.RequestServices.GetRequiredService<
                ILogger<Program>
            >();

            logger.LogError(String.Format("Stacktrace of error: {0}", exception?.StackTrace));

            await Task.CompletedTask;
        });
    });

    app.UseIdentityConfiguration();

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
    app.WithAuthenticationEndpoints();
    await app.RunAsync();
}

public partial class Program { }
