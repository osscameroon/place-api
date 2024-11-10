using Place.Api.Profile.Application.Queries.GetPersonalInformation;

namespace Place.Api.Profile.Apis.V1.Responses;

public record PersonalInformationResponse
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

    public PersonalInformationResponse(PersonalInformationViewModel vm)
    {
        FirstName = vm.FirstName;
        LastName = vm.LastName;
        Email = vm.Email;
        PhoneNumber = vm.PhoneNumber;
        Street = vm.Street;
        City = vm.City;
        ZipCode = vm.ZipCode;
        Country = vm.Country;
        Gender = vm.Gender;
        FormattedAddress = vm.FormattedAddress;
    }
}
