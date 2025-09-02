namespace BlazorBookApp.Client.Services;


/// <inheritdoc cref="IApiClient"/>
public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiClient> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="ApiClient"/>.
    /// </summary>
    /// <param name="http">The HTTP client for API requests.</param>
    /// <param name="logger">The logger instance.</param>
    public ApiClient(HttpClient http, ILogger<ApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<OperationResult<BookSearchResultDto[]>> SearchAsync(string title)
    {
        try
        {
            var response = await _http.GetAsync($"api/books/search?title={Uri.EscapeDataString(title)}");

            var result = await response.Content.ReadFromJsonAsync<OperationResult<BookSearchResultDto[]>>();

            if (result != null)
            {
                return result;
            }

            return OperationResult<BookSearchResultDto[]>.Failure(
                "Invalid response from server", 500, "INVALID_RESPONSE");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during search for '{Title}'", title);
            return OperationResult<BookSearchResultDto[]>.Failure(
                "An error occurred while searching", 500, "CLIENT_ERROR");
        }
    }

    /// <inheritdoc/>
    public async Task<OperationResult<BookDetailsDto>> GetDetailsAsync(string workId)
    {
        try
        {
            var response = await _http.GetAsync($"api/books/{Uri.EscapeDataString(workId)}");

            var result = await response.Content.ReadFromJsonAsync<OperationResult<BookDetailsDto>>();

            if (result != null)
            {
                return result;
            }

            return OperationResult<BookDetailsDto>.Failure(
                "Invalid response from server", 500, "INVALID_RESPONSE");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching details for work ID '{WorkId}'", workId);
            return OperationResult<BookDetailsDto>.Failure(
                "An error occurred while fetching details", 500, "CLIENT_ERROR");
        }
    }
}