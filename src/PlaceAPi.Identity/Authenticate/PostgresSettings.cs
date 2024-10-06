namespace PlaceAPi.Identity.Authenticate;

public record PostgresOptions
{
    public const string Key = "Postgres";
    public string ConnectionString { get; set; } = default!;
}
