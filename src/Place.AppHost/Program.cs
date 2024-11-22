using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);


{
    IResourceBuilder<PostgresServerResource> postgres = builder
        .AddPostgres("postgres")
        .WithPgAdmin()
        .WithLifetime(ContainerLifetime.Persistent);

    IResourceBuilder<PostgresDatabaseResource> profileDb = postgres.AddDatabase("profileDb");
    IResourceBuilder<PostgresDatabaseResource> identityDb = postgres.AddDatabase("identityDb");

    builder
        .AddProject<Projects.Profile_API>("Profile")
        .WithReference(profileDb)
        .WithReference(identityDb)
        .WaitFor(postgres)
        .WaitFor(profileDb);

    builder
        .AddProject<Projects.Identity_API>("Identity")
        .WithReference(identityDb)
        .WaitFor(postgres)
        .WaitFor(identityDb);
}

await builder.Build().RunAsync();
