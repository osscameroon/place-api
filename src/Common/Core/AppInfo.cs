namespace Core;

public record AppInfo(string Name, string Version)
{
    public override string ToString() => $"{Name} {Version}";
}
