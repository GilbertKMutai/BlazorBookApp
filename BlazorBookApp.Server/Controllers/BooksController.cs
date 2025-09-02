namespace BlazorBookApp.Server.Controllers;

/// <summary>
/// API controller for book search and details retrieval.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="BooksController"/>.
    /// </summary>
    /// <param name="bookService">Service for book operations.</param>
    /// <param name="logger">Logger instance.</param>
    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Searches for books by title.
    /// </summary>
    /// <param name="title">The title (or partial title) of the book to search for.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> with a list of <see cref="BookSearchResultDto"/>
    /// if successful, otherwise an error result.
    /// </returns>
    [HttpGet("search")]
    public async Task<ActionResult<OperationResult<IEnumerable<BookSearchResultDto>>>> Search([FromQuery] string title)
    {
        var result = await _bookService.SearchAsync(title);

        if (!result.IsSuccess)
        {
            _logger.LogError("Search failed: {ErrorCode} - {ErrorMessage}", result.ErrorCode, result.Error);
        }

        return result;
    }

    /// <summary>
    /// Gets detailed information about a specific book.
    /// </summary>
    /// <param name="workId">The unique identifier of the book.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> with <see cref="BookDetailsDto"/> if successful,
    /// otherwise an error result.
    /// </returns>
    [HttpGet("{workId}")]
    public async Task<ActionResult<OperationResult<BookDetailsDto>>> GetDetails(string workId)
    {
        var result = await _bookService.GetWorkDetailsAsync(workId);

        if (!result.IsSuccess)
        {
            _logger.LogError("GetDetails failed: {ErrorCode} - {ErrorMessage}", result.ErrorCode, result.Error);
        }

        return result;
    }
}