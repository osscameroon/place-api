using System;

namespace Place.Api.Common.Mediatr.Behaviours.Logging;

/// <summary>
/// Attribute to configure how a property or class should be logged.
/// Can be applied to both properties and classes to control their logging behavior.
/// </summary>
/// <remarks>
/// When applied to a class, it controls whether the class's properties should be logged by default.
/// When applied to a property, it controls how that specific property is logged.
/// </remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true
)]
public class LoggableAttribute : Attribute
{
    /// <summary>
    /// Gets or sets whether this property/class should be included in logs.
    /// Defaults to true.
    /// </summary>
    /// <example>
    /// [Loggable(IncludeInLogs = false)]
    /// public string SensitiveData { get; set; }
    /// </example>
    public bool IncludeInLogs { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this property contains sensitive information that should be masked.
    /// When true, the value will be masked according to the LogMask attribute if present,
    /// or completely masked if no LogMask attribute is specified.
    /// Defaults to false.
    /// </summary>
    /// <example>
    /// [Loggable(IsSensitive = true)]
    /// public string CreditCardNumber { get; set; }
    /// </example>
    public bool IsSensitive { get; set; } = false;

    /// <summary>
    /// Gets or sets an alternative name to use when logging this property.
    /// If null or empty, the property's actual name will be used.
    /// </summary>
    /// <example>
    /// [Loggable(DisplayName = "CustomerEmail")]
    /// public string Email { get; set; }
    /// </example>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Initializes a new instance of the LoggableAttribute class with default values.
    /// </summary>
    public LoggableAttribute() { }

    /// <summary>
    /// Initializes a new instance of the LoggableAttribute class with the specified inclusion state.
    /// </summary>
    /// <param name="includeInLogs">Whether to include this property/class in logs.</param>
    public LoggableAttribute(bool includeInLogs)
    {
        IncludeInLogs = includeInLogs;
    }

    /// <summary>
    /// Initializes a new instance of the LoggableAttribute class with a display name.
    /// </summary>
    /// <param name="displayName">The name to use when logging this property.</param>
    public LoggableAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

/// <summary>
/// Specifies how sensitive data should be masked in logs.
/// Must be used in conjunction with LoggableAttribute when masking is needed.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class LogMaskAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the strategy to use when masking the value.
    /// Defaults to MaskingStrategy.Complete.
    /// </summary>
    public MaskingStrategy Strategy { get; set; } = MaskingStrategy.Complete;

    /// <summary>
    /// Gets or sets a custom mask pattern.
    /// Only used when Strategy is set to MaskingStrategy.Custom.
    /// The pattern can include {value} placeholder which will be replaced with the actual value.
    /// </summary>
    /// <example>
    /// [LogMask(Strategy = MaskingStrategy.Custom, CustomMask = "ID:{value}")]
    /// public string CustomerId { get; set; }
    /// </example>
    public string? CustomMask { get; set; }

    /// <summary>
    /// Initializes a new instance of the LogMaskAttribute class with default masking strategy.
    /// </summary>
    public LogMaskAttribute() { }

    /// <summary>
    /// Initializes a new instance of the LogMaskAttribute class with the specified masking strategy.
    /// </summary>
    /// <param name="strategy">The strategy to use for masking values.</param>
    public LogMaskAttribute(MaskingStrategy strategy)
    {
        Strategy = strategy;
    }
}

/// <summary>
/// Defines the available strategies for masking sensitive data in logs.
/// </summary>
public enum MaskingStrategy
{
    /// <summary>
    /// Completely masks the value with asterisks.
    /// Example: "sensitive" -> "***"
    /// </summary>
    Complete = 0,

    /// <summary>
    /// Shows only the last few characters of the value.
    /// Example: "1234567890" -> "***7890"
    /// </summary>
    PartialStart = 1,

    /// <summary>
    /// Shows only the first few characters of the value.
    /// Example: "1234567890" -> "1234***"
    /// </summary>
    PartialEnd = 2,

    /// <summary>
    /// For email addresses, shows only the domain part.
    /// Example: "user@example.com" -> "***@example.com"
    /// </summary>
    EmailDomain = 3,

    /// <summary>
    /// For phone numbers, shows only the country code.
    /// Example: "+1 234-567-8900" -> "+1 ***"
    /// </summary>
    PhoneCountryCode = 4,

    /// <summary>
    /// Uses a custom masking pattern specified in LogMaskAttribute.CustomMask.
    /// Example: With CustomMask = "ID:{value}" -> "ID:123***"
    /// </summary>
    Custom = 5,
}
