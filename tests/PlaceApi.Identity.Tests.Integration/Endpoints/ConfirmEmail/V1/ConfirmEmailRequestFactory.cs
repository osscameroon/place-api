using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.ConfirmEmail.V1;

public static class ConfirmEmailRequestFactory
{
    public static string CreateEncodedToken(string token) =>
        WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    public static class DefaultValues
    {
        public const string NewEmail = "newemail@example.com";
    }
}
