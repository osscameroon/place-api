using FastEndpoints;

namespace PlaceApi.Authentication.Endpoints.V1.Login;

public class LoginSummary : Summary<Login.Endpoint>
{
    public LoginSummary()
    {
        Summary = V1.Routes.Login.OpenApi.Summary;
        Description = V1.Routes.Login.OpenApi.Description;
        Response(200, "User authenticated");
        Response(403, "Access denied");
    }
}
