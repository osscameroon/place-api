using System.Threading;
using System.Threading.Tasks;

using FastEndpoints;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using PlaceApi.Authentication.UseCases.Register;

namespace PlaceApi.Authentication.Endpoints.V1.Register;

public class Endpoint(ISender sender)
    : Endpoint<RegisterRequest, Results<Ok<RegisterResult>, ValidationProblem>>
{
    public override void Configure()
    {
        Post(V1.Routes.Register.Endpoint);
        AllowAnonymous();
        Description(b =>
            b.ProducesProblemDetails(400, "application/json+problem") //if using RFC errors
                .ProducesProblemFE<InternalErrorResponse>(500)
        );
    }

    public override async Task<Results<Ok<RegisterResult>, ValidationProblem>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken
    ) =>
        await sender.Send(
            new RegisterCommand(request.UserName, request.Email, request.Password),
            cancellationToken
        );
}
