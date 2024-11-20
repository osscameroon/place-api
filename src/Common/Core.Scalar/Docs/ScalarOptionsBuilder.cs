namespace Core.Scalar.Docs;

public class ScalarOptionsBuilder : IScalarOptionsBuilder
{
    private readonly ScalarOptions _options = new();

    public IScalarOptionsBuilder Enable(bool enabled = true)
    {
        _options.Enabled = enabled;
        return this;
    }

    public IScalarOptionsBuilder WithName(string name)
    {
        _options.Name = name;
        return this;
    }

    public IScalarOptionsBuilder WithTitle(string title)
    {
        _options.Title = title;
        return this;
    }

    public IScalarOptionsBuilder WithSideBar(bool enabled = true)
    {
        _options.SideBar = enabled;
        return this;
    }

    public ScalarOptions Build() => _options;
}
