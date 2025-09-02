[assembly: InternalsVisibleTo("BlazorBookApp.Client.BUnitTests")]
namespace BlazorBookApp.Client.Components;

/// <summary>
/// Base class for the search bar component, providing support for 
/// entering queries, managing recent searches, and triggering 
/// search actions.
/// </summary>
public class SearchBarBase : ComponentBase, IDisposable
{
    private CancellationTokenSource _cts = new();
    internal bool ShouldShowSuggestions = false;
    protected ElementReference searchInput;
    private DotNetObjectReference<SearchBarBase>? dotNetHelper;
    internal string Query { get; set; } = string.Empty;
    internal int SelectedSuggestionIndex { get; set; } = -1;
    internal bool IsSuggestionsFocused { get; set; }

    /// <summary>
    /// Callback event that is invoked when a search is performed.
    /// The search query string is passed as the argument.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnSearch { get; set; }

    /// <summary>
    /// A list of recent search queries. Typically displayed as 
    /// suggestions under the search bar.
    /// </summary>
    [Parameter]
    public List<string> RecentSearches { get; set; } = new();
    [Inject]
    private IRecentSearchService RecentSearchService { get; set; } = default!;
    [Inject]
    private ILogger<SearchBarBase> Logger { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _cts = new CancellationTokenSource();
                dotNetHelper = DotNetObjectReference.Create(this);
                await LoadSearchHistory();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to initialize SearchBar component");
                StateHasChanged();
            }
        }
    }

    private async Task LoadSearchHistory()
    {
        try
        {
            RecentSearches = await RecentSearchService.GetRecentSearchesAsync();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An unexpected error has occured in LoadSearchHistory method under SearchBarBase class while trying to get recent searches from local storage.");
            RecentSearches = new List<string>();
        }
    }

    internal async Task ShowSuggestions()
    {
        try
        {
            await LoadSearchHistory();

            if (RecentSearches?.Count > 0)
            {
                ShouldShowSuggestions = true;
                SelectedSuggestionIndex = -1;
                IsSuggestionsFocused = false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show search suggestions");
        }
    }

    public void HideSuggestions()
    {
        ShouldShowSuggestions = false;
        SelectedSuggestionIndex = -1;
        IsSuggestionsFocused = false;
    }

    internal async Task DoSearch()
    {
        if (string.IsNullOrWhiteSpace(Query))
        {
            Logger.LogWarning("Attempted to search with empty query");
            return;
        }

        try
        {
            ShouldShowSuggestions = false;
            SelectedSuggestionIndex = -1;
            IsSuggestionsFocused = false;

            await RecentSearchService.AddSearchQueryAsync(Query);

            using var searchCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            searchCts.CancelAfter(TimeSpan.FromSeconds(30));

            await OnSearch.InvokeAsync(Query.Trim());

            await LoadSearchHistory();
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("Search operation was cancelled");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to execute search for query: {Query}", Query);
        }
    }

    internal async Task SelectSuggestion(string suggestion)
    {
        try
        {
            Query = suggestion;
            ShouldShowSuggestions = false;
            SelectedSuggestionIndex = -1;
            IsSuggestionsFocused = false;
            await DoSearch();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to select search suggestion: {Suggestion}", suggestion);
            StateHasChanged();
        }
    }

    protected async Task ClearRecentSearches()
    {
        ShouldShowSuggestions = false;
        SelectedSuggestionIndex = -1;
        IsSuggestionsFocused = false;

        try
        {
            await RecentSearchService.ClearRecentSearchesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clear recent searches");
        }
    }

    internal async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await HandleEnterKey();
        }
        else if (e.Key == "Escape")
        {
            ShouldShowSuggestions = false;
            SelectedSuggestionIndex = -1;
            IsSuggestionsFocused = false;
        }
        else if (e.Key == "ArrowDown")
        {
            await HandleArrowDown();
        }
        else if (e.Key == "ArrowUp")
        {
            HandleArrowUp();
        }
        else {
            HideSuggestions();
        }
    }

    private async Task HandleEnterKey()
    {
        if (IsSuggestionsFocused && SelectedSuggestionIndex >= 0 &&
            SelectedSuggestionIndex < RecentSearches.Count)
        {
            await SelectSuggestion(RecentSearches[SelectedSuggestionIndex]);
        }
        else
        {
            await DoSearch();
        }
    }

    private async Task HandleArrowDown()
    {
        if (RecentSearches?.Count > 0)
        {
            if (!ShouldShowSuggestions)
            {
                await ShowSuggestions();
            }

            SelectedSuggestionIndex = Math.Min(SelectedSuggestionIndex + 1, RecentSearches.Count - 1);
            IsSuggestionsFocused = true;
        }
    }

    private void HandleArrowUp()
    {
        if (RecentSearches?.Count > 0 && ShouldShowSuggestions)
        {
            SelectedSuggestionIndex = Math.Max(SelectedSuggestionIndex - 1, 0);

            if (SelectedSuggestionIndex == -1)
            {
                IsSuggestionsFocused = false;
            }
        }
    }

    protected async Task HandleRecentSearchKeydown(KeyboardEventArgs e, string search)
    {
        if (e.Key == "Enter")
        {
            await SelectSuggestion(search);
        }
    }

    protected string GetSuggestionClass(int index)
    {
        return index == SelectedSuggestionIndex && IsSuggestionsFocused ? "suggestion-active" : "";
    }

    protected void HandleInputFocus()
    {
        IsSuggestionsFocused = false;
        SelectedSuggestionIndex = -1;
    }

    /// <summary>
    /// Releases resources used by the component, such as the 
    /// <see cref="DotNetObjectReference{T}"/> for JS interop.
    /// </summary>
    public void Dispose()
    {
        dotNetHelper?.Dispose();
    }
}