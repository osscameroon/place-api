namespace Core.Scalar.Docs;

public class ScalarOptions
{
    public bool Enabled { get; set; } = true;
    public string? Name { get; set; }
    public string? Title { get; set; }

    public bool SideBar { get; set; } = true;
    public string? RoutePrefix { get; set; }
}
