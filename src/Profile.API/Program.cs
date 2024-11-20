using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Profile.API;
using Profile.API.Infrastructure.Persistence.EF.Configurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterPlace(builder);
builder.Services.AddControllers();
builder.Services.RegisterMediatr();
builder.AddServiceDefaults();

WebApplication app = builder.Build();

app.UsePlaceServices();
app.MapControllers();
await app.InitializeDatabaseAsync();

await app.RunAsync();
