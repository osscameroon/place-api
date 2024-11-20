using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.Register.V1;

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
