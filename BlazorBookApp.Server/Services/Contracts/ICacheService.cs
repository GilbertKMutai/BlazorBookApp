namespace BlazorBookApp.Server.Services.Contracts;

/// <summary>
/// Defines a simple cache service.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value or creates it using the factory.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="key">Cache key.</param>
    /// <param name="factory">Factory function if not cached.</param>
    /// <param name="expiration">Cache expiration.</param>
    /// <returns>The cached or created value.</returns>
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);
}

