namespace BlazorBookApp.Client.BUnitTests;

public class HomeTests : TestContext
{
    private readonly Mock<IApiClient> _apiClientMock;
    private readonly Mock<IModalService> _modalServiceMock;
    private readonly Mock<ILogger<HomeBase>> _loggerMock;
    private readonly Mock<IRecentSearchService> _recentSearchServiceMock;

    public HomeTests()
    {
        _apiClientMock = new Mock<IApiClient>();
        _modalServiceMock = new Mock<IModalService>();
        _loggerMock = new Mock<ILogger<HomeBase>>();
        _recentSearchServiceMock = new Mock<IRecentSearchService>();

        Services.AddSingleton(_apiClientMock.Object);
        Services.AddSingleton(_modalServiceMock.Object);
        Services.AddSingleton(_loggerMock.Object);
        Services.AddSingleton(_recentSearchServiceMock.Object);
    }

    [Fact]
    public void InitialRender_ShowsSearchPlaceholder()
    {
        // Act
        var cut = RenderComponent<Home>();

        // Assert
        Assert.Contains("Search for Books", cut.Markup);
    }

    [Fact]
    public async Task OnSearch_WithResults_DisplaysResults()
    {
        // Arrange
        var results = new[]
        {
            new BookSearchResultDto { Title = "Test Book 1", WorkId = "1" },
            new BookSearchResultDto { Title = "Test Book 2", WorkId = "2" }
        };

        var operationResult = OperationResult<BookSearchResultDto[]>.Success(results);

        _apiClientMock
            .Setup(a => a.SearchAsync("test"))
            .ReturnsAsync(operationResult);

        var cut = RenderComponent<Home>();
        var homeInstance = cut.Instance as HomeBase;

        // Act
        await cut.InvokeAsync(() => homeInstance.OnSearch("test"));
        cut.Render();

        // Assert
        Assert.False(homeInstance.ShowError);
        Assert.Empty(homeInstance.ErrorMessage);
        Assert.NotNull(homeInstance.Results);
        Assert.Equal(2, homeInstance.Results.Length);
    }

    [Fact]
    public async Task OnSearch_WithError_ShowsErrorMessage()
    {
        // Arrange
        var operationResult = OperationResult<BookSearchResultDto[]>.Failure(
            "Search failed", 500, "SEARCH_ERROR"
        );

        _apiClientMock
            .Setup(a => a.SearchAsync("test"))
            .ReturnsAsync(operationResult);

        var cut = RenderComponent<Home>();
        var homeInstance = cut.Instance as HomeBase;

        // Act
        await cut.InvokeAsync(() => homeInstance.OnSearch("test"));
        cut.Render();

        // Assert
        Assert.True(homeInstance.ShowError);
        Assert.NotEmpty(homeInstance.ErrorMessage);
        Assert.NotNull(homeInstance.Results);
        Assert.Empty(homeInstance.Results);
    }

    [Fact]
    public async Task ShowDetails_WithValidBook_CallsModalService()
    {
        // Arrange
        var book = new BookSearchResultDto { WorkId = "test-id", Title = "Test Book" };
        var details = new BookDetailsDto { Title = "Test Book Details" };

        var operationResult = OperationResult<BookDetailsDto>.Success(details);

        _apiClientMock
            .Setup(a => a.GetDetailsAsync("test-id"))
            .ReturnsAsync(operationResult);

        var cut = RenderComponent<Home>();
        var homeInstance = cut.Instance as HomeBase;

        // Act
        await homeInstance.ShowDetails(book);

        // Assert
        _modalServiceMock.Verify(m => m.ShowDetails(details), Times.Once);
        Assert.False(homeInstance.ShowError);
        Assert.Empty(homeInstance.ErrorMessage);
    }

    [Fact]
    public async Task ShowDetails_WithNullWorkId_DoesNothing()
    {
        // Arrange
        var book = new BookSearchResultDto { WorkId = null, Title = "Test Book" };

        var cut = RenderComponent<Home>();
        var homeInstance = cut.Instance as HomeBase;

        // Act
        await cut.InvokeAsync(() => homeInstance.ShowDetails(book));

        // Assert
        _apiClientMock.Verify(a => a.GetDetailsAsync(It.IsAny<string>()), Times.Never);
        _modalServiceMock.Verify(m => m.ShowDetails(It.IsAny<BookDetailsDto>()), Times.Never);
    }

    [Fact]
    public void GetUserFriendlyErrorMessage_WithKnownErrorCode_ReturnsFriendlyMessage()
    {
        // Arrange
        var cut = RenderComponent<Home>();
        var homeInstance = cut.Instance as HomeBase;

        // Act
        var result = homeInstance.GetUserFriendlyErrorMessage("Error", 404, "NOT_FOUND");

        // Assert
        Assert.Equal("The requested book was not found", result);
    }

    [Fact]
    public void GetUserFriendlyErrorMessage_WithUnknownErrorCode_ReturnsGenericMessage()
    {
        // Arrange
        var cut = RenderComponent<Home>();
        var homeInstance = cut.Instance as HomeBase;

        // Act
        var result = homeInstance.GetUserFriendlyErrorMessage("Error", 418, "UNKNOWN_ERROR");

        // Assert
        Assert.Equal("An error occurred. Please try again", result);
    }
}