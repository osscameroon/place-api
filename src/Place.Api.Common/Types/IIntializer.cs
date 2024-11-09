using System.Threading.Tasks;

namespace Place.Api.Common.Types;

public interface IInitializer
{
    Task InitializeAsync();
}
