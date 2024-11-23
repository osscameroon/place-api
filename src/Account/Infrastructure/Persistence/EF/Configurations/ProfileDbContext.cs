using Account.Infrastructure.Persistence.EF.Models;
using Core.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Account.Infrastructure.Persistence.EF.Configurations;

public class ProfileDbContext : AppDbContextBase
{
    private IDbContextTransaction _currentTransaction;

    public ProfileDbContext(
        DbContextOptions<ProfileDbContext> options,
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
