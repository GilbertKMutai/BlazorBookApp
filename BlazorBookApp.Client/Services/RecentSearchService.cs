namespace BlazorBookApp.Server.Services;

/// <summary>
/// Implementation of IRecentSearchService using local storage
/// </summary>
public class RecentSearchService : IRecentSearchService
{

    private readonly ILocalStorageService _localStorage;
    private const string StorageKey = "bookSearchHistory";
    private const int MaxHistoryItems = 5;

    public RecentSearchService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }


    /// <inheritdoc />
    public async Task AddSearchQueryAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        var history = await GetRecentSearchesAsync();

        history.RemoveAll(q => q.Equals(query, StringComparison.OrdinalIgnoreCase));

        history.Insert(0, query.Trim());

        if (history.Count > MaxHistoryItems)
            history = history.GetRange(0, MaxHistoryItems);

        await _localStorage.SetItemAsync(StorageKey, history);
    }


    /// <inheritdoc />
    public async Task<List<string>> GetRecentSearchesAsync()
    {
        var history = await _localStorage.GetItemAsync<List<string>>(StorageKey);
        return history ?? new List<string>();
    }


    /// <inheritdoc />
    public async Task ClearRecentSearchesAsync()
    {
        await _localStorage.RemoveItemAsync(StorageKey);
    }
}