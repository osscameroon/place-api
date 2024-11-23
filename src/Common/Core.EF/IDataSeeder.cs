using System.Threading.Tasks;

namespace Core.EF;

public interface IDataSeeder
{
    Task SeedAllAsync();
}
