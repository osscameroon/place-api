using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Mediatr.Behaviours.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Profile.API.Infrastructure.Persistence.EF.Configurations;
using Profile.API.Infrastructure.Persistence.EF.Models;

namespace Profile.API.Application.Queries.GetPersonalInformation;

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
