using Aspire.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);


{
    builder.AddProject<Projects.Identity_APi>("Identity");
    builder.AddProject<Projects.Profile_API>("Profile");
}

await builder.Build().RunAsync();
