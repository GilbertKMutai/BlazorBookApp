namespace BlazorBookApp.Server.Tests;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _bookServiceMock = new();
    private readonly Mock<ILogger<BooksController>> _loggerMock = new();
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _controller = new BooksController(_bookServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Search_ReturnsSuccess_WhenServiceReturnsSuccess()
    {
        // Arrange
        var dto = new BookSearchResultDto { Title = "Test Book" };
        var result = OperationResult<IEnumerable<BookSearchResultDto>>.Success(new[] { dto });

        _bookServiceMock
            .Setup(s => s.SearchAsync("test"))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Search("test");

        // Assert
        var ok = Assert.IsType<OperationResult<IEnumerable<BookSearchResultDto>>>(response.Value);
        Assert.True(ok.IsSuccess);
        Assert.Single(ok.Value);
    }

    [Fact]
    public async Task Search_LogsError_WhenServiceFails()
    {
        // Arrange
        var result = OperationResult<IEnumerable<BookSearchResultDto>>.Failure("Error", 500, "ERR");
        _bookServiceMock
            .Setup(s => s.SearchAsync("bad"))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Search("bad");

        // Assert
        var fail = Assert.IsType<OperationResult<IEnumerable<BookSearchResultDto>>>(response.Value);
        Assert.False(fail.IsSuccess);
        _bookServiceMock.Verify(s => s.SearchAsync("bad"), Times.Once);
        _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task GetDetails_ReturnsFailure_WhenNotFound()
    {
        // Arrange
        var result = OperationResult<BookDetailsDto>.Failure("Not found", 404, "NOT_FOUND");
        _bookServiceMock
            .Setup(s => s.GetWorkDetailsAsync("123"))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetDetails("123");

        // Assert
        var fail = Assert.IsType<OperationResult<BookDetailsDto>>(response.Value);
        Assert.False(fail.IsSuccess);
        Assert.Equal(404, fail.StatusCode);
    }
}