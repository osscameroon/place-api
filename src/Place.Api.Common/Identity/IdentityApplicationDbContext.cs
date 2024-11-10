using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Place.Api.Common.Identity;

public class IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options);
