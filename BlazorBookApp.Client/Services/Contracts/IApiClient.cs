namespace BlazorBookApp.Client.Services.Contracts;

/// <summary>
/// Contract for client-side API calls to the server.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Searches for books by title.
    /// </summary>
    /// <param name="title">The title (or partial title) of the book.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing an array of <see cref="BookSearchResultDto"/>
    /// if successful, otherwise an error result.
    /// </returns>
    Task<OperationResult<BookSearchResultDto[]>> SearchAsync(string title);

    /// <summary>
    /// Gets detailed information for a specific book.
    /// </summary>
    /// <param name="workId">The unique identifier of the book.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing <see cref="BookDetailsDto"/>
    /// if successful, otherwise an error result.
    /// </returns>
    Task<OperationResult<BookDetailsDto>> GetDetailsAsync(string workId);
}