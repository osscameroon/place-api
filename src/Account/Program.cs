using Account;
using Core.Framework;
using Core.MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddCoreFramework();
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Services.AddAccountModule(configuration);

builder.Services.AddCoreMediatR(typeof(AccountModule).Assembly);

WebApplication app = builder.Build();

await app.UseAccountModule(environment);

await app.RunAsync();
