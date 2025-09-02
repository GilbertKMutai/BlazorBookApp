namespace BlazorBookApp.Client.Tests;

public class ErrorHandlerServiceTests
{
    private readonly Mock<ILogger<ErrorHandlerService>> _loggerMock;
    private readonly ErrorHandlerService _service;

    public ErrorHandlerServiceTests()
    {
        _loggerMock = new Mock<ILogger<ErrorHandlerService>>();
        _service = new ErrorHandlerService(_loggerMock.Object);
    }

    [Fact]
    public void HandleError_WithApplicationException_ReturnsExceptionMessage()
    {
        // Arrange
        var exception = new ApplicationException("Test message");

        // Act
        var result = _service.HandleError(exception);

        // Assert
        Assert.Equal("Test message", result);
    }

    [Fact]
    public void HandleError_WithHttpRequestException_ReturnsNetworkMessage()
    {
        // Arrange
        var exception = new HttpRequestException();

        // Act
        var result = _service.HandleError(exception);

        // Assert
        Assert.Equal("Unable to connect to the service. Please check your internet connection.", result);
    }

    [Fact]
    public void HandleError_WithTaskCanceledException_ReturnsTimeoutMessage()
    {
        // Arrange
        var exception = new TaskCanceledException();

        // Act
        var result = _service.HandleError(exception);

        // Assert
        Assert.Equal("The request timed out. Please try again.", result);
    }

    [Fact]
    public void HandleError_WithGenericException_ReturnsGenericMessage()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = _service.HandleError(exception);

        // Assert
        Assert.Equal("An unexpected error occurred. Please try again.", result);
    }

    [Fact]
    public void HandleError_WithContext_LogsContext()
    {
        // Arrange
        var exception = new Exception();
        var context = "Test context";

        // Act
        _service.HandleError(exception, context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(context)),
                exception,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ),
            Times.Once
        );
    }
}
