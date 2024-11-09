using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Place.Api.Common;
using Place.Api.Common.Logging;
using Place.Api.Common.Swagger.Docs;
using Place.Api.Common.Swagger.WebApi;
using Place.Api.Profile;
using Place.Api.Profile.Apis;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseLogging((context, loggerConfiguration) => { });

builder
    .Services.AddPlace(builder.Configuration)
    .AddCorrelationContextLogging()
    .AddWebApiSwaggerDocs()
    .AddSwaggerDocs();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProfileDatabase(builder.Configuration);
builder.Services.RegisterMediatr();

WebApplication app = builder.Build();

await app.InitializeDatabaseAsync();

app.UsePlace().UserCorrelationContextLogging();

app.UseSwaggerDocs();

app.UseHttpsRedirection();

app.MapProfilesApiV1();
await app.RunAsync();
