using Account.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Account.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<AccountDbContext> builder = new();

        builder.UseNpgsql(
            "Server=localhost;Port=5432;Database=flight_db;User Id=postgres;Password=postgres;Include Error Detail=true"
        );
        return new AccountDbContext(builder.Options, null);
    }
}
