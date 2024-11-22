namespace Core.Swagger;

public class SwaggerOptions
{
    public string Title { get; set; } = "API Documentation";
    public string Description { get; set; } = "API Documentation";
    public string Version { get; set; } = "v1";
    public bool EnableBearerAuth { get; set; } = true;
    public string SecuritySchemaName { get; set; } = "Bearer";
    public string SecurityScheme { get; set; } = "JWT";
    public string SecurityDescription { get; set; } =
        "JWT Authorization header using the Bearer scheme.";
    public bool EnableVersioning { get; set; } = true;
    public string RoutePrefix { get; set; } = "swagger";
}

public class ApiVersionOptions
{
    public string GroupName { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
