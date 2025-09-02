namespace BlazorBookApp.Server.Services.Contracts;

/// <summary>
/// Provides operations for searching books and retrieving detailed book information.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Searches for books that match the given title.
    /// </summary>
    /// <param name="title">
    /// The title (or partial title) of the book to search for.
    /// </param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a collection of
    /// <see cref="BookSearchResultDto"/> results if successful,
    /// or an error result if the search fails.
    /// </returns>
    Task<OperationResult<IEnumerable<BookSearchResultDto>>> SearchAsync(string title);

    /// <summary>
    /// Retrieves detailed information about a specific book.
    /// </summary>
    /// <param name="workId">
    /// The unique identifier of the book (as provided by the external service).
    /// </param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a <see cref="BookDetailsDto"/> 
    /// if found, or an error result if the lookup fails.
    /// </returns>
    Task<OperationResult<BookDetailsDto>> GetWorkDetailsAsync(string workId);
}