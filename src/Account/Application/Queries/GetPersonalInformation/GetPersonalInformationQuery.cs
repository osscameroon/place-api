using System;
using System.Threading;
using System.Threading.Tasks;
using Account.Infrastructure.Persistence.EF.Configurations;
using Account.Infrastructure.Persistence.EF.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Queries.GetPersonalInformation;

public sealed record GetPersonalInformationQuery : IRequest<PersonalInformationViewModel?>
{
    public GetPersonalInformationQuery(Guid ProfileId)
    {
        this.ProfileId = ProfileId;
    }

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
