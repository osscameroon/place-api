using System;
using System.Threading.Tasks;
using Account.Data.Configurations;
using Account.Data.Models;
using Account.Profile.Models;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Account.IntegrationTests.Common;

public class TestDataSeeder
{
    private readonly AccountDbContext _dbContext;

    public TestDataSeeder(AccountDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<UserProfile> SeedBasicProfile()
    {
        ProfileReadModel profile = new ProfileReadModel
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

        ErrorOr<UserProfile> domainResult = profile.ToDomain();
        if (domainResult.IsError)
            throw new InvalidOperationException(
                $"Failed to create profile: {domainResult.FirstError.Description}"
            );

        return await SeedProfile(domainResult.Value);
    }

    public async Task<UserProfile> SeedProfile(UserProfile profile)
    {
        if (profile is null)
            throw new ArgumentNullException(nameof(profile));

        _dbContext.Profiles.Add(profile);
        await _dbContext.SaveChangesAsync();

        UserProfile? savedProfile = await _dbContext
            .Profiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == profile.Id);

        if (savedProfile is null)
            throw new InvalidOperationException(
                $"Failed to retrieve saved profile with ID: {profile.Id}"
            );

        return savedProfile;
    }

    public async Task<UserProfile> SeedPartialProfile()
    {
        ProfileReadModel profile = new ProfileReadModel
        {
            Id = Guid.NewGuid(),
            Email = "partial@example.com",
            City = "Lyon",
            Country = "France",
        };

        ErrorOr<UserProfile> domainResult = profile.ToDomain();
        if (domainResult.IsError)
            throw new InvalidOperationException(
                $"Failed to create partial profile: {domainResult.FirstError.Description}"
            );

        return await SeedProfile(domainResult.Value);
    }

    public async Task<UserProfile[]> SeedMultipleProfiles(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));

        UserProfile[] profiles = new UserProfile[count];

        for (int i = 0; i < count; i++)
        {
            ProfileReadModel readModel = new ProfileReadModel
            {
                Id = Guid.NewGuid(),
                FirstName = $"User{i}",
                LastName = $"Test{i}",
                Email = $"user{i}@example.com",
                PhoneNumber = $"+3361234{i:D4}",
                City = "Paris",
                Country = "France",
            };

            ErrorOr<UserProfile> domainResult = readModel.ToDomain();
            if (domainResult.IsError)
                throw new InvalidOperationException(
                    $"Failed to create profile {i}: {domainResult.FirstError.Description}"
                );

            profiles[i] = domainResult.Value;
        }

        await _dbContext.Profiles.AddRangeAsync(profiles);
        await _dbContext.SaveChangesAsync();

        return profiles;
    }
}
