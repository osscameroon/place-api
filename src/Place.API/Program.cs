using Account;
using Core.Framework;
using Core.MediatR;
using Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.AddCoreFramework();

builder.Services.AddIdentityModule(builder);

builder.Services.AddAccountModule(configuration);

builder.Services.AddCoreMediatR(typeof(IIdentityRoot).Assembly);

builder.Services.AddCoreMediatR(typeof(IAccountRoot).Assembly);

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.UseCoreFramework();

await app.UseIdentityModule(environment);

await app.UseAccountModule(environment);

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", x => x.Response.WriteAsync("It Works!"));

await app.RunAsync();
