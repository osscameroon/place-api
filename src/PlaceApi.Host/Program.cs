IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);


{
    builder.AddProject<Projects.PlaceAPi_Identity>("Identity");
}

builder.Build().Run();
