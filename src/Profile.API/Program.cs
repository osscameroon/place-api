using Core.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Profile.API;
using Profile.API.Infrastructure.Persistence.EF.Configurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddCoreFramework();

builder.Services.RegisterPlace(builder);
builder.Services.RegisterMediatr();
builder.AddServiceDefaults();

WebApplication app = builder.Build();

app.UsePlaceServices();
await app.InitializeDatabaseAsync();

await app.RunAsync();
