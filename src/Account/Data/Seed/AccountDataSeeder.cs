using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Account.Data.Configurations;
using Account.Data.Models;
using Account.Profile.Models;
using Core.EF;
using Microsoft.EntityFrameworkCore;

namespace Account.Data.Seed;

public class AccountDataSeeder(AccountDbContext context) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedProfileAsync();
    }

    private async Task SeedProfileAsync()
    {
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
