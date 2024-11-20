namespace Core.Scalar.Docs;

public interface IScalarOptionsBuilder
{
    IScalarOptionsBuilder Enable(bool enabled = true);
    IScalarOptionsBuilder WithName(string name);
    IScalarOptionsBuilder WithTitle(string title);
    IScalarOptionsBuilder WithSideBar(bool enabled = true);

    ScalarOptions Build();
}
