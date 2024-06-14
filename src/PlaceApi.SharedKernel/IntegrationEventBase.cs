using System;
using MediatR;

namespace PlaceApi.SharedKernel;

public abstract record IntegrationEventBase : INotification
{
    public DateTimeOffset DateTimeOffset { get; set; } = DateTimeOffset.UtcNow;
}
