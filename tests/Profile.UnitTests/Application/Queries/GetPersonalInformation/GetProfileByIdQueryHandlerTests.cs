using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Profile.API.Application.Queries.GetPersonalInformation;
using Profile.API.Infrastructure.Persistence.EF.Configurations;
using Profile.API.Infrastructure.Persistence.EF.Models;
using Profile.UnitTests.Common;

namespace Profile.UnitTests.Application.Queries.GetPersonalInformation;

[Trait("Category", "Unit")]
public class GetProfileByIdQueryHandlerTests
{
    private readonly ProfileDbContext _dbContext;
    private readonly GetProfileByIdQueryHandler _handler;

    public GetProfileByIdQueryHandlerTests()
    {
        DbContextOptions<ProfileDbContext> options = new DbContextOptionsBuilder<ProfileDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ProfileDbContext(options);
        _handler = new GetProfileByIdQueryHandler(_dbContext);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Handle_WhenProfileExists_ShouldReturnCompleteViewModel()
    {
        // Arrange
        ProfileReadModel profile = ProfileTestData.CreateBasicProfile();
        await _dbContext.Profiles.AddAsync(profile);
        await _dbContext.SaveChangesAsync();

        GetPersonalInformationQuery query = new(profile.Id);

        // Act
        PersonalInformationViewModel? result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be(profile.FirstName);
        result.LastName.Should().Be(profile.LastName);
        result.Email.Should().Be(profile.Email);
        result.PhoneNumber.Should().Be(profile.PhoneNumber);
        result.Street.Should().Be(profile.Street);
        result.City.Should().Be(profile.City);
        result.ZipCode.Should().Be(profile.ZipCode);
        result.Country.Should().Be(profile.Country);
        result.Gender.Should().Be("Male");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_WhenProfileDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        GetPersonalInformationQuery query = new(Guid.NewGuid());

        // Act
        PersonalInformationViewModel? result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ShouldRespectDeletionStatus(bool isDeleted)
    {
        // Arrange
        ProfileReadModel profile = ProfileTestData.CreateBasicProfile(p =>
        {
            p.IsDeleted = isDeleted;
            if (isDeleted)
            {
                p.DeletedAt = DateTime.UtcNow;
                p.DeletedBy = Guid.NewGuid();
            }
        });

        await _dbContext.Profiles.AddAsync(profile);
        await _dbContext.SaveChangesAsync();

        GetPersonalInformationQuery query = new(profile.Id);

        // Act
        PersonalInformationViewModel? result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        if (isDeleted)
            result.Should().BeNull();
        else
            result.Should().NotBeNull();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WithCancellationToken_ShouldRespectCancellation(bool isCancelled)
    {
        // Arrange
        GetPersonalInformationQuery query = new(Guid.NewGuid());
        CancellationTokenSource cts = new();
        if (isCancelled)
            await cts.CancelAsync();

        // Act
        Func<Task<PersonalInformationViewModel?>> act = () => _handler.Handle(query, cts.Token);

        // Assert
        if (isCancelled)
            await act.Should().ThrowAsync<OperationCanceledException>();
        else
            await act.Should().NotThrowAsync();
    }
}
