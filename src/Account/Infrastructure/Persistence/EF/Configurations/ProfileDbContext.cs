using Account.Infrastructure.Persistence.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.EF.Configurations;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext() { }

    public DbSet<ProfileReadModel> Profiles { get; set; } = null!;

    public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProfileConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
