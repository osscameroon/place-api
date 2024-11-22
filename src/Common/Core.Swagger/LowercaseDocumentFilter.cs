using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Swagger;

public class LowercaseDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        Dictionary<string, OpenApiPathItem> paths = swaggerDoc.Paths.ToDictionary(
            entry => entry.Key.ToLowerInvariant(),
            entry => entry.Value
        );

        swaggerDoc.Paths = new OpenApiPaths();
        foreach (KeyValuePair<string, OpenApiPathItem> path in paths)
        {
            swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }
}
