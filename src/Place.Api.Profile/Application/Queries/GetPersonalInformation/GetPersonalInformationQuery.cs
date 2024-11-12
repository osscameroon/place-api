using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Place.Api.Common.Mediatr.Behaviours.Logging;
using Place.Api.Profile.Infrastructure.Persistence.EF.Configurations;
using Place.Api.Profile.Infrastructure.Persistence.EF.Models;

namespace Place.Api.Profile.Application.Queries.GetPersonalInformation;

[Loggable]
public sealed record GetPersonalInformationQuery : IRequest<PersonalInformationViewModel?>
{
    public GetPersonalInformationQuery(Guid ProfileId)
    {
        this.ProfileId = ProfileId;
    }

    [Loggable]
    public Guid ProfileId { get; init; }
}

public class GetProfileByIdQueryHandler(ProfileDbContext dbContext)
    : IRequestHandler<GetPersonalInformationQuery, PersonalInformationViewModel?>
{
    public async Task<PersonalInformationViewModel?> Handle(
        GetPersonalInformationQuery request,
        CancellationToken cancellationToken
    )
    {
        ProfileReadModel? profile = await dbContext
            .Profiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProfileId, cancellationToken);

        return profile is null ? null : new PersonalInformationViewModel(profile);
    }
}
