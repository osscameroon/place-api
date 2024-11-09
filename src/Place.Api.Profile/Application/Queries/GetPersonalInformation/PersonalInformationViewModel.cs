using System.Collections.Generic;
using Place.Api.Profile.Infrastructure.Persistence.EF.Models;

namespace Place.Api.Profile.Application.Queries.GetPersonalInformation;

public sealed class PersonalInformationViewModel
{
    public string? FirstName { get; }
    public string? LastName { get; }
    public string Email { get; }
    public string? PhoneNumber { get; }
    public string? Street { get; }
    public string? City { get; }
    public string? ZipCode { get; }
    public string? Country { get; }
    public string? Gender { get; }
    public string FormattedAddress { get; }

    public PersonalInformationViewModel(ProfileReadModel profile)
    {
        FirstName = profile.FirstName;
        LastName = profile.LastName;
        Email = profile.Email;
        PhoneNumber = profile.PhoneNumber;
        Street = profile.Street;
        City = profile.City;
        ZipCode = profile.ZipCode;
        Country = profile.Country;
        Gender = profile.Gender?.ToString();
        FormattedAddress = FormatAddress();
    }

    private string FormatAddress()
    {
        List<string> addressParts = new();

        if (!string.IsNullOrWhiteSpace(this.Street))
        {
            addressParts.Add(this.Street);
        }

        List<string> cityParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(this.ZipCode))
        {
            cityParts.Add(this.ZipCode);
        }
        if (!string.IsNullOrWhiteSpace(this.City))
        {
            cityParts.Add(this.City);
        }

        if (cityParts.Count > 0)
        {
            addressParts.Add(string.Join(" ", cityParts));
        }

        if (!string.IsNullOrWhiteSpace(this.Country))
        {
            addressParts.Add(this.Country);
        }

        return addressParts.Count > 0 ? string.Join(", ", addressParts) : string.Empty;
    }
}
