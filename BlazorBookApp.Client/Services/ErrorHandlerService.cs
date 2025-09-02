namespace BlazorBookApp.Client.Services;

/// <summary>
/// Default implementation of <see cref="IErrorHandlerService"/> that logs errors
/// and generates user-friendly error messages.
/// </summary>
public class ErrorHandlerService : IErrorHandlerService
{
    private readonly ILogger<ErrorHandlerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlerService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used to log errors.</param>
    public ErrorHandlerService(ILogger<ErrorHandlerService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string HandleError(Exception ex)
    {
        return HandleError(ex, "An unexpected error occurred");
    }

    /// <inheritdoc />
    public string HandleError(Exception ex, string context)
    {
        _logger.LogError(ex, "Error: {Context}", context);

        return ex switch
        {
            ApplicationException appEx => appEx.Message,
            HttpRequestException => "Unable to connect to the service. Please check your internet connection.",
            TaskCanceledException => "The request timed out. Please try again.",
            TimeoutException => "The operation timed out. Please try again.",
            _ => "An unexpected error occurred. Please try again."
        };
    }
}
