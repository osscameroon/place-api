using Account.Data.Models;
using Account.Profile.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Account.Data.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<ProfileReadModel>
{
    public void Configure(EntityTypeBuilder<ProfileReadModel> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.UserId).IsRequired();

        builder.Property(p => p.Email).IsRequired().HasMaxLength(Email.MaxLength);

        // Date properties with appropriate data type
        builder.Property(p => p.DateOfBirth).HasColumnType("date"); // Stores only the date part, no time

        builder.Property(p => p.CreatedAt).IsRequired().HasColumnType("timestamp with time zone"); // Full timestamp for audit

        builder.Property(p => p.LastModifiedAt).HasColumnType("timestamp with time zone");

        builder.Property(p => p.DeletedAt).HasColumnType("timestamp with time zone");

        builder.Property(p => p.CreatedBy).IsRequired();

        // PersonalInfo properties
        builder.Property(p => p.FirstName).HasMaxLength(FirstName.MaxLength);

        builder.Property(p => p.LastName).HasMaxLength(LastName.MaxLength);

        builder.Property(p => p.PhoneNumber);

        // Address properties
        builder.Property(p => p.Street);

        builder.Property(p => p.ZipCode);

        builder.Property(p => p.City);

        builder.Property(p => p.Country);

        builder.Property(p => p.AdditionalAddressDetails);

        // Enum conversion
        builder.Property(p => p.Gender).HasConversion<int>();

        // Precision for coordinates
        builder.Property(p => p.Latitude).HasPrecision(9, 6); // Allows for precise GPS coordinates

        builder.Property(p => p.Longitude).HasPrecision(9, 6);

        // Query Filter to exclude soft deleted profiles
        builder.HasQueryFilter(p => !p.DeletedAt.HasValue);

        // Unique email index, only for non-deleted profiles
        builder.HasIndex(p => p.Email).IsUnique().HasFilter("\"IsDeleted\" = false");

        // Index for last name and first name, only for non-deleted profiles
        builder.HasIndex(p => new { p.LastName, p.FirstName }).HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(p => p.DateOfBirth);

        builder.HasIndex(p => p.DateOfBirth);

        builder.HasIndex(p => p.IsDeleted);

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
