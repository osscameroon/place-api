using System;

namespace PlaceApi.Domain.Common.ValueObjects;

public record LastName
{
    public string Value { get; init; }

    public LastName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Last name cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static implicit operator LastName(string value) => new(value);

    public static implicit operator string(LastName value) => value.Value;
}
