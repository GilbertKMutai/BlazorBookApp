namespace BlazorBookApp.Server.Services;


/// <inheritdoc cref="IBookService"/>
public sealed class BookService : IBookService
{
    private readonly HttpClient _http;
    private readonly ILogger<BookService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ExternalApiOptions _options;
    private readonly ICacheService _cacheService;

    public BookService(
        HttpClient http,
        ILogger<BookService> logger,
        IOptions<ExternalApiOptions> options,
        ICacheService cacheService)
    {
        _http = http;
        _logger = logger;
        _options = options.Value;
        _cacheService = cacheService;

        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                r.StatusCode == HttpStatusCode.TooManyRequests ||
                r.StatusCode >= HttpStatusCode.InternalServerError)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                _options.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryAttempt} after {TotalSeconds}s due to: {Exception}",
                        retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                });
    }

    /// <inheritdoc/>
    public async Task<OperationResult<IEnumerable<BookSearchResultDto>>> SearchAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return OperationResult<IEnumerable<BookSearchResultDto>>.Failure("Title is required", 400, "TITLE_REQUIRED");

        var cacheKey = $"book_search_{title.ToLowerInvariant()}";

        var cachedResults = await _cacheService.GetOrCreateAsync(cacheKey,
            async () => await FetchSearchResultsFromApi(title),
            TimeSpan.FromMinutes(_options.CacheDurationMinutes));

        return cachedResults ??
              OperationResult<IEnumerable<BookSearchResultDto>>.Success(Enumerable.Empty<BookSearchResultDto>());
    }

    private async Task<OperationResult<IEnumerable<BookSearchResultDto>>> FetchSearchResultsFromApi(string title)
    {
        try
        {
            var url = $"{_options.BaseUrl}volumes?q=intitle:{Uri.EscapeDataString(title)}&maxResults={_options.MaxResults}";

            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead));

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await GetErrorMessage(response);
                _logger.LogError("External API error during search: {StatusCode} - {ErrorMessage}",
                    response.StatusCode, errorMessage);

                return OperationResult<IEnumerable<BookSearchResultDto>>.Failure(
                    "Failed to fetch results from external service",
                    (int)response.StatusCode,
                    "EXTERNAL_API_ERROR");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            var items = doc.RootElement.GetPropertyOrNull("items");
            if (items == null || items.Value.ValueKind != JsonValueKind.Array)
            {
                _logger.LogWarning("No items found in response for: {Title}", title);
                return OperationResult<IEnumerable<BookSearchResultDto>>.Success(Enumerable.Empty<BookSearchResultDto>());
            }

            var list = new List<BookSearchResultDto>();
            foreach (var el in items.Value.EnumerateArray())
            {
                try
                {
                    var volumeInfo = el.GetPropertyOrNull("volumeInfo");
                    if (volumeInfo == null)
                    {
                        continue;
                    }

                    var titleStr = volumeInfo.Value.GetPropertyOrNull("title")?.GetString();
                    if (string.IsNullOrEmpty(titleStr))
                    {
                        continue;
                    }

                    var authorElement = volumeInfo.Value.GetPropertyOrNull("authors");
                    var authors = authorElement?.ValueKind == JsonValueKind.Array
                        ? authorElement.Value.EnumerateArray()
                                             .Select(x => x.GetString() ?? string.Empty)
                                             .Where(s => !string.IsNullOrEmpty(s))
                                             .ToArray()
                        : Array.Empty<string>();

                    int? firstYear = null;
                    var dateStr = volumeInfo.Value.GetPropertyOrNull("publishedDate")?.GetString();
                    if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var publishDate))
                    {
                        firstYear = publishDate.Year;
                    }

                    string? coverUrl = volumeInfo.Value
                                                 .GetPropertyOrNull("imageLinks")
                                                 ?.GetPropertyOrNull("thumbnail")
                                                 ?.GetString();
                    if (!string.IsNullOrEmpty(coverUrl))
                    {
                        coverUrl = NormalizeCoverUrl(coverUrl);
                    }

                    var workId = el.GetPropertyOrNull("id")?.GetString();

                    double? averageRating = volumeInfo.Value.GetPropertyOrNull("averageRating")?.GetDouble();
                    int? ratingsCount = volumeInfo.Value.GetPropertyOrNull("ratingsCount")?.GetInt32();

                    list.Add(new BookSearchResultDto
                    {
                        Title = titleStr,
                        Authors = authors,
                        FirstPublishYear = firstYear,
                        CoverUrl = coverUrl,
                        WorkId = workId,
                        AverageRating = averageRating,
                        RatingsCount = ratingsCount
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process book item in search results");
                }
            }

            return OperationResult<IEnumerable<BookSearchResultDto>>.Success(list);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for book search: {Title}", title);
            return OperationResult<IEnumerable<BookSearchResultDto>>.Failure(
                "Network error occurred while searching", 503, "NETWORK_ERROR");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response for book search: {Title}", title);
            return OperationResult<IEnumerable<BookSearchResultDto>>.Failure(
                "Invalid response from book service", 502, "INVALID_RESPONSE");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during book search: {Title}", title);
            return OperationResult<IEnumerable<BookSearchResultDto>>.Failure(
                "Unexpected error during search", 500, "UNEXPECTED_ERROR");
        }
    }

    /// <inheritdoc/>
    public async Task<OperationResult<BookDetailsDto>> GetWorkDetailsAsync(string workId)
    {
        if (string.IsNullOrWhiteSpace(workId))
        {
            _logger.LogWarning("Empty work ID provided");
            return OperationResult<BookDetailsDto>.Failure("Work ID is required", 400, "WORK_ID_REQUIRED");
        }

        var cacheKey = $"book_details_{workId}";

        var result = await _cacheService.GetOrCreateAsync(cacheKey,
            async () => await FetchWorkDetailsFromApi(workId),
            TimeSpan.FromHours(_options.DetailsCacheDurationHours));

        return result ?? OperationResult<BookDetailsDto>.Failure("Book details not found", 404, "NOT_FOUND");
    }

    private async Task<OperationResult<BookDetailsDto>> FetchWorkDetailsFromApi(string workId)
    {
        try
        {
            var url = $"{_options.BaseUrl}volumes/{Uri.EscapeDataString(workId)}";

            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead));

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Book not found for work ID: {WorkId}", workId);
                return OperationResult<BookDetailsDto>.Failure("Book not found", 404, "NOT_FOUND");
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await GetErrorMessage(response);
                _logger.LogError("External API error fetching details: {StatusCode} - {ErrorMessage}",
                    response.StatusCode, errorMessage);

                return OperationResult<BookDetailsDto>.Failure(
                    "Failed to fetch details from external service",
                    (int)response.StatusCode,
                    "EXTERNAL_API_ERROR");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            var volumeInfo = root.GetPropertyOrNull("volumeInfo");
            if (volumeInfo == null)
            {
                _logger.LogWarning("No volumeInfo in response for work ID: {WorkId}", workId);
                return OperationResult<BookDetailsDto>.Failure("Invalid response format", 502, "INVALID_RESPONSE");
            }

            var title = volumeInfo.Value.GetPropertyOrNull("title")?.GetString();
            if (string.IsNullOrEmpty(title))
            {
                _logger.LogWarning("No title in response for work ID: {WorkId}", workId);
                return OperationResult<BookDetailsDto>.Failure("Invalid response format", 502, "INVALID_RESPONSE");
            }

            var description = volumeInfo.Value.GetPropertyOrNull("description")?.GetString();

            var subjectsElement = volumeInfo.Value.GetPropertyOrNull("categories");
            var subjects = subjectsElement?.ValueKind == JsonValueKind.Array
                ? subjectsElement.Value.EnumerateArray()
                                       .Select(x => x.GetString() ?? string.Empty)
                                       .Where(s => !string.IsNullOrEmpty(s))
                                       .ToArray()
                : Array.Empty<string>();

            var coverUrls = new List<string>();
            var imagesElement = volumeInfo.Value.GetPropertyOrNull("imageLinks");
            if (imagesElement != null)
            {
                foreach (var size in new[] { "thumbnail", "small", "medium", "large" })
                {
                    var urlStr = imagesElement.Value.GetPropertyOrNull(size)?.GetString();
                    if (!string.IsNullOrEmpty(urlStr))
                    {
                        coverUrls.Add(NormalizeCoverUrl(urlStr));
                    }
                }
            }

            double? averageRating = volumeInfo.Value.GetPropertyOrNull("averageRating")?.GetDouble();
            int? ratingsCount = volumeInfo.Value.GetPropertyOrNull("ratingsCount")?.GetInt32();

            return OperationResult<BookDetailsDto>.Success(new BookDetailsDto
            {
                WorkId = workId,
                Title = title,
                Description = description,
                Subjects = subjects,
                CoverUrls = coverUrls.ToArray(),
                AverageRating = averageRating,
                RatingsCount = ratingsCount
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for work details: {WorkId}", workId);
            return OperationResult<BookDetailsDto>.Failure(
                "Network error occurred while fetching details", 503, "NETWORK_ERROR");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response for work details: {WorkId}", workId);
            return OperationResult<BookDetailsDto>.Failure(
                "Invalid response from book service", 502, "INVALID_RESPONSE");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching work details: {WorkId}", workId);
            return OperationResult<BookDetailsDto>.Failure(
                "Unexpected error fetching details", 500, "UNEXPECTED_ERROR");
        }
    }

    private string NormalizeCoverUrl(string url)
    {
        return url
            .Replace("http://", "https://")
            .Replace("&edge=curl", "")
            .Replace("&zoom=1", "");
    }

    private async Task<string> GetErrorMessage(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadAsStringAsync() ?? response.ReasonPhrase ?? "An error occurred";
        }
        catch
        {
            return response.ReasonPhrase ?? "An error occurred";
        }
    }
}