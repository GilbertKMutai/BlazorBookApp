using Microsoft.AspNetCore.Components.Web;

namespace BlazorBookApp.Client.BUnitTests;

public class SearchBarTests : TestContext
{
    private readonly Mock<IRecentSearchService> _recentSearchServiceMock;
    private readonly Mock<ILogger<SearchBarBase>> _loggerMock;

    public SearchBarTests()
    {
        _recentSearchServiceMock = new Mock<IRecentSearchService>();
        _loggerMock = new Mock<ILogger<SearchBarBase>>();

        Services.AddSingleton(_recentSearchServiceMock.Object);
        Services.AddSingleton(_loggerMock.Object);
    }

    [Fact]
    public void InitialRender_ShowsSearchInput()
    {
        // Act
        var cut = RenderComponent<SearchBar>();

        // Assert
        Assert.NotNull(cut.Find("input[type='text']"));
        Assert.NotNull(cut.Find("button"));
    }

    [Fact]
    public void EnteringQueryAndClickingButton_TriggersSearch()
    {
        // Arrange
        var searchInvoked = false;
        var cut = RenderComponent<SearchBar>(
            parameters => parameters.Add(p => p.OnSearch, (query) => { searchInvoked = true; })
        );

        // Act
        cut.Find("input").Input("test query");
        cut.Find("button").Click();

        // Assert
        Assert.True(searchInvoked);
    }

    [Fact]
    public async Task DoSearch_WithEmptyQuery_DoesNotTriggerSearch()
    {
        // Arrange
        var searchInvoked = false;
        var cut = RenderComponent<SearchBar>(
            parameters => parameters.Add(p => p.OnSearch, (query) => { searchInvoked = true; })
        );

        // Act
        await cut.InvokeAsync(() => cut.Instance.DoSearch());

        // Assert
        Assert.False(searchInvoked);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Attempted to search with empty query")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task ShowSuggestions_LoadsRecentSearches()
    {
        // Arrange
        var recentSearches = new List<string> { "search1", "search2" };

        _recentSearchServiceMock
            .Setup(x => x.GetRecentSearchesAsync())
            .ReturnsAsync(recentSearches);

        var cut = RenderComponent<SearchBar>();
        var searchBarInstance = cut.Instance as SearchBarBase;

        // Act

        await cut.InvokeAsync(() => 
            searchBarInstance.HandleKeyDown(new KeyboardEventArgs { Key = "ArrowDown" })
        );

        // Assert
        Assert.True(searchBarInstance.ShouldShowSuggestions);
        Assert.Equal(recentSearches, searchBarInstance.RecentSearches);
    }

    [Fact]
    public async Task HandleKeyDown_WithEnterKey_TriggersSearch()
    {
        // Arrange
        var searchInvoked = false;
        var cut = RenderComponent<SearchBar>(
            parameters => parameters.Add(p => p.OnSearch, (query) => { searchInvoked = true; })
        );
        cut.Instance.Query = "test query";

        // Act
        await cut.Instance.HandleKeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs
        {
            Key = "Enter"
        });

        // Assert
        Assert.True(searchInvoked);
    }

    [Fact]
    public async Task HandleKeyDown_WithEscapeKey_HidesSuggestions()
    {
        // Arrange
        var cut = RenderComponent<SearchBar>();
        var searchBarInstance = cut.Instance as SearchBarBase;
        searchBarInstance.ShouldShowSuggestions = true;

        // Act
        await searchBarInstance.HandleKeyDown(new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs
        {
            Key = "Escape"
        });

        // Assert
        Assert.False(searchBarInstance.ShouldShowSuggestions);
    }

    [Fact]
    public async Task HandleKeyDown_WithArrowDown_ShowsAndNavigatesSuggestions()
    {
        // Arrange
        var recentSearches = new List<string> { "search1", "search2" };

        _recentSearchServiceMock
            .Setup(x => x.GetRecentSearchesAsync())
            .ReturnsAsync(recentSearches);

        var cut = RenderComponent<SearchBar>();
        var searchBarInstance = cut.Instance;

        // Act
        await cut.InvokeAsync(() =>
            searchBarInstance.HandleKeyDown(new KeyboardEventArgs { Key = "ArrowDown" })
        );

        // Assert
        Assert.True(searchBarInstance.ShouldShowSuggestions);
        Assert.True(searchBarInstance.IsSuggestionsFocused);
        Assert.Equal(0, searchBarInstance.SelectedSuggestionIndex);
    }

    [Fact]
    public async Task SelectSuggestion_SetsQueryAndTriggersSearch()
    {
        // Arrange
        var searchInvoked = false;
        var cut = RenderComponent<SearchBar>(
            parameters => parameters.Add(p => p.OnSearch, (query) => { searchInvoked = true; })
        );
        cut.Instance.ShouldShowSuggestions = true;

        // Act
        await cut.Instance.SelectSuggestion("test suggestion");

        // Assert
        Assert.Equal("test suggestion", cut.Instance.Query);
        Assert.False(cut.Instance.ShouldShowSuggestions);
        Assert.True(searchInvoked);
    }
}