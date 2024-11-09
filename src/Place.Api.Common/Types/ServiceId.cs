using System;

namespace Place.Api.Common.Types;

/// <summary>
/// Default implementation of IServiceId using GUID.
/// </summary>
internal class ServiceId : IServiceId
{
    /// <inheritdoc />
    public string Id { get; } = $"{Guid.NewGuid():N}";
}
