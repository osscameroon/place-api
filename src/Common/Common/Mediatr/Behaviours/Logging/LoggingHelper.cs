using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Mediatr.Behaviours.Logging;

/// <summary>
/// Helper class providing common logging functionality and property extraction.
/// </summary>
public static class LoggingHelper
{
    /// <summary>
    /// Set of types that are considered simple enough to log directly.
    /// </summary>
    private static readonly HashSet<Type> SimpleTypes =
        new()
        {
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(decimal),
            typeof(bool),
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
        };

    /// <summary>
    /// Extracts properties from an object that should be included in logs.
    /// </summary>
    /// <param name="obj">The object to extract properties from.</param>
    /// <returns>A dictionary containing the properties to be logged.</returns>
    public static IDictionary<string, object?> ExtractLoggableProperties(object? obj)
    {
        Dictionary<string, object?> result = new Dictionary<string, object?>();
        if (obj is null)
            return result;

        Type type = obj.GetType();

        // Check if the type itself is loggable
        LoggableAttribute? typeLoggable = type.GetCustomAttribute<LoggableAttribute>();
        if (typeLoggable?.IncludeInLogs == false)
            return result;

        foreach (
            PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        )
        {
            try
            {
                // Check if property should be logged
                LoggableAttribute? propLoggable = property.GetCustomAttribute<LoggableAttribute>();
                if (propLoggable?.IncludeInLogs == false)
                    continue;

                object? value = property.GetValue(obj);
                if (value is null)
                    continue;

                string propertyName = propLoggable?.DisplayName ?? property.Name;

                // Handle masking for sensitive properties
                LogMaskAttribute? maskAttr = property.GetCustomAttribute<LogMaskAttribute>();
                if (propLoggable?.IsSensitive == true || maskAttr is not null)
                {
                    result[propertyName] = MaskValue(value.ToString(), maskAttr);
                    continue;
                }

                result[propertyName] = ProcessPropertyValue(value);
            }
            catch (Exception ex)
            {
                result[property.Name] = $"Error extracting property: {ex.Message}";
            }
        }

        return result;
    }

    /// <summary>
    /// Processes a value to determine how it should be logged based on its type.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <returns>The processed value suitable for logging.</returns>
    private static object? ProcessPropertyValue(object? value)
    {
        if (value is null)
            return null;

        Type type = value.GetType();

        // Handle simple types that can be logged directly
        if (type.IsPrimitive || SimpleTypes.Contains(type))
        {
            return value;
        }

        // Handle enums
        if (type.IsEnum)
        {
            return value.ToString();
        }

        // Handle collections (except strings, which are IEnumerable but should be treated as simple values)
        if (value is IEnumerable enumerable && value is not string)
        {
            return enumerable
                .Cast<object>()
                .Select(ProcessPropertyValue)
                .Where(x => x is not null)
                .ToList();
        }

        // Handle complex types by recursively extracting their properties
        return ExtractLoggableProperties(value);
    }

    /// <summary>
    /// Masks a value according to the specified masking strategy.
    /// </summary>
    /// <param name="value">The value to mask. Can be null.</param>
    /// <param name="maskAttr">The masking attribute containing the masking strategy. Can be null.</param>
    /// <returns>The masked value, or "***" if the value is null/empty or no masking strategy is specified.</returns>
    public static string MaskValue(string? value, LogMaskAttribute? maskAttr)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        MaskingStrategy strategy = maskAttr?.Strategy ?? MaskingStrategy.Complete;

        return strategy switch
        {
            MaskingStrategy.Complete => "***",

            MaskingStrategy.PartialStart => value.Length <= 4 ? "***" : $"***{value[^4..]}",

            MaskingStrategy.PartialEnd => value.Length <= 4 ? "***" : $"{value[..4]}***",

            MaskingStrategy.EmailDomain => value.Contains('@')
                ? $"***@{value[(value.IndexOf('@') + 1)..]}"
                : "***",

            MaskingStrategy.PhoneCountryCode => value.Contains(' ')
                ? $"{value[..value.IndexOf(' ')]} ***"
                : "+*** ***",

            MaskingStrategy.Custom when !string.IsNullOrEmpty(maskAttr?.CustomMask) =>
                maskAttr.CustomMask.Replace("{value}", value),

            _ => "***",
        };
    }
}
