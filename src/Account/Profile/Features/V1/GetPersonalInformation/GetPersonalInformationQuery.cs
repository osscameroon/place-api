using System;
using System.Threading;
using System.Threading.Tasks;
using Account.Data.Configurations;
using Account.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Account.Profile.Features.V1.GetPersonalInformation;

public sealed record GetPersonalInformationQuery(Guid ProfileId)
    : IRequest<PersonalInformationViewModel?>;

public class GetProfileByIdQueryHandler(AccountDbContext dbContext)
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
