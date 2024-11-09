namespace Place.Api.Common.Types;

public interface IStartupInitializer : IInitializer
{
    void AddInitializer(IInitializer initializer);
}
