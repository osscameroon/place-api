IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);


{
    builder.AddProject<Projects.PlaceAPi_Identity>("Identity");
    builder.AddProject<Projects.Place_Api_ProfileManagement>("ProfileManagement");
}

await builder.Build().RunAsync();
