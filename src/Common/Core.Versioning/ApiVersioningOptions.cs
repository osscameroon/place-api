// ApiVersioningConfiguration.cs

using System;

namespace Core.Versioning;

public class ApiVersioningOptions
{
    public int DefaultApiVersionMajor { get; set; } = 1;
    public int DefaultApiVersionMinor { get; set; } = 0;

    public bool AssumeDefaultVersionWhenUnspecified { get; set; } = true;
    public bool ReportApiVersions { get; set; } = true;
    public ApiVersionReaderType ApiVersionReaderType { get; set; } = ApiVersionReaderType.Combine;
    public ApiVersionReaderOptions ReaderOptions { get; set; } = new();

    public string GroupNameFormat { get; set; } = "'v'VVV";

    public bool SubstituteApiVersionInUrl { get; set; } = true;

    public DeprecatedApiVersionOptions DeprecatedVersionOptions { get; set; } = new();

    public ApiExplorerOptions ApiExplorerOptions { get; set; } = new();
}

public class ApiVersionReaderOptions
{
    public string HeaderName { get; set; } = "x-api-version";
    public string QueryStringParam { get; set; } = "api-version";
    public string MediaTypeParam { get; set; } = "v";
}

public enum ApiVersionReaderType
{
    Url,
    QueryString,
    Header,
    MediaType,
    Combine,
}

public class DeprecatedApiVersionOptions
{
    public string DeprecationMessage { get; set; } = "Cette version de l'API est obsolète.";

    public DateTime? SunsetDate { get; set; }

    /// <summary>
    /// URL vers la documentation concernant la dépréciation
    /// </summary>
    public string DocumentationUrl { get; set; }
}

public class ApiExplorerOptions
{
    public string GroupNameFormat { get; set; } = "'v'VVV";
    public bool SubstituteApiVersionInUrl { get; set; } = true;
    public string UrlFormat { get; set; } = "v{version:apiVersion}";
    public bool AddApiVersionParametersWhenVersionNeutral { get; set; } = true;
}
