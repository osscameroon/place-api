using System;
using Microsoft.EntityFrameworkCore;

namespace Core.Database;

/// <summary>
/// Configuration for a DbContext
/// </summary>
public sealed class DbContextConfig
{
    /// <summary>
    /// The type of the DbContext
    /// </summary>
    public required Type ContextType { get; init; }

    /// <summary>
    /// Optional configuration action for the DbContext
    /// </summary>
    public Action<DbContextOptionsBuilder>? ConfigureOptions { get; init; }
}
