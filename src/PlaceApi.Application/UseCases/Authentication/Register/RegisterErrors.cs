using ErrorOr;

namespace PlaceApi.Application.UseCases.Authentication.Register;

public static class RegisterErrors
{
    public static Error InvalidEmail { get; } =
        Error.Validation(
            code: nameof(InvalidEmail),
            description: "The email provided is not a valid. "
        );
}
