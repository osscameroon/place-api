using System;

namespace Place.Api.Common.Types;

internal class ServiceId : IServiceId
{
    public string Id { get; } = $"{Guid.NewGuid():N}";
}
