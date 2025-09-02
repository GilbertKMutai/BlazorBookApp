namespace BlazorBookApp.Server.Services.Contracts;

/// <summary>
/// Service for managing recent searches using local storage
/// </summary>
public interface IRecentSearchService
{
    /// <summary>
    /// Adds a query to the search history
    /// </summary>
    /// <param name="query">The search query to add</param>
    Task AddSearchQueryAsync(string query);

    /// <summary>
    /// Gets the search history
    /// </summary>
    /// <returns>A list of search queries</returns>
    Task<List<string>> GetRecentSearchesAsync();

    /// <summary>
    /// Clears the search history
    /// </summary>
    Task ClearRecentSearchesAsync();
}