using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Account.Data.Models;
using Account.Profile.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Account.Data.Configurations;

/// <summary>
/// Extension methods for database initialization and seeding.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Ensures the database is created and migrations are applied.
    /// Also seeds initial data in development environment.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ILogger<AccountDbContext> logger = services.GetRequiredService<ILogger<AccountDbContext>>();

        try
        {
            AccountDbContext context = services.GetRequiredService<AccountDbContext>();

            logger.LogInformation("Starting database migration...");

            await context.Database.MigrateAsync();

            logger.LogInformation("Database migration completed successfully");

            // Seed data only in development
            if (app.Environment.IsDevelopment())
            {
                logger.LogInformation("Development environment detected. Starting data seeding...");
                await SeedDataAsync(context);
                logger.LogInformation("Data seeding completed successfully");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedDataAsync(AccountDbContext context)
    {
        // Check if there's any data already
        if (await context.Profiles.AnyAsync())
        {
            return;
        }

        // Create sample profiles
        List<ProfileReadModel> profiles =
        [
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = Gender.Male,
                PhoneNumber = "+33612345678",
                Street = "123 Main St",
                ZipCode = "75001",
                City = "Paris",
                Country = "France",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                IsDeleted = false,
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1985, 5, 15),
                Gender = Gender.Female,
                PhoneNumber = "+237655555555",
                City = "Yaoundé",
                Country = "Cameroon",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                IsDeleted = false,
            },
            // Add a soft-deleted profile for testing

            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Email = "deleted@example.com",
                FirstName = "Deleted",
                LastName = "User",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                CreatedBy = Guid.NewGuid(),
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
                DeletedBy = Guid.NewGuid(),
            },
        ];

        await context.Profiles.AddRangeAsync(profiles);
        await context.SaveChangesAsync();
    }
}
