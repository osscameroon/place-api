using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.API.OpenApi;

public class ApiVersionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ApiParameterDescription? versionParameter =
            context.ApiDescription.ParameterDescriptions.FirstOrDefault(p =>
                p.Name.Equals("api-version", StringComparison.InvariantCultureIgnoreCase)
            );

        if (versionParameter is null)
        {
            return;
        }

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = versionParameter.Name,
                In = ParameterLocation.Query,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" },
            }
        );
    }
}
