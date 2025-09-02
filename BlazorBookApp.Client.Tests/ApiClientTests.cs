namespace BlazorBookApp.Client.Tests;

public class ApiClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<ApiClient>> _loggerMock;
    private readonly ApiClient _apiClient;

    public ApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };
        _loggerMock = new Mock<ILogger<ApiClient>>();
        _apiClient = new ApiClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsync_WithSuccessfulResponse_ReturnsOperationResult()
    {
        // Arrange
        var expectedResult = OperationResult<BookSearchResultDto[]>.Success(
            new[] { new BookSearchResultDto { Title = "Test Book" } }
        );

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                JsonSerializer.Serialize(expectedResult),
                System.Text.Encoding.UTF8,
                "application/json"
            )
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        // Act
        var result = await _apiClient.SearchAsync("test");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Test Book", result.Value[0].Title);
    }

    [Fact]
    public async Task SearchAsync_WithHttpError_ReturnsFailureOperationResult()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Server error")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        // Act
        var result = await _apiClient.SearchAsync("test");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("CLIENT_ERROR", result.ErrorCode);
    }

    [Fact]
    public async Task SearchAsync_WithException_ReturnsFailureOperationResult()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        // Act
        var result = await _apiClient.SearchAsync("test");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("CLIENT_ERROR", result.ErrorCode);
    }

    [Fact]
    public async Task GetDetailsAsync_WithSuccessfulResponse_ReturnsOperationResult()
    {
        // Arrange
        var expectedResult = OperationResult<BookDetailsDto>.Success(
            new BookDetailsDto { Title = "Test Book" }
        );

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                JsonSerializer.Serialize(expectedResult),
                System.Text.Encoding.UTF8,
                "application/json"
            )
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        // Act
        var result = await _apiClient.GetDetailsAsync("test-id");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Test Book", result.Value.Title);
    }
}
