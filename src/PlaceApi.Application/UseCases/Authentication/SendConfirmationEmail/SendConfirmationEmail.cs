using MediatR;
using PlaceApi.Domain.Authentication.Entities;

namespace PlaceApi.Application.UseCases.Authentication.Notifications.Confirmation;

public record SendConfirmationEmail(ApplicationUser User, string Email, bool IsChange = false)
    : INotification;
