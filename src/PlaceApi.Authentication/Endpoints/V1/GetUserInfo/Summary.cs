using FastEndpoints;
using Microsoft.AspNetCore.Identity.Data;

namespace PlaceApi.Authentication.Endpoints.V1.GetUserInfo;

public class GetUserInfoSummary : Summary<GetUserInfo.Endpoint>
{
    public GetUserInfoSummary()
    {
        Summary = V1.Routes.GetUserinfo.OpenApi.Summary;
        Description = V1.Routes.GetUserinfo.OpenApi.Description;
        Response<InfoResponse>(200, "User authenticated");
        Response(403, "User not found");
    }
}
