using Microsoft.EntityFrameworkCore;
using Profile.API.Infrastructure.Persistence.EF.Models;

namespace Profile.API.Infrastructure.Persistence.EF.Configurations;

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
