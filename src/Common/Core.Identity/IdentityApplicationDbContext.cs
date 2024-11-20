using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Identity;

/// <summary>
/// Database context for Identity authentication.
/// Handles user data persistence using Entity Framework Core.
/// </summary>
public class IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options);
