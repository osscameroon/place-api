using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Place.Api.Profile.Domain.Profile;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Place.Api.Profile.Infrastructure.Persistence.EF.Models;

namespace Place.Api.Integration.Tests.Common;

public class TestDataSeeder(ProfileDbContext dbContext)
{
    public async Task<ProfileReadModel> SeedBasicProfile()
    {
        ProfileReadModel profile =
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+33612345678",
                Street = "123 Main St",
                City = "Paris",
                ZipCode = "75001",
                Country = "France",
                Gender = Gender.Male,
            };

        return await SeedProfile(profile);
    }

    public async Task<ProfileReadModel> SeedProfile(ProfileReadModel profile)
    {
        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();

        return await dbContext.Profiles.AsNoTracking().FirstAsync(p => p.Id == profile.Id);
    }

    public async Task<ProfileReadModel> SeedPartialProfile()
    {
        ProfileReadModel profile =
            new()
            {
                Id = Guid.NewGuid(),
                Email = "partial@example.com",
                City = "Lyon",
                Country = "France",
            };

        return await SeedProfile(profile);
    }

    public async Task<ProfileReadModel[]> SeedMultipleProfiles(int count)
    {
        ProfileReadModel[] profiles = new ProfileReadModel[count];

        for (int i = 0; i < count; i++)
        {
            profiles[i] = new ProfileReadModel
            {
                Id = Guid.NewGuid(),
                FirstName = $"User{i}",
                LastName = $"Test{i}",
                Email = $"user{i}@example.com",
                PhoneNumber = $"+3361234{i:D4}",
                City = "Paris",
                Country = "France",
            };
        }

        dbContext.Profiles.AddRange(profiles);
        await dbContext.SaveChangesAsync();

        return profiles;
    }
}
