namespace BlazorBookApp.Server.Tests;

public class BookServiceTests
{
    private readonly Mock<ILogger<BookService>> _loggerMock;
    private readonly Mock<ICacheService> _cacheMock;

    public BookServiceTests()
    {
        _loggerMock = new Mock<ILogger<BookService>>();
        _cacheMock = new Mock<ICacheService>();
    }

    private BookService CreateService(HttpResponseMessage response, ExternalApiOptions options = null)
    {
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler);

        var opts = Options.Create(options ?? new ExternalApiOptions
        {
            BaseUrl = "https://fake-api/",
            RetryCount = 1,
            CacheDurationMinutes = 5,
            DetailsCacheDurationHours = 1,
            MaxResults = 10
        });

        return new BookService(client, _loggerMock.Object, opts, _cacheMock.Object);
    }

    [Fact]
    public async Task SearchAsync_ReturnsSuccess_WhenApiRespondsOk()
    {
        // Arrange
        var apiResponse = new
        {
            items = new[]
            {
                new
                {
                    id = "123",
                    volumeInfo = new
                    {
                        title = "Test Book",
                        authors = new[] { "Author One" },
                        publishedDate = "2020-01-01"
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        _cacheMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<OperationResult<IEnumerable<BookSearchResultDto>>>>>(), It.IsAny<TimeSpan>()))
                  .Returns<string, Func<Task<OperationResult<IEnumerable<BookSearchResultDto>>>>, TimeSpan>((_, factory, __) => factory());

        var service = CreateService(response);

        // Act
        var result = await service.SearchAsync("Test");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Test Book", result.Value.First().Title);
    }

    [Fact]
    public async Task SearchAsync_ReturnsFailure_WhenApiReturns500()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server error")
        };

        _cacheMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<OperationResult<IEnumerable<BookSearchResultDto>>>>>(), It.IsAny<TimeSpan>()))
                  .Returns<string, Func<Task<OperationResult<IEnumerable<BookSearchResultDto>>>>, TimeSpan>((_, factory, __) => factory());

        var service = CreateService(response);

        // Act
        var result = await service.SearchAsync("Test");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("EXTERNAL_API_ERROR", result.ErrorCode);
    }

    [Fact]
    public async Task GetWorkDetailsAsync_ReturnsFailure_WhenWorkIdIsEmpty()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        var service = CreateService(response);

        // Act
        var result = await service.GetWorkDetailsAsync("");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("WORK_ID_REQUIRED", result.ErrorCode);
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}