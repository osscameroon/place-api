using Account.Profile.Models;
using Core.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Account.Data.Configurations;

public class AccountDbContext : AppDbContextBase
{
    public AccountDbContext(
        DbContextOptions<AccountDbContext> options,
        IHttpContextAccessor httpContextAccessor
    )
        : base(options, httpContextAccessor) { }

    public DbSet<UserProfile> Profiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserProfileConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
