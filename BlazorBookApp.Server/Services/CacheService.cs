namespace BlazorBookApp.Server.Services;

/// <inheritdoc />
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan expiration)
    {
        try
        {
            if (_cache.TryGetValue(key, out T? cachedValue) && cachedValue is not null)
            {
                return cachedValue;
            }

            T value = await factory();

            if (value != null)
            {
                _cache.Set(key, value, expiration);
            }
            else
            {
                _logger.LogWarning("Factory returned null for key: {Key}.", key);
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching value for key: {Key}", key);

            if (_cache.TryGetValue(key, out T? fallbackValue) && fallbackValue is not null)
            {
                _logger.LogWarning("Returning stale cache for key: {Key}", key);
                return fallbackValue;
            }

            return default;
        }
    }
}
