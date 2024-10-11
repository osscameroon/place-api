using Microsoft.AspNetCore.Identity.Data;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.Register;

public class RegisterRequestFactory
{
    public static RegisterRequest CreateRegisterRequest()
    {
        return new RegisterRequest { Email = "newuser@example.com", Password = "Password123!" };
    }

    public class DefaultValues
    {
        public const string Email = "test@example.com";
        public const string Password = "Password123!";
    }
}
