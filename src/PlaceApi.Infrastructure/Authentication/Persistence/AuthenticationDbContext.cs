using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Infrastructure.Authentication.Persistence;

public class AuthenticationDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
        : base(options) { }
}
