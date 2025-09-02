namespace BlazorBookApp.Client.Tests;

public class RecentSearchServiceTests
{
    private readonly Mock<ILocalStorageService> _localStorageMock;
    private readonly RecentSearchService _service;

    public RecentSearchServiceTests()
    {
        _localStorageMock = new Mock<ILocalStorageService>();
        _service = new RecentSearchService(_localStorageMock.Object);
    }

    [Fact]
    public async Task AddSearchQueryAsync_WithValidQuery_AddsToStorage()
    {
        // Arrange
        var query = "test query";
        var initialHistory = new List<string> { "old query" };

        _localStorageMock
            .Setup(x => x.GetItemAsync<List<string>>("bookSearchHistory"))
            .ReturnsAsync(initialHistory);

        // Act
        await _service.AddSearchQueryAsync(query);

        // Assert
        _localStorageMock.Verify(x => x.SetItemAsync(
            "bookSearchHistory",
            It.Is<List<string>>(list =>
                list.Count == 2 &&
                list[0] == query &&
                list[1] == "old query"
            )
        ), Times.Once);
    }

    [Fact]
    public async Task AddSearchQueryAsync_WithDuplicateQuery_MovesToTop()
    {
        // Arrange
        var query = "test query";
        var initialHistory = new List<string> { "old query", query, "another query" };

        _localStorageMock
            .Setup(x => x.GetItemAsync<List<string>>("bookSearchHistory"))
            .ReturnsAsync(initialHistory);

        // Act
        await _service.AddSearchQueryAsync(query);

        // Assert
        _localStorageMock.Verify(x => x.SetItemAsync(
            "bookSearchHistory",
            It.Is<List<string>>(list =>
                list.Count == 3 &&
                list[0] == query
            )
        ), Times.Once);
    }

    [Fact]
    public async Task AddSearchQueryAsync_WithEmptyQuery_DoesNothing()
    {
        // Arrange
        var query = "";

        // Act
        await _service.AddSearchQueryAsync(query);

        // Assert
        _localStorageMock.Verify(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task GetRecentSearchesAsync_WithNoHistory_ReturnsEmptyList()
    {
        // Arrange
        _localStorageMock
            .Setup(x => x.GetItemAsync<List<string>>("bookSearchHistory"))
            .ReturnsAsync((List<string>)null);

        // Act
        var result = await _service.GetRecentSearchesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecentSearchesAsync_WithHistory_ReturnsHistory()
    {
        // Arrange
        var expectedHistory = new List<string> { "query1", "query2" };

        _localStorageMock
            .Setup(x => x.GetItemAsync<List<string>>("bookSearchHistory"))
            .ReturnsAsync(expectedHistory);

        // Act
        var result = await _service.GetRecentSearchesAsync();

        // Assert
        Assert.Equal(expectedHistory, result);
    }

    [Fact]
    public async Task ClearRecentSearchesAsync_RemovesFromStorage()
    {
        // Act
        await _service.ClearRecentSearchesAsync();

        // Assert
        _localStorageMock.Verify(x => x.RemoveItemAsync("bookSearchHistory"), Times.Once);
    }
}