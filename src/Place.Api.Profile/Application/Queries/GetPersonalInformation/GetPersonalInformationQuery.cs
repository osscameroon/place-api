using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Place.Api.Profile.Infrastructure.Persistence.EF.Models;

namespace Place.Api.Profile.Application.Queries.GetPersonalInformation;

public sealed record GetPersonalInformationQuery(Guid ProfileId)
    : IRequest<PersonalInformationViewModel?>;

public class GetProfileByIdQueryHandler(
    ILogger<GetProfileByIdQueryHandler> logger,
    ProfileDbContext dbContext
) : IRequestHandler<GetPersonalInformationQuery, PersonalInformationViewModel?>
{
    public async Task<PersonalInformationViewModel?> Handle(
        GetPersonalInformationQuery request,
        CancellationToken cancellationToken
    )
    {
        ProfileReadModel? profile = await dbContext
            .Profiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId, cancellationToken);

        if (profile is null)
        {
            logger.LogInformation("Profile with ID {ProfileId} was not found.", request.ProfileId);
            return null;
        }

        logger.LogInformation("Profile with ID {ProfileId} was found.", request.ProfileId);

        return new PersonalInformationViewModel(profile);
    }
}
