using PlaceApi.Domain.Common.ValueObjects;

namespace PlaceApi.Domain.Authentication.Entities;

public class Profile
{
    public FirstName? FirstName { get; set; }

    public LastName? LastName { get; set; }

    public Address? Address { get; set; }

    public DateOfBirth? DateOfBirth { get; set; }
}
