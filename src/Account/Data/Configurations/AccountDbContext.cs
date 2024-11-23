using Account.Data.Models;
using Core.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Account.Data.Configurations;

public class AccountDbContext : AppDbContextBase
{
    private IDbContextTransaction _currentTransaction;

    public AccountDbContext(
        DbContextOptions<AccountDbContext> options,
        IHttpContextAccessor httpContextAccessor
    )
        : base(options, httpContextAccessor) { }

    public DbSet<ProfileReadModel> Profiles => Set<ProfileReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
