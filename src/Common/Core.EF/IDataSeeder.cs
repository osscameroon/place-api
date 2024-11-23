using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.EF;

public interface IDataSeeder<TContext>
    where TContext : DbContext
{
    Task SeedAllAsync();
}
