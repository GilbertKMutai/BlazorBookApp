namespace BlazorBookApp.Server.Configuration;

/// <summary>
/// Configuration options for the external API.
/// </summary>
public sealed class ExternalApiOptions
{
    /// <summary>
    /// Base URL of the API.
    /// </summary>
    public string BaseUrl { get; set; } = default!;

    /// <summary>
    /// Maximum number of results to return in a search.
    /// </summary>
    public int MaxResults { get; set; } = 20;

    /// <summary>
    /// Number of retries for failed requests.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Cache duration (in minutes) for search results.
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Cache duration (in hours) for book details.
    /// </summary>
    public int DetailsCacheDurationHours { get; set; } = 24;
}
