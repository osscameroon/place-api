using System.Threading;
using System.Threading.Tasks;

namespace Core.Database;

/// <summary>
/// Interface for database context seeding operations
/// Implement this interface in your DbContext if you want to support data seeding
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Seeds initial or test data into the database
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation if needed</param>
    /// <returns>A task representing the seeding operation</returns>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
