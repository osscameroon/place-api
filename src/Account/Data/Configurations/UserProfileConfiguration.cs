using Account.Profile.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Id)
            .HasConversion(id => id.Value, value => new UserProfileId(value))
            .ValueGeneratedNever();

        builder.Property(p => p.UserId).IsRequired();

        builder
            .Property(p => p.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");

        builder.Property(p => p.CreatedBy).IsRequired();

        builder.Property(p => p.LastModifiedAt).HasColumnType("timestamp with time zone");

        builder.Property(p => p.LastModifiedBy);

        builder.Property(p => p.DeletedAt).HasColumnType("timestamp with time zone");

        builder.Property(p => p.DeletedBy);

        builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

        // Configuration de PersonalInfo comme Owned Entity
        builder.OwnsOne(
            p => p.PersonalInfo,
            personalInfo =>
            {
                personalInfo
                    .Property(pi => pi.FirstName)
                    .HasConversion(ln => ln!.Value, value => FirstName.Create(value).Value)
                    .HasMaxLength(FirstName.MaxLength)
                    .HasColumnName("FirstName");

                personalInfo
                    .Property(pi => pi.LastName)
                    .HasConversion(ln => ln!.Value, value => LastName.Create(value).Value)
                    .HasMaxLength(LastName.MaxLength)
                    .HasColumnName("LastName");

                personalInfo
                    .Property(pi => pi.DateOfBirth)
                    .HasConversion(dob => dob!.Value, value => DateOfBirth.Create(value).Value)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("DateOfBirth");

                personalInfo
                    .Property(pi => pi.PhoneNumber)
                    .HasConversion(phone => phone!.Value, value => PhoneNumber.Parse(value).Value)
                    .HasColumnName("PhoneNumber");

                personalInfo.Property(pi => pi.Gender).HasConversion<int>().HasColumnName("Gender");

                personalInfo.OwnsOne(
                    pi => pi.Address,
                    address =>
                    {
                        address.Property(a => a.Street).HasColumnName("Street");

                        address.Property(a => a.ZipCode).HasColumnName("ZipCode");

                        address.Property(a => a.City).HasColumnName("City");

                        address.Property(a => a.Country).HasColumnName("Country");

                        address
                            .Property(a => a.AdditionalDetails)
                            .HasColumnName("AdditionalAddressDetails");

                        address.OwnsOne(
                            a => a.Coordinates,
                            coordinates =>
                            {
                                coordinates
                                    .Property(c => c.Latitude)
                                    .HasPrecision(9, 6)
                                    .HasColumnName("Latitude");

                                coordinates
                                    .Property(c => c.Longitude)
                                    .HasPrecision(9, 6)
                                    .HasColumnName("Longitude");
                            }
                        );
                    }
                );
            }
        );

        builder
            .Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.DeletedBy).HasColumnName("DeletedBy");

        builder
            .Property(p => p.IsDeleted)
            .HasColumnName("IsDeleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(p => p.Email).IsUnique();

        builder.HasIndex(p => p.IsDeleted);

        builder.HasIndex(p => p.IsDeleted);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
