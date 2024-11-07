IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);


{
    builder.AddProject<Projects.PlaceAPi_Identity>("Identity");
    builder.AddProject<Projects.Place_Api_Profile>("Profile");
}

await builder.Build().RunAsync();
