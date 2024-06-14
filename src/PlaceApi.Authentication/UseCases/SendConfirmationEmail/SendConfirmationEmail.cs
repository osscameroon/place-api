using MediatR;
using PlaceApi.Authentication.Domain;

namespace PlaceApi.Authentication.UseCases.SendConfirmationEmail;

internal record SendConfirmationEmail(ApplicationUser User, string Email, bool IsChange = false)
    : INotification;
