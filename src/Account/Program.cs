using Account;
using Account.Infrastructure.Persistence.EF.Configurations;
using Core.Framework;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddCoreFramework();

builder.Services.RegisterPlace(builder);

WebApplication app = builder.Build();

app.UsePlaceServices();
await app.InitializeDatabaseAsync();

await app.RunAsync();
